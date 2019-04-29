using System;

namespace Apex.Serialization
{
	public interface IValueConverter
	{
		Type[] handledTypes
		{
			get;
		}

		object FromString(string value, Type targetType);

		string ToString(object value);
	}
}