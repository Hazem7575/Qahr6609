/*using Extensions;
using Conquer6609.Client;
using Conquer6609.Game.MsgServer;
using Conquer6609.Role;
using Conquer6609.ServerSockets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Conquer6609.Game.MsgTournaments
{
  public class MsgTConquerWar 
  {
    public const uint RewardConquerPoints = 25000;

        public ProcesType Process { get; set; }
        public DateTime StartTimer = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public uint Secounds = 60;
        public Role.GameMap Map;
        public uint DinamicMap = 0;
        public KillerSystem KillSystem;
        public TournamentType Type { get; set; }
        public TConquerWar(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }

public    TConquerWar()
    {
        // TODO: Complete member initialization
    }

    public static void Load()
    {
      MsgTConquerWar.Furnitures.Add(SobNpc.StaticMesh.Pole, Conquer6609.Database.Server.ServerMaps[(uint) MsgTConquerWar.MapID].View.GetMapObject<SobNpc>(MapObjectType.SobNpc, 3897U, (Predicate<IMapObj>) null));
    }

    public static void Start()
    {
      MsgTConquerWar.RoundOwner = (MsgTConquerWar.Info) null;
      MsgTConquerWar.EmperrosList = new SafeDictionary<uint, MsgTConquerWar.Info>();
      MsgTConquerWar.Proces = ProcesType.Alive;
      MsgSchedules.SendInvitation("TConquerWar", "CPS", (ushort) 293, (ushort) 360, (ushort) 1002, (ushort) 0, 60, MsgStaticMessage.Messages.None);
      MsgSchedules.SendSysMesage("TConquerWar has been begun..", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red, false);
    }

    public static void End()
    {
      if (MsgTConquerWar.Proces != ProcesType.Alive)
        return;
      MsgTConquerWar.Proces = ProcesType.Dead;
      MsgSchedules.SendSysMesage(MsgTConquerWar.RoundOwner == null ? "TConquerWar War has ended and there is no winner" : MsgTConquerWar.RoundOwner.Name + ", He is the winner of TConquerWar War.", MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red, false);
      MsgTConquerWar.EmperrosList.Clear();
    }

    public static void CheckUP()
    {
      using (RecycledPacket recycledPacket = new RecycledPacket())
      {
        Packet stream = recycledPacket.GetStream();
        DateTime now = DateTime.Now;
        if (MsgTConquerWar.Proces == ProcesType.Dead && now.Hour == 15)
          MsgTConquerWar.Start();
        if (MsgTConquerWar.Proces != ProcesType.Alive)
          return;
        if (now.Minute == 30 && now.Second <= 2)
          Program.SendGlobalPackets.Enqueue(new MsgMessage("TConquerWar War will ended after 30 Minutes.", MsgMessage.MsgColor.orange, MsgMessage.ChatMode.SlideCrosTheServer).GetArray(stream, 0U));
        if (now.Hour == 15 && now.Second <= 2)
          MsgTConquerWar.End();
      }
    }

    public static void Reset(Packet stream)
    {
      MsgTConquerWar.EmperrosList = new SafeDictionary<uint, MsgTConquerWar.Info>();
      foreach (SobNpc sobNpc in MsgTConquerWar.Furnitures.Values)
        sobNpc.HitPoints = sobNpc.MaxHitPoints;
      SobNpc Pole = MsgTConquerWar.Furnitures[SobNpc.StaticMesh.Pole];
      GameClient[] array = Conquer6609.Database.Server.GamePoll.Values.Where<GameClient>((Func<GameClient, bool>) (u => Core.GetDistance(u.Player.X, u.Player.Y, Pole.X, Pole.Y) <= (short) 19)).ToArray<GameClient>();
      if (array == null)
        return;
      foreach (GameClient gameClient in array)
      {
        MsgUpdate msgUpdate = new MsgUpdate(stream, Pole.UID, 2U);
        stream = msgUpdate.Append(stream, MsgUpdate.DataType.Mesh, (long) Pole.Mesh);
        stream = msgUpdate.Append(stream, MsgUpdate.DataType.Hitpoints, (long) Pole.HitPoints);
        stream = msgUpdate.GetArray(stream);
        gameClient.Send(stream);
        if (Pole.Mesh == SobNpc.StaticMesh.Pole)
          gameClient.Send(Pole.GetArray(stream, false));
      }
    }

    public static void FinishRound(Packet stream)
    {
      MsgTConquerWar.SortScores(true);
      MsgTConquerWar.Furnitures[SobNpc.StaticMesh.Pole].Name = MsgTConquerWar.RoundOwner.Name;
      MsgSchedules.SendSysMesage("The Round Ownered by " + MsgTConquerWar.RoundOwner.Name + ".", MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red, false);
      MsgTConquerWar.Reset(stream);
    }

    public static void UpdateScore(Packet stream, uint Score, Player Player)
    {
      if (!MsgTConquerWar.EmperrosList.ContainsKey(Player.UID))
        MsgTConquerWar.EmperrosList.Add(Player.UID, new MsgTConquerWar.Info()
        {
          Name = Player.Name,
          UID = Player.UID,
          Score = Score
        });
      else
        MsgTConquerWar.EmperrosList[Player.UID].Score += Score;
      MsgTConquerWar.SortScores(false);
      if (MsgTConquerWar.Furnitures[SobNpc.StaticMesh.Pole].HitPoints >= 1)
        return;
      MsgTConquerWar.FinishRound(stream);
    }

    private static void SortScores(bool getwinner = false)
    {
      if (MsgTConquerWar.Proces == ProcesType.Dead)
        return;
      MsgTConquerWar.Info[] array = ((IEnumerable<MsgTConquerWar.Info>) MsgTConquerWar.EmperrosList.Values.ToArray<MsgTConquerWar.Info>()).OrderByDescending<MsgTConquerWar.Info, uint>((Func<MsgTConquerWar.Info, uint>) (p => p.Score)).ToArray<MsgTConquerWar.Info>();
      for (int index = 0; index < array.Length; ++index)
      {
        MsgTConquerWar.Info info = array[index];
        if (index == 0 && getwinner)
          MsgTConquerWar.RoundOwner = info;
        using (RecycledPacket recycledPacket = new RecycledPacket())
          MsgTConquerWar.SendMapPacket(new MsgMessage("No " + (index + 1).ToString() + ". " + info.Name + " (" + (object) info.Score + ")", MsgMessage.MsgColor.yellow, index == 0 ? MsgMessage.ChatMode.FirstRightCorner : MsgMessage.ChatMode.ContinueRightCorner).GetArray(recycledPacket.GetStream(), 0U));
        if (index == 4)
          break;
      }
    }

    public static void SendMapPacket(Packet packet)
    {
      foreach (GameClient gameClient in Conquer6609.Database.Server.ServerMaps[(uint) MsgTConquerWar.MapID].Values)
        gameClient.Send(packet);
    }

    public class Info
    {
      public uint UID;
      public uint Score;
      public string Name;
    }
  }
}*/
