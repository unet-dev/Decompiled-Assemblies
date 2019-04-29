using Apex.Serialization;
using System;
using System.Globalization;

namespace Apex.Serialization.Converters
{
	public sealed class PrimitivesConverter : IValueConverter
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(byte), typeof(short), typeof(int), typeof(long), typeof(ushort), typeof(uint), typeof(ulong) };
			}
		}

		public PrimitivesConverter()
		{
		}

		public object FromString(string value, Type targetType)
		{
			return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
		}

		public string ToString(object value)
		{
			return ((IConvertible)value).ToString(CultureInfo.InvariantCulture);
		}
	}
}