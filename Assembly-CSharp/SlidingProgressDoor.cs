using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class SlidingProgressDoor : ProgressDoor
{
	public Vector3 openPosition;

	public Vector3 closedPosition;

	public GameObject doorObject;

	private float lastEnergyTime;

	private float lastServerUpdateTime;

	public SlidingProgressDoor()
	{
	}

	public override void AddEnergy(float amount)
	{
		this.lastEnergyTime = Time.time;
		base.AddEnergy(amount);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ProtoBuf.SphereEntity sphereEntity = info.msg.sphereEntity;
	}

	public override void NoEnergy()
	{
		base.NoEnergy();
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.storedEnergy;
	}

	public void ServerUpdate()
	{
		if (base.isServer)
		{
			if (this.lastServerUpdateTime == 0f)
			{
				this.lastServerUpdateTime = Time.realtimeSinceStartup;
			}
			float single = Time.realtimeSinceStartup - this.lastServerUpdateTime;
			this.lastServerUpdateTime = Time.realtimeSinceStartup;
			if (Time.time > this.lastEnergyTime + 0.333f)
			{
				float single1 = this.energyForOpen * single / this.secondsToClose;
				float single2 = Mathf.Min(this.storedEnergy, single1);
				this.storedEnergy -= single2;
				this.storedEnergy = Mathf.Clamp(this.storedEnergy, 0f, this.energyForOpen);
				if (single2 > 0f)
				{
					IOEntity.IOSlot[] oSlotArray = this.outputs;
					for (int i = 0; i < (int)oSlotArray.Length; i++)
					{
						IOEntity.IOSlot oSlot = oSlotArray[i];
						if (oSlot.connectedTo.Get(true) != null)
						{
							oSlot.connectedTo.Get(true).IOInput(this, this.ioType, -single2, oSlot.connectedToSlot);
						}
					}
				}
			}
			this.UpdateProgress();
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		base.InvokeRepeating(new Action(this.ServerUpdate), 0f, 0.1f);
	}

	public override void UpdateProgress()
	{
		base.UpdateProgress();
		Vector3 vector3 = this.doorObject.transform.localPosition;
		float single = this.storedEnergy / this.energyForOpen;
		Vector3 vector31 = Vector3.Lerp(this.closedPosition, this.openPosition, single);
		this.doorObject.transform.localPosition = vector31;
		if (base.isServer)
		{
			bool flag = Vector3.Distance(vector3, vector31) > 0.01f;
			base.SetFlag(BaseEntity.Flags.Reserved1, flag, false, true);
		}
	}
}