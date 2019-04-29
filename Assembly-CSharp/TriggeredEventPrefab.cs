using System;
using UnityEngine;

public class TriggeredEventPrefab : TriggeredEvent
{
	public GameObjectRef targetPrefab;

	public TriggeredEventPrefab()
	{
	}

	private void RunEvent()
	{
		Debug.Log(string.Concat("[event] ", this.targetPrefab.resourcePath));
		GameManager gameManager = GameManager.server;
		string str = this.targetPrefab.resourcePath;
		Vector3 vector3 = new Vector3();
		Quaternion quaternion = new Quaternion();
		BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
		if (baseEntity)
		{
			baseEntity.SendMessage("TriggeredEventSpawn", SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
		}
	}
}