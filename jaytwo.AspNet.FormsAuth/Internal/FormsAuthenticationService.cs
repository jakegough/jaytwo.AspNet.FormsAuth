using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace jaytwo.AspNet.FormsAuth.Internal
{
	internal class FormsAuthenticationService
	{
		public virtual string CookieName
		{
			get 
			{                 
				return FormsAuthentication.FormsCookieName; 
			}
		}

		public virtual string EncryptFormsAuthenticationTicket(FormsAuthenticationTicket ticket)
		{
			return FormsAuthentication.Encrypt(ticket);
		}

		public virtual FormsAuthenticationTicket DecryptFormsAuthenticationTicket(string encryptedTicket)
		{
			return FormsAuthentication.Decrypt(encryptedTicket);
		}

		public virtual HttpContextBase CurrentHttpContext
		{
			get 
			{ 
				return new HttpContextWrapper(HttpContext.Current); 
			}
		}

		public virtual IUserProfile GetCurrentUserProfile()
		{
			return GetCurrentUserProfile<IUserProfile>();
		}

		public virtual T GetCurrentUserProfile<T>() where T : IUserProfile
		{
            var ticket = TryGetFormsAuthenticationTicket();

			if (ticket != null)
			{
				return FormsAuthenticationServiceHelpers.GetUserProfile<T>(ticket.Name, ticket.UserData);
			}
			else
			{
				return default(T);
			}
		}

		public virtual string[] GetCurrentUserRoles()
		{
            string[] result = null;

            var ticket = TryGetFormsAuthenticationTicket();

			if (ticket != null)
			{
				result = FormsAuthenticationServiceHelpers.GetUserRoles(ticket.UserData);
			}

            if (result == null)
            {
                result = new string[] { };
            }

            return result;
			
		}

		public virtual void SignIn(IUserProfile profile, string[] roles)
		{
			if (profile == null)
			{
				throw new ArgumentNullException("profile");
			}

			var userDataJson = FormsAuthenticationServiceHelpers.GetUserDataJson(profile, roles);

			var ticket = new FormsAuthenticationTicket(
				version: 0,
				name: profile.UserName,
				issueDate: DateTime.Now,
				expiration: DateTime.Now.AddYears(1),
				isPersistent: false,
				userData: userDataJson);

			SetFormsAuthenticationTicket(ticket);
		}

		public virtual void SignOut()
		{
			ClearFormsAuthenticationTicket();

			if (CurrentHttpContext.Session != null)
			{
				CurrentHttpContext.Session.Abandon();
			}
		}

		public virtual void ClearFormsAuthenticationTicket()
		{
			var cookie = (CurrentHttpContext.Request.Cookies.AllKeys.Contains(CookieName))
				? CurrentHttpContext.Request.Cookies[CookieName]
				: new HttpCookie(CookieName);

			cookie.Value = null;
			cookie.Expires = DateTime.Now.AddYears(-100);

			CurrentHttpContext.Response.SetCookie(cookie);
		}

        private FormsAuthenticationTicket TryGetFormsAuthenticationTicket()
        {
            try
            {
                return GetFormsAuthenticationTicket();
            }
            catch { }

            return null;
        }

		public virtual FormsAuthenticationTicket GetFormsAuthenticationTicket()
		{
			FormsAuthenticationTicket result = null;

			if (CurrentHttpContext.Request.Cookies.AllKeys.Contains(CookieName))
			{
				var encryptedTicket = CurrentHttpContext.Request.Cookies[CookieName].Value;
				result = DecryptFormsAuthenticationTicket(encryptedTicket);
			}

			return result;
		}

		public virtual void SetFormsAuthenticationTicket(FormsAuthenticationTicket ticket)
		{
			var encryptedTicket = EncryptFormsAuthenticationTicket(ticket);

			if (CurrentHttpContext.Request.Cookies.AllKeys.Contains(CookieName))
			{
				CurrentHttpContext.Request.Cookies.Remove(CookieName);
			}

			CurrentHttpContext.Request.Cookies.Add(new HttpCookie(CookieName, encryptedTicket));

			if (CurrentHttpContext.Response.Cookies.AllKeys.Contains(CookieName))
			{
				CurrentHttpContext.Response.Cookies.Remove(CookieName);
			}

			CurrentHttpContext.Response.Cookies.Add(new HttpCookie(CookieName, encryptedTicket));
		}

	}
}
