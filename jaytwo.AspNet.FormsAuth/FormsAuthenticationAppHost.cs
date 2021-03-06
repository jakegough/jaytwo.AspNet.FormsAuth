﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Collections.Specialized;
using System.Web.Security;
using System.Web.Routing;
using jaytwo.AspNet.FormsAuth.Internal;

namespace jaytwo.AspNet.FormsAuth
{
	public static class FormsAuthenticationAppHost
	{
		public delegate void SignedInHandler(IUserProfile profile, string[] roles);
		public static event SignedInHandler SignedIn;

		private static void RaiseSignedIn(IUserProfile profile, string[] roles)
		{
			if (SignedIn != null)
			{
				SignedIn(profile, roles);
			}
		}

		public delegate void SignedOutHandler(IUserProfile profile);
		public static event SignedOutHandler SignedOut;
		private static void RaiseSignedOut(IUserProfile profile)
		{
			if (SignedOut != null)
			{
				SignedOut(profile);
			}
		}

		private static FormsAuthenticationService _formsAuthentication;
		private static FormsAuthenticationService _FormsAuthentication
		{
			get
			{
				if (_formsAuthentication == null)
				{
					InitializeDefault();
				}

				return _formsAuthentication;
			}
		}

		internal static void Initialize(FormsAuthenticationService formsAuthentication)
		{
			_formsAuthentication = formsAuthentication;
		}

		internal static void InitializeDefault()
		{
			Initialize(new FormsAuthenticationService());
		}

		public static void SignIn(string userName)
		{
			SignIn(new SimpleUserProfile(userName), null);
		}

		public static void SignIn(string userName, string[] roles)
		{
			SignIn(new SimpleUserProfile(userName), roles);
		}

		public static void SignIn(IUserProfile profile)
		{
			SignIn(profile, null);
		}

		public static void SignIn(IUserProfile profile, string[] roles)
		{
			_FormsAuthentication.SignIn(profile, roles);
			RaiseSignedIn(profile, roles);
		}

		public static void SignOut()
		{
			var profile = SignedInUserProfile;
			_FormsAuthentication.SignOut();
			RaiseSignedOut(profile);
		}

        internal static string[] GetRolesForUser(string userName)
        {
            bool hasIdentity = _FormsAuthentication.CurrentHttpContext != null 
                && _FormsAuthentication.CurrentHttpContext.User != null 
                && _FormsAuthentication.CurrentHttpContext.User.Identity != null;

            if (!hasIdentity)
            {
                throw new InvalidOperationException("HttpContext.User.Identity cannot be null");
            }

            if (userName != _FormsAuthentication.CurrentHttpContext.User.Identity.Name)
            {
                throw new NotSupportedException("GetRolesForUser only supports the current FormsAuthentication user");
            }

            return SignedInUserRoles;
        }

		public static string[] SignedInUserRoles
		{
            get
            {
                return _FormsAuthentication.GetSignedInUserRoles();
            }
		}

		public static IUserProfile SignedInUserProfile
		{
            get
            {
                return _FormsAuthentication.GetSignedInUserProfile();
            }
		}

		public static T GetSignedInUserProfileAs<T>() where T : IUserProfile
		{
			return _FormsAuthentication.GetSignedInUserProfileAs<T>();
		}

        public static string SignedInUserName
        {
            get
            {
                return _FormsAuthentication.GetSignedInUserName();
            }            
        }

        public static DateTime? SignedInTimestampUtc
        {
            get
            {
                return _FormsAuthentication.GetSignedInTimestampUtc();
            }
        }
	}
}
