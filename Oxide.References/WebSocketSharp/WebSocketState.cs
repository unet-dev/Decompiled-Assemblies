using System;

namespace WebSocketSharp
{
	public enum WebSocketState : ushort
	{
		Connecting,
		Open,
		Closing,
		Closed
	}
}