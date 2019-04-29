using Apex.Serialization;
using System;
using UnityEngine;

namespace Apex.Serialization.Stagers
{
	public sealed class RectSerializer : IStager
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(Rect) };
			}
		}

		public RectSerializer()
		{
		}

		public StageItem StageValue(string name, object value)
		{
			Rect rect = (Rect)value;
			return new StageElement(name, new StageItem[] { SerializationMaster.ToStageAttribute("left", rect.xMin), SerializationMaster.ToStageAttribute("top", rect.yMin), SerializationMaster.ToStageAttribute("width", rect.width), SerializationMaster.ToStageAttribute("height", rect.height) });
		}

		public object UnstageValue(StageItem item, Type targetType)
		{
			StageElement stageElement = (StageElement)item;
			return new Rect(stageElement.AttributeValue<float>("left"), stageElement.AttributeValue<float>("top"), stageElement.AttributeValue<float>("width"), stageElement.AttributeValue<float>("height"));
		}
	}
}