using System;

namespace Mono.Unix.Native
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class off_tAttribute : MapAttribute
	{
		public off_tAttribute() : base("off_t")
		{
		}
	}
}