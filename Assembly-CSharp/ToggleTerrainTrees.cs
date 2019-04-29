using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTerrainTrees : MonoBehaviour
{
	public Toggle toggleControl;

	public Text textControl;

	public ToggleTerrainTrees()
	{
	}

	protected void OnEnable()
	{
		if (Terrain.activeTerrain)
		{
			this.toggleControl.isOn = Terrain.activeTerrain.drawTreesAndFoliage;
		}
	}

	public void OnToggleChanged()
	{
		if (Terrain.activeTerrain)
		{
			Terrain.activeTerrain.drawTreesAndFoliage = this.toggleControl.isOn;
		}
	}

	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = "Terrain Trees";
		}
	}
}