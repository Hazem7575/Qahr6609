using Conquer6609.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public enum ConquererMode
    {
        MobsKiller = 0,
        BansheeKiller = 1,
        SpookKiller = 2,
        NemsisKiller = 3
    }
    public class MsgDragonIsland
    {
        public Tuple<ConquererMode, string, uint, uint>[] BossConquerers = new Tuple<ConquererMode, string, uint, uint>[4];
        public const ushort MapID = 10137;
        public ProcesType Process { get; set; }
        public Fan SafeArea = new Fan(0, 400, 150, 419, 150, 180);
        public Role.GameMap Map
        {
            get { return Database.Server.ServerMaps[MapID]; }
        }
        public DateTime ScoreStamp = new DateTime();
        public MsgDragonIsland(ProcesType _process)
        {
            Process = _process;
            //Map = Database.Server.ServerMaps[MapID];
            BossConquerers[0] = Tuple.Create<ConquererMode, string, uint, uint>(ConquererMode.MobsKiller, "None", 0, 0);
            BossConquerers[1] = Tuple.Create<ConquererMode, string, uint, uint>(ConquererMode.BansheeKiller, "None", 0, 0);
            BossConquerers[2] = Tuple.Create<ConquererMode, string, uint, uint>(ConquererMode.SpookKiller, "None", 0, 0);
            BossConquerers[3] = Tuple.Create<ConquererMode, string, uint, uint>(ConquererMode.NemsisKiller, "None", 0, 0);
        }
        public bool Attackable(uint Map, ushort X, ushort Y)
        {
            if (Map != MapID)
                return true;
            return SafeArea.IsInFan(X, Y) == true ? false : true;
        }
        private void AnnounceKillers(ServerSockets.Packet stream)
        {
          //  return;
            Game.MsgServer.MsgMessage msg = null;
            {
                msg = new MsgServer.MsgMessage("Rank [" + BossConquerers[0].Item1.ToString() + "] Name [" + BossConquerers[0].Item2 + "] Score [" + BossConquerers[0].Item3.ToString() + "].", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);

                SendMapPacket(msg.GetArray(stream));
            }
            msg = null;
            {
                msg = new MsgServer.MsgMessage("Rank [" + BossConquerers[1].Item1.ToString() + "] Name [" + BossConquerers[1].Item2 + "] Score [" + BossConquerers[1].Item3.ToString() + "].", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

                SendMapPacket(msg.GetArray(stream));
            }
            msg = null;
            {
                msg = new MsgServer.MsgMessage("Rank [" + BossConquerers[2].Item1.ToString() + "] Name [" + BossConquerers[2].Item2 + "] Score [" + BossConquerers[2].Item3.ToString() + "].", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

                SendMapPacket(msg.GetArray(stream));
            }
            msg = null;
            {
                msg = new MsgServer.MsgMessage("Rank [" + BossConquerers[3].Item1.ToString() + "] Name [" + BossConquerers[3].Item2 + "] Score [" + BossConquerers[3].Item3.ToString() + "].", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

                SendMapPacket(msg.GetArray(stream));
            }
            msg = null;
            ScoreStamp = DateTime.Now.AddSeconds(20);
        }
        public void SendMapPacket(ServerSockets.Packet stream)
        {
            foreach (var user in MapPlayers())
                user.Send(stream);
        }
        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => InTournament(p)).ToArray();
        }
        public bool InTournament(Client.GameClient user)
        {
            if (Map == null)
                return false;
            return user.Player.Map == Map.ID;
        }
        public void BossKilled()
        {
            if (Process == ProcesType.Alive)
            {
                Process = ProcesType.Idle;
            }
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream, bool Chasm = false)
        {
            if (user.Player.Level < 120 || user.Player.Reborn < 1)
                return false;

            {
                if (Chasm)
                {
                    user.Teleport((ushort)(455 - Program.GetRandom.Next(0, 5)), (ushort)(479 - Program.GetRandom.Next(0, 5)), 10137);
                }
                else
                {
                    user.Teleport((ushort)(95 - Program.GetRandom.Next(0, 5)), (ushort)(412 - Program.GetRandom.Next(0, 5)), MapID);
                }
                return true;
            }
          
        }
        public void SortKiller(Client.GameClient client, uint Score, ConquererMode Mode)
        {
            if (Process == ProcesType.Alive)
            {
                if (BossConquerers[(int)Mode].Item4 == client.Player.UID)
                {
                    BossConquerers[(int)Mode] = null;
                    BossConquerers[(int)Mode] = new Tuple<ConquererMode, string, uint, uint>(Mode, client.Player.Name, Score, client.Player.UID);
                }
                else
                {
                 
                    if (BossConquerers[(int)Mode].Item3 < Score)
                    {
                        BossConquerers[(int)Mode] = null;
                        BossConquerers[(int)Mode] = new Tuple<ConquererMode, string, uint, uint>(Mode, client.Player.Name, Score, client.Player.UID);
                    }
                }
            }
        }
        public void CheckUp()
        {
            if (DateTime.Now > ScoreStamp)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    AnnounceKillers(stream);
                }
            }
           
         
        }
        public void GivePrizes()
        {
            if (Process == ProcesType.Alive)
                Process = ProcesType.Idle;
           
        }
    }
}
