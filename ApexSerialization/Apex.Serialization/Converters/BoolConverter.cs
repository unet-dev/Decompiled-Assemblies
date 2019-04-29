using Apex.Serialization;
using System;

namespace Apex.Serialization.Converters
{
	public sealed class BoolConverter : IValueConverter
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(bool) };
			}
		}

		public BoolConverter()
		{
		}

		public object FromString(string value, Type targetType)
		{
			return (string.IsNullOrEmpty(value) ? false : value[0] == 't');
		}

		public string ToString(object value)
		{
			if (!(bool)value)
			{
				return "false";
			}
			return "true";
		}
	}
}