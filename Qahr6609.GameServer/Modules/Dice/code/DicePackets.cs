using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Conquer6609.Modules.Dice
{
    /// <summary>3285 CMsgBetDiceOpt (client Conquer.exe: PB parse 0x8fca90, build 0x8fc80d, Process 0x8fccb5).
    ///   1 Type, 2 player uid, 3 string (unused), 4 cell / sub-state, 5 money type / dice, 6 int64 amount / seconds, 7 int64 (unused).
    /// Client -> server: {1:0} join (every time the window is shown), {1:1,4:cell,5:money type,6:amount} bet,
    ///   {1:3} leave (close button).
    /// Server -> client: {1:1,2:uid,4:cell,5:money type,6:amount} a bet (the client adds it to the cell; uid = own: also to
    ///   the own stake and the "Council betting" box), {1:2,4:0,6:seconds+1} new round: clears the table and starts the
    ///   alarm clock (at most 50 s), {1:2,4:1,5:d1+10*d2+100*d3} result: the dice roll, {1:3} clear + the client asks 3286 {1:0}.</summary>
    [ProtoContract]
    public sealed class DiceOpt
    {
        [ProtoMember(1, IsRequired = true)] public uint Type;
        [ProtoMember(2)] public uint Uid;
        [ProtoMember(3)] public string Text;
        [ProtoMember(4)] public uint Param;
        [ProtoMember(5)] public uint Value;
        [ProtoMember(6)] public long Amount;
        [ProtoMember(7)] public long Extra;
    }

    /// <summary>CBetInfoPB: 1 varint, 2 varint, 3 int64 (client parse 0x8f89d5).</summary>
    [ProtoContract]
    public sealed class DiceBetInfo
    {
        [ProtoMember(1, IsRequired = true)] public uint Cell;
        [ProtoMember(2, IsRequired = true)] public uint Value;
        [ProtoMember(3, IsRequired = true)] public long Amount;
    }

    /// <summary>3286 CMsgBetDiceList (client PB parse 0x8f9081, build 0x8f885c, Process 0x8f8dd7). 1 Type, 2 repeated CBetInfoPB.
    /// Client -> server: {1:0} Refresh button (the client cleared its table: send every cell), {1:1} the "Lottery" window opened.
    /// Server -> client: {1:0,[cell, money type, amount]} adds to the cells, {1:1,[_, d1+10*d2+100*d3, yyyyMMddHHmmss]} lottery
    ///   record (replaces the list), {1:2,[cell, money type, amount won]} own winnings of the round: "Winning" list, win effect,
    ///   "Congrats! You won ..." (icon glow when the window is hidden), {1:3} the round was lost ("you won nothing").</summary>
    [ProtoContract]
    public sealed class DiceList
    {
        [ProtoMember(1, IsRequired = true)] public uint Type;
        [ProtoMember(2)] public List<DiceBetInfo> Items = new List<DiceBetInfo>();
    }

    public static class DicePackets
    {
        public const ushort MsgBetDiceOpt = 3285, MsgBetDiceList = 3286;

        public static ServerSockets.Packet Create(ServerSockets.Packet stream, DiceOpt obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(MsgBetDiceOpt);
            return stream;
        }

        public static ServerSockets.Packet Create(ServerSockets.Packet stream, DiceList obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(MsgBetDiceList);
            return stream;
        }

        [Packet(MsgBetDiceOpt)]
        static void OnOpt(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null || !client.FullLoading) return;
            DiceOpt msg;
            try { msg = stream.ProtoBufferDeserialize(new DiceOpt()); }
            catch { return; }
            if (msg != null) DiceGame.HandleOpt(client, msg);
        }

        [Packet(MsgBetDiceList)]
        static void OnList(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null || !client.FullLoading) return;
            DiceList msg;
            try { msg = stream.ProtoBufferDeserialize(new DiceList()); }
            catch { return; }
            if (msg != null) DiceGame.HandleList(client, msg);
        }
    }
}
