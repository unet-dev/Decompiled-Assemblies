using System;
using UnityEngine;

public class CH47ReinforcementListener : BaseEntity
{
	public string listenString;

	public GameObjectRef heliPrefab;

	public float startDist = 300f;

	public CH47ReinforcementListener()
	{
	}

	public void Call()
	{
		GameManager gameManager = GameManager.server;
		string str = this.heliPrefab.resourcePath;
		Vector3 vector3 = new Vector3();
		Quaternion quaternion = new Quaternion();
		CH47HelicopterAIController component = gameManager.CreateEntity(str, vector3, quaternion, true).GetComponent<CH47HelicopterAIController>();
		if (component)
		{
			Vector3 size = TerrainMeta.Size;
			CH47LandingZone closest = CH47LandingZone.GetClosest(base.transform.position);
			Vector3 vector31 = Vector3.zero;
			vector31.y = closest.transform.position.y;
			Vector3 vector32 = Vector3Ex.Direction2D(closest.transform.position, vector31);
			Vector3 vector33 = closest.transform.position + (vector32 * this.startDist);
			vector33.y = closest.transform.position.y;
			component.transform.position = vector33;
			component.SetLandingTarget(closest.transform.position);
			component.Spawn();
		}
	}

	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == this.listenString)
		{
			this.Call();
		}
	}
}