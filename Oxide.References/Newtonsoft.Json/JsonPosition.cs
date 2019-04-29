using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Newtonsoft.Json
{
	[Preserve]
	internal struct JsonPosition
	{
		private readonly static char[] SpecialCharacters;

		internal JsonContainerType Type;

		internal int Position;

		internal string PropertyName;

		internal bool HasIndex;

		static JsonPosition()
		{
			JsonPosition.SpecialCharacters = new char[] { '.', ' ', '[', ']', '(', ')' };
		}

		public JsonPosition(JsonContainerType type)
		{
			this.Type = type;
			this.HasIndex = JsonPosition.TypeHasIndex(type);
			this.Position = -1;
			this.PropertyName = null;
		}

		internal static string BuildPath(List<JsonPosition> positions, JsonPosition? currentPosition)
		{
			JsonPosition item;
			int num = 0;
			if (positions != null)
			{
				for (int i = 0; i < positions.Count; i++)
				{
					item = positions[i];
					num += item.CalculateLength();
				}
			}
			if (currentPosition.HasValue)
			{
				item = currentPosition.GetValueOrDefault();
				num += item.CalculateLength();
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			if (positions != null)
			{
				foreach (JsonPosition position in positions)
				{
					position.WriteTo(stringBuilder);
				}
			}
			if (currentPosition.HasValue)
			{
				currentPosition.GetValueOrDefault().WriteTo(stringBuilder);
			}
			return stringBuilder.ToString();
		}

		internal int CalculateLength()
		{
			switch (this.Type)
			{
				case JsonContainerType.Object:
				{
					return this.PropertyName.Length + 5;
				}
				case JsonContainerType.Array:
				case JsonContainerType.Constructor:
				{
					return MathUtils.IntLength((ulong)this.Position) + 2;
				}
			}
			throw new ArgumentOutOfRangeException("Type");
		}

		internal static string FormatMessage(IJsonLineInfo lineInfo, string path, string message)
		{
			if (!message.EndsWith(Environment.NewLine, StringComparison.Ordinal))
			{
				message = message.Trim();
				if (!message.EndsWith('.'))
				{
					message = string.Concat(message, ".");
				}
				message = string.Concat(message, " ");
			}
			message = string.Concat(message, "Path '{0}'".FormatWith(CultureInfo.InvariantCulture, path));
			if (lineInfo != null && lineInfo.HasLineInfo())
			{
				message = string.Concat(message, ", line {0}, position {1}".FormatWith(CultureInfo.InvariantCulture, lineInfo.LineNumber, lineInfo.LinePosition));
			}
			message = string.Concat(message, ".");
			return message;
		}

		internal static bool TypeHasIndex(JsonContainerType type)
		{
			if (type == JsonContainerType.Array)
			{
				return true;
			}
			return type == JsonContainerType.Constructor;
		}

		internal void WriteTo(StringBuilder sb)
		{
			switch (this.Type)
			{
				case JsonContainerType.Object:
				{
					string propertyName = this.PropertyName;
					if (propertyName.IndexOfAny(JsonPosition.SpecialCharacters) != -1)
					{
						sb.Append("['");
						sb.Append(propertyName);
						sb.Append("']");
						return;
					}
					if (sb.Length > 0)
					{
						sb.Append('.');
					}
					sb.Append(propertyName);
					return;
				}
				case JsonContainerType.Array:
				case JsonContainerType.Constructor:
				{
					sb.Append('[');
					sb.Append(this.Position);
					sb.Append(']');
					return;
				}
				default:
				{
					return;
				}
			}
		}
	}
}