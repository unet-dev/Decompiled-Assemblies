using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixBinaryClientFormatterSink : IClientChannelSink, IMessageSink, IChannelSinkBase, IClientFormatterSink
	{
		private UnixBinaryCore _binaryCore = UnixBinaryCore.DefaultInstance;

		private IClientChannelSink _nextInChain;

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

		public IClientChannelSink NextChannelSink
		{
			get
			{
				return this._nextInChain;
			}
		}

		public IMessageSink NextSink
		{
			get
			{
				return null;
			}
		}

		public IDictionary Properties
		{
			get
			{
				return null;
			}
		}

		public UnixBinaryClientFormatterSink(IClientChannelSink nextSink)
		{
			this._nextInChain = nextSink;
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			ITransportHeaders transportHeader = new TransportHeaders();
			Stream requestStream = this._nextInChain.GetRequestStream(msg, transportHeader);
			if (requestStream == null)
			{
				requestStream = new MemoryStream();
			}
			this._binaryCore.Serializer.Serialize(requestStream, msg, null);
			if (requestStream is MemoryStream)
			{
				requestStream.Position = (long)0;
			}
			ClientChannelSinkStack clientChannelSinkStack = new ClientChannelSinkStack(replySink);
			clientChannelSinkStack.Push(this, msg);
			this._nextInChain.AsyncProcessRequest(clientChannelSinkStack, msg, transportHeader, requestStream);
			return null;
		}

		public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			throw new NotSupportedException("UnixBinaryClientFormatterSink must be the first sink in the IClientChannelSink chain");
		}

		public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream)
		{
			IMessage message = (IMessage)this._binaryCore.Deserializer.DeserializeMethodResponse(stream, null, (IMethodCallMessage)state);
			sinkStack.DispatchReplyMessage(message);
		}

		public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
		{
			throw new NotSupportedException();
		}

		public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			responseHeaders = null;
			responseStream = null;
			throw new NotSupportedException();
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			Stream stream;
			ITransportHeaders transportHeader;
			IMessage returnMessage;
			try
			{
				ITransportHeaders uri = new TransportHeaders();
				uri["__RequestUri"] = ((IMethodCallMessage)msg).Uri;
				uri["Content-Type"] = "application/octet-stream";
				Stream requestStream = this._nextInChain.GetRequestStream(msg, uri);
				if (requestStream == null)
				{
					requestStream = new MemoryStream();
				}
				this._binaryCore.Serializer.Serialize(requestStream, msg, null);
				if (requestStream is MemoryStream)
				{
					requestStream.Position = (long)0;
				}
				this._nextInChain.ProcessMessage(msg, uri, requestStream, out transportHeader, out stream);
				returnMessage = (IMessage)this._binaryCore.Deserializer.DeserializeMethodResponse(stream, null, (IMethodCallMessage)msg);
			}
			catch (Exception exception)
			{
				returnMessage = new ReturnMessage(exception, (IMethodCallMessage)msg);
			}
			return returnMessage;
		}
	}
}