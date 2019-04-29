using ConVar;
using System;
using UnityEngine;

public class NPCBarricadeTriggerBox : MonoBehaviour
{
	private Barricade target;

	private static int playerServerLayer;

	static NPCBarricadeTriggerBox()
	{
		NPCBarricadeTriggerBox.playerServerLayer = -1;
	}

	public NPCBarricadeTriggerBox()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.target == null || this.target.isClient)
		{
			return;
		}
		if (NPCBarricadeTriggerBox.playerServerLayer < 0)
		{
			NPCBarricadeTriggerBox.playerServerLayer = LayerMask.NameToLayer("Player (Server)");
		}
		if ((other.gameObject.layer & NPCBarricadeTriggerBox.playerServerLayer) > 0)
		{
			BasePlayer component = other.gameObject.GetComponent<BasePlayer>();
			if (component != null && component.IsNpc)
			{
				this.target.Kill(BaseNetworkable.DestroyMode.Gib);
			}
		}
	}

	public void Setup(Barricade t)
	{
		this.target = t;
		base.transform.SetParent(this.target.transform, false);
		base.gameObject.layer = 18;
		BoxCollider npcDoorTriggerSize = base.gameObject.AddComponent<BoxCollider>();
		npcDoorTriggerSize.isTrigger = true;
		npcDoorTriggerSize.center = Vector3.zero;
		npcDoorTriggerSize.size = (Vector3.one * AI.npc_door_trigger_size) + (Vector3.right * this.target.bounds.size.x);
	}
}