using Apex.AI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Apex.AI.Visualization
{
	internal sealed class SelectorVisualizer : Selector, IVisualizedObject
	{
		private Selector _selector;

		private UtilityAIVisualizer _parent;

		object Apex.AI.Visualization.IVisualizedObject.target
		{
			get
			{
				return this._selector;
			}
		}

		internal new Guid id
		{
			get
			{
				return this._selector.id;
			}
		}

		internal IQualifier lastSelectedQualifier
		{
			get;
			private set;
		}

		internal UtilityAIVisualizer parent
		{
			get
			{
				return this._parent;
			}
		}

		internal Selector selector
		{
			get
			{
				return this._selector;
			}
		}

		internal SelectorVisualizer(Selector s, UtilityAIVisualizer parent)
		{
			this._selector = s;
			this._parent = parent;
			List<IQualifier> qualifiers = this._selector.qualifiers;
			int count = qualifiers.Count;
			for (int i = 0; i < count; i++)
			{
				IQualifier item = qualifiers[i];
				if (!(item is ICompositeScorer))
				{
					base.qualifiers.Add(new QualifierVisualizer(item, this));
				}
				else
				{
					base.qualifiers.Add(new CompositeQualifierVisualizer((ICompositeScorer)item, this));
				}
			}
			base.defaultQualifier = new DefaultQualifierVisualizer(this._selector.defaultQualifier, this);
		}

		internal void ClearBreakpoints()
		{
			int count = base.qualifiers.Count;
			for (int i = 0; i < count; i++)
			{
				((IQualifierVisualizer)base.qualifiers[i]).isBreakPoint = false;
			}
		}

		internal void Init()
		{
			int count = base.qualifiers.Count;
			for (int i = 0; i < count; i++)
			{
				((IQualifierVisualizer)base.qualifiers[i]).Init();
			}
			((IQualifierVisualizer)base.defaultQualifier).Init();
		}

		internal void Reset()
		{
			this.lastSelectedQualifier = null;
			int count = base.qualifiers.Count;
			for (int i = 0; i < count; i++)
			{
				((IQualifierVisualizer)base.qualifiers[i]).Reset();
			}
			((IQualifierVisualizer)base.defaultQualifier).Reset();
		}

		public override IQualifier Select(IAIContext context, IList<IQualifier> qualifiers, IDefaultQualifier defaultQualifier)
		{
			this.lastSelectedQualifier = this._selector.Select(context, qualifiers, defaultQualifier);
			IQualifierVisualizer qualifierVisualizer = this.lastSelectedQualifier as IQualifierVisualizer;
			if (qualifierVisualizer != null && qualifierVisualizer.isBreakPoint)
			{
				if (qualifierVisualizer.breakpointCondition == null)
				{
					qualifierVisualizer.breakPointHit = true;
				}
				else
				{
					qualifierVisualizer.breakPointHit = qualifierVisualizer.breakpointCondition.Evaluate(qualifierVisualizer.lastScore);
				}
			}
			return this.lastSelectedQualifier;
		}
	}
}