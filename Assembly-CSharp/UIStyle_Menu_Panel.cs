using System;
using UnityEngine;
using UnityEngine.UI;

public class UIStyle_Menu_Panel : MonoBehaviour, IClientComponent
{
	public bool toggle;

	public UIStyle_Menu_Panel()
	{
	}

	private void OnValidate()
	{
		base.GetComponent<Image>().color = new Color32(29, 32, 31, 255);
	}
}