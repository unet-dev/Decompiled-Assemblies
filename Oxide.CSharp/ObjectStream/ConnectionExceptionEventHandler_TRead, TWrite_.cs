using System;

namespace ObjectStream
{
	public delegate void ConnectionExceptionEventHandler<TRead, TWrite>(ObjectStreamConnection<TRead, TWrite> connection, Exception exception)
	where TRead : class
	where TWrite : class;
}