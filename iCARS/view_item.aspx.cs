using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iCARS
{
    public partial class view_item : System.Web.UI.Page
    {
        // sets content, by matt yauch
        protected void _sc(string s)
        {
            _content.InnerHtml = s;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((Request.QueryString["id"] == null) || (Request.QueryString["id"].Length == 0))
                Response.Redirect("default.aspx");

            try
            {
                _sc("String: " + Request.QueryString["id"].ToString());
            }
            catch (Exception m)
            {
                // do nothing
            }
            
        }
    }
}