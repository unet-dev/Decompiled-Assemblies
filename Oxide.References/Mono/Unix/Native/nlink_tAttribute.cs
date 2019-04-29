using System;

namespace Mono.Unix.Native
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class nlink_tAttribute : MapAttribute
	{
		public nlink_tAttribute() : base("nlink_t")
		{
		}
	}
}