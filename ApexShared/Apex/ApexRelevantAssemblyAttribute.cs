using System;

namespace Apex
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class ApexRelevantAssemblyAttribute : Attribute
	{
		public ApexRelevantAssemblyAttribute()
		{
		}
	}
}