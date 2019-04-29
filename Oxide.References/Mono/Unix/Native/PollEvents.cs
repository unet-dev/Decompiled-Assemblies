using System;

namespace Mono.Unix.Native
{
	[Flags]
	[Map]
	public enum PollEvents : short
	{
		POLLIN = 1,
		POLLPRI = 2,
		POLLOUT = 4,
		POLLERR = 8,
		POLLHUP = 16,
		POLLNVAL = 32,
		POLLRDNORM = 64,
		POLLRDBAND = 128,
		POLLWRNORM = 256,
		POLLWRBAND = 512
	}
}