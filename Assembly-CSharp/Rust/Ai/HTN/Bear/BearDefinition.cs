using Rust.Ai.HTN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.Bear
{
	[CreateAssetMenu(menuName="Rust/AI/Animals/Bear Definition")]
	public class BearDefinition : BaseNpcDefinition
	{
		[Header("Sensory Extensions")]
		public float StandingAggroRange = 40f;

		[Header("Corpse")]
		public GameObjectRef CorpsePrefab;

		[Header("Equipment")]
		public LootContainer.LootSpawnSlot[] Loot;

		[Header("Audio")]
		public Vector2 IdleEffectRepeatRange = new Vector2(10f, 15f);

		public GameObjectRef IdleEffect;

		public GameObjectRef DeathEffect;

		private bool _isEffectRunning;

		public float SqrStandingAggroRange
		{
			get
			{
				return this.StandingAggroRange * this.StandingAggroRange;
			}
		}

		public BearDefinition()
		{
		}

		public float AggroRange(bool isStanding)
		{
			if (isStanding)
			{
				return this.StandingAggroRange;
			}
			return this.Engagement.AggroRange;
		}

		public override BaseCorpse OnCreateCorpse(HTNAnimal target)
		{
			BaseCorpse baseCorpse;
			if (this.DeathEffect.isValid)
			{
				Effect.server.Run(this.DeathEffect.resourcePath, target, 0, Vector3.zero, Vector3.zero, null, false);
			}
			using (TimeWarning timeWarning = TimeWarning.New("Create corpse", 0.1f))
			{
				BaseCorpse navAgent = target.DropCorpse(this.CorpsePrefab.resourcePath);
				if (navAgent)
				{
					if (target.AiDomain != null && target.AiDomain.NavAgent != null && target.AiDomain.NavAgent.isOnNavMesh)
					{
						navAgent.transform.position = navAgent.transform.position + (Vector3.down * target.AiDomain.NavAgent.baseOffset);
					}
					navAgent.InitCorpse(target);
					navAgent.Spawn();
					navAgent.TakeChildren(target);
				}
				baseCorpse = navAgent;
			}
			return baseCorpse;
		}

		private IEnumerator PlayEffects(HTNAnimal target)
		{
			BearDefinition bearDefinition = null;
			while (bearDefinition._isEffectRunning && target != null && target.transform != null && !target.IsDestroyed && !target.IsDead())
			{
				if (bearDefinition.IdleEffect.isValid)
				{
					Effect.server.Run(bearDefinition.IdleEffect.resourcePath, target, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
				}
				float single = UnityEngine.Random.Range(bearDefinition.IdleEffectRepeatRange.x, bearDefinition.IdleEffectRepeatRange.y + 1f);
				yield return CoroutineEx.waitForSeconds(single);
			}
		}

		public float SqrAggroRange(bool isStanding)
		{
			if (isStanding)
			{
				return this.SqrStandingAggroRange;
			}
			return this.Engagement.SqrAggroRange;
		}

		public override void StartVoices(HTNAnimal target)
		{
			if (this._isEffectRunning)
			{
				return;
			}
			this._isEffectRunning = true;
			target.StartCoroutine(this.PlayEffects(target));
		}

		public override void StopVoices(HTNAnimal target)
		{
			if (!this._isEffectRunning)
			{
				return;
			}
			this._isEffectRunning = false;
		}
	}
}