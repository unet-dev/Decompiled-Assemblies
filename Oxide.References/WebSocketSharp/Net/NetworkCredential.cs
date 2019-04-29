using System;

namespace WebSocketSharp.Net
{
	public class NetworkCredential
	{
		private string _domain;

		private string _password;

		private string[] _roles;

		private string _userName;

		public string Domain
		{
			get
			{
				return this._domain ?? string.Empty;
			}
			internal set
			{
				this._domain = value;
			}
		}

		public string Password
		{
			get
			{
				return this._password ?? string.Empty;
			}
			internal set
			{
				this._password = value;
			}
		}

		public string[] Roles
		{
			get
			{
				string[] strArrays = this._roles;
				if (strArrays == null)
				{
					string[] strArrays1 = new string[0];
					string[] strArrays2 = strArrays1;
					this._roles = strArrays1;
					strArrays = strArrays2;
				}
				return strArrays;
			}
			internal set
			{
				this._roles = value;
			}
		}

		public string UserName
		{
			get
			{
				return this._userName;
			}
			internal set
			{
				this._userName = value;
			}
		}

		public NetworkCredential(string userName, string password) : this(userName, password, null, null)
		{
		}

		public NetworkCredential(string userName, string password, string domain, params string[] roles)
		{
			if (userName == null)
			{
				throw new ArgumentNullException("userName");
			}
			if (userName.Length == 0)
			{
				throw new ArgumentException("An empty string.", "userName");
			}
			this._userName = userName;
			this._password = password;
			this._domain = domain;
			this._roles = roles;
		}
	}
}