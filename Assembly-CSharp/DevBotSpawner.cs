using System;
using System.Collections.Generic;
using UnityEngine;

public class DevBotSpawner : FacepunchBehaviour
{
	public GameObjectRef bot;

	public Transform waypointParent;

	public bool autoSelectLatestSpawnedGameObject = true;

	public float spawnRate = 1f;

	public int maxPopulation = 1;

	private Transform[] waypoints;

	private List<BaseEntity> _spawned = new List<BaseEntity>();

	public DevBotSpawner()
	{
	}

	public bool HasFreePopulation()
	{
		for (int i = this._spawned.Count - 1; i >= 0; i--)
		{
			BaseEntity item = this._spawned[i];
			if (item == null || item.Health() <= 0f)
			{
				this._spawned.Remove(item);
			}
		}
		if (this._spawned.Count < this.maxPopulation)
		{
			return true;
		}
		return false;
	}

	public void SpawnBot()
	{
		while (this.HasFreePopulation())
		{
			Vector3 vector3 = this.waypoints[0].position;
			GameManager gameManager = GameManager.server;
			string str = this.bot.resourcePath;
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
			if (baseEntity == null)
			{
				return;
			}
			this._spawned.Add(baseEntity);
			baseEntity.SendMessage("SetWaypoints", this.waypoints, SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
		}
	}

	public void Start()
	{
		this.waypoints = this.waypointParent.GetComponentsInChildren<Transform>();
		base.InvokeRepeating(new Action(this.SpawnBot), 5f, this.spawnRate);
	}
}