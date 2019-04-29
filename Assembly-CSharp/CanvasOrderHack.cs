using System;
using UnityEngine;

public class CanvasOrderHack : MonoBehaviour
{
	public CanvasOrderHack()
	{
	}

	private void OnEnable()
	{
		int i;
		Canvas[] componentsInChildren = base.GetComponentsInChildren<Canvas>(true);
		for (i = 0; i < (int)componentsInChildren.Length; i++)
		{
			Canvas canva = componentsInChildren[i];
			if (canva.overrideSorting)
			{
				Canvas canva1 = canva;
				canva1.sortingOrder = canva1.sortingOrder + 1;
			}
		}
		componentsInChildren = base.GetComponentsInChildren<Canvas>(true);
		for (i = 0; i < (int)componentsInChildren.Length; i++)
		{
			Canvas canva2 = componentsInChildren[i];
			if (canva2.overrideSorting)
			{
				Canvas canva3 = canva2;
				canva3.sortingOrder = canva3.sortingOrder - 1;
			}
		}
	}
}