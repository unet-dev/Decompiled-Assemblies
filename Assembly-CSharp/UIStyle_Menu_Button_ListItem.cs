using System;
using UnityEngine;
using UnityEngine.UI;

public class UIStyle_Menu_Button_ListItem : MonoBehaviour, IClientComponent
{
	public bool apply;

	public UIStyle_Menu_Button_ListItem()
	{
	}

	private void OnValidate()
	{
		if (base.GetComponent<Image>() == null)
		{
			return;
		}
		if (base.GetComponent<Button>() == null)
		{
			return;
		}
		base.GetComponent<Image>().color = Color.white;
		ColorBlock component = base.GetComponent<Button>().colors;
		component.normalColor = new Color32(43, 41, 36, 255);
		component.highlightedColor = new Color32(72, 86, 46, 255);
		component.pressedColor = new Color32(37, 86, 122, 255);
		component.disabledColor = new Color32(72, 86, 46, 255);
		component.colorMultiplier = 1f;
		component.fadeDuration = 0.1f;
		base.GetComponent<Button>().colors = component;
	}
}