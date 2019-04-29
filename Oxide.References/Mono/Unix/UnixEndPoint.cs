using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mono.Unix
{
	[Serializable]
	public class UnixEndPoint : EndPoint
	{
		private string filename;

		public override System.Net.Sockets.AddressFamily AddressFamily
		{
			get
			{
				return System.Net.Sockets.AddressFamily.Unix;
			}
		}

		public string Filename
		{
			get
			{
				return this.filename;
			}
			set
			{
				this.filename = value;
			}
		}

		public UnixEndPoint(string filename)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (filename == string.Empty)
			{
				throw new ArgumentException("Cannot be empty.", "filename");
			}
			this.filename = filename;
		}

		public override EndPoint Create(SocketAddress socketAddress)
		{
			byte[] item = new byte[socketAddress.Size - 2 - 1];
			for (int i = 0; i < (int)item.Length; i++)
			{
				item[i] = socketAddress[i + 2];
			}
			return new UnixEndPoint(Encoding.Default.GetString(item));
		}

		public override bool Equals(object o)
		{
			UnixEndPoint unixEndPoint = o as UnixEndPoint;
			if (unixEndPoint == null)
			{
				return false;
			}
			return unixEndPoint.filename == this.filename;
		}

		public override int GetHashCode()
		{
			return this.filename.GetHashCode();
		}

		public override SocketAddress Serialize()
		{
			byte[] bytes = Encoding.Default.GetBytes(this.filename);
			SocketAddress socketAddress = new SocketAddress(this.AddressFamily, 2 + (int)bytes.Length + 1);
			for (int i = 0; i < (int)bytes.Length; i++)
			{
				socketAddress[2 + i] = bytes[i];
			}
			socketAddress[2 + (int)bytes.Length] = 0;
			return socketAddress;
		}

		public override string ToString()
		{
			return this.filename;
		}
	}
}