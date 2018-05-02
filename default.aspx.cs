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
using System.Threading;

namespace iCARS.admin
{
    public partial class _default : System.Web.UI.Page
    {
        protected const string szConnectionString = "Data Source=ROBERT\\SQLEXPRESS;Initial Catalog=myDB;Integrated Security=True";
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
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand(String.Format("SELECT password FROM [dbo].[admin] WHERE username='{0}'", adUser, _encryptPWD(adPass)), scConn);

            Boolean blnAuth = false;
            try
            {
                scConn.Open();
                SqlDataReader sdr = scStatement.ExecuteReader();

                while (sdr.Read())
                {
                    if (sdr[0].ToString() == _encryptPWD(adPass)) { blnAuth = true; }
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

            if (blnAuth == true) { return 0; } else { return -1; }
        }

        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _getAdminIndex
         * Desc:    Retreives an admin index from the admin table and
         *              then returns it.
         * 
         * Returns: "ERROR!" if an error occurred or admin is not found. Otherwise, it
         *              will return the data in the data field selected.
         *****************************************************************************/
        protected int _getAdminIndex(string szUsername)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand(String.Format("select * from admin where username='{0}'", szUsername), scConn);

            int nRet = -1;

            try
            {
                scConn.Open();

                SqlDataReader sdr = scStatement.ExecuteReader();

                sdr.Read();
                nRet = Convert.ToInt32(sdr[0]);
            }
            catch (Exception m)
            {
                szLastErrorMessage = m.Message;
            }
            finally
            {
                scConn.Close();
            }

            return nRet;
        }

        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _getAdminField
         * Desc:    Retreives an admin from the admin table and then returns 
         *              a piece of the admin field based on nField (0 to 5).
         * 
         * Returns: "ERROR!" if an error occurred or admin is not found. Otherwise, it
         *              will return the data in the data field selected.
         *****************************************************************************/
        protected string _getAdminField(int nIndex, int nField)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand(String.Format("select * from admin where id='{0}'", nIndex), scConn);

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
            SqlConnection scConn = new SqlConnection(szConnectionString);
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
            if (Request.Cookies["nimda"] != null)
            {
                if (Request.Cookies["nimda"]["aduser"] != null) { adUSER = Request.Cookies["nimda"]["aduser"]; }
                if (Request.Cookies["nimda"]["adlvl"] != null) { adLVL = Request.Cookies["nimda"]["adlvl"]; }
                if (Request.Cookies["nimda"]["auth"] != null) { adAUTH = Request.Cookies["nimda"]["auth"]; }

                nCookieExist = 1;
            }

            //adLVL = "2";

            if (nCookieExist == 1)
            {
                SqlConnection scConn = new SqlConnection(szConnectionString);
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
                            szPage += "      <td class=\"lite\"><a href=\"default.aspx?op=admin&act=edit&id=" + sdr[0].ToString() + "\">EDIT</a></td>\n";
                        }
                        else
                        {
                            szPage += "      <td class=\"lite\"><a href=\"default.aspx?op=admin&act=edit&id=" + sdr[0].ToString()  + "\">EDIT</a> | <a href=\"default.aspx?op=admin&act=delete&id=" + sdr[0].ToString() + "\">DELETE</a></td>\n";
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


            /**************************************************************
             * If the admin is authorized and a super admin, then we      *
             *    can allow them to have access to add a admin            *
             *    to the database.                                        *
             /*************************************************************/

            if ((adLVL == "2") && (adAUTH == "1"))
            {
                szPage += "<br />\n<hr />\n<br />\n";
                szPage += "\n<form id=\"frmLogin\" method=\"post\">\n";
                szPage += "   <input type=\"hidden\" id=\"op\" name=\"op\" value=\"admin\">\n";
                szPage += "   <input type=\"hidden\" id=\"act\" name=\"act\" value=\"add\">";
                szPage += "<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
                szPage += "   <tr style=\"font-weight: bold; text-decoration: underline;\">\n";
                szPage += "      <td class=\"dark\">\n";
                szPage += "         <h1>ADD ADMIN</h1>\n";
                szPage += "         <br />\n";
                szPage += "         <table style=\"margin: auto; width: 100 %;\">\n";
                szPage += "            <tr>\n";
                szPage += "               <td style=\"text-align: right; width: 40%;\">Username:&nbsp;</td>\n";
                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"adUSER\" name=\"adUSER\" type=\"text\" /></td>\n";
                szPage += "            </tr>\n";
                szPage += "            <tr>\n";
                szPage += "               <td style=\"text-align: right; width: 40%;\">Password:&nbsp;</td>\n";
                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"adPASS\" name=\"adPASS\" type=\"text\" /></td>\n";
                szPage += "            </tr>\n";
                szPage += "            <tr>\n";
                szPage += "               <td style=\"text-align: right; width: 40%;\">Confirm Password:&nbsp;</td>\n";
                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"adCONF\" name=\"adCONF\" type=\"text\" /></td>\n";
                szPage += "            </tr>\n";
                szPage += "            <tr>\n";
                szPage += "               <td style=\"text-align: right; width: 40%;\">First Name:&nbsp;</td>\n";
                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"adFNAME\" name=\"adFNAME\" type=\"text\" /></td>\n";
                szPage += "            </tr>\n";
                szPage += "            <tr>\n";
                szPage += "               <td style=\"text-align: right; width: 40%;\">Last Name:&nbsp;</td>\n";
                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"adLNAME\" name=\"adLNAME\" type=\"text\" /></td>\n";
                szPage += "            </tr>\n";
                szPage += "            <tr>\n";
                szPage += "               <td style=\"text-align: right; width: 40%;\">[0, 1, or 2] Level:&nbsp;</td>\n";
                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"adLVL\" name=\"adLVL\" type=\"text\" /></td>\n";
                szPage += "            </tr>\n";
                szPage += "            <tr>\n";
                szPage += "               <td style=\"text-align: right; width: 40%;\"></td>\n";
                szPage += "               <td style=\"text-align: left; width: 60%;\"><div style=\"width: 301px; text-align: right;\"><input style=\"width: 100px;\" id=\"btnAddAdmin\" name=\"submit\" type=\"submit\" value=\"Add Admin\" /></div></td>\n";
                szPage += "            </tr>\n";
                szPage += "         </table>\n";
                szPage += "      </td>\n";
                szPage += "   </tr>\n";
                szPage += "</table></form>\n";
                szPage += "<br />\n<hr />\n<br />\n";
            }

            _content.InnerHtml = szPage;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            // _content.InnerHtml = "Cookie: " + Request.Cookies["nimda"]["auth"].ToString();
            //_content.InnerHtml = "Index: " + _getAdminIndex("ad1") + "<br />Last Error: " + szLastErrorMessage;
            //return;

            /********************************
             * Check if cookies are enabled *
             /*******************************/
            if (!Request.Browser.Cookies)
            {
                _content.InnerHtml = "Cookies are not enabled on your web browser, please enable cookies and then return to the admin page!";
                return;
            }
            


            /************************************************************************************************************************
            /* We need to check for admin privs. If there is no login info, saved cookie, ect..... We need to display a login form. *
             ************************************************************************************************************************/
            int nIsAdmin = 0;
            if (Request.Cookies["nimda"] != null)
            {
                if (Request.Cookies["nimda"]["auth"].ToString() == "1") { nIsAdmin = 1; }
                if (nIsAdmin != 1)
                    return;
            }


            /*****************************************************************************
             * Check for a post message, if there is one then we need to handle options  *
             *    as we are working with admin privs. such as add, edit, delete.         *
             /****************************************************************************/
            if (!this.IsPostBack)
            {
                /*  Check for admin authentication before displaying the admin page  */
                if (nIsAdmin == 1)
                {
                    // Determine what page we are looking at
                    if (Request.QueryString.Count != 0)
                    {
                        // handle pages
                        if (Request.QueryString["pid"] != null)
                        {
                            if (Request.QueryString["pid"].ToLower() == "admin")
                            {
                                _displayAdminPage();
                            }

                            else if (Request.QueryString["pid"].ToLower() == "brand")
                            {
                                _content.InnerHtml = "<div style=\"text-align: center;\">Display Brand</div>";
                            }

                            else if (Request.QueryString["pid"].ToLower() == "category")
                            {
                                _content.InnerHtml = "<div style=\"text-align: center;\">Display Category</div>";
                            }

                            else if (Request.QueryString["pid"].ToLower() == "item")
                            {
                                _content.InnerHtml = "<div style=\"text-align: center;\">Display Item</div>";
                            }

                            else
                            {
                                _displayAdminPage();
                            }
                        }

                        //_content.InnerHtml = "Query String: " + Request.QueryString.ToString();
                    }
                    else
                    {
                        _displayAdminPage();
                    }
                    
                }
                else // Display login Form
                {
                    string szPage = "";

                    szPage = "\n<form id=\"frmLogin\" method=\"post\">\n";
                    szPage += "   <input type=\"hidden\" id=\"op\" name=\"op\" value=\"login\">\n";
                    szPage += "   <table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
                    szPage += "      <tr style=\"font-weight: bold;\">\n";
                    szPage += "         <td class=\"dark\">\n";
                    szPage += "            <h1>Admin Login</h1>\n";
                    szPage += "            <br />\n";
                    szPage += "            <table style=\"margin: auto; width: 100%;\">\n";
                    szPage += "               <tr>\n";
                    szPage += "                  <td style=\"text-align: right; width: 50%;\">Username:&nbsp;</td>\n";
                    szPage += "                  <td style=\"text-align: left; width: 50%;\">\n";
                    szPage += "                     <input style=\"width: 300px;\" name=\"aduser\" id=\"aduser\" type=\"text\" />\n";
                    szPage += "                  </td>\n";
                    szPage += "               </tr>\n";
                    szPage += "               <tr>\n";
                    szPage += "                  <td style=\"text-align: right; width: 50%;\">Password:&nbsp;</td>\n";
                    szPage += "                  <td style=\"text-align: left; width: 50%;\">\n";
                    szPage += "                     <input style=\"width: 300px;\" name=\"adpass\" id=\"aduser\" type=\"text\" />\n";
                    szPage += "                  </td>\n";
                    szPage += "               </tr>\n";
                    szPage += "               <tr>\n";
                    szPage += "                  <td style=\"text-align: right; width: 50%;\">&nbsp;</td>\n";
                    szPage += "                  <td style=\"text-align: left; width: 50%;\">\n";
                    szPage += "                     <div style=\"width: 301px; text-align: right;\"><input style=\"width: 100px;\" id=\"btnLogin\" type=\"submit\" value=\"Login\" /></div></td>\n";
                    szPage += "               </tr>\n";
                    szPage += "            </table>\n";
                    szPage += "         </td>\n";
                    szPage += "      </tr>\n";
                    szPage += "   </table>\n";
                    szPage += "</form>\n";

                    _content.InnerHtml = szPage;
                }
            }
            else
            {
                /*****************************************************************************
                 * We have received a post back, we need to grab variables from the header.  *
                 *                                                                           *
                 *    (operation) op = admin, brand, category, item                          *
                 *    (action) act = add, remove, edit                                       *
                 *                                                                           *
                 *    Each operation has its own set of params that can be pulled from       *
                 *      the header to add, remove, edit things from each category.           *
                 /****************************************************************************/
                if (Request.Form["op"] == null)
                {
                    _content.InnerHtml = "<div style=\"text-align: center;\">A unknown error has occurred!<br /> <a href=\"default.aspx\">CLICK HERE</a> to return to the main page.</div>";
                    return;
                }

                // Grab the OPERATION PARAM
                string szOP = Request.Form["op"].ToString();
                szOP = szOP.ToLower();
                
                // clear content
                _content.InnerHtml = null;

                // Reset error counter
                int nErrFound = 0;

                // Deal with login param
                if (szOP == "login")
                {
                    /* Ensure the user entered some info */
                    if (!(Request.Form["aduser"].Length > 0)) { nErrFound = 1; }
                    if (!(Request.Form["adpass"].Length > 0)) { nErrFound = 1; }

                    if (nErrFound == 0)
                    {
                        /* Now, lets try to authenticate the user */
                        if (_authAdmin(Request.Form["aduser"], Request.Form["adpass"]) == 0)
                        {
                            // User/Pass = correct, so save to cooke and authenticate the user.
                            int nIndex = _getAdminIndex(Request.Form["aduser"]);

                            _writeCookie(Request.Form["aduser"], _getAdminField(nIndex, 5), 1);

                            // Reload the page so that we can display to their admin options
                            Response.Redirect("default.aspx");
                        }
                        else // nErrFound = 1; then we have an error.
                        {
                            // User/Pass = correct, so save to cooke and authenticate the user.
                            int nIndex = _getAdminIndex(Request.Form["aduser"].ToString());
                            _content.InnerHtml += "\n<div style =\"text-align: center;\">Invalid username or password!<br /><a href=\"default.aspx\" style=\"text-decoration: underline;\">Click here</a> to return to the login screen.</div>\n";
                        }
                    }
                    else
                    {
                        /*******************************************
                         * Error occurred, display it to the user! *
                         *******************************************/
                        _content.InnerHtml = "\n<div style =\"text-align: center;\">\n";
                        if (!(Request.Form["aduser"].Length > 0)) { _content.InnerHtml += "   Enter a valid username!<br />\n"; }
                        if (!(Request.Form["adpass"].Length > 0)) { _content.InnerHtml += "   Enter a valid password!<br />\n"; }

                        _content.InnerHtml += "\n   <br /><a href=\"default.aspx\" style=\"text-decoration: underline;\">Click here</a> to return to the login screen.\n";
                        _content.InnerHtml += "</div>";
                    }
                }

                /*************************************************
                /* Handle admin operations                       *
                 *************************************************/
                if (szOP == "admin")
                {
                    // Ensure that "act" exists
                    if (Request.Form["act"].Length > 0)
                    {
                        // Return "act" in lowercase
                        string szAct = Request.Form["act"].ToString().ToLower();

                        // Add a admin
                        if (szAct == "add")
                        {
                            nErrFound = 0;

                            // Make sure all values passed from the form
                            if (Request.Form["adUSER"] == null) { nErrFound = 1; }
                            if (Request.Form["adPASS"] == null) { nErrFound = 1; }
                            if (Request.Form["adCONF"] == null) { nErrFound = 1; }
                            if (Request.Form["adFNAME"] == null) { nErrFound = 1; }
                            if (Request.Form["adLNAME"] == null) { nErrFound = 1; }
                            if (Request.Form["adLVL"] == null) { nErrFound = 1; }

                            if (nErrFound == 1)
                            {
                                _content.InnerHtml = Request.Form.ToString();
                                return;
                            }

                            if (nErrFound == 0)
                            {
                                string szUser = Request.Form["adUSER"];
                                string szPass = Request.Form["adPASS"];
                                string szConf = Request.Form["adCONF"];
                                string szLName = Request.Form["adFNAME"];
                                string szFName = Request.Form["adLNAME"];
                                string szLVL = Request.Form["adLVL"];

                                string szPage = "<div style=\"text-align: center;\">[ ERROR ]<hr style=\"width: 300px;\"/>\n";
                                if (szUser.Length == 0)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">You need to enter a &#34;username!&#34;</div><br />\n";
                                }


                                if (szPass.Length == 0)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">You need to enter a &#34;password!&#34;</div><br />\n";
                                }

                                if (szConf.Length == 0)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">You need to confirm your &#34;password!&#34;</div><br />\n";
                                }

                                if (szPass.ToLower() != szConf.ToLower())
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">Your &#34;password&#34; and &#34;password confirmation&#34; do not match!</div><br />\n";
                                }

                                if (szFName.Length == 0)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">You need to enter a &#34;first name!&#34;</div><br />\n";
                                }

                                if (szLName.Length == 0)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">You need to enter a &#34;last name!&#34;</div><br />\n";
                                }

                                Boolean blnRet = Int32.TryParse(szLVL, out int nLVL);
                                if (blnRet == false)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">You need to enter a valid number, alphanumeric values are not allowed for &#34;level!&#34;</div><br />\n";
                                }

                                if (Enumerable.Range(0, 3).Contains(nLVL) == false)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">You need to enter a valid number between of (0, 1, or 2) for &#34;level!&#34;</div><br />\n";
                                }

                                //// CHECK IF ADMIN EXISTS ////
                                if ((nErrFound == 0) && (_getAdminIndex(szUser) > -1))
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">[[[ !!!ADMIN ALREADY EXISTS!!! ]]]</div><br />\n";
                                }

                                //////////////////////////
                                if (nErrFound == 1)
                                {
                                    _content.InnerHtml = szPage + "<hr style=\"width: 300px;\"/><a href=\"default.aspx\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                }
                                else
                                {

                                    if (_addAdmin(szUser, szPass, szFName, szLName, szLVL) != -1)
                                    {
                                        Response.Redirect("default.aspx");
                                    }
                                    else
                                    {
                                        _content.InnerHtml = "<div style=\"text-align: center;\">[ ERROR ]<hr style=\"width: 300px;\"/>\n<div style=\"color: #FF0000\">Error creating the admin &#34;" + szUser + ".&#34;</div><br />\n";
                                    }
                                }
                            }
                            else
                            {
                                _content.InnerHtml = "<div style=\"text-align: center;\">An unknown error has occurred!<a href=\"default.aspx\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                            }
                        }

                        // Edit a admin
                        else if (szAct == "edit")
                        {

                        }

                        // Remove a admin
                        else if (szAct == "delete")
                        {

                        }
                        else
                        {
                            _content.InnerHtml = "<div style=\"text-align: center;\">ERROR: Invalid operation!<br /><a href=\"default.aspx\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>\n";
                        }
                    }
                }

                if (szOP == "brand")
                {
                    _content.InnerHtml = "BRAND!";
                }

                if (szOP == "category")
                {
                    _content.InnerHtml = "CATEGORY!";
                }

                if (szOP == "item")
                {
                    _content.InnerHtml = "ITEM!";
                }
            }
        }
    }
}