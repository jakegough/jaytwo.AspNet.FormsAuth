using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jaytwo.AspNet.FormsAuth
{
	public class SimpleUserProfile : IUserProfile
	{
		public static SimpleUserProfile Current
		{
			get
			{
				return FormsAuthenticationAppHost.GetSignedInUserProfileAs<SimpleUserProfile>();
			}
		}

		public string UserName { get; set; }

		public SimpleUserProfile()
		{
		}

		public SimpleUserProfile(string userName)
		{
			UserName = userName;
		}
	}
}
