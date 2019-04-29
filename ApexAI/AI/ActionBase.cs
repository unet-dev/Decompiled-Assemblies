using Apex.Serialization;
using System;

namespace Apex.AI
{
	[ApexSerializedType]
	public abstract class ActionBase : IAction
	{
		protected ActionBase()
		{
		}

		public abstract void Execute(IAIContext context);
	}
}