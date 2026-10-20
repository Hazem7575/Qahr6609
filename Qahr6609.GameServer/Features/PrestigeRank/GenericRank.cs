using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetGenericRanking(this ServerSockets.Packet stream, out MsgGenericRanking.Action Mode, out MsgGenericRanking.RankType ranktyp
            , out ushort RegisteredCount, out ushort Page, out int Count)
        {
            Mode = (MsgGenericRanking.Action)stream.ReadUInt32();
            ranktyp = (MsgGenericRanking.RankType)stream.ReadUInt32();

            RegisteredCount = stream.ReadUInt16();
            Page = stream.ReadUInt16();

            Count = stream.ReadInt32();
        }
        public static unsafe ServerSockets.Packet GenericRankingCreate(this ServerSockets.Packet stream, MsgGenericRanking.Action Mode, MsgGenericRanking.RankType ranktyp
            , ushort RegisteredCount, ushort Page, int Count)
        {
            stream.InitWriter();

            stream.Write((uint)Mode);
            stream.Write((uint)ranktyp);
            stream.Write(RegisteredCount);
            stream.Write(Page);
            stream.Write(Count);
            stream.Write((uint)0);//unknow

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemGenericRankingCreate(this ServerSockets.Packet stream, int Rank, uint Amount, uint UID, string name)
        {
            stream.Write((long)Rank);
            // stream.Write(Rank);
            stream.Write((long)Amount);
            //stream.Write(Amount);
            stream.Write(UID);
            stream.Write(UID);
            stream.Write(name, 16);
            stream.Write(name, 16);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write(0);
            stream.Write((long)(0));
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemGenericRankingCreate(this ServerSockets.Packet stream, int Rank, uint Amount, uint UID, string name
           , uint Level, uint Class, uint Mesh)
        {
            stream.Write((long)Rank);
            stream.Write((long)Amount);
            // stream.Write(UID);
            stream.Write(UID);
            // stream.Write(name, 16);
            stream.Write(name, 16);
            stream.Write(Level);
            stream.Write(Class);
            stream.Write(Mesh);
            stream.Write(0);
            stream.Write((long)(0));
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemGenericRankingCreate(this ServerSockets.Packet stream, int Rank, uint Amount, uint UID1, uint UID2, string name
          , uint Level, uint Class, uint Mesh)
        {
            stream.Write((long)Rank);
            stream.Write((long)Amount);
            stream.Write(UID1);
            stream.Write(UID2);
            stream.Write(name, 16);
            stream.Write(name, 16);
            stream.Write(Level);
            stream.Write(Class);
            stream.Write(Mesh);
            stream.Write(0);
            stream.Write((long)(0));
            return stream;
        }
        public static unsafe ServerSockets.Packet GenericRankingFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.GenericRanking);
            return stream;
        }
    }

    public unsafe struct MsgGenericRanking
    {
        public enum Action : uint
        {
            Ranking = 1,
            QueryCount = 2,
            UpdateScreen = 4,
            InformationRequest = 5,
            PrestigeRanks = 6
        }
        public enum RankType : uint
        {
            None = 0,
            RoseFairy = 30000002,
            LilyFairy = 30000102,
            OrchidFairy = 30000202,
            TulipFairy = 30000302,
            Kiss = 30000402,
            Love = 30000502,
            Tins = 30000602,
            Jade = 30000702,

            Chi = 60000000,
            DragonChi = 60000001,
            PhoenixChi = 60000002,
            TigerChi = 60000003,
            TurtleChi = 60000004,
            InnerPower = 70000000,
            PrestigeRank = 80000000,

            TopTrojans = 80000001,
            TopWarriors = 80000002,
            TopArchers = 80000003,
            TopNinjas = 80000004,
            TopMonks = 80000005,
            TopPirates = 80000006,
            TopDraonWarriors = 80000007,
            TopWaters = 80000008,
            TopFires = 80000009,
            TopWindWalker = 80000010
        }

        public static object SynRoot = new object();
        [PacketAttribute(GamePackets.GenericRanking)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            const int max = 10;

            try
            {

                MsgGenericRanking.Action Mode;
                MsgGenericRanking.RankType ranktyp;
                ushort RegisteredCount;
                ushort Page;
                int Count;
                stream.GetGenericRanking(out Mode, out ranktyp, out RegisteredCount, out Page, out Count);
                if ((uint)ranktyp == MsgTournaments.MsgQuizShow.RankingType)// Quiz Show ranking window
                {
                    if (MsgTournaments.MsgSchedules.QuizShow != null)
                        MsgTournaments.MsgSchedules.QuizShow.SendRanking(user, stream, Mode == Action.QueryCount, Page);
                    return;
                }

                switch (Mode)
                {

                    case Action.PrestigeRanks:
                        {
                            var array = Database.PrestigeRanking.Ranks.GetValues().OrderBy(p => p._Type);
                            var BestOf = Database.PrestigeRanking.BestOfTheWorld;
                            if (BestOf != null)
                            {
                                stream.GenericRankingCreate(Action.PrestigeRanks, 0, 0, 0, array.Count(p => p.BestOfClass != null) + 1);
                                stream.AddItemGenericRankingCreate((int)BestOf.Rank, BestOf.TotalPoints, (uint)(80000000), BestOf.UID, BestOf.Name, BestOf.Level, BestOf.Class, BestOf.Mesh);
                                foreach (var rank in array)
                                {
                                    BestOf = rank.BestOfClass;
                                    if (BestOf != null)//                                                              5000000 
                                        stream.AddItemGenericRankingCreate((int)1, BestOf.TotalPoints, (uint)(80000001 + (uint)rank._Type), BestOf.UID, BestOf.Name, BestOf.Level, BestOf.Class, BestOf.Mesh);
                                }
                                user.Send(stream.GenericRankingFinalize());
                            }
                            break;

                        }
                    case Action.QueryCount:
                        {
                            if (ranktyp == RankType.PrestigeRank)
                            {
                                stream.GenericRankingCreate(MsgGenericRanking.Action.QueryCount, MsgGenericRanking.RankType.PrestigeRank, 0, 0, 1);

                                uint Rank = Database.PrestigeRanking.GetMyRank(Database.PrestigeRanking.GetIndex(user.Player.Class), user.Player.UID);
                                stream.AddItemGenericRankingCreate((int)Rank, user.PrestrigeEntry.TotalPoints, user.Player.UID, user.Player.Name, user.Player.Level, user.Player.Class, user.Player.Mesh);
                                user.Send(stream.GenericRankingFinalize());

                                break;
                            }
                            lock (SynRoot)
                            {
                                if (Role.Core.IsGirl(user.Player.Body))
                                    user.Player.Flowers.UpdateMyRank(user);
                                else if (Role.Core.IsBoy(user.Player.Body))
                                {
                                    foreach (var Flower in user.Player.Flowers)
                                    {
                                        if (Flower.Rank > 0 && Flower.Rank <= 100)
                                        {
                                            stream.GenericRankingCreate(Action.QueryCount, (Game.MsgServer.MsgGenericRanking.RankType)user.Player.Flowers.CreateBoyIcon(Flower), RegisteredCount, Page, 1);

                                            // The reply holds exactly one row per flower; the client count is bounded to it.
                                            int rows = Conquer6609.Security.Guard.ClampCount(Count, 1);
                                            for (int x = 0; x < rows; x++)
                                            {
                                                stream.AddItemGenericRankingCreate(Flower.Rank, Flower.Amount, Flower.UID, Flower.Name);
                                            }
                                            user.Send(stream.GenericRankingFinalize());
                                        }
                                    }

                                    stream.GenericRankingCreate(Action.InformationRequest, ranktyp, RegisteredCount, Page, 0);
                                    stream.AddItemGenericRankingCreate(0, 0, 0, "");
                                    user.Send(stream.GenericRankingFinalize());
                                }
                            }
                            break;
                        }

                    case Action.Ranking:
                        {
                            var OldRank = ranktyp;
                            if (ranktyp == RankType.PrestigeRank || ranktyp >= RankType.TopTrojans && ranktyp <= RankType.TopWindWalker)
                            {
                                var _RankType = (Database.PrestigeRanking.Type)(((uint)ranktyp % 10));
                                if (ranktyp == RankType.TopWindWalker)
                                    _RankType = Database.PrestigeRanking.Type.WindWalker;
                                var rank = Database.PrestigeRanking.Ranks[_RankType];
                                var array = rank.Items.Values.OrderByDescending(x => x.TotalPoints).ToArray();
                                int offset = Page * max;
                                int count = Math.Min(max, array.Length);

                                stream.GenericRankingCreate(Action.Ranking, (RankType)OldRank, rank.MaxItems, Page, count);

                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length)
                                        break;
                                    var entity = array[x + offset];
                                    stream.AddItemGenericRankingCreate((int)(offset + x + 1), (uint)entity.TotalPoints, entity.UID, entity.UID, entity.Name, entity.Level, entity.Class, entity.Mesh);
                                }
                                user.Send(stream.GenericRankingFinalize());


                            }
                            if (ranktyp == RankType.InnerPower)
                            {
                                var array = Role.Instance.InnerPower.InnerPowerRank.GetRankingList();

                                int offset = Page * max;
                                int count = Math.Min(max, array.Length);

                                stream.GenericRankingCreate(Action.Ranking, OldRank, Role.Instance.InnerPower.InnerPowerRank.MaxPlayers, Page, count);

                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length)
                                        break;
                                    var entity = array[x + offset];
                                    stream.AddItemGenericRankingCreate((int)(offset + x + 1), (uint)entity.TotalScore, entity.UID, entity.Name);
                                }
                                user.Send(stream.GenericRankingFinalize());

                                for (int x = 0; x < array.Length; x++)
                                {
                                    if (array[x].UID == user.Player.UID)
                                    {
                                        stream.GenericRankingCreate(MsgGenericRanking.Action.QueryCount, RankType.InnerPower, 0, 0, 1);
                                        stream.AddItemGenericRankingCreate((int)(x + 1), (uint)array[x].TotalScore, array[x].UID, array[x].Name);
                                        user.Send(stream.GenericRankingFinalize());

                                        break;
                                    }
                                }
                            }
                            else if (ranktyp >= RankType.DragonChi && ranktyp <= RankType.TurtleChi)
                            {
                                Role.Instance.Chi.ChiPower[] Powers = null;
                                if (ranktyp == RankType.DragonChi)
                                    Powers = Program.ChiRanking.Dragon.Values.ToArray();
                                else if (ranktyp == RankType.PhoenixChi)
                                    Powers = Program.ChiRanking.Phoenix.Values.ToArray();
                                else if (ranktyp == RankType.TigerChi)
                                    Powers = Program.ChiRanking.Tiger.Values.ToArray();
                                else if (ranktyp == RankType.TurtleChi)
                                    Powers = Program.ChiRanking.Turtle.Values.ToArray();

                                if (Powers == null) return;

                                int offset = Page * max;
                                int count = Math.Min(max, Powers.Length);

                                stream.GenericRankingCreate(Action.Ranking, OldRank, Role.Instance.ChiRank.File_Size, Page, count);

                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= Powers.Length)
                                        break;
                                    var entity = Powers[x + offset];
                                    stream.AddItemGenericRankingCreate((int)entity.Rank, (uint)entity.Score, entity.UID, entity.Name);
                                }
                                user.Send(stream.GenericRankingFinalize());
                            }
                            else if (ranktyp >= RankType.RoseFairy && ranktyp <= RankType.TulipFairy)
                            {


                                Role.Instance.Flowers.Flower[] Powers = null;
                                if (ranktyp == RankType.RoseFairy)
                                    Powers = Program.GirlsFlowersRanking.Snapshot(Game.MsgServer.MsgFlower.FlowersType.Rouse);
                                else if (ranktyp == RankType.OrchidFairy)
                                    Powers = Program.GirlsFlowersRanking.Snapshot(Game.MsgServer.MsgFlower.FlowersType.Orchids);
                                else if (ranktyp == RankType.LilyFairy)
                                    Powers = Program.GirlsFlowersRanking.Snapshot(Game.MsgServer.MsgFlower.FlowersType.Lilies);
                                else if (ranktyp == RankType.TulipFairy)
                                    Powers = Program.GirlsFlowersRanking.Snapshot(Game.MsgServer.MsgFlower.FlowersType.Tulips);

                                if (Powers == null) return;
                                int offset = Page * max;
                                int count = Math.Min(max, Powers.Length);


                                stream.GenericRankingCreate(Action.Ranking, ranktyp, Role.Instance.Flowers.FlowerRanking.File_Size, Page, count);

                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= Powers.Length)
                                        break;
                                    var entity = Powers[x + offset];
                                    stream.AddItemGenericRankingCreate(entity.Rank, entity.Amount, entity.UID, entity.Name);
                                }

                                user.Send(stream.GenericRankingFinalize());

                            }
                            else if (ranktyp >= RankType.Kiss && ranktyp <= RankType.Jade)
                            {

                                Role.Instance.Flowers.Flower[] Powers = null;
                                if (ranktyp == RankType.Kiss)
                                    Powers = Program.BoysFlowersRanking.Snapshot(Game.MsgServer.MsgFlower.FlowersType.Rouse);
                                else if (ranktyp == RankType.Tins)
                                    Powers = Program.BoysFlowersRanking.Snapshot(Game.MsgServer.MsgFlower.FlowersType.Orchids);
                                else if (ranktyp == RankType.Love)
                                    Powers = Program.BoysFlowersRanking.Snapshot(Game.MsgServer.MsgFlower.FlowersType.Lilies);
                                else if (ranktyp == RankType.Jade)
                                    Powers = Program.BoysFlowersRanking.Snapshot(Game.MsgServer.MsgFlower.FlowersType.Tulips);

                                if (Powers == null) return;
                                int offset = Page * max;
                                int count = Math.Min(max, Powers.Length);

                                stream.GenericRankingCreate(Action.Ranking, ranktyp, Role.Instance.Flowers.FlowerRanking.File_Size, Page, count);

                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= Powers.Length)
                                        break;
                                    var entity = Powers[x + offset];
                                    stream.AddItemGenericRankingCreate(entity.Rank, entity.Amount, entity.UID, entity.Name);
                                }

                                user.Send(stream.GenericRankingFinalize());

                            }
                            break;
                        }
                }

            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}
