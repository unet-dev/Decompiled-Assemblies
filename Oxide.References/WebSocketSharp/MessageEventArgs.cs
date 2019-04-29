using System;

namespace WebSocketSharp
{
	public class MessageEventArgs : EventArgs
	{
		private string _data;

		private bool _dataSet;

		private WebSocketSharp.Opcode _opcode;

		private byte[] _rawData;

		public string Data
		{
			get
			{
				if (!this._dataSet)
				{
					this._data = (this._opcode != WebSocketSharp.Opcode.Binary ? this._rawData.UTF8Decode() : BitConverter.ToString(this._rawData));
					this._dataSet = true;
				}
				return this._data;
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

		public bool IsPing
		{
			get
			{
				return this._opcode == WebSocketSharp.Opcode.Ping;
			}
		}

		public bool IsText
		{
			get
			{
				return this._opcode == WebSocketSharp.Opcode.Text;
			}
		}

		internal WebSocketSharp.Opcode Opcode
		{
			get
			{
				return this._opcode;
			}
		}

		public byte[] RawData
		{
			get
			{
				return this._rawData;
			}
		}

		internal MessageEventArgs(WebSocketFrame frame)
		{
			this._opcode = frame.Opcode;
			this._rawData = frame.PayloadData.ApplicationData;
		}

		internal MessageEventArgs(WebSocketSharp.Opcode opcode, byte[] rawData)
		{
			if ((long)rawData.Length > PayloadData.MaxLength)
			{
				throw new WebSocketException(CloseStatusCode.TooBig);
			}
			this._opcode = opcode;
			this._rawData = rawData;
		}
	}
}