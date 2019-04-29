using System;

namespace Oxide.Plugins
{
	[AttributeUsage(AttributeTargets.Field)]
	public class OnlinePlayersAttribute : Attribute
	{
		public OnlinePlayersAttribute()
		{
		}
	}
}