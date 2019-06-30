using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TweakUISlider : MonoBehaviour
{
	public Slider sliderControl;

	public TextMeshProUGUI textControl;

	public string convarName = "effects.motionblur";

	internal ConsoleSystem.Command conVar;

	public TweakUISlider()
	{
	}

	protected void Awake()
	{
		this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
		if (this.conVar != null)
		{
			this.UpdateSliderValue();
			this.UpdateTextValue();
			return;
		}
		Debug.LogWarning(string.Concat("Tweak Slider Convar Missing: ", this.convarName));
	}

	public void OnChanged()
	{
		this.UpdateConVar();
		this.UpdateTextValue();
		this.UpdateSliderValue();
	}

	protected void OnEnable()
	{
		this.UpdateSliderValue();
		this.UpdateTextValue();
	}

	private void UpdateConVar()
	{
		if (this.conVar == null)
		{
			return;
		}
		float single = this.sliderControl.@value;
		if (this.conVar.AsFloat == single)
		{
			return;
		}
		this.conVar.Set(single);
	}

	private void UpdateSliderValue()
	{
		if (this.conVar == null)
		{
			return;
		}
		float asFloat = this.conVar.AsFloat;
		if (this.sliderControl.@value == asFloat)
		{
			return;
		}
		this.sliderControl.@value = asFloat;
	}

	private void UpdateTextValue()
	{
		float single;
		if (this.sliderControl.wholeNumbers)
		{
			TextMeshProUGUI str = this.textControl;
			single = this.sliderControl.@value;
			str.text = single.ToString("N0");
			return;
		}
		TextMeshProUGUI textMeshProUGUI = this.textControl;
		single = this.sliderControl.@value;
		textMeshProUGUI.text = single.ToString("0.0");
	}
}