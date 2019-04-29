using Apex.Serialization;
using System;
using System.Text;

namespace Apex.Serialization.Json
{
	internal class JsonPrettyWriter : IJsonWriter
	{
		private StringBuilder _b = new StringBuilder();

		private int _depth;

		private bool _indent;

		public JsonPrettyWriter()
		{
		}

		private void Indent()
		{
			if (this._indent)
			{
				this._b.Append(' ', this._depth * 2);
				this._indent = false;
			}
		}

		private void NextLine(int depthChange)
		{
			this._indent = true;
			this._depth += depthChange;
			this._b.AppendLine();
		}

		public override string ToString()
		{
			return this._b.ToString();
		}

		public void WriteAttributeLabel(StageAttribute a)
		{
			this.Indent();
			this._b.Append('\"');
			this._b.Append('@');
			StringHandler.EscapeString(a.name, this._b);
			this._b.Append('\"');
			this._b.Append(" : ");
		}

		public void WriteElementEnd()
		{
			this.NextLine(-1);
			this.Indent();
			this._b.Append('}');
		}

		public void WriteElementStart()
		{
			this.Indent();
			this._b.Append('{');
			this.NextLine(1);
		}

		public void WriteLabel(StageItem l)
		{
			this.Indent();
			this._b.Append('\"');
			StringHandler.EscapeString(l.name, this._b);
			this._b.Append('\"');
			this._b.Append(" : ");
		}

		public void WriteListEnd()
		{
			this.NextLine(-1);
			this.Indent();
			this._b.Append(']');
		}

		public void WriteListStart()
		{
			this._b.Append('[');
			this.NextLine(1);
		}

		public void WriteNull(StageNull n)
		{
			this.Indent();
			this._b.Append("null");
		}

		public void WriteSeparator()
		{
			this._b.Append(',');
			this.NextLine(0);
		}

		public void WriteValue(StageValue v)
		{
			this.Indent();
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