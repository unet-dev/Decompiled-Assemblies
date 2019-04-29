using System;
using UnityEngine;

public class MovementSoundTrigger : TriggerBase, IClientComponentEx, ILOD
{
	public SoundDefinition softSound;

	public SoundDefinition medSound;

	public SoundDefinition hardSound;

	public Collider collider;

	public MovementSoundTrigger()
	{
	}

	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this.collider);
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}
}