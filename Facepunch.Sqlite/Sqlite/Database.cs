using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Facepunch.Sqlite
{
	public class Database
	{
		private IntPtr _connection;

		private bool IsConnectionOpen
		{
			get;
			set;
		}

		public Database()
		{
		}

		private unsafe void Bind(IntPtr stmHandle, int index, object value)
		{
			// 
			// Current member / type: System.Void Facepunch.Sqlite.Database::Bind(System.IntPtr,System.Int32,System.Object)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Sqlite.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void Bind(System.IntPtr,System.Int32,System.Object)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public void Close()
		{
			if (this.IsConnectionOpen)
			{
				Interop.sqlite3_close(this._connection);
			}
			this.IsConnectionOpen = false;
		}

		public void Execute(string query, params object[] arguments)
		{
			if (!this.IsConnectionOpen)
			{
				throw new SqliteException("SQLite database is not open.");
			}
			IntPtr intPtr = this.Prepare(query);
			this.PushArguments(arguments, intPtr);
			if (Interop.sqlite3_step(intPtr) != 101)
			{
				throw new SqliteException("Could not execute SQL statement.");
			}
			this.Finalize(intPtr);
		}

		private void Finalize(IntPtr stmHandle)
		{
			if (Interop.sqlite3_finalize(stmHandle) != 0)
			{
				throw new SqliteException("Could not finalize SQL statement.");
			}
		}

		private object GetColumnValue(IntPtr stmHandle, int i)
		{
			switch (Interop.sqlite3_column_type(stmHandle, i))
			{
				case 1:
				{
					return Interop.sqlite3_column_int64(stmHandle, i);
				}
				case 2:
				{
					return Interop.sqlite3_column_double(stmHandle, i);
				}
				case 3:
				{
					return Marshal.PtrToStringUni(Interop.sqlite3_column_text16(stmHandle, i));
				}
				case 4:
				{
					IntPtr intPtr = Interop.sqlite3_column_blob(stmHandle, i);
					if (intPtr == IntPtr.Zero)
					{
						return null;
					}
					int num = Interop.sqlite3_column_bytes(stmHandle, i);
					byte[] numArray = new byte[num];
					Marshal.Copy(intPtr, numArray, 0, num);
					return numArray;
				}
				case 5:
				{
					return null;
				}
			}
			return null;
		}

		private Row GetRow(IntPtr stmHandle, string[] columnNames)
		{
			Row row = new Row();
			for (int i = 0; i < (int)columnNames.Length; i++)
			{
				string str = columnNames[i];
				row[str] = this.GetColumnValue(stmHandle, i);
			}
			return row;
		}

		public void Open(string path)
		{
			if (this.IsConnectionOpen)
			{
				throw new SqliteException("There is already an open connection");
			}
			if (Interop.sqlite3_open(path, out this._connection) != 0)
			{
				throw new SqliteException(string.Concat("Could not open database file: ", path));
			}
			this.IsConnectionOpen = true;
		}

		private IntPtr Prepare(string query)
		{
			IntPtr intPtr;
			if (Interop.sqlite3_prepare_v2(this._connection, query, query.Length, out intPtr, IntPtr.Zero) != 0)
			{
				throw new SqliteException(Marshal.PtrToStringAnsi(Interop.sqlite3_errmsg(this._connection)));
			}
			return intPtr;
		}

		private void PushArguments(object[] arguments, IntPtr stmHandle)
		{
			if (arguments == null)
			{
				return;
			}
			for (int i = 0; i < (int)arguments.Length; i++)
			{
				this.Bind(stmHandle, i + 1, arguments[i]);
			}
		}

		public Table Query(string query, params object[] arguments)
		{
			if (!this.IsConnectionOpen)
			{
				throw new SqliteException("SQLite database is not open.");
			}
			IntPtr intPtr = this.Prepare(query);
			this.PushArguments(arguments, intPtr);
			int num = Interop.sqlite3_column_count(intPtr);
			Table table = new Table();
			string[] stringAnsi = new string[num];
			for (int i = 0; i < num; i++)
			{
				stringAnsi[i] = Marshal.PtrToStringAnsi(Interop.sqlite3_column_name(intPtr, i));
			}
			int num1 = 0;
			while (Interop.sqlite3_step(intPtr) == 100)
			{
				table.Add(num1, this.GetRow(intPtr, stringAnsi));
				num1++;
			}
			this.Finalize(intPtr);
			return table;
		}

		public byte[] QueryBlob(string query, params object[] arguments)
		{
			return this.QueryValue<byte[]>(query, arguments);
		}

		public int QueryInt(string query, params object[] arguments)
		{
			return (int)this.QueryValue<long>(query, arguments);
		}

		public Row QueryRow(string query, params object[] arguments)
		{
			if (!this.IsConnectionOpen)
			{
				throw new SqliteException("SQLite database is not open.");
			}
			IntPtr intPtr = this.Prepare(query);
			this.PushArguments(arguments, intPtr);
			int num = Interop.sqlite3_column_count(intPtr);
			string[] stringAnsi = new string[num];
			for (int i = 0; i < num; i++)
			{
				stringAnsi[i] = Marshal.PtrToStringAnsi(Interop.sqlite3_column_name(intPtr, i));
			}
			Row row = null;
			if (Interop.sqlite3_step(intPtr) == 100)
			{
				row = this.GetRow(intPtr, stringAnsi);
			}
			this.Finalize(intPtr);
			return row;
		}

		private T QueryValue<T>(string query, object[] arguments)
		{
			T columnValue;
			if (!this.IsConnectionOpen)
			{
				throw new SqliteException("SQLite database is not open.");
			}
			IntPtr intPtr = this.Prepare(query);
			this.PushArguments(arguments, intPtr);
			try
			{
				if (Interop.sqlite3_step(intPtr) == 100)
				{
					columnValue = (T)this.GetColumnValue(intPtr, 0);
				}
				else
				{
					return default(T);
				}
			}
			finally
			{
				this.Finalize(intPtr);
			}
			return columnValue;
		}

		public string Safe(string str)
		{
			if (!str.Contains<char>('\''))
			{
				return str;
			}
			return str.Replace("'", "''");
		}

		public bool TableExists(string name)
		{
			return this.QueryInt(string.Concat("select count(type) from sqlite_master where type='table' and name='", this.Safe(name), "';"), Array.Empty<object>()) > 0;
		}
	}
}