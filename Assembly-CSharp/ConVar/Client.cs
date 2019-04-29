using System;

namespace ConVar
{
	public class Client
	{
		public static float tickrate;

		static Client()
		{
			Client.tickrate = 20f;
		}

		public Client()
		{
		}
	}
}