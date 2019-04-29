using System;
using WebSocketSharp.Net;

namespace WebSocketSharp.Server
{
	public class HttpRequestEventArgs : EventArgs
	{
		private HttpListenerRequest _request;

		private HttpListenerResponse _response;

		public HttpListenerRequest Request
		{
			get
			{
				return this._request;
			}
		}

		public HttpListenerResponse Response
		{
			get
			{
				return this._response;
			}
		}

		internal HttpRequestEventArgs(HttpListenerContext context)
		{
			this._request = context.Request;
			this._response = context.Response;
		}
	}
}