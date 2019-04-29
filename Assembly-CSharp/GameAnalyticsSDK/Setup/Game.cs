using System;
using System.Runtime.CompilerServices;

namespace GameAnalyticsSDK.Setup
{
	public class Game
	{
		public string GameKey
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public string SecretKey
		{
			get;
			private set;
		}

		public Game(string name, int id, string gameKey, string secretKey)
		{
			this.Name = name;
			this.ID = id;
			this.GameKey = gameKey;
			this.SecretKey = secretKey;
		}
	}
}