using System;
using UnityEngine;
using UnityEngine.UI;

public class HudElement : MonoBehaviour
{
	public Text[] ValueText;

	public Image[] FilledImage;

	private float LastValue;

	public HudElement()
	{
	}

	private void SetImage(float f)
	{
		for (int i = 0; i < (int)this.FilledImage.Length; i++)
		{
			this.FilledImage[i].fillAmount = f;
		}
	}

	private void SetText(string v)
	{
		for (int i = 0; i < (int)this.ValueText.Length; i++)
		{
			this.ValueText[i].text = v;
		}
	}

	public void SetValue(float value, float max = 1f)
	{
		using (TimeWarning timeWarning = TimeWarning.New("HudElement.SetValue", 0.1f))
		{
			float single = value / max;
			if (single != this.LastValue)
			{
				this.LastValue = single;
				this.SetText(value.ToString("0"));
				this.SetImage(single);
			}
		}
	}
}