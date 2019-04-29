using Apex.Serialization;
using System;
using UnityEngine;

namespace Apex.Serialization.Stagers
{
	public sealed class Vector3Serializer : IStager
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(Vector3) };
			}
		}

		public Vector3Serializer()
		{
		}

		public StageItem StageValue(string name, object value)
		{
			Vector3 vector3 = (Vector3)value;
			return new StageElement(name, new StageItem[] { SerializationMaster.ToStageAttribute("x", vector3.x), SerializationMaster.ToStageAttribute("y", vector3.y), SerializationMaster.ToStageAttribute("z", vector3.z) });
		}

		public object UnstageValue(StageItem item, Type targetType)
		{
			StageElement stageElement = (StageElement)item;
			return new Vector3(stageElement.AttributeValue<float>("x"), stageElement.AttributeValue<float>("y"), stageElement.AttributeValue<float>("z"));
		}
	}
}