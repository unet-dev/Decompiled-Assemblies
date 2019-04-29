using Apex.Serialization;
using System;
using UnityEngine;

namespace Apex.Serialization.Stagers
{
	public sealed class Vector2Serializer : IStager
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(Vector2) };
			}
		}

		public Vector2Serializer()
		{
		}

		public StageItem StageValue(string name, object value)
		{
			Vector2 vector2 = (Vector2)value;
			return new StageElement(name, new StageItem[] { SerializationMaster.ToStageAttribute("x", vector2.x), SerializationMaster.ToStageAttribute("y", vector2.y) });
		}

		public object UnstageValue(StageItem item, Type targetType)
		{
			StageElement stageElement = (StageElement)item;
			return new Vector2(stageElement.AttributeValue<float>("x"), stageElement.AttributeValue<float>("y"));
		}
	}
}