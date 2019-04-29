using System;
using UnityEngine;

public class LootPanelVendingMachine : LootPanel
{
	public GameObject sellOrderPrefab;

	public GameObject sellOrderContainer;

	public GameObject busyOverlayPrefab;

	private GameObject busyOverlayInstance;

	public LootPanelVendingMachine()
	{
	}
}