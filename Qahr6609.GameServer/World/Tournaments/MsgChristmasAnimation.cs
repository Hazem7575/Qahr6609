using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgChristmasAnimation
    {
        public class Reindeer
        {
            public ushort StartX = 0;
            public ushort StartY = 0;

            public ushort MoveToX = 0;
            public ushort MoveToY = 0;
            public int CanDrop = 0;
            public DateTime MoveStamp = new DateTime();
            public Role.StaticRole Role;
            public Reindeer(ServerSockets.Packet stream, ushort MapID, ushort X, ushort Y, ushort movetox, ushort movetoy, uint mesh = 0, string Name = "")
            {
                StartX = X;
                StartY = Y;
                MoveToX = movetox;
                MoveToY = movetoy;
              
                Role = new Role.StaticRole(X, Y, Name != "" ? Name : "Reindeer", mesh != 0 ? mesh : 297);
                Role.Map = MapID;
                Role.GMap.AddStaticRole(Role);
                Role.Send(Role.GetArray(stream, false));
                Role.AddFlag(MsgServer.MsgUpdate.Flags.Ride, 9999999, true);
            }
            public bool InitMove(bool Santa)
            {
                if (DateTime.Now > MoveStamp.AddMilliseconds(300))
                {
                    MoveStamp = DateTime.Now;
                 
                    bool move = Role.MoveTo(MoveToX, MoveToY);
                    if (Santa && move)
                    {
                        if (CanDrop % 3 == 0)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Role.SendString(stream, MsgStringPacket.StringID.Effect, "ssch_wlhd_hit");
                                DropItem(Role.X, Role.Y, stream);
                            }
                        }
                        CanDrop += 1;
                    }
                    if (IsMidle() && !Santa)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            DropItem(Role.X, Role.Y, stream);
                        }
                    }
                    return move;

                }
                return true;
            }
            public unsafe void Remove(ServerSockets.Packet stream)
            {
                var action = new ActionQuery()
                {
                    ObjId = Role.UID,
                    Type = ActionType.RemoveEntity
                };
                Role.Send(stream.ActionCreate(&action));
                Role.GMap.RemoveStaticRole(Role);
            }
            public bool IsMidle()
            {
                return Conquer6609.Role.Core.GetDistance(StartX, StartY, MoveToX, MoveToY) / 2 == StartX - Role.X; 
            }
            public void DropItem(ushort x, ushort y, ServerSockets.Packet stream)
            {
                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                uint Itemid = 3000316;
                DataItem.ITEM_ID = Itemid;
                var DBItem = Database.Server.ItemsBase[Itemid];
                DataItem.Durability = DBItem.Durability;
                DataItem.MaximDurability = DBItem.Durability;
                DataItem.Color = Conquer6609.Role.Flags.Color.Red;
                if (Role.GMap.AddGroundItem(ref x, ref y))
                {
                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, x, y, MsgFloorItem.MsgItem.ItemType.Item, 0, 0, Role.Map
                        , 0, false, Role.GMap);
                    if (Role.GMap.EnqueueItem(DropItem))
                    {
                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                    }
                }
            }
        }

        public ProcesType Process { get; set; }


        public Reindeer[] Reindeers = new Reindeer[1];

        public MsgChristmasAnimation()
        {
            Process = ProcesType.Dead;


        }

        public int CountReindeers = 1;
        public int CountGroups = 1;

        public bool PrepareFinish = false;

        public IDisposable Subscribe;
        public void Start()
        {
            if (Process == ProcesType.Dead)
            {
                CountReindeers = 1;
                CountGroups = 1;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Reindeers = new Reindeer[10];
                    ReindeerOne = Reindeers[CountReindeers++] = new Reindeer(stream, 1002, 319, 299, 308, 299);
                    ReindeerTwo = Reindeers[CountReindeers++] = new Reindeer(stream, 1002, 308, 299, 319, 299);
                    foreach (var user in Database.Server.GamePoll.Values)
                    {
                        user.Player.SendString(stream, MsgStringPacket.StringID.Sound, false, "sound/wind.wav", "1");
                    }
                }
                MsgSchedules.SendInvitation("ChristmasAnimation Event", "CPs,Souls,VIP6", 304, 298, 1002, 0, 60);
                Subscribe = ThreadPoll.Subscribe(CheckUp, 100);
                Process = ProcesType.Alive;
            }
            
        }
        public Reindeer Santa;
        public Reindeer ReindeerOne;
        public Reindeer ReindeerTwo;
        public DateTime InitFinish = new DateTime();
        public void Finish()
        {
            if (Process == ProcesType.Alive)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var obj in Reindeers)
                    {
                        if (obj != null)
                        {
                            obj.Remove(stream);
                        }
                    }
                    Santa.Remove(stream);
                }
                PrepareFinish = false;
                Subscribe.Dispose();
                Process = ProcesType.Dead;
            }
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Alive)
            {
                if (CheckNewReindeers(ReindeerOne, ReindeerTwo))
                {
                    if (CountGroups >= 4)
                    {
                        if (CountGroups >= 5)
                        {
                            if (Santa.InitMove(true) == false)
                            {
                                if (PrepareFinish == false)
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        foreach (var obj in Reindeers)
                                        {
                                            if (obj != null)
                                            {
                                                obj.Role.SendString(stream, MsgStringPacket.StringID.Effect, "accession"+Program.GetRandom.Next(1,5)+"");
                                            }
                                        }
                                    }
                                    PrepareFinish = true;
                                }
                                if (DateTime.Now > InitFinish.AddSeconds(8))
                                {
                                    Finish();
                                }
                            }
                        }
                        else
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Santa = new Reindeer(stream, 1002, 313, 284, 313, 301, 298, "SantaReindeer");
                                Santa.Role.SendString(stream, MsgStringPacket.StringID.Effect, "SuperXp-4");
                                CountGroups += 1;
                                InitFinish = DateTime.Now;
                            }
                        }
                    }
                    else
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            ReindeerOne = Reindeers[CountReindeers++] = new Reindeer(stream, 1002, 319, (ushort)(299 - 4 * CountGroups), 308, (ushort)(299 - 4 * CountGroups));
                            ReindeerTwo = Reindeers[CountReindeers++] = new Reindeer(stream, 1002, 308, (ushort)(299 - 4 * CountGroups), 319, (ushort)(299 - 4 * CountGroups));
                            CountGroups += 1;
                        }
                    }
                }
            }
        }
        public bool CheckNewReindeers(Reindeer one, Reindeer two)
        {
            bool next = one.InitMove(false);
            if (next)
                next = two.InitMove(false);
            return !next;
        }
    }
}
