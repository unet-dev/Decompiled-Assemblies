using System;
using UnityEngine;

public class DropBox : Mailbox
{
	public DropBox()
	{
	}

	public bool PlayerBehind(BasePlayer player)
	{
		Vector3 vector3 = base.transform.forward;
		Vector3 vector31 = player.transform.position - base.transform.position;
		return Vector3.Dot(vector3, vector31.normalized) <= -0.3f;
	}

	public bool PlayerInfront(BasePlayer player)
	{
		Vector3 vector3 = base.transform.forward;
		Vector3 vector31 = player.transform.position - base.transform.position;
		return Vector3.Dot(vector3, vector31.normalized) >= 0.7f;
	}

	public override bool PlayerIsOwner(BasePlayer player)
	{
		return this.PlayerBehind(player);
	}
}