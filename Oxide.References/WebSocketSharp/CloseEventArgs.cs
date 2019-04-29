using System;

namespace WebSocketSharp
{
	public class CloseEventArgs : EventArgs
	{
		private bool _clean;

		private WebSocketSharp.PayloadData _payloadData;

		public ushort Code
		{
			get
			{
				return this._payloadData.Code;
			}
		}

		internal WebSocketSharp.PayloadData PayloadData
		{
			get
			{
				return this._payloadData;
			}
		}

		public string Reason
		{
			get
			{
				return this._payloadData.Reason ?? string.Empty;
			}
		}

		public bool WasClean
		{
			get
			{
				return this._clean;
			}
			internal set
			{
				this._clean = value;
			}
		}

		internal CloseEventArgs()
		{
			this._payloadData = WebSocketSharp.PayloadData.Empty;
		}

		internal CloseEventArgs(ushort code) : this(code, null)
		{
		}

		internal CloseEventArgs(CloseStatusCode code) : this((ushort)code, null)
		{
		}

		internal CloseEventArgs(WebSocketSharp.PayloadData payloadData)
		{
			this._payloadData = payloadData;
		}

		internal CloseEventArgs(ushort code, string reason)
		{
			this._payloadData = new WebSocketSharp.PayloadData(code, reason);
		}

		internal CloseEventArgs(CloseStatusCode code, string reason) : this((ushort)code, reason)
		{
		}
	}
}