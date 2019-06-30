using System;

namespace Network
{
	public enum SendMethod
	{
		Reliable,
		ReliableUnordered,
		ReliableSequenced,
		Unreliable,
		UnreliableSequenced
	}
}