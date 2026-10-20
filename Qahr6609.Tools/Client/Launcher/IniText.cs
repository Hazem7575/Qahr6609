using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScriptHazemLauncher
{
    /// <summary>
    /// Minimal INI reader/writer that keeps the original file layout (comments, order)
    /// when a value is changed. Section and key names are case-insensitive.
    /// </summary>
    public class IniText
    {
        private readonly List<string> _lines = new List<string>();
        private Encoding _encoding = new UTF8Encoding(false);
        public string FilePath { get; private set; }

        public IniText(string path)
        {
            FilePath = path;
            if (!File.Exists(path)) return;
            byte[] raw = File.ReadAllBytes(path);
            string text;
            if (raw.Length >= 3 && raw[0] == 0xEF && raw[1] == 0xBB && raw[2] == 0xBF)
            {
                _encoding = new UTF8Encoding(true);
                text = new UTF8Encoding(false).GetString(raw, 3, raw.Length - 3);
            }
            else
            {
                try
                {
                    // strict UTF-8 first (theme.ini with Arabic text), otherwise the ANSI code page
                    text = new UTF8Encoding(false, true).GetString(raw);
                }
                catch (DecoderFallbackException)
                {
                    _encoding = Ansi();
                    text = _encoding.GetString(raw);
                }
            }
            _lines.AddRange(text.Replace("\r\n", "\n").Split('\n'));
            if (_lines.Count > 0 && _lines[_lines.Count - 1].Length == 0) _lines.RemoveAt(_lines.Count - 1);
        }

        private static Encoding Ansi()
        {
            try { return Encoding.GetEncoding(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ANSICodePage); }
            catch { return Encoding.GetEncoding(1252); }
        }

        public string Get(string section, string key, string def)
        {
            string current = null;
            foreach (string raw in _lines)
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith(";") || line.StartsWith("//") || line.StartsWith("#")) continue;
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    current = line.Substring(1, line.Length - 2).Trim();
                    continue;
                }
                int eq = line.IndexOf('=');
                if (eq <= 0 || current == null) continue;
                if (!string.Equals(current, section, StringComparison.OrdinalIgnoreCase)) continue;
                if (string.Equals(line.Substring(0, eq).Trim(), key, StringComparison.OrdinalIgnoreCase))
                    return line.Substring(eq + 1).Trim();
            }
            return def;
        }

        public int GetInt(string section, string key, int def)
        {
            int v;
            return int.TryParse(Get(section, key, ""), out v) ? v : def;
        }

        public void Set(string section, string key, string value)
        {
            string current = null;
            int sectionEnd = -1;
            for (int i = 0; i < _lines.Count; i++)
            {
                string line = _lines[i].Trim();
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    current = line.Substring(1, line.Length - 2).Trim();
                    if (string.Equals(current, section, StringComparison.OrdinalIgnoreCase)) sectionEnd = i + 1;
                    continue;
                }
                if (!string.Equals(current, section, StringComparison.OrdinalIgnoreCase)) continue;
                if (line.Length > 0) sectionEnd = i + 1;
                int eq = line.IndexOf('=');
                if (eq > 0 && !line.StartsWith(";") && string.Equals(line.Substring(0, eq).Trim(), key, StringComparison.OrdinalIgnoreCase))
                {
                    // keep the "Key = Value" spacing style of the existing line
                    string rawLine = _lines[i];
                    int rawEq = rawLine.IndexOf('=');
                    string left = rawLine.Substring(0, rawEq + 1);
                    string spacing = (rawEq + 1 < rawLine.Length && rawLine[rawEq + 1] == ' ') ? " " : "";
                    _lines[i] = left + spacing + value;
                    return;
                }
            }
            if (sectionEnd < 0)
            {
                if (_lines.Count > 0 && _lines[_lines.Count - 1].Trim().Length > 0) _lines.Add("");
                _lines.Add("[" + section + "]");
                _lines.Add(key + " = " + value);
            }
            else
            {
                _lines.Insert(sectionEnd, key + " = " + value);
            }
        }

        public void Save()
        {
            File.WriteAllText(FilePath, string.Join("\r\n", _lines.ToArray()) + "\r\n", _encoding);
        }
    }
}
