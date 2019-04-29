using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Facepunch.Network.Raknet
{
	[SuppressUnmanagedCodeSecurity]
	public class Native
	{
		public Native()
		{
		}

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern void NET_Close(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern void NET_CloseConnection(IntPtr nw, ulong connectionID);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr NET_Create();

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr NET_GetAddress(IntPtr nw, ulong connectionID);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int NET_GetAveragePing(IntPtr nw, ulong connectionID);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int NET_GetLastPing(IntPtr nw, ulong connectionID);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int NET_GetLowestPing(IntPtr nw, ulong connectionID);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool NET_GetStatistics(IntPtr nw, ulong connectionID, ref Native.RaknetStats data, int dataLength);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr NET_GetStatisticsString(IntPtr nw, ulong connectionID);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr NET_LastStartupError(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool NET_Receive(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern unsafe void NET_SendMessage(IntPtr nw, byte* data, int length, uint adr, ushort port);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int NET_StartClient(IntPtr nw, string hostName, int port, int retries, int retryDelay, int timeout);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int NET_StartServer(IntPtr nw, string ip, int port, int maxConnections);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern uint NETRCV_Address(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern ulong NETRCV_GUID(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int NETRCV_LengthBits(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern uint NETRCV_Port(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr NETRCV_RawData(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern unsafe bool NETRCV_ReadBytes(IntPtr nw, byte* data, int length);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern void NETRCV_SetReadPointer(IntPtr nw, int bitsOffset);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int NETRCV_UnreadBits(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern uint NETSND_Broadcast(IntPtr nw, int priority, int reliability, int channel);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern float NETSND_ReadCompressedFloat(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern uint NETSND_Send(IntPtr nw, ulong connectionID, int priority, int reliability, int channel);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern uint NETSND_Size(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern void NETSND_Start(IntPtr nw);

		[DllImport("RakNet", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern unsafe void NETSND_WriteBytes(IntPtr nw, byte* data, int length);

		public enum Metrics
		{
			USER_MESSAGE_BYTES_PUSHED,
			USER_MESSAGE_BYTES_SENT,
			USER_MESSAGE_BYTES_RESENT,
			USER_MESSAGE_BYTES_RECEIVED_PROCESSED,
			USER_MESSAGE_BYTES_RECEIVED_IGNORED,
			ACTUAL_BYTES_SENT,
			ACTUAL_BYTES_RECEIVED,
			RNS_PER_SECOND_METRICS_COUNT
		}

		public enum PacketPriority
		{
			IMMEDIATE_PRIORITY,
			HIGH_PRIORITY,
			MEDIUM_PRIORITY,
			LOW_PRIORITY,
			NUMBER_OF_PRIORITIES
		}

		public struct RaknetStats
		{
			[FixedBuffer(typeof(ulong), 7)]
			public Native.RaknetStats.<valueOverLastSecond>e__FixedBuffer valueOverLastSecond;

			[FixedBuffer(typeof(ulong), 7)]
			public Native.RaknetStats.<runningTotal>e__FixedBuffer runningTotal;

			public ulong connectionStartTime;

			public byte isLimitedByCongestionControl;

			public ulong BPSLimitByCongestionControl;

			public byte isLimitedByOutgoingBandwidthLimit;

			public ulong BPSLimitByOutgoingBandwidthLimit;

			[FixedBuffer(typeof(uint), 4)]
			public Native.RaknetStats.<messageInSendBuffer>e__FixedBuffer messageInSendBuffer;

			[FixedBuffer(typeof(double), 4)]
			public Native.RaknetStats.<bytesInSendBuffer>e__FixedBuffer bytesInSendBuffer;

			public uint messagesInResendBuffer;

			public ulong bytesInResendBuffer;

			public float packetlossLastSecond;

			public float packetlossTotal;
		}
	}
}