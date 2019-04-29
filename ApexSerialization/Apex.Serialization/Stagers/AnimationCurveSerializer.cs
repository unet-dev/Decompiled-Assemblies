using Apex.Serialization;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.Serialization.Stagers
{
	public sealed class AnimationCurveSerializer : IStager
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(AnimationCurve) };
			}
		}

		public AnimationCurveSerializer()
		{
		}

		public StageItem StageValue(string name, object value)
		{
			Keyframe[] keyframeArray = ((AnimationCurve)value).keys;
			StageElement stageElement = new StageElement(name);
			for (int i = 0; i < (int)keyframeArray.Length; i++)
			{
				Keyframe keyframe = keyframeArray[i];
				stageElement.Add(new StageElement("key", new StageItem[] { SerializationMaster.ToStageAttribute("time", keyframe.time), SerializationMaster.ToStageAttribute("value", keyframe.@value), SerializationMaster.ToStageAttribute("inTangent", keyframe.inTangent), SerializationMaster.ToStageAttribute("outTangent", keyframe.outTangent), SerializationMaster.ToStageAttribute("tangentMode", keyframe.tangentMode) }));
			}
			return stageElement;
		}

		public object UnstageValue(StageItem item, Type targetType)
		{
			return new AnimationCurve((
				from key in ((StageElement)item).Elements()
				select new Keyframe()
				{
					time = key.AttributeValue<float>("time"),
					@value = key.AttributeValue<float>("value"),
					inTangent = key.AttributeValue<float>("inTangent"),
					outTangent = key.AttributeValue<float>("outTangent"),
					tangentMode = key.AttributeValue<int>("tangentMode")
				}).ToArray<Keyframe>());
		}
	}
}