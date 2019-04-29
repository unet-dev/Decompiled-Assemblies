using Apex.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Apex.AI.Visualization
{
	internal sealed class UtilityAIVisualizer : IUtilityAI, ISelect
	{
		private IUtilityAI _ai;

		private List<Action> _postExecute;

		private SelectorVisualizer _visualizerRootSelector;

		private List<SelectorVisualizer> _selectorVisualizers;

		private List<UtilityAIVisualizer> _linkedAIs = new List<UtilityAIVisualizer>();

		public IUtilityAI ai
		{
			get
			{
				return this._ai;
			}
		}

		public Guid id
		{
			get
			{
				return this._ai.id;
			}
		}

		public Selector this[int idx]
		{
			get
			{
				return this._selectorVisualizers[idx];
			}
		}

		internal List<UtilityAIVisualizer> linkedAIs
		{
			get
			{
				return this._linkedAIs;
			}
		}

		public string name
		{
			get
			{
				return this._ai.name;
			}
			set
			{
				this._ai.name = value;
			}
		}

		public Selector rootSelector
		{
			get
			{
				return this._visualizerRootSelector;
			}
			set
			{
				throw new NotSupportedException("Cannot edit a Visualizer.");
			}
		}

		public int selectorCount
		{
			get
			{
				return this._selectorVisualizers.Count;
			}
		}

		internal UtilityAIVisualizer(IUtilityAI ai)
		{
			this._ai = ai;
			int num = ai.selectorCount;
			this._selectorVisualizers = new List<SelectorVisualizer>(ai.selectorCount);
			for (int i = 0; i < num; i++)
			{
				Selector item = this._ai[i];
				SelectorVisualizer selectorVisualizer = new SelectorVisualizer(item, this);
				this._selectorVisualizers.Add(selectorVisualizer);
				if (item == this._ai.rootSelector)
				{
					this._visualizerRootSelector = selectorVisualizer;
				}
			}
			for (int j = 0; j < num; j++)
			{
				this._selectorVisualizers[j].Init();
			}
			this._postExecute = new List<Action>(1);
		}

		void Apex.AI.IUtilityAI.AddSelector(Selector s)
		{
			throw new NotSupportedException("Cannot alter AI during visualization.");
		}

		void Apex.AI.IUtilityAI.RegenerateIds()
		{
			throw new NotSupportedException("Cannot alter AI during visualization.");
		}

		void Apex.AI.IUtilityAI.RemoveSelector(Selector s)
		{
			throw new NotSupportedException("Cannot alter AI during visualization.");
		}

		bool Apex.AI.IUtilityAI.ReplaceSelector(Selector current, Selector replacement)
		{
			throw new NotSupportedException("Cannot alter AI during visualization.");
		}

		internal void ClearBreakpoints()
		{
			int count = this._selectorVisualizers.Count;
			for (int i = 0; i < count; i++)
			{
				this._selectorVisualizers[i].ClearBreakpoints();
			}
		}

		internal ActionVisualizer FindActionVisualizer(IAction target)
		{
			ActionVisualizer actionVisualizer = null;
			int count = this._selectorVisualizers.Count;
			for (int i = 0; i < count; i++)
			{
				SelectorVisualizer item = this._selectorVisualizers[i];
				int num = item.qualifiers.Count;
				for (int j = 0; j < num; j++)
				{
					if (UtilityAIVisualizer.TryFindActionVisualizer(((IQualifierVisualizer)item.qualifiers[j]).action, target, out actionVisualizer))
					{
						return actionVisualizer;
					}
				}
				if (UtilityAIVisualizer.TryFindActionVisualizer(item.defaultQualifier.action, target, out actionVisualizer))
				{
					return actionVisualizer;
				}
			}
			return null;
		}

		internal IQualifierVisualizer FindQualifierVisualizer(IQualifier target)
		{
			int count = this._selectorVisualizers.Count;
			for (int i = 0; i < count; i++)
			{
				SelectorVisualizer item = this._selectorVisualizers[i];
				int num = item.qualifiers.Count;
				for (int j = 0; j < num; j++)
				{
					IQualifierVisualizer qualifierVisualizer = (IQualifierVisualizer)item.qualifiers[j];
					if (qualifierVisualizer.qualifier == target)
					{
						return qualifierVisualizer;
					}
				}
			}
			return null;
		}

		public Selector FindSelector(Guid id)
		{
			int count = this._selectorVisualizers.Count;
			for (int i = 0; i < count; i++)
			{
				if (this._selectorVisualizers[i].id.Equals(id))
				{
					return this._selectorVisualizers[i];
				}
			}
			return null;
		}

		internal void Hook(Action postExecute)
		{
			if (!this._postExecute.Contains(postExecute))
			{
				this._postExecute.Add(postExecute);
			}
		}

		internal void PostExecute()
		{
			int count = this._postExecute.Count;
			for (int i = 0; i < count; i++)
			{
				this._postExecute[i]();
			}
		}

		internal void Reset()
		{
			int count = this._selectorVisualizers.Count;
			for (int i = 0; i < count; i++)
			{
				this._selectorVisualizers[i].Reset();
			}
			int num = this._linkedAIs.Count;
			for (int j = 0; j < num; j++)
			{
				this._linkedAIs[j].Reset();
				this._linkedAIs[j].PostExecute();
			}
		}

		public IAction Select(IAIContext context)
		{
			this.Reset();
			IAction action = this._visualizerRootSelector.Select(context);
			if (action == null)
			{
				this.PostExecute();
			}
			return action;
		}

		private static bool TryFindActionVisualizer(IAction source, IAction target, out ActionVisualizer result)
		{
			result = null;
			ActionVisualizer actionVisualizer = source as ActionVisualizer;
			if (actionVisualizer == null)
			{
				return false;
			}
			if (actionVisualizer.action == target)
			{
				result = actionVisualizer;
				return true;
			}
			ICompositeVisualizer compositeVisualizer = actionVisualizer as ICompositeVisualizer;
			if (compositeVisualizer == null)
			{
				return false;
			}
			int count = compositeVisualizer.children.Count;
			for (int i = 0; i < count; i++)
			{
				if (UtilityAIVisualizer.TryFindActionVisualizer(compositeVisualizer.children[i] as IAction, target, out result))
				{
					return true;
				}
			}
			return false;
		}

		internal void Unhook(Action postExecute)
		{
			this._postExecute.Remove(postExecute);
		}
	}
}