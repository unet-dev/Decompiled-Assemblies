using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class ElectricWindmill : IOEntity
{
	public Animator animator;

	public int maxPowerGeneration = 100;

	public Transform vaneRot;

	public SoundDefinition wooshSound;

	public Transform wooshOrigin;

	private float serverWindSpeed;

	public ElectricWindmill()
	{
	}

	public bool AmIVisible()
	{
		int num = 15;
		Vector3 vector3 = base.transform.position + (Vector3.up * 6f);
		if (!base.IsVisible(vector3 + (base.transform.up * (float)num), (float)(num + 1)))
		{
			return false;
		}
		if (!base.IsVisible(vector3 + (this.GetWindAimDir(Time.time) * (float)num), (float)(num + 1)))
		{
			return false;
		}
		return true;
	}

	public Vector3 GetWindAimDir(float time)
	{
		float single = time / 3600f * 360f;
		int num = 10;
		Vector3 vector3 = new Vector3(Mathf.Sin(single * 0.0174532924f) * (float)num, 0f, Mathf.Cos(single * 0.0174532924f) * (float)num);
		return vector3.normalized;
	}

	public float GetWindSpeedScale()
	{
		float single = Time.time / 600f;
		float single1 = base.transform.position.x / 512f;
		float single2 = base.transform.position.z / 512f;
		float single3 = Mathf.PerlinNoise(single1 + single, single2 + single * 0.1f);
		float height = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		float single4 = base.transform.position.y - height;
		if (single4 < 0f)
		{
			single4 = 0f;
		}
		return Mathf.Clamp01(Mathf.InverseLerp(0f, 50f, single4) * 0.5f + single3);
	}

	public override bool IsRootEntity()
	{
		return true;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			if (info.msg.ioEntity == null)
			{
				info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
			}
			info.msg.ioEntity.genericFloat1 = Time.time;
			info.msg.ioEntity.genericFloat2 = this.serverWindSpeed;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.WindUpdate), 1f, 20f, 2f);
	}

	public void WindUpdate()
	{
		this.serverWindSpeed = this.GetWindSpeedScale();
		if (!this.AmIVisible())
		{
			this.serverWindSpeed = 0f;
		}
		int num = Mathf.FloorToInt((float)this.maxPowerGeneration * this.serverWindSpeed);
		bool flag = this.currentEnergy != num;
		this.currentEnergy = num;
		if (flag)
		{
			this.MarkDirty();
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}
}