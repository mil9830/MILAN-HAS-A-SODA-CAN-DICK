using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;                  // For UTF8Encoding
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;        // SQL Server Connection
using System.Security.Cryptography; // MD5, SHA256
using System.Data.SqlTypes;         // SQL Data Types

namespace iCARS.admin
{
    public partial class _default : System.Web.UI.Page
    {
        protected static string szLastErrorMessage = "";

        /*****************************************************************************
        * Author:  Robert Milan
        * Name:    _writeCookie
        * Desc:    Saves authentication of a logged in admin
        * 
        * Returns: None
        *****************************************************************************/
        protected void _writeCookie(string adUser, string adLVL, int nValue)
        {
            /* Create a cookie that lasts for 15 minutes */
            HttpCookie hcAdmin = new HttpCookie("nimda");
            hcAdmin["aduser"] = adUser;
            hcAdmin["adlvl"] = adLVL;
            hcAdmin["auth"] = nValue.ToString();
            hcAdmin.Expires = DateTime.Now.AddMinutes(15);

            /* Save cookie */
            Page.Response.Cookies.Add(hcAdmin);
        }


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _cryptMD5
         * Desc:    Encrypts a string into the MD5 encryption
         * 
         * Returns: MD5 HASH in uppercase format
         *****************************************************************************/

        protected string _cryptMD5(string szText)
        {
            // Calculate MD5
            byte[] btPWDEncoding = new UTF8Encoding().GetBytes(szText);
            byte[] btMD5Hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(btPWDEncoding);

            // Convert MD5 to all uppercase and return it
            return BitConverter.ToString(btMD5Hash).Replace("-", String.Empty).ToUpper();
        }

        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _cryptSHA256
         * Desc:    Encrypts a string into the SHA256 encryption
         * 
         * Returns: SHA256 encoding in Base64 Encoded format
         *****************************************************************************/
        protected string _cryptSHA256(string szText)
        {
            byte[] btPWDEncoding = Encoding.UTF8.GetBytes(szText);

            HashAlgorithm haSHA256 = new SHA256Managed();
            haSHA256.TransformFinalBlock(btPWDEncoding, 0, btPWDEncoding.Length);

            return Convert.ToBase64String(haSHA256.Hash);
        }

        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _encryptPWD
         * Desc:    Encrypts a a double encoding to confuse SQL Injectors incase
         *              access to the database is gained making it more difficult
         *              to figure out the correct hash to the password.
         * 
         * Returns: Double encoded encryption string.
         *****************************************************************************/

        protected string _encryptPWD(string szText)
        {
            return _cryptMD5(_cryptSHA256(szText));
        }


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _authAdmin
         * Desc:    Checks the password against the database of a admin.
         * 
         * Returns: 0 if the password is correct, -1 if the password is incorrect.
         *****************************************************************************/

        protected int _authAdmin(string adUser, string adPass)
        {
            SqlConnection scConn = new SqlConnection("Data Source=ROBERT\\SQLEXPRESS;Initial Catalog=myDB;Integrated Security=True");
            SqlCommand scStatement = new SqlCommand(String.Format("SELECT password FROM [dbo].[admin] WHERE username={0}", adUser, _encryptPWD(adPass)), scConn);

            scConn.Open();

            Boolean blnAuth = false;
            SqlDataReader sdr = scStatement.ExecuteReader();

            while (sdr.Read())
            {
                if (sdr[0].ToString() == _encryptPWD(adPass)) { blnAuth = true; }
            }

            scConn.Close();

            if (blnAuth == true) { return 0; } else { return -1; }
        }

        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _getAdminF
         * Desc:    Retreives an admin from the admin table and then returns 
         *              a piece of the admin field based on nField (0 to 5).
         * 
         * Returns: "ERROR!" if an error occurred or admin is not found. Otherwise, it
         *              will return the data in the data field selected.
         *****************************************************************************/
        protected string _getAdminF(int nIndex, int nField)
        {
            SqlConnection scConn = new SqlConnection("Data Source=ROBERT\\SQLEXPRESS;Initial Catalog=myDB;Integrated Security=True");
            SqlCommand scStatement = new SqlCommand(String.Format("select * from admin where id = {0}", nIndex), scConn);

            string szTemp = "";

            try
            {
                scConn.Open();

                SqlDataReader sdr = scStatement.ExecuteReader();

                sdr.Read();

                for (int n = 0; n <= 5; n++) { if (n == nField) { szTemp = sdr[n].ToString(); } }
            }
            catch (Exception m)
            {
                szLastErrorMessage = m.Message;
                szTemp = "ERROR!";
            }
            finally
            {
                scConn.Close();
            }

            return szTemp;
        }


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _addAdmin
         * Desc:    Adds an admin into the database.
         * 
         * Returns: (int) -1 if it fails, 0 if it succeeds.
         *****************************************************************************/
        protected int _addAdmin(string adUser, string adPass, string adFName, string adLName, string adLVL)
        {
            SqlConnection scConn = new SqlConnection("Data Source=ROBERT\\SQLEXPRESS;Initial Catalog=myDB;Integrated Security=True");
            SqlCommand scStatement = new SqlCommand("INSERT [dbo].[admin] ([username], [password], [fname], [lname], [lvl]) VALUES (@aduser, @adpass, @fname, @lname, @adlvl)", scConn);

            try
            {
                scStatement.Parameters.AddWithValue("@aduser", adUser);
                scStatement.Parameters.AddWithValue("@adpass", _encryptPWD(adPass));
                scStatement.Parameters.AddWithValue("@fname", adFName);
                scStatement.Parameters.AddWithValue("@lname", adLName);
                scStatement.Parameters.AddWithValue("@adlvl", adLVL);

                scStatement.Connection = scConn;
                scConn.Open();

                scStatement.ExecuteNonQuery();
            }
            catch (Exception m)
            {
                szLastErrorMessage = m.Message;
                return -1;
            }
            finally
            {
                scConn.Close();
            }

            return 0;
        }



        protected void _displayAdminPage()
        {
            string szPage = "\n<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
            szPage += "   <tr style=\"font-weight: bold; text-decoration: underline;\">\n";
            szPage += "      <td class=\"dark\">ID</td>\n";
            szPage += "      <td class=\"dark\">Username</td>\n";
            szPage += "      <td class=\"dark\">Password HASH</td>\n";
            szPage += "      <td class=\"dark\">First Name</td>\n";
            szPage += "      <td class=\"dark\">Last Name</td>\n";
            szPage += "      <td class=\"dark\">Level</td>\n";
            szPage += "      <td class=\"dark\"></td>\n";
            szPage += "   </tr>\n";

            string adUSER = "";
            string adLVL = "";
            string adAUTH = "";

            int nCookieExist = 0;
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["nimda"]["aduser"] != null) { adUSER = Request.Cookies["nimda"]["aduser"]; }
                if (Request.Cookies["nimda"]["adlvl"] != null) { adLVL = Request.Cookies["nimda"]["adlvl"]; }
                if (Request.Cookies["nimda"]["auth"] != null) { adAUTH = Request.Cookies["nimda"]["auth"]; }

                nCookieExist = 1;
            }

            adLVL = "2";
            nCookieExist = 1; ////////////////// TEMP
            if (nCookieExist == 1)
            {
                SqlConnection scConn = new SqlConnection("Data Source=ROBERT\\SQLEXPRESS;Initial Catalog=myDB;Integrated Security=True");
                SqlCommand scStatement = new SqlCommand("SELECT * from admin", scConn);

                try
                {
                    scConn.Open();

                    SqlDataReader sdr = scStatement.ExecuteReader();
                    while (sdr.Read())
                    {
                        szPage += "   <tr>\n";
                        szPage += "      <td class=\"lite\">" + sdr[0].ToString() + "</td>\n";
                        szPage += "      <td class=\"lite\">" + sdr[1].ToString() + "</td>\n";
                        szPage += "      <td class=\"lite\">" + sdr[2].ToString() + "</td>\n";
                        szPage += "      <td class=\"lite\">" + sdr[3].ToString() + "</td>\n";
                        szPage += "      <td class=\"lite\">" + sdr[4].ToString() + "</td>\n";

                        /*****************************************************************************
                         * Display the proper title based on the current admin level.                *
                         /****************************************************************************/
                        string szTemp = "";
                        if (sdr[5].ToString() == "0") { szTemp = "Moderator"; }
                        if (sdr[5].ToString() == "1") { szTemp = "Admin"; }
                        if (sdr[5].ToString() == "2") { szTemp = "Super Admin"; }

                        szPage += "      <td class=\"lite\">" + szTemp + "</td>\n";
                        /*****************************************************************************
                         *****************************************************************************/


                        szTemp = "";
                        int nADType = 0;
                        int nADCurr = 0;


                        // Current admin level of the current entry we are grabbing from the db
                        Int32.TryParse(sdr[5].ToString(), out nADType);

                        // Get the current level of the admin whose logged in
                        Int32.TryParse(adLVL, out nADCurr);

                        if ((nADCurr < nADType) || (nADCurr == nADType))
                        {
                            szPage += "      <td class=\"lite\"><a href=\"#\">EDIT</a></td>\n";
                        }
                        else
                        {
                            szPage += "      <td class=\"lite\"><a href=\"#\">EDIT</a> | <a href=\"#\">DELETE</a></td>\n";
                        }

                        szPage += "   </tr>\n";
                    }
                }
                catch (Exception m)
                {
                    szLastErrorMessage = m.Message;
                }
                finally
                {
                    scConn.Close();
                }
            }

            szPage += "</table>\n";
            /*szPage += "<br />\n";
            szPage += "<hr />\n";
            szPage += "<br />\n";
            szPage += "<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
            szPage += "   <tr style=\"font-weight: bold; text-decoration: underline;\">\n";
            szPage += "      <td class=\"dark\">\n";
            szPage += "         <h1>ADD ADMIN</h1>\n";
            szPage += "         <br />\n";
            szPage += "         <table style=\"margin: auto; width: 100 %;\">\n";
            szPage += "            <tr>\n";
            szPage += "               <td style=\"text-align: right; width: 40%;\">Username:&nbsp;</td>\n";
            szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"_aduser\" type=\"text\" /></td>\n";
            szPage += "            </tr>\n";
            szPage += "            <tr>\n";
            szPage += "               <td style=\"text-align: right; width: 40%;\">Password:&nbsp;</td>\n";
            szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"_adpass\" type=\"text\" /></td>\n";
            szPage += "            </tr>\n";
            szPage += "            <tr>\n";
            szPage += "               <td style=\"text-align: right; width: 40%;\">First Name:&nbsp;</td>\n";
            szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"_adfname\" type=\"text\" /></td>\n";
            szPage += "            </tr>\n";
            szPage += "            <tr>\n";
            szPage += "               <td style=\"text-align: right; width: 40%;\">Last Name:&nbsp;</td>\n";
            szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"_adlname\" type=\"text\" /></td>\n";
            szPage += "            </tr>\n";
            szPage += "            <tr>\n";
            szPage += "               <td style=\"text-align: right; width: 40%;\">Level:&nbsp;</td>\n";
            szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"_adlvl\" type=\"text\" /></td>\n";
            szPage += "            </tr>\n";
            szPage += "            <tr>\n";
            szPage += "               <td style=\"text-align: right; width: 40%;\">&nbsp;</td>\n";
            szPage += "               <td style=\"text-align: left; width: 60%;\"><div style=\"width: 301px; text-align: right;\"><input style=\"width: 100px;\" id=\"btnAddAdmin\" type=\"button\" value=\"Add Admin\" onclick=\"AddAdmin_OnCLICK\" runat=\"server\" /></div></td>\n";
            szPage += "            </tr>\n";
            szPage += "         </table>\n";
            szPage += "      </td>\n";
            szPage += "   </tr>\n";
            szPage += "</table>\n";*/

            if (adLVL == "2")
            {
                szPage += "<br />\n<hr />\n<br />\n";
                szPage += "<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
                szPage += "   <tr style=\"font-weight: bold; text-decoration: underline;\">\n";
                szPage += "      <td class=\"lite\"><br />[ADD ADMIN]<br /><br /></td>\n";
                szPage += "   </tr>\n";
                szPage += "</table>\n";
            }

            _content.InnerHtml = szPage;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                //_content.InnerHtml = "id = <b>" + Request.QueryString["id"] + "</b>";
                //_content.InnerHtml = _listAdmins();
                //_find_admin_by_id.InnerHtml = _getAdmin(1);


                /*  Check for admin authentication before displaying the admin page  */
                int nIsAdmin = 0;
                if (Request.Cookies["UserSettings"] != null)
                {
                    if (Request.Cookies["nimda"]["auth"] == "1") { nIsAdmin = 1; }
                }
                if (nIsAdmin == 1)
                {
                    _displayAdminPage();
                }
                else
                {

                 <table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">        " +
                        "<tr style=\"font-weight: bold; text-decoration: underline;\">            <td class=\"dark\">                <h1>Admin Login</h1>                <br />                <table style=\"margin: auto; width: 100%;\">                    <tr>                        <td style=\"text-align: right; width: 50%;\">Username:&nbsp;</td>                        <td style=\"text-align: left; width: 50%;\">                            <asp:TextBox ID=\"txtAdUser\" runat=\"server\" Width=\"300px\"></asp:TextBox>                        </td>                    </tr>                    <tr>                        <td style=\"text-align: right; width: 50%;\">Password:&nbsp;</td>                        <td style=\"text-align: left; width: 50%;\">                            <asp:TextBox ID=\"txtAdPass\" runat=\"server\" Width=\"300px\"></asp:TextBox>                        </td>                    </tr>                    <tr>                        <td style=\"text-align: right; width: 50%;\">&nbsp;</td>                        <td style=\"text-align: left; width: 50%;\"><div style=\"width: 301px; text-align: right;\"><asp:Button ID=\"btnLogin\" runat=\"server\" Text=\"Login\" /></div></td>                    </tr>                </table>            </td>        </tr>    </table>"

                        < table style="text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;">
        <tr style="font-weight: bold; text-decoration: underline;">
            <td class="dark">
                <h1>Admin Login</h1>
                <br />
                <table style="margin: auto; width: 100%;">
                    <tr>
                        <td style="text-align: right; width: 50%;">Username:&nbsp;</td>
                        <td style="text-align: left; width: 50%;">
                            <asp:TextBox ID="txtAdUser" runat="server" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 50%;">Password:&nbsp;</td>
                        <td style="text-align: left; width: 50%;">
                            <asp:TextBox ID="txtAdPass" runat="server" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 50%;">&nbsp;</td>
                        <td style="text-align: left; width: 50%;"><div style="width: 301px; text-align: right;"><asp:Button ID="btnLogin" runat="server" Text="Login" /></div></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
                }
                
            }
        }
    }
}