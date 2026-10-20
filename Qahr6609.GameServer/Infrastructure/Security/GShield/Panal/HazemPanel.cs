using Conquer6609.Database;
using Conquer6609.Game;
using Conquer6609.Game.MsgLoader;
using Conquer6609.Game.MsgServer;
using System;
using System.Linq;
using System.Windows.Forms;
using GShield;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Conquer6609.AdminTool
{
    public partial class HazemPanel : Form
    {
        public HazemPanel()
        {
            InitializeComponent();
        }

        private void FrmOnlinePlayers_Load(object sender, EventArgs e)
        {
            LoadOnlinePlayers();
            LoadingbannedPC();
            foreach (var Unlock in Database.UnLockLogin.UnLockLoginPoll.Values)
            {
                comboBox2.Items.Add(Unlock.PlayerName);
            }
            foreach (var Clicker in Database.Allow_Clicker.AllowClickerPoll.Values)
            {
                comboBox4.Items.Add(Clicker.PlayerName);
            }

        }
        public SystemBannedAccount.Client GetPlayerBanned(string Name)
        {
            var player = SystemBannedAccount.BannedPoll.Values.Where(s => s.Name == Name).FirstOrDefault();
            if (player == null)
                return null;
            return player;
        }

        public Conquer6609.Client.GameClient GetPlayer(string Name)
        {
            Client.GameClient player = Server.GamePoll.Values.Where(s => s.Player.Name == Name).FirstOrDefault();
            if (player == null)
                return null;
            return player;
        }

        public void LoadOnlinePlayers()
        {
            comboBox1.Items.Clear();
            comboBox1.Text = "";

            foreach (var C in Server.GamePoll.Values)
            {
                comboBox1.Items.Add(C.Player.Name);
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a player first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string playerName = comboBox1.SelectedItem.ToString();
            var target = GetPlayerByName(playerName);

            if (target != null)
            {
                try
                {
                    HazemShield.SendPacket(target, Types.SubType.ProcessesCheck);
                    target.ForceProcessLog = true;

                    MessageBox.Show($"Scan request sent to: {playerName}", "Scan Started", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while sending scan request: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("The selected player is no longer online!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Client.GameClient GetPlayerByName(string name)
        {
            foreach (var C in Server.GamePoll.Values)
            {
                if (C.Player.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return C;
            }
            return null;
        }

        private void btnShowPCInfo_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a player first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string playerName = comboBox1.SelectedItem.ToString();
            var target = GetPlayerByName(playerName);

            if (target == null)
            {
                MessageBox.Show("The selected player is no longer online!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Console.WriteLine("====================================");
                Console.WriteLine($"--- PC Info for {playerName} ---");
                Console.WriteLine($"HWID       : {target.HWID}");
                Console.WriteLine($"TqSerial   : {target.TqSerial}");
                Console.WriteLine($"Key        : {target.Key}");
                Console.WriteLine("====================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to display PC info: {ex.Message}");
            }
        }

        private void btnBanBc_Click_1(object sender, EventArgs e)
        {
            var player = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (player == null)
                return;
            if (Database.SystemBannedPC.AddBan(player))
            {
                comboBox1.Text = "";
                LoadOnlinePlayers();
                player.Socket.Disconnect();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        public void Loadingbanned()
        {
            //banned list
            comboBox3.Items.Clear();
            comboBox3.Text = "";
            textBox4.Text = "";//UID
            textBox5.Text = "";//PlayerName
            textBox7.Text = "";//reson
            foreach (var p in SystemBannedAccount.BannedPoll.Values)
            {
                comboBox3.Items.Add(p.Name);
            }
        }

        public void LoadingbannedPC()
        {
            //banned list
            comboBox4.Items.Clear();
            comboBox4.Text = "";
            foreach (var p in SystemBannedPC.BannedPoll.Values)
            {
                comboBox4.Items.Add(p.PlayerName);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            LoadingbannedPC();

        }

        private void button12_Click(object sender, EventArgs e)
        {
            string Name = comboBox4.Text;
            if (Name.Length <= 0)
                return;
            if (Database.SystemBannedPC.RemoveBan(Name))
            {
                LoadingbannedPC();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadOnlinePlayers();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            uint Hours = uint.Parse(textBox1.Text);
            if (Hours > 0)
            {
                var player = GetPlayer(comboBox1.Text);
                if (player != null)
                {
                    SystemBannedAccount.AddBan(player, Hours, "By~GM");
                    player.Socket.Disconnect();
                    LoadOnlinePlayers();
                }
                else
                    Console.WriteLine("sorry cant find player " + comboBox1.Text);
            }
            else
                Console.WriteLine("sorry please select hours banned from 1 to 999");

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            SystemBannedAccount.Client player = GetPlayerBanned(comboBox3.Text);
            if (player != null)
            {
                textBox4.Text = player.UID.ToString();//UID
                textBox5.Text = player.Name;//PlayerName
                textBox7.Text = player.Reason;//reson
            }
            else
                Console.WriteLine("sorry cant find player " + comboBox1.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Loadingbanned();

        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            uint UID = uint.Parse(textBox4.Text);
            if (SystemBannedAccount.RemoveBan(UID))
            {
                Console.WriteLine("Done remove player " + comboBox3.Text + " from banned ");
                textBox4.Text = "";//UID
                textBox5.Text = "";//PlayerName
                textBox7.Text = "";//reson
                Loadingbanned();
            }
            else
                Console.WriteLine("sorry cant remove player" + comboBox3.Text + " from banned");

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a player first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string playerName = comboBox1.SelectedItem.ToString();
            string messageText = txtMessage.Text.Trim();

            if (string.IsNullOrEmpty(messageText))
            {
                MessageBox.Show("Please enter a message first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var target = GetPlayerByName(playerName);

            if (target == null)
            {
                MessageBox.Show("The selected player is no longer online!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                HazemShield.SendForceClose(target, "Manage Server", messageText);
                System.Console.WriteLine($"Message sent to {playerName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send message: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            string Caption = textBox2.Text;
            string Message = richTextBox1.Text;
            foreach (var C in Database.Server.GamePoll.Values)
            {
                // SendForceClose schedules the disconnect itself after the message is delivered.
                HazemShield.SendForceClose(C, Caption, Message);

            }
            LoadOnlinePlayers();

            Console.WriteLine("Done Closed All Client's");

        }
        private void CMDCommand_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            foreach (var Unlock in Database.UnLockLogin.UnLockLoginPoll.Values)
            {
                comboBox2.Items.Add(Unlock.PlayerName);
            }
            foreach (var Clicker in Database.Allow_Clicker.AllowClickerPoll.Values)
            {
                comboBox2.Items.Add(Clicker.PlayerName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (Database.UnLockLogin.AddLockLog(client))
            {
                comboBox1.Text = "";
                LoadOnlinePlayers();
                // AddLockLog / AddAllowClicker already persisted their own file (no full server save needed)
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (client == null)
                return;
            if (Database.Allow_Clicker.AddAllowClicker(client))
            {
                comboBox1.Text = "";
                LoadOnlinePlayers();
                // AddLockLog / AddAllowClicker already persisted their own file (no full server save needed)
            }

        }

        private void RemoveLogin_Click(object sender, EventArgs e)
        {
            try
            {
                UnLockLogin.RemoveLock(comboBox2.Text);
                comboBox2.Items.Clear();
                foreach (var Unlock in Database.UnLockLogin.UnLockLoginPoll.Values)
                {
                    comboBox2.Items.Add(Unlock.PlayerName);
                }
            }
            catch
            {
                comboBox2.Items.Clear();
                foreach (var Unlock in Database.UnLockLogin.UnLockLoginPoll.Values)
                {
                    comboBox2.Items.Add(Unlock.PlayerName);

                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                Database.Allow_Clicker.RemoveAllowClicker(comboBox5.Text);
                comboBox5.Items.Clear();
                foreach (var Clicker in Database.Allow_Clicker.AllowClickerPoll.Values)
                {
                    comboBox5.Items.Add(Clicker.PlayerName);
                }
            }
            catch
            {
                comboBox5.Items.Clear();
                foreach (var Clicker in Database.Allow_Clicker.AllowClickerPoll.Values)
                {
                    comboBox5.Items.Add(Clicker.PlayerName);

                }
            }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }
    }
}
