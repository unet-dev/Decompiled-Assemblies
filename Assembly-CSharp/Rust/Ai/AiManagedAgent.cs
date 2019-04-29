using Rust;
using System;
using UnityEngine;

namespace Rust.Ai
{
	[DefaultExecutionOrder(-102)]
	public class AiManagedAgent : FacepunchBehaviour, IServerComponent
	{
		[Tooltip("TODO: Replace with actual agent type id on the NavMeshAgent when we upgrade to 5.6.1 or above.")]
		public int AgentTypeIndex;

		[ReadOnly]
		public Vector2i NavmeshGridCoord;

		private IAIAgent agent;

		private bool isRegistered;

		public AiManagedAgent()
		{
		}

		private void DelayedRegistration()
		{
			if (!this.isRegistered)
			{
				SingletonComponent<AiManager>.Instance.Add(this.agent);
				this.isRegistered = true;
			}
		}

		private void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || this.agent == null || this.agent.Entity == null || this.agent.Entity.isClient || !this.isRegistered)
			{
				return;
			}
			SingletonComponent<AiManager>.Instance.Remove(this.agent);
		}

		private void OnEnable()
		{
			this.isRegistered = false;
			if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || AiManager.nav_disable)
			{
				base.enabled = false;
				return;
			}
			this.agent = base.GetComponent<IAIAgent>();
			if (this.agent != null)
			{
				if (this.agent.Entity.isClient)
				{
					base.enabled = false;
					return;
				}
				this.agent.AgentTypeIndex = this.AgentTypeIndex;
				float single = SeedRandom.Value((uint)Mathf.Abs(base.GetInstanceID()));
				base.Invoke(new Action(this.DelayedRegistration), single * 3f);
			}
		}
	}
}