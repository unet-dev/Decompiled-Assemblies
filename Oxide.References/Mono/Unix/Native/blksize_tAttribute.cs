using System;

namespace Mono.Unix.Native
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class blksize_tAttribute : MapAttribute
	{
		public blksize_tAttribute() : base("blksize_t")
		{
		}
	}
}