using System;
using UnityEngine;

public class TweakUI : SingletonComponent<TweakUI>
{
	public static bool isOpen;

	static TweakUI()
	{
	}

	public TweakUI()
	{
	}

	protected bool CanToggle()
	{
		if (!LevelManager.isLoaded)
		{
			return false;
		}
		return true;
	}

	public void SetVisible(bool b)
	{
		if (b)
		{
			TweakUI.isOpen = true;
			return;
		}
		TweakUI.isOpen = false;
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "writecfg", Array.Empty<object>());
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "trackir.refresh", Array.Empty<object>());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F2) && this.CanToggle())
		{
			this.SetVisible(!TweakUI.isOpen);
		}
	}
}