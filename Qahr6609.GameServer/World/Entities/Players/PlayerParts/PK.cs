using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;
using Conquer6609.Database;

namespace Conquer6609.Role
{
    public unsafe partial class Player
    {
        public unsafe void SetPkMode(Flags.PKMode pkmode)
        {
            PreviousPkMode = PkMode;
            PkMode = pkmode;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                ActionQuery action = new ActionQuery()
                {
                    ObjId = UID,
                    dwParam = (uint)PkMode,
                    Type = ActionType.SetPkMode
                };
                Owner.Send(stream.ActionCreate(&action));
            }

        }
        public unsafe void RestorePkMode()
        {
            PkMode = PreviousPkMode;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                ActionQuery action = new ActionQuery()
                {
                    ObjId = UID,
                    dwParam = (uint)PkMode,
                    Type = ActionType.SetPkMode
                };
                Owner.Send(stream.ActionCreate(&action));
            }
        }
        public void AddCursed(int time)
        {
            if (time != 0)
            {
                if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cursed))
                    RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Cursed);

                CursedTimer += time;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, CursedTimer, Game.MsgServer.MsgUpdate.DataType.CursedTimer);
                }
                AddFlag(Game.MsgServer.MsgUpdate.Flags.Cursed, CursedTimer, false, 1);
            }
        }
        public ushort PKPoints
        {
            get { return _pkpoints; }
            set
            {
                _pkpoints = value;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (PKPoints > 99)
                    {
                        RemoveFlag(Game.MsgServer.MsgUpdate.Flags.RedName);
                        AddFlag(Game.MsgServer.MsgUpdate.Flags.BlackName, StatusFlagsBigVector32.PermanentFlag, false, 6 * 60);

                    }
                    else if (PKPoints > 29)
                    {
                        AddFlag(Game.MsgServer.MsgUpdate.Flags.RedName, StatusFlagsBigVector32.PermanentFlag, false, 6 * 60);
                        RemoveFlag(Game.MsgServer.MsgUpdate.Flags.BlackName);
                        SendUpdate(stream, PKPoints, Game.MsgServer.MsgUpdate.DataType.PKPoints);
                    }
                    else if (PKPoints < 30)
                    {
                        RemoveFlag(Game.MsgServer.MsgUpdate.Flags.RedName);
                        RemoveFlag(Game.MsgServer.MsgUpdate.Flags.BlackName);

                    }

                    UpdateFlagOffset();
                    SendUpdate(stream, PKPoints, Game.MsgServer.MsgUpdate.DataType.PKPoints);
                }
            }
        }
    }

}
