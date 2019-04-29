using Apex.Serialization;
using System;
using UnityEngine;

namespace Apex.Serialization.Stagers
{
	public sealed class ColorSerializer : IStager
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(Color) };
			}
		}

		public ColorSerializer()
		{
		}

		public StageItem StageValue(string name, object value)
		{
			Color color = (Color)value;
			return new StageElement(name, new StageItem[] { SerializationMaster.ToStageAttribute("r", color.r), SerializationMaster.ToStageAttribute("g", color.g), SerializationMaster.ToStageAttribute("b", color.b), SerializationMaster.ToStageAttribute("a", color.a) });
		}

		public object UnstageValue(StageItem item, Type targetType)
		{
			StageElement stageElement = (StageElement)item;
			return new Color(stageElement.AttributeValue<float>("r"), stageElement.AttributeValue<float>("g"), stageElement.AttributeValue<float>("b"), stageElement.AttributeValue<float>("a"));
		}
	}
}