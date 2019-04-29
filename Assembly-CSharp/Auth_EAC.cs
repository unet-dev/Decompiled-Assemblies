using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class Auth_EAC
{
	public static IEnumerator Run(Connection connection)
	{
		if (!connection.active)
		{
			yield break;
		}
		if (connection.rejected)
		{
			yield break;
		}
		connection.authStatus = string.Empty;
		EACServer.OnJoinGame(connection);
		while (connection.active && !connection.rejected && connection.authStatus == string.Empty)
		{
			yield return null;
		}
	}
}