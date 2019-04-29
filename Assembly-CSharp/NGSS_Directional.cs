using ConVar;
using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class NGSS_Directional : MonoBehaviour
{
	[Range(0f, 0.02f)]
	[Tooltip("Overall softness for both PCF and PCSS shadows.\nRecommended value: 0.01.")]
	public float PCSS_GLOBAL_SOFTNESS = 0.01f;

	[Range(0f, 1f)]
	[Tooltip("PCSS softness when shadows is close to caster.\nRecommended value: 0.05.")]
	public float PCSS_FILTER_DIR_MIN = 0.05f;

	[Range(0f, 0.5f)]
	[Tooltip("PCSS softness when shadows is far from caster.\nRecommended value: 0.25.\nIf too high can lead to visible artifacts when early bailout is enabled.")]
	public float PCSS_FILTER_DIR_MAX = 0.25f;

	[Range(0f, 10f)]
	[Tooltip("Amount of banding or noise. Example: 0.0 gives 100 % Banding and 10.0 gives 100 % Noise.")]
	public float BANDING_NOISE_AMOUNT = 1f;

	[Tooltip("Recommended values: Mobile = 16, Consoles = 25, Desktop Low = 32, Desktop High = 64")]
	public NGSS_Directional.SAMPLER_COUNT SAMPLERS_COUNT;

	public NGSS_Directional()
	{
	}

	private void SetGlobalSettings(bool enabled)
	{
		if (enabled)
		{
			Shader.SetGlobalFloat("NGSS_PCSS_GLOBAL_SOFTNESS", this.PCSS_GLOBAL_SOFTNESS);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MIN", (this.PCSS_FILTER_DIR_MIN > this.PCSS_FILTER_DIR_MAX ? this.PCSS_FILTER_DIR_MAX : this.PCSS_FILTER_DIR_MIN));
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MAX", (this.PCSS_FILTER_DIR_MAX < this.PCSS_FILTER_DIR_MIN ? this.PCSS_FILTER_DIR_MIN : this.PCSS_FILTER_DIR_MAX));
			Shader.SetGlobalFloat("NGSS_POISSON_SAMPLING_NOISE_DIR", this.BANDING_NOISE_AMOUNT);
		}
	}

	private void Update()
	{
		this.SetGlobalSettings(ConVar.Graphics.shadowquality == 2);
	}

	public enum SAMPLER_COUNT
	{
		SAMPLERS_16,
		SAMPLERS_25,
		SAMPLERS_32,
		SAMPLERS_64
	}
}