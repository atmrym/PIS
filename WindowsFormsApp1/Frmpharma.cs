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
using System.Security.Cryptography;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace WindowsFormsApp1
{
    public partial class Frmpharma : Form
    {
        //sql connection
        SqlConnection conn = new SqlConnection(@"Data Source=MRYM;Initial Catalog=PIS;Integrated Security=True");

        public DataRow activeuser;

        public Frmpharma(DataRow activeuser)
        {
            InitializeComponent();
            this.activeuser = activeuser;
        }



        //functions


        //reorderlimit function
        void Reorderalert()
        {

            string querry = "SELECT name ,quantity, reoder_limit FROM \"items\" ";
            SqlDataAdapter sda = new SqlDataAdapter(querry, conn);

            DataTable q = new DataTable();
            sda.Fill(q);

            foreach (DataRow quant in q.Rows)
            {
                if (int.Parse(quant["quantity"].ToString()) == 0)
                {
                    MessageBox.Show(quant["name"].ToString() + " is out of stock");
                }
                else if (int.Parse(quant["quantity"].ToString()) < int.Parse(quant["reoder_limit"].ToString()))
                {
                    MessageBox.Show(quant["name"].ToString() + " should be reordered");
                }
            }
        }

        // refreshable pharma table

        void Refreshtable()
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

        void SuppBox()
        {
            DataTable q = new DataTable();
            DataTable b = new DataTable();
            for (int i = 0; i < tblitems.Rows.Count; i++)
            {
                string itemname = tblitems.Rows[i].Cells[0].Value.ToString();

                string querry = "SELECT supplier_id FROM \"items\" WHERE name = '" + itemname + "' ";
                SqlDataAdapter sda = new SqlDataAdapter(querry, conn);
                sda.Fill(q);
            }
            foreach (DataRow quant in q.Rows)
            {
                int supplierid = int.Parse(quant["supplier_id"].ToString());

                string selectquerry = "SELECT email FROM \"supplier\" WHERE supplier_id = '" + supplierid + "' ";
                SqlDataAdapter sda2 = new SqlDataAdapter(selectquerry, conn);
                sda2.Fill(b);
            }

            foreach (DataRow suppemail in b.Rows)
            {
                if (!boxsupplier.Items.Contains(suppemail["email"].ToString()))
                {
                    boxsupplier.Items.Add(suppemail["email"].ToString());
                }
            }
        }

        void Catbox()
        {

            string querry = "SELECT category_name FROM \"category\" ";
            SqlDataAdapter sda = new SqlDataAdapter(querry, conn);

            DataTable q = new DataTable();
            sda.Fill(q);
            foreach (DataRow c in q.Rows)
            {
                comboBox1.Items.Add(c["category_name"].ToString());
            }
        }

        void Supplierbox()
        {
            string querry = "SELECT email FROM \"supplier\" ";
            SqlDataAdapter sda = new SqlDataAdapter(querry, conn);

            DataTable q = new DataTable();
            sda.Fill(q);
            foreach (DataRow c in q.Rows)
            {
                comboBox2.Items.Add(c["email"].ToString());
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------

        private void label2_Click(object sender, EventArgs e) { }

        //load name at runtime
        private void Frmpharma_Load(object sender, EventArgs e)
        {
            conn.Open();
            // all panels hidden
            pnladd.Hide();
            pnlitems.Hide();
            pnlsupplier.Hide();
            pnlorederitems.Hide();
            pnlsuppl.Hide();

            Reorderalert();
            //diplay name
            string fname, lname;
            fname = this.activeuser["first_name"].ToString();
            lname = this.activeuser["last_name"].ToString();
            lblname.Text = string.Format(fname + lname);
            //table
            Refreshtable();
            Refreshtable2();

            //edit buttons hide
            btnsave.Hide();
            btncancel.Hide();

            //category / supplier box load
            Catbox();
            Supplierbox();

        }


        //--------------------------------------------------------------------------------------------------------------------------

        //exit
        private void bttnexit_Click(object sender, EventArgs e)
        {
            conn.Close();
            System.Environment.Exit(0);
        }
        //logout
        private void button4_Click(object sender, EventArgs e)
        {
            conn.Close();
            this.Close();
        }
        //search items
        private void btnsearch_Click(object sender, EventArgs e)
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

        private void tblpharma_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        //pharma show
        private void pharma_Click(object sender, EventArgs e)
        {
            tblpharma.Show();
            tblnonpharma.Hide();
            tblsearch.Hide();
            Refreshtable();
        }
        //non-pharma show
        private void nonpharma_Click(object sender, EventArgs e)
        {
            tblnonpharma.Show();
            tblpharma.Hide();
            tblsearch.Hide();
            Refreshtable();
        }

        private void label6_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label14_Click(object sender, EventArgs e) { }





        //--------------------------------------------------------------------------------------------------------------------------


        //order to supplier

        // add items to order from supplier
        int prevsuppid = 0;
        private void btnadditems_Click(object sender, EventArgs e)
        {
            string itemnames, itemquants;
            itemnames = txtitemnames.Text;
            itemquants = txtitemquants.Text;
            DataTable dtable = new DataTable();

            if ((itemnames == "") || (itemquants == ""))
            {
                MessageBox.Show(" Missing Data");
            }

            else
            {
                try
                {

                    string querry = "SELECT name , sales_price , supplier_id FROM \"items\" WHERE name like '" + itemnames + "' ";
                    SqlDataAdapter sda = new SqlDataAdapter(querry, conn);

                    sda.Fill(dtable);
                    DataRow item = dtable.Rows[0];


                    int suppid = int.Parse(item["supplier_id"].ToString());
                    //Items must be same supplier
                    if (int.Parse(item["supplier_id"].ToString()) != prevsuppid && prevsuppid != 0)
                    {
                        MessageBox.Show("Items should have same supplier", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtitemnames.Clear();
                        txtitemquants.Clear();
                    }

                    else
                    {
                        prevsuppid = int.Parse(item["supplier_id"].ToString());

                        object[] data = { item["name"], itemquants, item["sales_price"] };
                        tblitems.Rows.Add(data);

                        txtitemnames.Clear();
                        txtitemquants.Clear();

                        //add availabel suppliers to combobox
                        SuppBox();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // Remove Row

        private void btndeletes_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to remove this item ??", "Confirm Delete!!", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                int rowindex = tblitems.CurrentCell.RowIndex;
                tblitems.Rows.RemoveAt(rowindex);

            }
        }
        // New order
        private void btnneworders_Click(object sender, EventArgs e)
        {
            tblitems.Rows.Clear();
            lbltotals.Text = "0";
            boxsupplier.Items.Clear();
            prevsuppid = 0;
        }

        // Send order to supplier
        private void btngenerates_Click(object sender, EventArgs e)
        {
            double totalamount = 0;
            string userid = this.activeuser["user_id"].ToString();

            try
            {
                //calculate total amount
                for (int i = 0; i < tblitems.Rows.Count; i++)
                {
                    totalamount += (double.Parse(tblitems.Rows[i].Cells[2].Value.ToString()) * double.Parse(tblitems.Rows[i].Cells[1].Value.ToString()));
                }

                //Generate Invoice
                lbltotals.Text = string.Format(totalamount.ToString());

                //supplier combobox
                int selectedIndex = boxsupplier.SelectedIndex;
                Object supplieremail = boxsupplier.SelectedItem;

                //Insert to database
                string querry1 = "SELECT supplier_id FROM \"supplier\" WHERE email = '" + supplieremail + "' ";
                SqlDataAdapter sda1 = new SqlDataAdapter(querry1, conn);

                DataTable s = new DataTable();
                sda1.Fill(s);
                DataRow sup = s.Rows[0];

                int supplierid = int.Parse(sup["supplier_id"].ToString());

                DateTime date = DateTime.Now;
                lbldatesupp.Text = date.ToString();

                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("INSERT INTO \"ordertosupplier\" ( total , user_id , supplier_id , date ) VALUES ( '" + totalamount + "' , '" + userid + "' , '" + supplierid + "' , '" + date + "')", conn);
                cmd.ExecuteNonQuery();

                // plus quantity from database
                for (int i = 0; i < tblitems.Rows.Count; i++)
                {
                    int currentquant, newquantity, orderedquant;
                    string itemname = tblitems.Rows[i].Cells[0].Value.ToString();

                    string querry = "SELECT quantity FROM \"items\" WHERE name = '" + itemname + "' ";
                    SqlDataAdapter sda = new SqlDataAdapter(querry, conn);

                    DataTable q = new DataTable();
                    sda.Fill(q);
                    DataRow quant = q.Rows[0];

                    currentquant = int.Parse(quant["quantity"].ToString());
                    orderedquant = int.Parse(tblitems.Rows[i].Cells[1].Value.ToString());

                    newquantity = currentquant + orderedquant;

                    SqlCommand cmd2 = new SqlCommand();
                    cmd2 = new SqlCommand("UPDATE \"items\" SET quantity = ( '" + newquantity + "' ) WHERE name = '" + itemname + "' ", conn);
                    cmd2.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Reorderalert();
        }



        //--------------------------------------------------------------------------------------------------------------------------


        // Nav bar


        private void orderfromsupplier_Click(object sender, EventArgs e)
        {
            pnlsupplier.Show();
            pnlitems.Hide();
            pnladd.Hide();
            pnlorederitems.Hide();
            pnlsuppl.Hide();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            pnlitems.Show();
            pnlsupplier.Hide();
            pnladd.Hide();
            pnlorederitems.Hide();
            pnlsuppl.Hide();

            Refreshtable();
        }

        private void searchitem_Click(object sender, EventArgs e)
        {
            pnladd.Show();
            pnlitems.Hide();
            pnlsupplier.Hide();
            pnlorederitems.Hide();
            pnlsuppl.Hide();
        }

        private void orderitem_Click(object sender, EventArgs e)
        {
            Refreshtable();
            pnlorederitems.Show();
            pnlitems.Hide();
            pnlsupplier.Hide();
            pnladd.Hide();
            pnlsuppl.Hide();
        }

        private void btnsupplier_Click(object sender, EventArgs e)
        {
            pnlsuppl.Show();
            pnlorederitems.Hide();
            pnlitems.Hide();
            pnlsupplier.Hide();
            pnladd.Hide();

            Refreshtable2();
        }


        //--------------------------------------------------------------------------------------------------------------------------


        //order items

        //Add Items to cart

        private void btnadd_Click_1(object sender, EventArgs e)
        {
            string itemname, itemquant;
            itemname = txtitemname.Text;
            itemquant = txtitemquant.Text;

            if ((itemname == "") || (itemquant == ""))
            {
                MessageBox.Show(" Missing Data");
            }

            else
            {
                try
                {
                    string querry = "SELECT name, sales_price FROM \"items\" WHERE name like '" + itemname + "' ";
                    SqlDataAdapter sda = new SqlDataAdapter(querry, conn);

                    DataTable dtable = new DataTable();
                    sda.Fill(dtable);
                    DataRow item = dtable.Rows[0];

                    double itemx = double.Parse(item["sales_price"].ToString()) * 1.1;

                    object[] data = { item["name"], itemquant, itemx };
                    tblitem.Rows.Add(data);


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            txtitemname.Clear();
            txtitemquant.Clear();
        }

        // Generate invoice
        private void bttninvoice_Click_1(object sender, EventArgs e)
        {
            double totalamount, subtotal = 0, salestaxes, tax = 0.14;
            string userid = this.activeuser["user_id"].ToString();

            try
            {
                //calculate total amount
                for (int i = 0; i < tblitem.Rows.Count; i++)
                {
                    subtotal += (double.Parse(tblitem.Rows[i].Cells[2].Value.ToString()) * double.Parse(tblitem.Rows[i].Cells[1].Value.ToString()));
                }

                salestaxes = subtotal * tax;
                totalamount = subtotal + salestaxes;

                //Generate Invoice
                lblsubtotal.Text = string.Format(subtotal.ToString());
                lblsalestax.Text = string.Format(salestaxes.ToString());
                lbltotal.Text = string.Format(totalamount.ToString());

                DateTime date = DateTime.Now;
                lbldateitem.Text = date.ToString();

                //Insert to database
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("INSERT INTO \"invoice\" ( total_amount , user_id , date) VALUES ( '" + totalamount + "' , '" + userid + "' , '" + date + "')", conn);
                cmd.ExecuteNonQuery();

                // Minus quantity from database
                for (int i = 0; i < tblitem.Rows.Count; i++)
                {
                    int currentquant, newquantity, orderedquant;
                    string itemname = tblitem.Rows[i].Cells[0].Value.ToString();

                    string querry = "SELECT quantity FROM \"items\" WHERE name = '" + itemname + "' ";
                    SqlDataAdapter sda = new SqlDataAdapter(querry, conn);

                    DataTable q = new DataTable();
                    sda.Fill(q);
                    DataRow quant = q.Rows[0];

                    currentquant = int.Parse(quant["quantity"].ToString());
                    orderedquant = int.Parse(tblitem.Rows[i].Cells[1].Value.ToString());

                    newquantity = currentquant - orderedquant;

                    SqlCommand cmd2 = new SqlCommand();
                    cmd2 = new SqlCommand("UPDATE \"items\" SET quantity = ( '" + newquantity + "' ) WHERE name = '" + itemname + "' ", conn);
                    cmd2.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Reorderalert();
        }
        //Remove Items from ordered items
        private void btnremove_Click_1(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to remove this item ??", "Confirm Delete!!", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                int rowindex = tblitem.CurrentCell.RowIndex;
                tblitem.Rows.RemoveAt(rowindex);
            }
        }
        //new order
        private void btnneworder_Click_1(object sender, EventArgs e)
        {
            tblitem.Rows.Clear();
            lblsubtotal.Text = "0";
            lblsalestax.Text = "0";
            lbltotal.Text = "0";
        }

        private void txtitemnames_TextChanged(object sender, EventArgs e)
        {

        }



        //--------------------------------------------------------------------------------------------------------------------------


        //Add Item 


        private void bttnadd_Click(object sender, EventArgs e)
        {
            try
            {
                string validate = "SELECT name FROM \"items\" Where name = '" + txtname.Text + "'";
                SqlDataAdapter vl = new SqlDataAdapter(validate, conn);
                DataTable vltable = new DataTable();
                vl.Fill(vltable);

                bool alreadyRegistered = (vltable.Rows.Count > 0);

                if (alreadyRegistered)
                {
                    MessageBox.Show("Item already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtname.Clear();
                }
                else
                {
                    Object catname = comboBox1.SelectedItem;
                    Object suppemail = comboBox2.SelectedItem;

                    //Insert to database
                    string querry1 = "SELECT category_id FROM \"category\" WHERE category_name = '" + catname + "' ";
                    SqlDataAdapter sda1 = new SqlDataAdapter(querry1, conn);
                    DataTable s = new DataTable();
                    sda1.Fill(s);
                    DataRow sup = s.Rows[0];
                    int categoryid = int.Parse(sup["category_id"].ToString());


                    string querry2 = "SELECT supplier_id FROM \"supplier\" WHERE email = '" + suppemail + "' ";
                    SqlDataAdapter sda2 = new SqlDataAdapter(querry2, conn);
                    DataTable s2 = new DataTable();
                    sda2.Fill(s2);
                    DataRow supp = s2.Rows[0];
                    int suppid = int.Parse(supp["supplier_id"].ToString());

                    int quantity = 0;

                    SqlCommand cmd = new SqlCommand("INSERT INTO \"items\" ( name , description , sales_price , active_ingeredient , reoder_limit , cat_id , supplier_id , quantity ) VALUES ( @name , @desc , @price , @activeing , @reorderlimit , @catid , @supplierid , @quant)", conn);

                    cmd.Parameters.AddWithValue("@name", txtname.Text);
                    cmd.Parameters.AddWithValue("@desc", txtdesc.Text);
                    cmd.Parameters.AddWithValue("@price", double.Parse(txtprice.Text));
                    cmd.Parameters.AddWithValue("@activeing", txtactiveing.Text);
                    cmd.Parameters.AddWithValue("@reorderlimit", int.Parse(txtreorder.Text));
                    cmd.Parameters.AddWithValue("@catid", categoryid);
                    cmd.Parameters.AddWithValue("@supplierid", suppid);
                    cmd.Parameters.AddWithValue("@quant", quantity);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("successfully inserted ");

                    txtname.Clear();
                    txtdesc.Clear();
                    txtprice.Clear();
                    txtactiveing.Clear();
                    txtreorder.Clear();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }



        //Delete Item
        private void btndelete_Click(object sender, EventArgs e)
        {
            try
            {
                var confirmResult = MessageBox.Show("Are you sure to delete this item ??", "Confirm Delete!!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand("SELECT item_id FROM \"items\" WHERE name like @name or active_ingeredient like @active_ingeredient", conn);

                    cmd.Parameters.AddWithValue("name", txtsearch.Text);
                    cmd.Parameters.AddWithValue("active_ingeredient", string.Format("%{0}%", txtsearch.Text));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable item = new DataTable();
                    adapter.Fill(item);
                    DataRow r = item.Rows[0];

                    int itemid = int.Parse(r["item_id"].ToString());
                    SqlCommand cmd2 = new SqlCommand("Delete \"items\" where item_id = @id", conn);
                    cmd2.Parameters.AddWithValue("@id", itemid);
                    cmd2.ExecuteNonQuery();
                    MessageBox.Show("successfully deleted ");
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Refreshtable();
        }

        void GetItemFromTable()
        {

            SqlCommand cmd = new SqlCommand("SELECT name , description , sales_price , active_ingeredient , reoder_limit , cat_id , supplier_id FROM \"items\" WHERE name like @name or active_ingeredient like @active_ingeredient", conn);

            cmd.Parameters.AddWithValue("name", txtsearch.Text);
            cmd.Parameters.AddWithValue("active_ingeredient", string.Format("%{0}%", txtsearch.Text));
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable item = new DataTable();
            adapter.Fill(item);
            DataRow r = item.Rows[0];

            txtname.Text = r["name"].ToString();
            txtprice.Text = r["sales_price"].ToString();
            txtdesc.Text = r["description"].ToString();
            txtactiveing.Text = r["active_ingeredient"].ToString();
            txtreorder.Text = r["reoder_limit"].ToString();


            string querry1 = "SELECT category_name FROM \"category\" WHERE category_id = '" + r["cat_id"].ToString() + "' ";
            SqlDataAdapter sda1 = new SqlDataAdapter(querry1, conn);
            DataTable s = new DataTable();
            sda1.Fill(s);
            DataRow sup = s.Rows[0];
            string categoryname = sup["category_name"].ToString();


            string querry2 = "SELECT email FROM \"supplier\" WHERE supplier_id = '" + r["supplier_id"].ToString() + "' ";
            SqlDataAdapter sda2 = new SqlDataAdapter(querry2, conn);
            DataTable s2 = new DataTable();
            sda2.Fill(s2);
            DataRow supp = s2.Rows[0];
            string suppname = supp["email"].ToString();

            comboBox1.SelectedItem = categoryname;
            comboBox2.SelectedItem = suppname;

        }

        // update item
        private void btnedit_Click(object sender, EventArgs e)
        {

            pnladd.Show();
            pnlitems.Hide();

            bttnadd.Hide();
            btnsave.Show();
            btncancel.Show();

            lbladd.Text = "Edit Item";

            GetItemFromTable();
        }

        private void btnsave_Click(object sender, EventArgs e)
        {

            Object catname = comboBox1.SelectedItem;
            Object suppemail = comboBox2.SelectedItem;

            string querry1 = "SELECT category_id FROM \"category\" WHERE category_name = '" + catname + "' ";
            SqlDataAdapter sda1 = new SqlDataAdapter(querry1, conn);
            DataTable s = new DataTable();
            sda1.Fill(s);
            DataRow sup = s.Rows[0];
            int categoryid = int.Parse(sup["category_id"].ToString());


            string querry2 = "SELECT supplier_id FROM \"supplier\" WHERE email = '" + suppemail + "' ";
            SqlDataAdapter sda2 = new SqlDataAdapter(querry2, conn);
            DataTable s2 = new DataTable();
            sda2.Fill(s2);
            DataRow supp = s2.Rows[0];
            int suppid = int.Parse(supp["supplier_id"].ToString());

            SqlCommand cmd2 = new SqlCommand("SELECT item_id FROM \"items\" WHERE name like @name or active_ingeredient like @active_ingeredient", conn);

            cmd2.Parameters.AddWithValue("name", txtsearch.Text);
            cmd2.Parameters.AddWithValue("active_ingeredient", string.Format("%{0}%", txtsearch.Text));
            SqlDataAdapter adapter = new SqlDataAdapter(cmd2);
            DataTable item = new DataTable();
            adapter.Fill(item);
            DataRow r = item.Rows[0];

            int itemid = int.Parse(r["item_id"].ToString());

            SqlCommand cmd = new SqlCommand(" Update \"items\" set name=@name , description=@desc , sales_price=@price , active_ingeredient = @activeing , reoder_limit=@reorderlimit , cat_id=@catid , supplier_id=@supplierid WHERE item_id = '" + itemid + "'", conn);

            cmd.Parameters.AddWithValue("@name", txtname.Text);
            cmd.Parameters.AddWithValue("@desc", txtdesc.Text);
            cmd.Parameters.AddWithValue("@price", double.Parse(txtprice.Text));
            cmd.Parameters.AddWithValue("@activeing", txtactiveing.Text);
            cmd.Parameters.AddWithValue("@reorderlimit", int.Parse(txtreorder.Text));
            cmd.Parameters.AddWithValue("@catid", categoryid);
            cmd.Parameters.AddWithValue("@supplierid", suppid);
            cmd.ExecuteNonQuery();
            MessageBox.Show("successfully updated ");


            txtname.Clear();
            txtdesc.Clear();
            txtprice.Clear();
            txtactiveing.Clear();
            txtreorder.Clear();

            lbladd.Text = "Add Item";
            btnsave.Hide();
            btncancel.Hide();
            bttnadd.Show();

            Refreshtable();

            pnlitems.Show();
            pnladd.Hide();
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            pnlitems.Show();
            pnladd.Hide();

            txtname.Clear();
            txtdesc.Clear();
            txtprice.Clear();
            txtactiveing.Clear();
            txtreorder.Clear();

            lbladd.Text = "Add Item";
            btnsave.Hide();
            btncancel.Hide();

            bttnadd.Show();

        }

        private void label22_Click(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label27_Click(object sender, EventArgs e) { }
        private void label20_Click(object sender, EventArgs e) { }
        private void txtactiveing_TextChanged(object sender, EventArgs e) { }
        private void label19_Click(object sender, EventArgs e) { }
        private void txtreorder_TextChanged(object sender, EventArgs e) { }
        private void label17_Click(object sender, EventArgs e) { }
        private void txtdesc_TextChanged(object sender, EventArgs e) { }
        private void label16_Click(object sender, EventArgs e) { }
        private void txtprice_TextChanged(object sender, EventArgs e) { }
        private void label15_Click(object sender, EventArgs e) { }
        private void txtname_TextChanged(object sender, EventArgs e) { }
        private void lbladd_Click(object sender, EventArgs e) { }


        //view supplier

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

        private void tblsearchss_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void btnviewss_Click(object sender, EventArgs e)
        {
            tblsupplierss.Show();
            tblsearchss.Hide();
            Refreshtable2();
        }

        private void tblnonpharma_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
