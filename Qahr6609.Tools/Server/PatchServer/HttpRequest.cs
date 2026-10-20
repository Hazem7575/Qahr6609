using System;
using System.Collections.Generic;

namespace PatchServer
{
    /// <summary>
    /// A parsed HTTP request head (request line + headers). Bodies are not
    /// supported because only GET and HEAD are served.
    /// </summary>
    internal sealed class HttpRequest
    {
        private static readonly char[] QueryChars = new char[] { '?', '#' };

        private HttpRequest()
        {
            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Request method exactly as sent (e.g. "GET").</summary>
        public string Method { get; private set; }

        /// <summary>Raw request target from the request line.</summary>
        public string Target { get; private set; }

        /// <summary>URL-decoded path without query string, always starting with "/".</summary>
        public string Path { get; private set; }

        /// <summary>Request headers (names are case-insensitive).</summary>
        public Dictionary<string, string> Headers { get; private set; }

        public bool IsHead
        {
            get { return Method == "HEAD"; }
        }

        /// <summary>
        /// Parses the header block (everything before the blank line).
        /// Returns null if the request line is malformed.
        /// </summary>
        public static HttpRequest Parse(string headerText)
        {
            if (headerText == null)
            {
                return null;
            }

            // Tolerate stray empty lines before the request line (RFC 7230 3.5).
            headerText = headerText.TrimStart('\r', '\n');
            string[] lines = headerText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            if (lines.Length == 0)
            {
                return null;
            }

            string[] parts = lines[0].Split(' ');
            if (parts.Length != 3)
            {
                return null;
            }

            string method = parts[0];
            string target = parts[1];
            string version = parts[2];
            if (method.Length == 0 || target.Length == 0
                || !version.StartsWith("HTTP/", StringComparison.Ordinal))
            {
                return null;
            }

            string path = DecodeTargetPath(target);
            if (path == null)
            {
                return null;
            }

            HttpRequest request = new HttpRequest();
            request.Method = method;
            request.Target = target;
            request.Path = path;

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Length == 0)
                {
                    continue;
                }
                int colon = line.IndexOf(':');
                if (colon <= 0)
                {
                    continue; // Ignore garbage header lines instead of failing.
                }
                string name = line.Substring(0, colon).Trim();
                string value = line.Substring(colon + 1).Trim();
                if (name.Length > 0)
                {
                    request.Headers[name] = value;
                }
            }

            return request;
        }

        /// <summary>
        /// Turns a request target into a decoded path: handles absolute-form
        /// targets (http://host/path), strips query/fragment and percent-decodes.
        /// Returns null when the target is not a usable path.
        /// </summary>
        private static string DecodeTargetPath(string target)
        {
            string raw = target;

            if (raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                int schemeEnd = raw.IndexOf("//", StringComparison.Ordinal) + 2;
                int slash = raw.IndexOf('/', schemeEnd);
                raw = slash >= 0 ? raw.Substring(slash) : "/";
            }

            int cut = raw.IndexOfAny(QueryChars);
            if (cut >= 0)
            {
                raw = raw.Substring(0, cut);
            }

            if (raw.Length == 0 || raw[0] != '/')
            {
                return null;
            }

            try
            {
                // Path semantics: '+' stays a literal plus, %XX is UTF-8 decoded.
                return Uri.UnescapeDataString(raw);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
