using System;

namespace ObjectStream
{
	public delegate void ConnectionMessageEventHandler<TRead, TWrite>(ObjectStreamConnection<TRead, TWrite> connection, TRead message)
	where TRead : class
	where TWrite : class;
}