using System;
using UnityEngine;

public class MapMarkerExplosion : MapMarker
{
	private float duration = 10f;

	public MapMarkerExplosion()
	{
	}

	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			Debug.LogWarning("Loaded explosion marker from disk, cleaning up");
			base.Invoke(new Action(this.DelayedDestroy), 3f);
		}
	}

	public void SetDuration(float newDuration)
	{
		this.duration = newDuration;
		if (base.IsInvoking(new Action(this.DelayedDestroy)))
		{
			base.CancelInvoke(new Action(this.DelayedDestroy));
		}
		base.Invoke(new Action(this.DelayedDestroy), this.duration * 60f);
	}
}