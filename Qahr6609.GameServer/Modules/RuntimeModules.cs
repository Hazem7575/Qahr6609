// Runtime modules: a module is a folder ServerDataBase\Modules\<Name>\ with its C# files (anywhere inside it,
// except data\, content\, logs\ and .build\) and its data. The server compiles the files with Roslyn when it starts, so a
// module is shared and installed by copying its folder - no build of the server.
//
// - Compiled as an assembly named "QahrModule" (Bootstrap\Properties\AssemblyInfo.cs lets it use internal server
//   types, like the modules compiled into the server) in its own collectible AssemblyLoadContext.
// - Cache: ServerDataBase\Modules\.modcache\<name>\module.dll + module.key (outside the module folder, so a copied module can not
//   bring its own DLL). The key hashes the server build and every source file (+ the DLL itself), so an
//   unchanged module is loaded from the cache without Roslyn (fast boot); any edit or a new server build recompiles.
// - Compile errors are printed (file, line, message) and that module is skipped; the server starts without it.
// - A folder name starting with '_' is ignored (a switched-off module).
// - LIVE modules (development): a module of the source tree (Qahr6609.GameServer\Modules\<Name>\ or Modules\Custom\<Name>\)
//   that has an empty ".live" file is left out of the server build and compiled here instead, from the source folder,
//   so ModuleWatcher reloads it whenever a .cs file is saved. LiveRoots = shell.ini [Modules] LiveSource, else the
//   source tree found above the server exe. The module's data stays in ServerDataBase\Modules\<Name>\ (DataPath).
// What the assembly contains is registered by ModuleManager like the server's own code: ModuleBase classes,
// [PacketAttribute] packet handlers, [NpcAttribute] NPC scripts, [DataAttribute] actions, [GmCommand] commands.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Conquer6609.Framework.Modules
{
    public static class RuntimeModules
    {
        public const string AssemblyName = "QahrModule";
        const string CacheVersion = "1";

        public sealed class Package
        {
            public string Name;
            public string Folder;
            public Assembly Assembly;
            public AssemblyLoadContext Context;
            public bool FromCache;
            public long CompileMs;
            public string Error;
        }

        sealed class ModuleLoadContext : AssemblyLoadContext
        {
            public ModuleLoadContext(string name) : base("module:" + name, isCollectible: true) { }
            // every other assembly (the server, its packages) comes from the default context, so types are shared
            protected override Assembly Load(AssemblyName assemblyName) { return null; }
        }

        static readonly List<Package> packages = new List<Package>();
        static List<MetadataReference> references;

        /// <summary>Modules loaded from ServerDataBase\Modules (successful ones have an Assembly).</summary>
        public static IReadOnlyList<Package> Packages { get { lock (packages) return packages.ToArray(); } }

        public static string Root
        {
            get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "Modules"); }
        }

        /// <summary>Source folders that may hold LIVE modules (a folder with a ".live" file): Modules and Modules\Custom of
        /// the server source (shell.ini [Modules] LiveSource, else found above the exe).</summary>
        public static List<string> LiveRoots
        {
            get
            {
                var list = new List<string>();
                try
                {
                    string src = Settings.String("Modules", "LiveSource", "");
                    if (string.IsNullOrWhiteSpace(src))
                    {
                        var d = new DirectoryInfo(AppContext.BaseDirectory);
                        for (int i = 0; i < 5 && d != null; i++, d = d.Parent)
                            if (File.Exists(Path.Combine(d.FullName, "Modules", "ModuleBase.cs"))) { src = Path.Combine(d.FullName, "Modules"); break; }
                    }
                    if (!string.IsNullOrWhiteSpace(src) && Directory.Exists(src))
                    {
                        list.Add(Path.GetFullPath(src));
                        if (Directory.Exists(Path.Combine(src, "Custom"))) list.Add(Path.GetFullPath(Path.Combine(src, "Custom")));
                    }
                }
                catch { }
                return list;
            }
        }

        /// <summary>The folder of a module: the LIVE source folder when it has a ".live" file, else ServerDataBase\Modules\<name>.</summary>
        public static string FolderOf(string name)
        {
            foreach (string root in LiveRoots)
            {
                string dir = Path.Combine(root, name);
                if (File.Exists(Path.Combine(dir, ".live"))) return dir;
            }
            return Path.Combine(Root, name);
        }

        /// <summary>Compiles (or loads from the cache) every module folder that has C# files. Call once at start-up,
        /// before the module types and attribute handlers are collected.</summary>
        public static void LoadAll()
        {
            if (!Settings.Bool("Modules", "Runtime", true))
                return;
            var dirs = new List<string>();
            foreach (string root in LiveRoots)
                foreach (string d in Directory.GetDirectories(root))
                    if (File.Exists(Path.Combine(d, ".live"))) dirs.Add(d);
            var live = new HashSet<string>(dirs.Select(Path.GetFileName), StringComparer.OrdinalIgnoreCase);
            if (Directory.Exists(Root))
                foreach (string d in Directory.GetDirectories(Root))
                    if (!live.Contains(Path.GetFileName(d))) dirs.Add(d);// a live module wins over a copy in ServerDataBase
            foreach (string dir in dirs.OrderBy(d => Path.GetFileName(d), StringComparer.OrdinalIgnoreCase))
            {
                string name = Path.GetFileName(dir);
                if (name.StartsWith("_") || name.StartsWith("."))
                    continue;
                var files = SourceFiles(dir);
                if (files.Count == 0)
                    continue;// a data folder of a module compiled into the server
                var pkg = Load(name, dir, files);
                lock (packages) packages.Add(pkg);
            }
        }

        /// <summary>Compiles ServerDataBase\Modules\<name>\ again (hot reload). The new package is returned but not
        /// published: ModuleManager swaps it in with Replace once the new code compiled. Null + error when it fails
        /// (the running code stays).</summary>
        public static Package Compile(string name, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(name) || name.StartsWith("_") || name.StartsWith(".") || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                error = "invalid module name";
                return null;
            }
            string dir = FolderOf(name);
            if (!Directory.Exists(dir))
            {
                error = "no folder " + dir;
                return null;
            }
            var files = SourceFiles(dir);
            if (files.Count == 0)
            {
                error = "no .cs files in " + dir;
                return null;
            }
            var pkg = Load(name, dir, files);
            if (pkg.Assembly == null)
            {
                error = pkg.Error ?? "not loaded";
                return null;
            }
            return pkg;
        }

        /// <summary>The loaded package of this module (null = none).</summary>
        public static Package Find(string name)
        {
            lock (packages)
                return packages.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Puts the new package in place of the old one (or adds it) and unloads the old code.</summary>
        public static void Replace(Package old, Package now)
        {
            lock (packages)
            {
                if (old != null) packages.Remove(old);
                packages.Add(now);
            }
            if (old != null && old.Context != null)
                try { old.Context.Unload(); } catch { }// collected once nothing refers to it any more
        }

        /// <summary>The module still has .cs files (its folder was not deleted / emptied / switched off).</summary>
        public static bool HasSource(string name)
        {
            string dir = FolderOf(name);
            return Directory.Exists(dir) && SourceFiles(dir).Count > 0;
        }

        /// <summary>Takes a package out (its module folder was deleted) and unloads its code.</summary>
        public static void Remove(Package pkg)
        {
            lock (packages) packages.Remove(pkg);
            if (pkg.Context != null)
                try { pkg.Context.Unload(); } catch { }
        }

        static List<string> SourceFiles(string dir)
        {
            var list = new List<string>();
            foreach (string f in Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories))
            {
                string rel = f.Substring(dir.Length).TrimStart('\\', '/');
                string first = rel.Split('\\', '/')[0];
                if (rel.Contains("\\") && (first.Equals("data", StringComparison.OrdinalIgnoreCase) || first.Equals("logs", StringComparison.OrdinalIgnoreCase) || first.Equals("content", StringComparison.OrdinalIgnoreCase) || first.Equals(".build", StringComparison.OrdinalIgnoreCase)))
                    continue;
                list.Add(f);
            }
            list.Sort(StringComparer.OrdinalIgnoreCase);
            return list;
        }

        static Package Load(string name, string dir, List<string> files)
        {
            var pkg = new Package { Name = name, Folder = dir };
            var clock = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                // [Security] the cache lives outside the module folder (ServerDataBase\Modules\.modcache\<name>): a module copied
                // from someone else can not bring a prebuilt DLL that differs from its source; only the source is compiled
                string buildDir = Path.Combine(Root, ".modcache", name);
                string dllPath = Path.Combine(buildDir, "module.dll"), pdbPath = Path.Combine(buildDir, "module.pdb"), keyPath = Path.Combine(buildDir, "module.key");
                string key = Key(files);
                byte[] dll = null, pdb = null;
                if (File.Exists(dllPath) && File.Exists(keyPath))
                {
                    string[] saved = File.ReadAllText(keyPath).Trim().Split('|');
                    byte[] cached = File.ReadAllBytes(dllPath);
                    if (saved.Length == 2 && saved[0] == key && saved[1] == Sha(cached))
                    {
                        dll = cached;
                        pdb = File.Exists(pdbPath) ? File.ReadAllBytes(pdbPath) : null;
                        pkg.FromCache = true;
                    }
                }
                if (dll == null)
                {
                    string error;
                    if (!Compile(name, files, out dll, out pdb, out error))
                    {
                        pkg.Error = error;
                        Print("[Modules] FAILED " + name + " (" + dir + "): does not compile" + Environment.NewLine + error, ConsoleColor.Red);
                        return pkg;
                    }
                    try
                    {
                        Directory.CreateDirectory(buildDir);
                        File.WriteAllBytes(dllPath, dll);
                        if (pdb != null) File.WriteAllBytes(pdbPath, pdb);
                        File.WriteAllText(keyPath, key + "|" + Sha(dll));
                    }
                    catch { }// the cache is only a speed-up
                }
                var ctx = new ModuleLoadContext(name);
                using (var ms = new MemoryStream(dll))
                using (var ps = pdb != null ? new MemoryStream(pdb) : null)
                    pkg.Assembly = ctx.LoadFromStream(ms, ps);
                pkg.Context = ctx;
            }
            catch (Exception e)
            {
                pkg.Error = e.GetType().Name + ": " + e.Message;
                Print("[Modules] FAILED " + name + ": " + pkg.Error, ConsoleColor.Red);
            }
            pkg.CompileMs = clock.ElapsedMilliseconds;
            return pkg;
        }

        static string Sha(byte[] data)
        {
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(data)).Replace("-", "");
        }

        // server build (module version id of GameServer.dll) + cache format + every file name and content
        static string Key(List<string> files)
        {
            using (var sha = SHA256.Create())
            {
                var sb = new StringBuilder();
                sb.Append(CacheVersion).Append('|').Append(typeof(RuntimeModules).Assembly.ManifestModule.ModuleVersionId).Append('|');
                foreach (string f in files)
                {
                    sb.Append(Path.GetFileName(f)).Append(':');
                    sb.Append(BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(f)))).Append('|');
                }
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()))).Replace("-", "");
            }
        }

        static bool Compile(string name, List<string> files, out byte[] dll, out byte[] pdb, out string error)
        {
            dll = pdb = null;
            error = null;
            var parse = new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: new[] { "TRACE", "DEBUG", "NET", "QAHR_MODULE" });
            var trees = files.Select(f => CSharpSyntaxTree.ParseText(File.ReadAllText(f), parse, f, Encoding.UTF8)).ToList();
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release, allowUnsafe: true, platform: Platform.AnyCpu,
                nullableContextOptions: NullableContextOptions.Disable);
            var compilation = CSharpCompilation.Create(AssemblyName, trees, References(), options);
            using (var peStream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                var result = compilation.Emit(peStream, pdbStream, options: new Microsoft.CodeAnalysis.Emit.EmitOptions(debugInformationFormat: Microsoft.CodeAnalysis.Emit.DebugInformationFormat.PortablePdb));
                if (!result.Success)
                {
                    var errors = result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
                    var sb = new StringBuilder();
                    foreach (var d in errors.Take(15))
                    {
                        var span = d.Location.GetLineSpan();
                        sb.Append("    ").Append(Path.GetFileName(span.Path)).Append('(').Append(span.StartLinePosition.Line + 1).Append("): ").Append(d.Id).Append(' ').AppendLine(d.GetMessage());
                    }
                    if (errors.Count > 15) sb.Append("    ... ").Append(errors.Count - 15).AppendLine(" more error(s)");
                    error = sb.ToString().TrimEnd();
                    return false;
                }
                dll = peStream.ToArray();
                pdb = pdbStream.ToArray();
                return true;
            }
        }

        // the .NET runtime + every assembly next to the server (GameServer.dll, its packages and libs)
        static List<MetadataReference> References()
        {
            if (references != null)
                return references;
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string tpa = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
            if (tpa != null)
                foreach (string p in tpa.Split(Path.PathSeparator))
                    if (p.Length > 0) paths.Add(p);
            paths.Add(typeof(RuntimeModules).Assembly.Location);
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try { if (!a.IsDynamic && a.Location.Length > 0) paths.Add(a.Location); } catch { }
            }
            var list = new List<MetadataReference>();
            foreach (string p in paths)
            {
                try
                {
                    if (!File.Exists(p)) continue;
                    using (var fs = File.OpenRead(p))
                    using (var pe = new System.Reflection.PortableExecutable.PEReader(fs))
                        if (!pe.HasMetadata) continue;// native dll
                    list.Add(MetadataReference.CreateFromFile(p));
                }
                catch { }
            }
            references = list;
            return list;
        }

        static void Print(string line, ConsoleColor color)
        {
            try { MyConsole.WriteLine(line, color); }
            catch { try { Console.WriteLine(line); } catch { } }
        }
    }
}
