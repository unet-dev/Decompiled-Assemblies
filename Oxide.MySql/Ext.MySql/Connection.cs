using MySql.Data.MySqlClient;
using Oxide.Core.Plugins;
using System;
using System.Runtime.CompilerServices;

namespace Oxide.Ext.MySql
{
	public sealed class Connection
	{
		internal MySqlConnection Con
		{
			get;
			set;
		}

		internal bool ConnectionPersistent
		{
			get;
			set;
		}

		internal string ConnectionString
		{
			get;
			set;
		}

		public long LastInsertRowId
		{
			get;
			internal set;
		}

		internal Oxide.Core.Plugins.Plugin Plugin
		{
			get;
			set;
		}

		public Connection(string connection, bool persistent)
		{
			this.ConnectionString = connection;
			this.ConnectionPersistent = persistent;
		}
	}
}