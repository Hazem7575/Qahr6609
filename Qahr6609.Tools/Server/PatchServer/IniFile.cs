using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PatchServer
{
    /// <summary>
    /// Very small INI reader. Section and key names are case-insensitive.
    /// Supports "; comment" and "# comment" lines, [Section] headers and
    /// key=value pairs. Values may optionally be wrapped in double quotes.
    /// </summary>
    internal sealed class IniFile
    {
        private readonly Dictionary<string, Dictionary<string, string>> sections =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Loads an INI file. A missing file yields an empty INI.</summary>
        public static IniFile Load(string path)
        {
            IniFile ini = new IniFile();
            if (!File.Exists(path))
            {
                return ini;
            }

            // Encoding.UTF8 also detects and strips a BOM if present.
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            string currentSection = string.Empty;

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (line.Length > 0 && line[0] == '﻿')
                {
                    line = line.Substring(1).Trim();
                }

                if (line.Length == 0 || line[0] == ';' || line[0] == '#')
                {
                    continue;
                }

                if (line[0] == '[')
                {
                    int close = line.IndexOf(']');
                    if (close > 0)
                    {
                        currentSection = line.Substring(1, close - 1).Trim();
                    }
                    continue;
                }

                int eq = line.IndexOf('=');
                if (eq <= 0)
                {
                    continue; // Not a key=value line; ignore it.
                }

                string key = line.Substring(0, eq).Trim();
                string value = line.Substring(eq + 1).Trim();
                if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
                {
                    value = value.Substring(1, value.Length - 2);
                }

                if (key.Length == 0)
                {
                    continue;
                }

                ini.Set(currentSection, key, value);
            }

            return ini;
        }

        /// <summary>Returns the value or defaultValue when the key is missing.</summary>
        public string Get(string section, string key, string defaultValue)
        {
            Dictionary<string, string> keys;
            if (sections.TryGetValue(section ?? string.Empty, out keys))
            {
                string value;
                if (keys.TryGetValue(key, out value))
                {
                    return value;
                }
            }
            return defaultValue;
        }

        private void Set(string section, string key, string value)
        {
            Dictionary<string, string> keys;
            if (!sections.TryGetValue(section, out keys))
            {
                keys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                sections[section] = keys;
            }
            keys[key] = value;
        }
    }
}
