using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour
{
	private Slider slider;

	public TimeSlider()
	{
	}

	public void OnValue(float f)
	{
		if (TOD_Sky.Instance == null)
		{
			return;
		}
		TOD_Sky.Instance.Cycle.Hour = f;
		TOD_Sky.Instance.UpdateAmbient();
		TOD_Sky.Instance.UpdateReflection();
		TOD_Sky.Instance.UpdateFog();
	}

	private void Start()
	{
		this.slider = base.GetComponent<Slider>();
	}

	private void Update()
	{
		if (TOD_Sky.Instance == null)
		{
			return;
		}
		this.slider.@value = TOD_Sky.Instance.Cycle.Hour;
	}
}