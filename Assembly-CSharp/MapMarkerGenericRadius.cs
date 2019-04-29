using Network;
using System;
using UnityEngine;

public class MapMarkerGenericRadius : MapMarker
{
	public float radius;

	public Color color1;

	public Color color2;

	public float alpha;

	public MapMarkerGenericRadius()
	{
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("MapMarkerGenericRadius.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public void SendUpdate(bool fullUpdate = true)
	{
		float single = this.color1.a;
		Vector3 vector3 = new Vector3(this.color1.r, this.color1.g, this.color1.b);
		Vector3 vector31 = new Vector3(this.color2.r, this.color2.g, this.color2.b);
		base.ClientRPC<Vector3, float, Vector3, float, float>(null, "MarkerUpdate", vector3, single, vector31, this.alpha, this.radius);
	}
}