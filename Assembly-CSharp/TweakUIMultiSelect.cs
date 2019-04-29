using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class TweakUIMultiSelect : MonoBehaviour
{
	public ToggleGroup toggleGroup;

	public string convarName = "effects.motionblur";

	internal ConsoleSystem.Command conVar;

	public TweakUIMultiSelect()
	{
	}

	protected void Awake()
	{
		this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
		if (this.conVar != null)
		{
			this.UpdateToggleGroup();
			return;
		}
		Debug.LogWarning(string.Concat("Tweak Slider Convar Missing: ", this.convarName));
	}

	public void OnChanged()
	{
		this.UpdateConVar();
	}

	protected void OnEnable()
	{
		this.UpdateToggleGroup();
	}

	private void UpdateConVar()
	{
		if (this.conVar == null)
		{
			return;
		}
		Toggle toggle = (
			from x in (IEnumerable<Toggle>)this.toggleGroup.GetComponentsInChildren<Toggle>()
			where x.isOn
			select x).FirstOrDefault<Toggle>();
		if (toggle == null)
		{
			return;
		}
		if (this.conVar.String == toggle.name)
		{
			return;
		}
		this.conVar.Set(toggle.name);
	}

	private void UpdateToggleGroup()
	{
		if (this.conVar == null)
		{
			return;
		}
		string str = this.conVar.String;
		Toggle[] componentsInChildren = this.toggleGroup.GetComponentsInChildren<Toggle>();
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			Toggle toggle = componentsInChildren[i];
			toggle.isOn = toggle.name == str;
		}
	}
}