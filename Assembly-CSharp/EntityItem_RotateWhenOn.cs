using System;
using UnityEngine;

public class EntityItem_RotateWhenOn : EntityComponent<BaseEntity>
{
	public EntityItem_RotateWhenOn.State on;

	public EntityItem_RotateWhenOn.State off;

	internal bool currentlyOn;

	internal bool stateInitialized;

	public BaseEntity.Flags targetFlag = BaseEntity.Flags.On;

	public EntityItem_RotateWhenOn()
	{
	}

	[Serializable]
	public class State
	{
		public Vector3 rotation;

		public float initialDelay;

		public float timeToTake;

		public AnimationCurve animationCurve;

		public string effectOnStart;

		public string effectOnFinish;

		public SoundDefinition movementLoop;

		public float movementLoopFadeOutTime;

		public SoundDefinition startSound;

		public SoundDefinition stopSound;

		public State()
		{
		}
	}
}