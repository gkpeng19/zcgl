using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GIS.Portal
{
    public partial class Thematic : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["USERNAME"] == null || Session["USERNAME"].ToString() == "")
                {
                    Response.Redirect("../Login.aspx");
                }
                else
                {
                }
            }
        }
    }
}