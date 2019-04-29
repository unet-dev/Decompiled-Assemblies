using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace WebSocketSharp.Net
{
	internal class ChunkStream
	{
		private int _chunkRead;

		private int _chunkSize;

		private List<Chunk> _chunks;

		private bool _gotIt;

		private WebSocketSharp.Net.WebHeaderCollection _headers;

		private StringBuilder _saved;

		private bool _sawCr;

		private InputChunkState _state;

		private int _trailerState;

		public int ChunkLeft
		{
			get
			{
				return this._chunkSize - this._chunkRead;
			}
		}

		internal WebSocketSharp.Net.WebHeaderCollection Headers
		{
			get
			{
				return this._headers;
			}
		}

		public bool WantMore
		{
			get
			{
				return this._state != InputChunkState.End;
			}
		}

		public ChunkStream(WebSocketSharp.Net.WebHeaderCollection headers)
		{
			this._headers = headers;
			this._chunkSize = -1;
			this._chunks = new List<Chunk>();
			this._saved = new StringBuilder();
		}

		public ChunkStream(byte[] buffer, int offset, int count, WebSocketSharp.Net.WebHeaderCollection headers) : this(headers)
		{
			this.Write(buffer, offset, count);
		}

		private int read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			int num1 = this._chunks.Count;
			for (int i = 0; i < num1; i++)
			{
				Chunk item = this._chunks[i];
				if (item != null)
				{
					if (item.ReadLeft != 0)
					{
						num += item.Read(buffer, offset + num, count - num);
						if (num == count)
						{
							break;
						}
					}
					else
					{
						this._chunks[i] = null;
					}
				}
			}
			return num;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num;
			num = (count > 0 ? this.read(buffer, offset, count) : 0);
			return num;
		}

		private static string removeChunkExtension(string value)
		{
			int num = value.IndexOf(';');
			return (num > -1 ? value.Substring(0, num) : value);
		}

		internal void ResetBuffer()
		{
			this._chunkRead = 0;
			this._chunkSize = -1;
			this._chunks.Clear();
		}

		private InputChunkState seekCrLf(byte[] buffer, ref int offset, int length)
		{
			int num;
			InputChunkState inputChunkState;
			if (!this._sawCr)
			{
				num = offset;
				offset = num + 1;
				if (buffer[num] != 13)
				{
					ChunkStream.throwProtocolViolation("CR is expected.");
				}
				this._sawCr = true;
				if (offset == length)
				{
					inputChunkState = InputChunkState.DataEnded;
					return inputChunkState;
				}
			}
			num = offset;
			offset = num + 1;
			if (buffer[num] != 10)
			{
				ChunkStream.throwProtocolViolation("LF is expected.");
			}
			inputChunkState = InputChunkState.None;
			return inputChunkState;
		}

		private InputChunkState setChunkSize(byte[] buffer, ref int offset, int length)
		{
			InputChunkState inputChunkState;
			byte num = 0;
			while (offset < length)
			{
				int num1 = offset;
				offset = num1 + 1;
				num = buffer[num1];
				if (this._sawCr)
				{
					if (num != 10)
					{
						ChunkStream.throwProtocolViolation("LF is expected.");
					}
					break;
				}
				else if (num != 13)
				{
					if (num == 10)
					{
						ChunkStream.throwProtocolViolation("LF is unexpected.");
					}
					if (num == 32)
					{
						this._gotIt = true;
					}
					if (!this._gotIt)
					{
						this._saved.Append((char)num);
					}
					if (this._saved.Length > 20)
					{
						ChunkStream.throwProtocolViolation("The chunk size is too long.");
					}
				}
				else
				{
					this._sawCr = true;
				}
			}
			if ((!this._sawCr ? false : num == 10))
			{
				this._chunkRead = 0;
				try
				{
					this._chunkSize = int.Parse(ChunkStream.removeChunkExtension(this._saved.ToString()), NumberStyles.HexNumber);
				}
				catch
				{
					ChunkStream.throwProtocolViolation("The chunk size cannot be parsed.");
				}
				if (this._chunkSize != 0)
				{
					inputChunkState = InputChunkState.Data;
				}
				else
				{
					this._trailerState = 2;
					inputChunkState = InputChunkState.Trailer;
				}
			}
			else
			{
				inputChunkState = InputChunkState.None;
			}
			return inputChunkState;
		}

		private InputChunkState setTrailer(byte[] buffer, ref int offset, int length)
		{
			InputChunkState inputChunkState;
			if ((this._trailerState != 2 || buffer[offset] != 13 ? false : this._saved.Length == 0))
			{
				offset++;
				if ((offset >= length ? false : buffer[offset] == 10))
				{
					offset++;
					inputChunkState = InputChunkState.End;
					return inputChunkState;
				}
				offset--;
			}
			while (true)
			{
				if ((offset >= length ? true : this._trailerState >= 4))
				{
					break;
				}
				int num = offset;
				offset = num + 1;
				byte num1 = buffer[num];
				this._saved.Append((char)num1);
				if (this._saved.Length > 4196)
				{
					ChunkStream.throwProtocolViolation("The trailer is too long.");
				}
				if ((this._trailerState == 1 ? true : this._trailerState == 3))
				{
					if (num1 != 10)
					{
						ChunkStream.throwProtocolViolation("LF is expected.");
					}
					this._trailerState++;
				}
				else if (num1 != 13)
				{
					if (num1 == 10)
					{
						ChunkStream.throwProtocolViolation("LF is unexpected.");
					}
					this._trailerState = 0;
				}
				else
				{
					this._trailerState++;
				}
			}
			if (this._trailerState >= 4)
			{
				StringBuilder stringBuilder = this._saved;
				stringBuilder.Length = stringBuilder.Length - 2;
				StringReader stringReader = new StringReader(this._saved.ToString());
				while (true)
				{
					string str = stringReader.ReadLine();
					string str1 = str;
					if ((str == null ? true : str1.Length <= 0))
					{
						break;
					}
					this._headers.Add(str1);
				}
				inputChunkState = InputChunkState.End;
			}
			else
			{
				inputChunkState = InputChunkState.Trailer;
			}
			return inputChunkState;
		}

		private static void throwProtocolViolation(string message)
		{
			throw new WebException(message, null, WebExceptionStatus.ServerProtocolViolation, null);
		}

		private void write(byte[] buffer, ref int offset, int length)
		{
			if (this._state == InputChunkState.End)
			{
				ChunkStream.throwProtocolViolation("The chunks were ended.");
			}
			if (this._state == InputChunkState.None)
			{
				this._state = this.setChunkSize(buffer, ref offset, length);
				if (this._state == InputChunkState.None)
				{
					return;
				}
				this._saved.Length = 0;
				this._sawCr = false;
				this._gotIt = false;
			}
			if ((this._state != InputChunkState.Data ? false : offset < length))
			{
				this._state = this.writeData(buffer, ref offset, length);
				if (this._state == InputChunkState.Data)
				{
					return;
				}
			}
			if ((this._state != InputChunkState.DataEnded ? false : offset < length))
			{
				this._state = this.seekCrLf(buffer, ref offset, length);
				if (this._state == InputChunkState.DataEnded)
				{
					return;
				}
				this._sawCr = false;
			}
			if ((this._state != InputChunkState.Trailer ? false : offset < length))
			{
				this._state = this.setTrailer(buffer, ref offset, length);
				if (this._state == InputChunkState.Trailer)
				{
					return;
				}
				this._saved.Length = 0;
			}
			if (offset < length)
			{
				this.write(buffer, ref offset, length);
			}
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			if (count > 0)
			{
				this.write(buffer, ref offset, offset + count);
			}
		}

		internal int WriteAndReadBack(byte[] buffer, int offset, int writeCount, int readCount)
		{
			this.Write(buffer, offset, writeCount);
			return this.Read(buffer, offset, readCount);
		}

		private InputChunkState writeData(byte[] buffer, ref int offset, int length)
		{
			int num = length - offset;
			int num1 = this._chunkSize - this._chunkRead;
			if (num > num1)
			{
				num = num1;
			}
			byte[] numArray = new byte[num];
			Buffer.BlockCopy(buffer, offset, numArray, 0, num);
			this._chunks.Add(new Chunk(numArray));
			offset += num;
			this._chunkRead += num;
			return (this._chunkRead == this._chunkSize ? InputChunkState.DataEnded : InputChunkState.Data);
		}
	}
}