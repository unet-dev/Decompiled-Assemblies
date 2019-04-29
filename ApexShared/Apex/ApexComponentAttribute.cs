using System;
using System.Runtime.CompilerServices;

namespace Apex
{
	[AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
	public class ApexComponentAttribute : Attribute
	{
		public string category
		{
			get;
			private set;
		}

		public ApexComponentAttribute(string category)
		{
			this.category = category;
		}
	}
}