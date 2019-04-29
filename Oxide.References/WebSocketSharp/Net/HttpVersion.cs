using System;

namespace WebSocketSharp.Net
{
	public class HttpVersion
	{
		public readonly static Version Version10;

		public readonly static Version Version11;

		static HttpVersion()
		{
			HttpVersion.Version10 = new Version(1, 0);
			HttpVersion.Version11 = new Version(1, 1);
		}

		public HttpVersion()
		{
		}
	}
}