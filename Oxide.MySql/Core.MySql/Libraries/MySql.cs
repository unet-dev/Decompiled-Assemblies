using MySql.Data.MySqlClient;
using Oxide.Core;
using Oxide.Core.Database;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Oxide.Core.MySql.Libraries
{
	public class MySql : Library, IDatabaseProvider
	{
		private readonly Queue<Oxide.Core.MySql.Libraries.MySql.MySqlQuery> _queue = new Queue<Oxide.Core.MySql.Libraries.MySql.MySqlQuery>();

		private readonly object _syncroot = new object();

		private readonly AutoResetEvent _workevent = new AutoResetEvent(false);

		private readonly HashSet<Connection> _runningConnections = new HashSet<Connection>();

		private bool _running = true;

		private readonly Dictionary<string, Dictionary<string, Connection>> _connections = new Dictionary<string, Dictionary<string, Connection>>();

		private readonly Thread _worker;

		private readonly Dictionary<Plugin, Event.Callback<Plugin, PluginManager>> _pluginRemovedFromManager;

		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		public MySql()
		{
			this._pluginRemovedFromManager = new Dictionary<Plugin, Event.Callback<Plugin, PluginManager>>();
			this._worker = new Thread(new ThreadStart(this.Worker));
			this._worker.Start();
		}

		[LibraryFunction("CloseDb")]
		public void CloseDb(Connection db)
		{
			Dictionary<string, Connection> strs;
			Event.Callback<Plugin, PluginManager> callback;
			object name;
			object obj;
			if (db == null)
			{
				return;
			}
			Dictionary<string, Dictionary<string, Connection>> strs1 = this._connections;
			Plugin plugin = db.get_Plugin();
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
			if (strs1.TryGetValue((string)name, out strs))
			{
				strs.Remove(db.get_ConnectionString());
				if (strs.Count == 0)
				{
					Dictionary<string, Dictionary<string, Connection>> strs2 = this._connections;
					Plugin plugin1 = db.get_Plugin();
					if (plugin1 != null)
					{
						obj = plugin1.get_Name();
					}
					else
					{
						obj = null;
					}
					if (obj == null)
					{
						obj = "null";
					}
					strs2.Remove((string)obj);
					if (db.get_Plugin() != null && this._pluginRemovedFromManager.TryGetValue(db.get_Plugin(), out callback))
					{
						callback.Remove();
						this._pluginRemovedFromManager.Remove(db.get_Plugin());
					}
				}
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
			Oxide.Core.MySql.Libraries.MySql.MySqlQuery mySqlQuery = new Oxide.Core.MySql.Libraries.MySql.MySqlQuery()
			{
				Sql = sql,
				Connection = db,
				CallbackNonQuery = callback,
				NonQuery = true
			};
			lock (this._syncroot)
			{
				this._queue.Enqueue(mySqlQuery);
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
			Dictionary<string, Connection> strs;
			Event.Callback<Plugin, PluginManager> callback;
			bool state;
			object connectionString;
			object name;
			if (this._connections.TryGetValue(sender.get_Name(), out strs))
			{
				foreach (KeyValuePair<string, Connection> keyValuePair in strs)
				{
					if ((object)keyValuePair.Value.get_Plugin() != (object)sender)
					{
						continue;
					}
					DbConnection con = keyValuePair.Value.get_Con();
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
						object[] objArray = new object[2];
						DbConnection dbConnection = keyValuePair.Value.get_Con();
						if (dbConnection != null)
						{
							connectionString = dbConnection.ConnectionString;
						}
						else
						{
							connectionString = null;
						}
						objArray[0] = connectionString;
						Plugin plugin = keyValuePair.Value.get_Plugin();
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
						objArray[1] = name;
						oxide.LogWarning("Unclosed mysql connection ({0}), by plugin '{1}', closing...", objArray);
					}
					DbConnection con1 = keyValuePair.Value.get_Con();
					if (con1 != null)
					{
						con1.Close();
					}
					else
					{
					}
					keyValuePair.Value.set_Plugin(null);
				}
				this._connections.Remove(sender.get_Name());
			}
			if (this._pluginRemovedFromManager.TryGetValue(sender, out callback))
			{
				callback.Remove();
				this._pluginRemovedFromManager.Remove(sender);
			}
		}

		[LibraryFunction("OpenDb")]
		public Connection OpenDb(string host, int port, string database, string user, string password, Plugin plugin, bool persistent = false)
		{
			return this.OpenDb(string.Format("Server={0};Port={1};Database={2};User={3};Password={4};Pooling=false;default command timeout=120;Allow Zero Datetime=true;", new object[] { host, port, database, user, password }), plugin, persistent);
		}

		public Connection OpenDb(string conStr, Plugin plugin, bool persistent = false)
		{
			Dictionary<string, Connection> strs;
			Connection connection;
			object name;
			object connectionString;
			object obj;
			Dictionary<string, Dictionary<string, Connection>> strs1 = this._connections;
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
			if (!strs1.TryGetValue((string)name, out strs))
			{
				Dictionary<string, Dictionary<string, Connection>> strs2 = this._connections;
				if (plugin != null)
				{
					obj = plugin.get_Name();
				}
				else
				{
					obj = null;
				}
				if (obj == null)
				{
					obj = "null";
				}
				Dictionary<string, Connection> strs3 = new Dictionary<string, Connection>();
				strs = strs3;
				strs2[(string)obj] = strs3;
			}
			if (!strs.TryGetValue(conStr, out connection))
			{
				Connection connection1 = new Connection(conStr, persistent);
				connection1.set_Plugin(plugin);
				connection1.set_Con(new MySqlConnection(conStr));
				connection = connection1;
				strs[conStr] = connection;
			}
			else
			{
				OxideMod oxide = Interface.get_Oxide();
				object[] objArray = new object[1];
				DbConnection con = connection.get_Con();
				if (con != null)
				{
					connectionString = con.ConnectionString;
				}
				else
				{
					connectionString = null;
				}
				objArray[0] = connectionString;
				oxide.LogWarning("Already open connection ({0}), using existing instead...", objArray);
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
			Oxide.Core.MySql.Libraries.MySql.MySqlQuery mySqlQuery = new Oxide.Core.MySql.Libraries.MySql.MySqlQuery()
			{
				Sql = sql,
				Connection = db,
				Callback = callback
			};
			lock (this._syncroot)
			{
				this._queue.Enqueue(mySqlQuery);
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
				Oxide.Core.MySql.Libraries.MySql.MySqlQuery mySqlQuery = null;
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
						mySqlQuery = this._queue.Dequeue();
					}
				}
				if (mySqlQuery == null)
				{
					if (!this._running)
					{
						continue;
					}
					this._workevent.WaitOne();
				}
				else
				{
					mySqlQuery.Handle();
					if (mySqlQuery.Connection == null)
					{
						continue;
					}
					this._runningConnections.Add(mySqlQuery.Connection);
				}
			}
		}

		public class MySqlQuery
		{
			private MySqlCommand _cmd;

			private MySqlConnection _connection;

			private IAsyncResult _result;

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

			public MySqlQuery()
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

			public bool Handle()
			{
				bool flag;
				List<Dictionary<string, object>> dictionaries = null;
				int num = 0;
				long lastInsertedId = (long)0;
				try
				{
					if (this.Connection == null)
					{
						throw new Exception("Connection is null");
					}
					this._connection = (MySqlConnection)this.Connection.get_Con();
					if (this._connection.State == ConnectionState.Closed)
					{
						this._connection.Open();
					}
					this._cmd = this._connection.CreateCommand();
					this._cmd.CommandTimeout = 120;
					this._cmd.CommandText = this.Sql.get_SQL();
					Sql.AddParams(this._cmd, this.Sql.get_Arguments(), "@");
					this._result = (this.NonQuery ? this._cmd.BeginExecuteNonQuery() : this._cmd.BeginExecuteReader());
					this._result.AsyncWaitHandle.WaitOne();
					if (!this.NonQuery)
					{
						using (MySqlDataReader mySqlDataReader = this._cmd.EndExecuteReader(this._result))
						{
							dictionaries = new List<Dictionary<string, object>>();
							while (mySqlDataReader.Read() && (!this.Connection.get_ConnectionPersistent() || this.Connection.get_Con().State != ConnectionState.Closed && this.Connection.get_Con().State != ConnectionState.Broken))
							{
								Dictionary<string, object> strs = new Dictionary<string, object>();
								for (int i = 0; i < mySqlDataReader.FieldCount; i++)
								{
									strs.Add(mySqlDataReader.GetName(i), mySqlDataReader.GetValue(i));
								}
								dictionaries.Add(strs);
							}
						}
					}
					else
					{
						num = this._cmd.EndExecuteNonQuery(this._result);
					}
					lastInsertedId = this._cmd.LastInsertedId;
					this.Cleanup();
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					string str1 = "MySql handle raised an exception";
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
							this.Connection.set_LastInsertRowId(lastInsertedId);
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
						string str = "MySql command callback raised an exception";
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
				return true;
			}
		}
	}
}