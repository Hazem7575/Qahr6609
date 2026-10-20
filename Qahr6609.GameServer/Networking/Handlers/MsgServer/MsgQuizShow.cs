using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
   /// <summary>
   /// MsgQuizShow (2068), layout of the 6609 client (CMsgQuizShow):
   ///   [4] byte action, [6] ushort, [8] ushort, [10] ushort, [12] ushort, [14] ushort, [24] string list.
   /// Client -> server: action 2 = answer ([6] question number, [8] answer 1-4), action 5 = quit the quiz.
   /// Server -> client: action 1 = question, action 3 = final result (see MsgTournaments.MsgQuizShow).
   /// </summary>
   public static class MsgQuizShow
    {
       public enum Action : byte
       {
           Question = 1,
           Answer = 2,
           Result = 3,
           Quit = 5
       }
       // the client builds this packet with 44 bytes; never send less
       private const int MinSize = 44;

       public static unsafe ServerSockets.Packet QuizShowCreate(this ServerSockets.Packet stream, Action type, ushort Param1, ushort Param2, ushort Param3
           , ushort Param4, ushort Param5, string[] texts)
       {
           stream.InitWriter();
           stream.Write((byte)type);
           stream.Write((byte)0);
           stream.Write(Param1);//6
           stream.Write(Param2);//8
           stream.Write(Param3);//10
           stream.Write(Param4);//12
           stream.Write(Param5);//14
           stream.ZeroFill(8);//16-23
           if (texts != null)
               stream.Write(texts);//24
           else
               stream.Write((byte)0);
           if (stream.Position < MinSize)
               stream.ZeroFill(MinSize - stream.Position);
           stream.Finalize(GamePackets.QuizShow);
           return stream;
       }

       [PacketAttribute(GamePackets.QuizShow)]
       private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
       {
           byte type = stream.ReadUInt8();
           stream.ReadUInt8();
           ushort questionNumber = stream.ReadUInt16();
           ushort answer = stream.ReadUInt16();
           var quiz = MsgTournaments.MsgSchedules.QuizShow;
           if (quiz == null)
               return;
           switch ((Action)type)
           {
               case Action.Answer:
                   {
                       quiz.Answer(user, questionNumber, answer);
                       break;
                   }
               case Action.Quit:
                   {
                       quiz.Quit(user);
                       break;
                   }
           }
       }
    }
}
