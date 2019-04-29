using Oxide.Core;
using Oxide.Core.Database;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Oxide.Core.SQLite.Libraries
{
	public class SQLite : Library, IDatabaseProvider
	{
		private readonly string _dataDirectory;

		private readonly Queue<Oxide.Core.SQLite.Libraries.SQLite.SQLiteQuery> _queue = new Queue<Oxide.Core.SQLite.Libraries.SQLite.SQLiteQuery>();

		private readonly object _syncroot = new object();

		private readonly AutoResetEvent _workevent = new AutoResetEvent(false);

		private readonly HashSet<Connection> _runningConnections = new HashSet<Connection>();

		private bool _running = true;

		private readonly Dictionary<string, Connection> _connections = new Dictionary<string, Connection>();

		private readonly Thread _worker;

		private readonly Dictionary<Plugin, Event.Callback<Plugin, PluginManager>> _pluginRemovedFromManager;

		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		public SQLite()
		{
			this._dataDirectory = Interface.get_Oxide().get_DataDirectory();
			this._pluginRemovedFromManager = new Dictionary<Plugin, Event.Callback<Plugin, PluginManager>>();
			this._worker = new Thread(new ThreadStart(this.Worker));
			this._worker.Start();
		}

		[LibraryFunction("CloseDb")]
		public void CloseDb(Connection db)
		{
			Event.Callback<Plugin, PluginManager> callback;
			if (db == null)
			{
				return;
			}
			this._connections.Remove(db.get_ConnectionString());
			if (db.get_Plugin() != null && this._connections.Values.All<Connection>((Connection c) => (object)c.get_Plugin() != (object)db.get_Plugin()) && this._pluginRemovedFromManager.TryGetValue(db.get_Plugin(), out callback))
			{
				callback.Remove();
				this._pluginRemovedFromManager.Remove(db.get_Plugin());
			}
			DbConnection con = db.get_Con();
			if (con != null)
			{
				con.Close();
			}
			else
			{
			}
			db.set_Plugin(null);
		}

		[LibraryFunction("Delete")]
		public void Delete(Sql sql, Connection db, Action<int> callback = null)
		{
			this.ExecuteNonQuery(sql, db, callback);
		}

		[LibraryFunction("ExecuteNonQuery")]
		public void ExecuteNonQuery(Sql sql, Connection db, Action<int> callback = null)
		{
			Oxide.Core.SQLite.Libraries.SQLite.SQLiteQuery sQLiteQuery = new Oxide.Core.SQLite.Libraries.SQLite.SQLiteQuery()
			{
				Sql = sql,
				Connection = db,
				CallbackNonQuery = callback,
				NonQuery = true
			};
			lock (this._syncroot)
			{
				this._queue.Enqueue(sQLiteQuery);
			}
			this._workevent.Set();
		}

		[LibraryFunction("Insert")]
		public void Insert(Sql sql, Connection db, Action<int> callback = null)
		{
			this.ExecuteNonQuery(sql, db, callback);
		}

		[LibraryFunction("NewSql")]
		public Sql NewSql()
		{
			return Sql.get_Builder();
		}

		private void OnRemovedFromManager(Plugin sender, PluginManager manager)
		{
			Event.Callback<Plugin, PluginManager> callback;
			bool state;
			object name;
			List<string> strs = new List<string>();
			foreach (KeyValuePair<string, Connection> _connection in this._connections)
			{
				if ((object)_connection.Value.get_Plugin() != (object)sender)
				{
					continue;
				}
				DbConnection con = _connection.Value.get_Con();
				if (con != null)
				{
					state = con.State != ConnectionState.Closed;
				}
				else
				{
					state = true;
				}
				if (state)
				{
					OxideMod oxide = Interface.get_Oxide();
					object[] connectionString = new object[] { _connection.Value.get_ConnectionString(), null };
					Plugin plugin = _connection.Value.get_Plugin();
					if (plugin != null)
					{
						name = plugin.get_Name();
					}
					else
					{
						name = null;
					}
					if (name == null)
					{
						name = "null";
					}
					connectionString[1] = name;
					oxide.LogWarning("Unclosed sqlite connection ({0}), by plugin '{1}', closing...", connectionString);
				}
				DbConnection dbConnection = _connection.Value.get_Con();
				if (dbConnection != null)
				{
					dbConnection.Close();
				}
				else
				{
				}
				_connection.Value.set_Plugin(null);
				strs.Add(_connection.Key);
			}
			foreach (string str in strs)
			{
				this._connections.Remove(str);
			}
			if (this._pluginRemovedFromManager.TryGetValue(sender, out callback))
			{
				callback.Remove();
				this._pluginRemovedFromManager.Remove(sender);
			}
		}

		[LibraryFunction("OpenDb")]
		public Connection OpenDb(string file, Plugin plugin, bool persistent = false)
		{
			Connection connection;
			if (string.IsNullOrEmpty(file))
			{
				return null;
			}
			string str = Path.Combine(this._dataDirectory, file);
			if (!str.StartsWith(this._dataDirectory, StringComparison.Ordinal))
			{
				throw new Exception("Only access to oxide directory!");
			}
			string str1 = string.Format("Data Source={0};Version=3;", str);
			if (!this._connections.TryGetValue(str1, out connection))
			{
				Connection connection1 = new Connection(str1, persistent);
				connection1.set_Plugin(plugin);
				connection1.set_Con(new SQLiteConnection(str1));
				connection = connection1;
				this._connections[str1] = connection;
			}
			else
			{
				if ((object)plugin != (object)connection.get_Plugin())
				{
					Interface.get_Oxide().LogWarning("Already open connection ({0}), by plugin '{1}'...", new object[] { str1, connection.get_Plugin() });
					return null;
				}
				Interface.get_Oxide().LogWarning("Already open connection ({0}), using existing instead...", new object[] { str1 });
			}
			if (plugin != null && !this._pluginRemovedFromManager.ContainsKey(plugin))
			{
				this._pluginRemovedFromManager[plugin] = plugin.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.OnRemovedFromManager));
			}
			return connection;
		}

		[LibraryFunction("Query")]
		public void Query(Sql sql, Connection db, Action<List<Dictionary<string, object>>> callback)
		{
			Oxide.Core.SQLite.Libraries.SQLite.SQLiteQuery sQLiteQuery = new Oxide.Core.SQLite.Libraries.SQLite.SQLiteQuery()
			{
				Sql = sql,
				Connection = db,
				Callback = callback
			};
			lock (this._syncroot)
			{
				this._queue.Enqueue(sQLiteQuery);
			}
			this._workevent.Set();
		}

		public override void Shutdown()
		{
			this._running = false;
			this._workevent.Set();
			this._worker.Join();
		}

		[LibraryFunction("Update")]
		public void Update(Sql sql, Connection db, Action<int> callback = null)
		{
			this.ExecuteNonQuery(sql, db, callback);
		}

		private void Worker()
		{
			while (this._running || this._queue.Count > 0)
			{
				Oxide.Core.SQLite.Libraries.SQLite.SQLiteQuery sQLiteQuery = null;
				lock (this._syncroot)
				{
					if (this._queue.Count <= 0)
					{
						foreach (Connection _runningConnection in this._runningConnections)
						{
							if (_runningConnection == null || _runningConnection.get_ConnectionPersistent())
							{
								continue;
							}
							this.CloseDb(_runningConnection);
						}
						this._runningConnections.Clear();
					}
					else
					{
						sQLiteQuery = this._queue.Dequeue();
					}
				}
				if (sQLiteQuery == null)
				{
					if (!this._running)
					{
						continue;
					}
					this._workevent.WaitOne();
				}
				else
				{
					sQLiteQuery.Handle();
					if (sQLiteQuery.Connection == null)
					{
						continue;
					}
					this._runningConnections.Add(sQLiteQuery.Connection);
				}
			}
		}

		public class SQLiteQuery
		{
			private SQLiteCommand _cmd;

			private SQLiteConnection _connection;

			public Action<List<Dictionary<string, object>>> Callback
			{
				get;
				internal set;
			}

			public Action<int> CallbackNonQuery
			{
				get;
				internal set;
			}

			public Connection Connection
			{
				get;
				internal set;
			}

			public bool NonQuery
			{
				get;
				internal set;
			}

			public Sql Sql
			{
				get;
				internal set;
			}

			public SQLiteQuery()
			{
			}

			private void Cleanup()
			{
				if (this._cmd != null)
				{
					this._cmd.Dispose();
					this._cmd = null;
				}
				this._connection = null;
			}

			public void Handle()
			{
				bool flag;
				List<Dictionary<string, object>> dictionaries = null;
				int num = 0;
				long lastInsertRowId = (long)0;
				try
				{
					if (this.Connection == null)
					{
						throw new Exception("Connection is null");
					}
					this._connection = (SQLiteConnection)this.Connection.get_Con();
					if (this._connection.State == ConnectionState.Closed)
					{
						this._connection.Open();
					}
					this._cmd = this._connection.CreateCommand();
					this._cmd.CommandText = this.Sql.get_SQL();
					Sql.AddParams(this._cmd, this.Sql.get_Arguments(), "@");
					if (!this.NonQuery)
					{
						using (SQLiteDataReader sQLiteDataReader = this._cmd.ExecuteReader())
						{
							dictionaries = new List<Dictionary<string, object>>();
							while (sQLiteDataReader.Read())
							{
								Dictionary<string, object> strs = new Dictionary<string, object>();
								for (int i = 0; i < sQLiteDataReader.FieldCount; i++)
								{
									strs.Add(sQLiteDataReader.GetName(i), sQLiteDataReader.GetValue(i));
								}
								dictionaries.Add(strs);
							}
						}
					}
					else
					{
						num = this._cmd.ExecuteNonQuery();
					}
					lastInsertRowId = this._connection.LastInsertRowId;
					this.Cleanup();
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					string str1 = "Sqlite handle raised an exception";
					Connection connection3 = this.Connection;
					if (connection3 != null)
					{
						flag = connection3.get_Plugin();
					}
					else
					{
						flag = false;
					}
					if (flag)
					{
						str1 = string.Concat(str1, string.Format(" in '{0} v{1}' plugin", this.Connection.get_Plugin().get_Name(), this.Connection.get_Plugin().get_Version()));
					}
					Interface.get_Oxide().LogException(str1, exception2);
					this.Cleanup();
				}
				Interface.get_Oxide().NextTick(() => {
					bool plugin;
					Connection connection = this.Connection;
					if (connection != null)
					{
						Plugin plugin1 = connection.get_Plugin();
						if (plugin1 != null)
						{
							plugin1.TrackStart();
						}
						else
						{
						}
					}
					else
					{
					}
					try
					{
						if (this.Connection != null)
						{
							this.Connection.set_LastInsertRowId(lastInsertRowId);
						}
						if (this.NonQuery)
						{
							Action<int> callbackNonQuery = this.CallbackNonQuery;
							if (callbackNonQuery != null)
							{
								callbackNonQuery(num);
							}
							else
							{
							}
						}
						else
						{
							this.Callback(dictionaries);
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						string str = "Sqlite command callback raised an exception";
						Connection connection1 = this.Connection;
						if (connection1 != null)
						{
							plugin = connection1.get_Plugin();
						}
						else
						{
							plugin = false;
						}
						if (plugin)
						{
							str = string.Concat(str, string.Format(" in '{0} v{1}' plugin", this.Connection.get_Plugin().get_Name(), this.Connection.get_Plugin().get_Version()));
						}
						Interface.get_Oxide().LogException(str, exception);
					}
					Connection connection2 = this.Connection;
					if (connection2 == null)
					{
						return;
					}
					Plugin plugin2 = connection2.get_Plugin();
					if (plugin2 == null)
					{
						return;
					}
					plugin2.TrackEnd();
				});
			}
		}
	}
}