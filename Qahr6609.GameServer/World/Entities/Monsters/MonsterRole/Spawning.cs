using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.MsgInterServer.Packets;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgTournaments;
using Conquer6609.Database;

namespace Conquer6609.Game.MsgMonster
{
    public unsafe partial class MonsterRole
    {
        public bool CanRespawn(Role.GameMap map)
        {
            Extensions.Time32 Now = Extensions.Time32.Now;
            if (Now > RespawnStamp)
            {
                if (Boss == 1) return false;
                if (!map.MonsterOnTile(RespawnX, RespawnY))
                {
                    return true;
                }
            }
            return false;

        }
        public void Respawn(bool SendEffect = true)
        {
            using (var rev = new ServerSockets.RecycledPacket())
            {
                var stream = rev.GetStream();

                ClearFlags(false);




                HitPoints = (uint)Family.MaxHealth;
                State = MobStatus.Idle;

                Game.MsgServer.ActionQuery action;

                action = new MsgServer.ActionQuery()
                {
                    ObjId = UID,
                    Type = MsgServer.ActionType.RemoveEntity
                };

                Send(stream.ActionCreate(&action));

                Send(GetArray(stream, false));

                if (SendEffect)
                {
                    action.Type = ActionType.ReviveMonster;
                    Send(stream.ActionCreate(&action));
                }



                if (Family.MaxHealth > ushort.MaxValue)
                {
                    Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, UID, 2);
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, Family.MaxHealth);
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, HitPoints);
                    Send(Upd.GetArray(stream));
                }

            }
        }
        public void AddFadeAway(int time, Role.GameMap map)
        {
            if (!Alive)
            {
                Extensions.Time32 timer = new Extensions.Time32(time);
                if (timer > DeadStamp.AddSeconds(5))
                {
                    if (AddFlag(MsgServer.MsgUpdate.Flags.FadeAway, Role.StatusFlagsBigVector32.PermanentFlag, true))
                    {
                        FadeAway = timer;

                    }
                }
            }
        }
        public unsafe bool RemoveView(int time, Role.GameMap map)
        {
            if (ContainFlag(MsgServer.MsgUpdate.Flags.FadeAway) && State != MobStatus.Respawning)
            {
                Extensions.Time32 timer = new Extensions.Time32(time);
                if (timer > FadeAway.AddSeconds(3))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        ActionQuery action;

                        action = new ActionQuery()
                        {
                            ObjId = UID,
                            Type = ActionType.RemoveEntity
                        };

                        Send(stream.ActionCreate(&action));
                    }

                    State = MobStatus.Respawning;

                    map.View.MoveTo<Role.IMapObj>(this, RespawnX, RespawnY);

                    X = RespawnX;
                    Y = RespawnY;
                    Target = null;

                    return true;
                }
            }
            return false;
        }
    }
}
