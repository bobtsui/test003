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
    public partial class MyCase清單Form : Form
    {
        public MyCase清單Form()
        {
            InitializeComponent();
        }
        ManPowerEntities dbContext = new ManPowerEntities();
        private void MyCase清單Form_Activated(object sender, EventArgs e)//狀態為6  已結案
        {
            int mem = int.Parse(label7.Name);
            this.flowLayoutPanel2.Controls.Clear();
            var q = from o in dbContext.Cases
                    join w in dbContext.Members on o.MemberID equals w.MemberID
                    join p in dbContext.Goods on o.CaseID equals p.CaseID
                    join j in dbContext.GdsSubClasses on p.GdsSubClassID equals j.GdsSubClassID
                    join c in dbContext.GoodsClasses on j.GdsClassID equals c.GdsClassID
                    where o.MemberID == mem
                    orderby o.StartDateTime
                    select new { CaseTitle = o.CaseTitle,o.StatusID, StartDateTime = o.StartDateTime, 分類 = c.GdsClass,小分=j.GdsSubClass1, 價點 = p.GdsPoint, o.CaseID };

            foreach (var a in q)
            {
                var test = (from v in dbContext.Pictures
                            where v.CaseID == a.CaseID
                            select v.Images).Take(1);
                UserControlsimplepo po = new UserControlsimplepo();
                foreach (var image in test)
                {
                    MemoryStream ms = new MemoryStream(image);
                    Bitmap bmp = new Bitmap(ms);
                    po.pictureBox1.Image = bmp;
                }
                po.label4.Text = a.CaseTitle;
                po.button1.Click += Button1_Click;
                po.button1.Name = a.CaseID.ToString();
                po.label1.Text = a.分類;
                po.label2.Text = a.小分;
                po.label5.Text = a.價點.ToString();
                if(a.StatusID==6)
                {
                    po.label7.Text = "已結案";
                }
                string id = a.CaseID.ToString();
                po.label4.Name = id;
                string tim = a.StartDateTime.ToString();
                po.label3.Text = tim;
                this.flowLayoutPanel2.Controls.Add(po);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string cas = ((Button)sender).Name; //CaseID
            修改CASEForm sh = new 修改CASEForm();
            sh.label2.Name = cas;
            sh.ShowDialog();
        }
    }
}
