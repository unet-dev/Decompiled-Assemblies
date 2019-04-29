using Facepunch;
using Facepunch.Models;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

public static class DeveloperList
{
	public static bool Contains(ulong steamid)
	{
		if (Application.Manifest == null)
		{
			return false;
		}
		if (Application.Manifest.Administrators == null)
		{
			return false;
		}
		return Application.Manifest.Administrators.Any<Facepunch.Models.Manifest.Administrator>((Facepunch.Models.Manifest.Administrator x) => x.UserId == steamid.ToString());
	}

	public static bool IsDeveloper(BasePlayer ply)
	{
		if (ply == null)
		{
			return false;
		}
		return DeveloperList.Contains(ply.userID);
	}
}