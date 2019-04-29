using System;

namespace Mono.Unix.Native
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class dev_tAttribute : MapAttribute
	{
		public dev_tAttribute() : base("dev_t")
		{
		}
	}
}