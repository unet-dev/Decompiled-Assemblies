using Network;
using System;
using System.Collections.Generic;
using System.IO;

namespace Facepunch.Network.Raknet
{
	internal class StreamWrite : Write
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

		public StreamWrite(NetworkPeer net, Peer peer)
		{
			this.net = net;
			this.peer = peer;
			this.stream = new MemoryStream();
		}

		public override void Bool(bool val)
		{
			this.stream.WriteByte((Byte)((val ? 1 : 0)));
		}

		public override void Bytes(byte[] val)
		{
			this.stream.Write(val, 0, (int)val.Length);
		}

		public override void Double(double val)
		{
			this.Write64(new Union64()
			{
				f = val
			});
		}

		public override void Float(float val)
		{
			this.Write32(new Union32()
			{
				f = val
			});
		}

		private byte[] GetWriteBuffer()
		{
			return this.stream.GetBuffer();
		}

		private long GetWriteOffset(long i)
		{
			long position = this.stream.Position;
			if (this.stream.Length < position + i)
			{
				this.stream.SetLength(position + i);
			}
			this.stream.Position = position + i;
			return position;
		}

		public override void Int16(short val)
		{
			this.Write16(new Union16()
			{
				i = val
			});
		}

		public override void Int32(int val)
		{
			this.Write32(new Union32()
			{
				i = val
			});
		}

		public override void Int64(long val)
		{
			this.Write64(new Union64()
			{
				i = val
			});
		}

		public override void Int8(sbyte val)
		{
			this.Write8(new Union8()
			{
				i = val
			});
		}

		public override void PacketID(Message.Type val)
		{
			this.UInt8((byte)((byte)val + (byte)(Message.Type.Ready | Message.Type.GroupDestroy | Message.Type.ConsoleCommand)));
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.stream.Seek(offset, origin);
		}

		public override void Send(SendInfo info)
		{
			if (info.connections != null)
			{
				foreach (Connection connection in info.connections)
				{
					MemoryStream memoryStream = this.stream;
					if (memoryStream.Length > (long)1 && this.net.cryptography != null && this.net.cryptography.IsEnabledOutgoing(connection))
					{
						memoryStream = this.net.cryptography.EncryptCopy(connection, memoryStream, 1);
					}
					this.peer.SendStart();
					this.peer.WriteBytes(memoryStream);
					this.peer.SendTo(connection.guid, info.priority, info.method, info.channel);
				}
			}
			if (info.connection != null)
			{
				Connection connection1 = info.connection;
				MemoryStream memoryStream1 = this.stream;
				if (memoryStream1.Length > (long)1 && this.net.cryptography != null && this.net.cryptography.IsEnabledOutgoing(connection1))
				{
					this.net.cryptography.Encrypt(connection1, memoryStream1, 1);
				}
				this.peer.SendStart();
				this.peer.WriteBytes(memoryStream1);
				this.peer.SendTo(connection1.guid, info.priority, info.method, info.channel);
			}
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
			return true;
		}

		public override void UInt16(ushort val)
		{
			this.Write16(new Union16()
			{
				u = val
			});
		}

		public override void UInt32(uint val)
		{
			this.Write32(new Union32()
			{
				u = val
			});
		}

		public override void UInt64(ulong val)
		{
			this.Write64(new Union64()
			{
				u = val
			});
		}

		public override void UInt8(byte val)
		{
			this.Write8(new Union8()
			{
				u = val
			});
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.stream.Write(buffer, offset, count);
		}

		private void Write16(Union16 u)
		{
			long writeOffset = this.GetWriteOffset((long)2);
			byte[] writeBuffer = this.GetWriteBuffer();
			writeBuffer[checked((IntPtr)writeOffset)] = u.b1;
			writeBuffer[checked((IntPtr)(writeOffset + (long)1))] = u.b2;
		}

		private void Write32(Union32 u)
		{
			long writeOffset = this.GetWriteOffset((long)4);
			byte[] writeBuffer = this.GetWriteBuffer();
			writeBuffer[checked((IntPtr)writeOffset)] = u.b1;
			writeBuffer[checked((IntPtr)(writeOffset + (long)1))] = u.b2;
			writeBuffer[checked((IntPtr)(writeOffset + (long)2))] = u.b3;
			writeBuffer[checked((IntPtr)(writeOffset + (long)3))] = u.b4;
		}

		private void Write64(Union64 u)
		{
			long writeOffset = this.GetWriteOffset((long)8);
			byte[] writeBuffer = this.GetWriteBuffer();
			writeBuffer[checked((IntPtr)writeOffset)] = u.b1;
			writeBuffer[checked((IntPtr)(writeOffset + (long)1))] = u.b2;
			writeBuffer[checked((IntPtr)(writeOffset + (long)2))] = u.b3;
			writeBuffer[checked((IntPtr)(writeOffset + (long)3))] = u.b4;
			writeBuffer[checked((IntPtr)(writeOffset + (long)4))] = u.b5;
			writeBuffer[checked((IntPtr)(writeOffset + (long)5))] = u.b6;
			writeBuffer[checked((IntPtr)(writeOffset + (long)6))] = u.b7;
			writeBuffer[checked((IntPtr)(writeOffset + (long)7))] = u.b8;
		}

		private void Write8(Union8 u)
		{
			long writeOffset = this.GetWriteOffset((long)1);
			this.GetWriteBuffer()[checked((IntPtr)writeOffset)] = u.b1;
		}

		public override void WriteByte(byte value)
		{
			this.stream.WriteByte(value);
		}
	}
}