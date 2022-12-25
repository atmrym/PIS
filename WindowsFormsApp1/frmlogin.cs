using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.ComponentModel.Design;

namespace WindowsFormsApp1
{
    public partial class frmlogin : Form
    {
        //sql connection
        SqlConnection conn = new SqlConnection(@"Data Source=MRYM;Initial Catalog=PIS;Integrated Security=True");
        public frmlogin( )
        {
            InitializeComponent();
        }
        //exit
        private void button1_Click(object sender, EventArgs e){
            conn.Close();
            System.Environment.Exit(0);
        }
        //connection open
        private void frmlogin_Load(object sender, EventArgs e){
            conn.Open();
        }
        //login
        private void bttnlogin_Click(object sender, EventArgs e){
            string email, password;
            int id, type;
            email = txtemail.Text;
            password = txtpassword.Text;
            try
            {
                string querry = "SELECT * FROM \"user\" WHERE email = '" + email + "' AND password = '" + password + "'";
                SqlDataAdapter sda = new SqlDataAdapter(querry, conn);

                DataTable dtable = new DataTable();
                sda.Fill(dtable);

                DataRow user = dtable.Rows[0];

                if (dtable.Rows.Count <= 0){
                    MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtemail.Clear();
                    txtpassword.Clear();

                }
                else{
                    // User Type
                    if (int.Parse(user["role_id"].ToString()) == 1 && int.Parse(user["user_id"].ToString()) != 0){
                        frmmanager manager = new frmmanager(user); //passing activeuser to Manager form
                        manager.ShowDialog();

                    }
                    else if (int.Parse(user["role_id"].ToString()) == 2 && int.Parse(user["user_id"].ToString()) != 0){
                            Frmpharma pharme = new Frmpharma(user); //passing activeuser to Phamra form
                            pharme.ShowDialog();

                        }
                    }
                txtemail.Clear();
                txtpassword.Clear();
            }
            
            catch (Exception ex){
                MessageBox.Show(ex.Message);
            }
            finally{
                conn.Close();
            }
        }
        //show and unshow password
        private void checkBox1_CheckedChanged(object sender, EventArgs e){
            if(checkBox1.Checked){
                txtpassword.PasswordChar = '\0';
            }
            else{
                txtpassword.PasswordChar = '*';
            }
        }

    }
}
