using System;

namespace Facepunch.Models.Analytics
{
	internal class SessionClose
	{
		public string Uid;

		public string Sid;

		public SessionUpdate FinalUpdate;

		public int Version
		{
			get
			{
				return 2;
			}
		}

		public SessionClose()
		{
		}
	}
}