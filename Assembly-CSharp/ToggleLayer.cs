using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLayer : MonoBehaviour, IClientComponent
{
	public Toggle toggleControl;

	public Text textControl;

	public LayerSelect layer;

	public ToggleLayer()
	{
	}

	protected void OnEnable()
	{
		if (MainCamera.mainCamera)
		{
			this.toggleControl.isOn = (MainCamera.mainCamera.cullingMask & this.layer.Mask) != 0;
		}
	}

	public void OnToggleChanged()
	{
		if (MainCamera.mainCamera)
		{
			if (this.toggleControl.isOn)
			{
				Camera mask = MainCamera.mainCamera;
				mask.cullingMask = mask.cullingMask | this.layer.Mask;
				return;
			}
			Camera camera = MainCamera.mainCamera;
			camera.cullingMask = camera.cullingMask & ~this.layer.Mask;
		}
	}

	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = this.layer.Name;
		}
	}
}