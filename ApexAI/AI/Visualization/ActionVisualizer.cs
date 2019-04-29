using Apex.AI;
using System;

namespace Apex.AI.Visualization
{
	internal class ActionVisualizer : IAction, IVisualizedObject
	{
		private IAction _action;

		private IQualifierVisualizer _parent;

		internal IAction action
		{
			get
			{
				return this._action;
			}
		}

		object Apex.AI.Visualization.IVisualizedObject.target
		{
			get
			{
				return this._action;
			}
		}

		internal IQualifierVisualizer parent
		{
			get
			{
				return this._parent;
			}
		}

		internal ActionVisualizer(IAction action, IQualifierVisualizer parent)
		{
			this._action = action;
			this._parent = parent;
		}

		internal virtual void Execute(IAIContext context, bool doCallback)
		{
			ICustomVisualizer customVisualizer;
			this._action.Execute(context);
			if (doCallback)
			{
				this._parent.parent.parent.PostExecute();
			}
			if (VisualizationManager.TryGetVisualizerFor(this._action.GetType(), out customVisualizer))
			{
				customVisualizer.EntityUpdate(this._action, context, this._parent.parent.parent.id);
			}
		}

		public void Execute(IAIContext context)
		{
			this.Execute(context, true);
		}

		internal virtual void Init()
		{
		}
	}
}