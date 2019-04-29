using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Facepunch.GoogleSheets
{
	public static class Import
	{
		private static Regex regex;

		static Import()
		{
			Import.regex = new Regex("(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
		}

		public static T[] FromUrl<T>(string sheetId)
		where T : new()
		{
			Func<PropertyInfo, bool> func = null;
			Import.MonoSecurityBullshitHack();
			string str = string.Format("http://docs.google.com/spreadsheets/d/{0}/pub?output=csv", sheetId);
			string str1 = (new WebClient()).DownloadString(str);
			Console.WriteLine(str1);
			string[] strArrays = str1.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			string[] cSVColumns = Import.GetCSVColumns(strArrays.First<string>());
			List<T> ts = new List<T>();
			foreach (string str2 in strArrays.Skip<string>(1))
			{
				string[] cSVColumns1 = Import.GetCSVColumns(str2);
				T t = Activator.CreateInstance<T>();
				for (int i = 0; i < (int)cSVColumns1.Length; i++)
				{
					PropertyInfo[] propertyInfoArray = properties;
					Func<PropertyInfo, bool> func1 = func;
					if (func1 == null)
					{
						Func<PropertyInfo, bool> func2 = (PropertyInfo x) => x.Name.Equals(cSVColumns[i], StringComparison.CurrentCultureIgnoreCase);
						Func<PropertyInfo, bool> func3 = func2;
						func = func2;
						func1 = func3;
					}
					foreach (PropertyInfo propertyInfo in ((IEnumerable<PropertyInfo>)propertyInfoArray).Where<PropertyInfo>(func1))
					{
						propertyInfo.SetValue(t, Convert.ChangeType(cSVColumns1[i], propertyInfo.PropertyType), null);
					}
				}
				ts.Add(t);
			}
			return ts.ToArray();
		}

		private static string[] GetCSVColumns(string line)
		{
			line = line.Trim(new char[] { '\n', '\r' });
			return Import.regex.Matches(line).Cast<Match>().Select<Match, string>((Match x) => {
				string str = x.Value.Replace("\"\"", "\"");
				if (str.EndsWith("\"") && str.StartsWith("\""))
				{
					str = str.Substring(1, str.Length - 2);
				}
				return str;
			}).ToArray<string>();
		}

		private static void MonoSecurityBullshitHack()
		{
			ServicePointManager.ServerCertificateValidationCallback = (object a, X509Certificate b, X509Chain c, SslPolicyErrors d) => true;
		}
	}
}