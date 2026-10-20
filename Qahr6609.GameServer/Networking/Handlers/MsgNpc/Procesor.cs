using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgNpc
{
    using ActionInvoker = CachedAttributeInvocation<ProcessAction, NpcAttribute, NpcID>;
    public unsafe delegate void ProcessAction(Client.GameClient user, ServerSockets.Packet stream, byte Option, string Input, uint id);

    

    public class Procesor
    {
        public static ExecuteNpcInvoker ExecuteNpc = new ExecuteNpcInvoker();
        public unsafe class InvokerClient
        {
            public Client.GameClient client;
            public byte InteractType;
            public byte option;
            public string input;
            public uint npcid;
            public InvokerClient(Client.GameClient Client, ServerSockets.Packet Server_Replay, uint _npcid, byte _InteractType, byte _option, string _input)
            {
                client = Client; 

                option = _option;
                InteractType = _InteractType;
                input = _input;
                npcid = _npcid;
            }
        }

        public static ActionInvoker invoker = new ActionInvoker(NpcAttribute.Translator);

        [PacketAttribute(GamePackets.NpcServerReplay)]
        private unsafe static void NpcServerReplay(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.InTrade == true || user.IsVendor || !user.Socket.Alive || user.Player.OnAutoHunt)
                return;
           
            uint npcid;
            ushort Mesh;
            byte option;
            byte type;
            NpcServerReplay.Mode Action;
            string input;
            //action ==6 place!
            stream.NpcDialog(out npcid, out Mesh, out option, out type, out Action, out input);
            if (Action == MsgNpc.NpcServerReplay.Mode.PlaceFurniture)
            {
                if (!user.Inventory.HaveSpace(1))
                {
                    user.CreateBoxDialog("Please make 1 more space your inventory.");
                    return;
                }
                Npc furniture;
                if (user.MyHouse.Furnitures.TryGetValue(npcid, out furniture))
                {
                    var npc = Database.NpcServer.GetNpcFromMesh(furniture.Mesh);
                    if (npc != null)
                    {
                        user.Inventory.Add(stream, npc.ItemID);
                        user.MyHouse.Furnitures.TryRemove(npcid, out furniture);
                        Database.ItemType.DBItem item;
                        if (Database.Server.ItemsBase.TryGetValue(npc.ItemID, out item))
                            user.SendSysMesage("You got a " + item.Name + "!", MsgMessage.ChatMode.System);
                        var action = new ActionQuery()
                        {
                            ObjId = npcid,
                            Type = ActionType.RemoveEntity
                        };
                        user.Send(stream.ActionCreate(&action));
                    }
                }
                return;
            }
            if (Action == MsgNpc.NpcServerReplay.Mode.Statue)
            {
                Npc furniture;
                if (user.MyHouse.Furnitures.TryGetValue(npcid, out furniture))
                {
                    user.MyHouse.Furnitures.TryRemove(npcid, out furniture);
                    //Database.ItemType.DBItem item;

                    var action = new ActionQuery()
                    {
                        ObjId = npcid,
                        Type = ActionType.RemoveEntity
                    };
                    user.Send(stream.ActionCreate(&action));
                }
                return;
            }
            if (option == 255)
                return;
            user.ActiveNpc = (uint)npcid;
            ExecuteNpc.Enqueue(new InvokerClient(user, stream, (uint)npcid, type, option, input));
        }
        [PacketAttribute(GamePackets.NpcServerRequest)]
        private unsafe static void NpcServerRequest(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.InTrade == true || user.IsVendor || !user.Socket.Alive || user.Player.OnAutoHunt)
                return;

            uint npcid;
            ushort Mesh;
            byte option;
            byte type;
            NpcServerReplay.Mode Action;
            string input;

            stream.NpcDialog(out npcid, out Mesh, out option, out type, out Action, out input);

           
            if (type == (byte)NpcReply.InteractTypes.MessageBox)
            {
                if (Program.BlockTeleportMap.Contains(user.Player.Map))
                    return;
                if (user.Player.StartMessageBox > Extensions.Time32.Now)
                {
                    if (option == 0 && user.Player.MessageOK != null)
                        user.Player.MessageOK.Invoke(user);
                    else if (user.Player.MessageCancel != null)
                        user.Player.MessageCancel.Invoke(user);
                }
                user.Player.MessageOK = null;
                user.Player.MessageCancel = null;
                return;
            }
            if (type == 102)
            {
                // Guild expel: only the leader or a deputy leader may expel, the target must be a member of the
                // same guild with a strictly lower rank, and nobody can expel themselves or the guild leader.
                if (user.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || user.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader)
                {
                    var guild = user.Player.MyGuild;
                    if (guild != null && !string.IsNullOrEmpty(input))
                    {
                        var target = guild.GetMember(input);
                        if (target == null || target.UID == user.Player.UID
                            || target.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                            return;
                        Role.Instance.Guild.Member self;
                        if (!guild.Members.TryGetValue(user.Player.UID, out self) || self == null)
                            return;
                        if ((ushort)target.Rank >= (ushort)self.Rank)
                            return;
                        guild.Quit(input, true, stream);
                    }
                }
                return;
            }
            #region AutoH
            if (user.ActiveNpc == 987977854)
            {//ayman
                if (!DialogOptionAllowed(user, user.ActiveNpc, option))
                    return;
                switch (option)
                {
                    case 1:
                        {
                            Catching.Start(user);
                            break;
                        }
                    case 2:
                        {
                            Catching.End(user);
                            break;
                        }
                    case 3:
                        {
                            user.ActiveNpc = 987977854;
                            Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(user, stream);
                            dialog.AddText("You are currently collecting items in order to selected below do you want to change them.");
                            dialog.AddOption(" DragonBall Items [Enabled].", 4);
                            dialog.AddOption(" Meteors Items [Enabled].", 5);
                            dialog.AddOption(" Quality Items [Enabled].", 6);
                            dialog.AddOption(" ExpBallEventItems [Enabled].", 7);
                            dialog.AddOption(" Socketed(SocketTwo) Items [Enabled].", 8);
                            dialog.AddOption(" BlessedItems [Enabled].", 10);
                            //dialog.AddOption(" MaterialItems [Enabled].", 11);
                            //dialog.Option(" SoulItems [Enabled].", 12);
                            dialog.AddOption(" PlusItems [Enabled].", 13);
                            dialog.AddOption(" Pickup Money [Enabled].", 16);
                            dialog.AddOption("thanks!", 255);
                            dialog.AddAvatar(0);
                            dialog.FinalizeDialog();
                            break;
                        }

                    case 33:
                        {
                            user.ActiveNpc = 987977854;
                            Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(user, stream);
                            dialog.AddText("You are currently collecting items in order to selected below do you want to change them.");
                            dialog.AddOption(" DragonBall Items [Disabled].", 20);
                            dialog.AddOption(" Meteors Items [Disabled].", 21);
                            dialog.AddOption(" Quality Items [Disabled].", 22);
                            dialog.AddOption(" ExpBallEventItems  [Disabled].", 23);
                            dialog.AddOption(" Socketed(SocketTwo) Items  [Disabled].", 24);
                            dialog.AddOption(" BlessedItems [Disabled].", 25);
                            //dialog.Option(" MaterialItems [Disabled].", 26);                          
                            //dialog.Option(" SoulItems [Disabled].", 27);                           
                            dialog.AddOption(" PlusItems [Disabled].", 28);
                            dialog.AddOption(" Pickup Money [Disabled].", 29);
                            dialog.AddOption("thanks!", 255);
                            dialog.AddAvatar(0);
                            dialog.FinalizeDialog();
                            break;
                        }

                    case 4:
                        {
                            user.SendSysMesage(" Congratolitin You Can Picke DragonBall ");
                            user.AutoHunting.DBalls = true;
                            break;
                        }
                    case 20:
                        {
                            user.SendSysMesage(" Oh!! Now You Can Not Picke DragonBall ");
                            user.AutoHunting.DBalls = false;
                            break;
                        }
                    case 5:
                        {
                            user.SendSysMesage(" Congratolitin You Can Picke Meteors ");
                            user.AutoHunting.Meteors = true;
                            break;
                        }
                    case 21:
                        {
                            user.SendSysMesage(" Oh!! Now You Can Not Picke Meteors ");
                            user.AutoHunting.Meteors = false;
                            break;
                        }
                    case 6:
                        {
                            user.SendSysMesage(" Congratolitin You Can Picke QualityItems ");
                            user.AutoHunting.QualityItems = true;
                            break;
                        }
                    case 22:
                        {
                            user.SendSysMesage(" Oh!! Now You Can Not Picke QualityItems ");
                            user.AutoHunting.QualityItems = false;
                            break;
                        }
                    case 7:
                        {

                            user.AutoHunting.ExpBallEventItems = true;
                            break;
                        }
                    case 23:
                        {

                            user.AutoHunting.ExpBallEventItems = false;

                            break;
                        }
                    case 8:
                        {
                            user.SendSysMesage(" Congratolitin You Can Picke SocketedItems ");
                            user.AutoHunting.SocketedItems = true;
                            break;
                        }
                    case 24:
                        {
                            user.SendSysMesage(" Oh!! Now You Can Not Picke SocketedItems ");
                            user.AutoHunting.SocketedItems = false;
                            break;
                        }
                    case 10:
                        {
                            user.SendSysMesage(" Congratolitin You Can Picke BlessedItems ");
                            user.AutoHunting.BlessedItems = true;
                            break;
                        }
                    case 25:
                        {
                            user.SendSysMesage(" Oh!! Now You Can Not Picke BlessedItems ");
                            user.AutoHunting.BlessedItems = false;
                            break;
                        }
                    case 11:
                        {

                            user.AutoHunting.MaterialItems = true;
                            break;
                        }
                    case 26:
                        {

                            user.AutoHunting.MaterialItems = false;
                            break;
                        }
                    case 12:
                        {

                            user.AutoHunting.SoulItems = true;
                            break;
                        }
                    case 27:
                        {

                            user.AutoHunting.SoulItems = false;
                            break;
                        }
                    case 13:
                        {
                            user.SendSysMesage(" Congratolitin You Can Picke PlusItems ");
                            user.AutoHunting.PlusItems = true;
                            break;
                        }
                    case 28:
                        {
                            user.SendSysMesage(" Oh!! Now You Can Not Picke PlusItems ");
                            user.AutoHunting.PlusItems = false;
                            break;
                        }
                    case 16:
                        {
                            user.SendSysMesage(" Congratolitin You Can Picke Money ");
                            user.AutoHunting.LootMoney = true;
                            break;
                        }
                    case 29:
                        {
                            user.SendSysMesage(" Oh!! Now You Can Not Picke Money ");
                            user.AutoHunting.LootMoney = false;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                return;
            }
            #endregion
            if (option == 255 || option == 0 || user.InTrade)
                return;
            npcid = (uint)user.ActiveNpc;

            ExecuteNpc.Enqueue(new InvokerClient(user, stream, (uint)npcid, type, option, input));
        }
        /// <summary>A dialog reply is only accepted when the option was offered by the last dialog of that NPC
        /// (server-side record kept by Dialog). StrictNpcOptions: 0 off, 1 audit only, 2 reject.</summary>
        public static bool DialogOptionAllowed(Client.GameClient client, uint npcid, byte option)
        {
            global::Conquer6609.Security.Guard.EnsureLoaded();
            int mode = global::Conquer6609.Security.Guard.StrictNpcOptions;
            if (mode <= 0 || client == null || client.Player == null)
                return true;
            if (global::Conquer6609.Security.NpcDialogState.IsAllowed(client.Player.UID, npcid, option))
                return true;
            global::Conquer6609.Security.Guard.Audit(mode >= 2 ? "npc-option-rejected" : "npc-option-unexpected", client.Player.UID, client.Player.Name,
                "npc=" + npcid + " option=" + option + " map=" + client.Player.Map);
            if (mode < 2)
                return true;
            client.SendSysMesage("This option is no longer available. Please talk to the NPC again.");
            return false;
        }
        public class ExecuteNpcInvoker : ConcurrentSmartThreadQueue<InvokerClient>
        {
            public ExecuteNpcInvoker()
                : base(3)
            {
                Start(5);
            }
            public void TryEnqueue(InvokerClient action)
            {
                Enqueue(action);
            }

            protected unsafe override void OnDequeue(InvokerClient action, int time)
            {
                try
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        if (!action.client.Player.OnMyOwnServer)
                        {
                            if (action.npcid == (ushort)NpcID.KingdomMissionEnvoy
                                || action.npcid == (ushort)NpcID.RealmEnvoy)
                            {
                                Tuple<NpcAttribute, ProcessAction> processFolded;
                                if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                    processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                            }
                            else if (action.client.Player.Map == 3935 || action.client.Player.Map == Game.MsgTournaments.MsgEliteGroup.WaitingAreaID)
                            {
                                 Game.MsgNpc.Npc _obj;
                                 if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                                 {
                                     if (action.client.ProjectManager)
                                         action.client.SendSysMesage("Active Npc [" + action.npcid + "] X[" + _obj.X + "] Y[" + _obj.Y + "]", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                     if (action.npcid >= (ushort)NpcID.Crystal1 && action.npcid <= (ushort)NpcID.Crystal5)
                                     {
                                         NpcHandler.KingDoomCrystals(action.client, stream, action.option, action.input, action.npcid);
                                         return;
                                     }

                                     Tuple<NpcAttribute, ProcessAction> processFolded;
                                     if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                         processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                                 }
                                 else
                                 {
                                     Role.IMapObj inpc;
                                     if (action.client.Player.View.TryGetValue((uint)action.npcid, out inpc, Role.MapObjectType.SobNpc))
                                     {
                                         if (action.client.ProjectManager)
                                             action.client.SendSysMesage("Active Npc [" + action.npcid + "] X[" + _obj.X + "] Y[" + _obj.Y + "]", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                         var npc = inpc as Role.SobNpc;
                                         Tuple<NpcAttribute, ProcessAction> processFolded;
                                         if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                             processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);

                                     }
                                 }
                            }
                            return;
                        }
                        if (action.InteractType == (byte)NpcReply.InteractTypes.MessageBox)
                        {
                            if (action.client.Player.StartMessageBox > Extensions.Time32.Now)
                            {
                                if (action.option == 255 && action.client.Player.MessageOK != null)
                                    action.client.Player.MessageOK.Invoke(action.client);
                                else if (action.client.Player.MessageCancel != null)
                                    action.client.Player.MessageCancel.Invoke(action.client);
                            }
                            action.client.Player.MessageOK = null;
                            action.client.Player.MessageCancel = null;
                            return;
                        }
                        if (!DialogOptionAllowed(action.client, action.npcid, action.option))
                            return;
                        if ((uint)action.npcid == 3124)//house WH
                        {
                            if (action.client.MyHouse != null && action.client.Player.DynamicID == action.client.Player.UID)
                            {
                                ActionQuery query = new ActionQuery()
                          {
                              Type = ActionType.OpenDialog,
                              ObjId = action.client.Player.UID,
                              dwParam = MsgServer.DialogCommands.Warehouse,
                              wParam1 = action.client.Player.X,
                              wParam2 = action.client.Player.Y
                          };
                                action.client.Send(stream.ActionCreate(&query));

                                return;
                            }
                            else
                            {
                                action.client.SendSysMesage("I'm sorry but you dont own this house !");
                               
                            }
                        }
                        if (action.client.Player.Map == 1038)//Guild War
                        {
                            Role.IMapObj inpc;
                            if (action.client.Player.View.TryGetValue((uint)action.npcid, out inpc, Role.MapObjectType.SobNpc))
                            {
                                var npc = inpc as Role.SobNpc;
                                Tuple<NpcAttribute, ProcessAction> processFolded;
                                if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                    processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                                return;
                            }

                        }
                        if (action.client.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)//Guild War
                        {
                            Role.IMapObj inpc;
                            if (action.client.Player.View.TryGetValue((uint)action.npcid, out inpc, Role.MapObjectType.SobNpc))
                            {
                                var npc = inpc as Role.SobNpc;
                                Tuple<NpcAttribute, ProcessAction> processFolded;
                                if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                    processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                                return;
                            }

                        }
                        if (action.npcid >= 7832 && action.npcid <= 7840)
                        {
                            Game.MsgNpc.Npc _obj;
                            if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                            {
                                action.client.OnRemoveNpc = _obj;
                                NpcHandler.CheckDesertGuardian(action.client, stream, action.option, action.input, action.npcid);

                            }
                            return;

                        }
                        if (action.npcid >= 8546 && action.npcid <= 8550)
                        {
                            Game.MsgNpc.Npc _obj;
                            if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                            {
                                action.client.OnRemoveNpc = _obj;
                                NpcHandler.SoldierBird(action.client, stream, action.option, action.input, action.npcid);

                            }
                            return;

                        }
                        if (action.npcid >= 8551 && action.npcid <= 8555)
                        {
                            Game.MsgNpc.Npc _obj;
                            if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                            {
                                action.client.OnRemoveNpc = _obj;
                                NpcHandler.BandittiFlowers(action.client, stream, action.option, action.input, action.npcid);

                            }
                            return;

                        }
                        if (action.npcid == (uint)NpcID.SelectSacredRefineryPack || action.npcid == (uint)NpcID.SelectP7WeaponSoulPack
                            || action.npcid == (uint)NpcID.SelectP7EquipmentSoulPack
                            || action.npcid == (uint)NpcID.Steed1
                            || action.npcid == (uint)NpcID.Steed3
                            || action.npcid == (uint)NpcID.Steed6 || action.npcid == (uint)NpcID.DailyItem1
                            || action.npcid == (uint)NpcID.DailyEliteSpiritBead
                            || action.npcid == (uint)NpcID.DailyNormalSpiritBead
                            || action.npcid == (uint)NpcID.DailyRefinedSpiritBead
                            || action.npcid == (uint)NpcID.DailyUniqueSpiritBead
                            || action.npcid == (uint)NpcID.DailySuperSpiritBead
                            || action.npcid == (uint)NpcID.Level43UniqueRingPack
                            || action.npcid == (uint)NpcID.NobleSteedPack
                            || action.npcid == (uint)NpcID.DazzlingDiamondBox
                            || action.npcid == (uint)NpcID.RareSteedPack6
                            || action.npcid == (uint)NpcID.TempestSecretLetter
                            || action.npcid == (uint)NpcID.SashFragment_Realm
                            // GarmentPacket/GarmentPacket2/MountPacket/MountPacket2/MountPacket3/AccesoryPacket/AccesoryPacket2
                            // are not opened by any item, so they are handled like normal NPCs (must be in screen).
                            || action.npcid == (uint)NpcID.GoldPrizeToken
                            || action.npcid == (uint)NpcID.BlackFridayGarmentPack
                            || action.npcid == (uint)NpcID.BlackFridayMountPack || action.npcid == (uint)NpcID.BlackFridayAccesory
                            || action.npcid == (uint)NpcID.Steed1Pack
                            || action.npcid == (uint)NpcID.Steed3Pack
                              || action.npcid == (uint)NpcID.HeavenDemonBox
                              || action.npcid == (uint)NpcID.ChaosDemonBox
                              || action.npcid == (uint)NpcID.SacredDemonBox
                              || action.npcid == (uint)NpcID.AuroraDemonBox
                              || action.npcid == (uint)NpcID.DemonBox
                              || action.npcid == (uint)NpcID.AncientDemonBox
                            || action.npcid == (uint)NpcID.FloodDemonBox
                            || action.npcid == (uint)NpcID.ChiToken
                           || action.npcid == (uint)NpcID.MrMirror2
                            || action.npcid == (uint)NpcID.SuperHeadgearPack || action.npcid == (uint)NpcID.RingPack
                            || action.npcid == (uint)NpcID.ClothingPack || action.npcid == (uint)NpcID.PowerBook
                            || action.npcid == (uint)NpcID.Level50UniqueWeaponPack
                            || action.npcid == (uint)NpcID.Level52UniqueHeadgearPack
                            || action.npcid == (uint)NpcID.Level55EliteWeaponPack
                            || action.npcid == (uint)NpcID.Level67EliteHeadgearPack
                            || action.npcid == (uint)NpcID.Joyful1StonePack
                            || action.npcid == (uint)NpcID.Joyful2StonePack
                            || action.npcid == (uint)NpcID.Joyful3StonePack
                            || action.npcid == (uint)NpcID.Joyful4StonePack
                            || action.npcid == (uint)NpcID.Joyful5StonePack)
                        {
                            Tuple<NpcAttribute, ProcessAction> processFolded;
                            if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        if (action.npcid == (uint)NpcID.Joyful1StonePack)
                        {
                            NpcHandler.Joyful1StonePack(action.client, stream, action.option, action.input, action.npcid);
                        }
                        if (action.npcid == (uint)NpcID.Joyful2StonePack)
                        {
                            NpcHandler.Joyful2StonePack(action.client, stream, action.option, action.input, action.npcid);
                        }
                        if (action.npcid == (uint)NpcID.Joyful3StonePack)
                        {
                            NpcHandler.Joyful3StonePack(action.client, stream, action.option, action.input, action.npcid);
                        }
                        if (action.npcid == (uint)NpcID.Joyful4StonePack)
                        {
                            NpcHandler.Joyful4StonePack(action.client, stream, action.option, action.input, action.npcid);
                        }
                        if (action.npcid == (uint)NpcID.Joyful5StonePack)
                        {
                            NpcHandler.Joyful5StonePack(action.client, stream, action.option, action.input, action.npcid);
                        }
                        Game.MsgNpc.Npc obj;
                        if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out obj))
                        {
                            if (action.client.ProjectManager)
                                action.client.SendSysMesage("Active Npc [" + action.npcid + "] X[" + obj.X + "] Y[" + obj.Y + "]", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                            if (action.client.Player.Map == 3820)
                            {
                                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.TreasureThief)
                                {
                                    var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgTreasureThief;
                                  tournament.Reward(action.client, obj, stream);
                                    return;
                                }
                            }
                            if (action.client.Player.Map == 3826 || action.client.Player.Map == 3827
                                || action.client.Player.Map == 3828 || action.client.Player.Map == 3829)
                            {
                                if (action.npcid >= 10584 && action.npcid <= 10588)
                                    NpcHandler.Epic2MonkMisery(action.client, stream, action.option, action.input, action.npcid);
                                if (action.npcid >= 10589 && action.npcid <= 10598)
                                    NpcHandler.ArraysEye(action.client, stream, action.option, action.input, action.npcid);
                                return;
                            }
                            if (action.client.Player.Map == 1511)
                            {
                                NpcHandler.Furnitures(action.client, stream, action.option, action.input, action.npcid);
                                return;
                            }
                            // R1 (docs\COORDINATION.md): modules (QuestEngine) see the click before the NPC script.
                            // 1 = handled, 2 = "something else" chosen in a module dialog -> NPC script with option 0.
                            int mh = Framework.Modules.ModuleManager.NpcTalk(action.client, stream, action.option, (uint)action.npcid);
                            if (mh == 1)
                                return;
                            if (mh == 2)
                                action.option = 0;
                            Tuple<NpcAttribute, ProcessAction> processFolded;
                            if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                            else
                            {

                                if (action.client.Player.Map == 1038)
                                {
                                    if (action.npcid == (uint)NpcID.GuildConductor1 || action.npcid == (uint)NpcID.GuildConductor2
                                        || action.npcid == (uint)NpcID.GuildConductor3 || action.npcid == (uint)NpcID.GuildConductor4)
                                        NpcHandler.GuildConductorsProces(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if (((int)action.npcid >= 10031 && (int)action.npcid <= 10041 || (int)action.npcid == 10043) && action.client.Player.DynamicID == 0)
                                {
                                    NpcHandler.SpaceMarks(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if (action.npcid == (uint)NpcID.TeleGuild1 || action.npcid == (uint)NpcID.TeleGuild2
                                   || action.npcid == (uint)NpcID.TeleGuild3 || action.npcid == (uint)NpcID.TeleGuild4)
                                {
                                    NpcHandler.GuildCondTeleBack(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if (action.npcid == (uint)NpcID.WHTwin || action.npcid == (uint)NpcID.wHPheonix
                                   || action.npcid == (uint)NpcID.WHMarket || action.npcid == (uint)NpcID.WHBird
                                   || action.npcid == (uint)NpcID.WHDesert || action.npcid == (uint)NpcID.WHApe
                                    || action.npcid == (uint)NpcID.WHPoker)
                                {
                                    NpcHandler.Warehause(action.client, stream, action.option, action.input, action.npcid);
                                }

                                else if ((int)action.npcid >= 925 && (int)action.npcid <= 930 && action.client.Player.Map == 700 && action.client.Player.DynamicID == 0)
                                {
                                    NpcHandler.LotteryBoxes(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else
                                {
                                    if (action.client.ProjectManager)
                                        MyConsole.WriteLine("Not find Npc -> " + action.npcid + " ");
                                }
                            }
                        }
                        else if (action.npcid == 12)
                        {
                            if (action.client.Player.VipLevel > 0)
                            {

                                ActionQuery query = new ActionQuery()
                                {
                                    Type = ActionType.OpenDialog,
                                    ObjId = action.client.Player.UID,
                                    dwParam = MsgServer.DialogCommands.VIPWarehouse,
                                    wParam1 = action.client.Player.X,
                                    wParam2 = action.client.Player.Y
                                };
                                action.client.Send(stream.ActionCreate(&query));



                            }
                        }
                        else
                        {
                            Role.IMapObj inpc;
                            if (action.client.Player.View.TryGetValue((uint)action.npcid, out inpc, Role.MapObjectType.SobNpc))
                            {
                                if (action.client.ProjectManager)
                                    action.client.SendSysMesage("Active Npc [" + action.npcid +"]", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                var npc = inpc as Role.SobNpc;
                                Tuple<NpcAttribute, ProcessAction> processFolded;
                                if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                    processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                          
                            }
                        }
                    }
                }
                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
            }
        }
    }
}
