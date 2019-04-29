using System;
using UnityEngine;

public class NeedsCursor : MonoBehaviour, IClientComponent
{
	public NeedsCursor()
	{
	}

	private void Update()
	{
		CursorManager.HoldOpen(false);
	}
}