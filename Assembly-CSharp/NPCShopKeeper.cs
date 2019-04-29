using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCShopKeeper : NPCPlayer
{
	private float greetDir;

	private Vector3 initialFacingDir;

	private BasePlayer lastWavedAtPlayer;

	public NPCShopKeeper()
	{
	}

	public void DelayedSleepEnd()
	{
		this.EndSleeping();
	}

	public void Greeting()
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(base.transform.position, 10f, list, 131072, QueryTriggerInteraction.Collide);
		Vector3 vector3 = base.transform.position;
		BasePlayer basePlayer = null;
		foreach (BasePlayer basePlayer1 in list)
		{
			if (basePlayer1.isClient || basePlayer1.IsNpc || basePlayer1 == this || !basePlayer1.IsVisible(this.eyes.position, Single.PositiveInfinity) || basePlayer1 == this.lastWavedAtPlayer || Vector3.Dot(Vector3Ex.Direction2D(basePlayer1.eyes.position, this.eyes.position), this.initialFacingDir) < 0.2f)
			{
				continue;
			}
			basePlayer = basePlayer1;
			if (basePlayer == null && !list.Contains(this.lastWavedAtPlayer))
			{
				this.lastWavedAtPlayer = null;
			}
			if (basePlayer == null)
			{
				this.SetAimDirection(this.initialFacingDir);
			}
			else
			{
				base.SignalBroadcast(BaseEntity.Signal.Gesture, "wave", null);
				this.SetAimDirection(Vector3Ex.Direction2D(basePlayer.eyes.position, this.eyes.position));
				this.lastWavedAtPlayer = basePlayer;
			}
			Pool.FreeList<BasePlayer>(ref list);
			return;
		}
		if (basePlayer == null && !list.Contains(this.lastWavedAtPlayer))
		{
			this.lastWavedAtPlayer = null;
		}
		if (basePlayer == null)
		{
			this.SetAimDirection(this.initialFacingDir);
		}
		else
		{
			base.SignalBroadcast(BaseEntity.Signal.Gesture, "wave", null);
			this.SetAimDirection(Vector3Ex.Direction2D(basePlayer.eyes.position, this.eyes.position));
			this.lastWavedAtPlayer = basePlayer;
		}
		Pool.FreeList<BasePlayer>(ref list);
	}

	public override void Hurt(HitInfo info)
	{
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawCube(base.transform.position + (Vector3.up * 1f), new Vector3(0.5f, 1f, 0.5f));
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.initialFacingDir = base.transform.rotation * Vector3.forward;
		base.Invoke(new Action(this.DelayedSleepEnd), 3f);
		this.SetAimDirection(base.transform.rotation * Vector3.forward);
		base.InvokeRandomized(new Action(this.Greeting), UnityEngine.Random.Range(5f, 10f), 5f, UnityEngine.Random.Range(0f, 2f));
	}

	public override void UpdateProtectionFromClothing()
	{
	}
}