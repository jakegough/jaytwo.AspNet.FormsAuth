using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jaytwo.AspNet.FormsAuth.Internal;

namespace jaytwo.AspNet.FormsAuth.Test.Internal
{
	[TestFixture]
	public static class FormsAuthenticationHelpersTests
	{
		[Test]
		public static void FormsAuthenticationHelpers_GetUserDataJson_profile_roles()
		{
			var userProfile = new SimpleUserProfile("Jake");
			var roles = new[] { "user", "bro" };
			var json = FormsAuthenticationServiceHelpers.GetUserDataJson(userProfile, roles);

            var deserialized = SerializationUtility.FromJson<IDictionary<string, object>>(json);
            Assert.AreEqual("jaytwo.AspNet.FormsAuth.SimpleUserProfile", deserialized["profileType"]);

            var deserializedProfile = deserialized["profile"] as IDictionary<string, object>;
            Assert.AreEqual("Jake", deserializedProfile["UserName"]);

            var deserializedRoles = deserialized["roles"] as object[];
            CollectionAssert.AreEqual(roles, deserializedRoles);
		}

		[Test]
		public static void FormsAuthenticationHelpers_GetUserProfile()
		{
			var userName = "Jake";
			var userDataJson = "{\"profile\":{\"UserName\":\"Jake\"},\"profileType\":\"jaytwo.AspNet.FormsAuth.SimpleUserProfile\",\"roles\":[\"user\",\"bro\"]}";

			var profile = FormsAuthenticationServiceHelpers.GetUserProfile<SimpleUserProfile>(userName, userDataJson);

			Assert.AreEqual(userName, profile.UserName);
		}

        [Test]
        public static void FormsAuthenticationHelpers_GetUserProfile_IUserProfile()
        {
            var userName = "Jake";
            var userDataJson = "{\"profile\":{\"UserName\":\"Jake\"},\"profileType\":\"some.random.UserProfile\",\"roles\":[\"user\",\"bro\"]}";

            var profile = FormsAuthenticationServiceHelpers.GetUserProfile<IUserProfile>(userName, userDataJson);

            Assert.AreEqual(userName, profile.UserName);
        }

        [Test]
        public static void FormsAuthenticationHelpers_GetUserProfile_SimpleUserProfile()
        {
            // case: serialized as an unknown type, deserialize to SimpleUserProfile

            var userName = "Jake";
            var userDataJson = "{\"profile\":{\"UserName\":\"Jake\"},\"profileType\":\"some.random.UserProfile\",\"roles\":[\"user\",\"bro\"]}";

            var profile = FormsAuthenticationServiceHelpers.GetUserProfile<SimpleUserProfile>(userName, userDataJson);

            Assert.AreEqual(userName, profile.UserName);
        }

        [Test]
        public static void FormsAuthenticationHelpers_GetUserProfile_bad_json()
        {
            var userName = "Jake";
            var userDataJson = "hello world";

            var profile = FormsAuthenticationServiceHelpers.GetUserProfile<SimpleUserProfile>(userName, userDataJson);

            Assert.IsNull(profile);
        }

		[Test]
		public static void FormsAuthenticationHelpers_GetUserRoles()
		{
			var userDataJson = "{\"profile\":{\"UserName\":\"Jake\"},\"profileType\":\"jaytwo.AspNet.FormsAuth.SimpleUserProfile\",\"roles\":[\"user\",\"bro\"]}";

			var expected = new[] { "user", "bro" };
			var roles = FormsAuthenticationServiceHelpers.GetUserRoles(userDataJson);

			CollectionAssert.AreEqual(expected, roles);
		}

        [Test]
        public static void FormsAuthenticationHelpers_GetUserRoles_bad_json()
        {
            var userDataJson = "hello world";
            var expected = new[] { "user", "bro" };
            Assert.IsNull(FormsAuthenticationServiceHelpers.GetUserRoles(userDataJson));
        }
	}
}
