using Facepunch.Models;
using System;

namespace Facepunch.Models.Feedback
{
	public struct GameInfo
	{
		public Facepunch.Models.AppInfo AppInfo;

		public Facepunch.Models.Auth Auth;

		public PlayerInfo[] Players;

		public int Version
		{
			get
			{
				return 2;
			}
		}
	}
}