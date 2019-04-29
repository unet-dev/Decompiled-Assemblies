using System;
using UnityEngine;

public class LootPanelContainer : MonoBehaviour
{
	public static string containerName;

	public GameObject NoLootPanel;

	static LootPanelContainer()
	{
		LootPanelContainer.containerName = "generic";
	}

	public LootPanelContainer()
	{
	}
}