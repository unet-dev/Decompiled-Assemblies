using ConVar;
using System;
using UnityEngine;

public class AttackEntity : HeldEntity
{
	[Header("Attack Entity")]
	public float deployDelay = 1f;

	public float repeatDelay = 0.5f;

	public float animationDelay;

	[Header("NPCUsage")]
	public float effectiveRange = 1f;

	public float npcDamageScale = 1f;

	public float attackLengthMin = -1f;

	public float attackLengthMax = -1f;

	public float attackSpacing;

	public float aiAimSwayOffset;

	public float aiAimCone;

	public bool aiOnlyInRange;

	public NPCPlayerApex.WeaponTypeEnum effectiveRangeType = NPCPlayerApex.WeaponTypeEnum.MediumRange;

	public float CloseRangeAddition;

	public float MediumRangeAddition;

	public float LongRangeAddition;

	public bool CanUseAtMediumRange = true;

	public bool CanUseAtLongRange = true;

	public SoundDefinition[] reloadSounds;

	public SoundDefinition thirdPersonMeleeSound;

	private float nextAttackTime = Single.NegativeInfinity;

	public float NextAttackTime
	{
		get
		{
			return this.nextAttackTime;
		}
	}

	public AttackEntity()
	{
	}

	public virtual float AmmoFraction()
	{
		return 0f;
	}

	protected float CalculateCooldownTime(float nextTime, float cooldown, bool catchup)
	{
		float single = UnityEngine.Time.time;
		float single1 = 0f;
		if (base.isServer)
		{
			BasePlayer ownerPlayer = base.GetOwnerPlayer();
			single1 += 0.1f;
			single1 = single1 + cooldown * 0.1f;
			single1 = single1 + (ownerPlayer ? ownerPlayer.desyncTime : 0.1f);
			single1 += Mathf.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime);
		}
		if (nextTime < 0f)
		{
			nextTime = Mathf.Max(0f, single + cooldown - single1);
		}
		else if (single - nextTime > single1)
		{
			nextTime = Mathf.Max(nextTime + cooldown, single + cooldown - single1);
		}
		else
		{
			nextTime = Mathf.Min(nextTime + cooldown, single + cooldown);
		}
		return nextTime;
	}

	public virtual bool CanReload()
	{
		return false;
	}

	protected float GetAttackCooldown()
	{
		return Mathf.Max(this.nextAttackTime - UnityEngine.Time.time, 0f);
	}

	protected float GetAttackIdle()
	{
		return Mathf.Max(UnityEngine.Time.time - this.nextAttackTime, 0f);
	}

	public virtual void GetAttackStats(HitInfo info)
	{
	}

	public bool HasAttackCooldown()
	{
		return UnityEngine.Time.time < this.nextAttackTime;
	}

	public virtual Vector3 ModifyAIAim(Vector3 eulerInput, float swayModifier = 1f)
	{
		return eulerInput;
	}

	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		this.StartAttackCooldown(this.deployDelay * 0.9f);
	}

	protected void ResetAttackCooldown()
	{
		this.nextAttackTime = Single.NegativeInfinity;
	}

	public virtual bool ServerIsReloading()
	{
		return false;
	}

	public virtual void ServerReload()
	{
	}

	public virtual void ServerUse()
	{
	}

	public virtual void ServerUse(float damageModifier)
	{
		this.ServerUse();
	}

	protected void StartAttackCooldown(float cooldown)
	{
		this.nextAttackTime = this.CalculateCooldownTime(this.nextAttackTime, cooldown, true);
	}

	public virtual void TopUpAmmo()
	{
	}

	protected bool ValidateEyePos(BasePlayer player, Vector3 eyePos)
	{
		bool flag = true;
		if (eyePos.IsNaNOrInfinity())
		{
			string shortPrefabName = base.ShortPrefabName;
			AntiHack.Log(player, AntiHackType.EyeHack, string.Concat("Contains NaN (", shortPrefabName, ")"));
			player.stats.combat.Log(this, "eye_nan");
			flag = false;
		}
		if (ConVar.AntiHack.eye_protection > 0)
		{
			float eyeForgiveness = 1f + ConVar.AntiHack.eye_forgiveness;
			float eyeClientframes = ConVar.AntiHack.eye_clientframes;
			float eyeServerframes = ConVar.AntiHack.eye_serverframes;
			float single = eyeClientframes / 60f;
			float single1 = eyeServerframes * Mathx.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime, UnityEngine.Time.fixedDeltaTime);
			float single2 = (player.desyncTime + single + single1) * eyeForgiveness;
			if (ConVar.AntiHack.eye_protection >= 1)
			{
				float single3 = player.MaxVelocity() + player.GetParentVelocity().magnitude;
				float single4 = player.BoundsPadding() + single2 * single3;
				float single5 = Vector3.Distance(player.eyes.position, eyePos);
				if (single5 > single4)
				{
					string str = base.ShortPrefabName;
					AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[] { "Distance (", str, " on attack with ", single5, "m > ", single4, "m)" }));
					player.stats.combat.Log(this, "eye_distance");
					flag = false;
				}
			}
			if (ConVar.AntiHack.eye_protection >= 2)
			{
				Vector3 vector3 = player.eyes.center;
				Vector3 vector31 = player.eyes.position;
				Vector3 vector32 = eyePos;
				if (!GamePhysics.LineOfSight(vector3, vector31, vector32, 2162688, 0f))
				{
					string shortPrefabName1 = base.ShortPrefabName;
					AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[] { "Line of sight (", shortPrefabName1, " on attack) ", vector3, " ", vector31, " ", vector32 }));
					player.stats.combat.Log(this, "eye_los");
					flag = false;
				}
			}
			if (!flag)
			{
				AntiHack.AddViolation(player, AntiHackType.EyeHack, ConVar.AntiHack.eye_penalty);
			}
		}
		return flag;
	}

	protected virtual bool VerifyClientAttack(BasePlayer player)
	{
		if (!this.VerifyClientRPC(player))
		{
			return false;
		}
		if (!this.HasAttackCooldown())
		{
			return true;
		}
		AntiHack.Log(player, AntiHackType.CooldownHack, string.Concat(new object[] { "T-", this.GetAttackCooldown(), "s (", base.ShortPrefabName, ")" }));
		player.stats.combat.Log(this, "attack_cooldown");
		return false;
	}

	protected bool VerifyClientRPC(BasePlayer player)
	{
		if (player == null)
		{
			Debug.LogWarning("Received RPC from null player");
			return false;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			AntiHack.Log(player, AntiHackType.AttackHack, string.Concat("Owner not found (", base.ShortPrefabName, ")"));
			player.stats.combat.Log(this, "owner_missing");
			return false;
		}
		if (ownerPlayer != player)
		{
			AntiHack.Log(player, AntiHackType.AttackHack, string.Concat("Player mismatch (", base.ShortPrefabName, ")"));
			player.stats.combat.Log(this, "player_mismatch");
			return false;
		}
		if (player.IsDead())
		{
			AntiHack.Log(player, AntiHackType.AttackHack, string.Concat("Player dead (", base.ShortPrefabName, ")"));
			player.stats.combat.Log(this, "player_dead");
			return false;
		}
		if (player.IsWounded())
		{
			AntiHack.Log(player, AntiHackType.AttackHack, string.Concat("Player down (", base.ShortPrefabName, ")"));
			player.stats.combat.Log(this, "player_down");
			return false;
		}
		if (player.IsSleeping())
		{
			AntiHack.Log(player, AntiHackType.AttackHack, string.Concat("Player sleeping (", base.ShortPrefabName, ")"));
			player.stats.combat.Log(this, "player_sleeping");
			return false;
		}
		if (player.desyncTime > ConVar.AntiHack.maxdesync)
		{
			AntiHack.Log(player, AntiHackType.AttackHack, string.Concat(new object[] { "Player stalled (", base.ShortPrefabName, " with ", player.desyncTime, "s)" }));
			player.stats.combat.Log(this, "player_stalled");
			return false;
		}
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			AntiHack.Log(player, AntiHackType.AttackHack, string.Concat("Item not found (", base.ShortPrefabName, ")"));
			player.stats.combat.Log(this, "item_missing");
			return false;
		}
		if (!ownerItem.isBroken)
		{
			return true;
		}
		AntiHack.Log(player, AntiHackType.AttackHack, string.Concat("Item broken (", base.ShortPrefabName, ")"));
		player.stats.combat.Log(this, "item_broken");
		return false;
	}
}