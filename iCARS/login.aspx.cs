using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;        // SQL Connection
using System.Security.Cryptography; // MD5, SHA256
using System.Text;                  // For UTF8Encoding

namespace iCARS
{
    public partial class login : System.Web.UI.Page
    {
        protected const string szConnectionString = "Data Source=ROBERT\\SQLEXPRESS;Initial Catalog=myDB;Integrated Security=True";
        protected static string szLastErrorMessage = "";

        protected void _writeCookie(string adUser, int nValue)
        {
            /* Create a cookie that lasts for 15 minutes */
            HttpCookie hcUser = new HttpCookie("user");
            hcUser["username"] = adUser;
            hcUser["auth"] = nValue.ToString();
            hcUser.Expires = DateTime.Now.AddMinutes(60);

            /* Save cookie */
            Page.Response.Cookies.Add(hcUser);
        }

        protected string _cryptMD5(string szText)
        {
            // Calculate MD5
            byte[] btPWDEncoding = new UTF8Encoding().GetBytes(szText);
            byte[] btMD5Hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(btPWDEncoding);

            // Convert MD5 to all uppercase and return it
            return BitConverter.ToString(btMD5Hash).Replace("-", String.Empty).ToUpper();
        }
        
        protected string _cryptSHA256(string szText)
        {
            byte[] btPWDEncoding = Encoding.UTF8.GetBytes(szText);

            HashAlgorithm haSHA256 = new SHA256Managed();
            haSHA256.TransformFinalBlock(btPWDEncoding, 0, btPWDEncoding.Length);

            return Convert.ToBase64String(haSHA256.Hash);
        }

        protected string _encryptPWD(string szText)
        {
            return _cryptMD5(_cryptSHA256(szText));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //////
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            // account creation
            
        }
    }
}