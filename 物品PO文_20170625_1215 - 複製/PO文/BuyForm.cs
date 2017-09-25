using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PO文
{
    public partial class BuyForm : Form
    {
        public BuyForm()
        {
            InitializeComponent();
        }
        ManPowerEntities dbContext = new ManPowerEntities();
        int gID;
        int mID;//顧客
        int qty;
        int gpoint;
        int pmID;//版主ID
        int cas;
        private void BuyForm_Load(object sender, EventArgs e)
        {
            this.comboBox1.Items.Clear();
            gID = int.Parse(this.label2.Name);
            mID = int.Parse(this.label5.Name);
            var q = from p in dbContext.Goods
                    where p.GdsID == gID
                    select new { p.GdsName, p.GdsPoint, p.GdsCount };
            foreach (var a in q)
            {
                label2.Text = a.GdsName;
                label7.Text = a.GdsPoint.ToString();
                label5.Text = a.GdsCount.ToString();
                gpoint = int.Parse(label7.Text);
                qty = int.Parse(label5.Text);
            }
            for (int i = 0; i < qty; i++)
            {
                this.comboBox1.Items.Add(i + 1);
            }
        }
        int total;//合計金額
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            total = gpoint * (int.Parse(comboBox1.Text));
            this.label8.Text = total.ToString();
        }
        int mpc;//會員點數
        private void button1_Click(object sender, EventArgs e)
        {
            int cou = int.Parse(comboBox1.Text);
            var q = from p in dbContext.Members
                    where p.MemberID == mID
                    select new { p.PointCount };
            foreach (var a in q)
            {
                mpc = a.PointCount;
            }
            if (mpc > total)
            {
                var q1 = from p in dbContext.Goods
                         where p.GdsID.Equals(gID)
                         select p;
                foreach (var a in q1)
                {
                    a.GdsCount = a.GdsCount - cou;//扣商品數量
                }
                var q2 = from p in dbContext.Members
                         where p.MemberID.Equals(mID)
                         select p;
                foreach (var a in q2)
                {
                    a.PointCount = a.PointCount - total;//扣會員點數
                }
                var q3 = from p in dbContext.Cases
                         join o in dbContext.Goods on p.CaseID equals o.CaseID
                         where o.GdsID == gID
                         select new { p.MemberID,p.CaseID };
                foreach(var a in q3)
                {
                    pmID = a.MemberID;//找到版主
                    cas = a.CaseID;
                }
                var q4 = from p in dbContext.Members
                         where p.MemberID.Equals(pmID)
                         select p;
                foreach(var aa in q4)
                {
                    aa.PointCount = aa.PointCount + total;//加版主點數
                }
                    
                dbContext.Buys.Add(new Buy { GdsID = gID, MemberID = mID, BuyQty = cou });
                this.dbContext.SaveChanges();
                MessageBox.Show("成功購買!!");
                test.Invoke(cas);//呼叫刷新頁面方法  在這給cas
            }
            else
            {
                MessageBox.Show("點數不足");
            }
        }
        public delegate void testing(int a);
        public testing test;
    }
}
