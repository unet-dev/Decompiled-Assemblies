using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WebSocketSharp
{
	internal class PayloadData : IEnumerable<byte>, IEnumerable
	{
		private ushort _code;

		private bool _codeSet;

		private byte[] _data;

		private long _extDataLength;

		private long _length;

		private string _reason;

		private bool _reasonSet;

		public readonly static PayloadData Empty;

		public readonly static ulong MaxLength;

		public byte[] ApplicationData
		{
			get
			{
				return (this._extDataLength > (long)0 ? this._data.SubArray<byte>(this._extDataLength, this._length - this._extDataLength) : this._data);
			}
		}

		internal ushort Code
		{
			get
			{
				ushort num;
				if (!this._codeSet)
				{
					if (this._length > (long)1)
					{
						num = this._data.SubArray<byte>(0, 2).ToUInt16(ByteOrder.Big);
					}
					else
					{
						num = 1005;
					}
					this._code = num;
					this._codeSet = true;
				}
				return this._code;
			}
		}

		public byte[] ExtensionData
		{
			get
			{
				return (this._extDataLength > (long)0 ? this._data.SubArray<byte>((long)0, this._extDataLength) : WebSocket.EmptyBytes);
			}
		}

		internal long ExtensionDataLength
		{
			get
			{
				return this._extDataLength;
			}
			set
			{
				this._extDataLength = value;
			}
		}

		internal bool HasReservedCode
		{
			get
			{
				return (this._length <= (long)1 ? false : this.Code.IsReserved());
			}
		}

		public ulong Length
		{
			get
			{
				return (ulong)this._length;
			}
		}

		internal string Reason
		{
			get
			{
				if (!this._reasonSet)
				{
					this._reason = (this._length > (long)2 ? this._data.SubArray<byte>((long)2, this._length - (long)2).UTF8Decode() : string.Empty);
					this._reasonSet = true;
				}
				return this._reason;
			}
		}

		static PayloadData()
		{
			PayloadData.Empty = new PayloadData();
			PayloadData.MaxLength = 9223372036854775807L;
		}

		internal PayloadData()
		{
			this._code = 1005;
			this._reason = string.Empty;
			this._data = WebSocket.EmptyBytes;
			this._codeSet = true;
			this._reasonSet = true;
		}

		internal PayloadData(byte[] data) : this(data, (long)data.Length)
		{
		}

		internal PayloadData(byte[] data, long length)
		{
			this._data = data;
			this._length = length;
		}

		internal PayloadData(ushort code, string reason)
		{
			this._code = code;
			this._reason = reason ?? string.Empty;
			this._data = code.Append(reason);
			this._length = (long)this._data.Length;
			this._codeSet = true;
			this._reasonSet = true;
		}

		public IEnumerator<byte> GetEnumerator()
		{
			byte[] numArray = this._data;
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				yield return numArray[i];
			}
			numArray = null;
		}

		internal void Mask(byte[] key)
		{
			for (long i = (long)0; i < this._length; i += (long)1)
			{
				this._data[checked((IntPtr)i)] = (byte)(this._data[checked((IntPtr)i)] ^ key[checked((IntPtr)(i % (long)4))]);
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public byte[] ToArray()
		{
			return this._data;
		}

		public override string ToString()
		{
			return BitConverter.ToString(this._data);
		}
	}
}