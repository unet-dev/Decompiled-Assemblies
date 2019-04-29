using System;

namespace Facepunch.Sqlite
{
	public class SqliteException : Exception
	{
		public SqliteException(string message) : base(message)
		{
		}
	}
}