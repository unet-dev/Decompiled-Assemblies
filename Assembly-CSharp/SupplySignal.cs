using System;
using UnityEngine;

public class SupplySignal : TimedExplosive
{
	public GameObjectRef smokeEffectPrefab;

	public GameObjectRef EntityToCreate;

	[NonSerialized]
	public GameObject smokeEffect;

	public SupplySignal()
	{
	}

	public override void Explode()
	{
		GameManager gameManager = GameManager.server;
		string entityToCreate = this.EntityToCreate.resourcePath;
		Vector3 vector3 = new Vector3();
		Quaternion quaternion = new Quaternion();
		BaseEntity baseEntity = gameManager.CreateEntity(entityToCreate, vector3, quaternion, true);
		if (baseEntity)
		{
			Vector3 vector31 = new Vector3(UnityEngine.Random.Range(-20f, 20f), 0f, UnityEngine.Random.Range(-20f, 20f));
			baseEntity.SendMessage("InitDropPosition", base.transform.position + vector31, SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
		}
		base.Invoke(new Action(this.FinishUp), 210f);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	public void FinishUp()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}
}