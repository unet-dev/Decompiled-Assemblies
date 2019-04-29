using Apex.AI;
using System;
using System.Reflection;

namespace Rust.Ai
{
	public abstract class BaseAction : ActionBase
	{
		private string DebugName;

		public BaseAction()
		{
			this.DebugName = base.GetType().Name;
		}

		public abstract void DoExecute(BaseContext context);

		public override void Execute(IAIContext context)
		{
			BaseContext baseContext = context as BaseContext;
			if (baseContext != null)
			{
				this.DoExecute(baseContext);
			}
		}
	}
}