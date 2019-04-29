using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;

namespace Mono.Remoting.Channels.Unix
{
	internal class HostConnectionPool
	{
		private ArrayList _pool = new ArrayList();

		private int _activeConnections;

		private string _path;

		public HostConnectionPool(string path)
		{
			this._path = path;
		}

		private void CancelConnection(UnixConnection entry)
		{
			try
			{
				entry.Stream.Close();
				this._activeConnections--;
			}
			catch
			{
			}
		}

		private UnixConnection CreateConnection()
		{
			UnixConnection unixConnection;
			try
			{
				UnixConnection unixConnection1 = new UnixConnection(this, new ReusableUnixClient(this._path));
				this._activeConnections++;
				unixConnection = unixConnection1;
			}
			catch (Exception exception)
			{
				throw new RemotingException(exception.Message);
			}
			return unixConnection;
		}

		public UnixConnection GetConnection()
		{
			UnixConnection item = null;
			ArrayList arrayLists = this._pool;
			Monitor.Enter(arrayLists);
			try
			{
				do
				{
					if (this._pool.Count > 0)
					{
						item = (UnixConnection)this._pool[this._pool.Count - 1];
						this._pool.RemoveAt(this._pool.Count - 1);
						if (!item.IsAlive)
						{
							this.CancelConnection(item);
							item = null;
							continue;
						}
					}
					if (item != null || this._activeConnections >= UnixConnectionPool.MaxOpenConnections)
					{
						if (item != null)
						{
							continue;
						}
						Monitor.Wait(this._pool);
					}
					else
					{
						break;
					}
				}
				while (item == null);
			}
			finally
			{
				Monitor.Exit(arrayLists);
			}
			if (item != null)
			{
				return item;
			}
			return this.CreateConnection();
		}

		public void PurgeConnections()
		{
			ArrayList arrayLists = this._pool;
			Monitor.Enter(arrayLists);
			try
			{
				for (int i = 0; i < this._pool.Count; i++)
				{
					UnixConnection item = (UnixConnection)this._pool[i];
					if ((DateTime.Now - item.ControlTime).TotalSeconds > (double)UnixConnectionPool.KeepAliveSeconds)
					{
						this.CancelConnection(item);
						this._pool.RemoveAt(i);
						i--;
					}
				}
			}
			finally
			{
				Monitor.Exit(arrayLists);
			}
		}

		public void ReleaseConnection(UnixConnection entry)
		{
			ArrayList arrayLists = this._pool;
			Monitor.Enter(arrayLists);
			try
			{
				entry.ControlTime = DateTime.Now;
				this._pool.Add(entry);
				Monitor.Pulse(this._pool);
			}
			finally
			{
				Monitor.Exit(arrayLists);
			}
		}
	}
}