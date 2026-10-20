using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;

namespace Conquer6609
{
    public class CachedAttributeInvocation<TAction, TAttribute, TKey>
        where TAttribute : Attribute
        where TAction : class
        where TKey : struct
    {
        private readonly Func<TAttribute, TKey> translator;
        // replaced as a whole (copy-on-write) when a runtime module adds or removes handlers; readers never lock
        private volatile Dictionary<TKey, Tuple<TAttribute, TAction>> map;
        // key -> runtime module that added it (keys of the server itself are not listed)
        private readonly Dictionary<TKey, string> owners = new Dictionary<TKey, string>();
        private readonly object writeLock = new object();

        public CachedAttributeInvocation(Func<TAttribute, TKey> translator)
        {
            this.translator = translator;
            map = new Dictionary<TKey, Tuple<TAttribute, TAction>>();
            try
            {
                Scan(Assembly.GetCallingAssembly(), map, null, null);
            }
            catch (ReflectionTypeLoadException e) { MyConsole.WriteException(e); }
        }

        void Scan(Assembly assembly, Dictionary<TKey, Tuple<TAttribute, TAction>> into, string owner, List<string> conflicts)
        {
            foreach (var types in assembly.GetTypes())
            {
                try
                {
                    var methods = types.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (var method in methods)
                    {
                        foreach (var attribute in method.GetCustomAttributes(true))
                        {
                            try
                            {
                                var testAttr = attribute as TAttribute;
                                if (testAttr != null)
                                {
                                    var key = translator(testAttr);
                                    var invoker = Delegate.CreateDelegate(typeof(TAction), method) as TAction;
                                    if (owner != null && into.ContainsKey(key))
                                    {
                                        string by;
                                        conflicts.Add(key + " (" + typeof(TAttribute).Name + ") is already handled by " + (owners.TryGetValue(key, out by) ? "module " + by : "the server") + " - " + types.Name + "." + method.Name + " ignored");
                                        continue;
                                    }
                                    into.Add(key, new Tuple<TAttribute, TAction>(testAttr, invoker));
                                    if (owner != null)
                                        owners[key] = owner;
                                }
                            }
                            catch (Exception e) { Console.WriteLine(e.ToString()); }
                        }
                    }
                }
                catch (Exception e) { MyConsole.WriteException(e); }
            }
        }

        /// <summary>Adds the handlers of a runtime module's assembly. A key the server or another module already handles
        /// is not replaced; it is returned as a conflict line.</summary>
        public List<string> AddAssembly(Assembly assembly, string owner)
        {
            var conflicts = new List<string>();
            lock (writeLock)
            {
                var next = new Dictionary<TKey, Tuple<TAttribute, TAction>>(map);
                Scan(assembly, next, owner, conflicts);
                map = next;
            }
            return conflicts;
        }

        /// <summary>Removes every handler a runtime module added (module unloaded / reloaded).</summary>
        public int RemoveOwner(string owner)
        {
            lock (writeLock)
            {
                var keys = owners.Where(kv => string.Equals(kv.Value, owner, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Key).ToList();
                if (keys.Count == 0)
                    return 0;
                var next = new Dictionary<TKey, Tuple<TAttribute, TAction>>(map);
                foreach (var k in keys)
                {
                    next.Remove(k);
                    owners.Remove(k);
                }
                map = next;
                return keys.Count;
            }
        }

        /// <summary>Number of handlers a runtime module added.</summary>
        public int CountOwner(string owner)
        {
            lock (writeLock)
                return owners.Count(kv => string.Equals(kv.Value, owner, StringComparison.OrdinalIgnoreCase));
        }

        public bool TryGetInvoker(TKey key, out Tuple<TAttribute, TAction> folded)
        {
            return map.TryGetValue(key, out folded);
        }
        public bool TryGetInvoker(TKey key, out TAction action)
        {
            Tuple<TAttribute, TAction> folded;
            if (this.TryGetInvoker(key, out folded))
            {
                action = folded.Item2;
                return true;
            }
            action = null;
            return false;
        }
    }
}
