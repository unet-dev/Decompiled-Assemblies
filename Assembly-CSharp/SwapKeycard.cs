using System;
using UnityEngine;

public class SwapKeycard : MonoBehaviour
{
	public GameObject[] accessLevels;

	public SwapKeycard()
	{
	}

	public void UpdateAccessLevel(int level)
	{
		GameObject[] gameObjectArray = this.accessLevels;
		for (int i = 0; i < (int)gameObjectArray.Length; i++)
		{
			gameObjectArray[i].SetActive(false);
		}
		this.accessLevels[level - 1].SetActive(true);
	}
}