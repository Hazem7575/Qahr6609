using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static void GetHeroInfo(this ServerSockets.Packet stream, Client.GameClient Owner, out Role.Player user)
        {
            user = new Role.Player(Owner);
            user.InitTransfer = stream.ReadUInt32();

            user.RealUID = stream.ReadUInt32();
            user.AparenceType = stream.ReadUInt16();
            uint mesh = stream.ReadUInt32();
            user.Body = (ushort)(mesh % 10000);
            user.Face = (ushort)((mesh - user.Body) / 10000);
            user.Hair = stream.ReadUInt16();
            user.Money = stream.ReadInt64();
            user.ConquerPoints = stream.ReadUInt32();
            user.Experience = stream.ReadUInt64();

            user.ServerID = (ushort)stream.ReadUInt16();

            user.SetLocationType = (ushort)stream.ReadUInt16();

            user.SpecialTitleID = stream.ReadUInt32();
            user.SpecialWingID = stream.ReadUInt32();
            //   stream.SeekForward(2 * sizeof(uint));
            user.VirtutePoints = stream.ReadUInt32();
            user.HeavenBlessing = stream.ReadInt32();
            user.Strength = stream.ReadUInt16();
            user.Agility = stream.ReadUInt16();
            user.Vitality = stream.ReadUInt16();
            user.Spirit = stream.ReadUInt16();
            user.Atributes = stream.ReadUInt16();
            user.HitPoints = stream.ReadInt32();
            user.Mana = stream.ReadUInt16();
            user.PKPoints = stream.ReadUInt16();
            user.Level = stream.ReadUInt8();
            user.Class = stream.ReadUInt8();
            user.FirstClass = stream.ReadUInt8();
            user.SecoundeClass = stream.ReadUInt8();
            user.NobilityRank = (Role.Instance.Nobility.NobilityRank)stream.ReadUInt8();
            user.Reborn = stream.ReadUInt8();
            stream.SeekForward(sizeof(byte));
            user.QuizPoints = stream.ReadUInt32();
            stream.SeekForward(sizeof(uint));
            user.Enilghten = stream.ReadUInt16();
            user.EnlightenReceive = (ushort)(stream.ReadUInt16() / 100);
            stream.SeekForward(sizeof(uint));
            user.VipLevel = (byte)stream.ReadUInt32();
            user.MyTitle = (byte)stream.ReadUInt16();
            user.BoundConquerPoints = stream.ReadInt32();
            stream.SeekForward(sizeof(byte));
            stream.SeekForward(sizeof(uint));
            stream.SeekForward(2 * sizeof(uint));
            stream.SeekForward(sizeof(ushort));
            stream.SeekForward(sizeof(uint));
            stream.SeekForward(sizeof(uint));
            stream.SeekForward(sizeof(uint));
            string[] strs = stream.ReadStringList();
            user.Name = strs[0];
            user.Spouse = strs[1];


        }


        public static unsafe ServerSockets.Packet HeroInfo(this ServerSockets.Packet stream, Role.Player client, int inittransfer = 0)
        {
            stream.InitWriter();
            if (inittransfer == 0)
                stream.Write(Extensions.Time32.Now.Value);
            else
                stream.Write(inittransfer);
            stream.Write(client.UID);
            stream.Write((ushort)client.AparenceType);
            stream.Write(client.Mesh);
            stream.Write(client.Hair);
            stream.Write(client.Money);
            stream.Write(client.ConquerPoints);
            stream.Write(client.Experience);
            stream.Write((ushort)Database.GroupServerList.MyServerInfo.ID);//for inter server.

            stream.Write(client.SetLocationType);
            stream.Write(client.SpecialTitleID);//for inter server.
            stream.Write(client.SpecialWingID);//for inter server.
            stream.Write(client.VirtutePoints);
            stream.Write(client.HeavenBlessing);//forinterserver
            stream.Write(client.Strength);
            stream.Write(client.Agility);
            stream.Write(client.Vitality);
            stream.Write(client.Spirit);
            stream.Write(client.Atributes);
            stream.Write(client.HitPoints);
            stream.Write(client.Mana);
            stream.Write(client.PKPoints);
            stream.Write((byte)client.Level);
            stream.Write(client.Class);
            stream.Write(client.FirstClass);
            stream.Write(client.SecoundeClass);
            stream.Write((byte)client.NobilityRank);
            stream.Write(client.Reborn);
            stream.Write((byte)0);//unknow
            stream.Write(client.QuizPoints);

            stream.Write((uint)client.MainFlag);

            stream.Write(client.Enilghten);
            stream.Write((ushort)(client.EnlightenReceive * 100));
            stream.Write((uint)0);//unknow
            stream.Write((uint)client.VipLevel);
            stream.Write((ushort)client.MyTitle);
            stream.Write(client.BoundConquerPoints);

            if (client.SubClass != null)
            {
                stream.Write((byte)client.ActiveSublass);
                stream.Write(client.SubClass.GetHashPoint());
            }
            else
                stream.ZeroFill(5);

            stream.Write((uint)0);
            stream.Write(client.RacePoints);//race points?
            stream.Write((ushort)client.CountryID);//country

            stream.Write((uint)0);
            //   stream.Write((uint)0);//union ?
            stream.Write((uint)0);
            stream.Write((uint)0);

            stream.Write(client.Name, "", client.Spouse);

            stream.Finalize(GamePackets.HeroInfo);
          

            return stream;
        }

    }

}
