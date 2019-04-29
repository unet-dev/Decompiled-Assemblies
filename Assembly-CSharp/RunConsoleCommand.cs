using System;
using UnityEngine;

public class RunConsoleCommand : MonoBehaviour
{
	public RunConsoleCommand()
	{
	}

	public void ClientRun(string command)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, command, Array.Empty<object>());
	}
}