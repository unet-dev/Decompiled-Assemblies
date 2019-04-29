using System;

namespace Mono.Unix.Native
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class time_tAttribute : MapAttribute
	{
		public time_tAttribute() : base("time_t")
		{
		}
	}
}