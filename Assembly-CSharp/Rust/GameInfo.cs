using ConVar;
using System;
using UnityEngine;

namespace Rust
{
	internal static class GameInfo
	{
		internal static bool HasAchievements
		{
			get
			{
				return GameInfo.IsOfficialServer;
			}
		}

		internal static bool IsOfficialServer
		{
			get
			{
				if (UnityEngine.Application.isEditor)
				{
					return true;
				}
				return ConVar.Server.official;
			}
		}
	}
}