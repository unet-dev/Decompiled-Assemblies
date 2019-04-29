using System;
using UnityEngine;

public class SupplyDrop : LootContainer
{
	public GameObjectRef parachutePrefab;

	public BaseEntity parachute;

	public SupplyDrop()
	{
	}

	public void MakeLootable()
	{
		this.isLootable = true;
	}

	private void OnCollisionEnter(Collision collision)
	{
		this.RemoveParachute();
		this.MakeLootable();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.RemoveParachute();
	}

	public void RemoveParachute()
	{
		if (this.parachute)
		{
			this.parachute.Kill(BaseNetworkable.DestroyMode.None);
			this.parachute = null;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (this.parachutePrefab.isValid)
		{
			GameManager gameManager = GameManager.server;
			string str = this.parachutePrefab.resourcePath;
			Vector3 vector3 = new Vector3();
			Quaternion quaternion = new Quaternion();
			this.parachute = gameManager.CreateEntity(str, vector3, quaternion, true);
		}
		if (this.parachute)
		{
			this.parachute.SetParent(this, "parachute_attach", false, false);
			this.parachute.Spawn();
		}
		this.isLootable = false;
		base.Invoke(new Action(this.MakeLootable), 300f);
	}
}