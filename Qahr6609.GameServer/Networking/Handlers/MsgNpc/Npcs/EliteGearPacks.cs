using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;
using Conquer6609.MsgInterServer.Packets;
using Conquer6609.Game.MsgFloorItem;
using Project2D.Database;
using Conquer6609.Game.MsgTournaments;

namespace Conquer6609.Game.MsgNpc
{
    public unsafe partial class NpcHandler
    {
        [NpcAttribute(NpcID.Level67EliteHeadgearPack)]
        public static void Level67EliteHeadgearPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        if (client.Player.Class <= 15)
                        {
                            data.AddOption("Elite~War~Coronet.", 1);
                            data.AddOption("Elite~Heart~of~Ocean.", 2);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 25)
                            {
                                data.AddOption("Elite~Gold~Helmet.", 3);
                                data.AddOption("Elite~Heart~of~Ocean.", 2);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 45)
                                {
                                    data.AddOption("Elite~Leopard~Hat.", 4);
                                    data.AddOption("Elite~Heart~of~Ocean.", 2);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 55)
                                    {
                                        data.AddOption("Elite~Blaze~Hood.", 5);
                                        data.AddOption("Elite~Heart~of~Ocean.", 2);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 65)
                                        {
                                            data.AddOption("Elite~Headband~Of~Repentance.", 6);
                                            data.AddOption("Elite~Heart~of~Ocean.", 2);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 75)
                                            {
                                                data.AddOption("Elite~Calm~Hat.", 7);
                                                data.AddOption("Elite~Heart~of~Ocean.", 2);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 85)
                                                {
                                                    data.AddOption("Elite~WindHood.", 8);
                                                    data.AddOption("Elite~Heart~of~Ocean.", 2);
                                                    data.FinalizeDialog();
                                                }
                                                else if (client.Player.Class <= 145)
                                                {
                                                    data.AddOption("Elite~Shark~Cap.", 9);
                                                    data.AddOption("Elite~Heart~of~Ocean.", 2);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Elite~WindWalker~Cap.", 10);
                                                    data.AddOption("Elite~Heart~of~Ocean.", 2);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 101068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Shark~Cap!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }

                case 9:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 114068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Shark~Cap!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 148068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~received~an~Elite~Wind~Hood~and~an~EXP~Ball!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 144068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Calm~Hat.");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 143068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Headband~Of~Repentance!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 123068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Blaze~Hood!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 113038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Leopard~Hat!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 111068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Gold~Helmet!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 118068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~War~Coronet!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!client.Inventory.Remove(728245, 1, stream)) break;
                            client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 117068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Heart~of~Ocean!");
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Level55EliteWeaponPack)]
        public static void Level55EliteWeaponPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        if (client.Player.Class <= 15)
                        {
                            data.AddOption("Elite~Cold~Blade.", 1);
                            data.AddOption("Elite~Mirrored~Sword.", 2);
                            data.AddOption("Elite~Battle~Club.", 3);
                            data.AddOption("Others.", 4);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 25)
                            {
                                data.AddOption("Elite~Octagonal~Shield.", 22);
                                data.AddOption("Others.", 4);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 45)
                                {
                                    data.AddOption("Elite~Long~Bow.", 23);
                                    data.AddOption("Elite~Solar~Knife.", 24);
                                    data.AddOption("Others.", 4);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 55)
                                    {
                                        data.AddOption("Elite~Fierce~Scythe.", 25);
                                        data.AddOption("Elite~Night~Katana.", 26);
                                        data.AddOption("Others.", 4);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 65)
                                        {
                                            data.AddOption("Elite~Beads~Of~Truth.", 27);
                                            data.AddOption("Others.", 4);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 75)
                                            {
                                                data.AddOption("Elite~Blood~Pistol.", 28);
                                                data.AddOption("Elite~Melody~Rapier.", 29);
                                                data.AddOption("Others.", 4);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 85)
                                                {
                                                    data.AddOption("Elite~Flame~Nunchaku.", 30);
                                                    data.AddOption("Others.", 4);
                                                    data.FinalizeDialog();
                                                }
                                                else if (client.Player.Class <= 145)
                                                {
                                                    data.AddOption("Elite~Copper~Backsword.", 31);
                                                    data.AddOption("Elite~Bend~Spear.", 32);
                                                    data.AddOption("Others.", 4);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Elite~WindWalker~Fan.", 33);
                                                    data.AddOption("Others.", 4);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 33:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 626108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~WindWalker~Fan!");
                        }
                        break;
                    }
                case 32:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 560108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Bend~Spear!");
                        }
                        break;
                    }
                case 31:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 421108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Copper~Backsword!");
                        }
                        break;
                    }
                case 30:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 617108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~an~Elite~Flame~Nunchaku!");
                        }
                        break;
                    }
                case 29:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 611108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Melody~Rapier.");
                        }
                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 612108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Blood~Pistol.");
                        }
                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 610108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Beads~Of~Truth!");
                        }
                        break;
                    }
                case 26:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 601108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Night~Katana!");
                        }
                        break;
                    }
                case 25:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 511108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Fierce~Scythe(B).");
                        }
                        break;
                    }
                case 24:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 613098, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Solar~Knife(B).");
                        }
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 500098, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Long~Bow!");
                        }
                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 900028, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Octagonal~Shield!");
                        }
                        break;
                    }

                case 4:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Elite~Cold~Blade.", 7);
                        data.AddOption("Elite~Mirrored~Sword.", 8);
                        data.AddOption("Elite~Scorpion~Hook.", 9);
                        data.AddOption("Elite~Leopard~Whip.", 10);
                        data.AddOption("Elite~War~Axe.", 11);
                        data.AddOption("Elite~Iron~Hammer.", 12);
                        data.AddOption("Elite~Battle~Club.", 13);
                        data.AddOption("Next.", 5);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Elite~Gold~Scepter.", 14);
                        data.AddOption("Elite~Steel~Dagger.", 15);
                        data.AddOption("Elite~Sharp~Glaive.", 16);
                        data.AddOption("Elite~Destiny~Poleaxe.", 17);
                        data.AddOption("Elite~Giant~Long~Hammer.", 18);
                        data.AddOption("Elite~Bend~Spear.", 19);
                        data.AddOption("Previous.", 4);
                        data.AddOption("Next.", 6);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Elite~Spring~Wand.", 20);
                        data.AddOption("Elite~Star~Halbert.", 21);
                        data.AddOption("Previous.", 5);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 580108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Star~Halbert!");
                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 561108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Spring~Wand!");
                        }
                        break;
                    }
                case 19:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 560108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Bend~Spear!");
                        }
                        break;
                    }
                case 18:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 540108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Giant~Long~Hammer!");
                        }
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 530108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Destiny~Poleaxe!");
                        }
                        break;
                    }
                case 16:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 510108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Sharp~Glaive!");
                        }
                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 490108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Steel~Dagger!");
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 481108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Gold~Scepter!");
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 480108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Battle~Club!");
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 460108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Iron~Hammer!");
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 450108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~War~Axe!");
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 440108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Leopard~Whip!");
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 430108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Scorpion~Hook!");
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 420108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Mirrored~Sword!");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 410108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Cold~Blade!");
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 410108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Cold~Blade!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 420108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Mirrored~Sword!");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 480108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Battle~Club!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Level52UniqueHeadgearPack)]
        public static void Level52UniqueHeadgearPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Unique~Thread~Earring.", 1);
                        if (client.Player.Class <= 15)
                        {
                            data.AddOption("Unique~Gold~Coronet.", 2);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 25)
                            {
                                data.AddOption("Unique~Pearl~Helmet.", 3);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 45)
                                {
                                    data.AddOption("Unique~Jackal~Hat.", 4);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 55)
                                    {
                                        data.AddOption("Unique~Demon~Hood.", 5);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 65)
                                        {
                                            data.AddOption("Unique~Pure~Headband.", 6);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 75)
                                            {
                                                data.AddOption("Unique~Tranquil~Hat.", 7);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 85)
                                                {
                                                    data.AddOption("Unique~Mercy~Hood.", 8);
                                                    data.FinalizeDialog();
                                                }
                                                else if (client.Player.Class <= 145)
                                                {
                                                    data.AddOption("Unique~Lotus~Cap.", 9);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Unique~WindWalker~Cap.", 10);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 101047, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~WindWalker~Cap!");
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 117057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Thread~Earring!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 118057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Gold~Coronet!");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 111057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Pearl~Helmet!");
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 113027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Jacka~lHat!");
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 123057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Demon~Hood!");
                        }

                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 143057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Pure~Headband!");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 144057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Tranquil~Hat.");
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 148057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~a~Unique~MercyHood!");
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 114057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Lotus~Cap!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Level50UniqueWeaponPack)]
        public static void Level50UniqueWeaponPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        if (client.Player.Class <= 25)
                        {
                            data.AddOption("Unique~Moon~Blade", 1);
                            data.AddOption("Unique~Fang~Sword.", 2);
                            data.AddOption("Unique~Antler~Hook.", 3);
                            data.AddOption("Unique~Twin~Whip.", 4);
                            data.AddOption("Unique~Peace~Axe.", 5);
                            data.AddOption("Unique~Melon~Hammer.", 6);
                            data.AddOption("Unique~War~Club.", 7);
                            data.AddOption("Next.", 10);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 45)
                            {
                                data.AddOption("Unique~Qin~Bow.", 20);
                                data.AddOption("Unique~Sparky~Knife.", 21);
                                data.AddOption("Others.", 9);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 55)
                                {
                                    data.AddOption("Unique~Fang~Katana.", 22);
                                    data.AddOption("Unique~Evil~Scythe.", 23);
                                    data.AddOption("Others.", 9);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 65)
                                    {
                                        data.AddOption("Unique~Jeweled~Prayer~Beads.", 24);
                                        data.AddOption("Others.", 9);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 75)
                                        {
                                            data.AddOption("Unique~Gold~Pistol.", 25);
                                            data.AddOption("Unique~Stream~Rapier.", 26);
                                            data.AddOption("Others.", 9);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 85)
                                            {
                                                data.AddOption("Unique~Dragon~Nunchaku.", 27);
                                                data.AddOption("Others.", 9);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 145)
                                                {
                                                    data.AddOption("Unique~Iron~Backsword.", 28);
                                                    data.AddOption("Others.", 9);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Unique~WindWalker~Fan.", 29);
                                                    data.AddOption("Others.", 9);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 29:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 626088, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~WindWalker~Fan!");
                        }
                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 421087, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Iron~Backsword!");
                        }
                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 617097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~a~Unique~Dragon~Nunchaku!");
                        }
                        break;
                    }
                case 26:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 611097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Stream~Rapier.");
                        }
                        break;
                    }
                case 25:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 612097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Gold~Pistol.");
                        }
                        break;
                    }
                case 24:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 610097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Jeweled~Prayer~Beads!");
                        }
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 511097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Evil~Scythe(B)!");
                        }
                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 601097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Fang~Katana!");
                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 500087, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Qin~Bow!");
                        }
                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 613087, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Sparky~Knife(B)!");
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 410097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Moon~Blade!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 420097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Fang~Sword!");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 430097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Antler~Hook!");
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 440097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Twin~Whip!");
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 450097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Peace~Axe!");
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 460097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Melon~Hammer!");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 480097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~War~Club!");
                        }
                        break;
                    }
                case 8:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Unique~Shaolin~Wand.", 17);
                        data.AddOption("Unique~Gold~Halbert.", 18);
                        data.AddOption("Previous.", 10);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        data.AddOption("Unique~Moon~Blade", 1);
                        data.AddOption("Unique~Fang~Sword.", 2);
                        data.AddOption("Unique~Antler~Hook.", 3);
                        data.AddOption("Unique~Twin~Whip.", 4);
                        data.AddOption("Unique~Peace~Axe.", 5);
                        data.AddOption("Unique~Melon~Hammer.", 6);
                        data.AddOption("Unique~War~Club.", 7);
                        data.AddOption("Next.", 10);
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Unique~Wish~Scepter.", 11);
                        data.AddOption("Unique~Cao~Dagger.", 12);
                        data.AddOption("Unique~Union~Glaive.", 13);
                        data.AddOption("Unique~Twin~Poleaxe.", 14);
                        data.AddOption("Unique~Great~Long~Hammer.", 15);
                        data.AddOption("Unique~Luck~Spear", 16);
                        data.AddOption("Previous.", 9);
                        data.AddOption("Next.", 8);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 481097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Wish~Scepter!");
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 490097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Cao~Dagger!");
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 510097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Union~Glaive!");

                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 530097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Twin~Poleaxe!");
                        }
                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 540097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Great~Long~Hammer!");
                        }
                        break;
                    }
                case 16:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 560097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Luck~Spear!");
                        }
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 561097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Shaolin~Wand!");
                        }
                        break;
                    }
                case 18:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 580097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Gold~Halbert!");
                        }
                        break;
                    }
            }
        }
    }
}
