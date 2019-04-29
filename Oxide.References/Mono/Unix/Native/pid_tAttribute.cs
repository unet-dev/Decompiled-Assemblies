using System;

namespace Mono.Unix.Native
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class pid_tAttribute : MapAttribute
	{
		public pid_tAttribute() : base("pid_t")
		{
		}
	}
}