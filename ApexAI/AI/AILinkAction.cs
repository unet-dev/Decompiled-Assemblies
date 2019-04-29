using Apex.Serialization;
using System;
using UnityEngine;

namespace Apex.AI
{
	[FriendlyName("AI Link")]
	[Hidden]
	public sealed class AILinkAction : IConnectorAction, IAction, IInitializeAfterDeserialization
	{
		[ApexSerialization(hideInEditor=true)]
		private Guid _aiId;

		private ISelect _linkedAI;

		public Guid aiId
		{
			get
			{
				return this._aiId;
			}
			set
			{
				this._aiId = value;
			}
		}

		internal IUtilityAI linkedAI
		{
			get
			{
				return this._linkedAI as IUtilityAI;
			}
		}

		internal AILinkAction()
		{
		}

		public AILinkAction(Guid aiId)
		{
			this._aiId = aiId;
		}

		void Apex.Serialization.IInitializeAfterDeserialization.Initialize(object rootObject)
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this._linkedAI = AIManager.GetAI(this._aiId);
			if (this._linkedAI == null)
			{
				this._linkedAI = new AILinkAction.BrokenLink();
				Debug.LogWarning(string.Format("{0} : Failed to initialize a linked AI, the ID does not match an existing AI.", ((IUtilityAI)rootObject).name));
			}
		}

		public void Execute(IAIContext context)
		{
		}

		public IAction Select(IAIContext context)
		{
			return this._linkedAI.Select(context);
		}

		private class BrokenLink : ISelect
		{
			public Guid id
			{
				get
				{
					return Guid.Empty;
				}
			}

			public BrokenLink()
			{
			}

			public IAction Select(IAIContext context)
			{
				return null;
			}
		}
	}
}