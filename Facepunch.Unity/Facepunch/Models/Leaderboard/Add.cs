using Facepunch.Models;
using System;

namespace Facepunch.Models.Leaderboard
{
	public class Add
	{
		public string Parent;

		public float Score;

		public string Extra;

		public bool ReplaceIfHigher;

		public bool ReplaceIfLower;

		public Facepunch.Models.Auth Auth;

		public int Version
		{
			get
			{
				return 2;
			}
		}

		public Add()
		{
		}
	}
}