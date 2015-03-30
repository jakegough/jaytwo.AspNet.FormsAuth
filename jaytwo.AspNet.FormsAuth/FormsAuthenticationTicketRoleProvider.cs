using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Web;
using jaytwo.AspNet.FormsAuth.Internal;

namespace jaytwo.AspNet.FormsAuth
{
    public class FormsAuthenticationTicketRoleProvider : RoleProvider
    {
        public FormsAuthenticationTicketRoleProvider()
        {
        }

        public override string[] GetRolesForUser(string username)
        {			
			return FormsAuthenticationAppHost.GetRolesForUser(username);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var result = false;

            var roles = GetRolesForUser(username);

            if (roles != null)
            {
                result = roles.Contains(roleName, StringComparer.InvariantCultureIgnoreCase);
            }

            return result;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotSupportedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotSupportedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotSupportedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotSupportedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotSupportedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotSupportedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotSupportedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotSupportedException();
        }

		private string applicationName;
        public override string ApplicationName
        {
            get
            {
				return applicationName;
            }
            set
            {
				applicationName = value;
            }
        }
    }
}
