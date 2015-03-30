using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace jaytwo.AspNet.FormsAuth.Internal
{
    internal static class FormsAuthenticationServiceHelpers
    {
        private static class TicketUserDataKeys
        {
            public static readonly string Roles = "roles";
            public static readonly string Profile = "profile";
            public static readonly string ProfileType = "profileType";
        }

        public static string GetUserDataJson(IUserProfile profile, string[] roles)
        {
            var userData = new Dictionary<string, object>();

            if (roles != null)
            {
                userData[TicketUserDataKeys.Roles] = roles;
            }

            if (profile != null)
            {
                userData[TicketUserDataKeys.Profile] = profile;
                userData[TicketUserDataKeys.ProfileType] = profile.GetType().FullName;
            }

            var result = SerializationUtility.ToJson(userData);
            return result;
        }

        private static IDictionary<string, object> TryGetUserDataDictionary(string userDataJson)
        {
            try
            {
                if (!string.IsNullOrEmpty(userDataJson))
                {
                    return SerializationUtility.FromJson<IDictionary<string, object>>(userDataJson);
                }
            }
            catch { }

            return null;
        }

        public static string[] GetUserRoles(string userDataJson)
        {
            var userData = TryGetUserDataDictionary(userDataJson);
            var userDataArray = GetValueIfExists(userData, TicketUserDataKeys.Roles) as object[];

            if (userDataArray != null)
            {
                var result = userDataArray.Cast<string>().ToArray();
                return result;
            }

            return null;
        }

        public static T GetUserProfile<T>(string userName, string userDataJson) where T : IUserProfile
        {
            var userData = TryGetUserDataDictionary(userDataJson);
            var profileDictionary = GetValueIfExists(userData, TicketUserDataKeys.Profile) as IDictionary<string, object>;
            var profileTypeName = GetValueIfExists(userData, TicketUserDataKeys.ProfileType) as string;

            IUserProfile result = null;

            if (profileDictionary != null)
            {
                if (!string.IsNullOrEmpty(profileTypeName))
                {
                    var profileType = Type.GetType(profileTypeName);

                    if (profileType != null && typeof(T).IsAssignableFrom(profileType))
                    {
                        result = SerializationUtility.FromDictionary(profileDictionary, profileType) as IUserProfile;
                    }
                }

                if (result == null)
                {
                    if (typeof(T) == typeof(IUserProfile))
                    {
                        result = new SimpleUserProfile();
                    }
                    else
                    {
                        result = SerializationUtility.FromDictionary<T>(profileDictionary);
                    }                    
                }

                result.UserName = userName;
            }

            return (T)result;
        }

        private static TValue GetValueIfExists<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
        {
            if (dictionary != null && dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            else
            {
                return null;
            }
        }
    }
}
