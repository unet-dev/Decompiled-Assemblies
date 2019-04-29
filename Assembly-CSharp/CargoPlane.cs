using Oxide.Core;
using System;
using UnityEngine;

public class CargoPlane : BaseEntity
{
	public GameObjectRef prefabDrop;

	public SpawnFilter filter;

	public Vector3 startPos;

	public Vector3 endPos;

	public float secondsToTake;

	public float secondsTaken;

	public bool dropped;

	public Vector3 dropPosition = Vector3.zero;

	public CargoPlane()
	{
	}

	public void InitDropPosition(Vector3 newDropPosition)
	{
		this.dropPosition = newDropPosition;
		this.dropPosition.y = 0f;
	}

	public Vector3 RandomDropPosition()
	{
		float single;
		Vector3 vector3 = Vector3.zero;
		float single1 = 100f;
		float size = TerrainMeta.Size.x;
		do
		{
			vector3 = Vector3Ex.Range(-(size / 3f), size / 3f);
			if (this.filter.GetFactor(vector3) != 0f)
			{
				break;
			}
			single = single1 - 1f;
			single1 = single;
		}
		while (single > 0f);
		vector3.y = 0f;
		return vector3;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (this.dropPosition == Vector3.zero)
		{
			this.dropPosition = this.RandomDropPosition();
		}
		this.UpdateDropPosition(this.dropPosition);
	}

	private void Update()
	{
		if (!base.isServer)
		{
			return;
		}
		this.secondsTaken += Time.deltaTime;
		float single = Mathf.InverseLerp(0f, this.secondsToTake, this.secondsTaken);
		if (!this.dropped && single >= 0.5f)
		{
			this.dropped = true;
			GameManager gameManager = GameManager.server;
			string str = this.prefabDrop.resourcePath;
			Vector3 vector3 = base.transform.position;
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
			if (baseEntity)
			{
				baseEntity.globalBroadcast = true;
				baseEntity.Spawn();
			}
		}
		base.transform.position = Vector3.Lerp(this.startPos, this.endPos, single);
		base.transform.hasChanged = true;
		if (single >= 1f)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	public void UpdateDropPosition(Vector3 newDropPosition)
	{
		float size = TerrainMeta.Size.x;
		float highestPoint = TerrainMeta.HighestPoint.y + 250f;
		this.startPos = Vector3Ex.Range(-1f, 1f);
		this.startPos.y = 0f;
		this.startPos.Normalize();
		this.startPos = this.startPos * (size * 2f);
		this.startPos.y = highestPoint;
		this.endPos = this.startPos * -1f;
		this.endPos.y = this.startPos.y;
		this.startPos += newDropPosition;
		this.endPos += newDropPosition;
		this.secondsToTake = Vector3.Distance(this.startPos, this.endPos) / 50f;
		this.secondsToTake *= UnityEngine.Random.Range(0.95f, 1.05f);
		base.transform.position = this.startPos;
		base.transform.rotation = Quaternion.LookRotation(this.endPos - this.startPos);
		this.dropPosition = newDropPosition;
		Interface.CallHook("OnAirdrop", this, newDropPosition);
	}
}