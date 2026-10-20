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
        public void StopMining()
        {
            Mining = false;
        }

        public bool IsBoy()
        {
            return Role.Core.IsBoy(Body);
        }
        public bool IsGirl()
        {
            return Role.Core.IsGirl(Body);
        }


        public void ResetEpicTrojan()
        {
            EpicTrojanMrMirrorPrograss = 0;
            StageEpicTrojanQuest = 0;
            EpicTrojanEvilArrayPoints = 0;
            EpicTrojanAbysalStage = 0;
            EpicTrijanKillGhostReaver = 0;
            ChangeEpicTrojan = ChangeArrayEpicTrojan = ChangeMr_MirrorEpicTrojan = ChangeGeneralPakEpicTrojan = 0;
            CanChangeEpicMaterial = CanChangeArrayEpicMaterial = CanChangeMr_MirrorEpicMaterial = CanChangGeneralPakMaterial = 1;
        }
        public string SaveEpicTrojan()
        {
            Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
            writer.Add(EpicTrojanItemChange[0]).Add(EpicTrojanItemChange[1]).Add(StageEpicTrojanQuest)
                .Add(ChangeEpicTrojan).Add(CanChangeEpicMaterial).Add(CanChangeArrayEpicMaterial)
                .Add(ChangeArrayEpicTrojan).Add(EpicTrojanArrayItemChange[0]).Add(EpicTrojanArrayItemChange[1])
                .Add(EpicTrojanEvilArrayPoints).Add(EpicTrojanAbysalStage).Add(EpicTrojanMr_MirrorItemChange[0])
                .Add(EpicTrojanMr_MirrorItemChange[1]).Add(ChangeMr_MirrorEpicTrojan)
                .Add(CanChangeMr_MirrorEpicMaterial)

                .Add(EpicTrojanGeneralPakItemChange[0]).Add(EpicTrojanGeneralPakItemChange[1])
                .Add(ChangeGeneralPakEpicTrojan).Add(CanChangGeneralPakMaterial)
                .Add(EpicTrojanMrMirrorPrograss);
            return writer.Close();
        }
        public void LoadEpicTrojan(string line)
        {
            if (line == null)
                return;
            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(line, '/');
            EpicTrojanItemChange[0] = reader.Read((uint)0);
            EpicTrojanItemChange[1] = reader.Read((uint)0);
            StageEpicTrojanQuest = reader.Read((byte)0);
            ChangeEpicTrojan = reader.Read((uint)0);
            CanChangeEpicMaterial = reader.Read((byte)0);
            CanChangeArrayEpicMaterial = reader.Read((byte)0);
            ChangeArrayEpicTrojan = reader.Read((uint)0);
            EpicTrojanArrayItemChange[0] = reader.Read((uint)0);
            EpicTrojanArrayItemChange[1] = reader.Read((uint)0);
            EpicTrojanEvilArrayPoints = reader.Read((uint)0);
            EpicTrojanAbysalStage = reader.Read((uint)0);
            EpicTrojanMr_MirrorItemChange[0] = reader.Read((uint)0);
            EpicTrojanMr_MirrorItemChange[1] = reader.Read((uint)0);
            ChangeMr_MirrorEpicTrojan = reader.Read((uint)0);
            CanChangeMr_MirrorEpicMaterial = reader.Read((byte)0);

            EpicTrojanGeneralPakItemChange[0] = reader.Read((uint)0);
            EpicTrojanGeneralPakItemChange[1] = reader.Read((uint)0);
            ChangeGeneralPakEpicTrojan = reader.Read((uint)0);
            CanChangGeneralPakMaterial = reader.Read((byte)0);
            EpicTrojanMrMirrorPrograss = reader.Read((byte)0);
        }

        public void InitializeTransfer(uint ServerID)
        {
            // The old paid permanent transfer deletes the character here before the target server saved it and loses
            // warehouse/mail/house: off unless [InterServer] AllowLegacyTransfer=1 (docs\MULTI_SERVER.md).
            if (!MsgInterServer.PipeConfig.AllowLegacyTransfer)
            {
                Owner.CreateBoxDialog("Server transfer is not available at the moment.");
                return;
            }
            if (Database.GroupServerList.GetServer(ServerID) == null)
                return;
            if (ConquerPoints > 200000)
            {
                var server = Database.GroupServerList.GetServer(ServerID);
                TransferToServer = server.Name;
                CheckTransfer = true;
                MsgInterServer.PipeClient.Connect(Owner, server.IPAddress, server.Port);
                Owner.CreateBoxDialog("We're preparing your transfer , please stand by ...");
            }
            else
                Owner.CreateBoxDialog("You need to have 200k CPs for transfer ! ");
        }

        public bool OnMyOwnServer
        {
            get { return ServerID == Database.GroupServerList.MyServerInfo.ID; }
        }
        public bool InUnion
        {
            get { return MyUnion != null && UnionMemeber != null; }
        }
        public bool IsUnionEmperror
        {
            get { return MyUnion != null && UnionMemeber != null && UnionMemeber.Rank == Instance.Union.Member.MilitaryRanks.Emperor; }
        }
        public bool Rate(int value)
        {
            return value > MyRandom.Next() % 100;
        }
        public uint DailyDays
        {
            get
            {
                uint days = 0;
                for (byte x = 0; x < 31; x++)
                    if ((DailySignUpDays & (1ul << x)) == (1ul << x))
                        days += 1;
                return days;
            }
        }
        public uint OnlineHours
        {
            get { return OnlineMinutes / 60; }
        }
        public Instance.Clan.Member MyClanMember
        {
            get
            {
                if (clanmemb == null)
                {
                    if (MyClan != null)
                    {
                        MyClan.Members.TryGetValue(UID, out clanmemb);
                    }
                }
                return clanmemb;
            }
            set
            {
                clanmemb = value;
            }
        }
        public Role.Instance.Nobility.NobilityRank NobilityRank
        {
            get { return _NobilityRank; }
            set
            {
                _NobilityRank = value;
                if (MyGuild != null && MyGuildMember != null)
                    MyGuildMember.NobilityRank = (uint)_NobilityRank;
                if (UnionMemeber != null)
                    UnionMemeber.NobilityRank = _NobilityRank;
            }
        }
        public uint KingDomExploits
        {
            get { return _KingDomExploits; }
            set
            {
                _KingDomExploits = value;
                if (InUnion)
                    UnionMemeber.Exploits = value;
                UpdateExploitsRank();

            }
        }
        public void UpdateExploitsRank()
        {
            if (KingDomExploits < 200)
                return;

            if (KingDomExploits >= 23000)
                SetExploitsRank(Role.Flags.ExploitsRank.GeneralinChief);
            else if (KingDomExploits >= 15000)
                SetExploitsRank(Role.Flags.ExploitsRank.FlyingCavalryGeneral);
            else if (KingDomExploits >= 10000)
                SetExploitsRank(Role.Flags.ExploitsRank.ChariotsandCavalryGeneral);
            else if (KingDomExploits >= 7500)
                SetExploitsRank(Role.Flags.ExploitsRank.ChiefofStaff);
            else if (KingDomExploits >= 6000)
                SetExploitsRank(Role.Flags.ExploitsRank.General);
            else if (KingDomExploits >= 4700)
                SetExploitsRank(Role.Flags.ExploitsRank.AssistantGeneral);
            else if (KingDomExploits >= 3700)
                SetExploitsRank(Role.Flags.ExploitsRank.DeputyGeneral);
            else if (KingDomExploits >= 2800)
                SetExploitsRank(Role.Flags.ExploitsRank.MasterSergeant);
            else if (KingDomExploits >= 2100)
                SetExploitsRank(Role.Flags.ExploitsRank.StaffSergeant);
            else if (KingDomExploits >= 1500)
                SetExploitsRank(Role.Flags.ExploitsRank.Sergeant);
            else if (KingDomExploits >= 1000)
                SetExploitsRank(Role.Flags.ExploitsRank.Centurion);
            else if (KingDomExploits >= 500)
                SetExploitsRank(Role.Flags.ExploitsRank.Decurion);
            else if (KingDomExploits >= 200)
                SetExploitsRank(Role.Flags.ExploitsRank.Corporal);
        }
        public void SetExploitsRank(Role.Flags.ExploitsRank rank)
        {
            ExploitsRank = rank;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                SendUpdate(stream, (long)rank, MsgUpdate.DataType.ExploitsRank, true);
            }
        }
        public bool IsStillBanned
        {
            get
            {
                return (this.BannedChatStamp >= DateTime.Now && this.IsBannedChat) || this.PermenantBannedChat;
            }
        }
    }

}
