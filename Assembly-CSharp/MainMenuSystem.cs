using System;

public class MainMenuSystem : SingletonComponent<MainMenuSystem>
{
	public static bool isOpen;

	static MainMenuSystem()
	{
		MainMenuSystem.isOpen = true;
	}

	public MainMenuSystem()
	{
	}
}