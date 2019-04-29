using ConVar;
using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DropUtil
{
	public DropUtil()
	{
	}

	public static void DropItems(ItemContainer container, Vector3 position, float chance = 1f)
	{
		if (!Server.dropitems)
		{
			return;
		}
		if (container == null)
		{
			return;
		}
		if (container.itemList == null)
		{
			return;
		}
		if (Interface.CallHook("OnContainerDropItems", container) != null)
		{
			return;
		}
		float single = 0.25f;
		Item[] array = container.itemList.ToArray();
		for (int i = 0; i < (int)array.Length; i++)
		{
			Item item = array[i];
			if (UnityEngine.Random.Range(0f, 1f) <= chance)
			{
				float single1 = UnityEngine.Random.Range(0f, 2f);
				item.RemoveFromContainer();
				Vector3 vector3 = position + new Vector3(UnityEngine.Random.Range(-single, single), 1f, UnityEngine.Random.Range(-single, single));
				Quaternion quaternion = new Quaternion();
				BaseEntity baseEntity = item.CreateWorldObject(vector3, quaternion, null, 0);
				if (baseEntity == null)
				{
					item.Remove(0f);
				}
				else if (single1 > 0f)
				{
					baseEntity.SetVelocity(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(-1f, 1f)) * single1);
					baseEntity.SetAngularVelocity(new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f)) * single1);
				}
			}
		}
	}
}