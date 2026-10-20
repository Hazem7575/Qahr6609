using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgPowerArena
    {

        public void CheckMonstersMap(Role.Player user)
        {
            if (Process == ProcesType.Alive)
            {
                user.DefeatedArenaGuardians += 1;
                switch (user.Map)
                {
                    case 1770:
                        {
                            if (user.DefeatedArenaGuardians >= 30)
                                user.Owner.SendSysMesage("You have flattened enough guardians to claim experience from the Agate now.", MsgServer.MsgMessage.ChatMode.System);
                            break;
                        }
                    case 1771:
                        {
                            if (user.DefeatedArenaGuardians >= 45)
                                user.Owner.SendSysMesage("You have flattened enough guardians to claim experience from the Agate now.", MsgServer.MsgMessage.ChatMode.System);
                            break;
                        }
                    case 1772:
                        {
                            if (user.DefeatedArenaGuardians >= 60)
                                user.Owner.SendSysMesage("You have flattened enough guardians to claim experience from the Agate now.", MsgServer.MsgMessage.ChatMode.System);
                            break;
                        }
                    case 1773:
                        {
                            if (user.DefeatedArenaGuardians >= 35)
                                user.Owner.SendSysMesage("You have flattened enough guardians to claim experience from the Agate now.", MsgServer.MsgMessage.ChatMode.System);
                            break;
                        }
                    case 1774:
                        {
                            if (user.DefeatedArenaGuardians >= 45)
                                user.Owner.SendSysMesage("You have flattened enough guardians to claim experience from the Agate now.", MsgServer.MsgMessage.ChatMode.System);
                            break;
                        }
                    case 1775:
                        {
                            if (user.DefeatedArenaGuardians >= 60)
                                user.Owner.SendSysMesage("You have flattened enough guardians to claim experience from the Agate now.", MsgServer.MsgMessage.ChatMode.System);
                            break;
                        }
                    case 1777:
                        {
                            if (user.DefeatedArenaGuardians >= 90)
                                user.Owner.SendSysMesage("You have flattened enough guardians to claim experience from the Agate now.", MsgServer.MsgMessage.ChatMode.System);
                            break;
                        }
                }
            }
        }
        public bool InPowerArena(Client.GameClient user)
        {
            return user.Player.Map == 1770 || user.Player.Map == 1771 || user.Player.Map == 1772 || user.Player.Map == 1773 || user.Player.Map == 1774 || user.Player.Map == 1775 || user.Player.Map == 1777;
        }
        /* 
         * Power Arena will be opened in 5 minutes. Please get ready for that!
           The Power Arena is open! Find Arena Manager Wang in Twin City (465,234) to sign up for the Arena.
           Power Arena will be closed in 5 minutes. Go and claim your reward now!
           You have flattened enough guardians to claim experience from the Agate now.
         * The Power Arena is closed, please come tomorrow!
         * 
         */

        public ProcesType Process { get; set; }
        public int StartMinutes = 5;
        public DateTime StartTimer = new DateTime();
        public DateTime PrepareStart = new DateTime();


        public MsgPowerArena()
        {
            Process = ProcesType.Dead;
        }
        public void Start()
        {
            if (Process == ProcesType.Dead)
            {
                // Official (client Activity.ini): open daily 13:00-14:00 and 20:00-21:00; started 5 minutes before.
                // Reset the countdown: the previous session's closing countdown left it at 0.
                StartMinutes = 5;
                MsgSchedules.SendInvitation("PowerArena", "EXP,Refinary..etc", 338, 138, 1002, 0, 60, MsgServer.MsgStaticMessage.Messages.PowerArena);
                MsgSchedules.SendSysMesage("Power Arena will be opened in " + StartMinutes + " minutes. Please get ready for that!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                PrepareStart = DateTime.Now;
                StartMinutes = 4;
                Process = ProcesType.Idle;
            }
        }
        public void Finish()
        {
            if (Process == ProcesType.Alive)
            {
                MsgSchedules.SendSysMesage("The Power Arena is closed. It opens every day at 13:00 and 20:00.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.yellow);
                Process = ProcesType.Dead;
            }
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Idle)
            {
                if (DateTime.Now > PrepareStart.AddSeconds(60))
                {
                    if (StartMinutes == 0)
                    {
                        MsgSchedules.SendSysMesage("The Power Arena is open! Find Arena Manager Wang in Twin City (465,234) to sign up for the Arena.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        Process = ProcesType.Alive;
                        StartTimer = DateTime.Now;
                        StartMinutes = 5;
                    }
                    else
                    {
                        MsgSchedules.SendSysMesage("Power Arena will be opened in " + StartMinutes + " minutes. Please get ready for that!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        PrepareStart = DateTime.Now;
                        StartMinutes -= 1;
                    }
                }
            }
            if (Process == ProcesType.Alive)
            {
                DateTime Now = DateTime.Now;
                if (Now > StartTimer.AddMinutes(60))
                    Finish();
                if (Now > StartTimer.AddMinutes(55))
                {
                    if (DateTime.Now > PrepareStart.AddSeconds(60))
                    {
                        MsgSchedules.SendSysMesage("Power Arena will be closed in " + StartMinutes + " minutes. Go and claim your reward now!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        PrepareStart = DateTime.Now;
                        StartMinutes -= 1;

                    }
                }
            }
        }
    }
}
