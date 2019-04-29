using Apex.Serialization;
using System;
using System.Text;

namespace Apex.Serialization.Json
{
	internal class JsonCompactWriter : IJsonWriter
	{
		private StringBuilder _b = new StringBuilder();

		public JsonCompactWriter()
		{
		}

		public override string ToString()
		{
			return this._b.ToString();
		}

		public void WriteAttributeLabel(StageAttribute a)
		{
			this._b.Append('\"');
			this._b.Append('@');
			StringHandler.EscapeString(a.name, this._b);
			this._b.Append('\"');
			this._b.Append(':');
		}

		public void WriteElementEnd()
		{
			this._b.Append('}');
		}

		public void WriteElementStart()
		{
			this._b.Append('{');
		}

		public void WriteLabel(StageItem l)
		{
			this._b.Append('\"');
			StringHandler.EscapeString(l.name, this._b);
			this._b.Append('\"');
			this._b.Append(':');
		}

		public void WriteListEnd()
		{
			this._b.Append(']');
		}

		public void WriteListStart()
		{
			this._b.Append('[');
		}

		public void WriteNull(StageNull n)
		{
			this._b.Append("null");
		}

		public void WriteSeparator()
		{
			this._b.Append(',');
		}

		public void WriteValue(StageValue v)
		{
			if (!v.isText)
			{
				this._b.Append(v.@value);
				return;
			}
			this._b.Append('\"');
			StringHandler.EscapeString(v.@value, this._b);
			this._b.Append('\"');
		}
	}
}