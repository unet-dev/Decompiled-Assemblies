using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

public class XMasRefill : BaseEntity
{
	public GameObjectRef[] giftPrefabs;

	public List<BasePlayer> goodKids;

	public List<Stocking> stockings;

	public AudioSource bells;

	public XMasRefill()
	{
	}

	public bool DistributeGiftsForPlayer(BasePlayer player)
	{
		int num = this.GiftsPerPlayer();
		int num1 = this.GiftSpawnAttempts();
		for (int i = 0; i < num1 && num > 0; i++)
		{
			Vector2 vector2 = UnityEngine.Random.insideUnitCircle * this.GiftRadius();
			Vector3 vector3 = player.transform.position + new Vector3(vector2.x, 10f, vector2.y);
			Quaternion quaternion = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
			if (this.DropToGround(ref vector3))
			{
				string str = this.giftPrefabs[UnityEngine.Random.Range(0, (int)this.giftPrefabs.Length)].resourcePath;
				BaseEntity baseEntity = GameManager.server.CreateEntity(str, vector3, quaternion, true);
				if (baseEntity)
				{
					baseEntity.Spawn();
					num--;
				}
			}
		}
		return true;
	}

	public void DistributeLoot()
	{
		if (this.goodKids.Count > 0)
		{
			BasePlayer basePlayer = null;
			foreach (BasePlayer goodKid in this.goodKids)
			{
				if (goodKid.IsSleeping() || goodKid.IsWounded() || !goodKid.IsAlive())
				{
					continue;
				}
				basePlayer = goodKid;
				goto Label0;
			}
		Label0:
			if (basePlayer)
			{
				this.DistributeGiftsForPlayer(basePlayer);
				this.goodKids.Remove(basePlayer);
			}
		}
		if (this.stockings.Count > 0)
		{
			Stocking item = this.stockings[0];
			if (item != null)
			{
				item.SpawnLoot();
			}
			this.stockings.RemoveAt(0);
		}
	}

	protected bool DropToGround(ref Vector3 pos)
	{
		RaycastHit raycastHit;
		int num = 1235288065;
		int num1 = 8454144;
		if (TerrainMeta.TopologyMap && (TerrainMeta.TopologyMap.GetTopology(pos) & 82048) != 0)
		{
			return false;
		}
		if (TerrainMeta.HeightMap && TerrainMeta.Collision && !TerrainMeta.Collision.GetIgnore(pos, 0.01f))
		{
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			pos.y = Mathf.Max(pos.y, height);
		}
		if (!TransformUtil.GetGroundInfo(pos, out raycastHit, 80f, num, null))
		{
			return false;
		}
		if ((1 << (raycastHit.transform.gameObject.layer & 31) & num1) == 0)
		{
			return false;
		}
		pos = raycastHit.point;
		return true;
	}

	public float GiftRadius()
	{
		return XMas.spawnRange;
	}

	public int GiftSpawnAttempts()
	{
		return XMas.giftsPerPlayer * XMas.spawnAttempts;
	}

	public int GiftsPerPlayer()
	{
		return XMas.giftsPerPlayer;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("XMasRefill.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public void RemoveMe()
	{
		if (this.goodKids.Count == 0 && this.stockings.Count == 0)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
			return;
		}
		base.Invoke(new Action(this.RemoveMe), 60f);
	}

	public void SendBells()
	{
		base.ClientRPC(null, "PlayBells");
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!XMas.enabled)
		{
			base.Invoke(new Action(this.RemoveMe), 0.1f);
			return;
		}
		this.goodKids = (BasePlayer.activePlayerList != null ? new List<BasePlayer>(BasePlayer.activePlayerList) : new List<BasePlayer>());
		this.stockings = (Stocking.stockings != null ? new List<Stocking>(Stocking.stockings.Values) : new List<Stocking>());
		base.Invoke(new Action(this.RemoveMe), 60f);
		base.InvokeRepeating(new Action(this.DistributeLoot), 3f, 0.02f);
		base.Invoke(new Action(this.SendBells), 0.5f);
	}
}