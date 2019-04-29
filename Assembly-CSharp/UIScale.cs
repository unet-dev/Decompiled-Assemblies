using ConVar;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIScale : MonoBehaviour
{
	public CanvasScaler scaler;

	public UIScale()
	{
	}

	private void Update()
	{
		Vector2 vector2 = new Vector2(1280f / ConVar.Graphics.uiscale, 720f / ConVar.Graphics.uiscale);
		if (this.scaler.referenceResolution != vector2)
		{
			this.scaler.referenceResolution = vector2;
		}
	}
}