using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Conquer6609
{
    public partial class JiangHu : Form
    {
        private byte MyStage = 0;
        private byte MyLevel = 1;
        public object sync = new object();
        private string MyName = "";
        public JiangHu()
        {
            InitializeComponent();
            foreach (var item in Conquer6609.Role.Instance.JiangHu.Poll.Values)
            {
                Conquer6609.Client.GameClient client = null;
                if (Conquer6609.Database.Server.GamePoll.TryGetValue(item.UID, out client))
                {
                    comboBox1.Items.Add(client.Player.Name);
                }
            }
        }
       
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
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
                MyName = comboBox1.Text;
            }
        }
        private void JiangHu_Load(object sender, EventArgs e)
        {
            foreach (var item in Conquer6609.Role.Instance.JiangHu.Poll.Values)
            {
                Conquer6609.Client.GameClient client = null;
                if (Conquer6609.Database.Server.GamePoll.TryGetValue(item.UID, out client))
                {
                    comboBox1.Items.Add(client.Player.Name);
                }
            }
        }
       
        public string[] AtributesType = 
            {
               "MaxLife",
               "PAttack",
               "MAttack",
               "PDefense",
               "Mdefense",
               "FinalAttack",
               "FinalMagicAttack",
               "FinalDefense",
               "FinalMagicDefense",
               "CriticalStrike",
               "SkillCriticalStrike",
               "Immunity",
               "Breakthrough",
               "Counteraction",
               "MaxMana"
            };
     
        private void button_Click(object sender, EventArgs e)
        {
            lock (sync)
            {

                Button btn = sender as Button;
               
                var temp = (byte.Parse(btn.Text.Substring(btn.Text.Length - 1, 1)));
                var c = Conquer6609.Client.GameClient.CharacterFromName(MyName);
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
            var c = Conquer6609.Client.GameClient.CharacterFromName(MyName);
            if (c != null)
            {
                ComboBox Star = sender as ComboBox;
                c.Player.MyJiangHu.ArrayStages[MyStage - 1].Activate = true;
                c.Player.MyJiangHu.ArrayStages[MyStage - 1].ArrayStars[int.Parse(Star.Name.Substring(Star.Name.Length - 1, 1)) - 1].Activate = true;
                c.Player.MyJiangHu.ArrayStages[MyStage - 1].ArrayStars[int.Parse(Star.Name.Substring(Star.Name.Length - 1, 1)) - 1].Level = MyLevel;
                c.Player.MyJiangHu.ArrayStages[MyStage - 1].ArrayStars[int.Parse(Star.Name.Substring(Star.Name.Length - 1, 1)) - 1].Typ = (Conquer6609.Role.Instance.JiangHu.Stage.AtributesType)Enum.Parse(typeof(Conquer6609.Role.Instance.JiangHu.Stage.AtributesType), Star.Text);
                c.Player.MyJiangHu.ArrayStages[MyStage - 1].ArrayStars[int.Parse(Star.Name.Substring(Star.Name.Length - 1, 1)) - 1].UID = ValueToRoll(c.Player.MyJiangHu.ArrayStages[MyStage - 1].ArrayStars[int.Parse(Star.Name.Substring(Star.Name.Length - 1, 1)) - 1].Typ, MyLevel);
                if(MyStage < 9)
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

                using (var x = new Conquer6609.ServerSockets.RecycledPacket())
                {
                    var stream = x.GetStream();
                    c.Player.MyJiangHu.LoginClient(stream, c);
                }
            }

        }
        public ushort ValueToRoll(Conquer6609.Role.Instance.JiangHu.Stage.AtributesType status, byte level)
        {
            return (ushort)(((ushort)status) + (level * 0x100));
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

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (sync)
            {

                MyLevel = byte.Parse(comboBox10.Text);
            }
        }
    }
}
