using System;

public class MenuButtonArcadeEntity : TextArcadeEntity
{
	public string titleText = "";

	public string selectionSuffix = " - ";

	public string clickMessage = "";

	public MenuButtonArcadeEntity()
	{
	}

	public bool IsHighlighted()
	{
		return this.alpha == 1f;
	}
}