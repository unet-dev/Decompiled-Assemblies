using System;

namespace Facepunch.Models.Analytics
{
	internal class SessionUpdate
	{
		public string Uid;

		public string Sid;

		public int[] Frames;

		public int Mem;

		public int Gc;

		public string StatText;

		public string PerfText;

		public int Version
		{
			get
			{
				return 2;
			}
		}

		public SessionUpdate()
		{
		}
	}
}