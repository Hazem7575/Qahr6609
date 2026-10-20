using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
  public  class MsgSquidwardOctopus
    {

      public int GoldRemain = 0;
      public int SilverRemain = 0;
      public int CopperRemain = 0;

        //12:30-13:29 and 20:30-21:29, Monday - Saturday
      public  ProcesType Process { get; set; }

      public DateTime StartTimer = new DateTime();

      public MsgSquidwardOctopus()
      {
          Process = ProcesType.Dead;
      }
      public void Start()
      {
          if (Process == ProcesType.Dead)
          {
              // Client string 10552: "`Treasure in the Blue` adventure has started..." (Mon-Sat 12:30 and 20:30).
              MsgSchedules.SendInvitation("Treasure in the Blue", "Mounts,Expericence,Refinary", 297, 208, 1002, 0, 60, (MsgServer.MsgStaticMessage.Messages)10552);
              StartTimer = DateTime.Now;
              GoldRemain = 100;
              SilverRemain = 200;
              CopperRemain = 300;

              Process = ProcesType.Alive;
          }
      }
      public void Finish()
      {
          if (Process == ProcesType.Alive)
          {
              MsgSchedules.SendSysMesage("The `Treasure in the Blue` adventure has finished.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.yellow);
              Process = ProcesType.Dead;
          }
      }
      public void CheckUp()
      {
         if (Process == ProcesType.Alive)
          {
              DateTime Now = DateTime.Now;
              if (Now > StartTimer.AddMinutes(60))
                  Finish();
           
          }
      }
    }
}
