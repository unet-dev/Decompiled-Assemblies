using ConVar;
using System;
using UnityEngine;

public class NPCDoorTriggerBox : MonoBehaviour
{
	private Door door;

	private static int playerServerLayer;

	static NPCDoorTriggerBox()
	{
		NPCDoorTriggerBox.playerServerLayer = -1;
	}

	public NPCDoorTriggerBox()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.door == null || this.door.isClient || this.door.IsLocked())
		{
			return;
		}
		if (!this.door.isSecurityDoor && this.door.IsOpen())
		{
			return;
		}
		if (this.door.isSecurityDoor && !this.door.IsOpen())
		{
			return;
		}
		if (NPCDoorTriggerBox.playerServerLayer < 0)
		{
			NPCDoorTriggerBox.playerServerLayer = LayerMask.NameToLayer("Player (Server)");
		}
		if ((other.gameObject.layer & NPCDoorTriggerBox.playerServerLayer) > 0)
		{
			BasePlayer component = other.gameObject.GetComponent<BasePlayer>();
			if (component != null && component.IsNpc && !this.door.isSecurityDoor)
			{
				this.door.SetOpen(true);
			}
		}
	}

	public void Setup(Door d)
	{
		this.door = d;
		base.transform.SetParent(this.door.transform, false);
		base.gameObject.layer = 18;
		BoxCollider npcDoorTriggerSize = base.gameObject.AddComponent<BoxCollider>();
		npcDoorTriggerSize.isTrigger = true;
		npcDoorTriggerSize.center = Vector3.zero;
		npcDoorTriggerSize.size = Vector3.one * AI.npc_door_trigger_size;
	}
}