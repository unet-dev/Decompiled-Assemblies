using System;
using UnityEngine;

public class EggSwap : MonoBehaviour
{
	public Renderer[] eggRenderers;

	public EggSwap()
	{
	}

	public void HideAll()
	{
		Renderer[] rendererArray = this.eggRenderers;
		for (int i = 0; i < (int)rendererArray.Length; i++)
		{
			rendererArray[i].enabled = false;
		}
	}

	public void Show(int index)
	{
		this.HideAll();
		this.eggRenderers[index].enabled = true;
	}
}