using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerComfort : TriggerBase
{
	public float triggerSize;

	public float baseComfort = 0.5f;

	public float minComfortRange = 2.5f;

	private const float perPlayerComfortBonus = 0.25f;

	private const float bonusComfort = 0f;

	private List<BasePlayer> _players = new List<BasePlayer>();

	public TriggerComfort()
	{
	}

	public float CalculateComfort(Vector3 position, BasePlayer forPlayer = null)
	{
		float single = Vector3.Distance(base.gameObject.transform.position, position);
		float single1 = 1f - Mathf.Clamp(single - this.minComfortRange, 0f, single / (this.triggerSize - this.minComfortRange));
		float single2 = 0f;
		foreach (BasePlayer _player in this._players)
		{
			if (_player == forPlayer)
			{
				continue;
			}
			single2 = single2 + 0.25f * (_player.IsSleeping() ? 0.5f : 1f) * (_player.IsAlive() ? 1f : 0f);
		}
		return (this.baseComfort + (0f + single2)) * single1;
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (!basePlayer)
		{
			return;
		}
		this._players.Add(basePlayer);
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (!basePlayer)
		{
			return;
		}
		this._players.Remove(basePlayer);
	}

	private void OnValidate()
	{
		this.triggerSize = base.GetComponent<SphereCollider>().radius * base.transform.localScale.y;
	}
}