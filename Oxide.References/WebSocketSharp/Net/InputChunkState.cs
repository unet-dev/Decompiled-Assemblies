using System;

namespace WebSocketSharp.Net
{
	internal enum InputChunkState
	{
		None,
		Data,
		DataEnded,
		Trailer,
		End
	}
}