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
    public partial class 版主PO文送出Form : Form
    {
        public 版主PO文送出Form()
        {
            InitializeComponent();
        }

        int memberID1;//

        ManPowerEntities dbContext = new ManPowerEntities();
        int regionID1;
        int CaseId1;
        int GdssubClassID1;
        private void button1_Click(object sender, EventArgs e)
        {
            //CASE優先
            memberID1 = int.Parse(label13.Name);
            string CaseContent1 = textBox3.Text;
            string CaseTitle1 = textBox5.Text;
            string contact1 = textBox2.Text;
            string Location1 = textBox6.Text;//縣市要Load進來  區要在comboBox2屬性變化時讀取,條件等於 comboBox2.Text
            var q = from p in dbContext.Regions
                    where p.RegionName == comboBox3.Text//用comboBox3.Text 來找到區的ID
                    select new { p.RegionID };
            foreach (var a in q)
            {
                regionID1 = a.RegionID;
            }
            DateTime dt = DateTime.Now;
            int statusID = 4;
            dbContext.Cases.Add(new Case { Contact = contact1, MemberID = memberID1, CaseContent = CaseContent1, CaseTitle = CaseTitle1
                                                           , StartDateTime = dt, Location = Location1, RegionID = regionID1, StatusID = statusID });
            dbContext.SaveChanges();

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //GOODS
            string GdsName1 = textBox7.Text;
            int GdsCount1 = int.Parse(textBox8.Text);
            int GdsPoint1 = int.Parse(textBox4.Text);//價
            var q1 =( from p in dbContext.Cases
                     where p.MemberID == memberID1
                     orderby p.CaseID descending
                     select new { p.CaseID }).Take(1);
            foreach (var a in q1)
            {
                CaseId1 = a.CaseID;
            }//先寫死  上面完成後讀取CASE  條件用會員ID與內容  找到CaseID
            var q2 = from p in dbContext.GdsSubClasses
                     where p.GdsSubClass1 == comboBox4.Text
                     select new { p.GdsSubClassID };
            foreach (var a in q2)
            {
                GdssubClassID1 = a.GdsSubClassID;
            } //要先全部的項目load到這裡 然後用選到的取得分類ID
            dbContext.Goods.Add(new Good { GdsName = GdsName1, GdsCount = GdsCount1, GdsPoint = GdsPoint1, GdsDeadline = dateTimePicker1.Value
                                                            , CaseID = CaseId1, GdsSubClassID = GdssubClassID1 });
            dbContext.SaveChanges();
            if (qty > 0)
            {
                for (int i = 1; i <= qty; i++)
                {
                    string s;
                    s = ((PictureBox)this.flowLayoutPanel1.Controls[i - 1]).ImageLocation;
                    FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
                    byte[] data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    dbContext.Pictures.Add(new Picture { CaseID = CaseId1, Images = data });
                    dbContext.SaveChanges();
                }
            }

            //for (int i =0; i<= this.flowLayoutPanel1.Controls.Count-1; i++)
            //{
            //  ((PictureBox ) this.flowLayoutPanel1.Controls[i]).Tag
            //}
            MessageBox.Show("成功送出");

        }

        private void 版主PO文送出Form_Load(object sender, EventArgs e)
        {
            //縣市
            var q = from o in dbContext.Cities
                    orderby o.CityID
                    select new { o.CityName };

            foreach (var a in q)
            {
                comboBox2.Items.Add(a.CityName);
            }

            //物品分類
            var q1 = from o in dbContext.GoodsClasses
                     select new { o.GdsClass };
            foreach (var a in q1)
            {
                comboBox1.Items.Add(a.GdsClass);
            }

        }
        int s;//用來存cityID
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            var q = from o in dbContext.Cities
                    where o.CityName == comboBox2.Text
                    select new { o.CityID, o.CityName };

            foreach (var a in q)
            {
                s = a.CityID;
            }
            var q2 = from p in dbContext.Regions
                     where p.CityID == s
                     select new { p.RegionName };
            foreach (var a in q2)
            {
                comboBox3.Items.Add(a.RegionName);
            }
        }
        int gb;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Items.Clear();
            var q1 = from o in dbContext.GoodsClasses
                     where o.GdsClass == comboBox1.Text
                     select new { o.GdsClassID };
            foreach (var a in q1)
            {
                gb = a.GdsClassID;
            }
            var q2 = from o in dbContext.GdsSubClasses
                     where o.GdsClassID == gb
                     select new { o.GdsSubClass1 };
            foreach (var a in q2)
            {
                comboBox4.Items.Add(a.GdsSubClass1);
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
            qty = 0;
        }
    }
}
