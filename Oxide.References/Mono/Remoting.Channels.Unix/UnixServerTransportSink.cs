using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixServerTransportSink : IServerChannelSink, IChannelSinkBase
	{
		private IServerChannelSink next_sink;

		public IServerChannelSink NextChannelSink
		{
			get
			{
				return this.next_sink;
			}
		}

		public IDictionary Properties
		{
			get
			{
				if (this.next_sink == null)
				{
					return null;
				}
				return this.next_sink.Properties;
			}
		}

		public UnixServerTransportSink(IServerChannelSink next)
		{
			this.next_sink = next;
		}

		public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream responseStream)
		{
			ClientConnection clientConnection = (ClientConnection)state;
			NetworkStream networkStream = new NetworkStream(clientConnection.Client);
			UnixMessageIO.SendMessageStream(networkStream, responseStream, headers, clientConnection.Buffer);
			networkStream.Flush();
			networkStream.Close();
		}

		public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
		{
			return null;
		}

		internal void InternalProcessMessage(ClientConnection connection, Stream stream)
		{
			ITransportHeaders transportHeader;
			ITransportHeaders transportHeader1;
			Stream stream1;
			IMessage message;
			Stream stream2 = UnixMessageIO.ReceiveMessageStream(stream, out transportHeader, connection.Buffer);
			ServerChannelSinkStack serverChannelSinkStack = new ServerChannelSinkStack();
			serverChannelSinkStack.Push(this, connection);
			switch (this.next_sink.ProcessMessage(serverChannelSinkStack, null, transportHeader, stream2, out message, out transportHeader1, out stream1))
			{
				case ServerProcessing.Complete:
				{
					UnixMessageIO.SendMessageStream(stream, stream1, transportHeader1, connection.Buffer);
					stream.Flush();
					break;
				}
			}
		}

		public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			responseMsg = null;
			responseHeaders = null;
			responseStream = null;
			throw new NotSupportedException();
		}
	}
}