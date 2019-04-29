using Apex.Serialization;
using System;

namespace Apex.Serialization.Converters
{
	public sealed class GuidConverter : IValueConverter
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(Guid) };
			}
		}

		public GuidConverter()
		{
		}

		public object FromString(string value, Type targetType)
		{
			return new Guid(value);
		}

		public string ToString(object value)
		{
			return ((Guid)value).ToString("N");
		}
	}
}