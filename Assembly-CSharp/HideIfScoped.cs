using System;
using UnityEngine;

public class HideIfScoped : MonoBehaviour
{
	public Renderer[] renderers;

	public HideIfScoped()
	{
	}

	public void SetVisible(bool vis)
	{
		Renderer[] rendererArray = this.renderers;
		for (int i = 0; i < (int)rendererArray.Length; i++)
		{
			rendererArray[i].enabled = vis;
		}
	}
}