using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.ComponentModel.Design;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace WindowsFormsApp1
{
    public partial class frmmanager : Form
    {
        //sql connection
        SqlConnection conn = new SqlConnection(@"Data Source=MRYM;Initial Catalog=PIS;Integrated Security=True");

        public DataRow activeuser;

        public frmmanager(DataRow activeuser)
        {
            InitializeComponent();
            this.activeuser = activeuser;
        }
        private void label1_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }

        //exit
        private void bttnexit_Click(object sender, EventArgs e)
        {
            conn.Close();
            System.Environment.Exit(0);
        }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void lblname_Click(object sender, EventArgs e) { }

        //load name at runtime
        private void frmmanager_Load(object sender, EventArgs e)
        {
            conn.Open();

            string fname, lname;
            fname = this.activeuser["first_name"].ToString();
            lname = this.activeuser["last_name"].ToString();
            lblname.Text = string.Format(fname + lname);

            // Hide all panels
            pnlreport.Hide();
            pnlsupplier.Hide();
            pnluser.Hide();
            pnlview.Hide();
            pnlviewitems.Hide();

            //edit buttons hide
            btnsave.Hide();
            btncancel.Hide();
            Rolebox();

            Refreshtable();
            Refreshtable2();
            Refreshtable3();

            tblsearchss.Hide();

            btncancelss.Hide();
            btnsavess.Hide();

            Viewreport();
        }
        //logout
        private void button4_Click(object sender, EventArgs e)
        {
            conn.Close();
            this.Close();
        }


        //add User
        void Rolebox()
        {

            string querry = "SELECT title FROM \"user_type\" ";
            SqlDataAdapter sda = new SqlDataAdapter(querry, conn);

            DataTable q = new DataTable();
            sda.Fill(q);
            foreach (DataRow c in q.Rows)
            {
                boxrole.Items.Add(c["title"].ToString());
            }
        }
        private void bttnadd_Click(object sender, EventArgs e)
        {
            try
            {
                string validate = "SELECT email FROM \"user\" Where email = '" + txtemail.Text + "'";
                SqlDataAdapter vl = new SqlDataAdapter(validate, conn);
                DataTable vltable = new DataTable();
                vl.Fill(vltable);

                bool alreadyRegistered = (vltable.Rows.Count > 0);

                if (alreadyRegistered)
                {
                    MessageBox.Show("User already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtemail.Clear();
                }
                else
                {
                    Object usertitle = boxrole.SelectedItem;

                    //Insert to database
                    string querry1 = "SELECT role_id FROM \"user_type\" WHERE title = '" + usertitle + "' ";
                    SqlDataAdapter sda1 = new SqlDataAdapter(querry1, conn);
                    DataTable s = new DataTable();
                    sda1.Fill(s);
                    DataRow sup = s.Rows[0];
                    int roleid = int.Parse(sup["role_id"].ToString());

                    SqlCommand cmd = new SqlCommand("INSERT INTO \"user\" ( first_name , last_name , age , email , phone , password , salary , role_id ) VALUES ( @fname , @lname , @age , @email , @phone , @pass , @salary , @roleid)", conn);

                    cmd.Parameters.AddWithValue("@fname", txtfname.Text);
                    cmd.Parameters.AddWithValue("@lname", txtlname.Text);
                    cmd.Parameters.AddWithValue("@age", int.Parse(txtage.Text));
                    cmd.Parameters.AddWithValue("@email", txtemail.Text);
                    cmd.Parameters.AddWithValue("@phone", int.Parse(txtphone.Text));
                    cmd.Parameters.AddWithValue("@pass", txtpass.Text);
                    cmd.Parameters.AddWithValue("@salary", int.Parse(txtsalary.Text));
                    cmd.Parameters.AddWithValue("@roleid", roleid);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("successfully inserted ");

                    txtfname.Clear();
                    txtlname.Clear();
                    txtage.Clear();
                    txtemail.Clear();
                    txtphone.Clear();
                    txtpass.Clear();
                    txtsalary.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }





        private void tblpharma_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnmanager_Click(object sender, EventArgs e)
        {
            tblmanager.Show();
            tblpharmacist.Hide();
            tblsearch.Hide();
            Refreshtable();
        }

        private void btnpharmacist_Click(object sender, EventArgs e)
        {
            tblpharmacist.Show();
            tblmanager.Hide();
            tblsearch.Hide();
            Refreshtable();
        }

        //search
        private void btnsearch_Click(object sender, EventArgs e)
        {
            tblsearch.Show();
            tblpharmacist.Hide();
            tblmanager.Hide();
            try
            {
                using (DataTable item = new DataTable("user"))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT first_name , last_name , age , email , phone , password , salary , role_id FROM \"user\" WHERE email like @email", conn))
                    {
                        cmd.Parameters.AddWithValue("email", string.Format("%{0}%", txtsearch.Text));
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(item);
                        tblsearch.DataSource = item;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        // refreshable user table

        void Refreshtable()
        {

            //Manager user
            try
            {
                using (DataTable mang = new DataTable("user"))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT first_name , last_name , age , email , phone , password , salary FROM \"user\" WHERE role_id = 1", conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(mang);
                        tblmanager.DataSource = mang;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //Pharmacist table
            try
            {
                using (DataTable pharma = new DataTable("user"))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT first_name , last_name , age , email , phone , password , salary FROM \"user\" WHERE role_id = 2", conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(pharma);
                        tblpharmacist.DataSource = pharma;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //delete
        private void btndelete_Click(object sender, EventArgs e)
        {
            try
            {

                var confirmResult = MessageBox.Show("Are you sure to delete this User??", "Confirm Delete!!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand("SELECT user_id FROM \"user\" WHERE email like @email", conn);

                    cmd.Parameters.AddWithValue("email", string.Format("%{0}%", txtsearch.Text));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable user = new DataTable();
                    adapter.Fill(user);
                    DataRow r = user.Rows[0];

                    int userid = int.Parse(r["user_id"].ToString());
                    SqlCommand cmd2 = new SqlCommand("Delete \"user\" where user_id = @id", conn);
                    cmd2.Parameters.AddWithValue("@id", userid);
                    cmd2.ExecuteNonQuery();
                    MessageBox.Show("successfully deleted ");

                    Refreshtable();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Edit
        void GetItemFromTable()
        {

            SqlCommand cmd = new SqlCommand("SELECT first_name , last_name , age , email , phone , password , salary , role_id FROM \"user\" WHERE email like @email", conn);

            cmd.Parameters.AddWithValue("email", string.Format("%{0}%", txtsearch.Text));
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable item = new DataTable();
            adapter.Fill(item);
            DataRow r = item.Rows[0];

            txtfname.Text = r["first_name"].ToString();
            txtlname.Text = r["last_name"].ToString();
            txtage.Text = r["age"].ToString();
            txtemail.Text = r["email"].ToString();
            txtphone.Text = r["phone"].ToString();
            txtpass.Text = r["password"].ToString();
            txtsalary.Text = r["salary"].ToString();


            string querry1 = "SELECT title FROM \"user_type\" WHERE role_id = '" + r["role_id"].ToString() + "' ";
            SqlDataAdapter sda1 = new SqlDataAdapter(querry1, conn);
            DataTable s = new DataTable();
            sda1.Fill(s);
            DataRow sup = s.Rows[0];
            string usertitle = sup["title"].ToString();

            boxrole.SelectedItem = usertitle;

        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            pnluser.Show();
            pnlview.Hide();

            bttnadd.Hide();
            btnsave.Show();
            btncancel.Show();

            lbladd.Text = "Edit User";

            GetItemFromTable();
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            Object usertitle = boxrole.SelectedItem;

            string querry1 = "SELECT role_id FROM \"user_type\" WHERE title = '" + usertitle + "' ";
            SqlDataAdapter sda1 = new SqlDataAdapter(querry1, conn);
            DataTable s = new DataTable();
            sda1.Fill(s);
            DataRow sup = s.Rows[0];
            int roleid = int.Parse(sup["role_id"].ToString());

            SqlCommand cmd2 = new SqlCommand("SELECT user_id FROM \"user\" WHERE email like @email ", conn);

            cmd2.Parameters.AddWithValue("email", string.Format("%{0}%", txtsearch.Text));
            SqlDataAdapter adapter = new SqlDataAdapter(cmd2);
            DataTable user = new DataTable();
            adapter.Fill(user);
            DataRow r = user.Rows[0];

            int userid = int.Parse(r["user_id"].ToString());

            SqlCommand cmd = new SqlCommand(" Update \"user\" set first_name=@fname , last_name=@lname , age=@age , email= @email , phone=@phone , password=@pass , salary=@salary , role_id=@roleid WHERE user_id = '" + userid + "'", conn);

            cmd.Parameters.AddWithValue("@fname", txtfname.Text);
            cmd.Parameters.AddWithValue("@lname", txtlname.Text);
            cmd.Parameters.AddWithValue("@age", int.Parse(txtage.Text));
            cmd.Parameters.AddWithValue("@email", txtemail.Text);
            cmd.Parameters.AddWithValue("@phone", int.Parse(txtphone.Text));
            cmd.Parameters.AddWithValue("@pass", txtpass.Text);
            cmd.Parameters.AddWithValue("@salary", int.Parse(txtsalary.Text));
            cmd.Parameters.AddWithValue("@roleid", roleid);
            cmd.ExecuteNonQuery();
            MessageBox.Show("successfully updated ");

            txtfname.Clear();
            txtlname.Clear();
            txtage.Clear();
            txtemail.Clear();
            txtphone.Clear();
            txtpass.Clear();
            txtsalary.Clear();

            lbladd.Text = "Add User";
            btnsave.Hide();
            btncancel.Hide();
            bttnadd.Show();

            Refreshtable();

            pnlview.Show();
            pnluser.Hide();
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            pnlview.Show();
            pnluser.Hide();

            txtfname.Clear();
            txtlname.Clear();
            txtage.Clear();
            txtemail.Clear();
            txtphone.Clear();
            txtpass.Clear();
            txtsalary.Clear();

            lbladd.Text = "Add User";
            btnsave.Hide();
            btncancel.Hide();

            bttnadd.Show();
        }

        private void tblitemsupplied_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pnlsupplier_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tblsupplierss_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //-------------------------------------------------------------------------------


        //supplier add

        void Refreshtable2()
        {

            //supplier
            try
            {
                using (DataTable supplier = new DataTable("supplier"))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT first_name , last_name , phone , address , email FROM \"supplier\"", conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(supplier);
                        tblsupplierss.DataSource = supplier;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //add supplier
        private void btnadss_Click(object sender, EventArgs e)
        {
            try
            {

                string validate = "SELECT email FROM \"supplier\" Where email = '" + txtemailss.Text + "'";
                SqlDataAdapter vl = new SqlDataAdapter(validate, conn);
                DataTable vltable = new DataTable();
                vl.Fill(vltable);

                bool alreadyRegistered = (vltable.Rows.Count > 0);

                if (alreadyRegistered)
                {
                    MessageBox.Show("User already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtemailss.Clear();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO \"supplier\" ( first_name , last_name , phone , address , email ) VALUES ( @fname , @lname , @phone , @address , @email)", conn);

                    cmd.Parameters.AddWithValue("@fname", txtfnamess.Text);
                    cmd.Parameters.AddWithValue("@lname", txtlnamess.Text);
                    cmd.Parameters.AddWithValue("@phone", int.Parse(txtphoness.Text));
                    cmd.Parameters.AddWithValue("@address", txtaddress.Text);
                    cmd.Parameters.AddWithValue("@email", txtemailss.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("successfully inserted ");

                    txtfnamess.Clear();
                    txtlnamess.Clear();
                    txtphoness.Clear();
                    txtaddress.Clear();
                    txtphone.Clear();
                    txtemailss.Clear();

                    Refreshtable2();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //search for supplier
        private void btnsearchss_Click(object sender, EventArgs e)
        {
            tblsearchss.Show();
            tblsupplierss.Hide();
            try
            {
                using (DataTable supplier = new DataTable("supplier"))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT first_name , last_name , phone , address , email FROM \"supplier\" WHERE email like @email", conn))
                    {
                        cmd.Parameters.AddWithValue("email", string.Format("%{0}%", txtsearchss.Text));
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(supplier);
                        tblsearchss.DataSource = supplier;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnviewss_Click(object sender, EventArgs e)
        {
            tblsupplierss.Show();
            tblsearchss.Hide();
            Refreshtable2();
        }


        //delete supplier
        private void btndeletess_Click(object sender, EventArgs e)
        {
            try
            {
                var confirmResult = MessageBox.Show("Are you sure to delete this supplier ??", "Confirm Delete!!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand("SELECT supplier_id FROM \"supplier\" WHERE email like @email", conn);

                    cmd.Parameters.AddWithValue("email", string.Format("%{0}%", txtsearchss.Text));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable supplier = new DataTable();
                    adapter.Fill(supplier);
                    DataRow r = supplier.Rows[0];

                    int supplierid = int.Parse(r["supplier_id"].ToString());
                    SqlCommand cmd2 = new SqlCommand("Delete \"supplier\" where supplier_id = @id", conn);
                    cmd2.Parameters.AddWithValue("@id", supplierid);
                    cmd2.ExecuteNonQuery();
                    MessageBox.Show("successfully deleted ");

                    Refreshtable();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Refreshtable2();
        }


        // show supplied Items
        private void bttnshow_Click(object sender, EventArgs e)
        {
            try
            {
                using (DataTable supplier = new DataTable("supplier"))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT supplier_id FROM \"supplier\" WHERE email like @email", conn))
                    {
                        cmd.Parameters.AddWithValue("email", string.Format("%{0}%", txtsearchss.Text));
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(supplier);
                        DataRow r = supplier.Rows[0];

                        int suppid = int.Parse(r["supplier_id"].ToString());

                        SqlCommand cmd2 = new SqlCommand(" SELECT name , sales_price FROM \"items\" WHERE supplier_id = '" + suppid + "' ", conn);
                        SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
                        DataTable item = new DataTable();
                        adapter2.Fill(item);
                        tblitemsuppliedss.DataSource = item;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //edit supplier

        void GetItemFromTable2()
        {

            SqlCommand cmd = new SqlCommand("SELECT first_name , last_name , phone , address , email  FROM \"supplier\" WHERE email like @email", conn);

            cmd.Parameters.AddWithValue("email", string.Format("%{0}%", txtsearchss.Text));
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable item = new DataTable();
            adapter.Fill(item);
            DataRow r = item.Rows[0];

            txtfnamess.Text = r["first_name"].ToString();
            txtlnamess.Text = r["last_name"].ToString();
            txtphoness.Text = r["phone"].ToString();
            txtaddress.Text = r["address"].ToString();
            txtemailss.Text = r["email"].ToString();

        }

        private void btnsavess_Click(object sender, EventArgs e)
        {
            SqlCommand cmd2 = new SqlCommand("SELECT supplier_id  FROM \"supplier\" WHERE email like @email", conn);

            cmd2.Parameters.AddWithValue("email", string.Format("%{0}%", txtsearchss.Text));
            SqlDataAdapter adapter = new SqlDataAdapter(cmd2);
            DataTable supplier = new DataTable();
            adapter.Fill(supplier);
            DataRow r = supplier.Rows[0];

            int supplierid = int.Parse(r["supplier_id"].ToString());

            SqlCommand cmd = new SqlCommand(" Update \"supplier\" set first_name=@fname , last_name=@lname , phone=@phone , address = @address , email=@email WHERE supplier_id = '" + supplierid + "'", conn);

            cmd.Parameters.AddWithValue("@fname", txtfnamess.Text);
            cmd.Parameters.AddWithValue("@lname", txtlnamess.Text);
            cmd.Parameters.AddWithValue("@phone", int.Parse(txtphoness.Text));
            cmd.Parameters.AddWithValue("@address", txtaddress.Text);
            cmd.Parameters.AddWithValue("@email", txtemailss.Text);
            cmd.ExecuteNonQuery();
            MessageBox.Show("successfully updated ");

            txtfnamess.Clear();
            txtlnamess.Clear();
            txtphoness.Clear();
            txtaddress.Clear();
            txtphone.Clear();
            txtemailss.Clear();

            btnsavess.Hide();
            btncancelss.Hide();
            btnadss.Show();

            Refreshtable2();

        }

        private void btneditss_Click(object sender, EventArgs e)
        {
            btnadss.Hide();
            btnsavess.Show();
            btncancelss.Show();

            GetItemFromTable2();
        }

        private void btncancelss_Click(object sender, EventArgs e)
        {
            txtfnamess.Clear();
            txtlnamess.Clear();
            txtphoness.Clear();
            txtaddress.Clear();
            txtphone.Clear();
            txtemailss.Clear();

            btnsavess.Hide();
            btncancelss.Hide();
            btnadss.Show();

        }

        void Refreshtable3()
        {

            //pharma table
            try
            {
                using (DataTable pharma = new DataTable("items"))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT name , description , sales_price , active_ingeredient , quantity FROM \"items\" WHERE cat_id = 1", conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(pharma);
                        tblpharma.DataSource = pharma;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //non-pharma table
            try
            {
                using (DataTable nonpharma = new DataTable("items"))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT name , description , sales_price , active_ingeredient , quantity FROM \"items\" WHERE cat_id = 2", conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(nonpharma);
                        tblnonpharma.DataSource = nonpharma;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            tblsearch.Show();
            tblnonpharma.Hide();
            tblpharma.Hide();
            try
            {
                using (DataTable item = new DataTable("items"))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT name , description , sales_price , active_ingeredient , quantity FROM \"items\" WHERE name like @name or active_ingeredient like @active_ingeredient", conn))
                    {
                        cmd.Parameters.AddWithValue("name", txtsearch.Text);
                        cmd.Parameters.AddWithValue("active_ingeredient", string.Format("%{0}%", txtsearch.Text));
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(item);
                        tblsearch.DataSource = item;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void pharma_Click(object sender, EventArgs e)
        {
            tblpharma.Show();
            tblnonpharma.Hide();
            tblsearch.Hide();
            Refreshtable3();
        }

        private void nonpharma_Click(object sender, EventArgs e)
        {
            tblnonpharma.Show();
            tblpharma.Hide();
            tblsearch.Hide();
            Refreshtable3();
        }



        //--------------------------------------------------------------------------------


        private void btnuser_Click(object sender, EventArgs e)
        {
            pnlview.Show();
            pnlreport.Hide();
            pnlsupplier.Hide();
            pnlviewitems.Hide();
            pnluser.Hide();

            Refreshtable();
        }

        private void btnadd_Click(object sender, EventArgs e)
        {
            pnluser.Show();
            pnlreport.Hide();
            pnlsupplier.Hide();
            pnlview.Hide();
            pnlviewitems.Hide();
        }

        private void btnreport_Click(object sender, EventArgs e)
        {
            pnlreport.Show();
            pnlsupplier.Hide();
            pnluser.Hide();
            pnlview.Hide();
            pnlviewitems.Hide();
        }

        private void btnsupplier_Click(object sender, EventArgs e)
        {
            pnlsupplier.Show();
            pnlreport.Hide();
            pnluser.Hide();
            pnlview.Hide();
            pnlviewitems.Hide();

            Refreshtable2();
        }

        private void viewitem_Click(object sender, EventArgs e)
        {
            pnlviewitems.Show();
            pnlreport.Hide();
            pnlsupplier.Hide();
            pnluser.Hide();
            pnlview.Hide();

            Refreshtable3();
        }


        //-----------------------------------------------------------------------------------

        //financial reports

        void Viewreport()
        {
            // View 
            string querry3 = "SELECT report_id , total_revenue , total_expenses , total_profit , date FROM \"reports\" ";
            SqlDataAdapter sda3 = new SqlDataAdapter(querry3, conn);

            DataTable dtable3 = new DataTable();
            sda3.Fill(dtable3);
            tblreport.DataSource = dtable3;
        }

        private void btngenerate_Click(object sender, EventArgs e)
        {
            double totalrevenue = 0, totalexpenses = 0, totalprofit;
            //calculate
            string querry = "SELECT total_amount , date FROM \"invoice\" ";
            SqlDataAdapter sda = new SqlDataAdapter(querry, conn);

            DataTable dtable = new DataTable();
            sda.Fill(dtable);

            foreach (DataRow amount in dtable.Rows)
            {
                totalrevenue += int.Parse(amount["total_amount"].ToString());
            }

            string querry2 = "SELECT total , date FROM \"ordertosupplier\" ";
            SqlDataAdapter sda2 = new SqlDataAdapter(querry2, conn);

            DataTable dtable2 = new DataTable();
            sda2.Fill(dtable2);

            foreach (DataRow amount2 in dtable2.Rows)
            {
                totalexpenses += int.Parse(amount2["total"].ToString());
            }

            totalprofit = totalrevenue - totalexpenses;

            DateTime date = DateTime.Now;
            //Insert
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("INSERT INTO \"reports\" ( total_revenue , total_expenses , total_profit , date) VALUES ( '" + totalrevenue + "' , '" + totalexpenses + "' ,'" + totalprofit + "','" + date + "')", conn);
            cmd.ExecuteNonQuery();

            Viewreport();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this item ??", "Confirm Delete!!", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                int rowindex = tblreport.CurrentCell.RowIndex;

                int reportid = int.Parse(tblreport.Rows[rowindex].Cells[0].Value.ToString());

                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("DELETE FROM \"reports\" WHERE report_id = '" + reportid + "' ", conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show(" Deleted successfully ");

                Viewreport();
            }
        }
    }
}

