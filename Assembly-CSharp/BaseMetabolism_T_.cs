using ConVar;
using Rust;
using System;
using UnityEngine;

public abstract class BaseMetabolism<T> : EntityComponent<T>
where T : BaseCombatEntity
{
	protected T owner;

	public MetabolismAttribute calories;

	public MetabolismAttribute hydration;

	public MetabolismAttribute heartrate;

	protected float timeSinceLastMetabolism;

	protected BaseMetabolism()
	{
	}

	public void ApplyChange(MetabolismAttribute.Type type, float amount, float time)
	{
		MetabolismAttribute metabolismAttribute = this.FindAttribute(type);
		if (metabolismAttribute == null)
		{
			return;
		}
		metabolismAttribute.Add(amount);
	}

	protected virtual void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
	{
		if (this.calories.@value <= 20f)
		{
			using (TimeWarning timeWarning = TimeWarning.New("Calories Hurt", 0.1f))
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, this.calories.@value) * delta * 0.0833333358f, DamageType.Hunger, null, true);
			}
		}
		if (this.hydration.@value <= 20f)
		{
			using (timeWarning = TimeWarning.New("Hyration Hurt", 0.1f))
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, this.hydration.@value) * delta * 0.13333334f, DamageType.Thirst, null, true);
			}
		}
	}

	public virtual MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		switch (type)
		{
			case MetabolismAttribute.Type.Calories:
			{
				return this.calories;
			}
			case MetabolismAttribute.Type.Hydration:
			{
				return this.hydration;
			}
			case MetabolismAttribute.Type.Heartrate:
			{
				return this.heartrate;
			}
		}
		return null;
	}

	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.owner = default(T);
	}

	public virtual void Reset()
	{
		this.calories.Reset();
		this.hydration.Reset();
		this.heartrate.Reset();
	}

	protected virtual void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
	{
		if (this.calories.@value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, this.calories.@value) * delta * 0.0166666675f);
		}
		if (this.hydration.@value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, this.hydration.@value) * delta * 0.0166666675f);
		}
		this.hydration.MoveTowards(0f, delta * 0.008333334f);
		this.calories.MoveTowards(0f, delta * 0.0166666675f);
		this.heartrate.MoveTowards(0.05f, delta * 0.0166666675f);
	}

	public virtual void ServerInit(T owner)
	{
		this.Reset();
		this.owner = owner;
	}

	public virtual void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
	{
		this.timeSinceLastMetabolism += delta;
		if (this.timeSinceLastMetabolism <= ConVar.Server.metabolismtick)
		{
			return;
		}
		if (this.owner && !this.owner.IsDead())
		{
			this.RunMetabolism(ownerEntity, this.timeSinceLastMetabolism);
			this.DoMetabolismDamage(ownerEntity, this.timeSinceLastMetabolism);
		}
		this.timeSinceLastMetabolism = 0f;
	}

	public bool ShouldDie()
	{
		if (!this.owner)
		{
			return false;
		}
		return this.owner.Health() <= 0f;
	}
}