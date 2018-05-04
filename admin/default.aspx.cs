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
         * Name:    _getBrandIndex
         * Desc:    Retreives a brand index from the brand table and
         *              then returns it.
         * 
         * Returns: "ERROR!" if an error occurred or admin is not found. Otherwise, it
         *              will return the data in the data field selected.
         *****************************************************************************/
        protected int _getBrandIndex(string szName)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand(String.Format("select * from brand where name='{0}'", szName), scConn);

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
         * Name:    _getBrandIndex
         * Desc:    Retreives a brand index from the brand table and
         *              then returns it.
         * 
         * Returns: "ERROR!" if an error occurred or admin is not found. Otherwise, it
         *              will return the data in the data field selected.
         *****************************************************************************/
        protected int _getCategoryIndex(string szName, int nBrandID)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand(String.Format("select * from category where name='{0}' and brand_id={1}", szName, nBrandID), scConn);

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
         * Name:    _getBrandField
         * Desc:    Retreives an admin from the admin table and then returns 
         *              a piece of the admin field based on nField (0 to 5).
         * 
         * Returns: "ERROR!" if an error occurred or admin is not found. Otherwise, it
         *              will return the data in the data field selected.
         *****************************************************************************/
        protected string _getBrandField(int nIndex, int nField)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand(String.Format("select * from brand where id='{0}'", nIndex), scConn);

            string szTemp = "";

            try
            {
                scConn.Open();

                SqlDataReader sdr = scStatement.ExecuteReader();

                sdr.Read();

                for (int n = 0; n <= 2; n++) { if (n == nField) { szTemp = sdr[n].ToString(); } }
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


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _addBrand
         * Desc:    Adds a brand into the database.
         * 
         * Returns: (int) -1 if it fails, 0 if it succeeds.
         *****************************************************************************/
        protected int _addBrand(string szName, string szDescription)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand("INSERT [dbo].[brand] ([name], [description]) VALUES (@BRAND, @DESC)", scConn);

            try
            {
                scStatement.Parameters.AddWithValue("@BRAND", szName);
                scStatement.Parameters.AddWithValue("@DESC", szDescription);

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


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _addCategory
         * Desc:    Adds a category into the database.
         * 
         * Returns: (int) -1 if it fails, 0 if it succeeds.
         *****************************************************************************/
        protected int _addCategory(string szName, int nBrandIndex)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand("INSERT [dbo].[category] ([name], [brand_id]) VALUES (@catName, @bID)", scConn);

            try
            {
                scStatement.Parameters.AddWithValue("@catName", szName);
                scStatement.Parameters.AddWithValue("@bID", nBrandIndex.ToString());

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


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _addAdmin
         * Desc:    Adds an admin into the database.
         * 
         * Returns: (int) -1 if it fails, 0 if it succeeds.
         *****************************************************************************/
        protected int _addItem(string szName, string szDescription, double fPrice, int nCatID, int nRatingID, int nLength, int nWidth, int nWeight, string szShippingInfo)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand("INSERT [dbo].[item] ([name], [description], [price], [cat_id], [rating_id], [length], [width], [weight], [shipping_info]) VALUES (@iName, @iDesc, @iPrice, @iCID, @iRID, @iLength, @iWidth, @iWeight, @iShippingInfo)", scConn);

            try
            {
                scStatement.Parameters.AddWithValue("@iName", szName);
                scStatement.Parameters.AddWithValue("@iDesc", szDescription);
                scStatement.Parameters.AddWithValue("@iPrice", fPrice.ToString());
                scStatement.Parameters.AddWithValue("@iCID", nCatID.ToString());
                scStatement.Parameters.AddWithValue("@iRID", nRatingID.ToString());
                scStatement.Parameters.AddWithValue("@iLength", nLength.ToString());
                scStatement.Parameters.AddWithValue("@iWidth", nWeight.ToString());
                scStatement.Parameters.AddWithValue("@iWeight", nWeight.ToString());
                scStatement.Parameters.AddWithValue("@iShippingInfo", szShippingInfo);

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


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _editAdmin
         * Desc:   Edits an admin in the database
         * 
         * Returns: (int) -1 if it fails, 0 if it succeeds.
         *****************************************************************************/
        protected int _editAdmin(int adID, string adUser, string adPass, string adFName, string adLName, string adLVL)
        {
            /////////////////////////////
            ////////////////////////////////////
            //////////////////////////////////////////////////////error
            ////////////////////////////////////////
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand("UPDATE admin SET username=@adUSER, password=@adPASS, fname=@adFNAME, lname=@adLNAME, lvl=@adLVL WHERE id=@adID", scConn);

            scStatement.Parameters.AddWithValue("@adUSER", adUser);
            scStatement.Parameters.AddWithValue("@adPASS", adPass);
            scStatement.Parameters.AddWithValue("@adFNAME", adFName);
            scStatement.Parameters.AddWithValue("@adLNAME", adLName);
            scStatement.Parameters.AddWithValue("@adLVL", adLVL);
            scStatement.Parameters.AddWithValue("@adID", adID.ToString());

            try
            {
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


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _editBrand
         * Desc:   Edits a Brand in the database
         * 
         * Returns: (int) -1 if it fails, 0 if it succeeds.
         *****************************************************************************/
        protected int _editBrand(int adID, string szName, string szDesc)
        {
            /////////////////////////////
            ////////////////////////////////////
            //////////////////////////////////////////////////////error
            ////////////////////////////////////////
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand("UPDATE brand SET name=@brNAME, description=@brDESC WHERE id=@brID", scConn);

            scStatement.Parameters.AddWithValue("@brNAME", szName);
            scStatement.Parameters.AddWithValue("@brDESC", szDesc);
            scStatement.Parameters.AddWithValue("@brID", adID.ToString());

            try
            {
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


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _editCategory
         * Desc:   Edits a category in the database
         * 
         * Returns: (int) -1 if it fails, 0 if it succeeds.
         *****************************************************************************/
        protected int _editCategory(int adID, string szName)
        {
            /////////////////////////////
            ////////////////////////////////////
            //////////////////////////////////////////////////////error
            ////////////////////////////////////////
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand("UPDATE category SET name=@catNAME WHERE id=@brID", scConn);

            scStatement.Parameters.AddWithValue("@catNAME", szName);
            scStatement.Parameters.AddWithValue("@brID", adID.ToString());

            try
            {
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


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _deleteAdmin
         * Desc:    deletes an admin from the database.
         * 
         * Returns: (int) -1 if it fails, 0 if it succeeds.
         *****************************************************************************/
        protected int _deleteAdmin(int adID)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand("DELETE FROM admin WHERE id=@adID", scConn);

            scStatement.Parameters.AddWithValue("@adID", adID.ToString());

            try
            {
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


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _deleteBrand
         * Desc:    deletes an brand from the database.
         * 
         * Returns: (int) -1 if it fails, 0 if it succeeds.
         *****************************************************************************/
        protected int _deleteBrand(int brandID)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand("DELETE FROM brand WHERE id=@brandID", scConn);

            scStatement.Parameters.AddWithValue("@brandID", brandID.ToString());

            try
            {
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


        /*****************************************************************************
        * Author:  Robert Milan
        * Name:    _deleteCategory
        * Desc:    deletes a category from the database.
        * 
        * Returns: (int) -1 if it fails, 0 if it succeeds.
        *****************************************************************************/
        protected int _deleteCategory(int catID)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand("DELETE FROM category WHERE id=@catID", scConn);

            scStatement.Parameters.AddWithValue("@catID", catID.ToString());

            try
            {
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


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _deleteItem
         * Desc:    deletes a item from the database.
         * 
         * Returns: (int) -1 if it fails, 0 if it succeeds.
         *****************************************************************************/
        protected int _deleteItem(int itemID)
        {
            SqlConnection scConn = new SqlConnection(szConnectionString);
            SqlCommand scStatement = new SqlCommand("DELETE FROM item WHERE id=@itemID", scConn);

            scStatement.Parameters.AddWithValue("@itemID", itemID.ToString());

            try
            {
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


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _displayAdminPage
         * Desc:    Display's the admin panel
         * 
         * Returns: None.
         *****************************************************************************/
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

                        // Current admin level of the current entry we are grabbing from the db
                        Int32.TryParse(sdr[5].ToString(), out int nADType);

                        // Get the current level of the admin whose logged in
                        Int32.TryParse(adLVL, out int nADCurr);

                        if ((nADCurr < nADType) || (nADCurr == nADType))
                        {
                            szPage += "      <td class=\"lite\"><a href=\"default.aspx?pid=admin&act=edit&id=" + sdr[0].ToString() + "\">EDIT</a></td>\n";
                        }
                        else
                        {
                            szPage += "      <td class=\"lite\"><a href=\"default.aspx?pid=admin&act=edit&id=" + sdr[0].ToString() + "\">EDIT</a> | <a href=\"default.aspx?pid=admin&act=delete&id=" + sdr[0].ToString() + "\">DELETE</a></td>\n";
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
                szPage += "<br />\n";
            }

            _content.InnerHtml = szPage;
        }


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _displayBrandPage
         * Desc:    Display's the brand panel
         * 
         * Returns: None.
         *****************************************************************************/
        protected void _displayBrandPage()
        {
            string szPage = "\n<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
            szPage += "   <tr style=\"font-weight: bold; text-decoration: underline;\">\n";
            szPage += "      <td class=\"dark\">ID</td>\n";
            szPage += "      <td class=\"dark\">Name</td>\n";
            szPage += "      <td class=\"dark\">Description</td>\n";
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

            if (nCookieExist == 1)
            {
                SqlConnection scConn = new SqlConnection(szConnectionString);
                SqlCommand scStatement = new SqlCommand("SELECT * from brand", scConn);

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

                        // Current admin level of the current entry we are grabbing from the db
                        Int32.TryParse(adLVL, out int nADType);

                        if (nADType == 0) // Moderators can edit but not delete
                        {
                            szPage += "      <td class=\"lite\"><a href=\"default.aspx?pid=brand&act=edit&id=" + sdr[0].ToString() + "\">EDIT</a></td>\n";
                        }
                        else
                        {
                            szPage += "      <td class=\"lite\"><a href=\"default.aspx?pid=brand&act=edit&id=" + sdr[0].ToString() + "\">EDIT</a> | <a href=\"default.aspx?pid=brand&act=delete&id=" + sdr[0].ToString() + "\">DELETE</a></td>\n";
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
            else
            {
                Response.Redirect("default.aspx");
            }

            szPage += "</table>\n";


            /**************************************************************
             * If the admin is authorized and a super admin, then we      *
             *    can allow them to have access to add a admin            *
             *    to the database.                                        *
             /*************************************************************/

            if (adAUTH == "1")
            {
                szPage += "<br />\n<hr />\n<br />\n";
                szPage += "\n<form id=\"frmAddBrand\" method=\"post\">\n";
                szPage += "   <input type=\"hidden\" id=\"op\" name=\"op\" value=\"brand\">\n";
                szPage += "   <input type=\"hidden\" id=\"act\" name=\"act\" value=\"add\">";
                szPage += "<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
                szPage += "   <tr style=\"font-weight: bold; text-decoration: underline;\">\n";
                szPage += "      <td class=\"dark\">\n";
                szPage += "         <h1>ADD BRAND</h1>\n";
                szPage += "         <br />\n";
                szPage += "         <table style=\"margin: auto; width: 100 %;\">\n";
                szPage += "            <tr>\n";
                szPage += "               <td style=\"text-align: right; width: 40%;\">Brand Name:&nbsp;</td>\n";
                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"brName\" name=\"brName\" type=\"text\" /></td>\n";
                szPage += "            </tr>\n";
                szPage += "            <tr>\n";
                szPage += "               <td style=\"text-align: right; width: 40%;\">Brand Description:&nbsp;</td>\n";
                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"brDesc\" name=\"brDesc\" type=\"text\" /></td>\n";
                szPage += "            </tr>\n";
                szPage += "            <tr>\n";
                szPage += "               <td style=\"text-align: right; width: 40%;\"></td>\n";
                szPage += "               <td style=\"text-align: left; width: 60%;\"><div style=\"width: 301px; text-align: right;\"><input style=\"width: 100px;\" id=\"btnAddBrand\" name=\"submit\" type=\"submit\" value=\"Add Brand\" /></div></td>\n";
                szPage += "            </tr>\n";
                szPage += "         </table>\n";
                szPage += "      </td>\n";
                szPage += "   </tr>\n";
                szPage += "</table></form>\n";
                szPage += "<br />\n";
            }

            _content.InnerHtml = szPage;
        }


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _displayCategoryPage
         * Desc:    Display's the category panel
         * 
         * Returns: None.
         *****************************************************************************/
        protected void _displayCategoryPage()
        {
            string szPage = "\n<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
            szPage += "   <tr style=\"font-weight: bold; text-decoration: underline;\">\n";
            szPage += "      <td class=\"dark\">ID</td>\n";
            szPage += "      <td class=\"dark\">Name</td>\n";
            szPage += "      <td class=\"dark\">Brand ID (Name)</td>\n";
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

            if (nCookieExist == 1)
            {
                SqlConnection scConn = new SqlConnection(szConnectionString);
                SqlCommand scStatement = new SqlCommand("SELECT * from category", scConn);

                try
                {
                    scConn.Open();

                    SqlDataReader sdr = scStatement.ExecuteReader();
                    while (sdr.Read())
                    {
                        // Get the index of the brand so we can lookup its name
                        Int32.TryParse(sdr[2].ToString(), out int nIndex);

                        szPage += "   <tr>\n";
                        szPage += "      <td class=\"lite\">" + sdr[0].ToString() + "</td>\n";
                        szPage += "      <td class=\"lite\">" + sdr[1].ToString() + "</td>\n";
                        szPage += "      <td class=\"lite\">" + sdr[2].ToString() + " (" + _getBrandField(nIndex, 1) + ")</td>\n";

                        // Current admin level of the current entry we are grabbing from the db
                        Int32.TryParse(adLVL, out int nADType);

                        if (nADType == 0) // Moderators can edit but not delete
                        {
                            szPage += "      <td class=\"lite\"><a href=\"default.aspx?pid=category&act=edit&id=" + sdr[0].ToString() + "\">EDIT</a></td>\n";
                        }
                        else
                        {
                            szPage += "      <td class=\"lite\"><a href=\"default.aspx?pid=category&act=edit&id=" + sdr[0].ToString() + "\">EDIT</a> | <a href=\"default.aspx?pid=category&act=delete&id=" + sdr[0].ToString() + "\">DELETE</a></td>\n";
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
            else
            {
                Response.Redirect("default.aspx");
            }

            szPage += "</table>\n";


            /**************************************************************
             * If the admin is authorized and a super admin, then we      *
             *    can allow them to have access to add a admin            *
             *    to the database.                                        *
             /*************************************************************/

            if (adAUTH == "1")
            {
                szPage += "<br />\n<hr />\n<br />\n";
                szPage += "\n<form id=\"frmAddCategory\" method=\"post\">\n";
                szPage += "   <input type=\"hidden\" id=\"op\" name=\"op\" value=\"category\">\n";
                szPage += "   <input type=\"hidden\" id=\"act\" name=\"act\" value=\"add\">";
                szPage += "<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
                szPage += "   <tr style=\"font-weight: bold; text-decoration: underline;\">\n";
                szPage += "      <td class=\"dark\">\n";
                szPage += "         <h1>ADD CATEGORY</h1>\n";
                szPage += "         <br />\n";
                szPage += "         <table style=\"margin: auto; width: 100 %;\">\n";
                szPage += "            <tr>\n";
                szPage += "               <td style=\"text-align: right; width: 40%;\">Category Name:&nbsp;</td>\n";
                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"catName\" name=\"catName\" type=\"text\" /></td>\n";
                szPage += "            </tr>\n";



                SqlConnection scConn = new SqlConnection(szConnectionString);
                SqlCommand scStatement = new SqlCommand("SELECT * from brand", scConn);

                try
                {
                    scConn.Open();

                    SqlDataReader sdr = scStatement.ExecuteReader();


                    szPage += "   <tr>\n";
                    szPage += "      <td style=\"text-align: right; width: 40%;\">Select a Brand:&nbsp;</td>\n";
                    szPage += "      <td style=\"text-align: left; width: 60%;\">\n";
                    szPage += "         <select id=\"bid\" name=\"bid\">\n";
                    while (sdr.Read())
                    {
                        szPage += "            <option value=\"" + sdr[1].ToString() + "\">" + sdr[1].ToString() + "</option>\n";
                    }
                    
                    szPage += "      </td>\n   </tr>\n";
                }
                catch (Exception m)
                {
                    szLastErrorMessage = m.Message;
                }
                finally
                {
                    scConn.Close();
                }


                szPage += "         <tr>\n";
                szPage += "            <td style=\"text-align: right; width: 40%;\"></td>\n";
                szPage += "            <td style=\"text-align: left; width: 60%;\"><div style=\"width: 301px; text-align: right;\"><input style=\"width: 100px;\" id=\"btnAddBrand\" name=\"submit\" type=\"submit\" value=\"Add Category\" /></div></td>\n";
                szPage += "         </tr>\n";
                szPage += "      </table>\n";
                szPage += "      </td>\n";
                szPage += "   </tr>\n";
                szPage += "</table></form>\n";
                szPage += "<br />\n";
            }

            _content.InnerHtml = szPage;
        }


        /*****************************************************************************
         * Author:  Robert Milan
         * Name:    _displayItemPage
         * Desc:    Display's the item panel
         * 
         * Returns: None.
         *****************************************************************************/
        protected void _displayItemPage()
        {
            ///
        }


        /*****************************************************************************
         * All content is setup through the page load.
         *****************************************************************************/
        protected void Page_Load(object sender, EventArgs e)
        {
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
             *****************************************************************************/
            if (!this.IsPostBack)
            {
                /*  Check for admin authentication before displaying the admin page  */
                if (nIsAdmin == 1)
                {
                    // Determine what page we are looking at
                    if (Request.QueryString.Count != 0)
                    {
                        // Ensure we have a page identifier
                        if (Request.QueryString["pid"] != null)
                        {
                            // Handle the admin panel
                            if (Request.QueryString["pid"].ToLower() == "admin")
                            {
                                if (Request.QueryString["act"] != null)
                                {
                                    string szACT = Request.QueryString["act"].ToString().ToLower();
                                    if (szACT == "edit")
                                    {
                                        if (Request.QueryString["id"] != null)
                                        {
                                            if (int.TryParse(Request.QueryString["id"].ToString(), out int nIndex))
                                            {
                                                string szUSER = _getAdminField(nIndex, 1);
                                                //string szPASS = _getAdminField(nIndex, 2);
                                                string szFNAME = _getAdminField(nIndex, 3);
                                                string szLNAME = _getAdminField(nIndex, 4);
                                                string szLVL = _getAdminField(nIndex, 5);

                                                string szPage = "";
                                                szPage += "\n<form id=\"frmEdit\" method=\"post\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"op\" name=\"op\" value=\"admin\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"act\" name=\"act\" value=\"edit\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"id\" name=\"id\" value=\"" + nIndex.ToString() + "\">\n";
                                                szPage += "<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
                                                szPage += "   <tr style=\"font-weight: bold; text-decoration: underline;\">\n";
                                                szPage += "      <td class=\"dark\">\n";
                                                szPage += "         <h1>EDIT ADMIN</h1>\n";
                                                szPage += "         <br />\n";
                                                szPage += "         <table style=\"margin: auto; width: 100 %;\">\n";
                                                szPage += "            <tr>\n";
                                                szPage += "               <td style=\"text-align: right; width: 40%;\">Username:&nbsp;</td>\n";
                                                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"adUSER\" name=\"adUSER\" type=\"text\" value=\"" + szUSER + "\" /></td>\n";
                                                szPage += "            </tr>\n";
                                                szPage += "            <tr>\n";
                                                szPage += "               <td style=\"text-align: right; width: 40%;\">Password (No change in left blank):&nbsp;</td>\n";
                                                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"adPASS\" name=\"adPASS\" type=\"text\" value=\"\" /></td>\n";
                                                szPage += "            </tr>\n";
                                                szPage += "            <tr>\n";
                                                szPage += "               <td style=\"text-align: right; width: 40%;\">First Name:&nbsp;</td>\n";
                                                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"adFNAME\" name=\"adFNAME\" type=\"text\" value=\"" + szFNAME + "\" /></td>\n";
                                                szPage += "            </tr>\n";
                                                szPage += "            <tr>\n";
                                                szPage += "               <td style=\"text-align: right; width: 40%;\">Last Name:&nbsp;</td>\n";
                                                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"adLNAME\" name=\"adLNAME\" type=\"text\" value=\"" + szLNAME + "\" /></td>\n";
                                                szPage += "            </tr>\n";
                                                szPage += "            <tr>\n";
                                                szPage += "               <td style=\"text-align: right; width: 40%;\">[0, 1, or 2] Level:&nbsp;</td>\n";
                                                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"adLVL\" name=\"adLVL\" type=\"text\" value=\"" + szLVL + "\" /></td>\n";
                                                szPage += "            </tr>\n";
                                                szPage += "            <tr>\n";
                                                szPage += "               <td style=\"text-align: right; width: 40%;\">All changes are final once you click 'Edit Admin'!</td>\n";
                                                szPage += "               <td style=\"text-align: left; width: 60%;\"><div style=\"width: 301px; text-align: right;\"><input style=\"width: 100px;\" id=\"btnEditAdmin\" name=\"submit\" type=\"submit\" value=\"Edit Admin\" /></div></td>\n";
                                                szPage += "            </tr>\n";
                                                szPage += "         </table>\n";
                                                szPage += "      </td>\n";
                                                szPage += "   </tr>\n";
                                                szPage += "</table></form>\n";

                                                _content.InnerHtml = szPage;
                                            }
                                            else
                                            {
                                                Response.Redirect("default.aspx");
                                            }
                                        }
                                        else
                                        {
                                            Response.Redirect("default.aspx");
                                        }
                                    }


                                    else if (szACT == "delete")
                                    {
                                        if (Request.QueryString["id"] != null)
                                        {
                                            if (int.TryParse(Request.QueryString["id"].ToString(), out int nIndex))
                                            {
                                                string szPage = "";
                                                szPage += "\n<form id=\"frmDelete\" method=\"post\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"op\" name=\"op\" value=\"admin\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"act\" name=\"act\" value=\"delete\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"id\" name=\"id\" value=\"" + nIndex.ToString() + "\">\n";
                                                szPage += "<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
                                                szPage += "   <tr style=\"font-weight: bold; text-decoration: none;\">\n";
                                                szPage += "      <td class=\"dark\">\n";
                                                szPage += "         <h1>DELETE ADMIN</h1>\n";
                                                szPage += "         <br />\n";
                                                szPage += "         <table style=\"margin: auto; width: 100 %;\">\n";
                                                szPage += "            <tr>\n";
                                                szPage += "               <td style=\"text-align: left; width: 60%;\">Are you sure you want to delete '" + _getAdminField(nIndex, 1) + "'?:&nbsp;&nbsp;<input style=\"width: 100px;\" id=\"btnDeleteAdmin\" name=\"submit\" type=\"submit\" value=\"Delete\" /></td>\n";
                                                szPage += "            </tr>\n";
                                                szPage += "         </table>\n";
                                                szPage += "      </td>\n";
                                                szPage += "   </tr>\n";
                                                szPage += "</table></form>\n";

                                                _content.InnerHtml = szPage;
                                            }
                                            else
                                            {
                                                Response.Redirect("default.aspx");
                                            }
                                        }
                                        else
                                        {
                                            Response.Redirect("default.aspx");
                                        }
                                    }
                                }
                                else
                                {
                                    // No identifier, display default page.
                                    _displayAdminPage();
                                }
                            }
                            /*****************************************************************************
                             *****************************************************************************
                             *****************************************************************************/
                            else if (Request.QueryString["pid"].ToLower() == "brand")
                            {
                                if (Request.QueryString["act"] != null)
                                {
                                    string szACT = Request.QueryString["act"].ToString().ToLower();
                                    if (szACT == "edit")
                                    {
                                        if (Request.QueryString["id"] != null)
                                        {
                                            if (int.TryParse(Request.QueryString["id"].ToString(), out int nIndex))
                                            {
                                                string szName = _getBrandField(nIndex, 1);
                                                string szDesc = _getBrandField(nIndex, 2);

                                                string szPage = "";
                                                szPage += "\n<form id=\"frmEdit\" method=\"post\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"op\" name=\"op\" value=\"brand\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"act\" name=\"act\" value=\"edit\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"id\" name=\"id\" value=\"" + nIndex.ToString() + "\">\n";
                                                szPage += "<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
                                                szPage += "   <tr style=\"font-weight: bold; text-decoration: underline;\">\n";
                                                szPage += "      <td class=\"dark\">\n";
                                                szPage += "         <h1>EDIT BRAND</h1>\n";
                                                szPage += "         <br />\n";
                                                szPage += "         <table style=\"margin: auto; width: 100 %;\">\n";
                                                szPage += "            <tr>\n";
                                                szPage += "               <td style=\"text-align: right; width: 40%;\">Brand Name:&nbsp;</td>\n";
                                                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"brName\" name=\"brName\" type=\"text\" value=\"" + szName + "\" /></td>\n";
                                                szPage += "            </tr>\n";
                                                szPage += "            <tr>\n";
                                                szPage += "               <td style=\"text-align: right; width: 40%;\">Brand Description:&nbsp;</td>\n";
                                                szPage += "               <td style=\"text-align: left; width: 60%;\"><input style=\"width: 300px;\" id=\"brDesc\" name=\"brDesc\" type=\"text\" value=\"" + szDesc + "\" /></td>\n";
                                                szPage += "            </tr>\n";
                                                szPage += "            <tr>\n";
                                                szPage += "               <td style=\"text-align: right; width: 40%;\">All changes are final once you click 'Edit Brand'!</td>\n";
                                                szPage += "               <td style=\"text-align: left; width: 60%;\"><div style=\"width: 301px; text-align: right;\"><input style=\"width: 100px;\" id=\"btnEditBrand\" name=\"submit\" type=\"submit\" value=\"Edit Brand\" /></div></td>\n";
                                                szPage += "            </tr>\n";
                                                szPage += "         </table>\n";
                                                szPage += "      </td>\n";
                                                szPage += "   </tr>\n";
                                                szPage += "</table></form>\n";

                                                _content.InnerHtml = szPage;
                                            }
                                            else
                                            {
                                                Response.Redirect("default.aspx?pid=brand");
                                            }
                                        }
                                        else
                                        {
                                            Response.Redirect("default.aspx?pid=brand");
                                        }
                                    }


                                    else if (szACT == "delete")
                                    {
                                        if (Request.QueryString["id"] != null)
                                        {
                                            if (int.TryParse(Request.QueryString["id"].ToString(), out int nIndex))
                                            {
                                                string szPage = "";
                                                szPage += "\n<form id=\"frmDelete\" method=\"post\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"op\" name=\"op\" value=\"brand\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"act\" name=\"act\" value=\"delete\">\n";
                                                szPage += "   <input type=\"hidden\" id=\"id\" name=\"id\" value=\"" + nIndex.ToString() + "\">\n";
                                                szPage += "<table style=\"text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;\">\n";
                                                szPage += "   <tr style=\"font-weight: bold; text-decoration: none;\">\n";
                                                szPage += "      <td class=\"dark\">\n";
                                                szPage += "         <h1>DELETE BRAND</h1>\n";
                                                szPage += "         <br />\n";
                                                szPage += "         <table style=\"margin: auto; width: 100 %;\">\n";
                                                szPage += "            <tr>\n";
                                                szPage += "               <td style=\"text-align: left; width: 60%;\">Are you sure you want to delete '" + _getBrandField(nIndex, 1) + "'?:&nbsp;&nbsp;<input style=\"width: 100px;\" id=\"btnDeleteBrand\" name=\"submit\" type=\"submit\" value=\"Delete\" /></td>\n";
                                                szPage += "            </tr>\n";
                                                szPage += "         </table>\n";
                                                szPage += "      </td>\n";
                                                szPage += "   </tr>\n";
                                                szPage += "</table></form>\n";

                                                _content.InnerHtml = szPage;
                                            }
                                            else
                                            {
                                                Response.Redirect("default.aspx?pid=brand");
                                            }
                                        }
                                        else
                                        {
                                            Response.Redirect("default.aspx?pid=brand");
                                        }
                                    }
                                }
                                else
                                {
                                    // No identifier, display default page.
                                    _displayBrandPage();
                                }
                            }
                            /*****************************************************************************
                             *****************************************************************************
                             *****************************************************************************/
                            else if (Request.QueryString["pid"].ToLower() == "category")
                            {
                                _displayCategoryPage();
                            }
                            /*****************************************************************************
                             *****************************************************************************
                             *****************************************************************************/
                            else if (Request.QueryString["pid"].ToLower() == "item")
                            {
                                _content.InnerHtml = "<div style=\"text-align: center;\">Display Item</div>";
                            }
                            /*****************************************************************************
                             *****************************************************************************
                             *****************************************************************************/
                            else if (Request.QueryString["pid"].ToLower() == "logout")
                            {
                                HttpCookie hcAdmin = new HttpCookie("nimda");
                                hcAdmin.Expires = DateTime.Now.AddDays(-1d);

                                Page.Response.Cookies.Add(hcAdmin);
                                Response.Redirect("default.aspx");
                            }
                            /*****************************************************************************
                             *****************************************************************************
                             *****************************************************************************/
                            else
                            {
                                _displayAdminPage();
                            }
                        }
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

                /*************************************************
                /* Handle admin operations                       *
                 *************************************************/
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
                                Response.Redirect("default.aspx");
                            }

                            if (nErrFound == 0)
                            {
                                string szUser = Request.Form["adUSER"];
                                string szPass = Request.Form["adPASS"];
                                string szConf = Request.Form["adCONF"];
                                string szFName = Request.Form["adFNAME"];
                                string szLName = Request.Form["adLNAME"];
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
                                    // Add the admin, if a error occurs adding the admin then we let the end-user know an error has occurred.
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
                            if (Request.Form["id"] != null)
                            {
                                nErrFound = 0;

                                // Make sure all values passed from the form
                                if (Request.Form["adUSER"] == null) { nErrFound = 1; }
                                if (Request.Form["adPASS"] == null) { nErrFound = 1; }
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
                                    string szPass = "";
                                    if (Request.Form["adPass"] != null) { szPass = Request.Form["adPass"]; }

                                    string szUser = Request.Form["adUser"];
                                    string szLName = Request.Form["adFName"];
                                    string szFName = Request.Form["adLName"];
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

                                    //////////////////////////
                                    if (nErrFound == 1)
                                    {
                                        _content.InnerHtml = szPage + "<hr style=\"width: 300px;\"/><a href=\"default.aspx?pid=admin&act=edit&id=" + Request.QueryString["id"] + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                    }


                                    if (int.TryParse(Request.Form["id"].ToString(), out int nIndex))
                                    {
                                        // Grab the password and check if it's blank or had data
                                        string szPWD = szPass;
                                        if (szPWD.Length > 0) // It has data so it's a new password that needs encrypted
                                        {
                                            szPWD = _encryptPWD(szPWD);
                                        }
                                        else
                                        {
                                            // It doesn't have data so grab the old password to re-enter into the database
                                            szPWD = _getAdminField(nIndex, 2);
                                        }

                                        // edit the admin table
                                        if (_editAdmin(nIndex, Request.Form["adUser"].ToString(), szPWD, Request.Form["adFName"].ToString(), Request.Form["adLName"].ToString(), Request.Form["adLVL"].ToString()) == 0)
                                        {
                                            Response.Redirect("default.aspx");
                                        }
                                        else
                                        {
                                            _content.InnerHtml = "<div style=\"text-align: center;\">Error: " + szLastErrorMessage + "<a href=\"default.aspx?pid=admin&act=edit&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                        }
                                    }
                                    else
                                    {
                                        _content.InnerHtml = "<div style=\"text-align: center;\">An unknown error has occurred!<a href=\"default.aspx?pid=admin&act=edit&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                    }

                                }
                                else
                                {
                                    Response.Redirect("default.aspx");
                                }
                            }
                        }

                        // Remove a admin
                        else if (szAct == "delete")
                        {
                            if (Request.Form["id"] != null)
                            {
                                if (int.TryParse(Request.Form["id"].ToString(), out int nIndex))
                                {
                                    // delete admin from the table
                                    if (_deleteAdmin(nIndex) == 0)
                                    {
                                        Response.Redirect("default.aspx");
                                    }
                                    else
                                    {
                                        _content.InnerHtml = "<div style=\"text-align: center;\">Error: " + szLastErrorMessage + "<a href=\"default.aspx?pid=admin&act=delete&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                    }
                                }
                                else
                                {
                                    _content.InnerHtml = "<div style=\"text-align: center;\">An unknown error has occurred!<a href=\"default.aspx?pid=admin&act=delete&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                }
                            }
                            else
                            {
                                Response.Redirect("default.aspx");
                            }
                        }
                        else
                        {
                            Response.Redirect("default.aspx");
                        }
                    }
                }

                if (szOP == "brand")
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
                            if (Request.Form["brName"] == null) { nErrFound = 1; }
                            if (Request.Form["brDesc"] == null) { nErrFound = 1; }

                            if (nErrFound == 1)
                            {
                                Response.Redirect("default.aspx?pid=brand");
                            }

                            if (nErrFound == 0)
                            {
                                string szName = Request.Form["brName"];
                                string szDesc = Request.Form["brDesc"];

                                string szPage = "<div style=\"text-align: center;\">[ ERROR ]<hr style=\"width: 300px;\"/>\n";
                                if (szName.Length == 0)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">You need to enter a &#34;brand name!&#34;</div><br />\n";
                                }


                                if (szDesc.Length == 0)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">You need to enter a &#34;brand description!&#34;</div><br />\n";
                                }

                                if (_getBrandIndex(szName) != -1)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">The &#34;brand name&#34; you entered already exists!</div><br />\n";
                                }
                            

                                //////////////////////////
                                if (nErrFound == 1)
                                {
                                    _content.InnerHtml = szPage + "<hr style=\"width: 300px;\"/><a href=\"default.aspx?pid=brand\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                }
                                else
                                {
                                    // Add the admin, if a error occurs adding the admin then we let the end-user know an error has occurred.
                                    if (_addBrand(szName, szDesc) != -1)
                                    {
                                        Response.Redirect("default.aspx?pid=brand");
                                    }
                                    else
                                    {
                                        _content.InnerHtml = "<div style=\"text-align: center;\">[ ERROR: " + szLastErrorMessage + " ]<hr style=\"width: 300px;\"/>\n<div style=\"color: #FF0000\">Error creating the brand name &#34;" + szName + ".&#34;<br /><a href=\"default.aspx?pid=brand\" style=\"text-decoration: underline;\">Click here</a> to go back.</div><br />\n";
                                    }
                                }
                            }
                            else
                            {
                                _content.InnerHtml = "<div style=\"text-align: center;\">An unknown error has occurred!<a href=\"default.aspx?pid=brand\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                            }
                        }


                        // Edit a admin
                        else if (szAct == "edit")
                        {
                            if (Request.Form["id"] != null)
                            {
                                nErrFound = 0;

                                // Make sure all values passed from the form
                                if (Request.Form["brName"] == null) { nErrFound = 1; }
                                if (Request.Form["brDesc"] == null) { nErrFound = 1; }

                                if (nErrFound == 1)
                                {
                                    _content.InnerHtml = Request.Form.ToString();
                                    return;
                                }

                                if (nErrFound == 0)
                                {
                                    string szName = Request.Form["brName"];
                                    string szDesc = Request.Form["brDesc"];


                                    string szPage = "<div style=\"text-align: center;\">[ ERROR ]<hr style=\"width: 300px;\"/>\n";
                                    if (szName.Length == 0)
                                    {
                                        nErrFound = 1;
                                        szPage += "<div style=\"color: #FF0000\">You need to enter a &#34;brand name!&#34;</div><br />\n";
                                    }

                                    //////////////////////////
                                    if (nErrFound == 1)
                                    {
                                        _content.InnerHtml = szPage + "<hr style=\"width: 300px;\"/><a href=\"default.aspx?pid=brand&act=edit&id=" + Request.QueryString["id"] + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                    }


                                    if (int.TryParse(Request.Form["id"].ToString(), out int nIndex))
                                    {

                                        // edit the brand table
                                        if (_editBrand(nIndex, szName, szDesc) == 0)
                                        {
                                            Response.Redirect("default.aspx?pid=brand");
                                        }
                                        else
                                        {
                                            _content.InnerHtml = "<div style=\"text-align: center;\">Error: " + szLastErrorMessage + "<a href=\"default.aspx?pid=brand&act=edit&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                        }
                                    }
                                    else
                                    {
                                        _content.InnerHtml = "<div style=\"text-align: center;\">An unknown error has occurred!<a href=\"default.aspx?pid=brand&act=edit&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                    }

                                }
                                else
                                {
                                    Response.Redirect("default.aspx?pid=brand");
                                }
                            }
                        }


                        // Remove a admin
                        else if (szAct == "delete")
                        {
                            if (Request.Form["id"] != null)
                            {
                                if (int.TryParse(Request.Form["id"].ToString(), out int nIndex))
                                {
                                    // delete admin from the table
                                    if (_deleteBrand(nIndex) == 0)
                                    {
                                        Response.Redirect("default.aspx?pid=brand");
                                    }
                                    else
                                    {
                                        _content.InnerHtml = "<div style=\"text-align: center;\">Error: " + szLastErrorMessage + "<a href=\"default.aspx?pid=brand&act=delete&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                    }
                                }
                                else
                                {
                                    _content.InnerHtml = "<div style=\"text-align: center;\">An unknown error has occurred!<a href=\"default.aspx?pid=brand&act=delete&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                }
                            }
                            else
                            {
                                Response.Redirect("default.aspx?pid=brand");
                            }
                        }
                        else
                        {
                            Response.Redirect("default.aspx?pid=brand");
                        }
                    }
                }

                if (szOP == "category")
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
                            if (Request.Form["catName"] == null) { nErrFound = 1; }

                            if (nErrFound == 1)
                            {
                                Response.Redirect("default.aspx?pid=category");
                            }

                            if (nErrFound == 0)
                            {
                                string szName = Request.Form["catName"];

                                string szPage = "<div style=\"text-align: center;\">[ ERROR ]<hr style=\"width: 300px;\"/>\n";
                                if (szName.Length == 0)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">You need to enter a &#34;category name!&#34;</div><br />\n";
                                }

                                /*
                                if (_getCategoryIndex(szName) != -1)
                                {
                                    nErrFound = 1;
                                    szPage += "<div style=\"color: #FF0000\">The &#34;brand name&#34; you entered already exists!</div><br />\n";
                                }


                                //////////////////////////
                                if (nErrFound == 1)
                                {
                                    _content.InnerHtml = szPage + "<hr style=\"width: 300px;\"/><a href=\"default.aspx?pid=brand\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                }
                                else
                                {
                                    // Add the admin, if a error occurs adding the admin then we let the end-user know an error has occurred.
                                    if (_addBrand(szName, szDesc) != -1)
                                    {
                                        Response.Redirect("default.aspx?pid=brand");
                                    }
                                    else
                                    {
                                        _content.InnerHtml = "<div style=\"text-align: center;\">[ ERROR: " + szLastErrorMessage + " ]<hr style=\"width: 300px;\"/>\n<div style=\"color: #FF0000\">Error creating the brand name &#34;" + szName + ".&#34;<br /><a href=\"default.aspx?pid=brand\" style=\"text-decoration: underline;\">Click here</a> to go back.</div><br />\n";
                                    }
                                }*/
                            }
                            else
                            {
                                _content.InnerHtml = "<div style=\"text-align: center;\">An unknown error has occurred!<a href=\"default.aspx?pid=brand\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                            }
                        }


                        // Edit a admin
                        else if (szAct == "edit")
                        {
                            if (Request.Form["id"] != null)
                            {
                                nErrFound = 0;

                                // Make sure all values passed from the form
                                if (Request.Form["brName"] == null) { nErrFound = 1; }
                                if (Request.Form["brDesc"] == null) { nErrFound = 1; }

                                if (nErrFound == 1)
                                {
                                    _content.InnerHtml = Request.Form.ToString();
                                    return;
                                }

                                if (nErrFound == 0)
                                {
                                    string szName = Request.Form["brName"];
                                    string szDesc = Request.Form["brDesc"];


                                    string szPage = "<div style=\"text-align: center;\">[ ERROR ]<hr style=\"width: 300px;\"/>\n";
                                    if (szName.Length == 0)
                                    {
                                        nErrFound = 1;
                                        szPage += "<div style=\"color: #FF0000\">You need to enter a &#34;brand name!&#34;</div><br />\n";
                                    }

                                    //////////////////////////
                                    if (nErrFound == 1)
                                    {
                                        _content.InnerHtml = szPage + "<hr style=\"width: 300px;\"/><a href=\"default.aspx?pid=brand&act=edit&id=" + Request.QueryString["id"] + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                    }


                                    if (int.TryParse(Request.Form["id"].ToString(), out int nIndex))
                                    {

                                        // edit the brand table
                                        if (_editBrand(nIndex, szName, szDesc) == 0)
                                        {
                                            Response.Redirect("default.aspx?pid=brand");
                                        }
                                        else
                                        {
                                            _content.InnerHtml = "<div style=\"text-align: center;\">Error: " + szLastErrorMessage + "<a href=\"default.aspx?pid=brand&act=edit&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                        }
                                    }
                                    else
                                    {
                                        _content.InnerHtml = "<div style=\"text-align: center;\">An unknown error has occurred!<a href=\"default.aspx?pid=brand&act=edit&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                    }

                                }
                                else
                                {
                                    Response.Redirect("default.aspx?pid=brand");
                                }
                            }
                        }


                        // Remove a admin
                        else if (szAct == "delete")
                        {
                            if (Request.Form["id"] != null)
                            {
                                if (int.TryParse(Request.Form["id"].ToString(), out int nIndex))
                                {
                                    // delete admin from the table
                                    if (_deleteBrand(nIndex) == 0)
                                    {
                                        Response.Redirect("default.aspx?pid=brand");
                                    }
                                    else
                                    {
                                        _content.InnerHtml = "<div style=\"text-align: center;\">Error: " + szLastErrorMessage + "<a href=\"default.aspx?pid=brand&act=delete&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                    }
                                }
                                else
                                {
                                    _content.InnerHtml = "<div style=\"text-align: center;\">An unknown error has occurred!<a href=\"default.aspx?pid=brand&act=delete&id=" + nIndex.ToString() + "\" style=\"text-decoration: underline;\">Click here</a> to go back.</div>";
                                }
                            }
                            else
                            {
                                Response.Redirect("default.aspx?pid=brand");
                            }
                        }
                        else
                        {
                            Response.Redirect("default.aspx?pid=brand");
                        }
                    }
                }

                if (szOP == "item")
                {
                    _content.InnerHtml = "ITEM!";
                }
            }

            // Specify logout link, if an admin is logged in
            if (nIsAdmin == 1)
            {
                _content.InnerHtml += "<hr><div class=\"bevel\" style=\"text-align: center;\">[<a href=\"default.aspx?pid=logout\" style=\"text-decoration: underline;\">Logout</a>]</div>";
            }
        }
    }
}