using jaytwo.AspNet.FormsAuth.Internal;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jaytwo.AspNet.FormsAuth.Test
{
    [TestFixture]
    public class FormsAuthenticationAppHostTests
    {
        [Test]
        public void FormsAuthenticationAppHost_GetCurrentUserProfile_GetCurrentUserRoles()
        {
            var userName = "Jake";
            var roles = new[] { "user", "bro" };
            var profile = new SimpleUserProfile(userName);
            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();
            mockService.Stub(x => x.GetCurrentUserProfile()).Return(profile);
            mockService.Stub(x => x.GetCurrentUserProfile<SimpleUserProfile>()).Return(profile);
            mockService.Stub(x => x.GetCurrentUserRoles()).Return(roles);

            FormsAuthenticationAppHost.Initialize(mockService);
            Assert.AreSame(profile, FormsAuthenticationAppHost.GetCurrentUserProfile());
            Assert.AreSame(profile, FormsAuthenticationAppHost.GetCurrentUserProfile<SimpleUserProfile>());
            CollectionAssert.AreEquivalent(roles, FormsAuthenticationAppHost.GetCurrentUserRoles());
        }

        [Test]
        public void FormsAuthenticationAppHost_SignIn_with_profile_roles_then_SignOut()
        {
            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();

            SimpleUserProfile mockServiceProfile = null;
            string[] mockServiceRoles = null;

            mockService.Stub(x => x.SignIn(Arg<IUserProfile>.Is.Anything, Arg<string[]>.Is.Anything))
                .WhenCalled(x =>
                {
                    mockServiceProfile = (SimpleUserProfile)x.Arguments[0];
                    mockServiceRoles = (string[])x.Arguments[1];
                });

            mockService.Stub(x => x.SignOut())
                .WhenCalled(x =>
                {
                    mockServiceProfile = null;
                    mockServiceRoles = null;
                });

            mockService.Stub(x => x.GetCurrentUserProfile())
                .WhenCalled(x => 
                { 
                    x.ReturnValue = mockServiceProfile; 
                })
                .Return(null); // end with .Return() to make Rhino happy;

            mockService.Stub(x => x.GetCurrentUserProfile<SimpleUserProfile>())
                .WhenCalled(x =>
                {
                    x.ReturnValue = mockServiceProfile;
                })
                .Return(null); // end with .Return() to make Rhino happy;

            mockService.Stub(x => x.GetCurrentUserRoles())
                .WhenCalled(x => 
                { 
                    x.ReturnValue = mockServiceRoles ?? new string[] { }; 
                })
                .Return(null); // end with .Return() to make Rhino happy

            var userName = "Jake";
            var roles = new[] { "user", "bro" };
            var profile = new SimpleUserProfile(userName);

            bool onSignedInRaised = false;
            bool onSignedOutRaised = false;

            FormsAuthenticationAppHost.SignedInHandler onSignedIn = (eventProfile, eventRoles) =>
            {
                Assert.AreSame(eventProfile, profile);
                Assert.AreSame(eventRoles, roles);
                onSignedInRaised = true;
            };

            FormsAuthenticationAppHost.SignedOutHandler onSignedOut = (eventProfile) =>
            {
                Assert.AreSame(eventProfile, profile);
                onSignedOutRaised = true;
            };

            FormsAuthenticationAppHost.Initialize(mockService);
            FormsAuthenticationAppHost.SignedIn += onSignedIn;
            FormsAuthenticationAppHost.SignedOut += onSignedOut;

            try
            {
                Assert.IsNull(FormsAuthenticationAppHost.GetCurrentUserProfile());
                CollectionAssert.IsEmpty(FormsAuthenticationAppHost.GetCurrentUserRoles());

                FormsAuthenticationAppHost.SignIn(profile, roles);
                Assert.AreEqual(profile.UserName, SimpleUserProfile.Current.UserName);
                Assert.AreEqual(profile.UserName, FormsAuthenticationAppHost.GetCurrentUserProfile().UserName);                
                CollectionAssert.AreEquivalent(roles, FormsAuthenticationAppHost.GetCurrentUserRoles());
                Assert.IsTrue(onSignedInRaised);

                FormsAuthenticationAppHost.SignOut();
                Assert.IsNull(FormsAuthenticationAppHost.GetCurrentUserProfile());
                CollectionAssert.IsEmpty(FormsAuthenticationAppHost.GetCurrentUserRoles());
                Assert.IsTrue(onSignedOutRaised);
            }
            finally
            {
                FormsAuthenticationAppHost.SignedIn -= onSignedIn;
                FormsAuthenticationAppHost.SignedOut -= onSignedOut;
            }           
        }

        [Test]
        public void FormsAuthenticationAppHost_SignIn_with_profile()
        {
            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();

            IUserProfile mockServiceProfile = null;
            
            mockService.Stub(x => x.SignIn(Arg<IUserProfile>.Is.Anything, Arg<string[]>.Is.Anything))
                .WhenCalled(x =>
                {
                    mockServiceProfile = (IUserProfile)x.Arguments[0];
                });

            mockService.Stub(x => x.GetCurrentUserProfile())
                .WhenCalled(x =>
                {
                    x.ReturnValue = mockServiceProfile;
                })
                .Return(null); // end with .Return() to make Rhino happy;

            FormsAuthenticationAppHost.Initialize(mockService);

            Assert.IsNull(FormsAuthenticationAppHost.GetCurrentUserProfile());

            var userName = "Jake";
            var profile = new SimpleUserProfile(userName);

            FormsAuthenticationAppHost.SignIn(profile);
            Assert.AreEqual(profile.UserName, FormsAuthenticationAppHost.GetCurrentUserProfile().UserName);
        }


        [Test]
        public void FormsAuthenticationAppHost_SignIn_with_userName()
        {
            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();

            IUserProfile mockServiceProfile = null;
            
            mockService.Stub(x => x.SignIn(Arg<IUserProfile>.Is.Anything, Arg<string[]>.Is.Anything))
                .WhenCalled(x =>
                {
                    mockServiceProfile = (IUserProfile)x.Arguments[0];
                });

            mockService.Stub(x => x.GetCurrentUserProfile())
                .WhenCalled(x =>
                {
                    x.ReturnValue = mockServiceProfile;
                })
                .Return(null); // end with .Return() to make Rhino happy;

            FormsAuthenticationAppHost.Initialize(mockService);

            Assert.IsNull(FormsAuthenticationAppHost.GetCurrentUserProfile());

            var userName = "Jake";

            FormsAuthenticationAppHost.SignIn(userName);
            Assert.AreEqual(userName, FormsAuthenticationAppHost.GetCurrentUserProfile().UserName);            
        }

        [Test]
        public void FormsAuthenticationAppHost_SignIn_with_userName_roles()
        {
            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();

            IUserProfile mockServiceProfile = null;
            string[] mockServiceRoles = null;

            mockService.Stub(x => x.SignIn(Arg<IUserProfile>.Is.Anything, Arg<string[]>.Is.Anything))
                .WhenCalled(x =>
                {
                    mockServiceProfile = (IUserProfile)x.Arguments[0];
                    mockServiceRoles = (string[])x.Arguments[1];
                });

            mockService.Stub(x => x.GetCurrentUserProfile())
                .WhenCalled(x =>
                {
                    x.ReturnValue = mockServiceProfile;
                })
                .Return(null); // end with .Return() to make Rhino happy;

            mockService.Stub(x => x.GetCurrentUserRoles())
                .WhenCalled(x =>
                {
                    x.ReturnValue = mockServiceRoles ?? new string[] { };
                })
                .Return(null); // end with .Return() to make Rhino happy

            FormsAuthenticationAppHost.Initialize(mockService);

            Assert.IsNull(FormsAuthenticationAppHost.GetCurrentUserProfile());
            CollectionAssert.IsEmpty(FormsAuthenticationAppHost.GetCurrentUserRoles());

            var userName = "Jake";
            var roles = new[] { "user", "bro" };

            FormsAuthenticationAppHost.SignIn(userName, roles);
            Assert.AreEqual(userName, FormsAuthenticationAppHost.GetCurrentUserProfile().UserName);
            CollectionAssert.AreEquivalent(roles, FormsAuthenticationAppHost.GetCurrentUserRoles());
        }
    }
}
