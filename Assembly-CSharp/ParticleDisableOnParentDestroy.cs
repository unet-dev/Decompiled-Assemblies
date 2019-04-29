using System;
using UnityEngine;

public class ParticleDisableOnParentDestroy : MonoBehaviour, IOnParentDestroying
{
	public float destroyAfterSeconds;

	public ParticleDisableOnParentDestroy()
	{
	}

	public void OnParentDestroying()
	{
		base.transform.parent = null;
		base.GetComponent<ParticleSystem>().enableEmission = false;
		if (this.destroyAfterSeconds > 0f)
		{
			GameManager.Destroy(base.gameObject, this.destroyAfterSeconds);
		}
	}
}