using System;
using System.Runtime.Remoting.Channels;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixClientTransportSinkProvider : IClientChannelSinkProvider
	{
		public IClientChannelSinkProvider Next
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public UnixClientTransportSinkProvider()
		{
		}

		public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
		{
			return new UnixClientTransportSink(url);
		}
	}
}