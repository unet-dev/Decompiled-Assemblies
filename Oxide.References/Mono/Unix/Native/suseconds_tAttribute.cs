using System;

namespace Mono.Unix.Native
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class suseconds_tAttribute : MapAttribute
	{
		public suseconds_tAttribute() : base("suseconds_t")
		{
		}
	}
}