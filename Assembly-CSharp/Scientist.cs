using Network;
using Rust.Ai;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scientist : NPCPlayerApex
{
	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	private readonly static HashSet<Scientist> AllScientists;

	private readonly static List<Scientist> CommQueryCache;

	private readonly static List<AiAnswer_ShareEnemyTarget> CommTargetCache;

	public override BaseNpc.AiStatistics.FamilyEnum Family
	{
		get
		{
			return BaseNpc.AiStatistics.FamilyEnum.Scientist;
		}
	}

	static Scientist()
	{
		Scientist.AllScientists = new HashSet<Scientist>();
		Scientist.CommQueryCache = new List<Scientist>();
		Scientist.CommTargetCache = new List<AiAnswer_ShareEnemyTarget>(10);
	}

	public Scientist()
	{
	}

	public override int AskQuestion(AiQuestion_ShareEnemyTarget question, out List<AiAnswer_ShareEnemyTarget> answers)
	{
		List<Scientist> scientists;
		Scientist.CommTargetCache.Clear();
		if (this.GetAlliesInRange(out scientists) > 0)
		{
			foreach (Scientist scientist in scientists)
			{
				AiAnswer_ShareEnemyTarget aiAnswerShareEnemyTarget = scientist.OnAiQuestion(this, question);
				if (aiAnswerShareEnemyTarget.PlayerTarget == null)
				{
					continue;
				}
				Scientist.CommTargetCache.Add(aiAnswerShareEnemyTarget);
			}
		}
		answers = Scientist.CommTargetCache;
		return Scientist.CommTargetCache.Count;
	}

	public override string Categorize()
	{
		return "scientist";
	}

	public override BaseCorpse CreateCorpse()
	{
		int i;
		BaseCorpse baseCorpse;
		using (TimeWarning timeWarning = TimeWarning.New("Create corpse", 0.1f))
		{
			NPCPlayerCorpse navAgent = base.DropCorpse("assets/prefabs/npc/scientist/scientist_corpse.prefab") as NPCPlayerCorpse;
			if (navAgent)
			{
				navAgent.transform.position = navAgent.transform.position + (Vector3.down * this.NavAgent.baseOffset);
				navAgent.SetLootableIn(2f);
				navAgent.SetFlag(BaseEntity.Flags.Reserved5, base.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
				navAgent.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
				navAgent.TakeFrom(new ItemContainer[] { this.inventory.containerMain, this.inventory.containerWear, this.inventory.containerBelt });
				navAgent.playerName = base.displayName;
				navAgent.playerSteamID = this.userID;
				navAgent.Spawn();
				navAgent.TakeChildren(this);
				ItemContainer[] itemContainerArray = navAgent.containers;
				for (i = 0; i < (int)itemContainerArray.Length; i++)
				{
					itemContainerArray[i].Clear();
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					LootContainer.LootSpawnSlot[] lootSpawnSlots = this.LootSpawnSlots;
					for (i = 0; i < (int)lootSpawnSlots.Length; i++)
					{
						LootContainer.LootSpawnSlot lootSpawnSlot = lootSpawnSlots[i];
						for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
						{
							if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
							{
								lootSpawnSlot.definition.SpawnIntoContainer(navAgent.containers[0]);
							}
						}
					}
				}
			}
			baseCorpse = navAgent;
		}
		return baseCorpse;
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		Scientist.AllScientists.Remove(this);
		this.OnDestroyComm();
	}

	public override int GetAlliesInRange(out List<Scientist> allies)
	{
		Scientist.CommQueryCache.Clear();
		foreach (Scientist allScientist in Scientist.AllScientists)
		{
			if (allScientist == this || !base.IsInCommunicationRange(allScientist))
			{
				continue;
			}
			Scientist.CommQueryCache.Add(allScientist);
		}
		allies = Scientist.CommQueryCache;
		return Scientist.CommQueryCache.Count;
	}

	private void InitComm()
	{
		base.OnAggro = (NPCPlayerApex.ActionCallback)Delegate.Combine(base.OnAggro, new NPCPlayerApex.ActionCallback(this.OnAggroComm));
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this._displayName = string.Format("Scientist {0}", (this.net != null ? (int)this.net.ID : "scientist".GetHashCode()));
	}

	public override float MaxHealth()
	{
		return this.startHealth;
	}

	private void OnAggroComm()
	{
		AiStatement_EnemyEngaged aiStatementEnemyEngaged = new AiStatement_EnemyEngaged()
		{
			Enemy = base.AiContext.EnemyPlayer,
			Score = base.AiContext.LastTargetScore
		};
		AiStatement_EnemyEngaged nullable = aiStatementEnemyEngaged;
		if (base.AiContext.EnemyPlayer != null)
		{
			Memory.SeenInfo info = base.AiContext.Memory.GetInfo(base.AiContext.EnemyPlayer);
			if (!(info.Entity != null) || info.Entity.IsDestroyed || base.AiContext.EnemyPlayer.IsDead())
			{
				nullable.Enemy = null;
			}
			else
			{
				nullable.LastKnownPosition = new Vector3?(info.Position);
			}
		}
		this.SendStatement(nullable);
	}

	public override void OnAiStatement(NPCPlayerApex source, AiStatement_EnemyEngaged statement)
	{
		Memory.ExtendedInfo extendedInfo;
		if (statement.Enemy != null && statement.LastKnownPosition.HasValue && base.HostilityConsideration(statement.Enemy) && (base.AiContext.EnemyPlayer == null || base.AiContext.EnemyPlayer == statement.Enemy))
		{
			if (source.GetFact(NPCPlayerApex.Facts.AttackedRecently) > 0)
			{
				base.SetFact(NPCPlayerApex.Facts.AllyAttackedRecently, 1, true, true);
				this.AllyAttackedRecentlyTimeout = Time.realtimeSinceStartup + 7f;
			}
			if (base.GetFact(NPCPlayerApex.Facts.IsBandit) > 0)
			{
				base.AiContext.LastAttacker = statement.Enemy;
				this.lastAttackedTime = source.lastAttackedTime;
			}
			base.UpdateTargetMemory(statement.Enemy, 0.1f, statement.LastKnownPosition.Value, out extendedInfo);
		}
	}

	public override void OnAiStatement(NPCPlayerApex source, AiStatement_EnemySeen statement)
	{
	}

	private void OnDestroyComm()
	{
		base.OnAggro = (NPCPlayerApex.ActionCallback)Delegate.Remove(base.OnAggro, new NPCPlayerApex.ActionCallback(this.OnAggroComm));
	}

	public override void SendStatement(AiStatement_EnemyEngaged statement)
	{
		foreach (Scientist allScientist in Scientist.AllScientists)
		{
			if (allScientist == this || !base.IsInCommunicationRange(allScientist))
			{
				continue;
			}
			allScientist.OnAiStatement(this, statement);
		}
	}

	public override void SendStatement(AiStatement_EnemySeen statement)
	{
		foreach (Scientist allScientist in Scientist.AllScientists)
		{
			if (allScientist == this || !base.IsInCommunicationRange(allScientist))
			{
				continue;
			}
			allScientist.OnAiStatement(this, statement);
		}
	}

	public override void ServerInit()
	{
		if (base.isClient)
		{
			return;
		}
		base.ServerInit();
		Scientist.AllScientists.Add(this);
		this.InitComm();
	}

	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	public override float StartHealth()
	{
		return UnityEngine.Random.Range(this.startHealth, this.startHealth);
	}

	public override float StartMaxHealth()
	{
		return this.startHealth;
	}
}