using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Oxide.Core.Database
{
	public class Sql
	{
		private readonly static Regex Filter;

		private readonly static Regex RxParams;

		private readonly object[] _args;

		private readonly string _sql;

		private object[] _argsFinal;

		private Sql _rhs;

		private string _sqlFinal;

		public object[] Arguments
		{
			get
			{
				this.Build();
				return this._argsFinal;
			}
		}

		public static Sql Builder
		{
			get
			{
				return new Sql();
			}
		}

		public string SQL
		{
			get
			{
				this.Build();
				return this._sqlFinal;
			}
		}

		static Sql()
		{
			Sql.Filter = new Regex("LOAD\\s*DATA|INTO\\s*(OUTFILE|DUMPFILE)|LOAD_FILE", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			Sql.RxParams = new Regex("(?<!@)@\\w+", RegexOptions.Compiled);
		}

		public Sql()
		{
		}

		public Sql(string sql, params object[] args)
		{
			this._sql = sql;
			this._args = args;
		}

		public static void AddParam(IDbCommand cmd, object item, string parameterPrefix)
		{
			IDbDataParameter dbDataParameter = item as IDbDataParameter;
			if (dbDataParameter != null)
			{
				dbDataParameter.ParameterName = string.Format("{0}{1}", parameterPrefix, cmd.Parameters.Count);
				cmd.Parameters.Add(dbDataParameter);
				return;
			}
			IDbDataParameter str = cmd.CreateParameter();
			str.ParameterName = string.Format("{0}{1}", parameterPrefix, cmd.Parameters.Count);
			if (item != null)
			{
				Type type = item.GetType();
				if (type.IsEnum)
				{
					str.Value = (int)item;
				}
				else if (type == typeof(Guid))
				{
					str.Value = item.ToString();
					str.DbType = DbType.String;
					str.Size = 40;
				}
				else if (type == typeof(string))
				{
					str.Size = Math.Max(((string)item).Length + 1, 4000);
					str.Value = item;
				}
				else if (type != typeof(bool))
				{
					str.Value = item;
				}
				else
				{
					str.Value = ((bool)item ? 1 : 0);
				}
			}
			else
			{
				str.Value = DBNull.Value;
			}
			cmd.Parameters.Add(str);
		}

		public static void AddParams(IDbCommand cmd, object[] items, string parameterPrefix)
		{
			object[] objArray = items;
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				Sql.AddParam(cmd, objArray[i], "@");
			}
		}

		public Sql Append(Sql sql)
		{
			if (this._rhs == null)
			{
				this._rhs = sql;
			}
			else
			{
				this._rhs.Append(sql);
			}
			return this;
		}

		public Sql Append(string sql, params object[] args)
		{
			return this.Append(new Sql(sql, args));
		}

		private void Build()
		{
			if (this._sqlFinal != null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			List<object> objs = new List<object>();
			this.Build(stringBuilder, objs, null);
			string str = stringBuilder.ToString();
			if (Sql.Filter.IsMatch(str))
			{
				throw new Exception("Commands LOAD DATA, LOAD_FILE, OUTFILE, DUMPFILE not allowed.");
			}
			this._sqlFinal = str;
			this._argsFinal = objs.ToArray();
		}

		private void Build(StringBuilder sb, List<object> args, Sql lhs)
		{
			if (!string.IsNullOrEmpty(this._sql))
			{
				if (sb.Length > 0)
				{
					sb.Append("\n");
				}
				string str = Sql.ProcessParams(this._sql, this._args, args);
				if (Sql.Is(lhs, "WHERE ") && Sql.Is(this, "WHERE "))
				{
					str = string.Concat("AND ", str.Substring(6));
				}
				if (Sql.Is(lhs, "ORDER BY ") && Sql.Is(this, "ORDER BY "))
				{
					str = string.Concat(", ", str.Substring(9));
				}
				sb.Append(str);
			}
			Sql sql = this._rhs;
			if (sql == null)
			{
				return;
			}
			sql.Build(sb, args, this);
		}

		public Sql From(params object[] tables)
		{
			return this.Append(new Sql(string.Concat("FROM ", string.Join(", ", (
				from x in (IEnumerable<object>)tables
				select x.ToString()).ToArray<string>())), Array.Empty<object>()));
		}

		public Sql GroupBy(params object[] columns)
		{
			return this.Append(new Sql(string.Concat("GROUP BY ", string.Join(", ", (
				from x in (IEnumerable<object>)columns
				select x.ToString()).ToArray<string>())), Array.Empty<object>()));
		}

		public Sql.SqlJoinClause InnerJoin(string table)
		{
			return this.Join("INNER JOIN ", table);
		}

		private static bool Is(Sql sql, string sqltype)
		{
			if (sql == null || sql._sql == null)
			{
				return false;
			}
			return sql._sql.StartsWith(sqltype, StringComparison.InvariantCultureIgnoreCase);
		}

		private Sql.SqlJoinClause Join(string joinType, string table)
		{
			return new Sql.SqlJoinClause(this.Append(new Sql(string.Concat(joinType, table), Array.Empty<object>())));
		}

		public Sql.SqlJoinClause LeftJoin(string table)
		{
			return this.Join("LEFT JOIN ", table);
		}

		public Sql OrderBy(params object[] columns)
		{
			return this.Append(new Sql(string.Concat("ORDER BY ", string.Join(", ", (
				from x in (IEnumerable<object>)columns
				select x.ToString()).ToArray<string>())), Array.Empty<object>()));
		}

		public static string ProcessParams(string sql, object[] argsSrc, List<object> argsDest)
		{
			return Sql.RxParams.Replace(sql, (Match m) => {
				object value;
				int num;
				int count;
				string str;
				string str1 = m.Value.Substring(1);
				if (!int.TryParse(str1, out num))
				{
					bool flag = false;
					value = null;
					object[] objArray = argsSrc;
					count = 0;
					while (count < (int)objArray.Length)
					{
						object obj = objArray[count];
						PropertyInfo property = obj.GetType().GetProperty(str1);
						if (property == null)
						{
							count++;
						}
						else
						{
							value = property.GetValue(obj, null);
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new ArgumentException(string.Concat(new string[] { "Parameter '@", str1, "' specified but none of the passed arguments have a property with this name (in '", sql, "')" }));
					}
				}
				else
				{
					if (num < 0 || num >= (int)argsSrc.Length)
					{
						throw new ArgumentOutOfRangeException(string.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", num, (int)argsSrc.Length, sql));
					}
					value = argsSrc[num];
				}
				if (!(value is IEnumerable) || value is string || value is byte[])
				{
					argsDest.Add(value);
					count = argsDest.Count - 1;
					return string.Concat("@", count.ToString());
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object obj1 in value as IEnumerable)
				{
					StringBuilder stringBuilder1 = stringBuilder;
					str = (stringBuilder.Length == 0 ? "@" : ",@");
					count = argsDest.Count;
					stringBuilder1.Append(string.Concat(str, count.ToString()));
					argsDest.Add(obj1);
				}
				return stringBuilder.ToString();
			});
		}

		public Sql Select(params object[] columns)
		{
			return this.Append(new Sql(string.Concat("SELECT ", string.Join(", ", (
				from x in (IEnumerable<object>)columns
				select x.ToString()).ToArray<string>())), Array.Empty<object>()));
		}

		public Sql Where(string sql, params object[] args)
		{
			return this.Append(new Sql(string.Concat("WHERE (", sql, ")"), args));
		}

		public class SqlJoinClause
		{
			private readonly Sql _sql;

			public SqlJoinClause(Sql sql)
			{
				this._sql = sql;
			}

			public Sql On(string onClause, params object[] args)
			{
				return this._sql.Append(string.Concat("ON ", onClause), args);
			}
		}
	}
}