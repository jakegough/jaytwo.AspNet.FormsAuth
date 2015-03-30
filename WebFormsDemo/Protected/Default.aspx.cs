using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace jaytwo.AspNet.FormsAuth.WebFormsDemo.Protected
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btnSignOut_Click(object sender, EventArgs e)
		{
			FormsAuthenticationAppHost.SignOut();
			Response.Redirect("~/");
		}
	}
}