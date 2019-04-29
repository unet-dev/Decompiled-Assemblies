using Apex.Serialization;
using System;

namespace Apex.Serialization.Converters
{
	public sealed class StringConverter : IValueConverter
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(string) };
			}
		}

		public StringConverter()
		{
		}

		public object FromString(string value, Type targetType)
		{
			return value;
		}

		public string ToString(object value)
		{
			return (string)value;
		}
	}
}