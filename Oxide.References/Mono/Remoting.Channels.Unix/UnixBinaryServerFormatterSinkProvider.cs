using System;
using System.Collections;
using System.Runtime.Remoting.Channels;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixBinaryServerFormatterSinkProvider : IServerChannelSinkProvider, IServerFormatterSinkProvider
	{
		private IServerChannelSinkProvider next;

		private UnixBinaryCore _binaryCore;

		internal static string[] AllowedProperties;

		public IServerChannelSinkProvider Next
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

		static UnixBinaryServerFormatterSinkProvider()
		{
			UnixBinaryServerFormatterSinkProvider.AllowedProperties = new string[] { "includeVersions", "strictBinding" };
		}

		public UnixBinaryServerFormatterSinkProvider()
		{
			this._binaryCore = UnixBinaryCore.DefaultInstance;
		}

		public UnixBinaryServerFormatterSinkProvider(IDictionary properties, ICollection providerData)
		{
			this._binaryCore = new UnixBinaryCore(this, properties, UnixBinaryServerFormatterSinkProvider.AllowedProperties);
		}

		public IServerChannelSink CreateSink(IChannelReceiver channel)
		{
			IServerChannelSink serverChannelSink = null;
			if (this.next != null)
			{
				serverChannelSink = this.next.CreateSink(channel);
			}
			return new UnixBinaryServerFormatterSink(serverChannelSink, channel)
			{
				BinaryCore = this._binaryCore
			};
		}

		public void GetChannelData(IChannelDataStore channelData)
		{
		}
	}
}