using System;

namespace Mono.Unix.Native
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class fsblkcnt_tAttribute : MapAttribute
	{
		public fsblkcnt_tAttribute() : base("fsblkcnt_t")
		{
		}
	}
}