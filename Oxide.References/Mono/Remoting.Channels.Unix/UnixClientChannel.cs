using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace Mono.Remoting.Channels.Unix
{
	public class UnixClientChannel : IChannelSender, IChannel
	{
		private int priority = 1;

		private string name = "unix";

		private IClientChannelSinkProvider _sinkProvider;

		public string ChannelName
		{
			get
			{
				return this.name;
			}
		}

		public int ChannelPriority
		{
			get
			{
				return this.priority;
			}
		}

		public UnixClientChannel()
		{
			this._sinkProvider = new UnixBinaryClientFormatterSinkProvider()
			{
				Next = new UnixClientTransportSinkProvider()
			};
		}

		public UnixClientChannel(IDictionary properties, IClientChannelSinkProvider sinkProvider)
		{
			object item = properties["name"];
			if (item != null)
			{
				this.name = item as string;
			}
			item = properties["priority"];
			if (item != null)
			{
				this.priority = Convert.ToInt32(item);
			}
			if (sinkProvider == null)
			{
				this._sinkProvider = new UnixBinaryClientFormatterSinkProvider()
				{
					Next = new UnixClientTransportSinkProvider()
				};
			}
			else
			{
				this._sinkProvider = sinkProvider;
				IClientChannelSinkProvider next = sinkProvider;
				while (next.Next != null)
				{
					next = next.Next;
				}
				next.Next = new UnixClientTransportSinkProvider();
			}
		}

		public UnixClientChannel(string name, IClientChannelSinkProvider sinkProvider)
		{
			this.name = name;
			this._sinkProvider = sinkProvider;
			IClientChannelSinkProvider next = sinkProvider;
			while (next.Next != null)
			{
				next = next.Next;
			}
			next.Next = new UnixClientTransportSinkProvider();
		}

		public IMessageSink CreateMessageSink(string url, object remoteChannelData, out string objectURI)
		{
			if (url != null && this.Parse(url, out objectURI) != null)
			{
				return (IMessageSink)this._sinkProvider.CreateSink(this, url, remoteChannelData);
			}
			if (remoteChannelData != null)
			{
				IChannelDataStore channelDataStore = remoteChannelData as IChannelDataStore;
				if (channelDataStore == null || (int)channelDataStore.ChannelUris.Length <= 0)
				{
					objectURI = null;
					return null;
				}
				url = channelDataStore.ChannelUris[0];
			}
			if (this.Parse(url, out objectURI) == null)
			{
				return null;
			}
			return (IMessageSink)this._sinkProvider.CreateSink(this, url, remoteChannelData);
		}

		public string Parse(string url, out string objectURI)
		{
			return UnixChannel.ParseUnixURL(url, out objectURI);
		}
	}
}