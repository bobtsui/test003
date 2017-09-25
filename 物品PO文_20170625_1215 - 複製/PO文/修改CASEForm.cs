using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PO文
{
    public partial class 修改CASEForm : Form
    {
        public 修改CASEForm()
        {
            InitializeComponent();
        }
        ManPowerEntities dbContext = new ManPowerEntities();
        int casid;
        int regionID1;

        int i;
        private void 修改CASEForm_Load(object sender, EventArgs e)
        {
            casid = int.Parse(label2.Name);
            var q = from p in dbContext.Cases
                    join o in dbContext.Goods on p.CaseID equals o.CaseID
                    join j in dbContext.GdsSubClasses on o.GdsSubClassID equals j.GdsSubClassID
                    join u in dbContext.GoodsClasses on j.GdsClassID equals u.GdsClassID

                    where p.CaseID == casid
                    select new
                    {
                        p.CaseTitle,
                        p.CaseContent,
                        o.GdsName,
                        u.GdsClass,
                        j.GdsSubClass1,
                        o.GdsPoint,
                        o.GdsCount,
                        p.Contact,
                        o.GdsDeadline,
                        p.Location,
                        p.RegionID
                    };
            foreach (var a in q)
            {
                textBox5.Text = a.CaseTitle;
                label1.Text = a.CaseTitle;
                textBox7.Text = a.GdsName;
                comboBox1.Items.Add(a.GdsClass);
                comboBox1.SelectedIndex = 0;
                comboBox4.Items.Add(a.GdsSubClass1);
                comboBox4.SelectedIndex = 0;
                textBox3.Text = a.CaseContent;
                textBox4.Text = a.GdsPoint.ToString();
                textBox8.Text = a.GdsCount.ToString();
                textBox2.Text = a.Contact;
                dateTimePicker1.Value = a.GdsDeadline;
                textBox6.Text = a.Location;
                regionID1 = a.RegionID;
            }
            var q1 = from p in dbContext.Regions
                     join c in dbContext.Cities on p.CityID equals c.CityID
                     where p.RegionID == regionID1
                     select new { p.RegionName, c.CityName };
            foreach (var a in q1)
            {
                comboBox2.Items.Add(a.CityName);
                comboBox2.SelectedIndex = 0;
                comboBox3.Items.Add(a.RegionName);
                comboBox3.SelectedIndex = 0;
            }
            var q6 = from p in dbContext.Pictures
                     where p.CaseID == casid
                     select new { p.Images };
            foreach (var a in q6)
            {
                MemoryStream ms = new MemoryStream(a.Images);
                Bitmap bmp = new Bitmap(ms);
                PictureBox image123 = new PictureBox();
                image123.SizeMode = PictureBoxSizeMode.StretchImage;
                image123.Size = new Size(200, 200);
                image123.BackColor = Color.White;
                image123.Padding = new Padding(7);
                image123.BorderStyle = BorderStyle.Fixed3D;
                image123.Image = bmp;
                this.flowLayoutPanel1.Controls.Add(image123);
            }
            //////////////////////////////////////////////////////
            i = 1;
            var q2 = from p in dbContext.Contents
                     join n in dbContext.Members on p.MemberID equals n.MemberID
                     where p.CaseID == casid
                     orderby p.MessageDateTime
                     select new { time = p.MessageDateTime, na = n.NickName, mess = p.MessageContent, p.ContentID, p.AuthorRepeat };
            foreach (var a in q2)
            {
                Ans QA = new Ans();
                QA.textBox1.Text = a.mess;
                QA.label3.Text = a.na;
                QA.label1.Text = a.time.ToString();
                QA.label2.Text = "問題" + i;
                QA.textBox2.Text = a.AuthorRepeat;
                i++;
                if (QA.textBox2.Text == "")
                {
                    QA.label4.Text = "賣家未回覆";
                }
                else
                {
                    QA.label4.Text = "賣家回覆";
                }
                QA.btnAns.Name = a.ContentID.ToString();
                QA.btnAns.Click += BtnAns_Click;
                QA.textBox2.TextChanged += TextBox2_TextChanged;
                this.flowLayoutPanel2.Controls.Add(QA);
            }
        }
        string A;
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            A = ((TextBox)sender).Text;
        }
        
        private void BtnAns_Click(object sender, EventArgs e)
        {
            int conID = int.Parse(((Button)sender).Name);
            var q = from p in dbContext.Contents
                    where p.ContentID.Equals(conID)
                    select p;
            
            foreach (var a in q)
            {
                a.AuthorRepeat = A;

            }
            this.dbContext.SaveChanges();
            this.flowLayoutPanel2.Controls.Clear();
            修改CASEForm_Load(sender, e);
            this.tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            var q = from p in dbContext.GoodsClasses
                    select new { p.GdsClass };
            foreach (var a in q)
            {
                comboBox1.Items.Add(a.GdsClass);
            }
        }
        int f;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Items.Clear();
            var q = from p in dbContext.GoodsClasses
                    where p.GdsClass == comboBox1.Text
                    select new { p.GdsClassID };
            foreach (var a in q)
            {
                f = a.GdsClassID;
            }
            var q1 = from p in dbContext.GdsSubClasses
                     where p.GdsClassID == f
                     select new { p.GdsSubClass1 };
            foreach (var a in q1)
            {
                comboBox4.Items.Add(a.GdsSubClass1);
            }
        }


        private void comboBox2_Click(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            var q = from p in dbContext.Cities
                    select new { p.CityName };
            foreach (var a in q)
            {
                comboBox2.Items.Add(a.CityName);
            }
        }

        int c;
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            var q = from p in dbContext.Cities
                    where p.CityName == comboBox2.Text
                    select new { p.CityID };
            foreach (var a in q)
            {
                c = a.CityID;
            }
            var q1 = from p in dbContext.Regions
                     where p.CityID == c
                     select new { p.RegionName };
            foreach (var a in q1)
            {
                comboBox3.Items.Add(a.RegionName);
            }
        }

        int qty = 0;//圖數量
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] filename = openFileDialog1.FileNames;
                for (int i = 1; i <= filename.Length; i++)
                {
                    PictureBox p = new PictureBox();
                    p.SizeMode = PictureBoxSizeMode.StretchImage;
                    p.Size = new Size(200, 200);
                    p.BackColor = Color.White;
                    p.Padding = new Padding(7);
                    p.BorderStyle = BorderStyle.Fixed3D;
                    p.ImageLocation = filename[i - 1];
                    this.flowLayoutPanel1.Controls.Add(p);
                    qty++;
                }
            }
        }
        
        private void btnRemove_Click(object sender, EventArgs e)
        {
            this.flowLayoutPanel1.Controls.Clear();
            var q = from p in dbContext.Pictures
                    where p.CaseID == casid
                    select p;
            foreach (var a in q)
            {
                dbContext.Pictures.Remove(a);
                
            }dbContext.SaveChanges();
            qty = 0;
        }
        int re, gd;
        private void button1_Click_1(object sender, EventArgs e)
        {
            var r = from p in dbContext.Regions
                    where p.RegionName == comboBox3.Text
                    select new { p.RegionID };
            foreach (var a in r)
            {
                re = a.RegionID;
            }
            var r1 = from p in dbContext.GdsSubClasses
                     where p.GdsSubClass1 == comboBox4.Text
                     select new { p.GdsSubClassID };
            foreach (var a in r1)
            {
                gd = a.GdsSubClassID;
            }
            //----------------------------------------------------
            var q = from p in dbContext.Cases
                    where p.CaseID.Equals(casid)
                    select p;
            foreach (var a in q)
            {
                a.CaseTitle = textBox5.Text;
                a.CaseContent = textBox3.Text;
                a.Contact = textBox2.Text;
                a.RegionID = re;
                a.Location = textBox6.Text;
            }
            var q2 = from p in dbContext.Goods
                     where p.CaseID.Equals(casid)
                     select p;
            foreach (var a in q2)
            {
                a.GdsCount = int.Parse(textBox8.Text);
                a.GdsPoint = int.Parse(textBox4.Text);
                a.GdsName = textBox7.Text;
                a.GdsSubClassID = gd;
                a.GdsDeadline = dateTimePicker1.Value;
            }
            this.dbContext.SaveChanges();
            if (qty > 0)
            {
                for (int i = 1; i <= qty; i++)
                {
                    string s;
                    s = ((PictureBox)this.flowLayoutPanel1.Controls[i - 1]).ImageLocation;
                    FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
                    byte[] data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    dbContext.Pictures.Add(new Picture { CaseID = casid, Images = data });//圖用存  不是修改
                   
                    dbContext.SaveChanges();
                }
            }
            MessageBox.Show("成功修改");
        }
    }
}
