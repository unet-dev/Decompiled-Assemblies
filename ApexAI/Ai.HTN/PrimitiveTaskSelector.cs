using Apex.AI;
using Apex.Serialization;
using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Apex.Ai.HTN
{
	public class PrimitiveTaskSelector : Selector, ITask
	{
		[ApexSerialization]
		public string Name;

		[ApexSerialization]
		[FriendlyName("Preconditions", "Requirements of state that must be in place for this task to be valid")]
		private List<ICompositeScorer> _preconditions = new List<ICompositeScorer>(2);

		[ApexSerialization]
		[FriendlyName("Operators", "The operators this task will execute")]
		[MemberCategory(null, 10000)]
		private List<IOperator> _operators = new List<IOperator>(1);

		[ApexSerialization]
		[FriendlyName("Effects", "The impact running this task will have on the world")]
		[MemberCategory(null, 10000)]
		private List<IEffect> _effects = new List<IEffect>(2);

		[ApexSerialization]
		[FriendlyName("Expected Effects", "When an effect will not be applied immediately, but we expect it to sometime in the future")]
		[MemberCategory(null, 10001)]
		private List<IEffect> _expectedEffects = new List<IEffect>(2);

		public bool breakPointHit
		{
			get;
			set;
		}

		public List<IEffect> ExpectedEffects
		{
			get
			{
				return this._expectedEffects;
			}
		}

		public bool isBreakPoint
		{
			get;
			set;
		}

		public List<IOperator> Operators
		{
			get
			{
				return this._operators;
			}
		}

		public List<ITask> Parents { get; } = new List<ITask>();

		public List<ICompositeScorer> Preconditions
		{
			get
			{
				return this._preconditions;
			}
		}

		public Apex.Ai.HTN.PrimitiveTaskAction PrimitiveTaskAction
		{
			get
			{
				Apex.Ai.HTN.PrimitiveTaskAction name = base.defaultQualifier.action as Apex.Ai.HTN.PrimitiveTaskAction;
				if (name == null)
				{
					return name;
				}
				name.Name = this.Name;
				return name;
			}
		}

		public PrimitiveTaskStateType State
		{
			get;
			set;
		}

		public PrimitiveTaskSelector()
		{
		}

		public float Decompose(DomainSelector domain, ITask parent, IHTNContext context, ref List<PrimitiveTaskSelector> plan, ref int score, int scoreThreshold, out int localCost)
		{
			localCost = 0;
			if (this._id == Guid.Empty)
			{
				return 0f;
			}
			if (parent != null && !this.Parents.Contains(parent))
			{
				this.Parents.Clear();
				this.Parents.Add(parent);
			}
			plan.Add(this);
			Stack<IEffect> effects = Pool.Get<Stack<IEffect>>();
			Stack<IEffect> effects1 = Pool.Get<Stack<IEffect>>();
			for (int i = 0; i < this._effects.Count; i++)
			{
				IEffect item = this._effects[i];
				item.Apply(context, true, false);
				effects.Push(item);
			}
			for (int j = 0; j < this._expectedEffects.Count; j++)
			{
				IEffect effect = this._expectedEffects[j];
				effect.Apply(context, true, true);
				effects1.Push(effect);
			}
			if (effects.Count <= 0)
			{
				Pool.Free<Stack<IEffect>>(ref effects);
			}
			else if (!context.AppliedEffects.ContainsKey(base.id))
			{
				context.AppliedEffects.Add(base.id, effects);
			}
			else
			{
				Stack<IEffect> item1 = context.AppliedEffects[base.id];
				if (item1 != null)
				{
					Pool.Free<Stack<IEffect>>(ref item1);
				}
				context.AppliedEffects[base.id] = effects;
			}
			if (effects1.Count <= 0)
			{
				Pool.Free<Stack<IEffect>>(ref effects1);
			}
			else if (!context.AppliedExpectedEffects.ContainsKey(base.id))
			{
				context.AppliedExpectedEffects.Add(base.id, effects1);
			}
			else
			{
				Stack<IEffect> item2 = context.AppliedExpectedEffects[base.id];
				if (item2 != null)
				{
					Pool.Free<Stack<IEffect>>(ref item2);
				}
				context.AppliedExpectedEffects[base.id] = effects1;
			}
			return 1f;
		}

		public void GetFullDecompositionCost(ref int cost)
		{
		}

		public void RemoveAppliedEffects(IHTNContext context, ref List<PrimitiveTaskSelector> plan)
		{
			Stack<IEffect> effects;
			if (context.AppliedExpectedEffects.TryGetValue(base.id, out effects))
			{
				while (effects.Count > 0)
				{
					effects.Pop().Reverse(context, true);
				}
				Stack<IEffect> effects1 = effects;
				Pool.Free<Stack<IEffect>>(ref effects1);
				context.AppliedExpectedEffects.Remove(base.id);
			}
			if (context.AppliedEffects.TryGetValue(base.id, out effects))
			{
				while (effects.Count > 0)
				{
					effects.Pop().Reverse(context, true);
				}
				Stack<IEffect> effects2 = effects;
				Pool.Free<Stack<IEffect>>(ref effects2);
				context.AppliedEffects.Remove(base.id);
			}
			plan.Remove(this);
		}

		public void Reset()
		{
		}

		public override IQualifier Select(IAIContext context, IList<IQualifier> qualifiers, IDefaultQualifier defaultQualifier)
		{
			for (int i = 0; i < qualifiers.Count; i++)
			{
				qualifiers[i].Score(context);
			}
			defaultQualifier.Score(context);
			return defaultQualifier;
		}

		public bool ValidatePreconditions(IHTNContext context)
		{
			for (int i = 0; i < this.Preconditions.Count; i++)
			{
				ICompositeScorer item = this.Preconditions[i];
				if (!item.Validate(context, item.scorers))
				{
					return false;
				}
			}
			for (int j = 0; j < this.Parents.Count; j++)
			{
				if (!this.Parents[j].ValidatePreconditions(context))
				{
					return false;
				}
			}
			return true;
		}
	}
}