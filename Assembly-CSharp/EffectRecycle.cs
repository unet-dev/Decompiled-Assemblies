using System;
using UnityEngine.Serialization;

public class EffectRecycle : BaseMonoBehaviour, IClientComponent, IRagdollInhert, IEffectRecycle
{
	[FormerlySerializedAs("lifeTime")]
	[ReadOnly]
	public float detachTime;

	[FormerlySerializedAs("lifeTime")]
	[ReadOnly]
	public float recycleTime;

	public EffectRecycle.PlayMode playMode;

	public EffectRecycle.ParentDestroyBehaviour onParentDestroyed;

	public EffectRecycle()
	{
	}

	public enum ParentDestroyBehaviour
	{
		Detach,
		Destroy,
		DetachWaitDestroy
	}

	public enum PlayMode
	{
		Once,
		Looped
	}
}