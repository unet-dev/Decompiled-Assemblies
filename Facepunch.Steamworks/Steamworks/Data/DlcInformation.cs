using Steamworks;
using System;
using System.Runtime.CompilerServices;

namespace Steamworks.Data
{
	public struct DlcInformation
	{
		public Steamworks.AppId AppId
		{
			get;
			internal set;
		}

		public bool Available
		{
			get;
			internal set;
		}

		public string Name
		{
			get;
			internal set;
		}
	}
}