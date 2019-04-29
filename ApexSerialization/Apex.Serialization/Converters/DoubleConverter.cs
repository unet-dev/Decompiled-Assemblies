using Apex.Serialization;
using System;
using System.Globalization;

namespace Apex.Serialization.Converters
{
	public sealed class DoubleConverter : IValueConverter
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(double) };
			}
		}

		public DoubleConverter()
		{
		}

		public object FromString(string value, Type targetType)
		{
			return Convert.ToDouble(value, CultureInfo.InvariantCulture);
		}

		public string ToString(object value)
		{
			return ((double)value).ToString("R", CultureInfo.InvariantCulture);
		}
	}
}