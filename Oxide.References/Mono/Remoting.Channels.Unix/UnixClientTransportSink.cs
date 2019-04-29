using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixClientTransportSink : IClientChannelSink, IChannelSinkBase
	{
		private string _path;

		public IClientChannelSink NextChannelSink
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

		public UnixClientTransportSink(string url)
		{
			string str;
			this._path = UnixChannel.ParseUnixURL(url, out str);
		}

		public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, Stream requestStream)
		{
			UnixConnection connection = null;
			bool flag = RemotingServices.IsOneWay(((IMethodMessage)msg).MethodBase);
			try
			{
				if (headers == null)
				{
					headers = new TransportHeaders();
				}
				headers["__RequestUri"] = ((IMethodMessage)msg).Uri;
				connection = UnixConnectionPool.GetConnection(this._path);
				UnixMessageIO.SendMessageStream(connection.Stream, requestStream, headers, connection.Buffer);
				connection.Stream.Flush();
				if (flag)
				{
					connection.Release();
				}
				else
				{
					sinkStack.Push(this, connection);
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReadAsyncUnixMessage), sinkStack);
				}
			}
			catch
			{
				if (connection != null)
				{
					connection.Release();
				}
				if (!flag)
				{
					throw;
				}
			}
		}

		public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream)
		{
			throw new NotSupportedException();
		}

		public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
		{
			return null;
		}

		public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			UnixConnection connection = null;
			try
			{
				if (requestHeaders == null)
				{
					requestHeaders = new TransportHeaders();
				}
				requestHeaders["__RequestUri"] = ((IMethodMessage)msg).Uri;
				connection = UnixConnectionPool.GetConnection(this._path);
				UnixMessageIO.SendMessageStream(connection.Stream, requestStream, requestHeaders, connection.Buffer);
				connection.Stream.Flush();
				if (UnixMessageIO.ReceiveMessageStatus(connection.Stream, connection.Buffer) != MessageStatus.MethodMessage)
				{
					throw new RemotingException("Unknown response message from server");
				}
				responseStream = UnixMessageIO.ReceiveMessageStream(connection.Stream, out responseHeaders, connection.Buffer);
			}
			finally
			{
				if (connection != null)
				{
					connection.Release();
				}
			}
		}

		private void ReadAsyncUnixMessage(object data)
		{
			ITransportHeaders transportHeader;
			IClientChannelSinkStack clientChannelSinkStack = (IClientChannelSinkStack)data;
			UnixConnection unixConnection = (UnixConnection)clientChannelSinkStack.Pop(this);
			try
			{
				if (UnixMessageIO.ReceiveMessageStatus(unixConnection.Stream, unixConnection.Buffer) != MessageStatus.MethodMessage)
				{
					throw new RemotingException("Unknown response message from server");
				}
				Stream stream = UnixMessageIO.ReceiveMessageStream(unixConnection.Stream, out transportHeader, unixConnection.Buffer);
				unixConnection.Release();
				unixConnection = null;
				clientChannelSinkStack.AsyncProcessResponse(transportHeader, stream);
			}
			catch
			{
				if (unixConnection != null)
				{
					unixConnection.Release();
				}
				throw;
			}
		}
	}
}