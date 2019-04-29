using System;
using System.Collections.Specialized;
using System.Text;

namespace WebSocketSharp.Net
{
	internal sealed class QueryStringCollection : NameValueCollection
	{
		public QueryStringCollection()
		{
		}

		public override string ToString()
		{
			string str;
			if (this.Count != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string[] allKeys = this.AllKeys;
				for (int i = 0; i < (int)allKeys.Length; i++)
				{
					string str1 = allKeys[i];
					stringBuilder.AppendFormat("{0}={1}&", str1, base[str1]);
				}
				if (stringBuilder.Length > 0)
				{
					StringBuilder length = stringBuilder;
					length.Length = length.Length - 1;
				}
				str = stringBuilder.ToString();
			}
			else
			{
				str = string.Empty;
			}
			return str;
		}
	}
}