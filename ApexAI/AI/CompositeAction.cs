using Apex;
using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Apex.AI
{
	[FriendlyName("Composite Action", "An action comprised of one or more child actions, which are executed in order")]
	public class CompositeAction : ICompositeAction, IConnectorAction, IAction, ICanClone
	{
		[ApexSerialization]
		[MemberCategory(null, 10000)]
		protected List<IAction> _actions = new List<IAction>(2);

		[ApexSerialization(defaultValue=null, hideInEditor=true)]
		private IConnectorAction _connectorAction;

		public IList<IAction> actions
		{
			get
			{
				return this._actions;
			}
		}

		bool Apex.AI.ICompositeAction.isConnector
		{
			get
			{
				return this._connectorAction != null;
			}
		}

		internal IConnectorAction connectorAction
		{
			get
			{
				return this._connectorAction;
			}
			set
			{
				this._connectorAction = value;
			}
		}

		public CompositeAction()
		{
		}

		public void CloneFrom(object other)
		{
			IAction action = other as IAction;
			if (action == null)
			{
				return;
			}
			CompositeAction compositeAction = other as CompositeAction;
			if (compositeAction != null)
			{
				this.actions.AddRange<IAction>(compositeAction.actions);
				this.connectorAction = compositeAction.connectorAction;
				return;
			}
			if (action is IConnectorAction)
			{
				this.connectorAction = (IConnectorAction)action;
				return;
			}
			this.actions.Add(action);
		}

		public void Execute(IAIContext context)
		{
			int count = this._actions.Count;
			for (int i = 0; i < count; i++)
			{
				this._actions[i].Execute(context);
			}
		}

		public IAction Select(IAIContext context)
		{
			if (this._connectorAction == null)
			{
				return null;
			}
			return this._connectorAction.Select(context);
		}
	}
}