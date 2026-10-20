using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgTreasureThief : ITournament
    {
        public const ushort
            MapID = 3820;
        public ProcesType Process { get; set; }
        public int CurrentBoxes = 0;
        // Boxes currently placed by the event; a box pays out only once, when it is taken from this set.
        private readonly System.Collections.Concurrent.ConcurrentDictionary<uint, Game.MsgNpc.Npc> Boxes = new System.Collections.Concurrent.ConcurrentDictionary<uint, Game.MsgNpc.Npc>();
        public DateTime StartTimer = new DateTime();
        public DateTime BoxesStamp = new DateTime();
        Role.GameMap _map;
        public Role.GameMap Map
        {
            get
            {
                if (_map == null)
                    _map = Database.Server.ServerMaps[MapID];
                return _map;
            }
        }
        public TournamentType Type { get; set; }
        public MsgTreasureThief(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }
        public bool InTournament(Client.GameClient user)
        {
            return user.Player.Map == MapID;
        }
        public void Open()
        {
            if (Process != ProcesType.Alive)
            {
                Create();
                foreach (var user in Database.Server.GamePoll.Values)
                    user.Player.CurrentTreasureBoxes = 0;
                Process = ProcesType.Alive;
                StartTimer = DateTime.Now.AddMinutes(8);
                BoxesStamp = DateTime.Now.AddSeconds(30);

                MsgSchedules.SendInvitation("TreasureThief", "ConquerPoints,Money,Vip and others treasures", 299, 230, 1002, 0, 60);
             
            }
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.Player.Level < 80)
            {
                user.SendSysMesage("Need to be level 80 at least.");
                return false;
            }
            if (Process == ProcesType.Alive)
            {
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, MapID);
                return true;
            }
            return false;
        }
        private void Create()
        {
            GenerateBoxes();
        }
        private void GenerateBoxes()
        {
            for (int i = CurrentBoxes; i < 10; i++)
            {
                byte rand = (byte)Program.GetRandom.Next(0, 5);
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);

                Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                while (true)
                {
                    np.UID = (uint)Program.GetRandom.Next(10000, 100000);
                    if (Map.View.Contain(np.UID, x, y) == false && !Boxes.ContainsKey(np.UID))
                        break;
                }
                np.NpcType = Role.Flags.NpcType.Talker;
                switch (rand)
                {
                    case 0: np.Mesh = 26586;break;
                    case 1: np.Mesh = 26596; break;
                    case 2: np.Mesh = 26606; break;
                    case 3: np.Mesh = 26616; break;
                    case 4: np.Mesh = 26626; break;
                    default: np.Mesh = 26586; break;
                }
                np.Map = MapID;
                np.X = x;
                np.Y = y;
                Boxes[np.UID] = np;
                Map.AddNpc(np);
            }
            CurrentBoxes = 10;
        }
        // Removes every remaining box from the map when the event ends.
        private void RemoveBoxes()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var key in Boxes.Keys.ToArray())
                {
                    Game.MsgNpc.Npc box;
                    if (Boxes.TryRemove(key, out box))
                        Map.RemoveNpc(box, stream);
                }
            }
            CurrentBoxes = 0;
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Alive)
            {
                FlushScores();
                if (DateTime.Now > StartTimer)
                {

                    MsgSchedules.SendSysMesage("All Players of Treasure Thief Stage 1 has teleported to Stage 2 in Frozen map!", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                   
                    var Map2 = Database.Server.ServerMaps[3954];
                    foreach (var user in Map.Values)
                    {
                        ushort x = 0;
                        ushort y = 0;
                        Map2.GetRandCoord(ref x, ref y);
                        user.Teleport(x, y, 3954);
                    }
                    if (!Map2.ContainMobID(20060))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, Map2, 20060, 61, 65, 5, 5, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                        }
                    }
                    Process = ProcesType.Dead;
                    RemoveBoxes();
                }
                else if (DateTime.Now > BoxesStamp)
                {
                    GenerateBoxes();
                    BoxesStamp = DateTime.Now.AddSeconds(30);
                }
            }
        }
        public void Reward(Client.GameClient user,Game.MsgNpc.Npc npc, ServerSockets.Packet stream)
        {
            // Rewards only while the event runs, only for a box of this event, and only once per box:
            // the box is taken and removed from the map before anything is paid.
            if (Process != ProcesType.Alive || npc == null || user.Player.Map != MapID)
                return;
            Game.MsgNpc.Npc box;
            if (!Boxes.TryRemove(npc.UID, out box) || !object.ReferenceEquals(box, npc))
            {
                if (box != null && !object.ReferenceEquals(box, npc))
                    Boxes.TryAdd(box.UID, box);
                return;
            }
            Map.RemoveNpc(npc, stream);
            CurrentBoxes -= 1;
            if (CurrentBoxes < 0) CurrentBoxes = 0;
            jmp:
            byte rand = (byte)Program.GetRandom.Next(0, 5);
            switch (rand)
            {
                case 0://money
                    {
                        uint value = (uint)Program.GetRandom.Next(1000000, 5000000);
                        user.Player.Money += value;
                        user.Player.SendUpdate(stream, user.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        user.CreateBoxDialog("You've received " + value + " Money.");
                        MsgSchedules.SendSysMesage(user.Player.Name + " got " + value.ToString() + " Money while opening the TreasureBox!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                       
                        break;
                    }
                case 1://experience
                    {
                        if (user.Player.Level == 140)
                            goto jmp;
                        user.GainExpBall(600 * 2, true, Role.Flags.ExperienceEffect.angelwing);

                        MsgSchedules.SendSysMesage(user.Player.Name + " got 2xExpBalls while opening the TreasureBox!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                     
                       break;
                    }
                case 2://cps
                    {
                        uint value = (uint)Program.GetRandom.Next(100, 1000);
                        user.Player.ConquerPoints += value;
                        MsgSchedules.SendSysMesage(user.Player.Name + " got " + value.ToString() + " CPs while opening the TreasureBox!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                        user.CreateBoxDialog("You've received " + value + " ConquerPoints.");
                      
                        break;
                    }
                case 3://dead.
                    {
                        user.Player.Dead(null, user.Player.X, user.Player.Y, 0);
                        MsgSchedules.SendSysMesage(user.Player.Name + " found DEATH! while opening the TreasureBox!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                      
                        break;
                    }
                case 4://item.
                    {
                        uint[] Items = new uint[]
                        {
                            780010,//vip 2 hours
                            Database.ItemType.DragonBall,
                            Database.ItemType.PowerExpBall,
                            3005126,
                            3005127,
                            3005125,
                            3005124,
                            (uint)(700000 + (uint)Role.Flags.Gem.SuperDragonGem),
                            (uint)(700000 + (uint)Role.Flags.Gem.SuperTortoiseGem),
                            Database.ItemType.MeteorScroll,
                            Database.ItemType.MeteorTearPacket,
                            Database.ItemType.Meteor,
                            Database.ItemType.DragonBall,
                            Database.ItemType.MoonBox,
                            720598,
                            729022,
                            729023,
                            723712,
                            723727,
                            723342,
                            1200002,
                            720173,
                            Database.ItemType.DragonBall
                        };
                        uint ItemID = Items[Program.GetRandom.Next(0, Items.Length)];
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            if (user.Inventory.HaveSpace(1))
                                user.Inventory.Add(stream, DBItem.ID);
                            else
                                user.Inventory.AddReturnedItem(stream, DBItem.ID);
                            MsgSchedules.SendSysMesage(user.Player.Name + " got " + DBItem.Name + " while opening the TreasureBox!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                      
                        }
                        break;
                    }

            }
            user.Player.CurrentTreasureBoxes += 1;
            user.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "accession1");

            // [Performance] ScoreboardThrottle=1: the map scoreboard is rebuilt at most once per second from CheckUp
            // instead of on every box (default 0 = original: on every box).
            if (MsgSchedules.ScoreboardThrottle)
                ScoresDirty = true;
            else
                ShuffleGuildScores(stream);

        }
        private volatile bool ScoresDirty;
        private void FlushScores()
        {
            if (!ScoresDirty)
                return;
            ScoresDirty = false;
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    ShuffleGuildScores(rec.GetStream());
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }
        public void ShuffleGuildScores(ServerSockets.Packet stream)
        {
            foreach (var user in Map.Values)
            {
                Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("---Your Score: " + user.Player.CurrentTreasureBoxes + "---", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                
               user.Send(msg.GetArray(stream));
            }
            var array = Map.Values.OrderByDescending(p => p.Player.CurrentTreasureBoxes).ToArray();
            for (int x = 0; x < Math.Min(10, array.Length); x++)
            {
                var element = array[x];

                Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + "- " + element.Player.Name + " Opened " + element.Player.CurrentTreasureBoxes.ToString() + " Boxes!", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
             
                Send(msg.GetArray(stream));
            }
        }
        public void Send(ServerSockets.Packet stream)
        {
            foreach (var user in Map.Values)
                user.Send(stream);
        }
    }
}
