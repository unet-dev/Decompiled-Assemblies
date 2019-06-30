using System;
using UnityEngine;

public class ScientistNPC : HumanNPC
{
	public GameObjectRef[] RadioChatterEffects;

	public GameObjectRef[] DeathEffects;

	public string deathStatName = "kill_scientist";

	public Vector2 IdleChatterRepeatRange = new Vector2(10f, 15f);

	public ScientistNPC.RadioChatterType radioChatterType;

	protected float lastAlertedTime = -100f;

	public ScientistNPC()
	{
	}

	public void Alert()
	{
		this.lastAlertedTime = Time.time;
		this.SetChatterType(ScientistNPC.RadioChatterType.Alert);
	}

	public override void EquipWeapon()
	{
		base.EquipWeapon();
		HeldEntity heldEntity = base.GetHeldEntity();
		if (heldEntity != null)
		{
			Item item = heldEntity.GetItem();
			if (item != null && item.contents != null)
			{
				if (UnityEngine.Random.Range(0, 3) == 0)
				{
					Item item1 = ItemManager.CreateByName("weapon.mod.flashlight", 1, (ulong)0);
					if (!item1.MoveToContainer(item.contents, -1, true))
					{
						item1.Remove(0f);
						return;
					}
					this.lightsOn = false;
					base.InvokeRandomized(new Action(this.LightCheck), 0f, 30f, 5f);
					base.LightCheck();
					return;
				}
				Item item2 = ItemManager.CreateByName("weapon.mod.lasersight", 1, (ulong)0);
				if (!item2.MoveToContainer(item.contents, -1, true))
				{
					item2.Remove(0f);
				}
				base.SetLightsOn(true);
				this.lightsOn = true;
			}
		}
	}

	public void IdleCheck()
	{
		if (Time.time > this.lastAlertedTime + 20f)
		{
			this.SetChatterType(ScientistNPC.RadioChatterType.Idle);
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		this.Alert();
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		this.SetChatterType(ScientistNPC.RadioChatterType.NONE);
		if (this.DeathEffects.Length != 0)
		{
			Effect.server.Run(this.DeathEffects[UnityEngine.Random.Range(0, (int)this.DeathEffects.Length)].resourcePath, this.ServerPosition, Vector3.up, null, false);
		}
		if (info.InitiatorPlayer != null && !info.InitiatorPlayer.IsNpc)
		{
			info.InitiatorPlayer.stats.Add(this.deathStatName, 1, Stats.Steam);
		}
	}

	public void PlayRadioChatter()
	{
		if (this.RadioChatterEffects.Length == 0)
		{
			return;
		}
		if (base.IsDestroyed || base.transform == null)
		{
			base.CancelInvoke(new Action(this.PlayRadioChatter));
			return;
		}
		Effect.server.Run(this.RadioChatterEffects[UnityEngine.Random.Range(0, (int)this.RadioChatterEffects.Length)].resourcePath, this, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
		this.QueueRadioChatter();
	}

	public void QueueRadioChatter()
	{
		if (!this.IsAlive() || base.IsDestroyed)
		{
			return;
		}
		base.Invoke(new Action(this.PlayRadioChatter), UnityEngine.Random.Range(this.IdleChatterRepeatRange.x, this.IdleChatterRepeatRange.y));
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.SetChatterType(ScientistNPC.RadioChatterType.Idle);
		base.InvokeRandomized(new Action(this.IdleCheck), 0f, 20f, 1f);
	}

	public void SetChatterType(ScientistNPC.RadioChatterType newType)
	{
		if (newType == this.radioChatterType)
		{
			return;
		}
		if (newType == ScientistNPC.RadioChatterType.Idle)
		{
			this.QueueRadioChatter();
			return;
		}
		base.CancelInvoke(new Action(this.PlayRadioChatter));
	}

	public override bool ShotTest()
	{
		bool flag = base.ShotTest();
		if (Time.time - this.lastGunShotTime < 5f)
		{
			this.Alert();
		}
		return flag;
	}

	public enum RadioChatterType
	{
		NONE,
		Idle,
		Alert
	}
}