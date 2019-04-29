using System;

public class ConsoleVar : Attribute
{
	public string Name;

	public bool ClientAdmin;

	public bool ServerAdmin;

	public bool ServerUser;

	public bool Saved;

	public string Help;

	public bool ClientInfo;

	public bool Clientside;

	public bool Serverside;

	public bool EditorOnly;

	public bool AllowRunFromServer;

	public ConsoleVar()
	{
	}
}