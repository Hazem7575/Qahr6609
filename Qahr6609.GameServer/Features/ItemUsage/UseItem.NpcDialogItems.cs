using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.Conquer6609_Work;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgItemUsuagePacket
    {
        // UseItem handlers: items that open an NPC dialog.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_NpcDialogItems(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
                case 728525:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Steed1;
                        Game.MsgNpc.NpcHandler.Steed1(client, stream, 0, "", 0);
                        break;
                    }
                case 728526:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Steed3;
                        Game.MsgNpc.NpcHandler.Steed3(client, stream, 0, "", 0);
                        break;
                    }
                case 728527://steeds
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Steed6;
                        Game.MsgNpc.NpcHandler.Steed6(client, stream, 0, "", 0);
                        break;
                    }
                case 729990://steeds
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.NobleSteedPack;
                        Game.MsgNpc.NpcHandler.NobleSteedPack(client, stream, 0, "", 0);
                        break;
                    }
                case 3001406://steeds
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.RareSteedPack6;
                        Game.MsgNpc.NpcHandler.RareSteedPack6(client, stream, 0, "", 0);
                        break;
                    }

                case 3004247://P7WeaponSoulPack
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.SelectP7WeaponSoulPack;
                        Game.MsgNpc.NpcHandler.SelectP7WeaponSoulPack(client, stream, 0, "", 0);
                        break;
                    }
                case 3004249://sacred material 
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.SelectSacredRefineryPack;
                        Game.MsgNpc.NpcHandler.SelectSacredMaterial(client, stream, 0, "", 0);

                        break;
                    }

                case 3004248://P7EquipmentSoulPack
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.SelectP7EquipmentSoulPack;
                        Game.MsgNpc.NpcHandler.SelectP7EquipmentSoulPack(client, stream, 40, "", 0);
                        break;
                    }
                case 3301223://Joyful+1StonePack
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Joyful1StonePack;
                        Game.MsgNpc.NpcHandler.Joyful1StonePack(client, stream, 0, "", 0);
                        break;
                    }
                case 3301224://Joyful+2StonePack
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Joyful2StonePack;
                        Game.MsgNpc.NpcHandler.Joyful2StonePack(client, stream, 0, "", 0);
                        break;
                    }
                case 3301225://Joyful+3StonePack
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Joyful3StonePack;
                        Game.MsgNpc.NpcHandler.Joyful3StonePack(client, stream, 0, "", 0);
                        break;
                    }
                case 3301226://Joyful+4StonePack
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Joyful4StonePack;
                        Game.MsgNpc.NpcHandler.Joyful4StonePack(client, stream, 0, "", 0);
                        break;
                    }
                case 3301227://Joyful+5StonePack
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Joyful5StonePack;
                        Game.MsgNpc.NpcHandler.Joyful5StonePack(client, stream, 0, "", 0);
                        break;
                    }
                case 728243:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Level55EliteWeaponPack;
                        Game.MsgNpc.NpcHandler.Level55EliteWeaponPack(client, stream, 0, "", 0);


                        break;
                    }
                case 728245:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Level67EliteHeadgearPack;
                        Game.MsgNpc.NpcHandler.Level67EliteHeadgearPack(client, stream, 0, "", 0);
                        break;
                    }
                case 728240:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Level52UniqueHeadgearPack;
                        Game.MsgNpc.NpcHandler.Level52UniqueHeadgearPack(client, stream, 0, "", 0);
                        break;
                    }
                case 3303698:
                    {//ayman
                        client.ActiveNpc = 987977854;
                        Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(client, stream);
                          dialog.AddText("Hello, you can use our autohunt system from here and you will get options to get easy hunt.");
                        if (!client.AutoHunting.Enable)
                            dialog.AddOption("Start Auto Hunting.", 1);
                        else dialog.AddOption("Stop Auto Hunting.", 2);
                        dialog.AddOption("Start AutoPicke.", 3);
                        dialog.AddOption("Stop AutoPicke.", 33);
                        dialog.AddAvatar(0);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 729087:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.DailyItem1;

                        Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(client, stream);
                        dialog.AddText("\t\t\t\t\t\t\t\t\t\tItem List\n")
                   .AddText("\t\t\t??????????????????????????????????????????????\n")
                   .AddText("\t\t\tItem: [BirdClaw]\n")
                   .AddText("\t\t\tAmount: [25]\n")
                   .AddText("\t\t\tMonster: [Birdman][BirdmanL88]\n")
                   .AddText("\t\t\tLocation: [Island]\n")
                   .AddText("\t\t\tChanged times: [1]\n")
                   .AddText("\t\t\t??????????????????????????????????????????????\n")
                            .AddOption("Go~collect~the~items.", 1)
                            .AddOption("find~the~Daily~Quest~Envoy.", 2)
                            .FinalizeDialog();


                        break;
                    }
                case 728239:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Level50UniqueWeaponPack;
                        Game.MsgNpc.NpcHandler.Level50UniqueWeaponPack(client, stream, 0, "", 0);


                        break;
                    }
                case 728238:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Level43UniqueRingPack;
                        Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(client, stream);
                        dialog.AddText("Take a good look and pick one.")
                            .AddOption("Unique~Ivory~Heavy~Ring.", 1)
                            .AddOption("Unique~Amethyst~Ring.", 2)
                            .FinalizeDialog();

                        break;
                    }
                case 721924:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.PowerBook;
                        Game.MsgNpc.NpcHandler.PowerBook(client, stream, 0, "", 0);

                        break;
                    }
                case 728251:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.ClothingPack;
                        Game.MsgNpc.NpcHandler.ClothingPack(client, stream, 0, "", 0);
                        break;
                    }
                case 729611:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.DailyNormalSpiritBead;
                        Game.MsgNpc.NpcHandler.DailyNormalSpiritBead(client, stream, 0, "", 0);
                        break;
                    }
                case 729612:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.DailyRefinedSpiritBead;
                        Game.MsgNpc.NpcHandler.DailyRefinedSpiritBead(client, stream, 0, "", 0);
                        break;
                    }
                case 729613:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.DailyUniqueSpiritBead;
                        Game.MsgNpc.NpcHandler.DailyUniqueSpiritBead(client, stream, 0, "", 0);
                        break;
                    }
                case 729614:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.DailyEliteSpiritBead;
                        Game.MsgNpc.NpcHandler.DailyEliteSpiritBead(client, stream, 0, "", 0);
                        break;
                    }
                case 729703:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.DailySuperSpiritBead;
                        Game.MsgNpc.NpcHandler.DailySuperSpiritBead(client, stream, 0, "", 0);
                        break;
                    }
                case 3006393:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.TempestSecretLetter;
                        Game.MsgNpc.NpcHandler.TempestSecretLetter(client, stream, 0, "", 0);
                        break;
                    }
                case 3000588:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.L60UniqueGearPack;
                        Game.MsgNpc.NpcHandler.L60UniqueGearPack(client, stream, 0, "", 0);

                        break;
                    }
                case 3008732:
                    {

                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.DazzlingDiamondBox;
                        Game.MsgNpc.NpcHandler.DazzlingDiamondBox(client, stream, 0, "", 0);
                        break;
                    }
                case 728250:
                    {
                        if (client.Player.Class <= 85)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Remove(728250, 1, stream);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.Inventory.Add(stream, 150219, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Dragon~Ring!");
                            }
                            else
                            {
                                client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                            }
                        }
                        else
                        {
                            client.ActiveNpc = (uint)Game.MsgNpc.NpcID.RingPack;
                            Game.MsgNpc.NpcHandler.RingPack(client, stream, 0, "", 0);
                        }
                        break;
                    }
                case 727236:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.ChiToken;
                        Game.MsgNpc.NpcHandler.ChiToken(client, stream, 0, "", 0);
                        break;
                    }
                case 728247:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.SuperHeadgearPack;
                        Game.MsgNpc.NpcHandler.SuperHeadgearPack(client, stream, 0, "", 0);
                        break;
                    }
                case 720671:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.HeavenDemonBox;
                        Game.MsgNpc.NpcHandler.HeavenDemonBox(client, stream, 0, "", 0);
                        break;
                    }
                case 720672:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.ChaosDemonBox;
                        Game.MsgNpc.NpcHandler.ChaosDemonBox(client, stream, 0, "", 0);
                        break;
                    }
                case 720673:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.SacredDemonBox;
                        Game.MsgNpc.NpcHandler.SacredDemonBox(client, stream, 0, "", 0);
                        break;
                    }
                case 720674:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.AuroraDemonBox;
                        Game.MsgNpc.NpcHandler.AuroraDemonBox(client, stream, 0, "", 0);
                        break;
                    }
                case 720650:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.DemonBox;
                        Game.MsgNpc.NpcHandler.DemonBox(client, stream, 0, "", 0);
                        break;
                    }
                case 720651:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.AncientDemonBox;
                        Game.MsgNpc.NpcHandler.AncientDemonBox(client, stream, 0, "", 0);
                        break;
                    }
                case 720652:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.FloodDemonBox;
                        Game.MsgNpc.NpcHandler.FloodDemonBox(client, stream, 0, "", 0);
                        break;
                    }
                case 3100055:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.BlackFridayGarmentPack;
                        Game.MsgNpc.NpcHandler.BlackFridayGarmentPack(client, stream, 0, "", 0);
                        break;
                    }
                case 3100054:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.BlackFridayMountPack;
                        Game.MsgNpc.NpcHandler.BlackFridayMountPack(client, stream, 0, "", 0);
                        break;
                    }
                case 3100053:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.BlackFridayAccesory;
                        Game.MsgNpc.NpcHandler.BlackFridayAccesory(client, stream, 0, "", 0);
                        break;
                    }
                case 3007287:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Steed1Pack;
                        Game.MsgNpc.NpcHandler.Steed1Pack(client, stream, 0, "", 0);
                        break;
                    }
                case 3007283:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Steed3Pack;
                        Game.MsgNpc.NpcHandler.Steed3Pack(client, stream, 0, "", 0);
                        break;
                    }
                case 3600024:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.SashFragment_Realm;
                        Game.MsgNpc.NpcHandler.SashFragment_Realm(client, stream, 0, "", 0);
                        break;
                    }

                case 723467:
                    {
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.GoldPrizeToken;
                        Game.MsgNpc.NpcHandler.GoldPrizeToken(client, stream, 0, "", 0);
                        break;
                    }
                default:
                    handled = false;
                    break;
            }
        }
    }
}
