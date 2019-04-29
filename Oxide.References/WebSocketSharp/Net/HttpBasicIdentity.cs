using System;
using System.Security.Principal;

namespace WebSocketSharp.Net
{
	public class HttpBasicIdentity : GenericIdentity
	{
		private string _password;

		public virtual string Password
		{
			get
			{
				return this._password;
			}
		}

		internal HttpBasicIdentity(string username, string password) : base(username, "Basic")
		{
			this._password = password;
		}
	}
}