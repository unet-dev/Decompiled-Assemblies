using System;

public class ItemModXMasTreeDecoration : ItemMod
{
	public ItemModXMasTreeDecoration.xmasFlags flagsToChange;

	public ItemModXMasTreeDecoration()
	{
	}

	public enum xmasFlags
	{
		pineCones = 128,
		candyCanes = 256,
		gingerbreadMen = 512,
		Tinsel = 1024,
		Balls = 2048,
		Star = 16384,
		Lights = 32768
	}
}