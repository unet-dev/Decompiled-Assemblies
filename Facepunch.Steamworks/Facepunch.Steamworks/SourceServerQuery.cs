using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Facepunch.Steamworks
{
	internal class SourceServerQuery : IDisposable
	{
		public static List<SourceServerQuery> Current;

		private readonly static byte[] A2S_SERVERQUERY_GETCHALLENGE;

		private readonly static byte A2S_RULES;

		public volatile bool IsRunning;

		public volatile bool IsSuccessful;

		private ServerList.Server Server;

		private UdpClient udpClient;

		private IPEndPoint endPoint;

		private Thread thread;

		private byte[] _challengeBytes;

		private Dictionary<string, string> rules = new Dictionary<string, string>();

		private byte[] readBuffer = new byte[4096];

		private byte[] sendBuffer = new byte[1024];

		static SourceServerQuery()
		{
			SourceServerQuery.Current = new List<SourceServerQuery>();
			SourceServerQuery.A2S_SERVERQUERY_GETCHALLENGE = new byte[] { 85, 255, 255, 255, 255 };
			SourceServerQuery.A2S_RULES = 86;
		}

		public SourceServerQuery(ServerList.Server server, IPAddress address, int queryPort)
		{
			this.Server = server;
			this.endPoint = new IPEndPoint(address, queryPort);
			SourceServerQuery.Current.Add(this);
			this.IsRunning = true;
			this.IsSuccessful = false;
			this.thread = new Thread(new ParameterizedThreadStart(this.ThreadedStart));
			this.thread.Start();
		}

		private byte[] Combine(byte[][] arrays)
		{
			byte[] numArray = new byte[((IEnumerable<byte[]>)arrays).Sum<byte[]>((byte[] a) => (int)a.Length)];
			int length = 0;
			byte[][] numArray1 = arrays;
			for (int i = 0; i < (int)numArray1.Length; i++)
			{
				byte[] numArray2 = numArray1[i];
				Buffer.BlockCopy(numArray2, 0, numArray, length, (int)numArray2.Length);
				length += (int)numArray2.Length;
			}
			return numArray;
		}

		public static void Cycle()
		{
			if (SourceServerQuery.Current.Count == 0)
			{
				return;
			}
			for (int i = SourceServerQuery.Current.Count; i > 0; i--)
			{
				SourceServerQuery.Current[i - 1].Update();
			}
		}

		public void Dispose()
		{
			if (this.thread != null && this.thread.IsAlive)
			{
				this.thread.Abort();
			}
			this.thread = null;
		}

		private void GetChallengeData()
		{
			if (this._challengeBytes != null)
			{
				return;
			}
			this.Send(SourceServerQuery.A2S_SERVERQUERY_GETCHALLENGE);
			byte[] numArray = this.Receive();
			if (numArray[0] != 65)
			{
				throw new Exception("Invalid Challenge");
			}
			this._challengeBytes = numArray;
		}

		private void GetRules()
		{
			this.GetChallengeData();
			this._challengeBytes[0] = SourceServerQuery.A2S_RULES;
			this.Send(this._challengeBytes);
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(this.Receive())))
			{
				if (binaryReader.ReadByte() != 69)
				{
					throw new Exception("Invalid data received in response to A2S_RULES request");
				}
				ushort num = binaryReader.ReadUInt16();
				for (int i = 0; i < num; i++)
				{
					this.rules.Add(binaryReader.ReadNullTerminatedUTF8String(this.readBuffer), binaryReader.ReadNullTerminatedUTF8String(this.readBuffer));
				}
			}
		}

		private byte[] Receive()
		{
			byte[] numArray;
			Func<byte[], bool> u003cu003e9_170 = null;
			byte[][] numArray1 = null;
			byte[][] numArray2 = null;
			byte num = 0;
			byte num1 = 1;
			do
			{
				byte[] numArray3 = this.udpClient.Receive(ref this.endPoint);
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(numArray3)))
				{
					int num2 = binaryReader.ReadInt32();
					if (num2 != -1)
					{
						if (num2 != -2)
						{
							throw new Exception("Invalid Header");
						}
						binaryReader.ReadInt32();
						num = binaryReader.ReadByte();
						num1 = binaryReader.ReadByte();
						binaryReader.ReadInt32();
						if (numArray2 == null)
						{
							numArray2 = new byte[num1][];
						}
						byte[] numArray4 = new byte[checked((IntPtr)((long)((int)numArray3.Length) - binaryReader.BaseStream.Position))];
						Buffer.BlockCopy(numArray3, (int)binaryReader.BaseStream.Position, numArray4, 0, (int)numArray4.Length);
						numArray2[num] = numArray4;
						goto Label0;
					}
					else
					{
						byte[] numArray5 = new byte[checked((IntPtr)((long)((int)numArray3.Length) - binaryReader.BaseStream.Position))];
						Buffer.BlockCopy(numArray3, (int)binaryReader.BaseStream.Position, numArray5, 0, (int)numArray5.Length);
						numArray = numArray5;
					}
				}
				return numArray;
			Label1:
			}
			while (((IEnumerable<byte[]>)numArray1).Any<byte[]>(u003cu003e9_170));
			return this.Combine(numArray2);
		Label0:
			numArray1 = numArray2;
			u003cu003e9_170 = SourceServerQuery.<>c.<>9__17_0;
			if (u003cu003e9_170 != null)
			{
				goto Label1;
			}
			u003cu003e9_170 = (byte[] p) => p == null;
			SourceServerQuery.<>c.<>9__17_0 = u003cu003e9_170;
			goto Label1;
		}

		private void Send(byte[] message)
		{
			this.sendBuffer[0] = 255;
			this.sendBuffer[1] = 255;
			this.sendBuffer[2] = 255;
			this.sendBuffer[3] = 255;
			Buffer.BlockCopy(message, 0, this.sendBuffer, 4, (int)message.Length);
			this.udpClient.Send(this.sendBuffer, (int)message.Length + 4);
		}

		private void ThreadedStart(object obj)
		{
			try
			{
				UdpClient udpClient = new UdpClient();
				UdpClient udpClient1 = udpClient;
				this.udpClient = udpClient;
				using (udpClient1)
				{
					this.udpClient.Client.SendTimeout = 3000;
					this.udpClient.Client.ReceiveTimeout = 3000;
					this.udpClient.Connect(this.endPoint);
					this.GetRules();
					this.IsSuccessful = true;
				}
			}
			catch (Exception exception)
			{
				this.IsSuccessful = false;
			}
			this.udpClient = null;
			this.IsRunning = false;
		}

		private void Update()
		{
			if (!this.IsRunning)
			{
				SourceServerQuery.Current.Remove(this);
				this.Server.OnServerRulesReceiveFinished(this.rules, this.IsSuccessful);
			}
		}
	}
}