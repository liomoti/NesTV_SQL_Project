using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace NesTv_MR_Interface
{
    public partial class Form1 : Form
    {
        bool DevicesFlag = false;
        string conStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\moti\source\repos\NesTv_MR_Interface\NesTv_DB\HW3_SQL_NEW.mdf;Integrated Security=True;Connect Timeout=30";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_show_all_Click(object sender, EventArgs e)
        {
            DevicesFlag = true;
            lbldescription.Text = "All Customers";
            SqlConnection CON = new SqlConnection(conStr);
            CON.Open();
            SqlCommand com = new SqlCommand("SELECT CustomerID, FirstName+' '+Surname as CustomerName, Address, PhoneNum FROM tblCustomer", CON);
            try
            {

                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = com;
                DataTable dbset = new DataTable();
                sda.Fill(dbset);
                BindingSource bsource = new BindingSource();

                bsource.DataSource = dbset;
                DGV_NesTV.DataSource = bsource;
                sda.Update(dbset);     
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            CON.Close();
            foreach (DataGridViewColumn column in DGV_NesTV.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void textbox_cust_OnValueChanged(object sender, EventArgs e)
        {

        }
 

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }


        private void btn_show_by_cust_Click_1(object sender, EventArgs e)
        {
            
            DevicesFlag = false;
            lbldevices.Text = "";
            bunifuCustomDataGrid1.DataSource = null;
            bunifuCustomDataGrid1.Rows.Clear();
            bunifuCustomDataGrid1.Refresh();

            DGV_NesTV.DataSource = null;
            DGV_NesTV.Rows.Clear();
            DGV_NesTV.Refresh();

            lbldescription.Text = "Find Customer";
            int CustomerID = 0;
            if (int.TryParse(textbox_cust.Text.ToString(), out CustomerID))
            {
                //MessageBox.Show(textbox_cust.Text.ToString());
                SqlConnection CON = new SqlConnection(conStr);
                CON.Open();
                SqlCommand com = new SqlCommand("SELECT tblCustomer.CustomerID, FirstName+' '+Surname as CustomerName, Address, " +
                    "COUNT(tblDevices.BelongsToCustomer) AS NumberOfDevices, " +
                    "COUNT(tblRequest.CustomerID) AS NumberOfRequest " +
                    "FROM tblCustomer left outer join tblDevices " +
                    "ON tblDevices.BelongsToCustomer = tblCustomer.CustomerId " +
                    "full join tblRequest ON tblRequest.CustomerID = tblCustomer.CustomerId " +
                    "WHERE tblCustomer.CustomerID =" + CustomerID +
                    " GROUP BY tblCustomer.CustomerID,FirstName,Surname, Address ", CON);
                try
                {

                    SqlDataAdapter sda = new SqlDataAdapter();
                    sda.SelectCommand = com;
                    DataTable dbset = new DataTable();
                    sda.Fill(dbset);
                    BindingSource bsource = new BindingSource();

                    bsource.DataSource = dbset;
                    DGV_NesTV.DataSource = bsource;
                    sda.Update(dbset);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                CON.Close();
            }
            else
            {
                MessageBox.Show("Please enter a corecct number");
            }
            
            if (DGV_NesTV.Rows.Count == 0)
                lbldescription.Text = "Customer not found";
            else
                lbldescription.Text = "Find Customer "+ CustomerID;
        }

        private void textbox_cust_Enter(object sender, EventArgs e)
        {
            textbox_cust.Text = "";
        }

        private void textbox_cust_Leave(object sender, EventArgs e)
        {
            textbox_cust.Text = "Enter Customer ID";
        }

        private void DGV_NesTV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            bunifuCustomDataGrid1.DataSource = null;
            bunifuCustomDataGrid1.Rows.Clear();
            bunifuCustomDataGrid1.Refresh();
            if (DevicesFlag)
            {           
                if (e.RowIndex > 0)
                {
                    int index = int.Parse(DGV_NesTV.Rows[e.RowIndex].Cells[0].Value.ToString());
                    //MessageBox.Show(DGV_NesTV.Rows[e.RowIndex].Cells[0].Value.ToString());
                    SqlConnection CON = new SqlConnection(conStr);
                    CON.Open();
                    SqlCommand com = new SqlCommand("SELECT tblCustomer.CustomerID, FirstName+' '+Surname as CustomerName, " +
                        "tblDevices.serialNum AS DeviceSerialNum " +
                        "FROM tblCustomer left outer join tblDevices " +
                        "ON tblDevices.BelongsToCustomer = tblCustomer.CustomerId " +
                        "WHERE tblCustomer.CustomerID =" + index +
                        " GROUP BY tblCustomer.CustomerID,FirstName,Surname, serialNum ", CON);
                    try
                    {

                        SqlDataAdapter sda = new SqlDataAdapter();
                        sda.SelectCommand = com;
                        DataTable dbset = new DataTable();
                        sda.Fill(dbset);
                        BindingSource bsource = new BindingSource();

                        bsource.DataSource = dbset;
                        bunifuCustomDataGrid1.DataSource = bsource;
                        sda.Update(dbset);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    CON.Close();
                }
                if (bunifuCustomDataGrid1.DataSource == null)
                    lbldevices.Text = "No devices found for the selected customer";
                else
                    lbldevices.Text = "The customer devices:";
            }
            
        }

        private void btn_trg1_Click(object sender, EventArgs e)
        {
            DevicesFlag = false;
            lbldevices.Text = "";
            bunifuCustomDataGrid1.DataSource = null;
            bunifuCustomDataGrid1.Rows.Clear();
            bunifuCustomDataGrid1.Refresh();
            lbldescription.Text = "Freezing request between 2-3 months and technician plan to visit next month";
                SqlConnection CON = new SqlConnection(conStr);
                CON.Open();
                SqlCommand com = new SqlCommand("select RequestID, DateOfRequest, Description,"+
                 "tblCustomer.FirstName + ' ' + tblCustomer.SurName AS FullName,"+
                 "(SELECT count(*) from tblDevices where " +
                 "tblCustomer.CustomerId = tblDevices.BelongsToCustomer) as numOfDevices " +
                "from tblRequest inner join tblCustomer " +
                "on tblRequest.CustomerID = tblCustomer.CustomerId " +
"                 left outer join tblRequestForFreezing " +
                "on tblRequestForFreezing.RequesFreezingtID = tblRequest.RequestID " +
                "WHERE Period between 2 and 3 UNION " +
                "select RequestID, DateOfRequest, Description, tblCustomer.FirstName + ' ' + " +
                "tblCustomer.SurName AS FullName, " +
                 "(SELECT count(*) from tblDevices where " +
                 "tblCustomer.CustomerId = tblDevices.BelongsToCustomer)as numOfDevices " +
                "from tblRequest inner join tblCustomer " +
                "on tblRequest.CustomerID = tblCustomer.CustomerId " +
                "left outer join tblCallATechnician " +
                "on tblCallATechnician.RequestInvitetionID = tblRequest.RequestID", CON);
                try
                {

                    SqlDataAdapter sda = new SqlDataAdapter();
                    sda.SelectCommand = com;
                    DataTable dbset = new DataTable();
                    sda.Fill(dbset);
                    BindingSource bsource = new BindingSource();

                    bsource.DataSource = dbset;
                    DGV_NesTV.DataSource = bsource;
                    sda.Update(dbset);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                CON.Close();


        }

        private void btn_trg2_Click(object sender, EventArgs e)
        {
            DevicesFlag = false;
            lbldevices.Text = "";
            bunifuCustomDataGrid1.DataSource = null;
            bunifuCustomDataGrid1.Rows.Clear();
            bunifuCustomDataGrid1.Refresh();
            lbldescription.Text = "Technicians who participated in training but never visit in customers";
            SqlConnection CON = new SqlConnection(conStr);
            CON.Open();
            SqlCommand com = new SqlCommand("select distinct tblTechnician.TechnicianID, TechnicianName, "+
            "DATEDIFF(year, StartWorkDate, getdate()) as Seniority, Manager " +
            "from tblTrainingsTechnician inner join tblTechnician " +
            "on tblTrainingsTechnician.TechnicianID = tblTechnician.TechnicianID " +
            "left outer join tblTechnicianVisits " +
            "on tblTrainingsTechnician.TechnicianID = tblTechnicianVisits.TechnicianID " +
            "where tblTechnicianVisits.TechnicianID IS NULL; ", CON);
            try
            {

                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = com;
                DataTable dbset = new DataTable();
                sda.Fill(dbset);
                BindingSource bsource = new BindingSource();

                bsource.DataSource = dbset;
                DGV_NesTV.DataSource = bsource;
                sda.Update(dbset);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            CON.Close();
        }
    }
}
