using System;

namespace Mono.Unix.Native
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class uid_tAttribute : MapAttribute
	{
		public uid_tAttribute() : base("uid_t")
		{
		}
	}
}