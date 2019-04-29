using ConVar;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIRootScaled : UIRoot
{
	private static UIRootScaled Instance;

	public CanvasScaler scaler;

	public static Canvas DragOverlayCanvas
	{
		get
		{
			return UIRootScaled.Instance.overlayCanvas;
		}
	}

	static UIRootScaled()
	{
	}

	public UIRootScaled()
	{
	}

	protected override void Awake()
	{
		UIRootScaled.Instance = this;
		base.Awake();
	}

	protected override void Refresh()
	{
		Vector2 vector2 = new Vector2(1280f / ConVar.Graphics.uiscale, 720f / ConVar.Graphics.uiscale);
		if (this.scaler.referenceResolution != vector2)
		{
			this.scaler.referenceResolution = vector2;
		}
	}
}