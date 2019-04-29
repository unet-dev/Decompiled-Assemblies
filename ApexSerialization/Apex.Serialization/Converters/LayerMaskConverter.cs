using Apex.Serialization;
using System;
using System.Globalization;
using UnityEngine;

namespace Apex.Serialization.Converters
{
	public sealed class LayerMaskConverter : IValueConverter
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(LayerMask) };
			}
		}

		public LayerMaskConverter()
		{
		}

		public object FromString(string value, Type targetType)
		{
			if (string.IsNullOrEmpty(value))
			{
				return 0;
			}
			return int.Parse(value, CultureInfo.InvariantCulture);
		}

		public string ToString(object value)
		{
			return ((LayerMask)value).@value.ToString(CultureInfo.InvariantCulture);
		}
	}
}