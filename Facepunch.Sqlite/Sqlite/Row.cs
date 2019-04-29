using System;
using System.Collections.Generic;

namespace Facepunch.Sqlite
{
	public class Row : Dictionary<string, object>
	{
		public Row()
		{
		}

		public byte[] GetBlob(string v)
		{
			object obj;
			if (!base.TryGetValue(v, out obj) || obj == null)
			{
				return null;
			}
			return (byte[])obj;
		}

		public double GetDouble(string v)
		{
			object obj;
			if (!base.TryGetValue(v, out obj) || obj == null)
			{
				return 0;
			}
			return (double)obj;
		}

		public int GetInt(string v)
		{
			object obj;
			if (!base.TryGetValue(v, out obj) || obj == null)
			{
				return 0;
			}
			return (int)((long)obj);
		}

		public string GetString(string v)
		{
			object obj;
			if (!base.TryGetValue(v, out obj) || obj == null)
			{
				return null;
			}
			return (string)obj;
		}
	}
}