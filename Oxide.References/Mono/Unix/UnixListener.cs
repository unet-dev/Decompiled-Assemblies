using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Mono.Unix
{
	public class UnixListener : MarshalByRefObject, IDisposable
	{
		private bool disposed;

		private bool listening;

		private Socket server;

		private EndPoint savedEP;

		public EndPoint LocalEndpoint
		{
			get
			{
				return this.savedEP;
			}
		}

		protected Socket Server
		{
			get
			{
				return this.server;
			}
		}

		public UnixListener(string path)
		{
			if (!Directory.Exists(Path.GetDirectoryName(path)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			}
			this.Init(new UnixEndPoint(path));
		}

		public UnixListener(UnixEndPoint localEndPoint)
		{
			if (localEndPoint == null)
			{
				throw new ArgumentNullException("localendPoint");
			}
			this.Init(localEndPoint);
		}

		public Socket AcceptSocket()
		{
			this.CheckDisposed();
			if (!this.listening)
			{
				throw new InvalidOperationException("Socket is not listening");
			}
			return this.server.Accept();
		}

		public UnixClient AcceptUnixClient()
		{
			this.CheckDisposed();
			if (!this.listening)
			{
				throw new InvalidOperationException("Socket is not listening");
			}
			return new UnixClient(this.AcceptSocket());
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing)
			{
				if (this.server != null)
				{
					this.server.Close();
				}
				this.server = null;
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

		private void Init(UnixEndPoint ep)
		{
			this.listening = false;
			string filename = ep.Filename;
			if (File.Exists(filename))
			{
				Socket socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
				try
				{
					socket.Connect(ep);
					socket.Close();
					throw new InvalidOperationException(string.Concat("There's already a server listening on ", filename));
				}
				catch (SocketException socketException)
				{
				}
				File.Delete(filename);
			}
			this.server = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
			this.server.Bind(ep);
			this.savedEP = this.server.LocalEndPoint;
		}

		public bool Pending()
		{
			this.CheckDisposed();
			if (!this.listening)
			{
				throw new InvalidOperationException("Socket is not listening");
			}
			return this.server.Poll(1000, SelectMode.SelectRead);
		}

		public void Start()
		{
			this.Start(5);
		}

		public void Start(int backlog)
		{
			this.CheckDisposed();
			if (this.listening)
			{
				return;
			}
			this.server.Listen(backlog);
			this.listening = true;
		}

		public void Stop()
		{
			this.CheckDisposed();
			this.Dispose(true);
		}
	}
}