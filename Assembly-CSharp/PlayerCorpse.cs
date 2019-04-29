using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class PlayerCorpse : LootableCorpse
{
	public Buoyancy buoyancy;

	public const BaseEntity.Flags Flag_Buoyant = BaseEntity.Flags.Reserved6;

	public PlayerCorpse()
	{
	}

	public void BuoyancyChanged(bool isSubmerged)
	{
		if (this.IsBuoyant())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved6, isSubmerged, false, false);
		base.SendNetworkUpdate_Flags();
	}

	public override string Categorize()
	{
		return "playercorpse";
	}

	public bool IsBuoyant()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved6);
	}

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if (baseEntity.InSafeZone() && baseEntity.userID != this.playerSteamID)
		{
			return false;
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (base.isServer && this.containers != null && (int)this.containers.Length > 1 && !info.forDisk)
		{
			info.msg.storageBox = Pool.Get<StorageBox>();
			info.msg.storageBox.contents = this.containers[1].Save();
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (this.buoyancy == null)
		{
			Debug.LogWarning(string.Concat("Player corpse has no buoyancy assigned, searching at runtime :", base.name));
			this.buoyancy = base.GetComponent<Buoyancy>();
		}
		if (this.buoyancy != null)
		{
			this.buoyancy.SubmergedChanged = new Action<bool>(this.BuoyancyChanged);
		}
	}
}