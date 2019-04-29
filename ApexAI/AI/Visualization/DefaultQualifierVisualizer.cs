using Apex.AI;
using System;

namespace Apex.AI.Visualization
{
	internal sealed class DefaultQualifierVisualizer : QualifierVisualizer, IDefaultQualifier, IQualifier, ICanBeDisabled
	{
		private IDefaultQualifier _defQualifier;

		public float score
		{
			get
			{
				base.lastScore = new float?(this._defQualifier.score);
				return this._defQualifier.score;
			}
			set
			{
				this._defQualifier.score = value;
			}
		}

		internal DefaultQualifierVisualizer(IDefaultQualifier q, SelectorVisualizer parent) : base(q, parent)
		{
			this._defQualifier = q;
		}
	}
}