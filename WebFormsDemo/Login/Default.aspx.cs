using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace jaytwo.AspNet.FormsAuth.WebFormsDemo.Login
{
	public partial class Default : System.Web.UI.Page
	{
		protected void btnGo_Click(object sender, EventArgs e)
		{
			var userProfile = new SimpleUserProfile(txtUserName.Text);
			var roles = new[] { "user" };
			FormsAuthenticationAppHost.SignIn(userProfile, roles);
			
			var redirectUrl = Request.QueryString["ReturnUrl"];

			if (string.IsNullOrEmpty(redirectUrl))
			{
				redirectUrl = "~/";
			}

			Response.Redirect(redirectUrl);
		}
	}
}