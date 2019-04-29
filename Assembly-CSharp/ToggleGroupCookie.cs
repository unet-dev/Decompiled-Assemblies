using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleGroupCookie : MonoBehaviour
{
	public ToggleGroup @group
	{
		get
		{
			return base.GetComponent<ToggleGroup>();
		}
	}

	public ToggleGroupCookie()
	{
	}

	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleChanged));
		}
	}

	private void OnEnable()
	{
		string str = PlayerPrefs.GetString(string.Concat("ToggleGroupCookie_", base.name));
		if (!string.IsNullOrEmpty(str))
		{
			Transform transforms = base.transform.Find(str);
			if (transforms)
			{
				Toggle component = transforms.GetComponent<Toggle>();
				if (component)
				{
					Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
					for (int i = 0; i < (int)componentsInChildren.Length; i++)
					{
						componentsInChildren[i].isOn = false;
					}
					component.isOn = false;
					component.isOn = true;
					this.SetupListeners();
					return;
				}
			}
		}
		Toggle toggle = this.@group.ActiveToggles().FirstOrDefault<Toggle>((Toggle x) => x.isOn);
		if (toggle)
		{
			toggle.isOn = false;
			toggle.isOn = true;
		}
		this.SetupListeners();
	}

	private void OnToggleChanged(bool b)
	{
		Toggle toggle = ((IEnumerable<Toggle>)base.GetComponentsInChildren<Toggle>()).FirstOrDefault<Toggle>((Toggle x) => x.isOn);
		if (toggle)
		{
			PlayerPrefs.SetString(string.Concat("ToggleGroupCookie_", base.name), toggle.gameObject.name);
		}
	}

	private void SetupListeners()
	{
		Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
		}
	}
}