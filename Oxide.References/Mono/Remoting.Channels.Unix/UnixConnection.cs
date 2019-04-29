using Mono.Unix;
using System;
using System.IO;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixConnection
	{
		private DateTime _controlTime;

		private System.IO.Stream _stream;

		private ReusableUnixClient _client;

		private HostConnectionPool _pool;

		private byte[] _buffer;

		public byte[] Buffer
		{
			get
			{
				return this._buffer;
			}
		}

		public DateTime ControlTime
		{
			get
			{
				return this._controlTime;
			}
			set
			{
				this._controlTime = value;
			}
		}

		public bool IsAlive
		{
			get
			{
				return this._client.IsAlive;
			}
		}

		public System.IO.Stream Stream
		{
			get
			{
				return this._stream;
			}
		}

		public UnixConnection(HostConnectionPool pool, ReusableUnixClient client)
		{
			this._pool = pool;
			this._client = client;
			this._stream = new BufferedStream(client.GetStream());
			this._controlTime = DateTime.Now;
			this._buffer = new byte[UnixMessageIO.DefaultStreamBufferSize];
		}

		public void Close()
		{
			this._client.Close();
		}

		public void Release()
		{
			this._pool.ReleaseConnection(this);
		}
	}
}