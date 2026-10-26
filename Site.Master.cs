
using System;
using System.Web.UI;

namespace ExpenseTrackerFrontend
{
    public partial class SiteMaster : MasterPage
    {
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/User/Register.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
