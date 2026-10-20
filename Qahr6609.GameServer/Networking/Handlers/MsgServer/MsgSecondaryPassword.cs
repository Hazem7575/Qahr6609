using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Conquer6609.Game.MsgServer
{
  public  static class MsgSecondaryPassword
    {
      public enum ActionID : uint
      {
          VerifiedPassword = 1,
          ForgetPassword = 2, 
          SetNewPass = 3, 
          SendInformation = 4,
          PasswordCorrect = 5, 
          PasswordWrong = 6
      }

      public static unsafe ServerSockets.Packet SecondaryPasswordCreate(this ServerSockets.Packet stream, ActionID Type, uint OldPassowrd, uint NewPassword)
      {
          stream.InitWriter();
          stream.Write((uint)Type);
          stream.Write(OldPassowrd);
          stream.Write(NewPassword);

          stream.Finalize(GamePackets.SecondaryPassword);
          return stream;
      }
      public static unsafe void GetSecondaryPassword(this ServerSockets.Packet stream, out ActionID Type, out uint OldPassowrd, out uint NewPassword)
      {
         Type = (ActionID)stream.ReadUInt32();
          OldPassowrd = stream.ReadUInt32();
          NewPassword = stream.ReadUInt32();
      }
      // Wrong secondary-password attempts per character UID: MaxPinFailures wrong PINs
      // disconnect the player and block further PIN checks for PinLockMinutes.
      public const int MaxPinFailures = 5;
      public const int PinLockMinutes = 10;
      private sealed class PinAttempts
      {
          public int Failures;
          public DateTime LockedUntil;
      }
      private static readonly ConcurrentDictionary<uint, PinAttempts> pinAttempts = new ConcurrentDictionary<uint, PinAttempts>();

      internal static bool PinLocked(Client.GameClient user)
      {
          PinAttempts a;
          if (!pinAttempts.TryGetValue(user.Player.UID, out a)) return false;
          lock (a)
          {
              if (a.LockedUntil > DateTime.Now) return true;
              if (a.LockedUntil != DateTime.MinValue && a.Failures == 0)
              {
                  PinAttempts removed;
                  pinAttempts.TryRemove(user.Player.UID, out removed);
              }
              return false;
          }
      }

      private static void RejectLocked(Client.GameClient user, ServerSockets.Packet stream)
      {
          user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordWrong, 0, 0));
          Conquer6609.Game.MsgLoader.HazemShield.SendForceClose(user, "Secondary password", "Too many wrong secondary passwords. Try again in " + PinLockMinutes + " minutes.");
      }

      internal static void PinFailed(Client.GameClient user)
      {
          var a = pinAttempts.GetOrAdd(user.Player.UID, k => new PinAttempts());
          bool lockNow = false;
          lock (a)
          {
              a.Failures++;
              if (a.Failures >= MaxPinFailures)
              {
                  a.Failures = 0;
                  a.LockedUntil = DateTime.Now.AddMinutes(PinLockMinutes);
                  lockNow = true;
              }
          }
          if (lockNow)
          {
              Conquer6609.Security.Guard.Audit("pin-lockout", user.Player.UID, user.Player.Name, MaxPinFailures + " wrong secondary passwords");
              Conquer6609.Game.MsgLoader.HazemShield.SendForceClose(user, "Secondary password", "Too many wrong secondary passwords. Try again in " + PinLockMinutes + " minutes.");
          }
      }

      internal static void PinSucceeded(Client.GameClient user)
      {
          PinAttempts removed;
          pinAttempts.TryRemove(user.Player.UID, out removed);
      }

      [PacketAttribute(GamePackets.SecondaryPassword)]
      private static void Process(Client.GameClient user, ServerSockets.Packet stream)
      {
          ActionID Type;
          uint OldPassoword;
          uint NewPassoword;
          stream.GetSecondaryPassword(out Type, out OldPassoword, out NewPassoword);

          switch (Type)
          {
              case ActionID.SetNewPass:
                  {
                      if (user.Player.SecurityPassword == 0)
                      {
                          user.Player.SecurityPassword = NewPassoword;
                          user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 0x0101, 0));
#if Arabic
                            user.SendSysMesage("Successfully set! Please remember your secondary password", MsgMessage.ChatMode.System);
#else
                          user.SendSysMesage("Successfully set! Please remember your secondary password", MsgMessage.ChatMode.System);
#endif
                        
                      }
                      break;
                  }
              case ActionID.SendInformation:
                  {
                      if (user.Player.SecurityPassword == 0)
                      {
                          user.Player.SecurityPassword = NewPassoword;
                          user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 0x0101, 0));
#if Arabic
                           user.SendSysMesage("Successfully set! Please remember your secondary password", MsgMessage.ChatMode.System);
#else
                          user.SendSysMesage("Successfully set! Please remember your secondary password", MsgMessage.ChatMode.System);
#endif
                         
                      }
                      else
                      {
                          if (PinLocked(user))
                          {
                              RejectLocked(user, stream);
                              break;
                          }
                          if (OldPassoword == user.Player.SecurityPassword)
                          {
                              PinSucceeded(user);
                              user.Player.OnReset = 0;
                              user.Player.SecurityPassword = NewPassoword;
                              user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 0x0101, 0));
#if Arabic
                                  user.SendSysMesage("Successfully modified! Please remember your secondary password", MsgMessage.ChatMode.System);
#else
                              user.SendSysMesage("Successfully modified! Please remember your secondary password", MsgMessage.ChatMode.System);
#endif
                          
                          }
                          else
                          {
                              user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordWrong, 1, 0));
                              PinFailed(user);
                          }
                      }

                      break;
                  }
              case ActionID.VerifiedPassword:
                  {
                      if (user.Player.SecurityPassword != 0)
                      {
                          if (PinLocked(user))
                          {
                              RejectLocked(user, stream);
                              break;
                          }
                          if (user.Player.SecurityPassword == NewPassoword)
                          {
                              PinSucceeded(user);
                              user.Player.IsCheckedPass = true;
                              user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 0x0101, 0));
#if Arabic
                                      user.SendSysMesage("Secondary verified!", MsgMessage.ChatMode.System);
#else
                              user.SendSysMesage("Secondary verified!", MsgMessage.ChatMode.System);
#endif
                      
                          }
                          else
                          {
                              user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordWrong, 0, 0));
                              PinFailed(user);
                          }
                      }
                      break;
                  }
              case ActionID.ForgetPassword:
                  {
                      if (user.Player.SecurityPassword != 0)
                      {
                          user.Player.OnReset = 1;
                          user.Player.ResetSecurityPassowrd = DateTime.Now.AddDays(7);
                          user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 0x0101, 0));
#if Arabic
                                  user.SendSysMesage("Your secondary password will be removed on " + user.Player.ResetSecurityPassowrd.ToString("d/M/yyyy (H:mm)") + ".", MsgMessage.ChatMode.System);
                   
#else
                          user.SendSysMesage("Your secondary password will be removed on " + user.Player.ResetSecurityPassowrd.ToString("d/M/yyyy (H:mm)") + ".", MsgMessage.ChatMode.System);
                   
#endif
                     }
                      break;
                  }
          }
      }
    }
}
