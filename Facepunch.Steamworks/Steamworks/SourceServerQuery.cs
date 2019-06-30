using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks
{
	internal static class SourceServerQuery
	{
		private readonly static byte[] A2S_SERVERQUERY_GETCHALLENGE;

		private readonly static byte A2S_RULES;

		private static byte[] readBuffer;

		private static byte[] sendBuffer;

		static SourceServerQuery()
		{
			SourceServerQuery.A2S_SERVERQUERY_GETCHALLENGE = new Byte[] { 85, 255, 255, 255, 255 };
			SourceServerQuery.A2S_RULES = 86;
			SourceServerQuery.readBuffer = new Byte[8192];
			SourceServerQuery.sendBuffer = new Byte[1024];
		}

		private static byte[] Combine(byte[][] arrays)
		{
			byte[] numArray = new Byte[arrays.Sum<byte[]>((byte[] a) => (int)a.Length)];
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

		private static async Task<byte[]> GetChallengeData(UdpClient client)
		{
			await SourceServerQuery.Send(client, SourceServerQuery.A2S_SERVERQUERY_GETCHALLENGE);
			byte[] numArray = await SourceServerQuery.Receive(client);
			byte[] numArray1 = numArray;
			numArray = null;
			if (numArray1[0] != 65)
			{
				throw new Exception("Invalid Challenge");
			}
			return numArray1;
		}

		internal static async Task<Dictionary<string, string>> GetRules(ServerInfo server)
		{
			SourceServerQuery.<GetRules>d__2 variable = null;
			AsyncTaskMethodBuilder<Dictionary<string, string>> asyncTaskMethodBuilder = AsyncTaskMethodBuilder<Dictionary<string, string>>.Create();
			asyncTaskMethodBuilder.Start<SourceServerQuery.<GetRules>d__2>(ref variable);
			return asyncTaskMethodBuilder.Task;
		}

		private static async Task<Dictionary<string, string>> GetRules(UdpClient client)
		{
			byte[] challengeData = await SourceServerQuery.GetChallengeData(client);
			byte[] a2SRULES = challengeData;
			challengeData = null;
			a2SRULES[0] = SourceServerQuery.A2S_RULES;
			await SourceServerQuery.Send(client, a2SRULES);
			byte[] numArray = await SourceServerQuery.Receive(client);
			byte[] numArray1 = numArray;
			numArray = null;
			Dictionary<string, string> strs = new Dictionary<string, string>();
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(numArray1)))
			{
				if (binaryReader.ReadByte() != 69)
				{
					throw new Exception("Invalid data received in response to A2S_RULES request");
				}
				ushort num = binaryReader.ReadUInt16();
				for (int i = 0; i < num; i++)
				{
					strs.Add(binaryReader.ReadNullTerminatedUTF8String(SourceServerQuery.readBuffer), binaryReader.ReadNullTerminatedUTF8String(SourceServerQuery.readBuffer));
				}
			}
			binaryReader = null;
			return strs;
		}

		private static async Task<byte[]> Receive(UdpClient client)
		{
			byte[] numArray;
			Func<byte[], bool> u003cu003e9_50;
			byte[][] numArray1;
			byte[][] numArray2 = null;
			byte num = 0;
			byte num1 = 1;
			do
			{
				UdpReceiveResult udpReceiveResult = await client.ReceiveAsync();
				UdpReceiveResult udpReceiveResult1 = udpReceiveResult;
				udpReceiveResult = new UdpReceiveResult();
				byte[] buffer = udpReceiveResult1.Buffer;
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer)))
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
						byte[] numArray3 = new Byte[checked((IntPtr)((long)((int)buffer.Length) - binaryReader.BaseStream.Position))];
						Buffer.BlockCopy(buffer, (int)binaryReader.BaseStream.Position, numArray3, 0, (int)numArray3.Length);
						numArray2[num] = numArray3;
						numArray3 = null;
					}
					else
					{
						byte[] numArray4 = new Byte[checked((IntPtr)((long)((int)buffer.Length) - binaryReader.BaseStream.Position))];
						Buffer.BlockCopy(buffer, (int)binaryReader.BaseStream.Position, numArray4, 0, (int)numArray4.Length);
						numArray = numArray4;
						return numArray;
					}
				}
				binaryReader = null;
				udpReceiveResult1 = new UdpReceiveResult();
				buffer = null;
				numArray1 = numArray2;
				u003cu003e9_50 = SourceServerQuery.<>c.<>9__5_0;
				if (u003cu003e9_50 != null)
				{
					continue;
				}
				u003cu003e9_50 = (byte[] p) => p == null;
				SourceServerQuery.<>c.<>9__5_0 = u003cu003e9_50;
			}
			while (numArray1.Any<byte[]>(u003cu003e9_50));
			numArray = SourceServerQuery.Combine(numArray2);
			return numArray;
		}

		private static async Task Send(UdpClient client, byte[] message)
		{
			SourceServerQuery.sendBuffer[0] = 255;
			SourceServerQuery.sendBuffer[1] = 255;
			SourceServerQuery.sendBuffer[2] = 255;
			SourceServerQuery.sendBuffer[3] = 255;
			Buffer.BlockCopy(message, 0, SourceServerQuery.sendBuffer, 4, (int)message.Length);
			await client.SendAsync(SourceServerQuery.sendBuffer, (int)message.Length + 4);
		}
	}
}