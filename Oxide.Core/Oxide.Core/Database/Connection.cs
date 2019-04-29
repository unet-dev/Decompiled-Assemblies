using Oxide.Core.Plugins;
using System;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Database
{
	public sealed class Connection
	{
		public DbConnection Con
		{
			get;
			set;
		}

		public bool ConnectionPersistent
		{
			get;
			set;
		}

		public string ConnectionString
		{
			get;
			set;
		}

		public long LastInsertRowId
		{
			get;
			set;
		}

		public Oxide.Core.Plugins.Plugin Plugin
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