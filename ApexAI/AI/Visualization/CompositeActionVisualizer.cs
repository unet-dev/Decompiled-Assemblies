using Apex.AI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Apex.AI.Visualization
{
	internal class CompositeActionVisualizer : ConnectorActionVisualizer, ICompositeVisualizer, ICompositeAction, IConnectorAction, IAction, IRequireTermination
	{
		private List<ActionVisualizer> _actions;

		private ConnectorActionVisualizer _connectorAction;

		bool Apex.AI.ICompositeAction.isConnector
		{
			get
			{
				return this._connectorAction != null;
			}
		}

		IList Apex.AI.Visualization.ICompositeVisualizer.children
		{
			get
			{
				return this._actions;
			}
		}

		internal CompositeActionVisualizer(CompositeAction action, IQualifierVisualizer parent) : base(action, parent)
		{
			this._actions = new List<ActionVisualizer>(action.actions.Count);
			for (int i = 0; i < action.actions.Count; i++)
			{
				IAction item = action.actions[i];
				CompositeAction compositeAction = item as CompositeAction;
				if (compositeAction == null)
				{
					this._actions.Add(new ActionVisualizer(item, parent));
				}
				else
				{
					this._actions.Add(new CompositeActionVisualizer(compositeAction, parent));
				}
			}
			SelectorAction selectorAction = action.connectorAction as SelectorAction;
			AILinkAction aILinkAction = action.connectorAction as AILinkAction;
			if (selectorAction != null)
			{
				this._connectorAction = new SelectorActionVisualizer(selectorAction, parent);
				return;
			}
			if (aILinkAction != null)
			{
				this._connectorAction = new AILinkActionVisualizer(aILinkAction, parent);
			}
		}

		void Apex.AI.Visualization.ICompositeVisualizer.Add(object item)
		{
			IAction action = item as IAction;
			CompositeAction compositeAction = item as CompositeAction;
			if (compositeAction != null)
			{
				this._actions.Add(new CompositeActionVisualizer(compositeAction, base.parent));
				return;
			}
			if (action != null)
			{
				this._actions.Add(new ActionVisualizer(action, base.parent));
			}
		}

		internal override void Execute(IAIContext context, bool doCallback)
		{
			int count = this._actions.Count;
			for (int i = 0; i < count; i++)
			{
				this._actions[i].Execute(context, false);
			}
			if (doCallback)
			{
				base.parent.parent.parent.PostExecute();
			}
		}

		internal override void Init()
		{
			if (this._connectorAction != null)
			{
				this._connectorAction.Init();
			}
		}

		public override IAction Select(IAIContext context)
		{
			if (this._connectorAction == null)
			{
				return null;
			}
			return this._connectorAction.Select(context);
		}

		public void Terminate(IAIContext context)
		{
			IRequireTermination requireTermination = base.action as IRequireTermination;
			if (requireTermination != null)
			{
				requireTermination.Terminate(context);
			}
		}
	}
}