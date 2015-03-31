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
            mockService.Stub(x => x.GetSignedInUserProfile()).Return(profile);
            mockService.Stub(x => x.GetSignedInUserProfileAs<SimpleUserProfile>()).Return(profile);
            mockService.Stub(x => x.GetSignedInUserRoles()).Return(roles);

            FormsAuthenticationAppHost.Initialize(mockService);
            Assert.AreSame(profile, FormsAuthenticationAppHost.SignedInUserProfile);
            Assert.AreSame(profile, FormsAuthenticationAppHost.GetSignedInUserProfileAs<SimpleUserProfile>());
            CollectionAssert.AreEquivalent(roles, FormsAuthenticationAppHost.SignedInUserRoles);
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

            mockService.Stub(x => x.GetSignedInUserProfile())
                .WhenCalled(x => 
                { 
                    x.ReturnValue = mockServiceProfile; 
                })
                .Return(null); // end with .Return() to make Rhino happy;

            mockService.Stub(x => x.GetSignedInUserProfileAs<SimpleUserProfile>())
                .WhenCalled(x =>
                {
                    x.ReturnValue = mockServiceProfile;
                })
                .Return(null); // end with .Return() to make Rhino happy;

            mockService.Stub(x => x.GetSignedInUserRoles())
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
                Assert.IsNull(FormsAuthenticationAppHost.SignedInUserProfile);
                CollectionAssert.IsEmpty(FormsAuthenticationAppHost.SignedInUserRoles);

                FormsAuthenticationAppHost.SignIn(profile, roles);
                Assert.AreEqual(profile.UserName, SimpleUserProfile.Current.UserName);
                Assert.AreEqual(profile.UserName, FormsAuthenticationAppHost.SignedInUserProfile.UserName);                
                CollectionAssert.AreEquivalent(roles, FormsAuthenticationAppHost.SignedInUserRoles);
                Assert.IsTrue(onSignedInRaised);

                FormsAuthenticationAppHost.SignOut();
                Assert.IsNull(FormsAuthenticationAppHost.SignedInUserProfile);
                CollectionAssert.IsEmpty(FormsAuthenticationAppHost.SignedInUserRoles);
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

            mockService.Stub(x => x.GetSignedInUserProfile())
                .WhenCalled(x =>
                {
                    x.ReturnValue = mockServiceProfile;
                })
                .Return(null); // end with .Return() to make Rhino happy;

            FormsAuthenticationAppHost.Initialize(mockService);

            Assert.IsNull(FormsAuthenticationAppHost.SignedInUserProfile);

            var userName = "Jake";
            var profile = new SimpleUserProfile(userName);

            FormsAuthenticationAppHost.SignIn(profile);
            Assert.AreEqual(profile.UserName, FormsAuthenticationAppHost.SignedInUserProfile.UserName);
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

            mockService.Stub(x => x.GetSignedInUserProfile())
                .WhenCalled(x =>
                {
                    x.ReturnValue = mockServiceProfile;
                })
                .Return(null); // end with .Return() to make Rhino happy;

            FormsAuthenticationAppHost.Initialize(mockService);

            Assert.IsNull(FormsAuthenticationAppHost.SignedInUserProfile);

            var userName = "Jake";

            FormsAuthenticationAppHost.SignIn(userName);
            Assert.AreEqual(userName, FormsAuthenticationAppHost.SignedInUserProfile.UserName);            
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

            mockService.Stub(x => x.GetSignedInUserProfile())
                .WhenCalled(x =>
                {
                    x.ReturnValue = mockServiceProfile;
                })
                .Return(null); // end with .Return() to make Rhino happy;

            mockService.Stub(x => x.GetSignedInUserRoles())
                .WhenCalled(x =>
                {
                    x.ReturnValue = mockServiceRoles ?? new string[] { };
                })
                .Return(null); // end with .Return() to make Rhino happy

            FormsAuthenticationAppHost.Initialize(mockService);

            Assert.IsNull(FormsAuthenticationAppHost.SignedInUserProfile);
            CollectionAssert.IsEmpty(FormsAuthenticationAppHost.SignedInUserRoles);

            var userName = "Jake";
            var roles = new[] { "user", "bro" };

            FormsAuthenticationAppHost.SignIn(userName, roles);
            Assert.AreEqual(userName, FormsAuthenticationAppHost.SignedInUserProfile.UserName);
            CollectionAssert.AreEquivalent(roles, FormsAuthenticationAppHost.SignedInUserRoles);
        }
    }
}
