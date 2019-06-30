using System;
using UnityEngine;

public class DestroyArcadeEntity : BaseMonoBehaviour
{
	public ArcadeEntity ent;

	public float TimeToDie = 1f;

	public float TimeToDieVariance;

	public DestroyArcadeEntity()
	{
	}

	private void DestroyAction()
	{
		if ((this.ent != null) & this.ent.host)
		{
			UnityEngine.Object.Destroy(this.ent.gameObject);
		}
	}

	private void Start()
	{
		base.Invoke(new Action(this.DestroyAction), this.TimeToDie + UnityEngine.Random.Range(this.TimeToDieVariance * -0.5f, this.TimeToDieVariance * 0.5f));
	}
}