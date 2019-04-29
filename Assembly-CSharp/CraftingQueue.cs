using System;
using UnityEngine;

public class CraftingQueue : SingletonComponent<CraftingQueue>
{
	public GameObject queueContainer;

	public GameObject queueItemPrefab;

	public CraftingQueue()
	{
	}
}