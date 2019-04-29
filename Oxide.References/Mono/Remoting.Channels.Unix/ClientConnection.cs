using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Mono.Remoting.Channels.Unix
{
	internal class ClientConnection
	{
		private Socket _client;

		private UnixServerTransportSink _sink;

		private Stream _stream;

		private UnixServerChannel _serverChannel;

		private byte[] _buffer = new byte[UnixMessageIO.DefaultStreamBufferSize];

		public byte[] Buffer
		{
			get
			{
				return this._buffer;
			}
		}

		public Socket Client
		{
			get
			{
				return this._client;
			}
		}

		public bool IsLocal
		{
			get
			{
				return true;
			}
		}

		public ClientConnection(UnixServerChannel serverChannel, Socket client, UnixServerTransportSink sink)
		{
			this._serverChannel = serverChannel;
			this._client = client;
			this._sink = sink;
		}

		public void ProcessMessages()
		{
			byte[] numArray = new byte[256];
			this._stream = new BufferedStream(new NetworkStream(this._client));
			try
			{
				try
				{
					bool flag = false;
					while (!flag)
					{
						MessageStatus messageStatu = UnixMessageIO.ReceiveMessageStatus(this._stream, numArray);
						if (messageStatu == MessageStatus.MethodMessage)
						{
							this._sink.InternalProcessMessage(this, this._stream);
						}
						else if (messageStatu == MessageStatus.CancelSignal || messageStatu == MessageStatus.Unknown)
						{
							flag = true;
						}
					}
				}
				catch (Exception exception)
				{
				}
			}
			finally
			{
				this._stream.Close();
				this._client.Close();
				this._serverChannel.ReleaseConnection(Thread.CurrentThread);
			}
		}
	}
}