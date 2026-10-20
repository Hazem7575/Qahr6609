using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.ServerSockets
{
    /// <summary>Per-IP count of logged-in sessions. Accessed from the login and socket threads, so all
    /// updates are atomic on a ConcurrentDictionary.</summary>
    public class LoginController
    {
        private static readonly ConcurrentDictionary<string, byte> LoginControllerContainer = new ConcurrentDictionary<string, byte>();
        public static bool CanLogin(string IP)
        {
            if (IP == null) return false;
            byte logintimes = LoginControllerContainer.GetOrAdd(IP, (byte)0);
            return logintimes < 3;
        }
        public static void LoggedIn(string IP)
        {
            if (IP == null) return;
            LoginControllerContainer.AddOrUpdate(IP, (byte)1, (k, times) => times < byte.MaxValue ? (byte)(times + 1) : times);
        }
        public static void Disconnect(string IP)
        {
            if (IP == null) return;
            byte times;
            while (LoginControllerContainer.TryGetValue(IP, out times))
            {
                if (times <= 1)
                {
                    // Last session from this IP: drop the entry so the table does not grow without bound.
                    if (((ICollection<KeyValuePair<string, byte>>)LoginControllerContainer).Remove(new KeyValuePair<string, byte>(IP, times)))
                        return;
                }
                else if (LoginControllerContainer.TryUpdate(IP, (byte)(times - 1), times))
                    return;
            }
        }
    }
}
