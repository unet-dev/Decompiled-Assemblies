using System;
using TMPro;
using UnityEngine;

public class BlueprintCategoryButton : MonoBehaviour, IInventoryChanged
{
	public TextMeshProUGUI amountLabel;

	public ItemCategory Category;

	public GameObject BackgroundHighlight;

	public SoundDefinition clickSound;

	public SoundDefinition hoverSound;

	public BlueprintCategoryButton()
	{
	}
}