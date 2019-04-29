using System;
using UnityEngine;

public class DevTimeAdjust : MonoBehaviour
{
	public DevTimeAdjust()
	{
	}

	private void OnGUI()
	{
		if (!TOD_Sky.Instance)
		{
			return;
		}
		float single = (float)Screen.width * 0.2f;
		Rect rect = new Rect((float)Screen.width - (single + 20f), (float)Screen.height - 30f, single, 20f);
		float hour = TOD_Sky.Instance.Cycle.Hour;
		hour = GUI.HorizontalSlider(rect, hour, 0f, 24f);
		rect.y = rect.y - 20f;
		GUI.Label(rect, "Time Of Day");
		if (hour != TOD_Sky.Instance.Cycle.Hour)
		{
			TOD_Sky.Instance.Cycle.Hour = hour;
			PlayerPrefs.SetFloat("DevTime", hour);
		}
	}

	private void Start()
	{
		if (!TOD_Sky.Instance)
		{
			return;
		}
		TOD_Sky.Instance.Cycle.Hour = PlayerPrefs.GetFloat("DevTime");
	}
}