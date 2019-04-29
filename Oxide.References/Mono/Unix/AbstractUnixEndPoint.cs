using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mono.Unix
{
	[Serializable]
	public class AbstractUnixEndPoint : EndPoint
	{
		private string path;

		public override System.Net.Sockets.AddressFamily AddressFamily
		{
			get
			{
				return System.Net.Sockets.AddressFamily.Unix;
			}
		}

		public string Path
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}

		public AbstractUnixEndPoint(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path == string.Empty)
			{
				throw new ArgumentException("Cannot be empty.", "path");
			}
			this.path = path;
		}

		public override EndPoint Create(SocketAddress socketAddress)
		{
			byte[] item = new byte[socketAddress.Size - 2 - 1];
			for (int i = 0; i < (int)item.Length; i++)
			{
				item[i] = socketAddress[3 + i];
			}
			return new AbstractUnixEndPoint(Encoding.Default.GetString(item));
		}

		public override bool Equals(object o)
		{
			AbstractUnixEndPoint abstractUnixEndPoint = o as AbstractUnixEndPoint;
			if (abstractUnixEndPoint == null)
			{
				return false;
			}
			return abstractUnixEndPoint.path == this.path;
		}

		public override int GetHashCode()
		{
			return this.path.GetHashCode();
		}

		public override SocketAddress Serialize()
		{
			byte[] bytes = Encoding.Default.GetBytes(this.path);
			SocketAddress socketAddress = new SocketAddress(this.AddressFamily, 3 + (int)bytes.Length);
			socketAddress[2] = 0;
			for (int i = 0; i < (int)bytes.Length; i++)
			{
				socketAddress[i + 2 + 1] = bytes[i];
			}
			return socketAddress;
		}

		public override string ToString()
		{
			return this.path;
		}
	}
}