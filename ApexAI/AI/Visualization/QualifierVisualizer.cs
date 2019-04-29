using Apex.AI;
using System;
using System.Runtime.CompilerServices;

namespace Apex.AI.Visualization
{
	internal class QualifierVisualizer : IQualifierVisualizer, IQualifier, ICanBeDisabled, IVisualizedObject
	{
		private IQualifier _qualifier;

		private ActionVisualizer _action;

		private SelectorVisualizer _parent;

		public IAction action
		{
			get
			{
				return this._action;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		object Apex.AI.Visualization.IVisualizedObject.target
		{
			get
			{
				return this._qualifier;
			}
		}

		public BreakpointCondition breakpointCondition
		{
			get;
			set;
		}

		public bool breakPointHit
		{
			get;
			set;
		}

		public bool isBreakPoint
		{
			get;
			set;
		}

		public bool isDisabled
		{
			get
			{
				return this._qualifier.isDisabled;
			}
			set
			{
				this._qualifier.isDisabled = value;
			}
		}

		public bool isHighScorer
		{
			get
			{
				return this._parent.lastSelectedQualifier == this;
			}
		}

		public float? lastScore
		{
			get
			{
				return JustDecompileGenerated_get_lastScore();
			}
			set
			{
				JustDecompileGenerated_set_lastScore(value);
			}
		}

		private float? JustDecompileGenerated_lastScore_k__BackingField;

		public float? JustDecompileGenerated_get_lastScore()
		{
			return this.JustDecompileGenerated_lastScore_k__BackingField;
		}

		protected void JustDecompileGenerated_set_lastScore(float? value)
		{
			this.JustDecompileGenerated_lastScore_k__BackingField = value;
		}

		public SelectorVisualizer parent
		{
			get
			{
				return this._parent;
			}
		}

		public IQualifier qualifier
		{
			get
			{
				return this._qualifier;
			}
		}

		internal QualifierVisualizer(IQualifier q, SelectorVisualizer parent)
		{
			this._qualifier = q;
			this._parent = parent;
			SelectorAction selectorAction = q.action as SelectorAction;
			AILinkAction aILinkAction = q.action as AILinkAction;
			CompositeAction compositeAction = q.action as CompositeAction;
			IRequireTermination requireTermination = q.action as IRequireTermination;
			if (selectorAction != null)
			{
				this._action = new SelectorActionVisualizer(selectorAction, this);
				return;
			}
			if (aILinkAction != null)
			{
				this._action = new AILinkActionVisualizer(aILinkAction, this);
				return;
			}
			if (compositeAction != null)
			{
				this._action = new CompositeActionVisualizer(compositeAction, this);
				return;
			}
			if (requireTermination != null)
			{
				this._action = new ActionRequiresTerminationVisualizer(q.action, this);
				return;
			}
			if (q.action != null)
			{
				this._action = new ActionVisualizer(q.action, this);
			}
		}

		void Apex.AI.Visualization.IQualifierVisualizer.Init()
		{
			if (this._action != null)
			{
				this._action.Init();
			}
		}

		public virtual void Reset()
		{
			this.lastScore = null;
			this.breakPointHit = false;
		}

		public virtual float Score(IAIContext context)
		{
			ICustomVisualizer customVisualizer;
			float single = this._qualifier.Score(context);
			this.lastScore = new float?(single);
			if (VisualizationManager.TryGetVisualizerFor(this._qualifier.GetType(), out customVisualizer))
			{
				customVisualizer.EntityUpdate(this._qualifier, context, this._parent.parent.id);
			}
			return single;
		}
	}
}