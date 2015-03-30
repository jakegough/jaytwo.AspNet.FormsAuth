using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace jaytwo.AspNet.FormsAuth.Internal
{
    internal static class SerializationUtility
    {
        public static string ToJson(object value)
        {
            return new JavaScriptSerializer().Serialize(value);
        }

        public static T FromJson<T>(string json)
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }

		public static object FromJson(string json, Type type)
		{
			var serializer = new JavaScriptSerializer();

// works in .Net 4.0 but not 4.5
//#if NET_4_0
//			var result = serializer.Deserialize(json, type);
//#else
			var result = typeof(JavaScriptSerializer)
				.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.ExactBinding)
				.Single(x => x.Name == "Deserialize" && x.IsGenericMethod)
				.MakeGenericMethod(type)
				.Invoke(serializer, new object[] { json });
//#endif
			return result;
		}

		public static IDictionary<string, object> ToDictionary(object value)
		{
			var json = new JavaScriptSerializer().Serialize(value);
			var result = new JavaScriptSerializer().Deserialize<IDictionary<string, object>>(json);
			return result;
		}

        public static T FromDictionary<T>(IDictionary<string, object> dictionary)
        {
			var serializer = new JavaScriptSerializer();
			var dictionaryJson = serializer.Serialize(dictionary);
			var result = serializer.Deserialize<T>(dictionaryJson);
            return result;
        }

		public static object FromDictionary(IDictionary<string, object> dictionary, Type type)
		{
			var serializer = new JavaScriptSerializer();
			var dictionaryJson = serializer.Serialize(dictionary);
			var result = FromJson(dictionaryJson, type);
			return result;
		}
    }
}
