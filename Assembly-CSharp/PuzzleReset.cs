using ConVar;
using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleReset : FacepunchBehaviour
{
	public SpawnGroup[] respawnGroups;

	public IOEntity[] resetEnts;

	public GameObject[] resetObjects;

	public bool playersBlockReset;

	public float playerDetectionRadius;

	public Transform playerDetectionOrigin;

	public float timeBetweenResets = 30f;

	public bool scaleWithServerPopulation;

	[HideInInspector]
	public Vector3[] resetPositions;

	private float resetTimeElapsed;

	private float resetTickTime = 10f;

	public PuzzleReset()
	{
	}

	public void DoReset()
	{
		int i;
		IOEntity component = base.GetComponent<IOEntity>();
		if (component != null)
		{
			PuzzleReset.ResetIOEntRecursive(component, UnityEngine.Time.frameCount);
			component.MarkDirty();
		}
		else if (this.resetPositions != null)
		{
			Vector3[] vector3Array = this.resetPositions;
			for (i = 0; i < (int)vector3Array.Length; i++)
			{
				Vector3 vector3 = vector3Array[i];
				Vector3 vector31 = base.transform.TransformPoint(vector3);
				List<IOEntity> list = Facepunch.Pool.GetList<IOEntity>();
				Vis.Entities<IOEntity>(vector31, 0.5f, list, 1235288065, QueryTriggerInteraction.Ignore);
				foreach (IOEntity oEntity in list)
				{
					if (!oEntity.IsRootEntity() || !oEntity.isServer)
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
		GameObject[] gameObjectArray = this.resetObjects;
		for (i = 0; i < (int)gameObjectArray.Length; i++)
		{
			GameObject gameObject = gameObjectArray[i];
			if (gameObject != null)
			{
				gameObject.SendMessage("OnPuzzleReset", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public float GetResetSpacing()
	{
		return this.timeBetweenResets * (this.scaleWithServerPopulation ? 1f - SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate) : 1f);
	}

	public bool PassesResetCheck()
	{
		bool flag;
		if (this.playersBlockReset)
		{
			List<BasePlayer>.Enumerator enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if (current.IsSleeping() || !current.IsAlive() || Vector3.Distance(current.transform.position, this.playerDetectionOrigin.position) >= this.playerDetectionRadius)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}
		return true;
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

	public void ResetTick()
	{
		if (this.PassesResetCheck())
		{
			this.resetTimeElapsed += this.resetTickTime;
		}
		if (this.resetTimeElapsed > this.GetResetSpacing())
		{
			this.resetTimeElapsed = 0f;
			this.DoReset();
		}
	}

	public void ResetTimer()
	{
		this.resetTimeElapsed = 0f;
		base.CancelInvoke(new Action(this.ResetTick));
		base.InvokeRandomized(new Action(this.ResetTick), UnityEngine.Random.Range(0f, 1f), this.resetTickTime, 0.5f);
	}

	public void Start()
	{
		if (this.timeBetweenResets != Single.PositiveInfinity)
		{
			this.ResetTimer();
		}
	}
}