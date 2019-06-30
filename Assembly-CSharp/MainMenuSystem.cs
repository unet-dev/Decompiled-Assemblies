using System;
using UnityEngine;

public class MainMenuSystem : SingletonComponent<MainMenuSystem>
{
	public static bool isOpen;

	public GameObject SessionButton;

	public GameObject SessionPanel;

	public GameObject NewsStoriesAlert;

	public GameObject ItemStoreAlert;

	static MainMenuSystem()
	{
		MainMenuSystem.isOpen = true;
	}

	public MainMenuSystem()
	{
	}
}