using System;
using UnityEngine;
using UnityEngine.UI;

public class UIStyle_Menu_Input : MonoBehaviour, IClientComponent
{
	public bool apply;

	public UIStyle_Menu_Input()
	{
	}

	private void OnValidate()
	{
		base.GetComponent<Image>().color = Color.white;
		ColorBlock component = base.GetComponent<InputField>().colors;
		component.normalColor = new Color32(43, 41, 36, 255);
		component.highlightedColor = new Color32(72, 86, 46, 255);
		component.pressedColor = new Color32(37, 86, 122, 255);
		component.disabledColor = new Color32(33, 31, 26, 255);
		base.GetComponent<InputField>().colors = component;
	}
}