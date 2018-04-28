using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Configuration;
using System.Data.SqlClient;

namespace FinalProject
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Brands.Items.Add("Brands");
            Categories.Items.Add("Category");
            string connectionstring = "Data Source = localhost\\SQLEXPRESS; Initial Catalog = WebII; Integrated Security = SSPI";
            SqlConnection connect = new SqlConnection(connectionstring);
            SqlCommand fill = new SqlCommand("select Brand_Name from BRANDS;", connect);
            SqlDataReader myreader;
            connect.Open();
            SqlCommand fillB = new SqlCommand("select Cat_Name from CATEGORIES", connect);
            SqlDataReader myreaderC;

            myreader = fill.ExecuteReader();

            while (myreader.Read())
            {
               
                Brands.Items.Add(myreader["Brand_Name"] + "");
            }
            myreader.Close();
            myreaderC = fillB.ExecuteReader();
            while (myreaderC.Read())
            {
                Categories.Items.Add(myreaderC["Cat_Name"] + "");
            }

            myreaderC.Close();
            connect.Close();

        }

        protected void Brands_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sUrl = "~/Site/" + Brands.SelectedItem.Text + ".aspx";
            Response.Redirect(sUrl);
        }
        
        protected void Categories_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            string sUrl = "~/Site/" + Categories.SelectedItem.Text + ".aspx";
            Response.Redirect(sUrl);
        }
    }
}