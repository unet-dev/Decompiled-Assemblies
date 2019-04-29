using ConVar;
using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleReset : FacepunchBehaviour
{
	public SpawnGroup[] respawnGroups;

	public IOEntity[] resetEnts;

	public bool playersBlockReset;

	public float playerDetectionRadius;

	public Transform playerDetectionOrigin;

	public float timeBetweenResets = 30f;

	public bool scaleWithServerPopulation;

	[HideInInspector]
	public Vector3[] resetPositions;

	public PuzzleReset()
	{
	}

	public void DoReset()
	{
		IOEntity component = base.GetComponent<IOEntity>();
		if (component != null)
		{
			PuzzleReset.ResetIOEntRecursive(component, UnityEngine.Time.frameCount);
			component.MarkDirty();
		}
		else if (this.resetPositions != null)
		{
			Vector3[] vector3Array = this.resetPositions;
			for (int i = 0; i < (int)vector3Array.Length; i++)
			{
				Vector3 vector3 = vector3Array[i];
				Vector3 vector31 = base.transform.TransformPoint(vector3);
				List<IOEntity> list = Facepunch.Pool.GetList<IOEntity>();
				Vis.Entities<IOEntity>(vector31, 0.5f, list, 1235288065, QueryTriggerInteraction.Ignore);
				foreach (IOEntity oEntity in list)
				{
					if (!oEntity.IsRootEntity())
					{
						continue;
					}
					PuzzleReset.ResetIOEntRecursive(oEntity, UnityEngine.Time.frameCount);
					oEntity.MarkDirty();
				}
				Facepunch.Pool.FreeList<IOEntity>(ref list);
			}
		}
		List<SpawnGroup> spawnGroups = Facepunch.Pool.GetList<SpawnGroup>();
		Vis.Components<SpawnGroup>(base.transform.position, 1f, spawnGroups, 262144, QueryTriggerInteraction.Collide);
		foreach (SpawnGroup spawnGroup in spawnGroups)
		{
			if (spawnGroup == null)
			{
				continue;
			}
			spawnGroup.Spawn();
		}
		Facepunch.Pool.FreeList<SpawnGroup>(ref spawnGroups);
	}

	public float GetResetSpacing()
	{
		return this.timeBetweenResets * (this.scaleWithServerPopulation ? 1f - SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate) : 1f);
	}

	public bool PassesResetCheck()
	{
		if (!this.playersBlockReset)
		{
			return true;
		}
		List<BasePlayer> list = Facepunch.Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(this.playerDetectionOrigin.position, this.playerDetectionRadius, list, 131072, QueryTriggerInteraction.Collide);
		bool flag = true;
		foreach (BasePlayer basePlayer in list)
		{
			if (basePlayer.IsSleeping() || !basePlayer.IsAlive() || basePlayer.IsNpc)
			{
				continue;
			}
			flag = false;
			Facepunch.Pool.FreeList<BasePlayer>(ref list);
			return flag;
		}
		Facepunch.Pool.FreeList<BasePlayer>(ref list);
		return flag;
	}

	public static void ResetIOEntRecursive(IOEntity target, int resetIndex)
	{
		if (target.lastResetIndex == resetIndex)
		{
			return;
		}
		target.lastResetIndex = resetIndex;
		target.ResetIOState();
		IOEntity.IOSlot[] oSlotArray = target.outputs;
		for (int i = 0; i < (int)oSlotArray.Length; i++)
		{
			IOEntity.IOSlot oSlot = oSlotArray[i];
			if (oSlot.connectedTo.Get(true) != null && oSlot.connectedTo.Get(true) != target)
			{
				PuzzleReset.ResetIOEntRecursive(oSlot.connectedTo.Get(true), resetIndex);
			}
		}
	}

	public void ResetTimer()
	{
		base.CancelInvoke(new Action(this.TimedReset));
		base.Invoke(new Action(this.TimedReset), this.GetResetSpacing());
	}

	public void Start()
	{
		if (this.timeBetweenResets != Single.PositiveInfinity)
		{
			this.ResetTimer();
		}
	}

	public void TimedReset()
	{
		if (!this.PassesResetCheck())
		{
			base.Invoke(new Action(this.TimedReset), 30f);
			return;
		}
		this.DoReset();
		base.Invoke(new Action(this.TimedReset), this.GetResetSpacing());
	}
}