using System;

namespace Mono.Unix.Native
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class blkcnt_tAttribute : MapAttribute
	{
		public blkcnt_tAttribute() : base("blkcnt_t")
		{
		}
	}
}