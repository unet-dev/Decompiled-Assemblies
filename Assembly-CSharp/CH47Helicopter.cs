using System;
using UnityEngine;

public class CH47Helicopter : BaseHelicopterVehicle
{
	public GameObjectRef mapMarkerEntityPrefab;

	private BaseEntity mapMarkerInstance;

	public CH47Helicopter()
	{
	}

	public void CreateMapMarker()
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this.mapMarkerInstance = baseEntity;
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
	}

	public override void ServerInit()
	{
		this.rigidBody.isKinematic = false;
		base.ServerInit();
		this.CreateMapMarker();
	}
}