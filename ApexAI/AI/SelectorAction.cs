using Apex.Serialization;
using Apex.Utilities;
using System;

namespace Apex.AI
{
	[FriendlyName("Continue")]
	[Hidden]
	public class SelectorAction : IConnectorAction, IAction, IPrepareForSerialization, IInitializeAfterDeserialization
	{
		[ApexSerialization(hideInEditor=true)]
		private Guid _selectorId;

		private Selector _selector;

		public Selector selector
		{
			get
			{
				return this._selector;
			}
			set
			{
				this._selector = value;
			}
		}

		internal SelectorAction()
		{
		}

		public SelectorAction(Selector selector)
		{
			Ensure.ArgumentNotNull(selector, "selector");
			this._selector = selector;
		}

		void Apex.Serialization.IInitializeAfterDeserialization.Initialize(object rootObject)
		{
			this._selector = ((IUtilityAI)rootObject).FindSelector(this._selectorId);
		}

		void Apex.Serialization.IPrepareForSerialization.Prepare()
		{
			if (this._selector != null)
			{
				this._selectorId = this._selector.id;
			}
		}

		public void Execute(IAIContext context)
		{
		}

		public IAction Select(IAIContext context)
		{
			return this._selector.Select(context);
		}
	}
}