using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemTextValue : MonoBehaviour
{
	public Text text;

	public Color bad;

	public Color good;

	public bool negativestat;

	public bool asPercentage;

	public bool useColors = true;

	public bool signed = true;

	public string suffix;

	public ItemTextValue()
	{
	}

	public void SetValue(float val, int numDecimals = 0, string overrideText = "")
	{
		this.text.text = (overrideText == "" ? string.Format(string.Concat("{0}{1:n", numDecimals, "}"), (val <= 0f || !this.signed ? "" : "+"), val) : overrideText);
		if (this.asPercentage)
		{
			Text text = this.text;
			text.text = string.Concat(text.text, " %");
		}
		if (this.suffix != "")
		{
			Text text1 = this.text;
			text1.text = string.Concat(text1.text, this.suffix);
		}
		bool flag = (val > 0f ? true : false);
		if (this.negativestat)
		{
			flag = !flag;
		}
		if (this.useColors)
		{
			this.text.color = (flag ? this.good : this.bad);
		}
	}
}