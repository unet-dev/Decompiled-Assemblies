using System;
using UnityEngine;

public class CursorManager : SingletonComponent<CursorManager>
{
	private static int iHoldOpen;

	private static int iPreviousOpen;

	static CursorManager()
	{
	}

	public CursorManager()
	{
	}

	public static void HoldOpen(bool cursorVisible = false)
	{
		CursorManager.iHoldOpen++;
	}

	private void SwitchToGame()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (Cursor.visible)
		{
			Cursor.visible = false;
		}
	}

	private void SwitchToUI()
	{
		if (Cursor.lockState != CursorLockMode.None)
		{
			Cursor.lockState = CursorLockMode.None;
		}
		if (!Cursor.visible)
		{
			Cursor.visible = true;
		}
	}

	private void Update()
	{
		if (SingletonComponent<CursorManager>.Instance != this)
		{
			return;
		}
		if (CursorManager.iHoldOpen != 0 || CursorManager.iPreviousOpen != 0)
		{
			this.SwitchToUI();
		}
		else
		{
			this.SwitchToGame();
		}
		CursorManager.iPreviousOpen = CursorManager.iHoldOpen;
		CursorManager.iHoldOpen = 0;
	}
}