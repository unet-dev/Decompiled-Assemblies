using Apex.Serialization;
using System;
using System.Globalization;

namespace Apex.Serialization.Converters
{
	public sealed class DateTimeConverter : IValueConverter
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(DateTime) };
			}
		}

		public DateTimeConverter()
		{
		}

		public object FromString(string value, Type targetType)
		{
			return DateTime.ParseExact(value, "yyyyMMddhhmmssFFFFFFFK", CultureInfo.InvariantCulture);
		}

		public string ToString(object value)
		{
			return ((DateTime)value).ToString("yyyyMMddhhmmssFFFFFFFK", CultureInfo.InvariantCulture);
		}
	}
}