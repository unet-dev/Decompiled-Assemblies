using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using WebSocketSharp;

namespace WebSocketSharp.Net
{
	internal abstract class AuthenticationBase
	{
		private AuthenticationSchemes _scheme;

		internal NameValueCollection Parameters;

		public string Algorithm
		{
			get
			{
				return this.Parameters["algorithm"];
			}
		}

		public string Nonce
		{
			get
			{
				return this.Parameters["nonce"];
			}
		}

		public string Opaque
		{
			get
			{
				return this.Parameters["opaque"];
			}
		}

		public string Qop
		{
			get
			{
				return this.Parameters["qop"];
			}
		}

		public string Realm
		{
			get
			{
				return this.Parameters["realm"];
			}
		}

		public AuthenticationSchemes Scheme
		{
			get
			{
				return this._scheme;
			}
		}

		protected AuthenticationBase(AuthenticationSchemes scheme, NameValueCollection parameters)
		{
			this._scheme = scheme;
			this.Parameters = parameters;
		}

		internal static string CreateNonceValue()
		{
			byte[] numArray = new byte[16];
			(new Random()).NextBytes(numArray);
			StringBuilder stringBuilder = new StringBuilder(32);
			byte[] numArray1 = numArray;
			for (int i = 0; i < (int)numArray1.Length; i++)
			{
				byte num = numArray1[i];
				stringBuilder.Append(num.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		internal static NameValueCollection ParseParameters(string value)
		{
			string str;
			string str1;
			NameValueCollection nameValueCollection = new NameValueCollection();
			foreach (string str2 in value.SplitHeaderValue(new char[] { ',' }))
			{
				int num = str2.IndexOf('=');
				if (num > 0)
				{
					str = str2.Substring(0, num).Trim();
				}
				else
				{
					str = null;
				}
				string str3 = str;
				if (num < 0)
				{
					str1 = str2.Trim().Trim(new char[] { '\"' });
				}
				else
				{
					str1 = (num < str2.Length - 1 ? str2.Substring(num + 1).Trim().Trim(new char[] { '\"' }) : string.Empty);
				}
				nameValueCollection.Add(str3, str1);
			}
			return nameValueCollection;
		}

		internal abstract string ToBasicString();

		internal abstract string ToDigestString();

		public override string ToString()
		{
			string basicString;
			if (this._scheme == AuthenticationSchemes.Basic)
			{
				basicString = this.ToBasicString();
			}
			else
			{
				basicString = (this._scheme == AuthenticationSchemes.Digest ? this.ToDigestString() : string.Empty);
			}
			return basicString;
		}
	}
}