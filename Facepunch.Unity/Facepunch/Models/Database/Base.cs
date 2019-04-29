using Facepunch.Models;
using System;

namespace Facepunch.Models.Database
{
	public class Base
	{
		public string Parent;

		public Facepunch.Models.Auth Auth;

		public int Version
		{
			get
			{
				return 2;
			}
		}

		public Base()
		{
		}
	}
}