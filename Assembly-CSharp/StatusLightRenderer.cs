using System;
using UnityEngine;

public class StatusLightRenderer : MonoBehaviour, IClientComponent
{
	public Material offMaterial;

	public Material onMaterial;

	private MaterialPropertyBlock propertyBlock;

	private Renderer targetRenderer;

	private Color lightColor;

	private Light targetLight;

	private int colorID;

	private int emissionID;

	public StatusLightRenderer()
	{
	}

	protected void Awake()
	{
		this.propertyBlock = new MaterialPropertyBlock();
		this.targetRenderer = base.GetComponent<Renderer>();
		this.targetLight = base.GetComponent<Light>();
		this.colorID = Shader.PropertyToID("_Color");
		this.emissionID = Shader.PropertyToID("_EmissionColor");
	}

	private Color GetColor(byte r, byte g, byte b, byte a)
	{
		return new Color32(r, g, b, a);
	}

	private Color GetColor(byte r, byte g, byte b, byte a, float intensity)
	{
		return new Color32(r, g, b, a) * intensity;
	}

	public void SetGreen()
	{
		this.propertyBlock.Clear();
		this.propertyBlock.SetColor(this.colorID, this.GetColor(19, 191, 13, 255));
		this.propertyBlock.SetColor(this.emissionID, this.GetColor(19, 191, 13, 255, 2.5f));
		this.lightColor = this.GetColor(156, 255, 102, 255);
		this.SetOn();
	}

	public void SetOff()
	{
		if (this.targetRenderer)
		{
			this.targetRenderer.sharedMaterial = this.offMaterial;
			this.targetRenderer.SetPropertyBlock(null);
		}
		if (this.targetLight)
		{
			this.targetLight.color = Color.clear;
		}
	}

	public void SetOn()
	{
		if (this.targetRenderer)
		{
			this.targetRenderer.sharedMaterial = this.onMaterial;
			this.targetRenderer.SetPropertyBlock(this.propertyBlock);
		}
		if (this.targetLight)
		{
			this.targetLight.color = this.lightColor;
		}
	}

	public void SetRed()
	{
		this.propertyBlock.Clear();
		this.propertyBlock.SetColor(this.colorID, this.GetColor(197, 46, 0, 255));
		this.propertyBlock.SetColor(this.emissionID, this.GetColor(191, 0, 2, 255, 2.916925f));
		this.lightColor = this.GetColor(255, 111, 102, 255);
		this.SetOn();
	}
}