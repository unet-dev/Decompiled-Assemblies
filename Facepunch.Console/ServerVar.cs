using System;

public class ServerVar : ConsoleVar
{
	public ServerVar()
	{
		this.Serverside = true;
		this.ServerAdmin = true;
	}
}