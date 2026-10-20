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
        // UseItem handlers: soul packs, exorcism packs, other material packs.
        // The case bodies were moved verbatim from UseItem's switch (item.ITEM_ID); a "break" or "return"
        // in them ends the item use exactly as before. handled = false means: not one of these items.
        private static void UseItem_SoulAndMaterialPacks(Game.MsgServer.MsgGameItem item, Client.GameClient client, ServerSockets.Packet stream, out bool handled)
        {
            handled = true;
            switch (item.ITEM_ID)
            {
                case 720948://exorcist packet
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(4, 10))
                        {
                            if (Role.Core.Rate(1, 8))
                            {
                                client.Inventory.Add(stream, 350004, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the pack, and received a Dragon Bone!");
                            }
                            else
                            {
                                if (Role.Core.Rate(1, 7))
                                {
                                    client.Inventory.Add(stream, 350010, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                    client.SendSysMesage("You opened the pack, and received a Hockey Stick!");
                                }
                                else
                                {
                                    if (Role.Core.Rate(1, 6))
                                    {
                                        client.Inventory.Add(stream, 360002, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        client.SendSysMesage("You opened the pack, and received a Wooden Club!");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 5))
                                        {
                                            client.Inventory.Add(stream, 360006, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                            client.SendSysMesage("You opened the pack, and received a Spatula!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 4))
                                            {
                                                client.Inventory.Add(stream, 360010, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                client.SendSysMesage("You opened the pack, and received a Handbag!");
                                            }
                                            else
                                            {
                                                if (Role.Core.Rate(1, 3))
                                                {
                                                    client.Inventory.Add(stream, 360012, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                    client.SendSysMesage("You opened the pack, and received a Sports Bag!");
                                                }
                                                else
                                                {
                                                    if (Role.Core.Rate(1, 2))
                                                    {
                                                        client.Inventory.Add(stream, 380001, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                        client.SendSysMesage("You opened the pack, and received a Wok!");
                                                    }
                                                    else
                                                    {
                                                        client.Inventory.Add(stream, 370005, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                        client.SendSysMesage("You opened the pack, and received a Force Bow!");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Role.Core.Rate(1, 14))
                            {
                                client.Inventory.Add(stream, 800802, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                client.SendSysMesage("You opened the pack and received a Desire Pistol.", MsgMessage.ChatMode.System);
                            }
                            else
                            {
                                if (Role.Core.Rate(1, 13))
                                {
                                    client.Inventory.Add(stream, 800915, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                    client.SendSysMesage("You~received~an~Abyssal~Knife~Soul!", MsgMessage.ChatMode.System);
                                }
                                else
                                {
                                    if (Role.Core.Rate(1, 12))
                                    {
                                        client.Inventory.Add(stream, 800807, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                        client.SendSysMesage("You opened the pack and received a Glory Rapier.", MsgMessage.ChatMode.System);
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 11))
                                        {
                                            client.Inventory.Add(stream, 822056, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                            client.SendSysMesage("You`ve opened the Exorcism Pack and received an Ocean Armor!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 10))
                                            {
                                                client.Inventory.Add(stream, 822057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                client.SendSysMesage("You`ve opened the Exorcism Pack and received a Heavenly Armor!");
                                            }
                                            else
                                            {
                                                if (Role.Core.Rate(1, 9))
                                                {
                                                    client.Inventory.Add(stream, 823052, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                    client.SendSysMesage("You`ve opened the Exorcism Pack and received a Violet Ring!");
                                                }
                                                else
                                                {
                                                    if (Role.Core.Rate(1, 8))
                                                    {
                                                        client.Inventory.Add(stream, 823053, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                        client.SendSysMesage("You`ve opened the Exorcism Pack and received a Holy Heavy Ring!");
                                                    }
                                                    else
                                                    {
                                                        if (Role.Core.Rate(1, 7))
                                                        {
                                                            client.Inventory.Add(stream, 823054, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                            client.SendSysMesage("You`ve opened the Exorcism Pack and received a Moon Bracelet!");
                                                        }
                                                        else
                                                        {
                                                            if (Role.Core.Rate(1, 6))
                                                            {
                                                                client.Inventory.Add(stream, 800019, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                                client.SendSysMesage("You`ve opened the Exorcism Pack and received an Aura Sword!");
                                                            }
                                                            else
                                                            {
                                                                if (Role.Core.Rate(1, 5))
                                                                {
                                                                    client.Inventory.Add(stream, 800071, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                                    client.SendSysMesage("You`ve opened the Exorcism Pack and received a Giant Axe!");
                                                                }
                                                                else
                                                                {
                                                                    if (Role.Core.Rate(1, 4))
                                                                    {
                                                                        client.Inventory.Add(stream, 800141, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                                        client.SendSysMesage("You`ve opened the Exorcism Pack and received a Scar Katana!");
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Role.Core.Rate(1, 3))
                                                                        {
                                                                            client.Inventory.Add(stream, 800200, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                                            client.SendSysMesage("You`ve opened the Exorcism Pack and received a Bloody Glaive!");
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Role.Core.Rate(1, 2))
                                                                            {
                                                                                client.Inventory.Add(stream, 800521, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                                                client.SendSysMesage("You`ve opened the Exorcism Pack and received a Dark Backsword!");
                                                                            }
                                                                            else
                                                                            {
                                                                                client.Inventory.Add(stream, 800617, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false);
                                                                                client.SendSysMesage("You`ve opened the Exorcism Pack and received an Ancient Bow!");
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        break;
                    }
                case 721873:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1829, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 91, 283) <= 10)
                            {
                                if (DateTime.Now > client.Player.RevealVialStamp.AddSeconds(30))
                                {
                                    client.Player.RevealVialStamp = DateTime.Now;
                                    client.Player.AddMapEffect(stream, 88, 279, "born");
                                    client.Player.AddMapEffect(stream, 88, 279, "soul-1");
                                    client.CreateBoxDialog("You~used~the~Reveal~Vial~and~revealed~the~hidden~compartment.");
                                }
                                else
                                {
                                    client.CreateBoxDialog("You`ve~already~used~the~Reveal~Vial.~Use~it~again~after~another~30~seconds.");
                                }
                            }
                            else
                            {
                                client.CreateBoxDialog("You~need~to~use~the~Reveal~Vial~at~the~Scripture~Vault~in~Mystic~Castle~(91,283).");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("You~need~to~use~the~Reveal~Vial~at~the~Scripture~Vault~in~Mystic~Castle~(91,283).");
                        }
                        break;
                    }
                case 728917:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        if (Role.Core.Rate(9, 10))
                        {
                            client.Inventory.Add(stream, 720957);
                            client.CreateBoxDialog("You~received~a~Dragon~Soul~Pack!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e104");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 728674);
                            client.CreateBoxDialog("You~received~a~Mighty~Soul~Pack!");
                        }
                        break;
                    }
                case 720957:
                    {
                        client.Inventory.Remove(720957, 1, stream);
                        if (Role.Core.Rate(81, 1000))
                        {
                            if (Role.Core.Rate(1, 29))
                            {
                                client.Inventory.Add(stream, 800723);
                                client.CreateBoxDialog("You`ve received Holy Beads of Percept!");
                            }
                            else
                            {
                                if (Role.Core.Rate(1, 28))
                                {
                                    client.Inventory.Add(stream, 800915);
                                    client.CreateBoxDialog("You~received~an~Abyssal~Knife~Soul!");
                                }
                                else
                                {
                                    if (Role.Core.Rate(1, 27))
                                    {
                                        client.Inventory.Add(stream, 800724);
                                        client.CreateBoxDialog("You`ve received Holy Beads of Dharma!");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 26))
                                        {
                                            client.Inventory.Add(stream, 800801);
                                            client.CreateBoxDialog("You`ve received a Slaughter Pistol!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 25))
                                            {
                                                client.Inventory.Add(stream, 800802);
                                                client.CreateBoxDialog("You`ve received a Desire Pistol!");
                                            }
                                            else
                                            {
                                                if (Role.Core.Rate(1, 24))
                                                {
                                                    client.Inventory.Add(stream, 800806);
                                                    client.CreateBoxDialog("You`ve received a Triumph Rapier!");
                                                }
                                                else
                                                {
                                                    if (Role.Core.Rate(1, 23))
                                                    {
                                                        client.Inventory.Add(stream, 800807);
                                                        client.CreateBoxDialog("You`ve received a Glory Rapier!");
                                                    }
                                                    else
                                                    {
                                                        if (Role.Core.Rate(1, 22))
                                                        {
                                                            client.Inventory.Add(stream, 820052);
                                                            client.CreateBoxDialog("You`ve received a Rage Headgear!");
                                                        }
                                                        else
                                                        {
                                                            if (Role.Core.Rate(1, 21))
                                                            {
                                                                client.Inventory.Add(stream, 820053);
                                                                client.CreateBoxDialog("You`ve received a Surge Headgear!");
                                                            }
                                                            else
                                                            {
                                                                if (Role.Core.Rate(1, 20))
                                                                {
                                                                    client.Inventory.Add(stream, 800414);
                                                                    client.CreateBoxDialog("You`ve received a Wave Shield!");
                                                                }
                                                                else
                                                                {
                                                                    if (Role.Core.Rate(1, 19))
                                                                    {
                                                                        client.Inventory.Add(stream, 823041);
                                                                        client.CreateBoxDialog("You`ve received a Soul Ring!");
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Role.Core.Rate(1, 18))
                                                                        {
                                                                            client.Inventory.Add(stream, 823043);
                                                                            client.CreateBoxDialog("You`ve received a Grass Bracelet!");
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Role.Core.Rate(1, 17))
                                                                            {
                                                                                client.Inventory.Add(stream, 823045);
                                                                                client.CreateBoxDialog("You`ve received an Ice Heavy Ring!");
                                                                            }
                                                                            else
                                                                            {
                                                                                if (Role.Core.Rate(1, 16))
                                                                                {
                                                                                    client.Inventory.Add(stream, 800070);
                                                                                    client.CreateBoxDialog("You`ve received a Jade Axe!");
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (Role.Core.Rate(1, 15))
                                                                                    {
                                                                                        client.Inventory.Add(stream, 800050);
                                                                                        client.CreateBoxDialog("You`ve received a Stiff Sword!");
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Role.Core.Rate(1, 14))
                                                                                        {
                                                                                            client.Inventory.Add(stream, 800140);
                                                                                            client.CreateBoxDialog("You`ve received a Phantom Katana!");
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (Role.Core.Rate(1, 13))
                                                                                            {
                                                                                                client.Inventory.Add(stream, 800230);
                                                                                                client.CreateBoxDialog("You`ve received a Celestial Spear!");
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Role.Core.Rate(1, 12))
                                                                                                {
                                                                                                    client.Inventory.Add(stream, 800520);
                                                                                                    client.CreateBoxDialog("You`ve received a Burnt Backsword!");
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (Role.Core.Rate(1, 11))
                                                                                                    {
                                                                                                        client.Inventory.Add(stream, 800615);
                                                                                                        client.CreateBoxDialog("You`ve received a Rune Bow!");
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (Role.Core.Rate(1, 10))
                                                                                                        {
                                                                                                            client.Inventory.Add(stream, 822056);
                                                                                                            client.CreateBoxDialog("You`ve received a Ocean Armor!");
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (Role.Core.Rate(1, 9))
                                                                                                            {
                                                                                                                client.Inventory.Add(stream, 822057);
                                                                                                                client.CreateBoxDialog("You`ve received a Heavenly Armor!");
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (Role.Core.Rate(1, 8))
                                                                                                                {
                                                                                                                    client.Inventory.Add(stream, 800019);
                                                                                                                    client.CreateBoxDialog("You`ve received a Aura Sword!");
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (Role.Core.Rate(1, 7))
                                                                                                                    {
                                                                                                                        client.Inventory.Add(stream, 800071);
                                                                                                                        client.CreateBoxDialog("You`ve received a Giant Axe!");
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (Role.Core.Rate(1, 6))
                                                                                                                        {
                                                                                                                            client.Inventory.Add(stream, 800141);
                                                                                                                            client.CreateBoxDialog("You`ve received a Scar Katana!");
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (Role.Core.Rate(1, 5))
                                                                                                                            {
                                                                                                                                client.Inventory.Add(stream, 800200);
                                                                                                                                client.CreateBoxDialog("You`ve received a Bloody Glaive!");
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (Role.Core.Rate(1, 4))
                                                                                                                                {
                                                                                                                                    client.Inventory.Add(stream, 800521);
                                                                                                                                    client.CreateBoxDialog("You`ve received a Dark Backsword!");
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (Role.Core.Rate(1, 3))
                                                                                                                                    {
                                                                                                                                        client.Inventory.Add(stream, 800617);
                                                                                                                                        client.CreateBoxDialog("You`ve received a Ancient Bow!");
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (Role.Core.Rate(1, 2))
                                                                                                                                        {
                                                                                                                                            client.Inventory.Add(stream, 801001);
                                                                                                                                            client.CreateBoxDialog("You~received~a~Mad~Dragon!");
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            client.Inventory.Add(stream, 801101);
                                                                                                                                            client.CreateBoxDialog("You~received~a~Hades~Hossu!");
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Role.Core.Rate(350, 919))
                            {
                                if (Role.Core.Rate(1, 16))
                                {
                                    client.Inventory.Add(stream, 800720);
                                    client.CreateBoxDialog("You`ve received Holy Beads of Magnanimity!");
                                }
                                else
                                {
                                    if (Role.Core.Rate(1, 15))
                                    {
                                        client.Inventory.Add(stream, 800913);
                                        client.CreateBoxDialog("You~received~a~Stygian~Knife~Soul!");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(1, 14))
                                        {
                                            client.Inventory.Add(stream, 800803);
                                            client.CreateBoxDialog("You`ve received a Gloom Pistol!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 13))
                                            {
                                                client.Inventory.Add(stream, 800808);
                                                client.CreateBoxDialog("You`ve received a Titan Rapier!");
                                            }
                                            else
                                            {
                                                if (Role.Core.Rate(1, 12))
                                                {
                                                    client.Inventory.Add(stream, 800014);
                                                    client.CreateBoxDialog("You`ve received a Radiant Sword!");
                                                }
                                                else
                                                {
                                                    if (Role.Core.Rate(1, 11))
                                                    {
                                                        client.Inventory.Add(stream, 800015);
                                                        client.CreateBoxDialog("You`ve received a Moon Hammer!");
                                                    }
                                                    else
                                                    {
                                                        if (Role.Core.Rate(1, 10))
                                                        {
                                                            client.Inventory.Add(stream, 800016);
                                                            client.CreateBoxDialog("You`ve received an Onima Katana!");
                                                        }
                                                        else
                                                        {
                                                            if (Role.Core.Rate(1, 9))
                                                            {
                                                                client.Inventory.Add(stream, 800214);
                                                                client.CreateBoxDialog("You`ve received a Snake Halbert!");
                                                            }
                                                            else
                                                            {
                                                                if (Role.Core.Rate(1, 8))
                                                                {
                                                                    client.Inventory.Add(stream, 800512);
                                                                    client.CreateBoxDialog("You`ve received a Lotus Backsword!");
                                                                }
                                                                else
                                                                {
                                                                    if (Role.Core.Rate(1, 7))
                                                                    {
                                                                        client.Inventory.Add(stream, 800613);
                                                                        client.CreateBoxDialog("You`ve received a Grace Bow!");
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Role.Core.Rate(1, 6))
                                                                        {
                                                                            client.Inventory.Add(stream, 823052);
                                                                            client.CreateBoxDialog("You`ve received a Violet Ring!");
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Role.Core.Rate(1, 5))
                                                                            {
                                                                                client.Inventory.Add(stream, 823053);
                                                                                client.CreateBoxDialog("You`ve received a Holy Heavy Ring!");
                                                                            }
                                                                            else
                                                                            {
                                                                                if (Role.Core.Rate(1, 4))
                                                                                {
                                                                                    client.Inventory.Add(stream, 823054);
                                                                                    client.CreateBoxDialog("You`ve received a Moon Bracelet!");
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (Role.Core.Rate(1, 3))
                                                                                    {
                                                                                        client.Inventory.Add(stream, 800253);
                                                                                        client.CreateBoxDialog("You~received~an~Annihilation~Scythe.");
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Role.Core.Rate(1, 2))
                                                                                        {
                                                                                            client.Inventory.Add(stream, 801002);
                                                                                            client.CreateBoxDialog("You~received~a~Blazing~Scale!");
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            client.Inventory.Add(stream, 801102);
                                                                                            client.CreateBoxDialog("You~received~a~Universal~Hossu!");
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Role.Core.Rate(152, 569))
                                {
                                    if (Role.Core.Rate(30, 152))
                                    {
                                        client.Inventory.Add(stream, 800415);
                                        client.CreateBoxDialog("You~received~a~ThunderShield.");
                                    }
                                    else
                                    {
                                        if (Role.Core.Rate(30, 122))
                                        {
                                            client.Inventory.Add(stream, 820056);
                                            client.CreateBoxDialog("You~received~an~Azure~Hat.");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(30, 92))
                                            {
                                                client.Inventory.Add(stream, 820057);
                                                client.CreateBoxDialog("You~received~a~Solar~Hat.");
                                            }
                                            else
                                            {
                                                if (Role.Core.Rate(32, 62))
                                                {
                                                    client.Inventory.Add(stream, 821028);
                                                    client.CreateBoxDialog("You~received~a~Spirit~Necklace.");
                                                }
                                                else
                                                {
                                                    client.Inventory.Add(stream, 821030);
                                                    client.CreateBoxDialog("You~received~an~Ebony~Bag.");
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Role.Core.Rate(400, 417))
                                    {
                                        if (Role.Core.Rate(1, 23))
                                        {
                                            client.Inventory.Add(stream, 800722);
                                            client.CreateBoxDialog("You`ve received Holy Beads of Consciousness!");
                                        }
                                        else
                                        {
                                            if (Role.Core.Rate(1, 22))
                                            {
                                                client.Inventory.Add(stream, 800916);
                                                client.CreateBoxDialog("You~received~a~Dominant~Knife~Soul!");
                                            }
                                            else
                                            {
                                                if (Role.Core.Rate(1, 21))
                                                {
                                                    client.Inventory.Add(stream, 800804);
                                                    client.CreateBoxDialog("You`ve received a Time Pistol!");
                                                }
                                                else
                                                {
                                                    if (Role.Core.Rate(1, 20))
                                                    {
                                                        client.Inventory.Add(stream, 800809);
                                                        client.CreateBoxDialog("You`ve received a Destiny Rapier!");
                                                    }
                                                    else
                                                    {
                                                        if (Role.Core.Rate(1, 19))
                                                        {
                                                            client.Inventory.Add(stream, 822053);
                                                            client.CreateBoxDialog("You`ve received a Whirlpool Armor!");
                                                        }
                                                        else
                                                        {
                                                            if (Role.Core.Rate(1, 18))
                                                            {
                                                                client.Inventory.Add(stream, 822055);
                                                                client.CreateBoxDialog("You`ve received a Waterflow Armor!");
                                                            }
                                                            else
                                                            {
                                                                if (Role.Core.Rate(1, 17))
                                                                {
                                                                    client.Inventory.Add(stream, 800000);
                                                                    client.CreateBoxDialog("You`ve received a Tomb Blade!");
                                                                }
                                                                else
                                                                {
                                                                    if (Role.Core.Rate(1, 16))
                                                                    {
                                                                        client.Inventory.Add(stream, 800110);
                                                                        client.CreateBoxDialog("You`ve received a Grim Hammer!");
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Role.Core.Rate(1, 15))
                                                                        {
                                                                            client.Inventory.Add(stream, 800017);
                                                                            client.CreateBoxDialog("You`ve received a Stealth Katana!");
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Role.Core.Rate(1, 14))
                                                                            {
                                                                                client.Inventory.Add(stream, 800320);
                                                                                client.CreateBoxDialog("You`ve received an Archon Wand!");
                                                                            }
                                                                            else
                                                                            {
                                                                                if (Role.Core.Rate(1, 13))
                                                                                {
                                                                                    client.Inventory.Add(stream, 800513);
                                                                                    client.CreateBoxDialog("You`ve received a Pneuma Backsword!");
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (Role.Core.Rate(1, 12))
                                                                                    {
                                                                                        client.Inventory.Add(stream, 800616);
                                                                                        client.CreateBoxDialog("You`ve received a Winged Bow!");
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Role.Core.Rate(1, 11))
                                                                                        {
                                                                                            client.Inventory.Add(stream, 823055);
                                                                                            client.CreateBoxDialog("You~received~a~Saint~Ring.");
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (Role.Core.Rate(1, 10))
                                                                                            {
                                                                                                client.Inventory.Add(stream, 823057);
                                                                                                client.CreateBoxDialog("You~received~a~Saint~Heavy~Ring.");
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Role.Core.Rate(1, 9))
                                                                                                {
                                                                                                    client.Inventory.Add(stream, 823056);
                                                                                                    client.CreateBoxDialog("You~received~a~Saint~Bracelet.");
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (Role.Core.Rate(1, 8))
                                                                                                    {
                                                                                                        client.Inventory.Add(stream, 824017);
                                                                                                        client.CreateBoxDialog("You~received~Saint~Boots.");
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (Role.Core.Rate(1, 7))
                                                                                                        {
                                                                                                            client.Inventory.Add(stream, 821031);
                                                                                                            client.CreateBoxDialog("You~received~a~Saint~Necklace.");
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (Role.Core.Rate(1, 6))
                                                                                                            {
                                                                                                                client.Inventory.Add(stream, 821032);
                                                                                                                client.CreateBoxDialog("You~received~a~Saint~Bag.");
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (Role.Core.Rate(1, 5))
                                                                                                                {
                                                                                                                    client.Inventory.Add(stream, 820071);
                                                                                                                    client.CreateBoxDialog("You~received~a~Saint~Headgear.");
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (Role.Core.Rate(1, 4))
                                                                                                                    {
                                                                                                                        client.Inventory.Add(stream, 820072);
                                                                                                                        client.CreateBoxDialog("You~received~a~Holy~Headgear.");
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (Role.Core.Rate(1, 3))
                                                                                                                        {
                                                                                                                            client.Inventory.Add(stream, 800254);
                                                                                                                            client.CreateBoxDialog("You~received~a~Suffering~Scythe.");
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (Role.Core.Rate(1, 2))
                                                                                                                            {
                                                                                                                                client.Inventory.Add(stream, 801003);
                                                                                                                                client.CreateBoxDialog("You~received~a~Dragon~Chant!");
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                client.Inventory.Add(stream, 801103);
                                                                                                                                client.CreateBoxDialog("You~received~a~Heaven~Hossu!");
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        client.Inventory.Add(stream, 800421);
                                        client.CreateBoxDialog("You~received~a~Saint~Shield.");
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 3007104:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3007104, 15))
                        {
                            client.Inventory.Remove(3007104, 15, stream);
                            var array = Database.ItemType.Garments.Values.ToArray();
                            var _item = array[Program.GetRandom.Next(0, array.Length)];
                            Database.CoatStorage.StorageItem DbGarment;
                            if (Database.CoatStorage.Garments.TryGetValue(_item.ID, out DbGarment))
                            {
                                client.Inventory.Add(stream, _item.ID, 1, 0, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You received a 1%Blessed " + _item.Name + ".");
                            }
                        }
                        else
                        {
                            client.SendSysMesage("You should have 15 Garment Fragments to combine them into a Garment.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 3004260:
                    {
                        if (client.Inventory.ContainItemWithStack(3004260, 7))
                        {
                            client.Inventory.RemoveStackItem(3004260, 7, stream);
                            client.Inventory.Add(stream, 720957);
                            client.SendSysMesage("You~obtained~Dragon~Soul~Pack.");
                            break;
                        }
                        else if (client.Inventory.ContainItemWithStack(3004260, 1))
                        {
                            client.Inventory.RemoveStackItem(3004260, 1, stream);
                            client.GainExpBall(300);
                            client.SendSysMesage("You~obtained~30~min~exp.");
                            break;
                        }
                        if (client.Inventory.Contain(3004260, 1))
                        {
                            client.Inventory.Remove(3004260, 1, stream);
                            client.GainExpBall(300);
                            client.SendSysMesage("You~obtained~30~min~exp.");
                            break;
                        }
                        break;
                    }
                case 3008100:
                    {
                        client.SendSysMesage("This packet contains all kinds of necessary materials needed to build a Class 6 house.", MsgMessage.ChatMode.System);
                        break;
                    }
                default:
                    handled = false;
                    break;
            }
        }
    }
}
