using Network;
using System;
using System.IO;

namespace Facepunch.Network.Raknet
{
	internal class StreamRead : Read
	{
		private NetworkPeer net;

		private Peer peer;

		private MemoryStream stream;

		public override bool CanSeek
		{
			get
			{
				return this.stream.CanSeek;
			}
		}

		public override long Length
		{
			get
			{
				return this.stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.stream.Position;
			}
			set
			{
				this.stream.Position = value;
			}
		}

		public StreamRead(NetworkPeer net, Peer peer)
		{
			this.net = net;
			this.peer = peer;
			this.stream = new MemoryStream();
		}

		public override bool Bit()
		{
			if (base.unread < 1)
			{
				return false;
			}
			return this.stream.ReadByte() != 0;
		}

		public override double Double()
		{
			if (base.unread < 8)
			{
				return 0;
			}
			return this.Read64().f;
		}

		public override float Float()
		{
			if (base.unread < 4)
			{
				return 0f;
			}
			return this.Read32().f;
		}

		private byte[] GetReadBuffer()
		{
			return this.stream.GetBuffer();
		}

		private long GetReadOffset(long i)
		{
			long position = this.stream.Position;
			this.stream.Position = position + i;
			return position;
		}

		public override short Int16()
		{
			if (base.unread < 2)
			{
				return 0;
			}
			return this.Read16().i;
		}

		public override int Int32()
		{
			if (base.unread < 4)
			{
				return 0;
			}
			return this.Read32().i;
		}

		public override long Int64()
		{
			if (base.unread < 8)
			{
				return (long)0;
			}
			return this.Read64().i;
		}

		public override sbyte Int8()
		{
			if (base.unread < 1)
			{
				return 0;
			}
			return this.Read8().i;
		}

		public override byte PacketID()
		{
			return this.UInt8();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.stream.Read(buffer, offset, count);
		}

		private Union16 Read16()
		{
			long readOffset = this.GetReadOffset((long)2);
			byte[] readBuffer = this.GetReadBuffer();
			Union16 union16 = new Union16()
			{
				b1 = readBuffer[checked((IntPtr)readOffset)],
				b2 = readBuffer[checked((IntPtr)(readOffset + (long)1))]
			};
			return union16;
		}

		private Union32 Read32()
		{
			long readOffset = this.GetReadOffset((long)4);
			byte[] readBuffer = this.GetReadBuffer();
			Union32 union32 = new Union32()
			{
				b1 = readBuffer[checked((IntPtr)readOffset)],
				b2 = readBuffer[checked((IntPtr)(readOffset + (long)1))],
				b3 = readBuffer[checked((IntPtr)(readOffset + (long)2))],
				b4 = readBuffer[checked((IntPtr)(readOffset + (long)3))]
			};
			return union32;
		}

		private Union64 Read64()
		{
			long readOffset = this.GetReadOffset((long)8);
			byte[] readBuffer = this.GetReadBuffer();
			Union64 union64 = new Union64()
			{
				b1 = readBuffer[checked((IntPtr)readOffset)],
				b2 = readBuffer[checked((IntPtr)(readOffset + (long)1))],
				b3 = readBuffer[checked((IntPtr)(readOffset + (long)2))],
				b4 = readBuffer[checked((IntPtr)(readOffset + (long)3))],
				b5 = readBuffer[checked((IntPtr)(readOffset + (long)4))],
				b6 = readBuffer[checked((IntPtr)(readOffset + (long)5))],
				b7 = readBuffer[checked((IntPtr)(readOffset + (long)6))],
				b8 = readBuffer[checked((IntPtr)(readOffset + (long)7))]
			};
			return union64;
		}

		private Union8 Read8()
		{
			long readOffset = this.GetReadOffset((long)1);
			byte[] readBuffer = this.GetReadBuffer();
			return new Union8()
			{
				b1 = readBuffer[checked((IntPtr)readOffset)]
			};
		}

		public override int ReadByte()
		{
			return this.stream.ReadByte();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.stream.Seek(offset, origin);
		}

		public void Shutdown()
		{
			this.stream.Dispose();
			this.stream = null;
			this.peer = null;
			this.net = null;
		}

		public override bool Start()
		{
			if (this.peer == null)
			{
				return false;
			}
			this.stream.Position = (long)0;
			this.stream.SetLength((long)0);
			this.peer.ReadBytes(this.stream, this.peer.incomingBytesUnread);
			return true;
		}

		public override bool Start(Connection connection)
		{
			if (!this.Start())
			{
				return false;
			}
			if (this.stream.Length > (long)1 && this.net.cryptography != null && this.net.cryptography.IsEnabledIncoming(connection))
			{
				this.net.cryptography.Decrypt(connection, this.stream, 1);
			}
			return true;
		}

		public override ushort UInt16()
		{
			if (base.unread < 2)
			{
				return (ushort)0;
			}
			return this.Read16().u;
		}

		public override uint UInt32()
		{
			if (base.unread < 4)
			{
				return (uint)0;
			}
			return this.Read32().u;
		}

		public override ulong UInt64()
		{
			if (base.unread < 8)
			{
				return (ulong)0;
			}
			return this.Read64().u;
		}

		public override byte UInt8()
		{
			if (base.unread < 1)
			{
				return (byte)0;
			}
			return this.Read8().u;
		}
	}
}