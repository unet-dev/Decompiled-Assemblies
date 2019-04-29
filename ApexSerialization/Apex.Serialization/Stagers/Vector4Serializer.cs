using Apex.Serialization;
using System;
using UnityEngine;

namespace Apex.Serialization.Stagers
{
	public sealed class Vector4Serializer : IStager
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(Vector4) };
			}
		}

		public Vector4Serializer()
		{
		}

		public StageItem StageValue(string name, object value)
		{
			Vector4 vector4 = (Vector4)value;
			return new StageElement(name, new StageItem[] { SerializationMaster.ToStageAttribute("x", vector4.x), SerializationMaster.ToStageAttribute("y", vector4.y), SerializationMaster.ToStageAttribute("z", vector4.z), SerializationMaster.ToStageAttribute("w", vector4.w) });
		}

		public object UnstageValue(StageItem item, Type targetType)
		{
			StageElement stageElement = (StageElement)item;
			return new Vector4(stageElement.AttributeValue<float>("x"), stageElement.AttributeValue<float>("y"), stageElement.AttributeValue<float>("z"), stageElement.AttributeValue<float>("w"));
		}
	}
}