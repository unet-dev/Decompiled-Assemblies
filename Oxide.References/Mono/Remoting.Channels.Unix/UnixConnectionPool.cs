using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Threading;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixConnectionPool
	{
		private static Hashtable _pools;

		private static int _maxOpenConnections;

		private static int _keepAliveSeconds;

		private static Thread _poolThread;

		public static int KeepAliveSeconds
		{
			get
			{
				return UnixConnectionPool._keepAliveSeconds;
			}
			set
			{
				UnixConnectionPool._keepAliveSeconds = value;
			}
		}

		public static int MaxOpenConnections
		{
			get
			{
				return UnixConnectionPool._maxOpenConnections;
			}
			set
			{
				if (value < 1)
				{
					throw new RemotingException("MaxOpenConnections must be greater than zero");
				}
				UnixConnectionPool._maxOpenConnections = value;
			}
		}

		static UnixConnectionPool()
		{
			UnixConnectionPool._pools = new Hashtable();
			UnixConnectionPool._maxOpenConnections = 2147483647;
			UnixConnectionPool._keepAliveSeconds = 15;
			UnixConnectionPool._poolThread = new Thread(new ThreadStart(UnixConnectionPool.ConnectionCollector));
			UnixConnectionPool._poolThread.Start();
			UnixConnectionPool._poolThread.IsBackground = true;
		}

		public UnixConnectionPool()
		{
		}

		private static void ConnectionCollector()
		{
			while (true)
			{
				Thread.Sleep(3000);
				Hashtable hashtables = UnixConnectionPool._pools;
				Monitor.Enter(hashtables);
				try
				{
					IEnumerator enumerator = UnixConnectionPool._pools.Values.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							((HostConnectionPool)enumerator.Current).PurgeConnections();
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable == null)
						{
						}
						disposable.Dispose();
					}
				}
				finally
				{
					Monitor.Exit(hashtables);
				}
			}
		}

		public static UnixConnection GetConnection(string path)
		{
			HostConnectionPool item;
			Hashtable hashtables = UnixConnectionPool._pools;
			Monitor.Enter(hashtables);
			try
			{
				item = (HostConnectionPool)UnixConnectionPool._pools[path];
				if (item == null)
				{
					item = new HostConnectionPool(path);
					UnixConnectionPool._pools[path] = item;
				}
			}
			finally
			{
				Monitor.Exit(hashtables);
			}
			return item.GetConnection();
		}

		public static void Shutdown()
		{
			if (UnixConnectionPool._poolThread != null)
			{
				UnixConnectionPool._poolThread.Abort();
			}
		}
	}
}