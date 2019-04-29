using Apex.Serialization;
using System;
using UnityEngine;

namespace Apex.Serialization.Stagers
{
	public sealed class BoundsStager : IStager
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(Bounds) };
			}
		}

		public BoundsStager()
		{
		}

		public StageItem StageValue(string name, object value)
		{
			Bounds bound = (Bounds)value;
			return new StageElement(name, new StageItem[] { SerializationMaster.ToStageAttribute("center.x", bound.center.x), SerializationMaster.ToStageAttribute("center.y", bound.center.y), SerializationMaster.ToStageAttribute("center.z", bound.center.z), SerializationMaster.ToStageAttribute("size.x", bound.size.x), SerializationMaster.ToStageAttribute("size.y", bound.size.y), SerializationMaster.ToStageAttribute("size.z", bound.size.z) });
		}

		public object UnstageValue(StageItem item, Type targetType)
		{
			StageElement stageElement = (StageElement)item;
			return new Bounds(new Vector3(SerializationMaster.FromString<float>(stageElement.Attribute("center.x").@value), SerializationMaster.FromString<float>(stageElement.Attribute("center.y").@value), SerializationMaster.FromString<float>(stageElement.Attribute("center.z").@value)), new Vector3(SerializationMaster.FromString<float>(stageElement.Attribute("size.x").@value), SerializationMaster.FromString<float>(stageElement.Attribute("size.y").@value), SerializationMaster.FromString<float>(stageElement.Attribute("size.z").@value)));
		}
	}
}