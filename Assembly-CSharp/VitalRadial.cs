using System;
using UnityEngine;

public class VitalRadial : MonoBehaviour
{
	public VitalRadial()
	{
	}

	private void Awake()
	{
		Debug.LogWarning(string.Concat("VitalRadial is obsolete ", base.transform.GetRecursiveName("")), base.gameObject);
	}
}