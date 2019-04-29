using System;
using System.Collections.Specialized;
using System.Text;

namespace WebSocketSharp.Net
{
	internal class AuthenticationChallenge : AuthenticationBase
	{
		public string Domain
		{
			get
			{
				return this.Parameters["domain"];
			}
		}

		public string Stale
		{
			get
			{
				return this.Parameters["stale"];
			}
		}

		private AuthenticationChallenge(AuthenticationSchemes scheme, NameValueCollection parameters) : base(scheme, parameters)
		{
		}

		internal AuthenticationChallenge(AuthenticationSchemes scheme, string realm) : base(scheme, new NameValueCollection())
		{
			this.Parameters["realm"] = realm;
			if (scheme == AuthenticationSchemes.Digest)
			{
				this.Parameters["nonce"] = AuthenticationBase.CreateNonceValue();
				this.Parameters["algorithm"] = "MD5";
				this.Parameters["qop"] = "auth";
			}
		}

		internal static AuthenticationChallenge CreateBasicChallenge(string realm)
		{
			return new AuthenticationChallenge(AuthenticationSchemes.Basic, realm);
		}

		internal static AuthenticationChallenge CreateDigestChallenge(string realm)
		{
			return new AuthenticationChallenge(AuthenticationSchemes.Digest, realm);
		}

		internal static AuthenticationChallenge Parse(string value)
		{
			AuthenticationChallenge authenticationChallenge;
			AuthenticationChallenge authenticationChallenge1;
			string[] strArrays = value.Split(new char[] { ' ' }, 2);
			if ((int)strArrays.Length == 2)
			{
				string lower = strArrays[0].ToLower();
				if (lower == "basic")
				{
					authenticationChallenge1 = new AuthenticationChallenge(AuthenticationSchemes.Basic, AuthenticationBase.ParseParameters(strArrays[1]));
				}
				else if (lower == "digest")
				{
					authenticationChallenge1 = new AuthenticationChallenge(AuthenticationSchemes.Digest, AuthenticationBase.ParseParameters(strArrays[1]));
				}
				else
				{
					authenticationChallenge1 = null;
				}
				authenticationChallenge = authenticationChallenge1;
			}
			else
			{
				authenticationChallenge = null;
			}
			return authenticationChallenge;
		}

		internal override string ToBasicString()
		{
			return string.Format("Basic realm=\"{0}\"", this.Parameters["realm"]);
		}

		internal override string ToDigestString()
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			string item = this.Parameters["domain"];
			if (item == null)
			{
				stringBuilder.AppendFormat("Digest realm=\"{0}\", nonce=\"{1}\"", this.Parameters["realm"], this.Parameters["nonce"]);
			}
			else
			{
				stringBuilder.AppendFormat("Digest realm=\"{0}\", domain=\"{1}\", nonce=\"{2}\"", this.Parameters["realm"], item, this.Parameters["nonce"]);
			}
			string str = this.Parameters["opaque"];
			if (str != null)
			{
				stringBuilder.AppendFormat(", opaque=\"{0}\"", str);
			}
			string item1 = this.Parameters["stale"];
			if (item1 != null)
			{
				stringBuilder.AppendFormat(", stale={0}", item1);
			}
			string str1 = this.Parameters["algorithm"];
			if (str1 != null)
			{
				stringBuilder.AppendFormat(", algorithm={0}", str1);
			}
			string item2 = this.Parameters["qop"];
			if (item2 != null)
			{
				stringBuilder.AppendFormat(", qop=\"{0}\"", item2);
			}
			return stringBuilder.ToString();
		}
	}
}