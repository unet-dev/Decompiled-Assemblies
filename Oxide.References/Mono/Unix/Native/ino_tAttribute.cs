using System;

namespace Mono.Unix.Native
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class ino_tAttribute : MapAttribute
	{
		public ino_tAttribute() : base("ino_t")
		{
		}
	}
}