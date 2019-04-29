using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Text;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixMessageIO
	{
		private static byte[][] _msgHeaders;

		public static int DefaultStreamBufferSize;

		private static byte[] msgUriTransportKey;

		private static byte[] msgContentTypeTransportKey;

		private static byte[] msgDefaultTransportKey;

		private static byte[] msgHeaderTerminator;

		static UnixMessageIO()
		{
			UnixMessageIO._msgHeaders = new byte[][] { new byte[] { typeof(<87bdb0d4-cafe-4b9b-a84c-2bc6cbcec820><PrivateImplementationDetails>).GetField("$$field-1").FieldHandle }, new byte[] { typeof(<87bdb0d4-cafe-4b9b-a84c-2bc6cbcec820><PrivateImplementationDetails>).GetField("$$field-2").FieldHandle } };
			UnixMessageIO.DefaultStreamBufferSize = 1000;
			UnixMessageIO.msgUriTransportKey = new byte[] { 4, 0, 1, 1 };
			UnixMessageIO.msgContentTypeTransportKey = new byte[] { 6, 0, 1, 1 };
			UnixMessageIO.msgDefaultTransportKey = new byte[] { 1, 0, 1 };
			UnixMessageIO.msgHeaderTerminator = new byte[2];
		}

		public UnixMessageIO()
		{
		}

		public static ITransportHeaders ReceiveHeaders(Stream networkStream, byte[] buffer)
		{
			string str;
			UnixMessageIO.StreamRead(networkStream, buffer, 2);
			byte num = buffer[0];
			TransportHeaders transportHeader = new TransportHeaders();
			while (num != 0)
			{
				UnixMessageIO.StreamRead(networkStream, buffer, 1);
				switch (num)
				{
					case 1:
					{
						str = UnixMessageIO.ReceiveString(networkStream, buffer);
						break;
					}
					case 2:
					case 3:
					case 5:
					{
						throw new NotSupportedException(string.Concat("Unknown header code: ", num));
					}
					case 4:
					{
						str = "__RequestUri";
						break;
					}
					case 6:
					{
						str = "Content-Type";
						break;
					}
					default:
					{
						throw new NotSupportedException(string.Concat("Unknown header code: ", num));
					}
				}
				UnixMessageIO.StreamRead(networkStream, buffer, 1);
				transportHeader[str] = UnixMessageIO.ReceiveString(networkStream, buffer);
				UnixMessageIO.StreamRead(networkStream, buffer, 2);
				num = buffer[0];
			}
			return transportHeader;
		}

		public static MessageStatus ReceiveMessageStatus(Stream networkStream, byte[] buffer)
		{
			MessageStatus messageStatu;
			try
			{
				UnixMessageIO.StreamRead(networkStream, buffer, 6);
			}
			catch (Exception exception)
			{
				throw new RemotingException("Unix transport error.", exception);
			}
			try
			{
				bool[] flagArray = new bool[(int)UnixMessageIO._msgHeaders.Length];
				bool flag = true;
				int num = 0;
				while (flag)
				{
					flag = false;
					byte num1 = buffer[num];
					for (int i = 0; i < (int)UnixMessageIO._msgHeaders.Length; i++)
					{
						if (num <= 0 || flagArray[i])
						{
							flagArray[i] = num1 == UnixMessageIO._msgHeaders[i][num];
							if (!flagArray[i] || num != (int)UnixMessageIO._msgHeaders[i].Length - 1)
							{
								flag = (flag ? true : flagArray[i]);
							}
							else
							{
								messageStatu = (MessageStatus)i;
								return messageStatu;
							}
						}
					}
					num++;
				}
				messageStatu = MessageStatus.Unknown;
			}
			catch (Exception exception1)
			{
				throw new RemotingException("Unix transport error.", exception1);
			}
			return messageStatu;
		}

		public static Stream ReceiveMessageStream(Stream networkStream, out ITransportHeaders headers, byte[] buffer)
		{
			headers = null;
			if (buffer == null)
			{
				buffer = new byte[UnixMessageIO.DefaultStreamBufferSize];
			}
			UnixMessageIO.StreamRead(networkStream, buffer, 8);
			int num = buffer[4] | buffer[5] << 8 | buffer[6] << 16 | buffer[7] << 24;
			headers = UnixMessageIO.ReceiveHeaders(networkStream, buffer);
			byte[] numArray = new byte[num];
			UnixMessageIO.StreamRead(networkStream, numArray, num);
			return new MemoryStream(numArray);
		}

		private static string ReceiveString(Stream networkStream, byte[] buffer)
		{
			UnixMessageIO.StreamRead(networkStream, buffer, 4);
			int num = buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24;
			if (num == 0)
			{
				return string.Empty;
			}
			if (num > (int)buffer.Length)
			{
				buffer = new byte[num];
			}
			UnixMessageIO.StreamRead(networkStream, buffer, num);
			char[] chars = Encoding.UTF8.GetChars(buffer, 0, num);
			return new string(chars);
		}

		private static void SendHeaders(Stream networkStream, ITransportHeaders requestHeaders, byte[] buffer)
		{
			int num;
			if (networkStream != null)
			{
				IEnumerator enumerator = requestHeaders.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DictionaryEntry current = (DictionaryEntry)enumerator.Current;
					string str = current.Key.ToString();
					if (str != null)
					{
						if (UnixMessageIO.<>f__switch$map1 == null)
						{
							Dictionary<string, int> strs = new Dictionary<string, int>(2)
							{
								{ "__RequestUri", 0 },
								{ "Content-Type", 1 }
							};
							UnixMessageIO.<>f__switch$map1 = strs;
						}
						if (UnixMessageIO.<>f__switch$map1.TryGetValue(str, out num))
						{
							if (num == 0)
							{
								networkStream.Write(UnixMessageIO.msgUriTransportKey, 0, 4);
								goto Label0;
							}
							else
							{
								if (num != 1)
								{
									goto Label2;
								}
								networkStream.Write(UnixMessageIO.msgContentTypeTransportKey, 0, 4);
								goto Label0;
							}
						}
					}
				Label2:
					networkStream.Write(UnixMessageIO.msgDefaultTransportKey, 0, 3);
					UnixMessageIO.SendString(networkStream, current.Key.ToString(), buffer);
					networkStream.WriteByte(1);
				Label0:
					UnixMessageIO.SendString(networkStream, current.Value.ToString(), buffer);
				}
			}
			networkStream.Write(UnixMessageIO.msgHeaderTerminator, 0, 2);
		}

		public static void SendMessageStream(Stream networkStream, Stream data, ITransportHeaders requestHeaders, byte[] buffer)
		{
			if (buffer == null)
			{
				buffer = new byte[UnixMessageIO.DefaultStreamBufferSize];
			}
			byte[] numArray = UnixMessageIO._msgHeaders[0];
			networkStream.Write(numArray, 0, (int)numArray.Length);
			if (requestHeaders["__RequestUri"] == null)
			{
				buffer[0] = 2;
			}
			else
			{
				buffer[0] = 0;
			}
			buffer[1] = 0;
			buffer[2] = 0;
			buffer[3] = 0;
			int length = (int)data.Length;
			buffer[4] = (byte)length;
			buffer[5] = (byte)(length >> 8);
			buffer[6] = (byte)(length >> 16);
			buffer[7] = (byte)(length >> 24);
			networkStream.Write(buffer, 0, 8);
			UnixMessageIO.SendHeaders(networkStream, requestHeaders, buffer);
			if (!(data is MemoryStream))
			{
				for (int i = data.Read(buffer, 0, (int)buffer.Length); i > 0; i = data.Read(buffer, 0, (int)buffer.Length))
				{
					networkStream.Write(buffer, 0, i);
				}
			}
			else
			{
				MemoryStream memoryStream = (MemoryStream)data;
				networkStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			}
		}

		private static void SendString(Stream networkStream, string str, byte[] buffer)
		{
			int maxByteCount = Encoding.UTF8.GetMaxByteCount(str.Length) + 4;
			if (maxByteCount > (int)buffer.Length)
			{
				buffer = new byte[maxByteCount];
			}
			int bytes = Encoding.UTF8.GetBytes(str, 0, str.Length, buffer, 4);
			buffer[0] = (byte)bytes;
			buffer[1] = (byte)(bytes >> 8);
			buffer[2] = (byte)(bytes >> 16);
			buffer[3] = (byte)(bytes >> 24);
			networkStream.Write(buffer, 0, bytes + 4);
		}

		private static bool StreamRead(Stream networkStream, byte[] buffer, int count)
		{
			int num = 0;
			do
			{
				int num1 = networkStream.Read(buffer, num, count - num);
				if (num1 == 0)
				{
					throw new RemotingException("Connection closed");
				}
				num += num1;
			}
			while (num < count);
			return true;
		}
	}
}