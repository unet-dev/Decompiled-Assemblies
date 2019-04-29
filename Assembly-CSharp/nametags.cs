using System;

[Factory("nametags")]
public class nametags : ConsoleSystem
{
	[ClientVar(Saved=true)]
	public static bool enabled;

	static nametags()
	{
		nametags.enabled = true;
	}

	public nametags()
	{
	}
}