using System;
using UnityEngine;
using UnityEngine.UI;

public class TweakUIToggle : MonoBehaviour
{
	public Toggle toggleControl;

	public string convarName = "effects.motionblur";

	public bool inverse;

	internal ConsoleSystem.Command conVar;

	public TweakUIToggle()
	{
	}

	protected void Awake()
	{
		this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
		if (this.conVar != null)
		{
			this.UpdateToggleState();
			return;
		}
		Debug.LogWarning(string.Concat("Tweak Toggle Convar Missing: ", this.convarName));
	}

	protected void OnEnable()
	{
		this.UpdateToggleState();
	}

	public void OnToggleChanged()
	{
		this.UpdateConVar();
	}

	private void UpdateConVar()
	{
		if (this.conVar == null)
		{
			return;
		}
		bool flag = this.toggleControl.isOn;
		if (this.inverse)
		{
			flag = !flag;
		}
		if (this.conVar.AsBool == flag)
		{
			return;
		}
		this.conVar.Set(flag);
	}

	private void UpdateToggleState()
	{
		if (this.conVar == null)
		{
			return;
		}
		bool asBool = this.conVar.AsBool;
		if (this.inverse)
		{
			asBool = !asBool;
		}
		this.toggleControl.isOn = asBool;
	}
}