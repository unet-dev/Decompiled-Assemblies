using System;
using UnityEngine;

public class UnparentOnDestroy : MonoBehaviour, IOnParentDestroying
{
	public float destroyAfterSeconds = 1f;

	public UnparentOnDestroy()
	{
	}

	public void OnParentDestroying()
	{
		base.transform.parent = null;
		GameManager.Destroy(base.gameObject, (this.destroyAfterSeconds <= 0f ? 1f : this.destroyAfterSeconds));
	}

	protected void OnValidate()
	{
		if (this.destroyAfterSeconds <= 0f)
		{
			this.destroyAfterSeconds = 1f;
		}
	}
}