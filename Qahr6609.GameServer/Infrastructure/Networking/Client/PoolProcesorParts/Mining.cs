using Conquer6609.Database;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.Game.MsgLoader;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgServer.AttackHandler;
using Conquer6609.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conquer6609.Client
{
    public partial class PoolProcesses
    {
        public static unsafe void MiningProcess(Client.GameClient client)
        {
            if (!client.Player.Mining)
                return;
            if (!client.Map.TypeStatus.HasFlag(Conquer6609.Role.MapTypeFlags.MineEnable))
            {
                client.Player.StopMining();
                return;
            }
            Game.MsgServer.MsgGameItem MiningWeapon = null;
            if (!client.Equipment.Alternante)
            {
                if (!client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out MiningWeapon))
                {
                    client.SendSysMesage("You have to wear PickAxe to start mining.");
                    client.Player.StopMining();
                    return;
                }
            }
            else
            {
                if (!client.Equipment.TryGetEquip(Role.Flags.ConquerItem.AleternanteRightWeapon, out MiningWeapon))
                {
                    client.SendSysMesage("You have to wear PickAxe to start mining.");
                    client.Player.StopMining();
                    return;
                }
            }
            if (MiningWeapon == null)
                return;
            if (!Database.ItemType.IsPickAxe(MiningWeapon.ITEM_ID))
            {
                client.SendSysMesage("You have to wear PickAxe to start mining.");
                client.Player.StopMining();
                return;
            }
            if (!client.Inventory.HaveSpace(1))
            {
                client.SendSysMesage("Your inventory is full. You can not mine anymore items.");
                client.Player.StopMining();
                return;
            }
            if (client.Player.MiningAttempts == 0)
            {
                client.SendSysMesage("Sorry, you need to get some rest come back tomorrow.");
                return;
            }
            using (var recycle = new ServerSockets.RecycledPacket())
            {
                var stream = recycle.GetStream();
                Conquer6609.Database.MineRule Role = null;
                if (Conquer6609.Database.MiningTable.GetRandomRole(out Role))
                {
                    switch (Role.RuleType)
                    {
                        case Database.MineRuleType.MineOre:
                            {
                                client.Inventory.Add(stream, Role.RuleValue, 1, 0, 0, 0, Conquer6609.Role.Flags.Gem.NoSocket, Conquer6609.Role.Flags.Gem.NoSocket, false, Conquer6609.Role.Flags.ItemEffect.None, true, "!", 0, 0, 0, true);
                                //client.Player.MiningAttempts--;
                                break;
                            }
                        case Database.MineRuleType.MineGem:
                            {
                                uint ItemID = Role.RuleValue;
                                if (ItemID % 10 == 1)
                                {
                                    if (Database.MiningTable.PercentSuccess(10.0))
                                        ItemID += 1;
                                    if (ItemID % 10 == 1)
                                    {
                                        if (Database.MiningTable.PercentSuccess(1.0))
                                            ItemID += 2;
                                    }
                                    if (ItemID % 10 == 2)
                                    {
                                        if (Database.MiningTable.PercentSuccess(1.0))
                                            ItemID += 1;
                                    }
                                }
                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0, Conquer6609.Role.Flags.Gem.NoSocket, Conquer6609.Role.Flags.Gem.NoSocket, false, Conquer6609.Role.Flags.ItemEffect.None, true, "~from~mining!", 0, 0, 0, true);
                                //client.Player.MiningAttempts--;
                                break;
                            }
                        case Database.MineRuleType.MineMisc:
                            {
                                client.Inventory.Add(stream, Role.RuleValue, 1, 0, 0, 0, Conquer6609.Role.Flags.Gem.NoSocket, Conquer6609.Role.Flags.Gem.NoSocket, false, Conquer6609.Role.Flags.ItemEffect.None, true, "~from~mining!", 0, 0, 0, true);
                                //client.Player.MiningAttempts--;
                                break;
                            }
                        case Database.MineRuleType.MineStone:
                            {
                                byte itemplus = (byte)(Role.RuleValue % 10);
                                client.Inventory.Add(stream, Role.RuleValue, 1, itemplus, 0, 0, Conquer6609.Role.Flags.Gem.NoSocket, Conquer6609.Role.Flags.Gem.NoSocket, false, Conquer6609.Role.Flags.ItemEffect.None, true, "~from~mining!", 0, 0, 0, true);
                                //client.Player.MiningAttempts--;
                                break;
                            }
                        default:
                            break;
                    }
                }
                ActionQuery daction = new ActionQuery()
                {
                    ObjId = client.Player.UID,
                    dwParam = client.Player.MiningAttempts,
                    Type = ActionType.Mining,
                    wParam1 = 24,
                    wParam2 = 68,
                };
                client.Send(stream.ActionCreate(&daction));
            }
        }
    }
}