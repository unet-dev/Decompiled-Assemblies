using System;
using UnityEngine;
using UnityEngine.UI;

public class PieOption : MonoBehaviour
{
	public PieShape background;

	public Image imageIcon;

	public PieOption()
	{
	}

	public void UpdateOption(float startSlice, float sliceSize, float border, string optionTitle, float outerSize, float innerSize, float imageSize, Sprite sprite)
	{
		if (this.background == null)
		{
			return;
		}
		float single = this.background.rectTransform.rect.height * 0.5f;
		float single1 = single * (innerSize + (outerSize - innerSize) * 0.5f);
		float single2 = single * (outerSize - innerSize);
		this.background.startRadius = startSlice;
		this.background.endRadius = startSlice + sliceSize;
		this.background.border = border;
		this.background.outerSize = outerSize;
		this.background.innerSize = innerSize;
		this.background.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0f);
		float single3 = startSlice + sliceSize * 0.5f;
		float single4 = Mathf.Sin(single3 * 0.0174532924f) * single1;
		float single5 = Mathf.Cos(single3 * 0.0174532924f) * single1;
		this.imageIcon.rectTransform.localPosition = new Vector3(single4, single5);
		this.imageIcon.rectTransform.sizeDelta = new Vector2(single2 * imageSize, single2 * imageSize);
		this.imageIcon.sprite = sprite;
	}
}