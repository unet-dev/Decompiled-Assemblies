using System;
using UnityEngine;

public class ReverbZoneTrigger : TriggerBase, IClientComponentEx, ILOD
{
	public Collider trigger;

	public AudioReverbZone reverbZone;

	public float lodDistance = 100f;

	public bool inRange;

	public ReverbSettings reverbSettings;

	public ReverbZoneTrigger()
	{
	}

	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this.trigger);
		p.RemoveComponent(this.reverbZone);
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}
}