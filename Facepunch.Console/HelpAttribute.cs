using System;

public class HelpAttribute : Attribute
{
	public string Help;

	public HelpAttribute(string h)
	{
		this.Help = h;
	}
}