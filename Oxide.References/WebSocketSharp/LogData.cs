using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace WebSocketSharp
{
	public class LogData
	{
		private StackFrame _caller;

		private DateTime _date;

		private WebSocketSharp.LogLevel _level;

		private string _message;

		public StackFrame Caller
		{
			get
			{
				return this._caller;
			}
		}

		public DateTime Date
		{
			get
			{
				return this._date;
			}
		}

		public WebSocketSharp.LogLevel Level
		{
			get
			{
				return this._level;
			}
		}

		public string Message
		{
			get
			{
				return this._message;
			}
		}

		internal LogData(WebSocketSharp.LogLevel level, StackFrame caller, string message)
		{
			this._level = level;
			this._caller = caller;
			this._message = message ?? string.Empty;
			this._date = DateTime.Now;
		}

		public override string ToString()
		{
			string str;
			string str1 = string.Format("{0}|{1,-5}|", this._date, this._level);
			MethodBase method = this._caller.GetMethod();
			Type declaringType = method.DeclaringType;
			string str2 = string.Format("{0}{1}.{2}|", str1, declaringType.Name, method.Name);
			string[] strArrays = this._message.Replace("\r\n", "\n").TrimEnd(new char[] { '\n' }).Split(new char[] { '\n' });
			if ((int)strArrays.Length > 1)
			{
				StringBuilder stringBuilder = new StringBuilder(string.Format("{0}{1}\n", str2, strArrays[0]), 64);
				string str3 = string.Format("{{0,{0}}}{{1}}\n", str1.Length);
				for (int i = 1; i < (int)strArrays.Length; i++)
				{
					stringBuilder.AppendFormat(str3, "", strArrays[i]);
				}
				StringBuilder length = stringBuilder;
				length.Length = length.Length - 1;
				str = stringBuilder.ToString();
			}
			else
			{
				str = string.Format("{0}{1}", str2, this._message);
			}
			return str;
		}
	}
}