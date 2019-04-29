using Apex.Ai.HTN;
using Apex.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	public abstract class CompositeQualifier : IQualifier, ICanBeDisabled, ICompositeScorer, ICanClone
	{
		[ApexSerialization]
		[MemberCategory(null, 10000)]
		protected List<IContextualScorer> _scorers;

		[ApexSerialization(hideInEditor=true)]
		public IAction action
		{
			get;
			set;
		}

		[ApexSerialization(hideInEditor=true, defaultValue=false)]
		public bool isDisabled
		{
			get;
			set;
		}

		public IList<IContextualScorer> scorers
		{
			get
			{
				return this._scorers;
			}
		}

		protected CompositeQualifier()
		{
			this._scorers = new List<IContextualScorer>();
		}

		public virtual void CloneFrom(object other)
		{
			CompositeQualifier compositeQualifier = other as CompositeQualifier;
			if (compositeQualifier == null)
			{
				return;
			}
			this.action = compositeQualifier.action;
			foreach (IContextualScorer _scorer in compositeQualifier._scorers)
			{
				this._scorers.Add(_scorer);
			}
		}

		public float Score(IAIContext context)
		{
			return this.Score(context, this._scorers);
		}

		public abstract float Score(IAIContext context, IList<IContextualScorer> scorers);

		public virtual bool Validate(IHTNContext context, IList<IContextualScorer> scorers)
		{
			return true;
		}
	}
}