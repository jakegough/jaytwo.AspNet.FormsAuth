using System;
using NUnit.Framework;
using jaytwo.AspNet.FormsAuth.Internal;
using System.Collections.Generic;

namespace jaytwo.AspNet.FormsAuth.Test.Internal
{
	[TestFixture]
	public static class SerializationUtilityTests
	{
		[Test]
		public static void SerializationUtility_ToJson()
		{
			var dictionary = new Dictionary<string, string>();
			dictionary["foo"] = "fooValue";
			dictionary["bar"] = "barValue";

			var json = SerializationUtility.ToJson(dictionary);
			var expected = "{\"foo\":\"fooValue\",\"bar\":\"barValue\"}";
			Assert.AreEqual(expected, json);
		}

		[Test]
		public static void SerializationUtility_FromJson_generic()
		{
			var json = "{\"foo\":\"fooValue\",\"bar\":\"barValue\"}";
			var dictionary = SerializationUtility.FromJson<IDictionary<string, string>>(json);

			Assert.AreEqual(dictionary["foo"], "fooValue");
			Assert.AreEqual(dictionary["bar"], "barValue");
		}

		[Test]
		public static void SerializationUtility_FromJson_string_Type()
		{
			var json = "{\"foo\":\"fooValue\",\"bar\":\"barValue\"}";
			var dictionary = SerializationUtility.FromJson(json, typeof(IDictionary<string, string>)) as IDictionary<string, string>;

			Assert.AreEqual(dictionary["foo"], "fooValue");
			Assert.AreEqual(dictionary["bar"], "barValue");
		}

		[Test]
		public static void SerializationUtility_ToDictionary()
		{
			var jake = new Person();
			jake.Name = "Jake";
			jake.Eyes = "Lazy";

			var dictionary = SerializationUtility.ToDictionary(jake);

			Assert.AreEqual(jake.Name, dictionary["Name"]);
			Assert.AreEqual(jake.Eyes, dictionary["Eyes"]);
		}

		[Test]
		public static void SerializationUtility_FromDictionary_generic()
		{
			var dictionary = new Dictionary<string,object>();
			dictionary["Name"] = "Jake";
			dictionary["Eyes"] = "Lazy";

			var jake = SerializationUtility.FromDictionary<Person>(dictionary);

			Assert.AreEqual(dictionary["Name"], jake.Name);
			Assert.AreEqual(dictionary["Eyes"], jake.Eyes);
		}

		[Test]
		public static void SerializationUtility_FromDictionary_dictionary_type()
		{
			var dictionary = new Dictionary<string, object>();
			dictionary["Name"] = "Jake";
			dictionary["Eyes"] = "Lazy";

			var jake = SerializationUtility.FromDictionary(dictionary, typeof(Person)) as Person;

			Assert.AreEqual(dictionary["Name"], jake.Name);
			Assert.AreEqual(dictionary["Eyes"], jake.Eyes);
		}

		private class Person
		{
			public string Name { get; set; }
			public string Eyes { get; set; }
		}
	}
}