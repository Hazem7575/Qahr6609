using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Cryptography
{
    public unsafe static class DHKeyExchange
    {
        // Upper bound for the client public key carried in the handshake reply.
        public const int MaxHandshakeKeyLength = 1024;
        public unsafe static bool GetHandshakeReplyKey(this ServerSockets.Packet msg, out string key)
        {
            key = null;
            // The padding length, key length and key bytes must all lie inside the received data.
            if (msg.Size < 15 || msg.Size > ServerSockets.Packet.MAX_SIZE)
                return false;
            msg.Seek(11);

            long offset = (long)msg.ReadInt32() + 4 + 11;
            if (offset >= 15 && offset + 4 <= msg.Size)
            {
                msg.Seek((int)offset);
                int nSize = msg.ReadInt32();
                if (nSize > 0 && nSize <= MaxHandshakeKeyLength && offset + 4 + nSize <= msg.Size)
                {
                    key = msg.ReadCString(nSize);
                }
            }
            return (key != null);
        }
        public class KeyExchange
        {
            //A320A85EDD79171C341459E94807D71D39BB3B3F3B5161CA84894F3AC3FC7FEC317A2DDEC83B66D30C29261C6492643061AECFCF4A051816D7C359A6A7B7D8FB
            public static string Str_P = "E7A69EBDF105F2A6BBDEAD7E798F76A209AD73FB466431E2E7352ED262F8C558F10BEFEA977DE9E21DCEE9B04D245F300ECCBBA03E72630556D011023F9E857F";
            public static string Str_G = "05";

            public static byte[] P;
            public static byte[] G;

            public static void CreateKeys()
            {
                P = global::Conquer6609.AnsiText.Value.GetBytes(Str_P);
                G = global::Conquer6609.AnsiText.Value.GetBytes(Str_G);
            }
        }
       

        public class ServerKeyExchange
        {
        
            /// <summary>Writes the server key packet into `stream`, which the caller owns until it was sent
            /// (the old overload returned a packet that was already back in the recycle pool).</summary>
            public ServerSockets.Packet CreateServerKeyPacket(Cryptography.DiffieHellman user_dh, ServerSockets.Packet stream)
            {

                byte[] _key = Encoding.ASCII.GetBytes(user_dh.GenerateRequest());

                return stream.Handshake(KeyExchange.P, KeyExchange.G, _key);
            }

            private string Hex(byte[] bytes)
            {
                char[] c = new char[bytes.Length * 2];
                byte b;
                for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
                {
                    b = ((byte)(bytes[bx] >> 4));
                    c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
                    b = ((byte)(bytes[bx] & 0x0F));
                    c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
                }
                return new string(c);
            }
            public byte[] PostProcessDHKey(byte[] key)
            {
                var hashService = new System.Security.Cryptography.MD5CryptoServiceProvider();
                var s1 = Hex(hashService.ComputeHash(key, 0, FixKey(key)));//key.TakeWhile<byte>(((Func<byte, bool>)(x => (x != 0)))).Count<byte>()));
                var s2 = Hex(hashService.ComputeHash(Encoding.ASCII.GetBytes(String.Concat(s1, s1))));
                var sresult = String.Concat(s1, s2);

                return GetArrayPostProcessDHKey(sresult);
            }
            public byte[] GetArrayPostProcessDHKey(string sresult)
            {
                byte[] skey = new byte[sresult.Length];
                for (int x = 0; x < sresult.Length; x++)
                    skey[x] = (byte)sresult[x];
                return skey;
            }
            public int FixKey(byte[] key)
            {
                for (int x = 0; x < key.Length; x++)
                {
                    if (key[x] == 0)
                        return x;
                }
                return key.Length;
            }
            
          /*  public ICrypto HandleClientKeyPacket(string PublicKey, ICrypto cryptographer)
            {
                byte[] key = _keyExchange.ComputeKey(OpenSSL.BigNumber.FromHexString(PublicKey));
                cryptographer.GenerateKey(PostProcessDHKey(key));
                cryptographer.SetIVs(_clientIv, _serverIv);
                return cryptographer;
            }*/
        }
    }
}
