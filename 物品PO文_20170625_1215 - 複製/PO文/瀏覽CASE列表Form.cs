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
    public partial class 瀏覽CASE列表Form : Form
    {
        public 瀏覽CASE列表Form()
        {
            InitializeComponent();
        }
        int mem = 12;//隨便弄一個會員先
        ManPowerEntities dbContext = new ManPowerEntities();

        private void 瀏覽CASE列表Form_Load(object sender, EventArgs e)
        {

        }
        int regionID; string lo;
        private void Label4_Click(object sender, EventArgs e)//標題點擊
        {
            f.flowLayoutPanel3.Controls.Clear();
            bpo.flowLayoutPanel1.Controls.Clear();
            bpo.pictureBox1.Image = null;
            int cas = int.Parse(((Label)sender).Name); //CaseID
            var q = from o in dbContext.Cases
                    join w in dbContext.Members on o.MemberID equals w.MemberID
                    join p in dbContext.Goods on o.CaseID equals p.CaseID
                    join j in dbContext.GdsSubClasses on p.GdsSubClassID equals j.GdsSubClassID
                    join c in dbContext.GoodsClasses on j.GdsClassID equals c.GdsClassID
                    join b in dbContext.CaseStatus on o.StatusID equals b.StatusID
                    join r in dbContext.Regions on o.RegionID equals r.RegionID

                    where o.CaseID == cas
                    select new
                    {
                        分類 = c.GdsClass,
                        小芬 = j.GdsSubClass1,
                        CaseTitle = o.CaseTitle,
                        內容 = o.CaseContent,
                        價點 = p.GdsPoint,
                        量 = p.GdsCount,
                        狀態 = b.StatusName,
                        截止日 = p.GdsDeadline,
                        p.GdsID,
                        區ID = r.RegionID,
                        地址 = o.Location,
                        電話 = o.Contact,
                        NickName = w.NickName,
                        開始日 = o.StartDateTime,
                        Case編號 = o.CaseID
                    };
            foreach (var a in q)
            {
                lo = a.地址;
                regionID = a.區ID;
                bpo.label1.Text = a.分類;
                bpo.label19.Text = a.小芬;
                bpo.label4.Text = a.CaseTitle;
                bpo.textBox3.Text = a.內容;
                bpo.textBox4.Text = a.價點.ToString();
                bpo.label18.Text = a.量.ToString();
                bpo.label10.Text = a.狀態;
                bpo.btnBuy.Click += BtnBuy_Click;
                bpo.btnBuy.Name = a.GdsID.ToString();
                bpo.btnBuy.Tag = mem;
                bpo.label13.Text = a.電話;
                bpo.textBox1.Text = a.NickName;
                bpo.textBox2.Text = a.開始日.ToString();
                bpo.label12.Text = a.Case編號.ToString();
            }
            var q5 = from p in dbContext.Regions
                     join c in dbContext.Cities on p.CityID equals c.CityID
                     where p.RegionID == regionID
                     select new { p.RegionName, c.CityName };
            foreach (var a in q5)
            {
                bpo.label11.Text = a.CityName + a.RegionName + lo;
            }
            var q6 = from p in dbContext.Pictures
                     where p.CaseID == cas
                     select new { p.Images };
            foreach (var a in q6)
            {
                MemoryStream ms = new MemoryStream(a.Images);
                Bitmap bmp = new Bitmap(ms);
                PictureBox image123 = new PictureBox();
                image123.SizeMode = PictureBoxSizeMode.StretchImage;
                image123.Size = new Size(160, 160);
                image123.BackColor = Color.White;
                image123.Padding = new Padding(7);
                image123.BorderStyle = BorderStyle.Fixed3D;
                image123.Image = bmp;
                image123.MouseEnter += Image123_MouseEnter;
                image123.MouseLeave += Image123_MouseLeave;
                image123.Click += Image123_Click;
                bpo.flowLayoutPanel1.Controls.Add(image123);
            }
            var q7 = (from p in dbContext.Pictures
                      where p.CaseID == cas
                      select new { p.Images }).Take(1);
            foreach (var a in q7)
            {
                MemoryStream ms = new MemoryStream(a.Images);
                Bitmap bmp = new Bitmap(ms);
                bpo.pictureBox1.Image = bmp;
            }
            //CASE物品完整資訊Form f = new CASE物品完整資訊Form();
            //發問 QQ = new 發問();
            var q1 = from p in dbContext.Members
                     where p.MemberID == mem
                     select new { p.NickName };

            foreach (var a in q1)
            {
                QQ.label3.Text = a.NickName;
                // QQ.label3.Name = cas.ToString();
            }
            QQ.button1.Click += Button1_Click;
            QQ.button1.Name = cas.ToString();
            f.flowLayoutPanel3.Controls.Add(bpo);
            f.flowLayoutPanel3.Controls.Add(QQ);
            //----------------------------------------------------------
            //屬於這CASEID的留言一開始就要進去

            i = 1;
            var q2 = from p in dbContext.Contents
                     join n in dbContext.Members on p.MemberID equals n.MemberID
                     where p.CaseID == cas
                     orderby p.MessageDateTime
                     select new { time = p.MessageDateTime, na = n.NickName, mess = p.MessageContent, p.AuthorRepeat };
            foreach (var a in q2)
            {
                QandA QA = new QandA();
                QA.textBox1.Text = a.mess;
                QA.label3.Text = a.na;
                QA.label1.Text = a.time.ToString();
                QA.textBox2.Text = a.AuthorRepeat;
                if (QA.textBox2.Text == "")
                {
                    QA.label4.Text = "賣家未回覆";
                }
                else
                {
                    QA.label4.Text = "賣家回覆";
                }
                QA.label2.Text = "問題" + i;
                i++;
                f.flowLayoutPanel3.Controls.Add(QA);
            }
            f.ShowDialog();

        }
        BuyForm buycar = new BuyForm();
        private void BtnBuy_Click(object sender, EventArgs e)
        {
            if (n == 0)
            {
                buycar.label2.Name = ((Button)sender).Name;//gdsID
                buycar.label5.Name = ((Button)sender).Tag.ToString();//mem
                buycar.test = functions;
                buycar.ShowDialog();
                n++;
            }
        }

        private void Image123_Click(object sender, EventArgs e)
        {
            bpo.pictureBox1.Image = ((PictureBox)sender).Image;
        }

        private void Image123_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackColor = Color.White;
            ((PictureBox)sender).Width -= 10;
            ((PictureBox)sender).Height -= 10;
        }

        private void Image123_MouseEnter(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackColor = Color.Blue;
            ((PictureBox)sender).Width += 10;
            ((PictureBox)sender).Height += 10;
        }
        ShowPo bpo = new ShowPo();
        CASE物品完整資訊Form f = new CASE物品完整資訊Form();
        int i = 1;
        發問 QQ = new 發問();  //不能一次開很多  事件會重複觸發  會存進多筆相同資料
        private void Button1_Click(object sender, EventArgs e)//送出提問
        {
            int cas = int.Parse(((Button)sender).Name); //CaseID
            if (QQ.textBox1.Text != "")
            {

                string messagecontent1 = QQ.textBox1.Text;
                dbContext.Contents.Add(new Content { CaseID = cas, MemberID = mem, MessageContent = messagecontent1, MessageDateTime = DateTime.Now });
                dbContext.SaveChanges();
                QQ.textBox1.Text = "";
                functions(cas);//刷新
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            版主PO文送出Form f = new 版主PO文送出Form();
            f.label13.Name = mem.ToString();
            f.Show();
        }

        private void 瀏覽CASE列表Form_Activated(object sender, EventArgs e)
        {
            n = 0;
            statu();//把該結案的結案
            this.flowLayoutPanel2.Controls.Clear();
            var q = from o in dbContext.Cases
                    join w in dbContext.Members on o.MemberID equals w.MemberID
                    join p in dbContext.Goods on o.CaseID equals p.CaseID
                    join s in dbContext.GdsSubClasses on p.GdsSubClassID equals s.GdsSubClassID
                    join c in dbContext.GoodsClasses on s.GdsClassID equals c.GdsClassID
                    where o.StatusID == 4//狀態為進行中  秀出
                    orderby o.StartDateTime descending
                    select new { CaseTitle = o.CaseTitle, NickName = w.NickName, StartDateTime = o.StartDateTime, 分類 = c.GdsClass, 小分類 = s.GdsSubClass1, 價點 = p.GdsPoint, o.CaseID };
            foreach (var a in q)
            {
                var test = (from v in dbContext.Pictures
                            where v.CaseID == a.CaseID
                            select v.Images).Take(1);
                simplePo po = new simplePo();
                foreach (var image in test)
                {
                    MemoryStream ms = new MemoryStream(image);
                    Bitmap bmp = new Bitmap(ms);
                    po.pictureBox1.Image = bmp;
                }
                po.label4.Text = a.CaseTitle;
                po.label4.Click += Label4_Click;
                po.label1.Text = a.分類;
                po.label7.Text = a.小分類;
                po.label5.Text = a.價點.ToString();
                po.label2.Text = a.NickName;
                po.pictureBox1.Click += PictureBox1_Click;
                string id = a.CaseID.ToString();
                po.label4.Name = id;
                po.pictureBox1.Name = id;
                string tim = a.StartDateTime.ToString();
                po.label3.Text = tim;
                this.flowLayoutPanel2.Controls.Add(po);
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            f.flowLayoutPanel3.Controls.Clear();
            bpo.pictureBox1.Image = null;
            bpo.flowLayoutPanel1.Controls.Clear();
            int cas = int.Parse(((PictureBox)sender).Name); //CaseID
            var q = from o in dbContext.Cases
                    join w in dbContext.Members on o.MemberID equals w.MemberID
                    join p in dbContext.Goods on o.CaseID equals p.CaseID
                    join j in dbContext.GdsSubClasses on p.GdsSubClassID equals j.GdsSubClassID
                    join c in dbContext.GoodsClasses on j.GdsClassID equals c.GdsClassID
                    join b in dbContext.CaseStatus on o.StatusID equals b.StatusID
                    join r in dbContext.Regions on o.RegionID equals r.RegionID

                    where o.CaseID == cas
                    select new
                    {
                        分類 = c.GdsClass,
                        小芬 = j.GdsSubClass1,
                        CaseTitle = o.CaseTitle,
                        內容 = o.CaseContent,
                        價點 = p.GdsPoint,
                        量 = p.GdsCount,
                        狀態 = b.StatusName,
                        截止日 = p.GdsDeadline,
                        p.GdsID,
                        區ID = r.RegionID,
                        地址 = o.Location,
                        電話 = o.Contact,
                        NickName = w.NickName,
                        開始日 = o.StartDateTime,
                        Case編號 = o.CaseID
                    };
            foreach (var a in q)
            {
                lo = a.地址;
                regionID = a.區ID;
                bpo.label1.Text = a.分類;
                bpo.label19.Text = a.小芬;
                bpo.label4.Text = a.CaseTitle;
                bpo.textBox3.Text = a.內容;
                bpo.textBox4.Text = a.價點.ToString();
                bpo.label18.Text = a.量.ToString();
                bpo.label10.Text = a.狀態;
                bpo.btnBuy.Click += BtnBuy_Click1;
                bpo.btnBuy.Name = a.GdsID.ToString();
                bpo.btnBuy.Tag = mem;
                bpo.label13.Text = a.電話;
                bpo.textBox1.Text = a.NickName;
                bpo.textBox2.Text = a.開始日.ToString();
                bpo.label12.Text = a.Case編號.ToString();
            }
            var q5 = from p in dbContext.Regions
                     join c in dbContext.Cities on p.CityID equals c.CityID
                     where p.RegionID == regionID
                     select new { p.RegionName, c.CityName };
            foreach (var a in q5)
            {
                bpo.label11.Text = a.CityName + a.RegionName + lo;
            }
            var q6 = from p in dbContext.Pictures
                     where p.CaseID == cas
                     select new { p.Images };
            foreach (var a in q6)
            {
                MemoryStream ms = new MemoryStream(a.Images);
                Bitmap bmp = new Bitmap(ms);
                PictureBox image123 = new PictureBox();
                image123.SizeMode = PictureBoxSizeMode.StretchImage;
                image123.Size = new Size(160, 160);
                image123.BackColor = Color.White;
                image123.Padding = new Padding(7);
                image123.BorderStyle = BorderStyle.Fixed3D;
                image123.Image = bmp;
                image123.MouseEnter += Image123_MouseEnter;
                image123.MouseLeave += Image123_MouseLeave;
                image123.Click += Image123_Click;
                bpo.flowLayoutPanel1.Controls.Add(image123);
            }
            var q7 = (from p in dbContext.Pictures
                      where p.CaseID == cas
                      select new { p.Images }).Take(1);
            foreach (var a in q7)
            {
                MemoryStream ms = new MemoryStream(a.Images);
                Bitmap bmp = new Bitmap(ms);
                bpo.pictureBox1.Image = bmp;
            }
            //CASE物品完整資訊Form f = new CASE物品完整資訊Form();
            //發問 QQ = new 發問();
            var q1 = from p in dbContext.Members
                     where p.MemberID == mem
                     select new { p.NickName };

            foreach (var a in q1)
            {
                QQ.label3.Text = a.NickName;
                // QQ.label3.Name = cas.ToString();
            }
            QQ.button1.Click += Button1_Click;
            QQ.button1.Name = cas.ToString();
            f.flowLayoutPanel3.Controls.Add(bpo);
            f.flowLayoutPanel3.Controls.Add(QQ);
            //----------------------------------------------------------
            //屬於這CASEID的留言一開始就要進去

            i = 1;
            var q2 = from p in dbContext.Contents
                     join n in dbContext.Members on p.MemberID equals n.MemberID
                     where p.CaseID == cas
                     orderby p.MessageDateTime
                     select new { time = p.MessageDateTime, na = n.NickName, mess = p.MessageContent, p.AuthorRepeat };
            foreach (var a in q2)
            {
                QandA QA = new QandA();
                QA.textBox1.Text = a.mess;
                QA.label3.Text = a.na;
                QA.label1.Text = a.time.ToString();
                QA.textBox2.Text = a.AuthorRepeat;
                if (QA.textBox2.Text == "")
                {
                    QA.label4.Text = "賣家未回覆";
                }
                else
                {
                    QA.label4.Text = "賣家回覆";
                }
                QA.label2.Text = "問題" + i;
                i++;
                f.flowLayoutPanel3.Controls.Add(QA);
            }
            f.ShowDialog();
        }
        int n = 0;
        private void BtnBuy_Click1(object sender, EventArgs e)
        {
            if (n == 0)
            {
                int gds = int.Parse(((Button)sender).Name);
                BuyForm buycar = new BuyForm();
                buycar.label2.Name = gds.ToString();//gdsID
                buycar.label5.Name = ((Button)sender).Tag.ToString();//mem
                buycar.test = functions;
                buycar.ShowDialog();
                n++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MyCase清單Form mycase = new MyCase清單Form();
            mycase.label7.Name = mem.ToString();
            mycase.ShowDialog();
        }
        
        public void functions(int cas)//方法:刷新完整資訊
        {
            f.flowLayoutPanel3.Controls.Clear();
            bpo.flowLayoutPanel1.Controls.Clear();
            var q = from o in dbContext.Cases
                    join w in dbContext.Members on o.MemberID equals w.MemberID
                    join p in dbContext.Goods on o.CaseID equals p.CaseID
                    join j in dbContext.GdsSubClasses on p.GdsSubClassID equals j.GdsSubClassID
                    join c in dbContext.GoodsClasses on j.GdsClassID equals c.GdsClassID
                    join b in dbContext.CaseStatus on o.StatusID equals b.StatusID
                    join r in dbContext.Regions on o.RegionID equals r.RegionID

                    where o.CaseID == cas
                    select new
                    {
                        分類 = c.GdsClass,
                        小芬 = j.GdsSubClass1,
                        CaseTitle = o.CaseTitle,
                        內容 = o.CaseContent,
                        價點 = p.GdsPoint,
                        量 = p.GdsCount,
                        狀態 = b.StatusName,
                        截止日 = p.GdsDeadline,
                        p.GdsID,
                        區ID = r.RegionID,
                        地址 = o.Location,
                        電話 = o.Contact,
                        NickName = w.NickName,
                        開始日 = o.StartDateTime,
                        Case編號 = o.CaseID
                    };
            foreach (var a in q)
            {
                lo = a.地址;
                regionID = a.區ID;
                bpo.label1.Text = a.分類;
                bpo.label19.Text = a.小芬;
                bpo.label4.Text = a.CaseTitle;
                bpo.textBox3.Text = a.內容;
                bpo.textBox4.Text = a.價點.ToString();
                bpo.label18.Text = a.量.ToString();
                bpo.label10.Text = a.狀態;
                bpo.btnBuy.Click += BtnBuy_Click;
                bpo.btnBuy.Name = a.GdsID.ToString();
                bpo.btnBuy.Tag = mem;
                bpo.label13.Text = a.電話;
                bpo.textBox1.Text = a.NickName;
                bpo.textBox2.Text = a.開始日.ToString();
                bpo.label12.Text = a.Case編號.ToString();
            }
            var q5 = from p in dbContext.Regions
                     join c in dbContext.Cities on p.CityID equals c.CityID
                     where p.RegionID == regionID
                     select new { p.RegionName, c.CityName };
            foreach (var a in q5)
            {
                bpo.label11.Text = a.CityName + a.RegionName + lo;
            }
            var q6 = from p in dbContext.Pictures
                     where p.CaseID == cas
                     select new { p.Images };
            foreach (var a in q6)
            {
                MemoryStream ms = new MemoryStream(a.Images);
                Bitmap bmp = new Bitmap(ms);
                PictureBox image123 = new PictureBox();
                image123.SizeMode = PictureBoxSizeMode.StretchImage;
                image123.Size = new Size(160, 160);
                image123.BackColor = Color.White;
                image123.Padding = new Padding(7);
                image123.BorderStyle = BorderStyle.Fixed3D;
                image123.Image = bmp;
                image123.MouseEnter += Image123_MouseEnter;
                image123.MouseLeave += Image123_MouseLeave;
                image123.Click += Image123_Click;
                bpo.flowLayoutPanel1.Controls.Add(image123);
            }
            var q7 = (from p in dbContext.Pictures
                      where p.CaseID == cas
                      select new { p.Images }).Take(1);
            foreach (var a in q7)
            {
                MemoryStream ms = new MemoryStream(a.Images);
                Bitmap bmp = new Bitmap(ms);
                bpo.pictureBox1.Image = bmp;
            }
            //CASE物品完整資訊Form f = new CASE物品完整資訊Form();
            //發問 QQ = new 發問();
            var q1 = from p in dbContext.Members
                     where p.MemberID == mem
                     select new { p.NickName };

            foreach (var a in q1)
            {
                QQ.label3.Text = a.NickName;
                // QQ.label3.Name = cas.ToString();
            }
            QQ.button1.Click += Button1_Click;
            QQ.button1.Name = cas.ToString();
            f.flowLayoutPanel3.Controls.Add(bpo);
            f.flowLayoutPanel3.Controls.Add(QQ);
            //----------------------------------------------------------
            //屬於這CASEID的留言一開始就要進去

            i = 1;
            var q2 = from p in dbContext.Contents
                     join n in dbContext.Members on p.MemberID equals n.MemberID
                     where p.CaseID == cas
                     orderby p.MessageDateTime
                     select new { time = p.MessageDateTime, na = n.NickName, mess = p.MessageContent, p.AuthorRepeat };
            foreach (var a in q2)
            {
                QandA QA = new QandA();
                QA.textBox1.Text = a.mess;
                QA.label3.Text = a.na;
                QA.label1.Text = a.time.ToString();
                QA.textBox2.Text = a.AuthorRepeat;
                if (QA.textBox2.Text == "")
                {
                    QA.label4.Text = "賣家未回覆";
                }
                else
                {
                    QA.label4.Text = "賣家回覆";
                }
                QA.label2.Text = "問題" + i;
                i++;
                f.flowLayoutPanel3.Controls.Add(QA);
            }
        }
        void statu()
        {
            var q9 = from p in dbContext.Cases
                    join o in dbContext.Goods on p.CaseID equals o.CaseID
                     where o.GdsCount ==0 || o.GdsDeadline <= DateTime.Now
                     select p;
            foreach (var a in q9)
            {
                a.StatusID = 6;
                a.EndDateTime = DateTime.Now;
            }
            var q1 = from p in dbContext.Cases
                     join o in dbContext.Goods on p.CaseID equals o.CaseID
                     where o.GdsCount != 0 && o.GdsDeadline >= DateTime.Now
                     select p;
            foreach (var a in q1)
            {
                a.StatusID = 4;
                a.EndDateTime = null;
            }
            this.dbContext.SaveChanges();
        }
    }
}
