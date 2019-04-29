using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Apex.AI
{
	public abstract class Selector : ISelect, ICanClone
	{
		[ApexSerialization(hideInEditor=true)]
		protected Guid _id;

		[ApexSerialization(hideInEditor=true)]
		protected List<IQualifier> _qualifiers;

		[ApexSerialization(hideInEditor=true)]
		protected IDefaultQualifier _defaultQualifier;

		public IDefaultQualifier defaultQualifier
		{
			get
			{
				return this._defaultQualifier;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Value cannot be null.", "defaultQualifier");
				}
				this._defaultQualifier = value;
			}
		}

		public Guid id
		{
			get
			{
				return this._id;
			}
		}

		public List<IQualifier> qualifiers
		{
			get
			{
				return this._qualifiers;
			}
		}

		public Selector()
		{
			this._id = Guid.NewGuid();
			this._qualifiers = new List<IQualifier>();
			this._defaultQualifier = new DefaultQualifier();
		}

		public virtual void CloneFrom(object other)
		{
			Selector selector = other as Selector;
			if (selector == null)
			{
				return;
			}
			for (int i = 0; i < selector.qualifiers.Count; i++)
			{
				this._qualifiers.Add(selector.qualifiers[i]);
			}
			this._id = selector.id;
			this._defaultQualifier = selector._defaultQualifier;
		}

		public void RegenerateId()
		{
			this._id = Guid.NewGuid();
		}

		public IAction Select(IAIContext context)
		{
			IQualifier qualifier = this.Select(context, this._qualifiers, this._defaultQualifier);
			if (qualifier == null)
			{
				return null;
			}
			return qualifier.action;
		}

		public abstract IQualifier Select(IAIContext context, IList<IQualifier> qualifiers, IDefaultQualifier defaultQualifier);
	}
}