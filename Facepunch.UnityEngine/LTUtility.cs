using System;
using UnityEngine;

public class LTUtility
{
	public LTUtility()
	{
	}

	public static Vector3[] reverse(Vector3[] arr)
	{
		int num = 0;
		for (int i = (int)arr.Length - 1; num < i; i--)
		{
			Vector3 vector3 = arr[num];
			arr[num] = arr[i];
			arr[i] = vector3;
			num++;
		}
		return arr;
	}
}