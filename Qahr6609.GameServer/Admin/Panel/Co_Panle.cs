using Conquer6609.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Conquer6609.Client;
using Conquer6609.Game.MsgServer;
using Conquer6609.WindowsAPI;
using Project2D.Database;


namespace Conquer6609
{
    public partial class Co_Panle : DevExpress.XtraEditors.XtraForm
    {
        public Co_Panle()
        {
            InitializeComponent();
            loadePlayerName();
        }
       
        #region Buttn Panle
        // accuont data
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage1;
            LoadeAllUserName();//لود لاسماء الحسابات
            
        }
        //new accuont
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage2;
        }
        private void barButtonItem19_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage3;
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage3;
        }
        private void ButtanBan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage6;
        }
        private void BtnChatAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage8;
        }
        private void PuttnSpill_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage5;
            loadePlayerName();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                try
                {
                    var stream = rec.GetStream();
                    foreach (var spell in Database.Server.Magic.Values)
                    {
                        try
                        {
                            if (spell.Keys.Count > 1)
                            {

                                var sp = spell[(ushort)(spell.Keys.Count -1 )];

                                SpellList.Items.Add(sp.Name + "-" + sp.ID);

                            }
                        }
                        catch (Exception ex)
                        {
                            MyConsole.WriteLine(ex.Message);
                        }

                    }
                }
                catch(Exception ex)
                {
                    MyConsole.WriteLine(ex.Message);
                }

            }
        }
        private void ButtnChatPlaer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage7;
            loadChatPanl();
        }
        private void ButtonChiPanle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage9;
            Chi();
        }
        private void SkillTrojan_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage11;
        }
        private void SkillFire_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage12;
        }

        private void SkillArcher_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage13;
        }

        private void SkillWarrior_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage14;
        }

        private void SkillMonk_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage16;
        }

        private void SkillNinja_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage15;
        }

        private void SkillParite_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage17;
        }
        private void BtnWaterSkll_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage18;
        }
        private void BtnWater_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage18;
        }

        private void DragonWarrir_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage19;
        }

        private void WindWilker_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage20;
        }
        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (var item in Conquer6609.Role.Instance.JiangHu.Poll.Values)
            {
                Conquer6609.Client.GameClient client = null;
                if (Conquer6609.Database.Server.GamePoll.TryGetValue(item.UID, out client))
                {
                    comboBox1.Items.Add(client.Player.Name);
                }
            }
            JiangHu f = new JiangHu();
            f.ShowDialog();
        }
        private void AllChar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage10;
            LoadAttack();
            UpdateTxt(); // اضافه القيم فى التيكست
        }
        //تحميل اسماء الاعبين
        private void loadePlayerName()
        {
            SpellUserName.Items.Clear();
            Status_AllUser.Items.Clear();
            CBPlayerName.Items.Clear();
            BanPlayerName.Items.Clear();
            ComName.Items.Clear();
            ComName.Items.Clear();
            ChiPlayerName.Items.Clear();
            if (Conquer6609.Database.Server.GamePoll != null)
            {
                foreach (var user in Conquer6609.Database.Server.GamePoll.Values)
                {
                    SpellUserName.Items.Add(user.Player.Name);
                    Status_AllUser.Items.Add(user.Player.Name);
                    CBPlayerName.Items.Add(user.Player.Name);
                    BanPlayerName.Items.Add(user.Player.Name);
                    ComName.Items.Add(user.Player.Name);
                    ChiPlayerName.Items.Add(user.Player.Name);

                }
            }
        }
        private void SendMessagebox(string Msg,string Snedr= "scripthazem",MessageBoxButtons Buttun = MessageBoxButtons.OK,MessageBoxIcon Icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(Msg, Snedr, Buttun, Icon);
        }
        #endregion

        #region Class Accuont

        private void LoadeAllUserName()
        {
            try
            {
                Accuont_AllUser.Items.Clear();
                MySqlReader reader = new MySqlReader(new MySqlCommand(MySqlCommandType.SELECT).Select("accounts"));
                while (reader.Read())
                {
                    Accuont_AllUser.Items.Add(reader.ReadLongString("Username"));

                }
            }
            catch (Exception e) { SendMessagebox(e.Message); }
        }
        /// <summary>Same as Qahr6609.AccServer\Database\PasswordHasher: Laravel bcrypt "$2y$10$...". A value that is
        /// already a hash (the panel shows the stored hash) is saved unchanged.</summary>
        static string PanelPasswordHash(string password)
        {
            if (string.IsNullOrEmpty(password))
                return password;
            if (password.Length == 60 && password.StartsWith("$2", StringComparison.Ordinal) && password[3] == '$')
                return password;
            byte[] salt = new byte[16];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                rng.GetBytes(salt);
            return Org.BouncyCastle.Crypto.Generators.OpenBsdBCrypt.Generate("2y", password.ToCharArray(), salt, 10);
        }
        private void AddNewAcc_Click(object sender, EventArgs e)
        {
            try
            {
                if (AccName.Text != "")
                {
                    var cmd = new MySqlCommand(MySqlCommandType.INSERT);
                    cmd.Insert("accounts").Insert("Username", AccName.Text).Insert("Password", PanelPasswordHash(AccPass.Text)).Insert("Email", AccEmail.Text).Execute();
                    System.Windows.Forms.MessageBox.Show("Done");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void AccDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (Accuont_AllUser.SelectedItem != null)
                {
                    var cmd = new MySqlCommand(MySqlCommandType.DELETE);
                    cmd.Delete("accounts", "Username", Accuont_AllUser.SelectedItem.ToString());
                    cmd.Execute2();
                    SendMessagebox("Done Delete");
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        private void Change_Click(object sender, EventArgs e)
        {
            try
            {
                if (NewID.Text != "")
                {
                    var cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmd.Update("accounts").Set("Username", NewName.Text).Set("Password", PanelPasswordHash(NewPass.Text)).Set("Email", NewEmil.Text)
                    .Where("EntityID", NewID.Text);
                    if (cmd.Execute2() > 0)
                    {
                        SendMessagebox("Done Save");

                    }
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        private void Accuont_AllUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Accuont_AllUser.SelectedItem != null)
                {
                    using (var cmd = new MySqlCommand(MySqlCommandType.SELECT))
                    {
                        cmd.Select("accounts").Where("Username", Accuont_AllUser.SelectedItem.ToString());
                        using (MySqlReader rdr = new MySqlReader(cmd))
                        {
                            if (rdr.Read())
                            {

                                Acc_UID.Text = rdr.ReadInt32("EntityID").ToString();
                                Acc_UserName.Text = rdr.ReadString("Username");
                                Acc_Pass.Text = rdr.ReadString("Password");
                                Acc_Emil.Text = rdr.ReadString("Email");
                                Acc_Mac.Text = rdr.ReadString("EarthID");
                                Acc_Ip.Text = rdr.ReadString("IP");
                                //
                                NewID.Text = rdr.ReadInt32("EntityID").ToString();
                                NewName.Text = rdr.ReadString("Username");
                                NewPass.Text = rdr.ReadString("Password");
                                NewEmil.Text = rdr.ReadString("Email");
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("Username not found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        #endregion

        #region Class Stauts
       
        private void Status_Ref_Click(object sender, EventArgs e)
        {
            loadePlayerName();
        }
        private void Status_AllUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCharInfo();
        }
        private void LoadCharInfo()
        {
            try
            {
                Conquer6609.Client.GameClient client = null;
                client = Conquer6609.Client.GameClient.CharacterFromName(Status_AllUser.SelectedItem.ToString());
                if (client == null)
                    return;
                CPS.Text = client.Player.ConquerPoints.ToString();
                BCps.Text = client.Player.BoundConquerPoints.ToString();
                Money.Text = client.Player.Money.ToString();
                Level.Text = client.Player.Level.ToString();
                WhPass.Text = client.Player.SecurityPassword.ToString();
                if (client.Player.MyGuild != null)
                GuildName.Text = client.Player.MyGuild.GuildName;
                if (client.Player.MyClan != null)
                Clan.Text = client.Player.ClanName;
                Sex.Text = client.Player.GetGender.ToString();
                switch (client.Player.Reborn)
                {
                    case 2: Reborn.Text = "2nd Reborn"; break;
                    case 1: Reborn.Text = "1st Reborn"; break;
                    default: Reborn.Text = "Nono"; break;
                }
                switch (client.Player.Class)
                {
                    #region Get Class
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                        {
                            Class.Text = "Trojan";
                            break;
                        }
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                        {
                            Class.Text = "Warrior";
                            break;
                        }
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                        {
                            Class.Text = "Archer";
                            break;
                        }
                    case 50:
                    case 51:
                    case 52:
                    case 53:
                    case 54:
                    case 55:
                        {
                            Class.Text = "Ninja";
                            break;
                        }
                    case 60:
                    case 61:
                    case 62:
                    case 63:
                    case 64:
                    case 65:
                        {
                            Class.Text = "Monk";
                            break;
                        }
                    case 130:
                    case 131:
                    case 132:
                    case 133:
                    case 134:
                    case 135:
                        {
                            Class.Text = "Water";
                            break;
                        }
                    case 140:
                    case 141:
                    case 142:
                    case 143:
                    case 144:
                    case 145:
                        {
                            Class.Text = "Fire";
                            break;
                        }
                    default: Class.Text = "Taoist"; break;
                    #endregion
                }
                double x = 0;
                if ((client.Player.ExpireVip > DateTime.Now))
                {
                    x = (client.Player.ExpireVip - DateTime.Now).TotalDays;
                }
                VIP.Text = client.Player.VipLevel.ToString();
                VipDay.Text = x.ToString();
                OnlinePoint.Text = client.Player.OnlineMinutes.ToString();
                Status_Sbouse.Text = client.Player.Spouse;
                BanckMoney.Text = client.Player.WHMoney.ToString();
                VotePoint.Text = client.Player.VotePoints.ToString();
                PkPoint.Text = client.Player.PKPoints.ToString();
                Donation.Text = client.Player.Nobility.Donation.ToString();
                Rank.Text = client.Player.NobilityRank.ToString();
                Donate.Text = client.Player.RacePoints.ToString();
                Agility.Text = client.Player.Agility.ToString();
                Strength.Text = client.Player.Strength.ToString();
                Status_Vit.Text = client.Player.Vitality.ToString();
                Spirit.Text = client.Player.Spirit.ToString();
                Atributes.Text = client.Player.Atributes.ToString();
                Attack.Text = client.Status.MaxAttack.ToString();
                MinAttack.Text = client.Status.MinAttack.ToString();
                brek.Text = client.Status.Breakthrough.ToString();
                Defnce.Text = client.Status.Defence.ToString();
                HitPoint.Text = client.Status.MaxHitpoints.ToString();
                Mana.Text = client.Status.MaxMana.ToString();
                Strake.Text = client.Status.CriticalStrike.ToString();
                Immunty.Text = client.Status.Immunity.ToString();
                AntBrake.Text = client.Status.Counteraction.ToString();
                Power.Text = client.Player.BattlePower.ToString();
                Mg_Attack.Text = client.Status.MagicAttack.ToString();
                Mg_Defen.Text = client.Status.MagicDefence.ToString();
                Bless.Text = client.Status.ItemBless.ToString();
                Block.Text = client.Status.Block.ToString();
                Map.Text = client.Player.Map.ToString();
                X.Text = client.Player.X.ToString();
                Y.Text = client.Player.Y.ToString();
                Mac.Text = client.Player.Mac.ToString();
                UID.Text = client.Player.UID.ToString();
                Damage.Text = client.Status.Damage.ToString();
                using (var cmd = new MySqlCommand(MySqlCommandType.SELECT))
                {
                    cmd.Select("accounts").Where("EntityID", client.Player.UID);
                    using (MySqlReader rdr = new MySqlReader(cmd))
                    {
                        if (rdr.Read())
                        {

                            UserName.Text = rdr.ReadUInt32("Username").ToString();
                            Pass.Text = rdr.ReadUInt32("Password").ToString();
                            IP.Text = rdr.ReadUInt32("IP").ToString();
                            Mac.Text = rdr.ReadUInt32("EarthID").ToString();

                        }

                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }
        private void SaveCharInfo()
        {
            try
            {
                Conquer6609.Client.GameClient client = null;
                if (Status_AllUser.SelectedItem == null) return;
                client = Conquer6609.Client.GameClient.CharacterFromName(Status_AllUser.SelectedItem.ToString());
                if (client == null)
                    return;
                client.Player.Map = Convert.ToUInt16(Map.Text);
                client.Player.X = Convert.ToUInt16(X.Text);
                client.Player.Y = Convert.ToUInt16(Y.Text);
                client.Player.ConquerPoints = Convert.ToUInt32(CPS.Text);
                client.Player.BoundConquerPoints = Convert.ToInt32(BCps.Text);
                client.Player.Money = Convert.ToInt64(Money.Text);
                client.Player.Level = Convert.ToByte(Level.Text);
                client.Player.SecurityPassword = Convert.ToUInt32(WhPass.Text);
                if(Class.Text == "15"||Class.Text == "25"||Class.Text == "45"||Class.Text == "55"||Class.Text == "65"||Class.Text == "75"||Class.Text == "135"||Class.Text == "145")
                client.Player.Class = Convert.ToByte(Class.Text);
                if (Reborn.Text == "1" || Reborn.Text == "2")
                client.Player.Reborn = Convert.ToByte(Reborn.Text);
                client.Player.ExpireVip = DateTime.Now.AddDays(double.Parse(VipDay.Text));
                client.Player.VipLevel = byte.Parse(VIP.Text);
                client.Player.OnlineMinutes = Convert.ToUInt32(OnlinePoint.Text);
                client.Player.WHMoney = Convert.ToInt64(BanckMoney.Text);
                client.Player.VotePoints = Convert.ToUInt32(VotePoint.Text);
                client.Player.PKPoints = Convert.ToUInt16(PkPoint.Text);
                client.Player.Nobility.Donation = Convert.ToUInt64(Donation.Text);
                client.Player.RacePoints = Convert.ToUInt32(Donate.Text);
                client.Player.Agility = Convert.ToUInt16(Agility.Text);
                client.Player.Strength = Convert.ToUInt16(Strength.Text);
                client.Player.Spirit = Convert.ToUInt16(Spirit.Text);
                client.Player.Vitality = Convert.ToUInt16(Status_Vit.Text);
                client.Player.Atributes = Convert.ToUInt16(Atributes.Text);
                client.Status.MinAttack = Convert.ToUInt32(MinAttack.Text);
                client.Status.MaxAttack = Convert.ToUInt32(Attack.Text);
                client.Status.Breakthrough = Convert.ToUInt32(brek.Text);
                client.Status.Defence = Convert.ToUInt32(Defnce.Text);
                client.Status.MaxHitpoints = Convert.ToUInt32(HitPoint.Text);
                client.Status.MaxMana = Convert.ToUInt32(Mana.Text);
                client.Status.CriticalStrike = Convert.ToUInt32(Strake.Text);
                client.Status.MagicAttack = Convert.ToUInt32(Mg_Attack.Text);
                client.Status.MagicDefence = Convert.ToUInt32(Mg_Defen.Text);
                client.Status.ItemBless = Convert.ToUInt32(Bless.Text);
                client.Status.Block = Convert.ToUInt32(Block.Text);
                client.Status.Damage = uint.Parse(Damage.Text);
                using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.SendUpdate(stream, client.Player.VipLevel, Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                    client.Player.UpdateVip(stream);
                    client.Player.SendUpdate(stream, client.Player.RacePoints, Game.MsgServer.MsgUpdate.DataType.RaceShopPoints);
                    client.Player.SendUpdate(stream, client.Player.ConquerPoints, Game.MsgServer.MsgUpdate.DataType.ConquerPoints);
                    client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, Game.MsgServer.MsgUpdate.DataType.BoundConquerPoints);
                    client.Player.SendUpdate(stream, client.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                    client.Player.SendUpdate(stream, client.Player.Level, Game.MsgServer.MsgUpdate.DataType.Level);
                    client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                    client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                    client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                    client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.VirtutePoints);
                    client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                    client.Player.SendUpdate(stream, client.Player.WHMoney, Game.MsgServer.MsgUpdate.DataType.WHMoney);

                }
               

            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        private void Save_Click(object sender, EventArgs e)
        {
            SaveCharInfo();
        }
        #endregion

        #region Class Items
        private void ButtunItems_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                navigationFrame1.SelectedPage = navigationPage4;
                loadePlayerName();
                CBAddItems.Items.Clear();
                //لود الايتمز
                foreach (var item in Conquer6609.Database.Server.ItemsBase.Values)
                {
                    CBAddItems.Items.Add(item.Name + " " + item.ID);
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
      
        }
        //اضافه ايتمز
        private void AddItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (CBPlayerName.SelectedItem != null)
                {
                    Conquer6609.Client.GameClient client = null;
                    client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                    if (client == null)
                        return;

                    Conquer6609.Database.ItemType.DBItem DBItem;
                    string[] id = CBAddItems.SelectedItem.ToString().Split(' ').ToArray();
                    if (Conquer6609.Database.Server.ItemsBase.TryGetValue(uint.Parse(id[1]), out DBItem))
                    {
                        using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            byte bound = byte.Parse(this.AddBound.Text);
                            bool Newbound = false;
                            Conquer6609.Role.Flags.Gem soc1;
                            Conquer6609.Role.Flags.Gem soc2;
                            if (bound == 1) Newbound = true; else Newbound = false;
                            if (AddSoc1.Text == "1") soc1 = Conquer6609.Role.Flags.Gem.SuperDragonGem; else soc1 = Conquer6609.Role.Flags.Gem.NoSocket;
                            if (AddSoc2.Text == "1") soc2 = Conquer6609.Role.Flags.Gem.SuperDragonGem; else soc2 = Conquer6609.Role.Flags.Gem.NoSocket;
                            client.Inventory.Add(stream, uint.Parse(id[1]), byte.Parse(this.AddCount.Text), byte.Parse(this.AddPlus.Text), byte.Parse(this.AddBless.Text), byte.Parse(this.AddHP.Text), soc1, soc2, Newbound);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Plase Chose PlayerName");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }
        private void Load_Click(object sender, EventArgs e)
        {
            if (CBPlayerName.SelectedItem != null)
            {
                Conquer6609.Client.GameClient client = null;
                client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                if (client == null)
                    return;
                listBox2.Items.Clear();
                foreach (var itemx in client.Equipment.ClientItems.Values)
                {
                    if (itemx == null || !Server.ItemsBase.ContainsKey(itemx.ITEM_ID))
                        continue;
                    listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Equipment]", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID));
                }
                foreach (var itemx in client.Inventory.ClientItems.Values)
                {
                    if (itemx == null || !Server.ItemsBase.ContainsKey(itemx.ITEM_ID))
                        continue;
                    listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Inventory]", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID));
                }
                foreach (var ware in client.Warehouse.ClientItems.Values)
                {
                    foreach (var itemx in ware.Values)
                    {
                        if (itemx == null || !Server.ItemsBase.ContainsKey(itemx.ITEM_ID))
                            continue;
                        listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Warehouse] @@ {2}", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID, itemx.WH_ID));
                    }
                }
            }
        }
        private void RemoveItem()
        {
            var c = listBox2.SelectedItem.ToString();
            string[] data = c.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
            if (client == null)
                return;
            uint UID = Convert.ToUInt32(data[1]);
            MsgGameItem item = null;
            if (data[2] == "[Equipment]")
            {
                if (client.Equipment.ClientItems.ContainsKey(UID))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Equipment.ClientItems.TryRemove(UID, out item);
                        client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveEquipment, item.UID, item.Position, 0, 0, 0, 0));
                        client.Equipment.QueryEquipment(client.Equipment.Alternante);

                    }
                }
            }
            else if (data[2] == "[Inventory]")
            {
                client.Inventory.TryGetItem(UID, out item);
                if (item == null)
                    return;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Inventory.Remove(item.ITEM_ID, 1, stream);
                }

            }
            else if (data[2] == "[Warehouse]")
            {
                foreach (var WH in client.Warehouse.ClientItems.Values)
                {
                    if (WH.ContainsKey(UID))
                    {
                        WH.TryRemove(UID, out item);
                    }
                }
            }

            listBox2.Items.Clear();
            foreach (var itemx in client.Equipment.ClientItems.Values)
            {
                if (itemx == null || !Server.ItemsBase.ContainsKey(itemx.ITEM_ID))
                    continue;
                listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Equipment]", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID));
            }
            foreach (var itemx in client.Inventory.ClientItems.Values)
            {
                if (itemx == null || !Server.ItemsBase.ContainsKey(itemx.ITEM_ID))
                    continue;
                listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Inventory]", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID));
            }
            foreach (var ware in client.Warehouse.ClientItems.Values)
            {
                foreach (var itemx in ware.Values)
                {
                    if (item == null || !Server.ItemsBase.ContainsKey(item.ITEM_ID))
                        continue;
                    listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Warehouse] @@ {2}", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID, itemx.WH_ID));
                }
            }

        }

        private void RemoveSp_Click(object sender, EventArgs e)
        {
            RemoveItem();
        }
        private void AddStuff(byte Class)
        {
            byte Plus = byte.Parse(FullPlus.Text);
            byte Bless = byte.Parse(FullBless.Text);
            byte HP = byte.Parse(FullHP.Text);
            byte bound = byte.Parse(this.FullBond.Text);
            bool Newbound = false;
            Conquer6609.Role.Flags.Gem soc1;
            Conquer6609.Role.Flags.Gem soc2;
            if (bound == 1) Newbound = true; else Newbound = false;
            if (FullSoc1.Text == "1") soc1 = Conquer6609.Role.Flags.Gem.SuperDragonGem; else soc1 = Conquer6609.Role.Flags.Gem.NoSocket;
            if (FullSoc2.Text == "1") soc2 = Conquer6609.Role.Flags.Gem.SuperDragonGem; else soc2 = Conquer6609.Role.Flags.Gem.NoSocket;

            switch (Class)
            {
                case 15:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                Conquer6609.Client.GameClient client = null;
                                client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 410339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SkyBlade                                    
                                    client.Inventory.Add(stream, 420339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SquallSword                                    
                                    client.Inventory.Add(stream, 480339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //NirvanaClub                                    
                                    client.Inventory.Add(stream, 130109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //ObsidianArmor                                    
                                    client.Inventory.Add(stream, 118109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //PeerlessCoronet                                   
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                  client.Inventory.Add(stream, 201009, 1, Plus, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, Newbound); //Fan                                    
                                    client.Inventory.Add(stream, 202009, 1, Plus, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, Newbound); //Tower                                                   
                                    client.Inventory.Add(stream, 300000, 1, Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Steed                                    
                                    client.Inventory.Add(stream, 203009, 1, Plus, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Crop
                                }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 25:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                Conquer6609.Client.GameClient client = null;
                                client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 900109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //CelestialShield                                    
                                    client.Inventory.Add(stream, 561339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //OccultWand                                    
                                    client.Inventory.Add(stream, 560339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SpearOfWrath                                    
                                    client.Inventory.Add(stream, 131109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //ImperiousArmor                                    
                                    client.Inventory.Add(stream, 111109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SteelHelmet                                   
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    client.Inventory.Add(stream, 201009, 1, Plus, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, Newbound); //Fan                                    
                                    client.Inventory.Add(stream, 202009, 1, Plus, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, Newbound); //Tower                                                   
                                    client.Inventory.Add(stream, 300000, 1, Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Steed                                    
                                    client.Inventory.Add(stream, 203009, 1, Plus, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Crop
                                }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 45:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                Conquer6609.Client.GameClient client = null;
                                client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 500329, 1, Plus, Bless, HP, soc1, soc2, Newbound); //HeavenlyBow                                    
                                    client.Inventory.Add(stream, 133109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //WelkinCoat                                    
                                    client.Inventory.Add(stream, 113109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //WhiteTigerHat                                   
                                    client.Inventory.Add(stream, 201009, 1, Plus, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, Newbound); //Fan                                    
                                    client.Inventory.Add(stream, 202009, 1, Plus, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, Newbound); //Tower                                                   
                                    client.Inventory.Add(stream, 300000, 1, Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Steed                                    
                                    client.Inventory.Add(stream, 203009, 1, Plus, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Crop    
                                }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 55:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                Conquer6609.Client.GameClient client = null;
                                client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 601439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //HanzoKatana                                    
                                    client.Inventory.Add(stream, 601439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //HanzoKatana                                    
                                    client.Inventory.Add(stream, 511439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SilenceScythe                                    
                                    client.Inventory.Add(stream, 135309, 1, Plus, Bless, HP, soc1, soc2, Newbound); //NightmareVest                                    
                                    client.Inventory.Add(stream, 112309, 1, Plus, Bless, HP, soc1, soc2, false); //RambleVeil                                   
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                   client.Inventory.Add(stream, 201009, 1, Plus, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, Newbound); //Fan                                    
                                    client.Inventory.Add(stream, 202009, 1, Plus, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, Newbound); //Tower                                                   
                                    client.Inventory.Add(stream, 300000, 1, Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Steed                                    
                                    client.Inventory.Add(stream, 203009, 1, Plus, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Crop

                                }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 65:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                Conquer6609.Client.GameClient client = null;
                                client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 610439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //LazuritePrayerBeads                                    
                                    client.Inventory.Add(stream, 610439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //LazuritePrayerBeads                                    
                                    client.Inventory.Add(stream, 136309, 1, Plus, Bless, HP, soc1, soc2, Newbound); //WhiteLotusFrock                                    
                                    client.Inventory.Add(stream, 143309, 1, Plus, Bless, HP, soc1, soc2, Newbound); //XumiCap                                   
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                  client.Inventory.Add(stream, 201009, 1, Plus, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, Newbound); //Fan                                    
                                    client.Inventory.Add(stream, 202009, 1, Plus, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, Newbound); //Tower                                                   
                                    client.Inventory.Add(stream, 300000, 1, Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Steed                                    
                                    client.Inventory.Add(stream, 203009, 1, Plus, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Crop
                                }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 75:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                Conquer6609.Client.GameClient client = null;
                                client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 612439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //LordPistol                                    
                                    client.Inventory.Add(stream, 611439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //CaptainRapier                                    
                                    client.Inventory.Add(stream, 139309, 1, Plus, Bless, HP, soc1, soc2, Newbound); //DarkDragonCoat                                    
                                    client.Inventory.Add(stream, 144309, 1, Plus, Bless, HP, soc1, soc2, Newbound); //DominatorHat                                   
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                   client.Inventory.Add(stream, 201009, 1, Plus, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, Newbound); //Fan                                    
                                    client.Inventory.Add(stream, 202009, 1, Plus, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, Newbound); //Tower                                                   
                                    client.Inventory.Add(stream, 300000, 1, Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Steed                                    
                                    client.Inventory.Add(stream, 203009, 1, Plus, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Crop
                                }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 135:
                    {
                        try
                        {
                            if (FullSoc1.Text == "1") soc1 = Conquer6609.Role.Flags.Gem.SuperTortoiseGem; else soc1 = Conquer6609.Role.Flags.Gem.NoSocket;
                            if (FullSoc1.Text == "1") soc2 = Conquer6609.Role.Flags.Gem.SuperTortoiseGem; else soc2 = Conquer6609.Role.Flags.Gem.NoSocket;
                            if (CBPlayerName.SelectedItem != null)
                            {
                                Conquer6609.Client.GameClient client = null;
                                client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot  
                                    client.Inventory.Add(stream, 560339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SpearOfWrath                                    
                                    client.Inventory.Add(stream, 134109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //EternalRobe                                    
                                    client.Inventory.Add(stream, 114109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //DistinctCap                                  
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    client.Inventory.Add(stream, 201009, 1, Plus, Bless, HP, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, Newbound); //Fan                                    
                                    client.Inventory.Add(stream, 202009, 1, Plus, Bless, HP, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, Newbound); //Tower
                                }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 145:
                    {
                        try
                        {
                            if (FullSoc1.Text == "1") soc1 = Conquer6609.Role.Flags.Gem.SuperPhoenixGem; else soc1 = Conquer6609.Role.Flags.Gem.NoSocket;
                            if (FullSoc2.Text == "1") soc2 = Conquer6609.Role.Flags.Gem.SuperPhoenixGem; else soc2 = Conquer6609.Role.Flags.Gem.NoSocket;
                            if (CBPlayerName.SelectedItem != null)
                            {
                                Conquer6609.Client.GameClient client = null;
                                client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 121249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //NiftyBag                                    
                                    client.Inventory.Add(stream, 152259, 1, Plus, Bless, HP, soc1, soc2, Newbound); //WyvernBracelet                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 421339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SupremeSword                                    
                                    client.Inventory.Add(stream, 134109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //EternalRobe                                    
                                    client.Inventory.Add(stream, 114109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //DistinctCap                                   
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    client.Inventory.Add(stream, 201009, 1, Plus, 1, 0, Role.Flags.Gem.SuperThunderGem, Role.Flags.Gem.SuperThunderGem, Newbound); //Fan                                    
                                    client.Inventory.Add(stream, 202009, 1, Plus, 1, 0, Role.Flags.Gem.SuperGloryGem, Role.Flags.Gem.SuperGloryGem, Newbound); //Tower                                                   
                                    client.Inventory.Add(stream, 300000, 1, Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Steed                                    
                                    client.Inventory.Add(stream, 203009, 1, Plus, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Newbound); //Crop
                                }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }

            }
        }

        private void StuffNinja_Click(object sender, EventArgs e)
        {
            AddStuff(55);
        }

        private void StuffMonk_Click(object sender, EventArgs e)
        {
            AddStuff(65);
        }

        private void StuffParte_Click(object sender, EventArgs e)
        {
            AddStuff(75);
        }

        private void StuffArcher_Click(object sender, EventArgs e)
        {
            AddStuff(45);
        }

        private void StuffWarrior_Click(object sender, EventArgs e)
        {
            AddStuff(25);
        }

        private void StuffTrojan_Click(object sender, EventArgs e)
        {
            AddStuff(15);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            AddStuff(145);
        }

        private void StuffTaoist_Click(object sender, EventArgs e)
        {
            AddStuff(135);
        }

        private void SpishelItems(int indx)
        {
            switch (indx)
            {
                case 0:
                    {//cup
                        if (CBPlayerName.SelectedItem != null)
                        {
                            Conquer6609.Client.GameClient client = null;
                            client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;
                            if (!client.Inventory.HaveSpace(1))
                            {
                                System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                return;
                            }
                            using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Inventory.Add(stream, 2100095, 1);
                            }
                        }
                        break;
                    }
                case 1:
                    {//vip 1 days
                        if (CBPlayerName.SelectedItem != null)
                        {
                            Conquer6609.Client.GameClient client = null;
                            client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;
                          
                            using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (DateTime.Now > client.Player.ExpireVip || client.Player.VipLevel != 6)
                                    client.Player.ExpireVip = DateTime.Now.AddDays(1);
                                else
                                    client.Player.ExpireVip = client.Player.ExpireVip.AddDays(1);
                                client.Player.VipLevel = 6;
                                client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                                client.Player.UpdateVip(stream);
                                client.CreateBoxDialog("You`ve received VIP6 (1 day).");
                            }
                        }
                        break;
                    }
                case 2:
                    {//vip 30 days
                        if (CBPlayerName.SelectedItem != null)
                        {
                            Conquer6609.Client.GameClient client = null;
                            client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;

                            using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (DateTime.Now > client.Player.ExpireVip || client.Player.VipLevel != 6)
                                    client.Player.ExpireVip = DateTime.Now.AddDays(30);
                                else
                                    client.Player.ExpireVip = client.Player.ExpireVip.AddDays(30);
                                client.Player.VipLevel = 6;
                                client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                                client.Player.UpdateVip(stream);
                                client.CreateBoxDialog("You`ve received VIP6 (30 day).");
                            }
                        }
                        break;
                    }
                case 3:
                    {//Soul 6
                        if (CBPlayerName.SelectedItem != null)
                        {
                            Conquer6609.Client.GameClient client = null;
                            client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;
                            if (!client.Inventory.HaveSpace(25))
                            {
                                System.Windows.Forms.MessageBox.Show("Character must have 25 free slots into inventory");
                                return;
                            }
                            using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                var array = Database.ItemType.PurificationItems[6].Values.ToArray();
                                foreach (var item in array)
                                {
                                    client.Inventory.Add(stream, item.ID,1);
                                }
                                
                            }
                        }
                        break;
                    }
                case 4:
                    {//Soul 7
                        if (CBPlayerName.SelectedItem != null)
                        {
                            Conquer6609.Client.GameClient client = null;
                            client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;
                            if (!client.Inventory.HaveSpace(25))
                            {
                                System.Windows.Forms.MessageBox.Show("Character must have 25 free slots into inventory");
                                return;
                            }
                            using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                var array = Database.ItemType.PurificationItems[7].Values.ToArray();
                                foreach (var item in array)
                                {
                                    client.Inventory.Add(stream, item.ID, 1);
                                }

                            }
                        }
                        break;
                    }
                case 5:
                    {//Ref 6
                        if (CBPlayerName.SelectedItem != null)
                        {
                            Conquer6609.Client.GameClient client = null;
                            client = Conquer6609.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;
                            if (!client.Inventory.HaveSpace(15))
                            {
                                System.Windows.Forms.MessageBox.Show("Character must have 15 free slots into inventory");
                                return;
                            }
                            using (var rec = new Conquer6609.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                var array = Database.ItemType.Refinary[5].Values.ToArray();
                                foreach (var item in array)
                                {
                                    client.Inventory.Add(stream, item.ItemID, 1);
                                }

                            }
                        }
                        break;
                    }
               
            }
        }

        private void CBSpecial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CBSpecial.SelectedIndex >= 0)
            {
                SpishelItems(CBSpecial.SelectedIndex);
            }
        }
        #endregion

        #region Class Spell

        private void AddSpell_Click(object sender, EventArgs e)
        {
            try
            {
                ushort ID = ushort.Parse(AddID.Text);
                byte Level = byte.Parse(AddLevel.Text);
                int exp = int.Parse(AddExp.Text);
                if (SpellUserName.SelectedItem != null)
                {
                    Conquer6609.Client.GameClient client = null;
                    client = Conquer6609.Client.GameClient.CharacterFromName(SpellUserName.SelectedItem.ToString());
                    if (client == null)
                        return;
                    using (var rec = new ServerSockets.RecycledPacket())
                        client.MySpells.Add(rec.GetStream(), (ushort)ID, Level, 0, 0, exp);
                    SendMessagebox("Done Add This Spell " + ID + "");
                }
                else
                {
                    MessageBox.Show("Plase Select The Player Name");
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        private void SpellList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string[] Arry = SpellList.SelectedItem.ToString().Split('-');
                AddSpellName.Text = Arry[0];
                AddID.Text = Arry[1];
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        private void LoadeRemoveSpell()
        {
            try
            {
                if (SpellUserName.SelectedItem != null)
                {
                    Conquer6609.Client.GameClient client = null;
                    client = Conquer6609.Client.GameClient.CharacterFromName(SpellUserName.SelectedItem.ToString());
                    if (client == null)
                        return;
                    PlayerSpell.Items.Clear();
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        foreach (var spell in Database.Server.Magic.Values)
                        {
                            if (spell.Keys.Count > 0)
                            {
                                var sp = spell[(ushort)(spell.Keys.Count - 1)];

                                foreach (var spelle in client.MySpells.ClientSpells.Values)
                                {
                                    if (sp.ID == spelle.ID)
                                    {
                                        PlayerSpell.Items.Add(sp.Name + "-" + sp.ID);
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ) { }
        }
        private void SpellUserName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadeRemoveSpell();
        }
        private bool done = false;
        private void PlayerSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!done)
            {
                LoadeRemoveSpell();
                done = true;
            }
            if (PlayerSpell.SelectedItem != null)
            {
                string[] Arry = PlayerSpell.SelectedItem.ToString().Split('-');
                RemoveName.Text = Arry[0];
                RemoveID.Text = Arry[1];
            }
        }

        private void RemoveSpell_Click(object sender, EventArgs e)
        {
            try
            {
                ushort ID = ushort.Parse(RemoveID.Text);
                if (SpellUserName.SelectedItem != null)
                {
                    Conquer6609.Client.GameClient client = null;
                    client = Conquer6609.Client.GameClient.CharacterFromName(SpellUserName.SelectedItem.ToString());
                    if (client == null)
                        return;
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)ID))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.MySpells.Remove((ushort)ID, stream);
                            SendMessagebox("Done Remove This Spell " + ID + "");
                        }

                    }
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }

        }

        private void RefName_Click(object sender, EventArgs e)
        {
            //لود الاسماء
            loadePlayerName();
        }
        #endregion

        #region class Ban
        private void Plyer_Name_Click(object sender, EventArgs e)
        {
            loadePlayerName();
        }
        private void AddBanACC_Click(object sender, EventArgs e)
        {

            if (BanPlayerName.SelectedItem != null)
            {
                Conquer6609.Client.GameClient client = null;
                client = Conquer6609.Client.GameClient.CharacterFromName(BanPlayerName.SelectedItem.ToString());
                if (client == null)
                    return;
                Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, uint.Parse(BanAccHours.Text), BanAccRespwen.Text);
                client.Socket.Disconnect();
                SendMessagebox("Done Ban Accuont " + client.Player.Name + "");
                LoadBanList("Acc");
            }

        }
        private void LoadBanList(string Type)
        {
            switch (Type)
            {
                case "Acc":
                    {
                        listBanAcc.Items.Clear();
                        foreach (var ban in SystemBannedAccount.BannedPoll.Values)
                        {
                            listBanAcc.Items.Add(ban.Name);
                        }
                        break;
                    }
                case "Chat":
                    {
                        listBanChat.Items.Clear();
                        foreach (var User in Server.GamePoll.Values)
                        {
                            if (User.Player.IsBannedChat)
                            {
                                listBanChat.Items.Add(User.Player.Name);
                            }
                        }
                        break;
                    }
                case "IP":
                    {
                        listBanIP.Items.Clear();
                        foreach (var User in SystemBanned.BannedPoll.Values)
                        {
                            listBanIP.Items.Add(User.IP);
                        }
                        break;
                    }
            }
        }
        private void LoadListAccBan_Click(object sender, EventArgs e)
        {
            LoadBanList("Acc");
        }

        private void RemoveBanACC_Click(object sender, EventArgs e)
        {
            if (listBanAcc.SelectedItem != null)
            {
                uint UID = 0;
                foreach (var obj in SystemBannedAccount.BannedPoll.Values)
                {
                    if (obj.Name == listBanAcc.SelectedItem.ToString())
                    {
                        UID = obj.UID;
                        break;
                    }
                }
                if (UID != 0)
                {
                    SystemBannedAccount.RemoveBan(UID);
                    LoadBanList("Acc");
                }
            }
        }

        private void AddBanChat_Click(object sender, EventArgs e)
        {
            if (BanPlayerName.SelectedItem == null) return;
            string Name = BanPlayerName.SelectedItem.ToString();
            int Hours = int.Parse(BanChatHours.Text);
            ChatBanned.AddChatBan(Name, Hours);
            SendMessagebox("Done Ban Chat " + Name + "");
            LoadBanList("Chat");

        }

        private void RemoveChatACC_Click(object sender, EventArgs e)
        {
            if (BanPlayerName.SelectedItem == null) return;
            string Name = BanPlayerName.SelectedItem.ToString();
            int Hours = int.Parse(BanChatHours.Text);
            ChatBanned.RemoveChatBan(Name);
            SendMessagebox("Done UnBan Chat " + Name + "");
            LoadBanList("Chat");
        }

        private void LoadListBanChat_Click(object sender, EventArgs e)
        {
            LoadBanList("Chat");
        }

        private void AddBanIP_Click(object sender, EventArgs e)
        {

            if (BanPlayerName.SelectedItem != null)
            {
                Conquer6609.Client.GameClient client = null;
                client = Conquer6609.Client.GameClient.CharacterFromName(BanPlayerName.SelectedItem.ToString());
                if (client == null)
                    return;
                Database.SystemBanned.AddBan(TxtBanIp.Text, uint.Parse(BanIPHours.Text));
                client.Socket.Disconnect();
                SendMessagebox("Done Ban IP " + client.Player.Name + "");
                LoadBanList("IP");
            }
        }

        private void LoadListIp_Click(object sender, EventArgs e)
        {
            LoadBanList("IP");
        }

        private void RemoveBanIP_Click(object sender, EventArgs e)
        {
            if (listBanIP.SelectedItem != null)
            {
                Database.SystemBanned.RemoveBan(listBanIP.SelectedItem.ToString());
                SendMessagebox("Done UnBan IP " + listBanIP.SelectedItem.ToString() + "");
                LoadBanList("IP");
            }
        }
        private void BanPlayerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BanPlayerName.SelectedItem != null)
            {
                Conquer6609.Client.GameClient client = null;
                client = Conquer6609.Client.GameClient.CharacterFromName(BanPlayerName.SelectedItem.ToString());
                if (client == null)
                    return;
                BanUid.Text = client.Player.UID.ToString();
                TxtBanIp.Text = client.Socket.RemoteIp.ToString();
            }
        }
        #endregion

        #region Class ChatPanle
        public string SelectedClient = "";
        //Role.GameMap _map;
        //public Role.GameMap Mapp
        //{
        //    get
        //    {
        //        if (_map == null)
        //            _map = Server.ServerMaps[1002];
        //        return _map;
        //    }
        //}
        public void loadChatPanl()
        {
            Clients.Clear();
            SelectedClient = "";
            ClientList.ForeColor = Color.Black;
            RecList.ForeColor = Color.Black;
            this.Text = "ChatBox " + Program.ServerConfig.ServerName;
            //base.Closing += this.Form1_Closing;
        }
        private void SendMes()//الارسال
        {
            try
            {

                foreach (var user in Server.GamePoll.Values)
                {
                    if (user.Player.Name == SelectedClient)
                    {
                        user.SendWhisper(SendText.Text, "Conquer[GM]", SelectedClient);
                        string line = "Conquer[GM]--->" + SendText.Text;
                        AddChatLine(SelectedClient, line, false);
                        RecList.Items.Add(line);
                        SendText.Text = "";
                    }
                }
                RefreshData();
            }
            catch (Exception d)
            {
                Console.WriteLine(d.ToString());
            }
        }
        private void ButtnSend_Click(object sender, EventArgs e)
        {
            //زر الارسال
            SendMes();
        }

        private void ClientList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Clientt client;
                SelectedClient = ClientList.Text;
                if (SelectedClient.Contains("(UnSeen)"))
                {
                    SelectedClient = SelectedClient.Remove(SelectedClient.Length - 8, 8);
                    ClientList.Text = SelectedClient;
                    ClientList.Items[ClientList.SelectedIndex] = SelectedClient;
                }
                if (Clients.TryGetValue(SelectedClient, out client))
                {
                    client.Seen = true;
                    RecList.Items.Clear();
                    foreach (var line in client.Lines)
                        RecList.Items.Add(line);
                }
            }
            catch
            {

            }
        }

        private void MisReady_SelectedIndexChanged(object sender, EventArgs e)
        {
            //رسائل جاهزه
            try
            {
                foreach (var user in Server.GamePoll.Values)
                {
                    if (user.Player.Name == SelectedClient)
                    {
                        user.SendWhisper(MisReady.Text, "Conquer[GM]", SelectedClient);
                        string line = "Conquer[GM]--->" + MisReady.Text;
                        AddChatLine(SelectedClient, line, false);
                        RecList.Items.Add(line);
                        MisReady.SelectedItem = null;
                    }
                    if (ComName.SelectedItem != null && SelectedClient == "")
                    {
                        if (user.Player.Name == ComName.SelectedItem.ToString())
                        {
                            user.SendWhisper(MisReady.Text, "Conquer[GM]", ComName.SelectedItem.ToString());
                            string line = "Conquer[GM]--->" + MisReady.Text;
                            AddChatLine(ComName.SelectedItem.ToString(), line, false);
                            RecList.Items.Add(line);
                            MisReady.SelectedItem = null;
                            ComName.SelectedItem = null;
                            RefreshData();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void RemovePlaer_Click(object sender, EventArgs e)
        {
            try
            {
                string name = ClientList.Items[ClientList.SelectedIndex].ToString();
                if (name.EndsWith("(UnSeen)"))
                    name = name.Substring(0, name.Length - 8);
                Clientt removed;
                Clients.TryRemove(name, out removed);
                ClientList.Items.RemoveAt(ClientList.SelectedIndex);
            }
            catch
            {

            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void ComName_Click(object sender, EventArgs e)
        {
            loadePlayerName();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (NewMessage)
            {
                RefreshData();
                NewMessage = false;
            }
        }
        private void RefreshData()
        {
            try
            {
                ClientList.Items.Clear();
                foreach (var x in Clients)
                {
                    if (x.Value.Seen)
                    {
                        ClientList.Items.Add(x.Key);
                    }
                    else
                    {
                        if (SelectedClient != x.Key)
                        {
                            ClientList.Items.Add(x.Key + "(UnSeen)");
                        }
                        else
                        {
                            ClientList.Items.Add(x.Key);
                        }
                    }
                }
                RecList.Items.Clear();
                Clientt client;
                if (SelectedClient != "")
                {
                    if (Clients.TryGetValue(SelectedClient, out client))
                    {
                        client.Seen = true;
                        foreach (var line in client.Lines)
                            RecList.Items.Add(line);
                    }
                }
            }
            catch (Exception d)
            {
                MessageBox.Show(d.ToString());
            }
        }

        private void ButtnSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendMes();
            }
        }

        private void ON_Off_GM_Click(object sender, EventArgs e)
        {
            Conquer6609.Client.GameClient pclient = new Conquer6609.Client.GameClient(null);
            if (!Database.Server.GamePoll.ContainsKey(2000000000))
            {

                pclient.Fake = true;
                pclient.Player = new Role.Player(pclient);
                pclient.Inventory = new Role.Instance.Inventory(pclient);
                pclient.Equipment = new Role.Instance.Equip(pclient);
                pclient.Warehouse = new Role.Instance.Warehouse(pclient);
                pclient.MyProfs = new Role.Instance.Proficiency(pclient);
                pclient.MySpells = new Role.Instance.Spell(pclient);
                pclient.Achievement = new Database.AchievementCollection();
                pclient.Status = new Game.MsgServer.MsgStatus();
                pclient.Player.Name = "Conquer[GM]";
                pclient.Player.Body = 1003;
                pclient.Player.UID = 2000000000;
                pclient.Player.HitPoints = ushort.MaxValue;
                pclient.Status.MaxHitpoints = ushort.MaxValue;
                pclient.Player.X = (ushort)312;
                pclient.Player.Y = (ushort)286;
                pclient.Player.Map = 1002;
                pclient.Player.Level = 255;
                pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
                pclient.Player.Face = 153;
                pclient.Player.Action = Role.Flags.ConquerAction.Sit;
                pclient.Player.Angle = Role.Flags.ConquerAngle.SouthWest;
                pclient.Player.Hair = 774;
                pclient.Player.GarmentId = 193625;
                pclient.Player.LeftWeaponId = 610409;
                pclient.Player.RightWeaponId = 610409;
                pclient.Player.LeftWeaponAccessoryId = 360008;
                pclient.Player.RightWeaponAccessoryId = 360008;
                pclient.Map = Database.Server.ServerMaps[1002];
                pclient.Map.Enquer(pclient);
                Database.Server.GamePoll.TryAdd(pclient.Player.UID, pclient);
                using (var p = new ServerSockets.RecycledPacket())
                {
                    var stream = p.GetStream();
                    pclient.Player.View.SendView(pclient.Player.GetArray(stream, false), false);
                    Program.SendGlobalPackets.Enqueue(new MsgMessage("GM Has Login In TwinCity", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));

                }
                Co_Panle.On = true;

            }
            else
            {
                if (!Co_Panle.On)
                {
                    Co_Panle.On = true;
                    using (var p = new ServerSockets.RecycledPacket())
                    {
                        var stream = p.GetStream();
                        Program.SendGlobalPackets.Enqueue(new MsgMessage("GM Has Login In TwinCity", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                    }

                }
                else
                {
                    using (var p = new ServerSockets.RecycledPacket())
                    {
                        var stream = p.GetStream();
                        Program.SendGlobalPackets.Enqueue(new MsgMessage("GM Has Close", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                        Co_Panle.On = false;
                    }
                }

            }
        }

        private void SendText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendMes();
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            RecList.Items.Clear();
        }

        private void SendMessAll_Click(object sender, EventArgs e)
        {
            if (ChatType.SelectedItem != null)
            {
                switch (ChatType.SelectedIndex)
                {
                    case 0:
                        {
                            // one global packet reaches every online player (it used to be queued once per player)
                            if (!Server.GamePoll.IsEmpty)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage(ChatAllMess.Text, MsgMessage.MsgColor.white, MsgMessage.ChatMode.System).GetArray(stream));
                                }
                            }
                            break;
                        }
                    case 1:
                        {
                            // one [GM] whisper to each player, addressed to that player
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                foreach (var cleint in Server.GamePoll.Values)
                                {
                                    var msg = new MsgMessage(ChatAllMess.Text, 1371006, cleint.Player.Name, "[GM]", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Whisper);
                                    cleint.Send(msg.GetArray(stream));
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            foreach (var cleint in Server.GamePoll.Values)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    cleint.SendSysMesage(ChatAllMess.Text, MsgMessage.ChatMode.Center);

                                }

                            }
                            break;
                        }

                    case 3:
                        {
                            foreach (var cleint in Server.GamePoll.Values)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    cleint.SendSysMesage(ChatAllMess.Text, MsgMessage.ChatMode.BroadcastMessage);

                                }

                            }
                            break;
                        }
                    case 4:
                        {
                            foreach (var cleint in Server.GamePoll.Values)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    cleint.SendSysMesage(ChatAllMess.Text, MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);

                                }

                            }
                            break;
                        }
                }
            }
        }
        #endregion

        #region class Chi
          public object sync = new object();
          private string MyName = "";
          public void Chi()
          {
              ChiPwor.Items.Clear();
              ChiPwor.Items.Add("Dragon");
              ChiPwor.Items.Add("Phoenix");
              ChiPwor.Items.Add("Tiger");
              ChiPwor.Items.Add("Turtle");
          }
          private void ChiPlayerName_SelectedIndexChanged(object sender, EventArgs e)
          {
              lock (sync)
              {
                  MyName = ChiPlayerName.Text;
              }
          }
          public string[] AtributesType = 
            {
               "CriticalStrike",
               "SkillCriticalStrike",
               "Immunity",
               "Breakthrough",
               "Counteraction",
               "MaxLife",
               "AddAttack",
               "AddMagicAttack",
               "AddMagicDefense",
               "FinalAttack",
               "FinalMagicAttack",
               "FinalDefense",
               "FinalMagicDefense",
            };

          private void ChiPwor_SelectedIndexChanged(object sender, EventArgs e)
          {
              lock (sync)
              {
                  Star1.Enabled = true;
                  Star2.Enabled = true;
                  Star3.Enabled = true;
                  Star4.Enabled = true;

                  Star1.Items.Clear();
                  Star2.Items.Clear();
                  Star3.Items.Clear();
                  Star4.Items.Clear();

                  Star1.SelectedText = "";
                  Star2.SelectedText = "";
                  Star3.SelectedText = "";
                  Star4.SelectedText = "";
                  ComboBox btn = sender as ComboBox;
                  if (MyName == "")
                  {
                      System.Windows.Forms.MessageBox.Show("Select Character First");
                      return;
                  }

                  var c = Client.GameClient.CharacterFromName(MyName);
                  if (c != null)
                  {
                      var PowerType = (Game.MsgServer.MsgChiInfo.ChiPowerType)Enum.Parse(typeof(Game.MsgServer.MsgChiInfo.ChiPowerType), ChiPwor.Text);
                      var Power = c.Player.MyChi.Where(p => p.Type == PowerType).FirstOrDefault();
                      if (Power.UnLocked)
                      {
                          foreach (var att in AtributesType)
                          {
                              if (Power.Fields[0].Item1.ToString() != att && Power.Fields[1].Item1.ToString() != att && Power.Fields[2].Item1.ToString() != att && Power.Fields[3].Item1.ToString() != att)
                              {
                                  Star1.Items.Add(att);
                                  Star2.Items.Add(att);
                                  Star3.Items.Add(att);
                                  Star4.Items.Add(att);
                              }
                          }
                          textBox1.Text = Power.Fields[0].Item1.ToString();
                          textBox2.Text = Power.Fields[1].Item1.ToString();
                          textBox3.Text = Power.Fields[2].Item1.ToString();
                          textBox4.Text = Power.Fields[3].Item1.ToString();
                      }
                      else
                      {
                          System.Windows.Forms.MessageBox.Show("Unlock the power first");
                      }

                  }

              }
          }

          private void Star1_SelectedIndexChanged(object sender, EventArgs e)
          {
              ComboBox btn = sender as ComboBox;
              if (Dont)
                  return;
              var xx = btn.Name.Length - 1;
              var i2 = (byte.Parse(btn.Name.Substring(xx, 1)));
              if (MyName == "")
              {
                  System.Windows.Forms.MessageBox.Show("Select Character First");
                  return;
              }
              if (btn.Text == "" || btn.SelectedText == "None")
              {

                  if (btn.SelectedText == "None")
                  {
                      System.Windows.Forms.MessageBox.Show("Select Att First");
                  }
                  return;
              }
              var c = Client.GameClient.CharacterFromName(MyName);
              if (c != null)
              {
                  var PowerType = (Game.MsgServer.MsgChiInfo.ChiPowerType)Enum.Parse(typeof(Game.MsgServer.MsgChiInfo.ChiPowerType), ChiPwor.Text);
                  var Power = c.Player.MyChi.Where(p => p.Type == PowerType).FirstOrDefault();
                  if (Power.UnLocked)
                  {
                      var attribte = (Role.Instance.Chi.ChiAttributeType)Enum.Parse(typeof(Role.Instance.Chi.ChiAttributeType), btn.Text);
                      if (Valuee.SelectedItem != null)
                      {
                          if (Valuee.SelectedItem.ToString() == "Epic")
                          {
                              var Value = Role.Instance.Chi.MaxPower(attribte);
                              Power.Fields[i2 - 1] = Tuple.Create((Role.Instance.Chi.ChiAttributeType)attribte, (int)((byte)attribte * 10000 + Value));
                          }
                          else
                          {
                              var Value = Role.Instance.Chi.MedPower(attribte);
                              Power.Fields[i2 - 1] = Tuple.Create((Role.Instance.Chi.ChiAttributeType)attribte, (int)((byte)attribte * 10000 + Value));
                          }
                      }
                  }
                  else
                  {
                      System.Windows.Forms.MessageBox.Show("Unlock the power first");
                  }
                  Star1.Items.Clear();
                  Star2.Items.Clear();
                  Star3.Items.Clear();
                  Star4.Items.Clear();
                  foreach (var att in AtributesType)
                  {
                      if (Power.Fields[0].Item1.ToString() != att && Power.Fields[1].Item1.ToString() != att && Power.Fields[2].Item1.ToString() != att && Power.Fields[3].Item1.ToString() != att)
                      {
                          Star1.Items.Add(att);
                          Star2.Items.Add(att);
                          Star3.Items.Add(att);
                          Star4.Items.Add(att);
                      }
                  }
                  Dont = true;
                  textBox1.Text = Power.Fields[0].Item1.ToString();
                  textBox2.Text = Power.Fields[1].Item1.ToString();
                  textBox3.Text = Power.Fields[2].Item1.ToString();
                  textBox4.Text = Power.Fields[3].Item1.ToString();
                  Dont = false;
                  Role.Instance.Chi.ComputeStatus(c.Player.MyChi);
                  c.Equipment.QueryEquipment(c.Equipment.Alternante, false);
                  Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(c, Game.MsgServer.MsgChiInfo.Action.Send);
              }
          }
          public bool Dont { get; set; }
        #endregion

        private void UpdateTxt()
        {
            // WindWalker
            Txt_TripleBlasts.Text = TripleBlastsPlayers.ToString("0.00");
            Txt_Omnipotence.Text = OmnipotencePlayers.ToString("0.00");
            Txt_SwirlingStorm.Text = SwirlingStormPlayers.ToString("0.00");
            Txt_RageofWar.Text = RageofWarPlayers.ToString("0.00");
            Txt_BurntFrost.Text = BurntFrostPlayers.ToString("0.00");
            Txt_AngerofStomper.Text = AngerofStomperPlayers.ToString("0.00");
            Txt_Thundercloud.Text = ThundercloudPlayers.ToString("0.00");
            Txt_ThundercloudAttack.Text = ThundercloudAttackPlayers.ToString("0.00");

           //Dragon Warrior
            Txt_CrackingSwipe.Text = CrackingSwipePlayers.ToString("0.00");
            Txt_SplittingSwipe.Text = SplittingSwipePlayers.ToString("0.00");
            Txt_SpeedKick.Text = SpeedKickPlayers.ToString("0.00");
            Txt_ViolentKick.Text = ViolentKickPlayers.ToString("0.00");
            Txt_StormKick.Text = StormKickPlayers.ToString("0.00");
            Txt_DragonPunch.Text = DragonPunchPlayers.ToString("0.00");
            Txt_DragonCyclone.Text = DragonCyclonePlayers.ToString("0.00");
            Txt_DragonFury.Text = DragonFuryPlayers.ToString("0.00");
            Txt_AirKick.Text = AirKickPlayers.ToString("0.00");
            Txt_ViolentKick.Text = ViolentKickPlayers.ToString("0.00");
            Txt_AirSweep.Text = AirSweepPlayers.ToString("0.00");
            Txt_DragonSlash.Text = DragonSlashPlayers.ToString("0.00");

            //Parite
            textBladeTempest.Text = BladeTempestPlayers.ToString("0.00");
            textGaleBomb.Text = GaleBombPlayers.ToString("0.00");
            textWindstorm.Text = WindstormPlayers.ToString("0.00");
            textScurvyBomb.Text = ScurvyBombPlayers.ToString("0.00");
            textCannonBarrage.Text = CannonBarragePlayers.ToString("0.00");
            textKrakensRevenge.Text = KrakensRevengePlayers.ToString("0.00");
            //Water
            textVulcano.Text = VulcanoPlayers.ToString("0.00");
            textLightning.Text = LightningPlayers.ToString("0.00");
            textSpeedLightning.Text = SpeedLightningPlayers.ToString("0.00");
            textPrevade.Text = PrevadePlayers.ToString("0.00");
            Txt_SearingTouch.Text = SearingTouchPlayers.ToString("0.00");
            //Trojan
            HercDameg.Text = HerculesPlayers.ToString("0.00");
            FastDamge.Text = FastBladePlayers.ToString("0.00");
            SSdamge.Text = ScentSwordPlayers.ToString("0.00");
            RageDamge.Text = RagePlayers.ToString("0.00");
            PhoenixDmg.Text = PhoenixPlayers.ToString("0.00");
            textDragonWhirl.Text = DragonWhirlPlayers.ToString("0.00");
            Txt_FatalCross.Text = FatalCrossPlayers.ToString("0.00");
            Txt_MortalStrike.Text = MortalStrikePlayers.ToString("0.00");
            //Fire
            textThunder.Text = ThunderPlayers.ToString("0.00");
            textFire.Text = FirePlayers.ToString("0.00");
            textTornado.Text = TornadoPlayers.ToString("0.00");
            textBomb.Text = BombPlayers.ToString("0.00");
            textChainBolt.Text = ChainBoltPlayers.ToString("0.00");
            textFireRing.Text = FireRingPlayers.ToString("0.00");
            textFireMeteor.Text = FireMeteorPlayers.ToString("0.00");
            textFireCircle.Text = FireCirclePlayers.ToString("0.00");
            textFireofHell.Text = FireofHellPlayers.ToString("0.00");
            textHeavenBlade.Text = HeavenBladePlayers.ToString("0.00");
            //ninja
            textTwofoldBlade.Text = TwofoldBladePlayers.ToString("0.00");
            textBloodyScythe.Text = BloodyScythePlayers.ToString("0.00");
            textCounterKill.Text = CounterKillPlayers.ToString("0.00");
            textShurikenVortex.Text = ShurikenVPlayers.ToString("0.00");
            textGapingWounds.Text = GapingWoundsPlayers.ToString("0.00");
            Txt_TwilightDance.Text = TwilightDancePlayers.ToString("0.00");
            Txt_SuperTwBlade.Text = SuperTwBladePlayers.ToString("0.00");
            Txt_FatalSpin.Text = FatalSpinPlayers.ToString("0.00");
        
            //Monk
            textPalm.Text = RadiantPalmPlayers.ToString("0.00");
            textWhirlwindKick.Text = WhirlwindKickPlayers.ToString("0.00");
            textTripleAttack.Text = TripleAttackPlayers.ToString("0.00");
            textOblivion.Text = OblivionPlayers.ToString("0.00");
            Txt_WrathoftheEmperor.Text = WrathoftheEmperorPlayers.ToString("0.00");
            Txt_BladeFlurry.Text = BladeFlurryPlayers.ToString("0.00");
            Txt_Strike.Text = StrikePlayers.ToString("0.00");
            //Archer
            textScatter.Text = ScatterPlayers.ToString("0.00");
            textRapidFire.Text = RapidFirePlayers.ToString("0.00");
            Txt_DaggerStorm.Text = DaggerStormPlayers.ToString("0.00");
            Txt_BladeFlurry.Text = BladeFlurryPlayers.ToString("0.00");
            Txt_BlisteringWave.Text = BlisteringWavePlayers.ToString("0.00");
            Txt_MortalWound.Text = MortalWoundPlayers.ToString("0.00");
            //Warrior
            textSpeedGun.Text = SpeedGunPlayers.ToString("0.00");
            textSnow.Text = SnowPlayers.ToString("0.00");
            textBoom.Text = BoomPlayers.ToString("0.00");
            textSeizer.Text = SeizerPlayers.ToString("0.00");
            textViperFang.Text = ViperFangPlayers.ToString("0.00");
            textPenetration.Text = PenetrationPlayers.ToString("0.00");
            textDragonTail.Text = DragonTailPlayers.ToString("0.00");
            textCelestial.Text = CelestialPlayers.ToString("0.00");
            textChargingVortex.Text = ChargingVortexPlayers.ToString("0.00");
            Txt_ManiacDance.Text = ManiacDancePlayers.ToString("0.00");
            Txt_ScarofEarth.Text = ScarofEarthPlayers.ToString("0.00");
            Txt_WaveofBlood.Text = WaveofBloodPlayers.ToString("0.00");
            Txt_Strike.Text = StrikePlayers.ToString("0.00");

        }
        #region Save
        private void SaveTrojan_Click(object sender, EventArgs e)
        {
            FatalCrossPlayers = double.Parse(Txt_FatalCross.Text);
            MortalStrikePlayers = double.Parse(Txt_MortalStrike.Text); 
            HerculesPlayers = double.Parse(HercDameg.Text);
            FastBladePlayers = double.Parse(FastDamge.Text);
            ScentSwordPlayers = double.Parse(SSdamge.Text);
            RagePlayers = double.Parse(RageDamge.Text);
            PhoenixPlayers = double.Parse(PhoenixDmg.Text);
            DragonWhirlPlayers = double.Parse(textDragonWhirl.Text);
            //////////////////////
            Update(FatalCrossSkilUID, FatalCrossPlayers);
            Update(MortalStrikeSkilUID, MortalStrikePlayers);
            Update(FastBladeSkilUID, FastBladePlayers);
            Update(ScentSwordSkilUID, ScentSwordPlayers);
            Update(RageSkilUID, RagePlayers);
            Update(PhoenixSkilUID, PhoenixPlayers);
            Update(DragonWhirlSkilUID, DragonWhirlPlayers);
            Update(HerculesSkilUID, HerculesPlayers);
        }
        private void SaveFire_Click(object sender, EventArgs e)
        {
            ThunderPlayers = double.Parse(textThunder.Text);
            FirePlayers = double.Parse(textFire.Text);
            TornadoPlayers = double.Parse(textTornado.Text);
            BombPlayers= double.Parse(textBomb.Text);
            ChainBoltPlayers =double.Parse(textChainBolt.Text);
            FireRingPlayers =double.Parse(textFireRing.Text);
            FireMeteorPlayers =  double.Parse(textFireMeteor.Text);
            FireCirclePlayers =double.Parse(textFireCircle.Text) ;
            FireofHellPlayers = double.Parse(textFireofHell.Text);
            HeavenBladePlayers = double.Parse(textHeavenBlade.Text);
            //
            Update(ThunderSkilUID, ThunderPlayers);
            Update(FireSkilUID, FirePlayers);
            Update(TornadoSkilUID, TornadoPlayers);
            Update(BombSkilUID, BombPlayers);
            Update(ChainBoltSkilUID, ChainBoltPlayers);
            Update(FireRingSkilUID, FireRingPlayers);
            Update(FireMeteorSkilUID, FireMeteorPlayers);
            Update(FireCircleSkilUID, FireCirclePlayers);
            Update(FireofHellSkilUID, FireofHellPlayers);
            Update(HeavenBladeSkilUID, HeavenBladePlayers);

        }

        private void SaveArcher_Click(object sender, EventArgs e)
        {
            BlisteringWavePlayers = double.Parse(Txt_BlisteringWave.Text);
            DaggerStormPlayers = double.Parse(Txt_DaggerStorm.Text);
            BladeFlurryPlayers = double.Parse(Txt_BladeFlurry.Text);
            MortalWoundPlayers = double.Parse(Txt_MortalWound.Text); 
            ScatterPlayers = double.Parse(textScatter.Text);
            RapidFirePlayers =double.Parse(textRapidFire.Text);
            //
            Update(BlisteringWaveSkilUID, BlisteringWavePlayers);
            Update(DaggerStormSkilUID, DaggerStormPlayers);
            Update(BladeFlurrySkilUID, BladeFlurryPlayers);
            Update(MortalWoundSkilUID, MortalWoundPlayers);
            Update(ScatterSkilUID, ScatterPlayers);
            Update(RapidFireSkilUID, RapidFirePlayers);

        }

        private void SaveWater_Click(object sender, EventArgs e)
        {
            SearingTouchPlayers = double.Parse(Txt_SearingTouch.Text);
            VulcanoPlayers = double.Parse(textVulcano.Text);
            LightningPlayers = double.Parse(textLightning.Text);
            SpeedLightningPlayers = double.Parse(textSpeedLightning.Text);
            PrevadePlayers = double.Parse(textPrevade.Text);
            //
            Update(SearingTouchSkilUID, SearingTouchPlayers);
            Update(VulcanoSkilUID, VulcanoPlayers);
            Update(LightningSkilUID, LightningPlayers);
            Update(SpeedLightningSkilUID, SpeedLightningPlayers);
            Update(PrevadeSkilUID, PrevadePlayers);
        }

        private void SaveWarrior_Click(object sender, EventArgs e)
        {

            ScarofEarthPlayers = double.Parse(Txt_ScarofEarth.Text);
            WaveofBloodPlayers = double.Parse(Txt_WaveofBlood.Text);
            ManiacDancePlayers = double.Parse(Txt_ManiacDance.Text);
            SpeedGunPlayers = double.Parse(textSpeedGun.Text);
            SnowPlayers = double.Parse(textSnow.Text);
            BoomPlayers =  double.Parse(textBoom.Text);
            SeizerPlayers = double.Parse(textSeizer.Text);
            ViperFangPlayers = double.Parse(textViperFang.Text);
            PenetrationPlayers= double.Parse(textPenetration.Text);
            DragonTailPlayers = double.Parse(textDragonTail.Text);
            CelestialPlayers = double.Parse(textCelestial.Text);
            ChargingVortexPlayers =  double.Parse(textChargingVortex.Text);


            Update(ScarofEarthSkilUID, ScarofEarthPlayers);
            Update(WaveofBloodSkilUID, WaveofBloodPlayers);
            Update(ManiacDanceSkilUID, ManiacDancePlayers);
            Update(SpeedGunSkilUID, SpeedGunPlayers);
            Update(SnowSkilUID, SnowPlayers);
            Update(BoomSkilUID, BoomPlayers);
            Update(SeizerSkilUID, SeizerPlayers);
            Update(ViperFangSkilUID, ViperFangPlayers);
            Update(PenetrationSkilUID, PenetrationPlayers);
            Update(DragonTailSkilUID, DragonTailPlayers);
            Update(CelestialSkilUID, CelestialPlayers);
            Update(ChargingVortexUID, ChargingVortexPlayers);
        }

        private void SaveNinja_Click(object sender, EventArgs e)
        {
            TwofoldBladePlayers = double.Parse(textTwofoldBlade.Text);
            BloodyScythePlayers = double.Parse(textBloodyScythe.Text);
            CounterKillPlayers = double.Parse(textCounterKill.Text);
            ShurikenVPlayers = double.Parse(textShurikenVortex.Text);
            GapingWoundsPlayers = double.Parse(textGapingWounds.Text);
            SuperTwBladePlayers = double.Parse(Txt_SuperTwBlade.Text);
            FatalSpinPlayers = double.Parse(Txt_FatalSpin.Text);
            TwilightDancePlayers = double.Parse(Txt_TwilightDance.Text);

            Update(SuperTwBladeUID, SuperTwBladePlayers);
            Update(FatalSpinUID, FatalSpinPlayers);
            Update(TwilightDanceUID, TwilightDancePlayers);

            Update(TwofoldBladeSkilUID, TwofoldBladePlayers);
            Update(CounterKillSkilUID, CounterKillPlayers);
            Update(BloodyScytheSkilUID, BloodyScythePlayers);
            Update(ShurikenVSkilUID, ShurikenVPlayers);
            Update(GapingWoundsSkilUID, GapingWoundsPlayers);

        }
        private void SaveMonk_Click(object sender, EventArgs e)
        {
            RadiantPalmPlayers = double.Parse(textPalm.Text);
            WhirlwindKickPlayers = double.Parse(textWhirlwindKick.Text);
            TripleAttackPlayers = double.Parse(textTripleAttack.Text);
            OblivionPlayers = double.Parse(textOblivion.Text);
            WrathoftheEmperorPlayers = double.Parse(Txt_WrathoftheEmperor.Text);
            InfernalEchoPlayers = double.Parse(Txt_InfernalEcho.Text);
            StrikePlayers = double.Parse(Txt_Strike.Text);


            Update(WrathoftheEmperorSkilUID, WrathoftheEmperorPlayers);
            Update(InfernalEchoSkillUID, InfernalEchoPlayers);
            Update(StrikeSkilUID, StrikePlayers);
            Update(RadiantPalmSkilUID, RadiantPalmPlayers);
            Update(WhirlwindKickSkilUID,WhirlwindKickPlayers );
            Update(TripleAttackSkilUID,TripleAttackPlayers );
            Update(OblivionSkilUID,OblivionPlayers );

        }

        private void SavePatite_Click(object sender, EventArgs e)
        {
            BladeTempestPlayers = double.Parse(textBladeTempest.Text);
            GaleBombPlayers = double.Parse(textGaleBomb.Text);
            WindstormPlayers = double.Parse(textWindstorm.Text);
            ScurvyBombPlayers = double.Parse(textScurvyBomb.Text);
            CannonBarragePlayers = double.Parse(textCannonBarrage.Text);
            KrakensRevengePlayers = double.Parse(textKrakensRevenge.Text);

            Update(BladeTempestSkilUID,BladeTempestPlayers );
            Update(GaleBombSkilUID,GaleBombPlayers );
            Update(WindstormSkilUID,WindstormPlayers );
            Update(ScurvyBombSkilUID,ScurvyBombPlayers);
            Update(CannonBarrageSkilUID,CannonBarragePlayers );
            Update(KrakensRevengeSkilUID,KrakensRevengePlayers);

        }
        private void Btn_Save_DragonW_Click(object sender, EventArgs e)
        {

            CrackingSwipePlayers = double.Parse(Txt_CrackingSwipe.Text);
            SplittingSwipePlayers = double.Parse(Txt_SplittingSwipe.Text);
            SpeedKickPlayers = double.Parse(Txt_SpeedKick.Text);
            ViolentKickPlayers = double.Parse(Txt_ViolentKick.Text);
            StormKickPlayers = double.Parse(Txt_StormKick.Text);
            DragonPunchPlayers = double.Parse(Txt_DragonPunch.Text);
            DragonCyclonePlayers = double.Parse(Txt_DragonCyclone.Text);
            DragonSlashPlayers = double.Parse(Txt_DragonSlash.Text);
            AirKickPlayers = double.Parse(Txt_AirKick.Text);
            AirSweepPlayers = double.Parse(Txt_AirSweep.Text);
            DragonFuryPlayers = double.Parse(Txt_DragonFury.Text);

            ////////////////////////////////////UPDATE////////////

            Update(CrackingSwipeSkilUID, CrackingSwipePlayers);
            Update(SplittingSwipeSkilUID, SplittingSwipePlayers);
            Update(SpeedKickSkilUID, SpeedKickPlayers);
            Update(ViolentKickSkilUID, ViolentKickPlayers);
            Update(StormKickSkilUID, StormKickPlayers);
            Update(DragonPunchSkilUID, DragonPunchPlayers);
            Update(DragonCycloneSkilUID, DragonCyclonePlayers);
            Update(DragonFurySkilUID, DragonFuryPlayers);
            Update(AirKickSkilUID, AirKickPlayers);
            Update(AirSweepSkilUID, AirSweepPlayers);
            Update(DragonSlashSkilUID, DragonSlashPlayers);
        }

        private void Btn_Save_Wind_Click(object sender, EventArgs e)
        {


            TripleBlastsPlayers = double.Parse(Txt_TripleBlasts.Text);
            OmnipotencePlayers = double.Parse(Txt_Omnipotence.Text);
            SwirlingStormPlayers = double.Parse(Txt_SwirlingStorm.Text);
            RageofWarPlayers = double.Parse(Txt_RageofWar.Text);
            BurntFrostPlayers = double.Parse(Txt_BurntFrost.Text);
            AngerofStomperPlayers = double.Parse(Txt_AngerofStomper.Text);
            ThundercloudPlayers = double.Parse(Txt_Thundercloud.Text);
            ThundercloudAttackPlayers = double.Parse(Txt_ThundercloudAttack.Text);


            ////////////////////////////////////UPDATE////////////
            Update(TripleBlastsSkilUID, TripleBlastsPlayers);
            Update(OmnipotenceSkilUID, OmnipotencePlayers);
            Update(SwirlingStormSkilUID, SwirlingStormPlayers);
            Update(RageofWarSkilUID, RageofWarPlayers);
            Update(BurntFrostSkilUID, BurntFrostPlayers);
            Update(AngerofStomperSkilUID, AngerofStomperPlayers);
            Update(ThundercloudSkilUID, ThundercloudPlayers);
            Update(ThundercloudAttackSkilUID, ThundercloudAttackPlayers);




        }
        #endregion

        private void SaveChar(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }


            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

       

      

        #region Jaing
        private byte MyStage = 0;
        private byte MyLevel = 1;
        //public object sync = new object();
        //private string MyName = "";

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (sync)
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
                button8.Enabled = true;
                button9.Enabled = true;
                button10.Enabled = true;
                MyName = comboBox5.Text;
            }
        }
       

        private void button_Click(object sender, EventArgs e)
        {
            lock (sync)
            {

                Button btn = sender as Button;

                var temp = (byte.Parse(btn.Text.Substring(btn.Text.Length - 1, 1)));
                var c = Client.GameClient.CharacterFromName(MyName);
                if (c != null)
                {
                    if (!c.Player.MyJiangHu.ArrayStages[temp - 1].Activate)
                    {
                        System.Windows.Forms.MessageBox.Show("Complete the stage Stage " + (temp - 1) + " first then you can open this");
                        return;
                    }
                }
                MyStage = (byte.Parse(btn.Text.Substring(btn.Text.Length - 1, 1)));

                label2.Text = btn.Text + " is Selected";
                Star1.Enabled = true;
                Star2.Enabled = true;
                Star3.Enabled = true;
                Star4.Enabled = true;
                Star5.Enabled = true;
                Star6.Enabled = true;
                Star7.Enabled = true;
                Star8.Enabled = true;
                Star9.Enabled = true;
                Star1.Items.Clear();
                Star2.Items.Clear();
                Star3.Items.Clear();
                Star4.Items.Clear();
                Star5.Items.Clear();
                Star6.Items.Clear();
                Star7.Items.Clear();
                Star8.Items.Clear();
                Star9.Items.Clear();
                Star1.Text = "";
                Star2.Text = "";
                Star3.Text = "";
                Star4.Text = "";
                Star5.Text = "";
                Star6.Text = "";
                Star7.Text = "";
                Star8.Text = "";
                Star9.Text = "";
                Star1.Items.AddRange(AtributesType.ToArray());
                Star2.Items.AddRange(AtributesType.ToArray());
                Star3.Items.AddRange(AtributesType.ToArray());
                Star4.Items.AddRange(AtributesType.ToArray());
                Star5.Items.AddRange(AtributesType.ToArray());
                Star6.Items.AddRange(AtributesType.ToArray());
                Star7.Items.AddRange(AtributesType.ToArray());
                Star8.Items.AddRange(AtributesType.ToArray());
                Star9.Items.AddRange(AtributesType.ToArray());
                comboBox10.Items.Clear();
                comboBox10.Items.Add("1");
                comboBox10.Items.Add("2");
                comboBox10.Items.Add("3");
                comboBox10.Items.Add("4");
                comboBox10.Items.Add("5");
                comboBox10.Items.Add("6");
                comboBox10.Text = "6";
                if (c != null)
                {
                    ComboBox Star = sender as ComboBox;
                    var myjiung = c.Player.MyJiangHu;
                    if (myjiung.ArrayStages[MyStage - 1] != null)
                    {
                        this.Star1.SelectedIndexChanged -= new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star2.SelectedIndexChanged -= new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star3.SelectedIndexChanged -= new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star4.SelectedIndexChanged -= new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star5.SelectedIndexChanged -= new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star6.SelectedIndexChanged -= new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star7.SelectedIndexChanged -= new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star8.SelectedIndexChanged -= new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star9.SelectedIndexChanged -= new System.EventHandler(this.Star_SelectedIndexChanged);
                        Star1.Text = myjiung.ArrayStages[MyStage - 1].ArrayStars[0].Typ.ToString();
                        Star2.Text = myjiung.ArrayStages[MyStage - 1].ArrayStars[1].Typ.ToString();
                        Star3.Text = myjiung.ArrayStages[MyStage - 1].ArrayStars[2].Typ.ToString();
                        Star4.Text = myjiung.ArrayStages[MyStage - 1].ArrayStars[3].Typ.ToString();
                        Star5.Text = myjiung.ArrayStages[MyStage - 1].ArrayStars[4].Typ.ToString();
                        Star6.Text = myjiung.ArrayStages[MyStage - 1].ArrayStars[5].Typ.ToString();
                        Star7.Text = myjiung.ArrayStages[MyStage - 1].ArrayStars[6].Typ.ToString();
                        Star8.Text = myjiung.ArrayStages[MyStage - 1].ArrayStars[7].Typ.ToString();
                        Star9.Text = myjiung.ArrayStages[MyStage - 1].ArrayStars[8].Typ.ToString();
                        this.Star1.SelectedIndexChanged += new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star2.SelectedIndexChanged += new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star3.SelectedIndexChanged += new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star4.SelectedIndexChanged += new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star5.SelectedIndexChanged += new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star6.SelectedIndexChanged += new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star7.SelectedIndexChanged += new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star8.SelectedIndexChanged += new System.EventHandler(this.Star_SelectedIndexChanged);
                        this.Star9.SelectedIndexChanged += new System.EventHandler(this.Star_SelectedIndexChanged);
                    }

                }
            }
        }

        private void Star_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (MyName == "")
            {
                System.Windows.Forms.MessageBox.Show("Select Character First");
                return;
            }
            if (MyStage == 0)
            {
                System.Windows.Forms.MessageBox.Show("Select Stage First");
                return;
            }
            var c = Client.GameClient.CharacterFromName(MyName);
            if (c != null)
            {
                ComboBox Star = sender as ComboBox;
                c.Player.MyJiangHu.ArrayStages[MyStage - 1].Activate = true;
                c.Player.MyJiangHu.ArrayStages[MyStage - 1].ArrayStars[int.Parse(Star.Name.Substring(Star.Name.Length - 1, 1)) - 1].Activate = true;
                c.Player.MyJiangHu.ArrayStages[MyStage - 1].ArrayStars[int.Parse(Star.Name.Substring(Star.Name.Length - 1, 1)) - 1].Level = MyLevel;
                c.Player.MyJiangHu.ArrayStages[MyStage - 1].ArrayStars[int.Parse(Star.Name.Substring(Star.Name.Length - 1, 1)) - 1].Typ = (Role.Instance.JiangHu.Stage.AtributesType)Enum.Parse(typeof(Role.Instance.JiangHu.Stage.AtributesType), Star.Text);
                c.Player.MyJiangHu.ArrayStages[MyStage - 1].ArrayStars[int.Parse(Star.Name.Substring(Star.Name.Length - 1, 1)) - 1].UID = ValueToRoll(c.Player.MyJiangHu.ArrayStages[MyStage - 1].ArrayStars[int.Parse(Star.Name.Substring(Star.Name.Length - 1, 1)) - 1].Typ, MyLevel);
                if (MyStage < 9)
                {
                    if (!c.Player.MyJiangHu.ArrayStages[MyStage].Activate)
                    {
                        int count = 0;
                        foreach (var x in c.Player.MyJiangHu.ArrayStages[MyStage - 1].ArrayStars)
                        {
                            if (x.Activate)
                            {
                                count += 1;
                            }
                        }
                        if (count == 9)
                        {
                            c.Player.MyJiangHu.ArrayStages[MyStage].Activate = true;
                        }
                    }
                }
                c.Equipment.QueryEquipment(c.Equipment.Alternante, false);

                using (var x = new ServerSockets.RecycledPacket())
                {
                    var stream = x.GetStream();
                    c.Player.MyJiangHu.LoginClient(stream, c);
                }
            }

        }
        public ushort ValueToRoll(Role.Instance.JiangHu.Stage.AtributesType status, byte level)
        {
            return (ushort)(((ushort)status) + (level * 0x100));
        }
        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (sync)
            {

                MyLevel = byte.Parse(comboBox10.Text);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            lock (sync)
            {

                Star2.Text = Star1.Text;
                Star3.Text = Star1.Text;
                Star4.Text = Star1.Text;
                Star5.Text = Star1.Text;
                Star6.Text = Star1.Text;
                Star7.Text = Star1.Text;
                Star8.Text = Star1.Text;
                Star9.Text = Star1.Text;
            }
        }

        #endregion

        private void UserName_EditValueChanged(object sender, EventArgs e)
        {

        }

      
    }
}
