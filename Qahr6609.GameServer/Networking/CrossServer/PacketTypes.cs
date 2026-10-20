using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.MsgInterServer
{
   public class PacketTypes
    {
       public const ushort
              ProgramChat = 3010,
           ProgramOnlineInfo = 3011,
           InterServer_NobilityRank = 3100,
           InterServer_TeamRank = 114,
           InterServer_RoleInfo = 113,
           InterServer_SpecialTitles = 112,
           InterServer_CheckTransfer = 111,
           InterServer_Achievement = 110,
           InterServer_Chi = 109,
           InterServer_JiangHu = 108,
           InterServer_InnerPower = 107,
           InterServer_NobilityInfo = 106,
           InterServer_EliteRank = 105,
           InterServer_CreateItem = 104,
           InterServer_ConnectionInfo = 103,
           InterServer_UnionInfo = 102,
           InterServer_GuildInfo = 101,
           InterServer_UnionRanks = 100,
           InterServer_ServerInfo = 99,
           // Pipe v2 (PipeSecurity.cs): 98 = connector's HMAC proof, 95/96 = listener hello / proof.
           InterServer_Auth = 98,
           PipeHello = 95,
           PipeAuthOk = 96,
           RelayClient = 130,
           PipeFrame = 131,
           PipeHeartbeat = 133;

       /// <summary>Frame bytes around an inner packet: header 4 + seq 4 + mac 12 + seal 8.</summary>
       public const int PipeFrameOverhead = 28;
       public const int PipeMacSize = 12;
    }
}
