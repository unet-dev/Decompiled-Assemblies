using System;
using System.Collections.Generic;

namespace Apex.AI
{
	[FriendlyName("Terminable Composite Action", "An action comprised of one or more child actions, which are executed in order. These children can implement IRequireTermination.")]
	public class TerminableCompositeAction : CompositeAction, IRequireTermination
	{
		public TerminableCompositeAction()
		{
		}

		public void Terminate(IAIContext context)
		{
			int count = this._actions.Count;
			for (int i = 0; i < count; i++)
			{
				IRequireTermination item = this._actions[i] as IRequireTermination;
				if (item != null)
				{
					item.Terminate(context);
				}
			}
		}
	}
}