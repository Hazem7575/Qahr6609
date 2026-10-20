using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Cryptography
{
    public class DiffieHellman
    {
        private BigInteger p = 0;
        private BigInteger g = 0;
        private BigInteger a = 0;
        private BigInteger b = 0;
        private BigInteger s = 0;
        private BigInteger A = 0;
        private BigInteger B = 0;
        private readonly string pHex, gHex;

        public BigInteger GetKey() { return s; }
        public BigInteger GetRequest() { return A; }
        public BigInteger GetResponse() { return A; }

        public String Key { get { return s.ToHexString(); } }

        public override String ToString() { return s.ToHexString(); }
        public Byte[] ToBytes() { return s.getBytes(); }

        /// <summary>
        /// Create a new Diffie-Hellman exchange where the prime number is p and the base is g.
        /// </summary>
        public DiffieHellman(String p, String g)
        {
            pHex = p;
            gHex = g;
            this.p = new BigInteger(p, 16);
            this.g = new BigInteger(g, 16);
        }

        ~DiffieHellman() { }

        private static readonly System.Security.Cryptography.RNGCryptoServiceProvider Rng = new System.Security.Cryptography.RNGCryptoServiceProvider();

        /// <summary>
        /// Random 256-bit private exponent (top bit set, like the 256-bit prime used before). The exponent does
        /// not need to be prime; p, g and the public key format are unchanged.
        /// </summary>
        internal static BigInteger RandomExponent()
        {
            byte[] bytes = new byte[32];
            Rng.GetBytes(bytes);   // thread-safe
            bytes[0] |= 0x80;
            return new BigInteger(bytes);
        }

        /// <summary>
        /// Generates the server request and return the A key.
        /// </summary>
        public String GenerateRequest()
        {
            BigInteger priv, pub;
            if (DHKeyPool.TryTake(pHex, gHex, out priv, out pub))
            {
                a = priv;
                A = pub;
            }
            else
            {
                a = RandomExponent();
                A = g.modPow(a, p);
            }

            return A.ToHexString();
        }

        /// <summary>
        /// Generates the client response and the S key with the A key.
        /// The B key will be returned.
        /// </summary>
        public String GenerateResponse(String PubKey)
        {
            b = RandomExponent();
            B = g.modPow(b, p);

            A = new BigInteger(PubKey, 16);
            s = A.modPow(b, p);

            return B.ToHexString();
        }

        /// <summary>
        /// Handles the client response to generate the S key with the B key.
        /// </summary>
        public bool HandleResponse(String PubKey)
        {
            B = new BigInteger(PubKey, 16);
            // [Security] a public key of 0, 1 or p-1 (or outside 2..p-2) forces a known shared secret: refuse it
            if (B <= 1 || B >= p - 1)
                return false;
            s = B.modPow(a, p);
            return true;
        }
    }

    /// <summary>
    /// Precomputed (private, public) key pairs for the server's standard p/g, filled by a background thread,
    /// so accepting a connection does not cost a modular exponentiation on the socket thread.
    /// [Performance] DHKeyPoolSize (default 16, 0 = off).
    /// </summary>
    internal static class DHKeyPool
    {
        private static readonly System.Collections.Concurrent.ConcurrentQueue<BigInteger[]> Pool = new System.Collections.Concurrent.ConcurrentQueue<BigInteger[]>();
        private static readonly System.Threading.AutoResetEvent Refill = new System.Threading.AutoResetEvent(true);
        private static readonly int Size = Math.Max(0, Math.Min(256, Conquer6609.Framework.Settings.Int("Performance", "DHKeyPoolSize", 16)));
        private static int started;

        public static bool TryTake(string pHex, string gHex, out BigInteger priv, out BigInteger pub)
        {
            priv = null;
            pub = null;
            if (Size <= 0 || pHex != DHKeyExchange.KeyExchange.Str_P || gHex != DHKeyExchange.KeyExchange.Str_G)
                return false;
            if (System.Threading.Interlocked.Exchange(ref started, 1) == 0)
            {
                var t = new System.Threading.Thread(Run) { IsBackground = true, Name = "DHKeyPool", Priority = System.Threading.ThreadPriority.BelowNormal };
                t.Start();
            }
            BigInteger[] pair;
            bool taken = Pool.TryDequeue(out pair);
            Refill.Set();
            if (!taken)
                return false;
            priv = pair[0];
            pub = pair[1];
            return true;
        }

        private static void Run()
        {
            var p = new BigInteger(DHKeyExchange.KeyExchange.Str_P, 16);
            var g = new BigInteger(DHKeyExchange.KeyExchange.Str_G, 16);
            while (true)
            {
                try
                {
                    while (Pool.Count < Size)
                    {
                        var priv = DiffieHellman.RandomExponent();
                        Pool.Enqueue(new BigInteger[] { priv, g.modPow(priv, p) });
                    }
                }
                catch (Exception e)
                {
                    MyConsole.WriteException(e);
                    System.Threading.Thread.Sleep(1000);
                }
                Refill.WaitOne(10000);
            }
        }
    }
}
