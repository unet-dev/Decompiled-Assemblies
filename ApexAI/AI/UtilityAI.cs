using Apex.Serialization;
using Apex.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	public sealed class UtilityAI : IUtilityAI, ISelect, IPrepareForSerialization, IInitializeAfterDeserialization
	{
		[ApexSerialization(hideInEditor=true)]
		private Guid _rootSelectorId;

		[ApexSerialization(hideInEditor=true)]
		private Guid _id;

		[ApexSerialization(hideInEditor=true)]
		private List<Selector> _selectors;

		private Selector _rootSelector;

		public Guid id
		{
			get
			{
				return this._id;
			}
		}

		public Selector this[int idx]
		{
			get
			{
				return this._selectors[idx];
			}
		}

		public string name
		{
			get;
			set;
		}

		public Selector rootSelector
		{
			get
			{
				return this._rootSelector;
			}
			set
			{
				this._rootSelector = value;
			}
		}

		public int selectorCount
		{
			get
			{
				return this._selectors.Count;
			}
		}

		public UtilityAI()
		{
			this._id = Guid.NewGuid();
			this._selectors = new List<Selector>(1);
		}

		public void AddSelector(Selector s)
		{
			Ensure.ArgumentNotNull(s, "selector cannot be null");
			this._selectors.Add(s);
		}

		void Apex.Serialization.IInitializeAfterDeserialization.Initialize(object rootObject)
		{
			this._rootSelector = this.FindSelector(this._rootSelectorId);
		}

		void Apex.Serialization.IPrepareForSerialization.Prepare()
		{
			this._rootSelectorId = this._rootSelector.id;
		}

		public Selector FindSelector(Guid id)
		{
			int count = this._selectors.Count;
			for (int i = 0; i < count; i++)
			{
				if (this._selectors[i].id.Equals(id))
				{
					return this._selectors[i];
				}
			}
			return null;
		}

		public void RegenerateIds()
		{
			this._id = Guid.NewGuid();
			int count = this._selectors.Count;
			for (int i = 0; i < count; i++)
			{
				this._selectors[i].RegenerateId();
			}
		}

		public void RemoveSelector(Selector s)
		{
			if (s == this._rootSelector)
			{
				throw new ArgumentException("The root selector cannot be removed!");
			}
			this._selectors.Remove(s);
			int count = this._selectors.Count;
			for (int i = 0; i < count; i++)
			{
				List<IQualifier> item = this._selectors[i].qualifiers;
				int num = item.Count;
				for (int j = 0; j < num; j++)
				{
					SelectorAction selectorAction = item[j].action as SelectorAction;
					if (selectorAction == null)
					{
						CompositeAction compositeAction = item[j].action as CompositeAction;
						if (compositeAction != null)
						{
							selectorAction = compositeAction.connectorAction as SelectorAction;
						}
					}
					if (selectorAction != null && selectorAction.selector == s)
					{
						item[j].action = null;
					}
				}
			}
		}

		public bool ReplaceSelector(Selector current, Selector replacement)
		{
			Ensure.ArgumentNotNull(current, "current cannot be null");
			Ensure.ArgumentNotNull(replacement, "replacement cannot be null");
			int num = this._selectors.IndexOf(current);
			if (num < 0)
			{
				return false;
			}
			this._selectors[num] = replacement;
			if (this._rootSelector == current)
			{
				this._rootSelector = replacement;
			}
			return true;
		}

		public IAction Select(IAIContext context)
		{
			return this._rootSelector.Select(context);
		}
	}
}