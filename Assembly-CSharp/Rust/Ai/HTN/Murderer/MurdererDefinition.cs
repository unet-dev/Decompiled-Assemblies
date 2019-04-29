using Rust.Ai.HTN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.Murderer
{
	[CreateAssetMenu(menuName="Rust/AI/Murderer Definition")]
	public class MurdererDefinition : BaseNpcDefinition
	{
		[Header("Aim")]
		public AnimationCurve MissFunction = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[Header("Equipment")]
		public PlayerInventoryProperties[] loadouts;

		public LootContainer.LootSpawnSlot[] Loot;

		[Header("Audio")]
		public GameObjectRef DeathEffect;

		public MurdererDefinition()
		{
		}

		private IEnumerator EquipWeapon(HTNPlayer target)
		{
			yield return CoroutineEx.waitForSeconds(0.25f);
			if (target == null || target.IsDestroyed || target.IsDead() || target.IsWounded() || target.inventory == null || target.inventory.containerBelt == null)
			{
				yield break;
			}
			Item item = target.inventory.containerBelt.GetSlot(0);
			if (item == null)
			{
				yield break;
			}
			target.UpdateActiveItem(item.uid);
			yield return CoroutineEx.waitForSeconds(0.25f);
			MurdererDomain aiDomain = target.AiDomain as MurdererDomain;
			if (aiDomain)
			{
				if (item.info.category == ItemCategory.Weapon)
				{
					BaseEntity heldEntity = item.GetHeldEntity();
					if (heldEntity is BaseProjectile)
					{
						aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.ProjectileWeapon, true, true, true);
						aiDomain.ReloadFirearm();
					}
					else if (heldEntity is BaseMelee)
					{
						aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.MeleeWeapon, true, true, true);
						Chainsaw chainsaw = heldEntity as Chainsaw;
						if (chainsaw)
						{
							chainsaw.ServerNPCStart();
						}
					}
					else if (heldEntity is ThrownWeapon)
					{
						aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.ThrowableWeapon, true, true, true);
					}
				}
				else if (item.info.category == ItemCategory.Medical)
				{
					aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.HealingItem, true, true, true);
				}
				else if (item.info.category == ItemCategory.Tool)
				{
					BaseEntity baseEntity = item.GetHeldEntity();
					if (!(baseEntity is BaseMelee))
					{
						aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.LightSourceItem, true, true, true);
					}
					else
					{
						aiDomain.MurdererContext.SetFact(Facts.HeldItemType, ItemType.MeleeWeapon, true, true, true);
						Chainsaw chainsaw1 = baseEntity as Chainsaw;
						if (chainsaw1)
						{
							chainsaw1.ServerNPCStart();
						}
					}
				}
			}
		}

		public override void Loadout(HTNPlayer target)
		{
			if (target == null || target.IsDestroyed || target.IsDead() || target.IsWounded() || target.inventory == null || target.inventory.containerBelt == null || target.inventory.containerMain == null || target.inventory.containerWear == null)
			{
				return;
			}
			if (this.loadouts == null || this.loadouts.Length == 0)
			{
				UnityEngine.Debug.LogWarning(string.Concat("Loadout for NPC ", base.name, " was empty."));
			}
			else
			{
				PlayerInventoryProperties playerInventoryProperty = this.loadouts[UnityEngine.Random.Range(0, (int)this.loadouts.Length)];
				if (playerInventoryProperty != null)
				{
					playerInventoryProperty.GiveToPlayer(target);
					target.StartCoroutine(this.EquipWeapon(target));
					return;
				}
			}
		}

		public override BaseCorpse OnCreateCorpse(HTNPlayer target)
		{
			int i;
			BaseCorpse baseCorpse;
			if (this.DeathEffect.isValid)
			{
				Effect.server.Run(this.DeathEffect.resourcePath, target, 0, Vector3.zero, Vector3.zero, null, false);
			}
			using (TimeWarning timeWarning = TimeWarning.New("Create corpse", 0.1f))
			{
				NPCPlayerCorpse navAgent = target.DropCorpse("assets/prefabs/npc/murderer/murderer_corpse.prefab") as NPCPlayerCorpse;
				if (navAgent)
				{
					if (target.AiDomain != null && target.AiDomain.NavAgent != null && target.AiDomain.NavAgent.isOnNavMesh)
					{
						navAgent.transform.position = navAgent.transform.position + (Vector3.down * target.AiDomain.NavAgent.baseOffset);
					}
					navAgent.SetLootableIn(2f);
					navAgent.SetFlag(BaseEntity.Flags.Reserved5, target.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
					navAgent.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
					int num = 0;
					while (num < target.inventory.containerWear.itemList.Count)
					{
						Item item = target.inventory.containerWear.itemList[num];
						if (item == null || !(item.info.shortname == "gloweyes"))
						{
							num++;
						}
						else
						{
							target.inventory.containerWear.Remove(item);
							break;
						}
					}
					navAgent.TakeFrom(new ItemContainer[] { target.inventory.containerMain, target.inventory.containerWear, target.inventory.containerBelt });
					navAgent.playerName = target.displayName;
					navAgent.playerSteamID = target.userID;
					navAgent.Spawn();
					navAgent.TakeChildren(target);
					ItemContainer[] itemContainerArray = navAgent.containers;
					for (i = 0; i < (int)itemContainerArray.Length; i++)
					{
						itemContainerArray[i].Clear();
					}
					if (this.Loot.Length != 0)
					{
						LootContainer.LootSpawnSlot[] loot = this.Loot;
						for (i = 0; i < (int)loot.Length; i++)
						{
							LootContainer.LootSpawnSlot lootSpawnSlot = loot[i];
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

		public override void OnlyLoadoutWeapons(HTNPlayer target)
		{
			if (target == null || target.IsDestroyed || target.IsDead() || target.IsWounded() || target.inventory == null || target.inventory.containerBelt == null || target.inventory.containerMain == null || target.inventory.containerWear == null)
			{
				return;
			}
			if (this.loadouts == null || this.loadouts.Length == 0)
			{
				UnityEngine.Debug.LogWarning(string.Concat("Loadout for NPC ", base.name, " was empty."));
			}
			else
			{
				PlayerInventoryProperties playerInventoryProperty = this.loadouts[UnityEngine.Random.Range(0, (int)this.loadouts.Length)];
				if (playerInventoryProperty != null)
				{
					foreach (ItemAmount itemAmount in playerInventoryProperty.belt)
					{
						if (itemAmount.itemDef.category != ItemCategory.Weapon)
						{
							continue;
						}
						target.inventory.GiveItem(ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, (ulong)0), target.inventory.containerBelt);
					}
					target.StartCoroutine(this.EquipWeapon(target));
					return;
				}
			}
		}

		public override void StartVoices(HTNPlayer target)
		{
		}

		public override void StopVoices(HTNPlayer target)
		{
		}
	}
}