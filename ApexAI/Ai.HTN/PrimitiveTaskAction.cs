using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Apex.Ai.HTN
{
	public class PrimitiveTaskAction : ActionBase
	{
		[ApexSerialization]
		[FriendlyName("Actions", "Actions that should be run on plan execution rather than on plan generation.")]
		private List<IAction> _actions = new List<IAction>(2);

		public List<IAction> Actions
		{
			get
			{
				return this._actions;
			}
		}

		public string Name
		{
			get;
			internal set;
		}

		public PrimitiveTaskAction()
		{
		}

		public override void Execute(IAIContext context)
		{
		}
	}
}