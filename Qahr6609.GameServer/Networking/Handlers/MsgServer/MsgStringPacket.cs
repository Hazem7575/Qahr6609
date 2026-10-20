using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetStringPacket(this ServerSockets.Packet stream, out MsgStringPacket str)
        {
            str = new MsgStringPacket();
            uint timer = stream.ReadUInt32();

            str.UID = stream.ReadUInt32();
            str.ID = (MsgStringPacket.StringID)stream.ReadUInt8();

            str.Strings = stream.ReadStringList();
        }

        public static unsafe ServerSockets.Packet StringPacketCreate(this ServerSockets.Packet stream, MsgStringPacket pac)
        {
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);

            if (pac.ID == MsgStringPacket.StringID.LocationEffect)
            {
                stream.Write(pac.X);
                stream.Write(pac.Y);
            }
            else
                stream.Write(pac.UID);

            stream.Write((byte)pac.ID);

            if (pac.Strings != null)
            {
                stream.Write(pac.Strings);
            }
            else
                stream.ZeroFill(2);


            stream.Finalize(GamePackets.String_);
            return stream;
        }
    }
    public unsafe class MsgStringPacket
    {
        public enum StringID : byte
        {
            Fireworks =1,
            GuildName = 3,
            Spouse = 6,
            LocationEffect = 9,
            Effect = 10,
            GuildList = 11,
            Unknown = 13,
            ViewEquipSpouse = 16,
            StartGamble = 17,
            EndGamble = 18,
            Sound = 20,
            GuildAllies = 21,
            GuildEnemies = 22,
            WhisperDetails = 26,
            ServerName = 61
         
        }

        public string[] Strings;
        public uint UID;
        public ushort X;
        public ushort Y;
        public byte StringsLength;
        public StringID ID;
       

        [PacketAttribute(GamePackets.String_)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgStringPacket Str;

            stream.GetStringPacket(out Str);

            switch (Str.ID)
            {
                case StringID.ViewEquipSpouse:
                    {
                        Client.GameClient viewClient;
                        if (Database.Server.GamePoll.TryGetValue(Str.UID, out viewClient))
                        {
                            Str.Strings = new string[1];
                            Str.Strings[0] = viewClient.Player.Spouse;
                            Str.StringsLength = (byte)viewClient.Player.Spouse.Length;
                            Str.UID = viewClient.Player.UID;
                            user.Send(stream.StringPacketCreate(Str));
                        }
                        break;
                    }
                case StringID.WhisperDetails:
                    {

                        foreach (var Target in Database.Server.GamePoll.Values)
                        {
                            if (Target.Player.Name == Str.Strings[0])
                            {
                                Str.Strings = new string[2];
                                Str.Strings[0] = Target.Player.Name;
                                Str.StringsLength = 0;
                                Str.StringsLength += (byte)Target.Player.Name.Length;
                                string otherstring = "";
                                otherstring += Target.Player.UID + " ";
                                otherstring += Target.Player.Level + " ";
                                otherstring += Target.Player.BattlePower + " #";
                                if (Target.Player.MyGuild != null)
                                {
                                    otherstring += Target.Player.MyGuild.GuildName + " # ";
                                }
                                else
                                    otherstring += " # ";
                                otherstring += Target.Player.Spouse + " ";
                                otherstring += (byte)(Target.Player.NobilityRank) + " ";
                                //1 girl
                                //2 = boy

                                if (Target.Player.Body % 10 < 3)
                                    otherstring += "2";
                                else
                                    otherstring += "1";
                                Str.StringsLength += (byte)otherstring.Length;
                                Str.Strings[1] = otherstring;
                                user.Send(stream.StringPacketCreate(Str));
                            }
                        }
                        break;
                    }
            }
        
        
        }
        
    }
}
