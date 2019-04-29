using System;

namespace WebSocketSharp.Net
{
	internal sealed class HttpListenerPrefix
	{
		private string _host;

		private HttpListener _listener;

		private string _original;

		private string _path;

		private string _port;

		private string _prefix;

		private bool _secure;

		public string Host
		{
			get
			{
				return this._host;
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public HttpListener Listener
		{
			get
			{
				return this._listener;
			}
			set
			{
				this._listener = value;
			}
		}

		public string Original
		{
			get
			{
				return this._original;
			}
		}

		public string Path
		{
			get
			{
				return this._path;
			}
		}

		public string Port
		{
			get
			{
				return this._port;
			}
		}

		internal HttpListenerPrefix(string uriPrefix)
		{
			this._original = uriPrefix;
			this.parse(uriPrefix);
		}

		public static void CheckPrefix(string uriPrefix)
		{
			if (uriPrefix == null)
			{
				throw new ArgumentNullException("uriPrefix");
			}
			int length = uriPrefix.Length;
			if (length == 0)
			{
				throw new ArgumentException("An empty string.", "uriPrefix");
			}
			if ((uriPrefix.StartsWith("http://") ? false : !uriPrefix.StartsWith("https://")))
			{
				throw new ArgumentException("The scheme isn't 'http' or 'https'.", "uriPrefix");
			}
			int num = uriPrefix.IndexOf(':') + 3;
			if (num >= length)
			{
				throw new ArgumentException("No host is specified.", "uriPrefix");
			}
			if (uriPrefix[num] == ':')
			{
				throw new ArgumentException("No host is specified.", "uriPrefix");
			}
			int num1 = uriPrefix.IndexOf('/', num, length - num);
			if (num1 == num)
			{
				throw new ArgumentException("No host is specified.", "uriPrefix");
			}
			if ((num1 == -1 ? true : uriPrefix[length - 1] != '/'))
			{
				throw new ArgumentException("Ends without '/'.", "uriPrefix");
			}
			if (uriPrefix[num1 - 1] == ':')
			{
				throw new ArgumentException("No port is specified.", "uriPrefix");
			}
			if (num1 == length - 2)
			{
				throw new ArgumentException("No path is specified.", "uriPrefix");
			}
		}

		public override bool Equals(object obj)
		{
			HttpListenerPrefix httpListenerPrefix = obj as HttpListenerPrefix;
			return (httpListenerPrefix == null ? false : httpListenerPrefix._prefix == this._prefix);
		}

		public override int GetHashCode()
		{
			return this._prefix.GetHashCode();
		}

		private void parse(string uriPrefix)
		{
			if (uriPrefix.StartsWith("https"))
			{
				this._secure = true;
			}
			int length = uriPrefix.Length;
			int num = uriPrefix.IndexOf(':') + 3;
			int num1 = uriPrefix.IndexOf('/', num + 1, length - num - 1);
			int num2 = uriPrefix.LastIndexOf(':', num1 - 1, num1 - num - 1);
			if ((uriPrefix[num1 - 1] == ']' ? true : num2 <= num))
			{
				this._host = uriPrefix.Substring(num, num1 - num);
				this._port = (this._secure ? "443" : "80");
			}
			else
			{
				this._host = uriPrefix.Substring(num, num2 - num);
				this._port = uriPrefix.Substring(num2 + 1, num1 - num2 - 1);
			}
			this._path = uriPrefix.Substring(num1);
			this._prefix = string.Format("http{0}://{1}:{2}{3}", new object[] { (this._secure ? "s" : ""), this._host, this._port, this._path });
		}

		public override string ToString()
		{
			return this._prefix;
		}
	}
}