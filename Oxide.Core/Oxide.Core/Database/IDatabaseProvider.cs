using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;

namespace Oxide.Core.Database
{
	public interface IDatabaseProvider
	{
		void CloseDb(Connection db);

		void Delete(Sql sql, Connection db, Action<int> callback = null);

		void ExecuteNonQuery(Sql sql, Connection db, Action<int> callback = null);

		void Insert(Sql sql, Connection db, Action<int> callback = null);

		Sql NewSql();

		Connection OpenDb(string file, Plugin plugin, bool persistent = false);

		void Query(Sql sql, Connection db, Action<List<Dictionary<string, object>>> callback);

		void Update(Sql sql, Connection db, Action<int> callback = null);
	}
}