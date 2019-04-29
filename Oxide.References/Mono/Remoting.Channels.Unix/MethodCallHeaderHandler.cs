using System;
using System.Runtime.Remoting.Messaging;

namespace Mono.Remoting.Channels.Unix
{
	internal class MethodCallHeaderHandler
	{
		private string _uri;

		public MethodCallHeaderHandler(string uri)
		{
			this._uri = uri;
		}

		public object HandleHeaders(Header[] headers)
		{
			return this._uri;
		}
	}
}