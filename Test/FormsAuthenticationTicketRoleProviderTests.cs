using jaytwo.AspNet.FormsAuth.Internal;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace jaytwo.AspNet.FormsAuth.Test
{
    [TestFixture]
    public static class FormsAuthenticationTicketRoleProviderTests
    {
        [Test]
        public static void FormsAuthenticationTicketRoleProvider_ApplicationName()
        {
            var provider = new FormsAuthenticationTicketRoleProvider();
            var randomName = "hello world";
            provider.ApplicationName = randomName;
            Assert.AreEqual(randomName, provider.ApplicationName);
        }

        [Test]
        public static void FormsAuthenticationTicketRoleProvider_GetRolesForUser_IsUserInRole()
        {
            var userName = "Jake";
            var roles = new[] { "user", "bro" };
            var profile = new SimpleUserProfile(userName);

            var mockIdentity = MockRepository.GenerateStub<IIdentity>();
            mockIdentity.Stub(x => x.Name).Return(userName);

            var mockUser = MockRepository.GenerateStub<IPrincipal>();
            mockUser.Stub(x => x.Identity).Return(mockIdentity);
            
            var mockContext = MockRepository.GenerateStub<HttpContextBase>();
            mockContext.User = mockUser;

            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();
            mockService.Stub(x => x.CurrentHttpContext).Return(mockContext);

            mockService.Stub(x => x.GetSignedInUserProfile()).Return(profile);
            mockService.Stub(x => x.GetSignedInUserRoles()).Return(roles);

            FormsAuthenticationAppHost.Initialize(mockService);
            var provider = new FormsAuthenticationTicketRoleProvider();
            CollectionAssert.AreEquivalent(roles, provider.GetRolesForUser(userName));

            Assert.IsTrue(provider.IsUserInRole(userName, "user"));
            Assert.IsTrue(provider.IsUserInRole(userName, "bro"));
            Assert.IsFalse(provider.IsUserInRole(userName, "other"));

            Assert.Throws<NotSupportedException>(() => provider.IsUserInRole("randomUser", "user"));
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public static void FormsAuthenticationTicketRoleProvider_CreateRole_NotSupported()
        {
            new FormsAuthenticationTicketRoleProvider().CreateRole("role");
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public static void FormsAuthenticationTicketRoleProvider_DeleteRole_NotSupported()
        {
            new FormsAuthenticationTicketRoleProvider().DeleteRole("role", true);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public static void FormsAuthenticationTicketRoleProvider_RoleExists_NotSupported()
        {
            new FormsAuthenticationTicketRoleProvider().RoleExists("role");
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public static void FormsAuthenticationTicketRoleProvider_AddUsersToRoles_NotSupported()
        {
            new FormsAuthenticationTicketRoleProvider().AddUsersToRoles(new string[] { }, new string[] { });
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public static void FormsAuthenticationTicketRoleProvider_RemoveUsersFromRoles_NotSupported()
        {
            new FormsAuthenticationTicketRoleProvider().RemoveUsersFromRoles(new string[] { }, new string[] { });
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public static void FormsAuthenticationTicketRoleProvider_GetUsersInRole_NotSupported()
        {
            new FormsAuthenticationTicketRoleProvider().GetUsersInRole("role");
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public static void FormsAuthenticationTicketRoleProvider_GetAllRoles_NotSupported()
        {
            new FormsAuthenticationTicketRoleProvider().GetAllRoles();
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public static void FormsAuthenticationTicketRoleProvider_FindUsersInRole_NotSupported()
        {
            new FormsAuthenticationTicketRoleProvider().FindUsersInRole("role", "username");
        }
    }
}
