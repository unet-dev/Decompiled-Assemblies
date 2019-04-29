using System;

namespace Facepunch.Models.Leaderboard
{
	public class Entry
	{
		public int Rank;

		public float Score;

		public string UserId;

		public string Name;

		public string Country;

		public string City;

		public DateTime Created;

		public Entry()
		{
		}
	}
}