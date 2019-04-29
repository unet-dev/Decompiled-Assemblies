using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace Mono.Remoting.Channels.Unix
{
	public class UnixChannel : IChannelSender, IChannel, IChannelReceiver
	{
		private UnixClientChannel _clientChannel;

		private UnixServerChannel _serverChannel;

		private string _name = "unix";

		private int _priority = 1;

		public object ChannelData
		{
			get
			{
				if (this._serverChannel == null)
				{
					return null;
				}
				return this._serverChannel.ChannelData;
			}
		}

		public string ChannelName
		{
			get
			{
				return this._name;
			}
		}

		public int ChannelPriority
		{
			get
			{
				return this._priority;
			}
		}

		public UnixChannel() : this(null)
		{
		}

		public UnixChannel(string path)
		{
			Hashtable hashtables = new Hashtable();
			hashtables["path"] = path;
			this.Init(hashtables, null, null);
		}

		public UnixChannel(IDictionary properties, IClientChannelSinkProvider clientSinkProvider, IServerChannelSinkProvider serverSinkProvider)
		{
			this.Init(properties, clientSinkProvider, serverSinkProvider);
		}

		public IMessageSink CreateMessageSink(string url, object remoteChannelData, out string objectURI)
		{
			return this._clientChannel.CreateMessageSink(url, remoteChannelData, out objectURI);
		}

		public string[] GetUrlsForUri(string uri)
		{
			if (this._serverChannel == null)
			{
				return null;
			}
			return this._serverChannel.GetUrlsForUri(uri);
		}

		private void Init(IDictionary properties, IClientChannelSinkProvider clientSink, IServerChannelSinkProvider serverSink)
		{
			this._clientChannel = new UnixClientChannel(properties, clientSink);
			if (properties["path"] != null)
			{
				this._serverChannel = new UnixServerChannel(properties, serverSink);
			}
			object item = properties["name"];
			if (item != null)
			{
				this._name = item as string;
			}
			item = properties["priority"];
			if (item != null)
			{
				this._priority = Convert.ToInt32(item);
			}
		}

		public string Parse(string url, out string objectURI)
		{
			return UnixChannel.ParseUnixURL(url, out objectURI);
		}

		internal static string ParseUnixURL(string url, out string objectURI)
		{
			objectURI = null;
			if (!url.StartsWith("unix://"))
			{
				return null;
			}
			int num = url.IndexOf('?');
			if (num == -1)
			{
				return url.Substring(7);
			}
			objectURI = url.Substring(num + 1);
			if (objectURI.Length == 0)
			{
				objectURI = null;
			}
			return url.Substring(7, num - 7);
		}

		public void StartListening(object data)
		{
			if (this._serverChannel != null)
			{
				this._serverChannel.StartListening(data);
			}
		}

		public void StopListening(object data)
		{
			if (this._serverChannel != null)
			{
				this._serverChannel.StopListening(data);
			}
		}
	}
}