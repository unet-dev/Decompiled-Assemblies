using System;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintButton : MonoBehaviour, IClientComponent, IInventoryChanged
{
	public Image image;

	public Button button;

	public CanvasGroup @group;

	public GameObject newNotification;

	public GameObject lockedOverlay;

	public BlueprintButton()
	{
	}
}