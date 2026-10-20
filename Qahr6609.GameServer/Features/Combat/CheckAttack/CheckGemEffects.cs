using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CheckGemEffects
    {
        public enum GemEffect : byte
        {
            moon,//moon
            phoenix,//phonex
            rainbow,//rainbow
            purpleray,//violt
            goldendragon,//dragon
            fastflash,//fury gem
            goldenkylin,//Kylin
            recovery,//tortoiseGem
            Aegis4,//bless Effect
        }
        public static void CheckRespouseDamage(Client.GameClient client)
        {
            if (client.Equipment.SuprtTortoiseGem)
            {
                if (Calculate.Base.Rate(10))
                {
                    if (Calculate.Base.Rate(5))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendString(stream,MsgStringPacket.StringID.Effect, true, new string[1] { GemEffect.recovery.ToString() });
                        }
                    }
                }
            }
            if (client.Equipment.HaveBless)
            {
                if (Calculate.Base.Rate(10))
                {
                    if (Calculate.Base.Rate(5))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendString(stream,MsgStringPacket.StringID.Effect, true, new string[1] { GemEffect.Aegis4.ToString() });
                        }
                    }
                }
            }
        }
        public static void TryngEffect(Client.GameClient client)
        {
            if (client.Equipment.SuperDragonGem)
            {
                if (Calculate.Base.Rate(4))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.SendString(stream,MsgStringPacket.StringID.Effect, true, new string[1] { GemEffect.goldendragon.ToString() });
                    }
                }
            }
            if (client.Equipment.SuperPheonixGem)
            {
                if (Calculate.Base.Rate(5))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.SendString(stream,MsgStringPacket.StringID.Effect, true, new string[1] { GemEffect.phoenix.ToString() });
                    }
                }
            }
            if (client.Equipment.SuperRaibowGem)
            {
                if (Calculate.Base.Rate(5))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.SendString(stream,MsgStringPacket.StringID.Effect, true, new string[1] { GemEffect.rainbow.ToString() });
                    }
                }
            }
            if (client.Equipment.SuperMoonGem)
            {
                if (Calculate.Base.Rate(5))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[1] { GemEffect.moon.ToString() });
                    }
                }
            }
            if (client.Equipment.SuperVioletGem)
            {
                if (Calculate.Base.Rate(5))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.SendString(stream,MsgStringPacket.StringID.Effect, true, new string[1] { GemEffect.purpleray.ToString() });
                    }
                }
            }
            if (client.Equipment.SuperKylinGem)
            {
                if (Calculate.Base.Rate(5))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[1] { GemEffect.goldenkylin.ToString() });
                    }
                }
            }
            if (client.Equipment.SuperFuryGem)
            {
                if (Calculate.Base.Rate(5))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[1] { GemEffect.fastflash.ToString() });
                    }
                }
            }
        }
    }
}
