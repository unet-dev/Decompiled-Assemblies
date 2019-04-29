using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using WebSocketSharp;

namespace WebSocketSharp.Net
{
	internal sealed class EndPointManager
	{
		private readonly static Dictionary<IPEndPoint, EndPointListener> _endpoints;

		static EndPointManager()
		{
			EndPointManager._endpoints = new Dictionary<IPEndPoint, EndPointListener>();
		}

		private EndPointManager()
		{
		}

		public static void AddListener(WebSocketSharp.Net.HttpListener listener)
		{
			List<string> strs = new List<string>();
			object syncRoot = ((ICollection)EndPointManager._endpoints).SyncRoot;
			Monitor.Enter(syncRoot);
			try
			{
				try
				{
					foreach (string prefix in listener.Prefixes)
					{
						EndPointManager.addPrefix(prefix, listener);
						strs.Add(prefix);
					}
				}
				catch
				{
					foreach (string str in strs)
					{
						EndPointManager.removePrefix(str, listener);
					}
					throw;
				}
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}

		private static void addPrefix(string uriPrefix, WebSocketSharp.Net.HttpListener listener)
		{
			int num;
			EndPointListener endPointListener;
			HttpListenerPrefix httpListenerPrefix = new HttpListenerPrefix(uriPrefix);
			IPAddress pAddress = EndPointManager.convertToIPAddress(httpListenerPrefix.Host);
			if (!pAddress.IsLocal())
			{
				throw new WebSocketSharp.Net.HttpListenerException(87, "Includes an invalid host.");
			}
			if (!int.TryParse(httpListenerPrefix.Port, out num))
			{
				throw new WebSocketSharp.Net.HttpListenerException(87, "Includes an invalid port.");
			}
			if (!num.IsPortNumber())
			{
				throw new WebSocketSharp.Net.HttpListenerException(87, "Includes an invalid port.");
			}
			string path = httpListenerPrefix.Path;
			if (path.IndexOf('%') != -1)
			{
				throw new WebSocketSharp.Net.HttpListenerException(87, "Includes an invalid path.");
			}
			if (path.IndexOf("//", StringComparison.Ordinal) != -1)
			{
				throw new WebSocketSharp.Net.HttpListenerException(87, "Includes an invalid path.");
			}
			IPEndPoint pEndPoint = new IPEndPoint(pAddress, num);
			if (!EndPointManager._endpoints.TryGetValue(pEndPoint, out endPointListener))
			{
				endPointListener = new EndPointListener(pEndPoint, httpListenerPrefix.IsSecure, listener.CertificateFolderPath, listener.SslConfiguration, listener.ReuseAddress);
				EndPointManager._endpoints.Add(pEndPoint, endPointListener);
			}
			else if (endPointListener.IsSecure ^ httpListenerPrefix.IsSecure)
			{
				throw new WebSocketSharp.Net.HttpListenerException(87, "Includes an invalid scheme.");
			}
			endPointListener.AddPrefix(httpListenerPrefix, listener);
		}

		public static void AddPrefix(string uriPrefix, WebSocketSharp.Net.HttpListener listener)
		{
			object syncRoot = ((ICollection)EndPointManager._endpoints).SyncRoot;
			Monitor.Enter(syncRoot);
			try
			{
				EndPointManager.addPrefix(uriPrefix, listener);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}

		private static IPAddress convertToIPAddress(string hostname)
		{
			return (hostname == "*" || hostname == "+" ? IPAddress.Any : hostname.ToIPAddress());
		}

		internal static bool RemoveEndPoint(IPEndPoint endpoint)
		{
			EndPointListener endPointListener;
			bool flag;
			object syncRoot = ((ICollection)EndPointManager._endpoints).SyncRoot;
			Monitor.Enter(syncRoot);
			try
			{
				if (EndPointManager._endpoints.TryGetValue(endpoint, out endPointListener))
				{
					EndPointManager._endpoints.Remove(endpoint);
					endPointListener.Close();
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			return flag;
		}

		public static void RemoveListener(WebSocketSharp.Net.HttpListener listener)
		{
			object syncRoot = ((ICollection)EndPointManager._endpoints).SyncRoot;
			Monitor.Enter(syncRoot);
			try
			{
				foreach (string prefix in listener.Prefixes)
				{
					EndPointManager.removePrefix(prefix, listener);
				}
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}

		private static void removePrefix(string uriPrefix, WebSocketSharp.Net.HttpListener listener)
		{
			int num;
			EndPointListener endPointListener;
			HttpListenerPrefix httpListenerPrefix = new HttpListenerPrefix(uriPrefix);
			IPAddress pAddress = EndPointManager.convertToIPAddress(httpListenerPrefix.Host);
			if (pAddress.IsLocal())
			{
				if (int.TryParse(httpListenerPrefix.Port, out num))
				{
					if (num.IsPortNumber())
					{
						string path = httpListenerPrefix.Path;
						if (path.IndexOf('%') == -1)
						{
							if (path.IndexOf("//", StringComparison.Ordinal) == -1)
							{
								IPEndPoint pEndPoint = new IPEndPoint(pAddress, num);
								if (EndPointManager._endpoints.TryGetValue(pEndPoint, out endPointListener))
								{
									if (!(endPointListener.IsSecure ^ httpListenerPrefix.IsSecure))
									{
										endPointListener.RemovePrefix(httpListenerPrefix, listener);
									}
								}
							}
						}
					}
				}
			}
		}

		public static void RemovePrefix(string uriPrefix, WebSocketSharp.Net.HttpListener listener)
		{
			object syncRoot = ((ICollection)EndPointManager._endpoints).SyncRoot;
			Monitor.Enter(syncRoot);
			try
			{
				EndPointManager.removePrefix(uriPrefix, listener);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}
	}
}