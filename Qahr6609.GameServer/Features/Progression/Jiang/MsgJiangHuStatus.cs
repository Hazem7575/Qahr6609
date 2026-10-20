using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet JiangHuStatusCreate(this ServerSockets.Packet stream, string Name
            , byte Stage = 0, byte Talent = 0, uint Timer = 0, ulong StudyPoints = 0, uint FreeTimeToday =0
            , byte FreeTimeTodeyUsed = 0, uint RoundBuyPoints = 0, ICollection<Role.Instance.JiangHu.Stage> array = null)
        {
            stream.InitWriter();

            stream.Write(Name, 16);
            stream.Write((byte)((Stage == 0) ? 1 : Stage));
            stream.Write(Talent);
            stream.Write(Timer);
            stream.Write((byte)0);
            stream.Write(StudyPoints);
            stream.Write(FreeTimeToday);
            stream.Write(9999999);
            stream.Write(FreeTimeTodeyUsed);
            stream.Write(RoundBuyPoints);

            if (array != null)
            {
                foreach (var obj in array)
                {
                    if (obj.Activate)
                    {
                        foreach (var star in obj.ArrayStars)
                        {
                            stream.Write(star.UID);
                        }
                    }
                }
            }

            stream.Finalize(GamePackets.JiangHuStatus);

            return stream;
        }
    }
}
