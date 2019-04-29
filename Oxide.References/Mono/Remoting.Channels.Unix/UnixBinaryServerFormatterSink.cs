using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixBinaryServerFormatterSink : IServerChannelSink, IChannelSinkBase
	{
		private UnixBinaryCore _binaryCore = UnixBinaryCore.DefaultInstance;

		private IServerChannelSink next_sink;

		private IChannelReceiver receiver;

		internal UnixBinaryCore BinaryCore
		{
			get
			{
				return this._binaryCore;
			}
			set
			{
				this._binaryCore = value;
			}
		}

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
				return null;
			}
		}

		public UnixBinaryServerFormatterSink(IServerChannelSink nextSink, IChannelReceiver receiver)
		{
			this.next_sink = nextSink;
			this.receiver = receiver;
		}

		public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage message, ITransportHeaders headers, Stream stream)
		{
			ITransportHeaders transportHeader = new TransportHeaders();
			if (sinkStack != null)
			{
				stream = sinkStack.GetResponseStream(message, transportHeader);
			}
			if (stream == null)
			{
				stream = new MemoryStream();
			}
			this._binaryCore.Serializer.Serialize(stream, message, null);
			if (stream is MemoryStream)
			{
				stream.Position = (long)0;
			}
			sinkStack.AsyncProcessResponse(message, transportHeader, stream);
		}

		public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
		{
			return null;
		}

		public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			ServerProcessing serverProcessing;
			string str;
			sinkStack.Push(this, null);
			try
			{
				string item = (string)requestHeaders["__RequestUri"];
				this.receiver.Parse(item, out str);
				if (str == null)
				{
					str = item;
				}
				MethodCallHeaderHandler methodCallHeaderHandler = new MethodCallHeaderHandler(str);
				requestMsg = (IMessage)this._binaryCore.Deserializer.Deserialize(requestStream, new HeaderHandler(methodCallHeaderHandler.HandleHeaders));
				serverProcessing = this.next_sink.ProcessMessage(sinkStack, requestMsg, requestHeaders, null, out responseMsg, out responseHeaders, out responseStream);
			}
			catch (Exception exception)
			{
				responseMsg = new ReturnMessage(exception, (IMethodCallMessage)requestMsg);
				serverProcessing = ServerProcessing.Complete;
				responseHeaders = null;
				responseStream = null;
			}
			if (serverProcessing == ServerProcessing.Complete)
			{
				for (int i = 0; i < 3; i++)
				{
					responseStream = null;
					responseHeaders = new TransportHeaders();
					if (sinkStack != null)
					{
						responseStream = sinkStack.GetResponseStream(responseMsg, responseHeaders);
					}
					if (responseStream == null)
					{
						responseStream = new MemoryStream();
					}
					try
					{
						this._binaryCore.Serializer.Serialize(responseStream, responseMsg);
						break;
					}
					catch (Exception exception2)
					{
						Exception exception1 = exception2;
						if (i == 2)
						{
							throw exception1;
						}
						responseMsg = new ReturnMessage(exception1, (IMethodCallMessage)requestMsg);
					}
				}
				if (responseStream is MemoryStream)
				{
					responseStream.Position = (long)0;
				}
				sinkStack.Pop(this);
			}
			return serverProcessing;
		}
	}
}