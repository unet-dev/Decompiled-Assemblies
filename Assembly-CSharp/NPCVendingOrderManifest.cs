using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/NPCVendingOrderManifest")]
public class NPCVendingOrderManifest : ScriptableObject
{
	public NPCVendingOrder[] orderList;

	public NPCVendingOrderManifest()
	{
	}

	public NPCVendingOrder GetFromIndex(int index)
	{
		return this.orderList[index];
	}

	public int GetIndex(NPCVendingOrder sample)
	{
		for (int i = 0; i < (int)this.orderList.Length; i++)
		{
			if (sample == this.orderList[i])
			{
				return i;
			}
		}
		return -1;
	}
}