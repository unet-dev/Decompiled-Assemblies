using Network;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine;

namespace Facepunch.Network.Raknet
{
	[SuppressUnmanagedCodeSecurity]
	internal class Peer
	{
		private IntPtr ptr;

		private static byte[] ByteBuffer;

		public string incomingAddress
		{
			get
			{
				return this.GetAddress(this.incomingGUID);
			}
		}

		public virtual uint incomingAddressInt
		{
			get
			{
				this.Check();
				return Native.NETRCV_Address(this.ptr);
			}
		}

		public virtual int incomingBits
		{
			get
			{
				this.Check();
				return Native.NETRCV_LengthBits(this.ptr);
			}
		}

		public virtual int incomingBitsUnread
		{
			get
			{
				this.Check();
				return Native.NETRCV_UnreadBits(this.ptr);
			}
		}

		public virtual int incomingBytes
		{
			get
			{
				return this.incomingBits / 8;
			}
		}

		public virtual int incomingBytesUnread
		{
			get
			{
				return this.incomingBitsUnread / 8;
			}
		}

		public virtual ulong incomingGUID
		{
			get
			{
				this.Check();
				return Native.NETRCV_GUID(this.ptr);
			}
		}

		public virtual uint incomingPort
		{
			get
			{
				this.Check();
				return Native.NETRCV_Port(this.ptr);
			}
		}

		static Peer()
		{
			Peer.ByteBuffer = new byte[512];
		}

		public Peer()
		{
		}

		protected virtual void Check()
		{
			if (this.ptr == IntPtr.Zero)
			{
				throw new NullReferenceException("Peer has already shut down!");
			}
		}

		public void Close()
		{
			if (this.ptr != IntPtr.Zero)
			{
				Native.NET_Close(this.ptr);
				this.ptr = IntPtr.Zero;
			}
		}

		public static Peer CreateConnection(string hostname, int port, int retries, int retryDelay, int timeout)
		{
			Peer peer = new Peer()
			{
				ptr = Native.NET_Create()
			};
			if (Native.NET_StartClient(peer.ptr, hostname, port, retries, retryDelay, timeout) == 0)
			{
				return peer;
			}
			string str = Peer.StringFromPointer(Native.NET_LastStartupError(peer.ptr));
			Debug.LogWarning(string.Concat(new object[] { "Couldn't connect to server ", hostname, ":", port, " (", str, ")" }));
			peer.Close();
			peer = null;
			return null;
		}

		public static Peer CreateServer(string ip, int port, int maxConnections)
		{
			Peer peer = new Peer()
			{
				ptr = Native.NET_Create()
			};
			if (Native.NET_StartServer(peer.ptr, ip, port, maxConnections) == 0)
			{
				return peer;
			}
			peer.Close();
			string str = Peer.StringFromPointer(Native.NET_LastStartupError(peer.ptr));
			Debug.LogWarning(string.Concat(new object[] { "Couldn't create server on port ", port, " (", str, ")" }));
			return null;
		}

		public string GetAddress(ulong guid)
		{
			this.Check();
			return Peer.StringFromPointer(Native.NET_GetAddress(this.ptr, guid));
		}

		public virtual int GetPingAverage(ulong guid)
		{
			this.Check();
			return Native.NET_GetAveragePing(this.ptr, guid);
		}

		public virtual int GetPingLast(ulong guid)
		{
			this.Check();
			return Native.NET_GetLastPing(this.ptr, guid);
		}

		public virtual int GetPingLowest(ulong guid)
		{
			this.Check();
			return Native.NET_GetLowestPing(this.ptr, guid);
		}

		public virtual ulong GetStat(Connection connection, NetworkPeer.StatTypeLong type)
		{
			unsafe
			{
				this.Check();
				Native.RaknetStats raknetStat = (connection == null ? this.GetStatistics((ulong)0) : this.GetStatistics(connection.guid));
				switch (type)
				{
					case NetworkPeer.StatTypeLong.BytesSent:
					{
						return (ulong)(*(&raknetStat.runningTotal.FixedElementField + 5 * 8));
					}
					case NetworkPeer.StatTypeLong.BytesSent_LastSecond:
					{
						return (ulong)(*(&raknetStat.valueOverLastSecond.FixedElementField + 5 * 8));
					}
					case NetworkPeer.StatTypeLong.BytesReceived:
					{
						return (ulong)(*(&raknetStat.runningTotal.FixedElementField + 6 * 8));
					}
					case NetworkPeer.StatTypeLong.BytesReceived_LastSecond:
					{
						return (ulong)(*(&raknetStat.valueOverLastSecond.FixedElementField + 6 * 8));
					}
					case NetworkPeer.StatTypeLong.MessagesInSendBuffer:
					case NetworkPeer.StatTypeLong.MessagesInResendBuffer:
					{
						return (ulong)0;
					}
					case NetworkPeer.StatTypeLong.BytesInSendBuffer:
					{
						return (ulong)(&raknetStat.bytesInSendBuffer.FixedElementField);
					}
					case NetworkPeer.StatTypeLong.BytesInResendBuffer:
					{
						return raknetStat.bytesInResendBuffer;
					}
					case NetworkPeer.StatTypeLong.PacketLossAverage:
					{
						return (ulong)raknetStat.packetlossTotal * (long)10000;
					}
					case NetworkPeer.StatTypeLong.PacketLossLastSecond:
					{
						return (ulong)raknetStat.packetlossLastSecond * (long)10000;
					}
					case NetworkPeer.StatTypeLong.ThrottleBytes:
					{
						if (raknetStat.isLimitedByCongestionControl == 0)
						{
							return (ulong)0;
						}
						return raknetStat.BPSLimitByCongestionControl;
					}
					default:
					{
						return (ulong)0;
					}
				}
			}
		}

		public virtual Native.RaknetStats GetStatistics(ulong guid)
		{
			this.Check();
			Native.RaknetStats raknetStat = new Native.RaknetStats();
			int num = (int)sizeof(Native.RaknetStats);
			if (!Native.NET_GetStatistics(this.ptr, guid, ref raknetStat, num))
			{
				Debug.Log(string.Concat("NET_GetStatistics:  Wrong size ", num));
			}
			return raknetStat;
		}

		public virtual string GetStatisticsString(ulong guid)
		{
			this.Check();
			return string.Format("Average Ping:\t\t{0}\nLast Ping:\t\t{1}\nLowest Ping:\t\t{2}\n{3}", new object[] { this.GetPingAverage(guid), this.GetPingLast(guid), this.GetPingLowest(guid), Peer.StringFromPointer(Native.NET_GetStatisticsString(this.ptr, guid)) });
		}

		public void Kick(Connection connection)
		{
			this.Check();
			Native.NET_CloseConnection(this.ptr, connection.guid);
		}

		public virtual IntPtr RawData()
		{
			this.Check();
			return Native.NETRCV_RawData(this.ptr);
		}

		protected virtual unsafe bool Read(byte* data, int length)
		{
			this.Check();
			return Native.NETRCV_ReadBytes(this.ptr, data, length);
		}

		public unsafe byte ReadByte()
		{
			// 
			// Current member / type: System.Byte Facepunch.Network.Raknet.Peer::ReadByte()
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Raknet.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Byte ReadByte()
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public unsafe int ReadBytes(byte[] buffer, int offset, int length)
		{
			// 
			// Current member / type: System.Int32 Facepunch.Network.Raknet.Peer::ReadBytes(System.Byte[],System.Int32,System.Int32)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Raknet.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Int32 ReadBytes(System.Byte[],System.Int32,System.Int32)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public unsafe int ReadBytes(MemoryStream memoryStream, int length)
		{
			// 
			// Current member / type: System.Int32 Facepunch.Network.Raknet.Peer::ReadBytes(System.IO.MemoryStream,System.Int32)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Raknet.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Int32 ReadBytes(System.IO.MemoryStream,System.Int32)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public bool Receive()
		{
			if (this.ptr == IntPtr.Zero)
			{
				return false;
			}
			return Native.NET_Receive(this.ptr);
		}

		public virtual uint SendBroadcast(Priority priority, SendMethod reliability, sbyte channel)
		{
			this.Check();
			return Native.NETSND_Broadcast(this.ptr, this.ToRaknetPriority(priority), this.ToRaknetPacketReliability(reliability), channel);
		}

		public virtual void SendStart()
		{
			this.Check();
			Native.NETSND_Start(this.ptr);
		}

		public virtual uint SendTo(ulong guid, Priority priority, SendMethod reliability, sbyte channel)
		{
			this.Check();
			return Native.NETSND_Send(this.ptr, guid, this.ToRaknetPriority(priority), this.ToRaknetPacketReliability(reliability), channel);
		}

		public unsafe void SendUnconnectedMessage(byte* data, int length, uint adr, ushort port)
		{
			this.Check();
			Native.NET_SendMessage(this.ptr, data, length, adr, port);
		}

		public virtual void SetReadPos(int bitsOffset)
		{
			this.Check();
			Native.NETRCV_SetReadPointer(this.ptr, bitsOffset);
		}

		private static string StringFromPointer(IntPtr p)
		{
			if (p == IntPtr.Zero)
			{
				return string.Empty;
			}
			return Marshal.PtrToStringAnsi(p);
		}

		public int ToRaknetPacketReliability(SendMethod priority)
		{
			switch (priority)
			{
				case SendMethod.Reliable:
				{
					return 3;
				}
				case SendMethod.ReliableUnordered:
				{
					return 2;
				}
				case SendMethod.ReliableSequenced:
				{
					return 4;
				}
				case SendMethod.Unreliable:
				{
					return 0;
				}
				case SendMethod.UnreliableSequenced:
				{
					return 1;
				}
			}
			return 3;
		}

		public int ToRaknetPriority(Priority priority)
		{
			switch (priority)
			{
				case Priority.Immediate:
				{
					return 0;
				}
				case Priority.High:
				{
					return 1;
				}
				case Priority.Medium:
				{
					return 2;
				}
			}
			return 3;
		}

		protected virtual unsafe void Write(byte* data, int size)
		{
			if (size == 0)
			{
				return;
			}
			if (data == null)
			{
				return;
			}
			this.Check();
			Native.NETSND_WriteBytes(this.ptr, data, size);
		}

		public void WriteByte(byte val)
		{
			unsafe
			{
				this.Write(ref val, 1);
			}
		}

		public unsafe void WriteBytes(byte[] val, int offset, int length)
		{
			// 
			// Current member / type: System.Void Facepunch.Network.Raknet.Peer::WriteBytes(System.Byte[],System.Int32,System.Int32)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Raknet.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void WriteBytes(System.Byte[],System.Int32,System.Int32)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public unsafe void WriteBytes(byte[] val)
		{
			// 
			// Current member / type: System.Void Facepunch.Network.Raknet.Peer::WriteBytes(System.Byte[])
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Raknet.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void WriteBytes(System.Byte[])
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public void WriteBytes(MemoryStream stream)
		{
			this.WriteBytes(stream.GetBuffer(), 0, (int)stream.Length);
		}

		public enum PacketReliability
		{
			UNRELIABLE,
			UNRELIABLE_SEQUENCED,
			RELIABLE,
			RELIABLE_ORDERED,
			RELIABLE_SEQUENCED,
			UNRELIABLE_WITH_ACK_RECEIPT,
			RELIABLE_WITH_ACK_RECEIPT,
			RELIABLE_ORDERED_WITH_ACK_RECEIPT
		}
	}
}