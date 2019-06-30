using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TweakUIDropdown : MonoBehaviour
{
	public Button Left;

	public Button Right;

	public TextMeshProUGUI Current;

	public Image BackgroundImage;

	public TweakUIDropdown.NameValue[] nameValues;

	public string convarName = "effects.motionblur";

	public bool assignImageColor;

	internal ConsoleSystem.Command conVar;

	public int currentValue;

	public TweakUIDropdown()
	{
	}

	protected void Awake()
	{
		this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
		if (this.conVar != null)
		{
			this.UpdateState();
			return;
		}
		Debug.LogWarning(string.Concat("TweakUIDropDown Convar Missing: ", this.convarName));
	}

	public void ChangeValue(int change)
	{
		this.currentValue += change;
		if (this.currentValue < 0)
		{
			this.currentValue = 0;
		}
		if (this.currentValue > (int)this.nameValues.Length - 1)
		{
			this.currentValue = (int)this.nameValues.Length - 1;
		}
		this.Left.interactable = this.currentValue > 0;
		this.Right.interactable = this.currentValue < (int)this.nameValues.Length - 1;
		this.UpdateConVar();
	}

	protected void OnEnable()
	{
		this.UpdateState();
	}

	public void OnValueChanged()
	{
		this.UpdateConVar();
	}

	private void UpdateConVar()
	{
		TweakUIDropdown.NameValue nameValue = this.nameValues[this.currentValue];
		if (this.conVar == null)
		{
			return;
		}
		if (this.conVar.String == nameValue.@value)
		{
			return;
		}
		this.conVar.Set(nameValue.@value);
		this.UpdateState();
	}

	private void UpdateState()
	{
		if (this.conVar == null)
		{
			return;
		}
		string str = this.conVar.String;
		for (int i = 0; i < (int)this.nameValues.Length; i++)
		{
			if (this.nameValues[i].@value == str)
			{
				this.Current.text = this.nameValues[i].label.translated;
				this.currentValue = i;
				if (this.assignImageColor)
				{
					this.BackgroundImage.color = this.nameValues[i].imageColor;
				}
			}
		}
	}

	[Serializable]
	public class NameValue
	{
		public string @value;

		public Color imageColor;

		public Translate.Phrase label;

		public NameValue()
		{
		}
	}
}