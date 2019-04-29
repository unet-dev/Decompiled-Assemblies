using System;

namespace Mono.Remoting.Channels.Unix
{
	internal enum MessageStatus
	{
		MethodMessage = 0,
		CancelSignal = 1,
		Unknown = 10
	}
}