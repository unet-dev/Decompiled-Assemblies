using System;

namespace Apex.Serialization
{
	public interface IStager
	{
		Type[] handledTypes
		{
			get;
		}

		StageItem StageValue(string name, object value);

		object UnstageValue(StageItem item, Type targetType);
	}
}