using Apex.Serialization;
using System;
using System.Globalization;

namespace Apex.Serialization.Converters
{
	public sealed class CharConverter : IValueConverter
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(char) };
			}
		}

		public CharConverter()
		{
		}

		public object FromString(string value, Type targetType)
		{
			if (string.IsNullOrEmpty(value))
			{
				return '\0';
			}
			return Convert.ToChar(value, CultureInfo.InvariantCulture);
		}

		public string ToString(object value)
		{
			char chr = (char)value;
			string str = chr.ToString(CultureInfo.InvariantCulture).TrimStart(new char[1]);
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			return str;
		}
	}
}