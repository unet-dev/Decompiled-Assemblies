using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class SphereEntity : BaseEntity
{
	public float currentRadius = 1f;

	public float lerpRadius = 1f;

	public float lerpSpeed = 1f;

	public SphereEntity()
	{
	}

	public void LerpRadiusTo(float radius, float speed)
	{
		this.lerpRadius = radius;
		this.lerpSpeed = speed;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer)
		{
			if (info.msg.sphereEntity != null)
			{
				float single = info.msg.sphereEntity.radius;
				float single1 = single;
				this.lerpRadius = single;
				this.currentRadius = single1;
			}
			this.UpdateScale();
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.currentRadius;
	}

	protected void Update()
	{
		if (this.currentRadius == this.lerpRadius)
		{
			return;
		}
		if (base.isServer)
		{
			this.currentRadius = Mathf.MoveTowards(this.currentRadius, this.lerpRadius, Time.deltaTime * this.lerpSpeed);
			this.UpdateScale();
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	protected void UpdateScale()
	{
		base.transform.localScale = new Vector3(this.currentRadius, this.currentRadius, this.currentRadius);
	}
}