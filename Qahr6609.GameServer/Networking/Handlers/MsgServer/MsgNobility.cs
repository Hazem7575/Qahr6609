using Conquer6609.Game.Conquer6609_Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetNobility(this ServerSockets.Packet stream, out MsgNobility.NobilityAction mode, out ulong UID, out MsgNobility.DonationTyp donationtyp)
        {
            mode = (MsgNobility.NobilityAction)stream.ReadInt32();
            UID = stream.ReadUInt64();
            stream.SeekForward(88);
            donationtyp = (MsgNobility.DonationTyp)stream.ReadUInt8();

        }
        public static unsafe ServerSockets.Packet NobilityIconCreate(this ServerSockets.Packet stream, Role.Instance.Nobility nobility)
        {
            stream.InitWriter();

            stream.Write((uint)MsgNobility.NobilityAction.Icon);//4
            stream.Write(nobility.UID);//8

            string StrList = "" + nobility.UID + " " + nobility.Donation + " " + (byte)nobility.Rank + " " + nobility.Position + "";

            stream.ZeroFill(108);

            stream.Write(StrList);

            stream.Finalize(GamePackets.Nobility);

            return stream;
        }
    }

    public unsafe struct MsgNobility
    {


        public enum NobilityAction : uint
        {
            Donate = 1,
            RankListen = 2,
            Icon = 3,
            NobilityInformarion = 4,
        }
        public enum DonationTyp : byte
        {
            Money = 0,
            ConquerPoints = 1,
            BoundConquerPoints = 2
        }

        [PacketAttribute(GamePackets.Nobility)]
        public unsafe static void HandlerNobility(Client.GameClient user, ServerSockets.Packet stream)
        {
            NobilityAction Action;
            ulong UID;
            DonationTyp donationtyp;
            stream.GetNobility(out Action, out UID, out donationtyp);

            switch (Action)
            {
                case NobilityAction.Donate:
                    {
                        if (!user.Player.OnMyOwnServer)
                            return;
                        if (user.InTrade || user.IsVendor || !user.Socket.Alive)
                            return;
                        // Donation is validated on the full 64-bit value; every later comparison uses the validated amount.
                        long Amount;
                        if (!Conquer6609.Security.Guard.ValidAmount(UID, Conquer6609.Security.Guard.MaxDonation, out Amount))
                        {
                            Conquer6609.Security.Guard.Audit("nobility-donate", user.Player.UID, user.Player.Name, "rejected amount " + UID + " type " + donationtyp);
                            break;
                        }
                        switch (donationtyp)
                        {
                            case DonationTyp.Money:
                                {
                                    if (!user.Player.OnMyOwnServer)
                                        return;
                                    if (user.InTrade)
                                        return;

                                    if (user.Player.Money >= Amount)
                                    {
                                        user.Player.Money -= Amount;
                                        user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);

                                        user.Player.Nobility.Donation += (ulong)Amount;
                                        user.Send(stream.NobilityIconCreate(user.Player.Nobility));
                                        Program.NobilityRanking.UpdateRank(user.Player.Nobility);
                                    }
                                    else
                                    {
                                        user.SendSysMesage("you don't have enough money to donate , or less number to donate.", MsgMessage.ChatMode.Service);
                                        return;
                                    }

                                    break;
                                }
                            case DonationTyp.ConquerPoints:
                                {

                                    if (!user.Player.OnMyOwnServer)
                                        return;
                                    if (user.InTrade)
                                        return;

                                    // CP cost is ceil(amount / 5000) and always at least 1.
                                    long cpCost = (Amount + 4999) / 5000;
                                    if (cpCost >= 1 && (long)user.Player.ConquerPoints >= cpCost)
                                    {
                                        user.Player.ConquerPoints -= (uint)cpCost;
                                        user.Player.Nobility.Donation += (ulong)Amount;
                                        user.Send(stream.NobilityIconCreate(user.Player.Nobility));
                                        Program.NobilityRanking.UpdateRank(user.Player.Nobility);
                                    }



                                    break;
                                }
                            case DonationTyp.BoundConquerPoints:
                                {
                                    if (!user.Player.OnMyOwnServer)
                                        return;
                                    if (user.InTrade)
                                        return;

                                    // Bound CP cost is ceil(amount / 50000) and always at least 1.
                                    long boundCost = (Amount + 49999) / 50000;
                                    if (boundCost >= 1 && (long)user.Player.BoundConquerPoints >= boundCost)
                                    {
                                        user.Player.BoundConquerPoints -= (int)boundCost;
                                        user.Player.Nobility.Donation += (ulong)Amount;
                                        user.Send(stream.NobilityIconCreate(user.Player.Nobility));
                                        Program.NobilityRanking.UpdateRank(user.Player.Nobility);
                                    }


                                    break;
                                }

                        }
                        break;
                    }

                case NobilityAction.RankListen:
                    {
                        int displyPage = UID > 100 ? 0 : (int)UID;
                        var info = Program.NobilityRanking.GetArray();
                        try
                        {
                            int max = 11;
                            int offset = displyPage * max;
                            int count = Math.Min(max, Math.Max(0, info.Length - offset));


                            stream.InitWriter();

                            stream.Write((uint)NobilityAction.RankListen);
                            stream.Write((ushort)displyPage);
                            stream.Write((ushort)(info.Length > 50 ? 5 : ((info.Length / max) + 1)));//10
                            stream.Write((ushort)count);

                            stream.ZeroFill(106);

                            for (int x = 0; x < count; x++)
                            {
                                if (info.Length > offset + x)
                                {
                                    var element = info[offset + x];
                                    if (element.Position < 50)
                                    {
                                        stream.Write(element.UID);
                                        stream.Write((uint)element.Gender);
                                        stream.Write(element.Mesh);
                                        stream.Write(element.Name, 16);
                                        stream.Write((uint)0);
                                        stream.Write(element.Donation);
                                        stream.Write((uint)element.Rank);
                                        stream.Write(element.Position);


                                    }
                                }
                            }
                            stream.Finalize(Game.GamePackets.Nobility);

                            user.Send(stream);

                        }
                        catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                        break;
                    }
                case NobilityAction.NobilityInformarion:
                    {
                        stream.InitWriter();

                        stream.Write((uint)NobilityAction.NobilityInformarion);

                        stream.Write(Program.NobilityRanking.KnightDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.Knight);

                        stream.Write(Program.NobilityRanking.KnightDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.Baron);

                        stream.Write(Program.NobilityRanking.EarlDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.Earl);

                        stream.Write(Program.NobilityRanking.DukeDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.Duke);

                        stream.Write(Program.NobilityRanking.PrinceDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.Prince);

                        stream.Write(Program.NobilityRanking.KingDonation);
                        stream.Write(uint.MaxValue);
                        stream.Write((uint)Role.Instance.Nobility.NobilityRank.King);

                        stream.Finalize(GamePackets.Nobility);

                        user.Send(stream);


                        break;
                    }

            }

        }
    }
}
