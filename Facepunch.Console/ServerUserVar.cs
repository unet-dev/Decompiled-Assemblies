using System;

public class ServerUserVar : ConsoleVar
{
	public ServerUserVar()
	{
		this.Serverside = true;
		this.ServerAdmin = false;
		this.ServerUser = true;
	}
}