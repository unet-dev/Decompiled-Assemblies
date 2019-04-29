using ConVar;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BradleySpawner : MonoBehaviour, IServerComponent
{
	public BasePath path;

	public GameObjectRef bradleyPrefab;

	[NonSerialized]
	public BradleyAPC spawned;

	public bool initialSpawn;

	public float minRespawnTimeMinutes = 5f;

	public float maxRespawnTimeMinutes = 5f;

	public static BradleySpawner singleton;

	private bool pendingRespawn;

	public BradleySpawner()
	{
	}

	public void CheckIfRespawnNeeded()
	{
		if (!this.pendingRespawn && (this.spawned == null || !this.spawned.IsAlive()))
		{
			this.ScheduleRespawn();
		}
	}

	public void DelayedStart()
	{
		if (this.initialSpawn)
		{
			this.DoRespawn();
		}
		base.InvokeRepeating("CheckIfRespawnNeeded", 0f, 5f);
	}

	public void DoRespawn()
	{
		if (!Rust.Application.isLoading && !Rust.Application.isLoadingSave)
		{
			this.SpawnBradley();
		}
		this.pendingRespawn = false;
	}

	public void ScheduleRespawn()
	{
		base.CancelInvoke("DoRespawn");
		base.Invoke("DoRespawn", UnityEngine.Random.Range(Bradley.respawnDelayMinutes - Bradley.respawnDelayVariance, Bradley.respawnDelayMinutes + Bradley.respawnDelayVariance) * 60f);
		this.pendingRespawn = true;
	}

	public void SpawnBradley()
	{
		if (this.spawned != null)
		{
			Debug.LogWarning("Bradley attempting to spawn but one already exists!");
			return;
		}
		if (!Bradley.enabled)
		{
			return;
		}
		Vector3 item = this.path.interestZones[UnityEngine.Random.Range(0, this.path.interestZones.Count)].transform.position;
		GameManager gameManager = GameManager.server;
		string str = this.bradleyPrefab.resourcePath;
		Quaternion quaternion = new Quaternion();
		BaseEntity baseEntity = gameManager.CreateEntity(str, item, quaternion, true);
		BradleyAPC component = baseEntity.GetComponent<BradleyAPC>();
		if (!component)
		{
			baseEntity.Kill(BaseNetworkable.DestroyMode.None);
		}
		else
		{
			baseEntity.Spawn();
			component.InstallPatrolPath(this.path);
		}
		Debug.Log(string.Concat("BradleyAPC Spawned at :", item));
		this.spawned = component;
	}

	public void Start()
	{
		BradleySpawner.singleton = this;
		base.Invoke("DelayedStart", 3f);
	}
}