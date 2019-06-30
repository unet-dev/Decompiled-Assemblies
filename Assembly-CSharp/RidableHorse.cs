using Network;
using System;
using UnityEngine;

public class RidableHorse : BaseRidableAnimal
{
	[ServerVar(Help="Population active on the server, per square km")]
	public static float Population;

	public string distanceStatName = "";

	private const string suffixCull = ".prefab";

	private float distanceRecordingSpacing = 5f;

	private float totalDistance;

	private float kmDistance;

	private float tempDistanceTravelled;

	static RidableHorse()
	{
		RidableHorse.Population = 2f;
	}

	public RidableHorse()
	{
	}

	public override void MarkDistanceTravelled(float amount)
	{
		this.tempDistanceTravelled += amount;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("RidableHorse.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		base.CancelInvoke(new Action(this.RecordDistance));
	}

	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		base.InvokeRepeating(new Action(this.RecordDistance), this.distanceRecordingSpacing, this.distanceRecordingSpacing);
	}

	public void RecordDistance()
	{
		BasePlayer driver = base.GetDriver();
		if (driver == null)
		{
			this.tempDistanceTravelled = 0f;
			return;
		}
		this.kmDistance = this.kmDistance + this.tempDistanceTravelled / 1000f;
		if (this.kmDistance >= 1f)
		{
			driver.stats.Add(string.Concat(this.distanceStatName, "_km"), 1, Stats.Steam);
			this.kmDistance -= 1f;
		}
		driver.stats.Add(this.distanceStatName, Mathf.FloorToInt(this.tempDistanceTravelled), Stats.Steam);
		driver.stats.Save();
		this.totalDistance += this.tempDistanceTravelled;
		this.tempDistanceTravelled = 0f;
	}
}