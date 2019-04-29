using System;
using System.Collections;
using System.Runtime.Remoting.Channels;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixBinaryClientFormatterSinkProvider : IClientChannelSinkProvider, IClientFormatterSinkProvider
	{
		private IClientChannelSinkProvider next;

		private UnixBinaryCore _binaryCore;

		private static string[] allowedProperties;

		public IClientChannelSinkProvider Next
		{
			get
			{
				return this.next;
			}
			set
			{
				this.next = value;
			}
		}

		static UnixBinaryClientFormatterSinkProvider()
		{
			UnixBinaryClientFormatterSinkProvider.allowedProperties = new string[] { "includeVersions", "strictBinding" };
		}

		public UnixBinaryClientFormatterSinkProvider()
		{
			this._binaryCore = UnixBinaryCore.DefaultInstance;
		}

		public UnixBinaryClientFormatterSinkProvider(IDictionary properties, ICollection providerData)
		{
			this._binaryCore = new UnixBinaryCore(this, properties, UnixBinaryClientFormatterSinkProvider.allowedProperties);
		}

		public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
		{
			IClientChannelSink clientChannelSink = null;
			if (this.next != null)
			{
				clientChannelSink = this.next.CreateSink(channel, url, remoteChannelData);
			}
			return new UnixBinaryClientFormatterSink(clientChannelSink)
			{
				BinaryCore = this._binaryCore
			};
		}
	}
}