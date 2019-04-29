using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Libraries
{
	public static class HttpWebRequestExtensions
	{
		private readonly static string[] RestrictedHeaders;

		private readonly static Dictionary<string, PropertyInfo> HeaderProperties;

		static HttpWebRequestExtensions()
		{
			HttpWebRequestExtensions.RestrictedHeaders = new string[] { "Accept", "Connection", "Content-Length", "Content-Type", "Date", "Expect", "Host", "If-Modified-Since", "Keep-Alive", "Proxy-Connection", "Range", "Referer", "Transfer-Encoding", "User-Agent" };
			HttpWebRequestExtensions.HeaderProperties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
			Type type = typeof(HttpWebRequest);
			string[] restrictedHeaders = HttpWebRequestExtensions.RestrictedHeaders;
			for (int i = 0; i < (int)restrictedHeaders.Length; i++)
			{
				string property = restrictedHeaders[i];
				HttpWebRequestExtensions.HeaderProperties[property] = type.GetProperty(property.Replace("-", ""));
			}
		}

		public static void SetRawHeader(this WebRequest request, string name, string value)
		{
			if (!HttpWebRequestExtensions.HeaderProperties.ContainsKey(name))
			{
				request.Headers[name] = value;
				return;
			}
			PropertyInfo item = HttpWebRequestExtensions.HeaderProperties[name];
			if (item.PropertyType == typeof(DateTime))
			{
				item.SetValue(request, DateTime.Parse(value), null);
				return;
			}
			if (item.PropertyType == typeof(bool))
			{
				item.SetValue(request, bool.Parse(value), null);
				return;
			}
			if (item.PropertyType != typeof(long))
			{
				item.SetValue(request, value, null);
				return;
			}
			item.SetValue(request, long.Parse(value), null);
		}

		public static void SetRawHeaders(this WebRequest request, Dictionary<string, string> headers)
		{
			foreach (KeyValuePair<string, string> header in headers)
			{
				request.SetRawHeader(header.Key, header.Value);
			}
		}
	}
}