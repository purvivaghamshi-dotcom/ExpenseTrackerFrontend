
using System;
using System.Web.UI;

namespace ExpenseTrackerFrontend.Admin
{
    public partial class AdminMaster : MasterPage
    {
        protected void btnAdminLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect(
                "~/Admin/AdminLogin.aspx", false);

            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
