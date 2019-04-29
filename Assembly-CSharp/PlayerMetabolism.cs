using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;

public class PlayerMetabolism : BaseMetabolism<BasePlayer>
{
	public const float HotThreshold = 40f;

	public const float ColdThreshold = 5f;

	public MetabolismAttribute temperature = new MetabolismAttribute();

	public MetabolismAttribute poison = new MetabolismAttribute();

	public MetabolismAttribute radiation_level = new MetabolismAttribute();

	public MetabolismAttribute radiation_poison = new MetabolismAttribute();

	public MetabolismAttribute wetness = new MetabolismAttribute();

	public MetabolismAttribute dirtyness = new MetabolismAttribute();

	public MetabolismAttribute oxygen = new MetabolismAttribute();

	public MetabolismAttribute bleeding = new MetabolismAttribute();

	public MetabolismAttribute comfort = new MetabolismAttribute();

	public MetabolismAttribute pending_health = new MetabolismAttribute();

	public bool isDirty;

	private float lastConsumeTime;

	public PlayerMetabolism()
	{
	}

	public bool CanConsume()
	{
		if (this.owner && this.owner.IsHeadUnderwater())
		{
			return false;
		}
		return UnityEngine.Time.time - this.lastConsumeTime > 1f;
	}

	private float DeltaCold()
	{
		return Mathf.InverseLerp(20f, -50f, this.temperature.@value);
	}

	private float DeltaHot()
	{
		return Mathf.InverseLerp(20f, 100f, this.temperature.@value);
	}

	private float DeltaWet()
	{
		return this.wetness.@value;
	}

	protected override void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
	{
		base.DoMetabolismDamage(ownerEntity, delta);
		if (this.temperature.@value < -20f)
		{
			this.owner.Hurt(Mathf.InverseLerp(1f, -50f, this.temperature.@value) * delta * 1f, DamageType.Cold, null, true);
		}
		else if (this.temperature.@value < -10f)
		{
			this.owner.Hurt(Mathf.InverseLerp(1f, -50f, this.temperature.@value) * delta * 0.3f, DamageType.Cold, null, true);
		}
		else if (this.temperature.@value < 1f)
		{
			this.owner.Hurt(Mathf.InverseLerp(1f, -50f, this.temperature.@value) * delta * 0.1f, DamageType.Cold, null, true);
		}
		if (this.temperature.@value > 60f)
		{
			this.owner.Hurt(Mathf.InverseLerp(60f, 200f, this.temperature.@value) * delta * 5f, DamageType.Heat, null, true);
		}
		if (this.oxygen.@value < 0.5f)
		{
			this.owner.Hurt(Mathf.InverseLerp(0.5f, 0f, this.oxygen.@value) * delta * 20f, DamageType.Drowned, null, false);
		}
		if (this.bleeding.@value > 0f)
		{
			float single = delta * 0.333333343f;
			this.owner.Hurt(single, DamageType.Bleeding, null, true);
			this.bleeding.Subtract(single);
		}
		if (this.poison.@value > 0f)
		{
			this.owner.Hurt(this.poison.@value * delta * 0.1f, DamageType.Poison, null, true);
		}
		if (ConVar.Server.radiation && this.radiation_poison.@value > 0f)
		{
			float single1 = (1f + Mathf.Clamp01(this.radiation_poison.@value / 25f) * 5f) * (delta / 5f);
			this.owner.Hurt(single1, DamageType.Radiation, null, true);
			this.radiation_poison.Subtract(single1);
		}
	}

	public override MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		switch (type)
		{
			case MetabolismAttribute.Type.Poison:
			{
				return this.poison;
			}
			case MetabolismAttribute.Type.Radiation:
			{
				return this.radiation_poison;
			}
			case MetabolismAttribute.Type.Bleeding:
			{
				return this.bleeding;
			}
			case MetabolismAttribute.Type.Health:
			{
				return base.FindAttribute(type);
			}
			case MetabolismAttribute.Type.HealthOverTime:
			{
				return this.pending_health;
			}
			default:
			{
				return base.FindAttribute(type);
			}
		}
	}

	internal bool HasChanged()
	{
		bool flag = this.isDirty;
		flag = this.calories.HasChanged() | flag;
		flag = this.hydration.HasChanged() | flag;
		flag = this.heartrate.HasChanged() | flag;
		flag = this.poison.HasChanged() | flag;
		flag = this.radiation_level.HasChanged() | flag;
		flag = this.radiation_poison.HasChanged() | flag;
		flag = this.temperature.HasChanged() | flag;
		flag = this.wetness.HasChanged() | flag;
		flag = this.dirtyness.HasChanged() | flag;
		flag = this.comfort.HasChanged() | flag;
		return this.pending_health.HasChanged() | flag;
	}

	public void Load(ProtoBuf.PlayerMetabolism s)
	{
		this.calories.SetValue(s.calories);
		this.hydration.SetValue(s.hydration);
		this.comfort.SetValue(s.comfort);
		this.heartrate.@value = s.heartrate;
		this.temperature.@value = s.temperature;
		this.radiation_level.@value = s.radiation_level;
		this.radiation_poison.@value = s.radiation_poisoning;
		this.wetness.@value = s.wetness;
		this.dirtyness.@value = s.dirtyness;
		this.oxygen.@value = s.oxygen;
		this.bleeding.@value = s.bleeding;
		this.pending_health.@value = s.pending_health;
		if (this.owner)
		{
			this.owner.health = s.health;
		}
	}

	public void MarkConsumption()
	{
		this.lastConsumeTime = UnityEngine.Time.time;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("PlayerMetabolism.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void Reset()
	{
		base.Reset();
		this.poison.Reset();
		this.radiation_level.Reset();
		this.radiation_poison.Reset();
		this.temperature.Reset();
		this.oxygen.Reset();
		this.bleeding.Reset();
		this.wetness.Reset();
		this.dirtyness.Reset();
		this.comfort.Reset();
		this.pending_health.Reset();
		this.lastConsumeTime = Single.NegativeInfinity;
		this.isDirty = true;
	}

	protected override void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
	{
		if (Interface.CallHook("OnRunPlayerMetabolism", this, ownerEntity, delta) != null)
		{
			return;
		}
		float single = this.owner.currentTemperature;
		float single1 = this.owner.currentComfort;
		float single2 = this.owner.currentCraftLevel;
		this.owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench1, single2 == 1f);
		this.owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench2, single2 == 2f);
		this.owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench3, single2 == 3f);
		this.owner.SetPlayerFlag(BasePlayer.PlayerFlags.SafeZone, this.owner.InSafeZone());
		float single3 = single - this.DeltaWet() * 34f;
		float single4 = Mathf.Clamp(this.owner.baseProtection.amounts[18] * 1.5f, -1f, 1f);
		float single5 = Mathf.InverseLerp(20f, -50f, single);
		float single6 = Mathf.InverseLerp(20f, 30f, single);
		single3 = single3 + single5 * 70f * single4;
		single3 = single3 + single6 * 10f * Mathf.Abs(single4);
		single3 = single3 + this.heartrate.@value * 5f;
		this.temperature.MoveTowards(single3, delta * 5f);
		if (this.temperature.@value >= 40f)
		{
			single1 = 0f;
		}
		this.comfort.MoveTowards(single1, delta / 5f);
		float single7 = 0.6f + 0.4f * this.comfort.@value;
		if ((this.calories.@value <= 100f || this.owner.healthFraction >= single7 || this.radiation_poison.Fraction() >= 0.25f || this.owner.SecondsSinceAttacked <= 10f || this.SignificantBleeding() || this.temperature.@value < 10f ? false : this.hydration.@value > 40f))
		{
			float single8 = Mathf.InverseLerp(this.calories.min, this.calories.max, this.calories.@value);
			float single9 = 5f;
			float single10 = single9 * this.owner.MaxHealth() * 0.8f / 600f;
			single10 = single10 + single10 * single8 * 0.5f;
			float single11 = single10 / single9;
			single11 = single11 + single11 * this.comfort.@value * 6f;
			ownerEntity.Heal(single11 * delta);
			this.calories.Subtract(single10 * delta);
			this.hydration.Subtract(single10 * delta * 0.2f);
		}
		float maxSpeed = this.owner.estimatedSpeed2D / this.owner.GetMaxSpeed() * 0.75f;
		float single12 = Mathf.Clamp(0.05f + maxSpeed, 0f, 1f);
		this.heartrate.MoveTowards(single12, delta * 0.1f);
		float single13 = this.heartrate.Fraction() * 0.375f;
		this.calories.MoveTowards(0f, delta * single13);
		float single14 = 0.008333334f;
		single14 = single14 + Mathf.InverseLerp(40f, 60f, this.temperature.@value) * 0.0833333358f;
		single14 = single14 + this.heartrate.@value * 0.06666667f;
		this.hydration.MoveTowards(0f, delta * single14);
		this.owner.SetPlayerFlag(BasePlayer.PlayerFlags.NoSprint, (this.hydration.Fraction() <= 0f ? true : this.radiation_poison.@value >= 100f));
		if (this.temperature.@value > 40f)
		{
			this.hydration.Add(Mathf.InverseLerp(40f, 200f, this.temperature.@value) * delta * -1f);
		}
		if (this.temperature.@value < 10f)
		{
			float single15 = Mathf.InverseLerp(20f, -100f, this.temperature.@value);
			this.heartrate.MoveTowards(Mathf.Lerp(0.2f, 1f, single15), delta * 2f * single15);
		}
		float single16 = this.owner.WaterFactor();
		if (single16 <= 0.85f)
		{
			this.oxygen.MoveTowards(1f, delta * 1f);
		}
		else
		{
			this.oxygen.MoveTowards(0f, delta * 0.1f);
		}
		float rain = 0f;
		float snow = 0f;
		if (this.owner.IsOutside(this.owner.eyes.position))
		{
			rain = Climate.GetRain(this.owner.eyes.position) * 0.6f;
			snow = Climate.GetSnow(this.owner.eyes.position) * 0.2f;
		}
		bool flag = this.owner.baseProtection.amounts[4] > 0f;
		if (!flag)
		{
			this.wetness.@value = Mathf.Max(this.wetness.@value, single16);
		}
		float single17 = Mathx.Max(this.wetness.@value, rain, snow);
		single17 = Mathf.Min(single17, (flag ? 0f : single17));
		this.wetness.MoveTowards(single17, delta * 0.05f);
		if (single16 < this.wetness.@value)
		{
			this.wetness.MoveTowards(0f, delta * 0.2f * Mathf.InverseLerp(0f, 100f, single));
		}
		this.poison.MoveTowards(0f, delta * 0.5555556f);
		if (this.wetness.Fraction() > 0.4f && this.owner.estimatedSpeed > 0.25f && this.radiation_level.Fraction() == 0f)
		{
			this.radiation_poison.Subtract(this.radiation_poison.@value * 0.2f * this.wetness.Fraction() * delta * 0.2f);
		}
		if (ConVar.Server.radiation)
		{
			this.radiation_level.@value = this.owner.radiationLevel;
			if (this.radiation_level.@value > 0f)
			{
				this.radiation_poison.Add(this.radiation_level.@value * delta);
			}
		}
		if (this.pending_health.@value > 0f)
		{
			float single18 = Mathf.Min(1f * delta, this.pending_health.@value);
			ownerEntity.Heal(single18);
			if (ownerEntity.healthFraction == 1f)
			{
				this.pending_health.@value = 0f;
				return;
			}
			this.pending_health.Subtract(single18);
		}
	}

	public ProtoBuf.PlayerMetabolism Save()
	{
		ProtoBuf.PlayerMetabolism radiationLevel = Facepunch.Pool.Get<ProtoBuf.PlayerMetabolism>();
		radiationLevel.calories = this.calories.@value;
		radiationLevel.hydration = this.hydration.@value;
		radiationLevel.heartrate = this.heartrate.@value;
		radiationLevel.temperature = this.temperature.@value;
		radiationLevel.radiation_level = this.radiation_level.@value;
		radiationLevel.radiation_poisoning = this.radiation_poison.@value;
		radiationLevel.wetness = this.wetness.@value;
		radiationLevel.dirtyness = this.dirtyness.@value;
		radiationLevel.oxygen = this.oxygen.@value;
		radiationLevel.bleeding = this.bleeding.@value;
		radiationLevel.comfort = this.comfort.@value;
		radiationLevel.pending_health = this.pending_health.@value;
		if (this.owner)
		{
			radiationLevel.health = this.owner.Health();
		}
		return radiationLevel;
	}

	public void SendChangesToClient()
	{
		if (!this.HasChanged())
		{
			return;
		}
		this.isDirty = false;
		using (ProtoBuf.PlayerMetabolism playerMetabolism = this.Save())
		{
			base.baseEntity.ClientRPCPlayer<ProtoBuf.PlayerMetabolism>(null, base.baseEntity, "UpdateMetabolism", playerMetabolism);
		}
	}

	public override void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
	{
		base.ServerUpdate(ownerEntity, delta);
		Interface.CallHook("OnPlayerMetabolize", this, ownerEntity, delta);
		this.SendChangesToClient();
	}

	public bool SignificantBleeding()
	{
		return this.bleeding.@value > 0f;
	}

	public void UseHeart(float frate)
	{
		if (this.heartrate.@value > frate)
		{
			this.heartrate.Add(frate);
			return;
		}
		this.heartrate.@value = frate;
	}
}