using jaytwo.AspNet.FormsAuth.Internal;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace jaytwo.AspNet.FormsAuth.Test.Internal
{
	[TestFixture]
	public static class FormsAuthenticationServiceTests
	{
        [Test]
        public static void FormsAuthenticationService_ClearFormsAuthenticationTicket_request_cookie_does_not_exist()
		{
            var mockRequest = MockRepository.GenerateMock<HttpRequestBase>();
            var requestCookies = new HttpCookieCollection();
            mockRequest.Stub(x => x.Cookies).Return(requestCookies);
            //mockRequest.Cookies.Add(new HttpCookie(mockService.CookieName, "request hello world"));

            var mockResponse = MockRepository.GenerateMock<HttpResponseBase>();
            var responseCookies = new HttpCookieCollection();
            mockResponse.Stub(x => x.Cookies).Return(responseCookies);
            //mockResponse.Cookies.Add(new HttpCookie(mockService.CookieName, "response hello world"));

            var mockHttpContext = MockRepository.GenerateMock<HttpContextBase>();
            mockHttpContext.Stub(x => x.Request).Return(mockRequest);
            mockHttpContext.Stub(x => x.Response).Return(mockResponse);

            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();
            mockService.Stub(x => x.CurrentHttpContext).Return(mockHttpContext);

            var requestCookieBefore = mockRequest.Cookies[mockService.CookieName];
            var responseCookieBefore = mockResponse.Cookies[mockService.CookieName];

            mockResponse
                .Stub(x => x.SetCookie(Arg<HttpCookie>.Is.Anything))
                .WhenCalled(x =>
                {
                    HttpCookie cookie = (HttpCookie)x.Arguments[0]; // first arg is an int
                    responseCookies.Add(cookie);
                });

            mockService.ClearFormsAuthenticationTicket();

            var requestCookieAfter = mockRequest.Cookies[mockService.CookieName];
            var responseCookieAfter = mockResponse.Cookies[mockService.CookieName];

            Assert.That(requestCookieBefore, Is.Null);
            Assert.That(requestCookieAfter, Is.Null);
            Assert.That(responseCookieBefore, Is.Null);
            Assert.That(responseCookieAfter.Expires, Is.LessThan(DateTime.Now));
            Assert.That(responseCookieAfter.Value, Is.Null);
		}

        [Test]
        public static void FormsAuthenticationService_ClearFormsAuthenticationTicket_request_cookie_already_exists()
        {
            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();

            var mockRequest = MockRepository.GenerateMock<HttpRequestBase>();
            var requestCookies = new HttpCookieCollection();
            mockRequest.Stub(x => x.Cookies).Return(requestCookies);
            mockRequest.Cookies.Add(new HttpCookie(mockService.CookieName, "request hello world"));

            var mockResponse = MockRepository.GenerateMock<HttpResponseBase>();
            var responseCookies = new HttpCookieCollection();
            mockResponse.Stub(x => x.Cookies).Return(responseCookies);
            //mockResponse.Cookies.Add(new HttpCookie(mockService.CookieName, "response hello world"));

            var mockHttpContext = MockRepository.GenerateMock<HttpContextBase>();
            mockHttpContext.Stub(x => x.Request).Return(mockRequest);
            mockHttpContext.Stub(x => x.Response).Return(mockResponse);

            mockService.Stub(x => x.CurrentHttpContext).Return(mockHttpContext);

            mockResponse
                .Stub(x => x.SetCookie(Arg<HttpCookie>.Is.Anything))
                .WhenCalled(x =>
                {
                    HttpCookie cookie = (HttpCookie)x.Arguments[0]; // first arg is an int
                    responseCookies.Add(cookie);
                });

            var requestCookieBefore = mockRequest.Cookies[mockService.CookieName];
            Assert.That(requestCookieBefore.Value, Is.EqualTo("request hello world"));

            var responseCookieBefore = mockResponse.Cookies[mockService.CookieName];
            Assert.That(responseCookieBefore, Is.Null);

            mockService.ClearFormsAuthenticationTicket();

            var requestCookieAfter = mockRequest.Cookies[mockService.CookieName];
            Assert.That(requestCookieAfter.Value, Is.Null);

            var responseCookieAfter = mockResponse.Cookies[mockService.CookieName];
            Assert.That(responseCookieAfter.Expires, Is.GreaterThan(DateTime.MinValue));
            Assert.That(responseCookieAfter.Expires, Is.LessThan(DateTime.Now));            
            Assert.That(responseCookieAfter.Value, Is.Null);
        }

        [Test]
        public static void FormsAuthenticationService_SetFormsAuthenticationTicket_GetFormsAuthenticationTicket()
        {
            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();

            var mockRequest = MockRepository.GenerateMock<HttpRequestBase>();
            var requestCookies = new HttpCookieCollection();
            mockRequest.Stub(x => x.Cookies).Return(requestCookies);
            mockRequest.Cookies.Add(new HttpCookie(mockService.CookieName, "request hello world"));

            var mockResponse = MockRepository.GenerateMock<HttpResponseBase>();
            var responseCookies = new HttpCookieCollection();
            mockResponse.Stub(x => x.Cookies).Return(responseCookies);
            mockResponse.Cookies.Add(new HttpCookie(mockService.CookieName, "response hello world"));

            var mockHttpContext = MockRepository.GenerateMock<HttpContextBase>();
            mockHttpContext.Stub(x => x.Request).Return(mockRequest);
            mockHttpContext.Stub(x => x.Response).Return(mockResponse);

            mockService.Stub(x => x.CurrentHttpContext).Return(mockHttpContext);

            var ticketBefore = new FormsAuthenticationTicket("name", false, 10);
            mockService.SetFormsAuthenticationTicket(ticketBefore);

            var ticketAfter = mockService.GetFormsAuthenticationTicket();

            Assert.AreEqual(ticketBefore.Name, ticketAfter.Name);
            Assert.AreEqual(ticketBefore.UserData, ticketAfter.UserData);
            Assert.AreEqual(ticketBefore.IssueDate, ticketAfter.IssueDate);
            Assert.AreEqual(ticketBefore.IsPersistent, ticketAfter.IsPersistent);
            Assert.AreEqual(ticketBefore.Expiration, ticketAfter.Expiration);
            
        }

        [Test]
        public static void FormsAuthenticationService_GetCurrentUserProfile_GetCurrentUserRoles()
        {
            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();

            var userName = "Jake";
            var profile = new SimpleUserProfile(userName);
            var roles = new[] { "user", "bro" };
            var userDataJson = FormsAuthenticationServiceHelpers.GetUserDataJson(profile, roles);
            var ticket = new FormsAuthenticationTicket(
                version: 0,
                name: userName,
                issueDate: DateTime.Now,
                expiration: DateTime.Now.AddYears(1),
                isPersistent: false,
                userData: userDataJson);

            mockService.Stub(x => x.GetFormsAuthenticationTicket())
                .Return(ticket);

            var resultProfile = mockService.GetSignedInUserProfile();
            Assert.AreEqual(profile.UserName, resultProfile.UserName);
            Assert.AreEqual(profile.GetType(), resultProfile.GetType());

            var resultRoles = mockService.GetSignedInUserRoles();
            CollectionAssert.AreEquivalent(roles, resultRoles);
        }

        [Test]
        public static void FormsAuthenticationService_GetCurrentUserProfile_with_null_FormsAuthenticationTicket()
        {
            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();

            mockService.Stub(x => x.GetFormsAuthenticationTicket())
                .Return(null);

            var userProfile = mockService.GetSignedInUserProfile();

            Assert.That(userProfile, Is.Null);
        }

        [Test]
        public static void FormsAuthenticationService_GetCurrentUserRoles_with_null_FormsAuthenticationTicket()
        {
            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();

            mockService.Stub(x => x.GetFormsAuthenticationTicket())
                .Return(null);

            var roles = mockService.GetSignedInUserRoles();

            Assert.That(roles, Is.EquivalentTo(new string[] { }));
        }

        [Test]
        public static void FormsAuthenticationService_SignIn_SignOut()
        {
            var mockRequest = MockRepository.GenerateMock<HttpRequestBase>();
            var requestCookies = new HttpCookieCollection();
            mockRequest.Stub(x => x.Cookies).Return(requestCookies);
            //mockRequest.Cookies.Add(new HttpCookie(mockService.CookieName, "request hello world"));

            var mockResponse = MockRepository.GenerateMock<HttpResponseBase>();
            var responseCookies = new HttpCookieCollection();
            mockResponse.Stub(x => x.Cookies).Return(responseCookies);
            //mockResponse.Cookies.Add(new HttpCookie(mockService.CookieName, "response hello world"));

            var mockSession = MockRepository.GenerateMock<HttpSessionStateBase>();
            
            var mockHttpContext = MockRepository.GenerateMock<HttpContextBase>();
            mockHttpContext.Stub(x => x.Request).Return(mockRequest);            
            mockHttpContext.Stub(x => x.Response).Return(mockResponse);
            mockHttpContext.Stub(x => x.Session).Return(mockSession);

            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();
            mockService.Stub(x => x.CurrentHttpContext).Return(mockHttpContext);

            var userName = "Jake";
            var profile = new SimpleUserProfile(userName);
            var roles = new[] { "user", "bro" };
            mockService.SignIn(profile, roles);

            var signedInUser = mockService.GetSignedInUserProfile();
            Assert.AreEqual(userName, signedInUser.UserName);

            var signedInUserRoles = mockService.GetSignedInUserRoles();
            CollectionAssert.AreEquivalent(roles, signedInUserRoles);

            mockService.SignOut();
            Assert.That(mockService.GetSignedInUserProfile(), Is.Null);
            Assert.That(mockService.GetSignedInUserRoles(), Is.EquivalentTo(new string[] { }));

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public static void FormsAuthenticationService_SignIn_null_profile_throws_exception()
        {
            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();
            var roles = new[] { "user", "bro" };
            mockService.SignIn(null, roles);
        }

        [Test]
        public static void FormsAuthenticationService_SignIn_null_roles_does_not_throw_exception()
        {
            var mockRequest = MockRepository.GenerateMock<HttpRequestBase>();
            var requestCookies = new HttpCookieCollection();
            mockRequest.Stub(x => x.Cookies).Return(requestCookies);
            
            var mockResponse = MockRepository.GenerateMock<HttpResponseBase>();
            var responseCookies = new HttpCookieCollection();
            mockResponse.Stub(x => x.Cookies).Return(responseCookies);
            
            var mockHttpContext = MockRepository.GenerateMock<HttpContextBase>();
            mockHttpContext.Stub(x => x.Request).Return(mockRequest);
            mockHttpContext.Stub(x => x.Response).Return(mockResponse);

            var mockService = MockRepository.GeneratePartialMock<FormsAuthenticationService>();
            mockService.Stub(x => x.CurrentHttpContext).Return(mockHttpContext);

            var userName = "Jake";
            var profile = new SimpleUserProfile(userName);
            mockService.SignIn(profile, null);

            var signedInUser = mockService.GetSignedInUserProfile();
            Assert.AreEqual(userName, signedInUser.UserName);

            var signedInUserRoles = mockService.GetSignedInUserRoles();
            Assert.That(mockService.GetSignedInUserRoles(), Is.EquivalentTo(new string[] { }));

            mockService.SignOut();
            Assert.That(mockService.GetSignedInUserProfile(), Is.Null);
            Assert.That(mockService.GetSignedInUserRoles(), Is.EquivalentTo(new string[] { }));
        }
	}
}
