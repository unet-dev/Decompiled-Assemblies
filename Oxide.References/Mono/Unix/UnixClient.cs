using System;
using System.IO;
using System.Net.Sockets;

namespace Mono.Unix
{
	public class UnixClient : MarshalByRefObject, IDisposable
	{
		private NetworkStream stream;

		private Socket client;

		private bool disposed;

		public Socket Client
		{
			get
			{
				return this.client;
			}
			set
			{
				this.client = value;
				this.stream = null;
			}
		}

		public LingerOption LingerState
		{
			get
			{
				this.CheckDisposed();
				return (LingerOption)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger);
			}
			set
			{
				this.CheckDisposed();
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, value);
			}
		}

		public PeerCred PeerCredential
		{
			get
			{
				this.CheckDisposed();
				return new PeerCred(this.client);
			}
		}

		public int ReceiveBufferSize
		{
			get
			{
				this.CheckDisposed();
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer);
			}
			set
			{
				this.CheckDisposed();
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value);
			}
		}

		public int ReceiveTimeout
		{
			get
			{
				this.CheckDisposed();
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
			}
			set
			{
				this.CheckDisposed();
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
			}
		}

		public int SendBufferSize
		{
			get
			{
				this.CheckDisposed();
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer);
			}
			set
			{
				this.CheckDisposed();
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value);
			}
		}

		public int SendTimeout
		{
			get
			{
				this.CheckDisposed();
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
			}
			set
			{
				this.CheckDisposed();
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);
			}
		}

		public UnixClient()
		{
			if (this.client != null)
			{
				this.client.Close();
				this.client = null;
			}
			this.client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
		}

		public UnixClient(string path) : this()
		{
			if (path == null)
			{
				throw new ArgumentNullException("ep");
			}
			this.Connect(path);
		}

		public UnixClient(UnixEndPoint ep) : this()
		{
			if (ep == null)
			{
				throw new ArgumentNullException("ep");
			}
			this.Connect(ep);
		}

		internal UnixClient(Socket sock)
		{
			this.Client = sock;
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		public void Close()
		{
			this.CheckDisposed();
			this.Dispose();
		}

		public void Connect(UnixEndPoint remoteEndPoint)
		{
			this.CheckDisposed();
			this.client.Connect(remoteEndPoint);
			this.stream = new NetworkStream(this.client, true);
		}

		public void Connect(string path)
		{
			this.CheckDisposed();
			this.Connect(new UnixEndPoint(path));
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing)
			{
				NetworkStream networkStream = this.stream;
				this.stream = null;
				if (networkStream != null)
				{
					networkStream.Close();
					networkStream = null;
				}
				else if (this.client != null)
				{
					this.client.Close();
				}
				this.client = null;
			}
			this.disposed = true;
		}

		protected override void Finalize()
		{
			try
			{
				this.Dispose(false);
			}
			finally
			{
				base.Finalize();
			}
		}

		public NetworkStream GetStream()
		{
			this.CheckDisposed();
			if (this.stream == null)
			{
				this.stream = new NetworkStream(this.client, true);
			}
			return this.stream;
		}
	}
}