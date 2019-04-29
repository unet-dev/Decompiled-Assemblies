using Apex.Serialization;
using System;

namespace Apex.Serialization.Converters
{
	public sealed class EnumConverter : IValueConverter
	{
		public readonly static EnumConverter instance;

		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(Enum) };
			}
		}

		static EnumConverter()
		{
			EnumConverter.instance = new EnumConverter();
		}

		public EnumConverter()
		{
		}

		public object FromString(string value, Type targetType)
		{
			return Enum.Parse(targetType, value);
		}

		public string ToString(object value)
		{
			return value.ToString();
		}
	}
}