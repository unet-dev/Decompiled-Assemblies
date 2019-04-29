using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTerrainRenderer : MonoBehaviour
{
	public Toggle toggleControl;

	public Text textControl;

	public ToggleTerrainRenderer()
	{
	}

	protected void OnEnable()
	{
		if (Terrain.activeTerrain)
		{
			this.toggleControl.isOn = Terrain.activeTerrain.drawHeightmap;
		}
	}

	public void OnToggleChanged()
	{
		if (Terrain.activeTerrain)
		{
			Terrain.activeTerrain.drawHeightmap = this.toggleControl.isOn;
		}
	}

	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = "Terrain Renderer";
		}
	}
}