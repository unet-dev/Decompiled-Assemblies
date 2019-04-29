using Apex.Serialization;
using System;
using System.Globalization;

namespace Apex.Serialization.Converters
{
	public sealed class FloatConverter : IValueConverter
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(float) };
			}
		}

		public FloatConverter()
		{
		}

		public object FromString(string value, Type targetType)
		{
			return Convert.ToSingle(value, CultureInfo.InvariantCulture);
		}

		public string ToString(object value)
		{
			return ((float)value).ToString("G8", CultureInfo.InvariantCulture);
		}
	}
}