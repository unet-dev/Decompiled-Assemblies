using Apex.AI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai
{
	public class BaseContext : IAIContext
	{
		public Rust.Ai.Memory Memory;

		public BasePlayer ClosestPlayer;

		public List<BasePlayer> Players = new List<BasePlayer>();

		public List<BaseNpc> Npcs = new List<BaseNpc>();

		public List<BasePlayer> PlayersBehindUs = new List<BasePlayer>();

		public List<BaseNpc> NpcsBehindUs = new List<BaseNpc>();

		public List<TimedExplosive> DeployedExplosives = new List<TimedExplosive>(1);

		public BasePlayer EnemyPlayer;

		public BaseNpc EnemyNpc;

		public float LastTargetScore;

		public float LastEnemyPlayerScore;

		public float LastEnemyNpcScore;

		public float NextRoamTime;

		public IAIAgent AIAgent
		{
			get;
			private set;
		}

		public Vector3 EnemyPosition
		{
			get
			{
				if (this.EnemyPlayer != null)
				{
					return this.EnemyPlayer.ServerPosition;
				}
				if (this.EnemyNpc == null)
				{
					return Vector3.zero;
				}
				return this.EnemyNpc.ServerPosition;
			}
		}

		public BaseCombatEntity Entity
		{
			get;
			private set;
		}

		public Vector3 lastSampledPosition
		{
			get;
			set;
		}

		public Vector3 Position
		{
			get
			{
				if (this.Entity.IsDestroyed || this.Entity.transform == null)
				{
					return Vector3.zero;
				}
				return this.Entity.ServerPosition;
			}
		}

		public List<Vector3> sampledPositions
		{
			get;
			private set;
		}

		public BaseContext(IAIAgent agent)
		{
			this.AIAgent = agent;
			this.Entity = agent.Entity;
			this.sampledPositions = new List<Vector3>();
			this.Memory = new Rust.Ai.Memory();
		}

		public byte GetFact(BaseNpc.Facts fact)
		{
			return this.AIAgent.GetFact(fact);
		}

		public byte GetFact(NPCPlayerApex.Facts fact)
		{
			return this.AIAgent.GetFact(fact);
		}

		public void SetFact(BaseNpc.Facts fact, byte value)
		{
			this.AIAgent.SetFact(fact, value, true, true);
		}

		public void SetFact(NPCPlayerApex.Facts fact, byte value, bool triggerCallback = true, bool onlyTriggerCallbackOnDiffValue = true)
		{
			this.AIAgent.SetFact(fact, value, triggerCallback, onlyTriggerCallbackOnDiffValue);
		}
	}
}