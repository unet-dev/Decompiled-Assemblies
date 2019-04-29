using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace WebSocketSharp
{
	internal class WebSocketFrame : IEnumerable<byte>, IEnumerable
	{
		private byte[] _extPayloadLength;

		private WebSocketSharp.Fin _fin;

		private WebSocketSharp.Mask _mask;

		private byte[] _maskingKey;

		private WebSocketSharp.Opcode _opcode;

		private WebSocketSharp.PayloadData _payloadData;

		private byte _payloadLength;

		private Rsv _rsv1;

		private Rsv _rsv2;

		private Rsv _rsv3;

		internal readonly static byte[] EmptyPingBytes;

		public byte[] ExtendedPayloadLength
		{
			get
			{
				return this._extPayloadLength;
			}
		}

		internal int ExtendedPayloadLengthCount
		{
			get
			{
				int num;
				if (this._payloadLength < 126)
				{
					num = 0;
				}
				else
				{
					num = (this._payloadLength == 126 ? 2 : 8);
				}
				return num;
			}
		}

		public WebSocketSharp.Fin Fin
		{
			get
			{
				return this._fin;
			}
		}

		internal ulong FullPayloadLength
		{
			get
			{
				ulong num;
				if (this._payloadLength < 126)
				{
					num = (ulong)this._payloadLength;
				}
				else
				{
					num = (this._payloadLength == 126 ? (ulong)this._extPayloadLength.ToUInt16(ByteOrder.Big) : this._extPayloadLength.ToUInt64(ByteOrder.Big));
				}
				return num;
			}
		}

		public bool IsBinary
		{
			get
			{
				return this._opcode == WebSocketSharp.Opcode.Binary;
			}
		}

		public bool IsClose
		{
			get
			{
				return this._opcode == WebSocketSharp.Opcode.Close;
			}
		}

		public bool IsCompressed
		{
			get
			{
				return this._rsv1 == Rsv.On;
			}
		}

		public bool IsContinuation
		{
			get
			{
				return this._opcode == WebSocketSharp.Opcode.Cont;
			}
		}

		public bool IsControl
		{
			get
			{
				return this._opcode >= WebSocketSharp.Opcode.Close;
			}
		}

		public bool IsData
		{
			get
			{
				return (this._opcode == WebSocketSharp.Opcode.Text ? true : this._opcode == WebSocketSharp.Opcode.Binary);
			}
		}

		public bool IsFinal
		{
			get
			{
				return this._fin == WebSocketSharp.Fin.Final;
			}
		}

		public bool IsFragment
		{
			get
			{
				return (this._fin == WebSocketSharp.Fin.More ? true : this._opcode == WebSocketSharp.Opcode.Cont);
			}
		}

		public bool IsMasked
		{
			get
			{
				return this._mask == WebSocketSharp.Mask.On;
			}
		}

		public bool IsPing
		{
			get
			{
				return this._opcode == WebSocketSharp.Opcode.Ping;
			}
		}

		public bool IsPong
		{
			get
			{
				return this._opcode == WebSocketSharp.Opcode.Pong;
			}
		}

		public bool IsText
		{
			get
			{
				return this._opcode == WebSocketSharp.Opcode.Text;
			}
		}

		public ulong Length
		{
			get
			{
				ulong length = (long)2 + (long)((int)this._extPayloadLength.Length + (int)this._maskingKey.Length) + this._payloadData.Length;
				return length;
			}
		}

		public WebSocketSharp.Mask Mask
		{
			get
			{
				return this._mask;
			}
		}

		public byte[] MaskingKey
		{
			get
			{
				return this._maskingKey;
			}
		}

		public WebSocketSharp.Opcode Opcode
		{
			get
			{
				return this._opcode;
			}
		}

		public WebSocketSharp.PayloadData PayloadData
		{
			get
			{
				return this._payloadData;
			}
		}

		public byte PayloadLength
		{
			get
			{
				return this._payloadLength;
			}
		}

		public Rsv Rsv1
		{
			get
			{
				return this._rsv1;
			}
		}

		public Rsv Rsv2
		{
			get
			{
				return this._rsv2;
			}
		}

		public Rsv Rsv3
		{
			get
			{
				return this._rsv3;
			}
		}

		static WebSocketFrame()
		{
			WebSocketFrame.EmptyPingBytes = WebSocketFrame.CreatePingFrame(false).ToArray();
		}

		private WebSocketFrame()
		{
		}

		internal WebSocketFrame(WebSocketSharp.Opcode opcode, WebSocketSharp.PayloadData payloadData, bool mask) : this(WebSocketSharp.Fin.Final, opcode, payloadData, false, mask)
		{
		}

		internal WebSocketFrame(WebSocketSharp.Fin fin, WebSocketSharp.Opcode opcode, byte[] data, bool compressed, bool mask) : this(fin, opcode, new WebSocketSharp.PayloadData(data), compressed, mask)
		{
		}

		internal WebSocketFrame(WebSocketSharp.Fin fin, WebSocketSharp.Opcode opcode, WebSocketSharp.PayloadData payloadData, bool compressed, bool mask)
		{
			this._fin = fin;
			this._rsv1 = (opcode.IsData() & compressed ? Rsv.On : Rsv.Off);
			this._rsv2 = Rsv.Off;
			this._rsv3 = Rsv.Off;
			this._opcode = opcode;
			ulong length = payloadData.Length;
			if (length < (long)126)
			{
				this._payloadLength = (byte)length;
				this._extPayloadLength = WebSocket.EmptyBytes;
			}
			else if (length >= (long)65536)
			{
				this._payloadLength = 127;
				this._extPayloadLength = length.InternalToByteArray(ByteOrder.Big);
			}
			else
			{
				this._payloadLength = 126;
				this._extPayloadLength = ((ushort)length).InternalToByteArray(ByteOrder.Big);
			}
			if (!mask)
			{
				this._mask = WebSocketSharp.Mask.Off;
				this._maskingKey = WebSocket.EmptyBytes;
			}
			else
			{
				this._mask = WebSocketSharp.Mask.On;
				this._maskingKey = WebSocketFrame.createMaskingKey();
				payloadData.Mask(this._maskingKey);
			}
			this._payloadData = payloadData;
		}

		internal static WebSocketFrame CreateCloseFrame(WebSocketSharp.PayloadData payloadData, bool mask)
		{
			return new WebSocketFrame(WebSocketSharp.Fin.Final, WebSocketSharp.Opcode.Close, payloadData, false, mask);
		}

		private static byte[] createMaskingKey()
		{
			byte[] numArray = new byte[4];
			WebSocket.RandomNumber.GetBytes(numArray);
			return numArray;
		}

		internal static WebSocketFrame CreatePingFrame(bool mask)
		{
			return new WebSocketFrame(WebSocketSharp.Fin.Final, WebSocketSharp.Opcode.Ping, WebSocketSharp.PayloadData.Empty, false, mask);
		}

		internal static WebSocketFrame CreatePingFrame(byte[] data, bool mask)
		{
			WebSocketFrame webSocketFrames = new WebSocketFrame(WebSocketSharp.Fin.Final, WebSocketSharp.Opcode.Ping, new WebSocketSharp.PayloadData(data), false, mask);
			return webSocketFrames;
		}

		private static string dump(WebSocketFrame frame)
		{
			int num2;
			string str1;
			ulong length = frame.Length;
			long num3 = (long)(length / (long)4);
			int num4 = (int)(length % (long)4);
			if (num3 < (long)10000)
			{
				num2 = 4;
				str1 = "{0,4}";
			}
			else if (num3 < (long)65536)
			{
				num2 = 4;
				str1 = "{0,4:X}";
			}
			else if (num3 >= 4294967296L)
			{
				num2 = 16;
				str1 = "{0,16:X}";
			}
			else
			{
				num2 = 8;
				str1 = "{0,8:X}";
			}
			string str2 = string.Format("{{0,{0}}}", num2);
			string str3 = string.Format("\r\n{0} 01234567 89ABCDEF 01234567 89ABCDEF\r\n{0}+--------+--------+--------+--------+\\n", str2);
			string str4 = string.Format("{0}|{{1,8}} {{2,8}} {{3,8}} {{4,8}}|\n", str1);
			string str5 = string.Format("{0}+--------+--------+--------+--------+", str2);
			StringBuilder stringBuilder = new StringBuilder(64);
			Action<string, string, string, string> action = new Func<Action<string, string, string, string>>(() => {
				long num1 = (long)0;
				return (string arg1, string arg2, string arg3, string arg4) => {
					StringBuilder cSu0024u003cu003e8_locals1 = stringBuilder;
					string str = str4;
					object[] objArray = new object[5];
					long num = num1 + (long)1;
					num1 = num;
					objArray[0] = num;
					objArray[1] = arg1;
					objArray[2] = arg2;
					objArray[3] = arg3;
					objArray[4] = arg4;
					cSu0024u003cu003e8_locals1.AppendFormat(str, objArray);
				};
			})();
			stringBuilder.AppendFormat(str3, string.Empty);
			byte[] array = frame.ToArray();
			for (long i = (long)0; i <= num3; i += (long)1)
			{
				long num5 = i * (long)4;
				if (i < num3)
				{
					action(Convert.ToString(array[checked((IntPtr)num5)], 2).PadLeft(8, '0'), Convert.ToString(array[checked((IntPtr)(num5 + (long)1))], 2).PadLeft(8, '0'), Convert.ToString(array[checked((IntPtr)(num5 + (long)2))], 2).PadLeft(8, '0'), Convert.ToString(array[checked((IntPtr)(num5 + (long)3))], 2).PadLeft(8, '0'));
				}
				else if (num4 > 0)
				{
					action(Convert.ToString(array[checked((IntPtr)num5)], 2).PadLeft(8, '0'), (num4 >= 2 ? Convert.ToString(array[checked((IntPtr)(num5 + (long)1))], 2).PadLeft(8, '0') : string.Empty), (num4 == 3 ? Convert.ToString(array[checked((IntPtr)(num5 + (long)2))], 2).PadLeft(8, '0') : string.Empty), string.Empty);
				}
			}
			stringBuilder.AppendFormat(str5, string.Empty);
			return stringBuilder.ToString();
		}

		public IEnumerator<byte> GetEnumerator()
		{
			byte[] array = this.ToArray();
			for (int i = 0; i < (int)array.Length; i++)
			{
				yield return array[i];
			}
			array = null;
		}

		private static string print(WebSocketFrame frame)
		{
			string empty;
			byte num = frame._payloadLength;
			string str = (num > 125 ? frame.FullPayloadLength.ToString() : string.Empty);
			string str1 = BitConverter.ToString(frame._maskingKey);
			if (num == 0)
			{
				empty = string.Empty;
			}
			else if (num > 125)
			{
				empty = "---";
			}
			else
			{
				empty = (!frame.IsText || frame.IsFragment || frame.IsMasked || frame.IsCompressed ? frame._payloadData.ToString() : frame._payloadData.ApplicationData.UTF8Decode());
			}
			string str2 = empty;
			string str3 = "\r\n                    FIN: {0}\r\n                   RSV1: {1}\r\n                   RSV2: {2}\r\n                   RSV3: {3}\r\n                 Opcode: {4}\r\n                   MASK: {5}\r\n         Payload Length: {6}\r\nExtended Payload Length: {7}\r\n            Masking Key: {8}\r\n           Payload Data: {9}";
			return string.Format(str3, new object[] { frame._fin, frame._rsv1, frame._rsv2, frame._rsv3, frame._opcode, frame._mask, num, str, str1, str2 });
		}

		public void Print(bool dumped)
		{
			Console.WriteLine((dumped ? WebSocketFrame.dump(this) : WebSocketFrame.print(this)));
		}

		public string PrintToString(bool dumped)
		{
			return (dumped ? WebSocketFrame.dump(this) : WebSocketFrame.print(this));
		}

		private static WebSocketFrame processHeader(byte[] header)
		{
			string str;
			if ((int)header.Length != 2)
			{
				throw new WebSocketException("The header of a frame cannot be read from the stream.");
			}
			WebSocketSharp.Fin fin = ((header[0] & 128) == 128 ? WebSocketSharp.Fin.Final : WebSocketSharp.Fin.More);
			Rsv rsv = ((header[0] & 64) == 64 ? Rsv.On : Rsv.Off);
			Rsv rsv1 = ((header[0] & 32) == 32 ? Rsv.On : Rsv.Off);
			Rsv rsv2 = ((header[0] & 16) == 16 ? Rsv.On : Rsv.Off);
			byte num = (byte)(header[0] & 15);
			WebSocketSharp.Mask mask = ((header[1] & 128) == 128 ? WebSocketSharp.Mask.On : WebSocketSharp.Mask.Off);
			byte num1 = (byte)(header[1] & 127);
			if (!num.IsSupported())
			{
				str = "An unsupported opcode.";
			}
			else if (!num.IsData() && rsv == Rsv.On)
			{
				str = "A non data frame is compressed.";
			}
			else if (num.IsControl() && fin == WebSocketSharp.Fin.More)
			{
				str = "A control frame is fragmented.";
			}
			else if (!num.IsControl() || num1 <= 125)
			{
				str = null;
			}
			else
			{
				str = "A control frame has a long payload length.";
			}
			string str1 = str;
			if (str1 != null)
			{
				throw new WebSocketException(CloseStatusCode.ProtocolError, str1);
			}
			WebSocketFrame webSocketFrames = new WebSocketFrame()
			{
				_fin = fin,
				_rsv1 = rsv,
				_rsv2 = rsv1,
				_rsv3 = rsv2,
				_opcode = (WebSocketSharp.Opcode)num,
				_mask = mask,
				_payloadLength = num1
			};
			return webSocketFrames;
		}

		private static WebSocketFrame readExtendedPayloadLength(Stream stream, WebSocketFrame frame)
		{
			WebSocketFrame webSocketFrames;
			int extendedPayloadLengthCount = frame.ExtendedPayloadLengthCount;
			if (extendedPayloadLengthCount != 0)
			{
				byte[] numArray = stream.ReadBytes(extendedPayloadLengthCount);
				if ((int)numArray.Length != extendedPayloadLengthCount)
				{
					throw new WebSocketException("The extended payload length of a frame cannot be read from the stream.");
				}
				frame._extPayloadLength = numArray;
				webSocketFrames = frame;
			}
			else
			{
				frame._extPayloadLength = WebSocket.EmptyBytes;
				webSocketFrames = frame;
			}
			return webSocketFrames;
		}

		private static void readExtendedPayloadLengthAsync(Stream stream, WebSocketFrame frame, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			int extendedPayloadLengthCount = frame.ExtendedPayloadLengthCount;
			if (extendedPayloadLengthCount != 0)
			{
				stream.ReadBytesAsync(extendedPayloadLengthCount, (byte[] bytes) => {
					if ((int)bytes.Length != extendedPayloadLengthCount)
					{
						throw new WebSocketException("The extended payload length of a frame cannot be read from the stream.");
					}
					frame._extPayloadLength = bytes;
					completed(frame);
				}, error);
			}
			else
			{
				frame._extPayloadLength = WebSocket.EmptyBytes;
				completed(frame);
			}
		}

		internal static WebSocketFrame ReadFrame(Stream stream, bool unmask)
		{
			WebSocketFrame webSocketFrames = WebSocketFrame.readHeader(stream);
			WebSocketFrame.readExtendedPayloadLength(stream, webSocketFrames);
			WebSocketFrame.readMaskingKey(stream, webSocketFrames);
			WebSocketFrame.readPayloadData(stream, webSocketFrames);
			if (unmask)
			{
				webSocketFrames.Unmask();
			}
			return webSocketFrames;
		}

		internal static void ReadFrameAsync(Stream stream, bool unmask, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			Action<WebSocketFrame> action6 = null;
			Action<WebSocketFrame> action7 = null;
			Action<WebSocketFrame> action8 = null;
			WebSocketFrame.readHeaderAsync(stream, (WebSocketFrame frame) => {
				Stream stream2 = stream;
				WebSocketFrame webSocketFrames2 = frame;
				Action<WebSocketFrame> u003cu003e9_1 = action8;
				if (u003cu003e9_1 == null)
				{
					Action<WebSocketFrame> action4 = (WebSocketFrame frame1) => {
						Stream stream1 = stream;
						WebSocketFrame webSocketFrames1 = frame1;
						Action<WebSocketFrame> u003cu003e9_2 = action7;
						if (u003cu003e9_2 == null)
						{
							Action<WebSocketFrame> action2 = (WebSocketFrame frame2) => {
								Stream stream3 = stream;
								WebSocketFrame webSocketFrames = frame2;
								Action<WebSocketFrame> u003cu003e9_3 = action6;
								if (u003cu003e9_3 == null)
								{
									Action<WebSocketFrame> action = (WebSocketFrame frame3) => {
										if (unmask)
										{
											frame3.Unmask();
										}
										completed(frame3);
									};
									Action<WebSocketFrame> action1 = action;
									action6 = action;
									u003cu003e9_3 = action1;
								}
								WebSocketFrame.readPayloadDataAsync(stream3, webSocketFrames, u003cu003e9_3, error);
							};
							Action<WebSocketFrame> action3 = action2;
							action7 = action2;
							u003cu003e9_2 = action3;
						}
						WebSocketFrame.readMaskingKeyAsync(stream1, webSocketFrames1, u003cu003e9_2, error);
					};
					Action<WebSocketFrame> action5 = action4;
					action8 = action4;
					u003cu003e9_1 = action5;
				}
				WebSocketFrame.readExtendedPayloadLengthAsync(stream2, webSocketFrames2, u003cu003e9_1, error);
			}, error);
		}

		private static WebSocketFrame readHeader(Stream stream)
		{
			return WebSocketFrame.processHeader(stream.ReadBytes(2));
		}

		private static void readHeaderAsync(Stream stream, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			stream.ReadBytesAsync(2, (byte[] bytes) => completed(WebSocketFrame.processHeader(bytes)), error);
		}

		private static WebSocketFrame readMaskingKey(Stream stream, WebSocketFrame frame)
		{
			WebSocketFrame webSocketFrames;
			int num = (frame.IsMasked ? 4 : 0);
			if (num != 0)
			{
				byte[] numArray = stream.ReadBytes(num);
				if ((int)numArray.Length != num)
				{
					throw new WebSocketException("The masking key of a frame cannot be read from the stream.");
				}
				frame._maskingKey = numArray;
				webSocketFrames = frame;
			}
			else
			{
				frame._maskingKey = WebSocket.EmptyBytes;
				webSocketFrames = frame;
			}
			return webSocketFrames;
		}

		private static void readMaskingKeyAsync(Stream stream, WebSocketFrame frame, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			int num = (frame.IsMasked ? 4 : 0);
			if (num != 0)
			{
				stream.ReadBytesAsync(num, (byte[] bytes) => {
					if ((int)bytes.Length != num)
					{
						throw new WebSocketException("The masking key of a frame cannot be read from the stream.");
					}
					frame._maskingKey = bytes;
					completed(frame);
				}, error);
			}
			else
			{
				frame._maskingKey = WebSocket.EmptyBytes;
				completed(frame);
			}
		}

		private static WebSocketFrame readPayloadData(Stream stream, WebSocketFrame frame)
		{
			WebSocketFrame webSocketFrames;
			ulong fullPayloadLength = frame.FullPayloadLength;
			if (fullPayloadLength != (long)0)
			{
				if (fullPayloadLength > WebSocketSharp.PayloadData.MaxLength)
				{
					throw new WebSocketException(CloseStatusCode.TooBig, "A frame has a long payload length.");
				}
				long num = (long)fullPayloadLength;
				byte[] numArray = (frame._payloadLength < 127 ? stream.ReadBytes((int)fullPayloadLength) : stream.ReadBytes(num, 1024));
				if ((long)numArray.Length != num)
				{
					throw new WebSocketException("The payload data of a frame cannot be read from the stream.");
				}
				frame._payloadData = new WebSocketSharp.PayloadData(numArray, num);
				webSocketFrames = frame;
			}
			else
			{
				frame._payloadData = WebSocketSharp.PayloadData.Empty;
				webSocketFrames = frame;
			}
			return webSocketFrames;
		}

		private static void readPayloadDataAsync(Stream stream, WebSocketFrame frame, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			ulong fullPayloadLength = frame.FullPayloadLength;
			if (fullPayloadLength != (long)0)
			{
				if (fullPayloadLength > WebSocketSharp.PayloadData.MaxLength)
				{
					throw new WebSocketException(CloseStatusCode.TooBig, "A frame has a long payload length.");
				}
				long num = (long)fullPayloadLength;
				Action<byte[]> length = (byte[] bytes) => {
					if ((long)bytes.Length != num)
					{
						throw new WebSocketException("The payload data of a frame cannot be read from the stream.");
					}
					frame._payloadData = new WebSocketSharp.PayloadData(bytes, num);
					completed(frame);
				};
				if (frame._payloadLength >= 127)
				{
					stream.ReadBytesAsync(num, 1024, length, error);
				}
				else
				{
					stream.ReadBytesAsync((int)fullPayloadLength, length, error);
				}
			}
			else
			{
				frame._payloadData = WebSocketSharp.PayloadData.Empty;
				completed(frame);
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public byte[] ToArray()
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = ((byte)this._fin << (byte)WebSocketSharp.Fin.Final) + (byte)this._rsv1;
				num = (num << 1) + (byte)this._rsv2;
				num = (num << 1) + (byte)this._rsv3;
				num = (num << 4) + (byte)this._opcode;
				num = (num << 1) + (byte)this._mask;
				num = (num << 7) + this._payloadLength;
				memoryStream.Write(((ushort)num).InternalToByteArray(ByteOrder.Big), 0, 2);
				if (this._payloadLength > 125)
				{
					memoryStream.Write(this._extPayloadLength, 0, (this._payloadLength == 126 ? 2 : 8));
				}
				if (this._mask == WebSocketSharp.Mask.On)
				{
					memoryStream.Write(this._maskingKey, 0, 4);
				}
				if (this._payloadLength > 0)
				{
					byte[] numArray = this._payloadData.ToArray();
					if (this._payloadLength >= 127)
					{
						memoryStream.WriteBytes(numArray, 1024);
					}
					else
					{
						memoryStream.Write(numArray, 0, (int)numArray.Length);
					}
				}
				memoryStream.Close();
				array = memoryStream.ToArray();
			}
			return array;
		}

		public override string ToString()
		{
			return BitConverter.ToString(this.ToArray());
		}

		internal void Unmask()
		{
			if (this._mask != WebSocketSharp.Mask.Off)
			{
				this._mask = WebSocketSharp.Mask.Off;
				this._payloadData.Mask(this._maskingKey);
				this._maskingKey = WebSocket.EmptyBytes;
			}
		}
	}
}