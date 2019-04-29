using System;
using UnityEngine;

public class Socket_Specific : Socket_Base
{
	public bool useFemaleRotation = true;

	public string targetSocketName;

	public Socket_Specific()
	{
	}

	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion quaternion;
		Quaternion quaternion1 = target.socket.rotation;
		if (target.socket.male && target.socket.female)
		{
			quaternion1 = target.socket.rotation * Quaternion.Euler(180f, 0f, 180f);
		}
		Transform transforms = target.entity.transform;
		Vector3 vector3 = transforms.localToWorldMatrix.MultiplyPoint3x4(target.socket.position);
		if (!this.useFemaleRotation)
		{
			Vector3 vector31 = new Vector3(vector3.x, 0f, vector3.z);
			Vector3 vector32 = new Vector3(target.player.eyes.position.x, 0f, target.player.eyes.position.z);
			Vector3 vector33 = vector31 - vector32;
			quaternion = Quaternion.LookRotation(vector33.normalized) * quaternion1;
		}
		else
		{
			quaternion = transforms.rotation * quaternion1;
		}
		Construction.Placement placement = new Construction.Placement();
		Quaternion quaternion2 = quaternion * Quaternion.Inverse(this.rotation);
		placement.position = vector3 - (quaternion2 * this.position);
		placement.rotation = quaternion2;
		return placement;
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	public override bool TestTarget(Construction.Target target)
	{
		if (!base.TestTarget(target))
		{
			return false;
		}
		Socket_Specific_Female socketSpecificFemale = target.socket as Socket_Specific_Female;
		if (socketSpecificFemale == null)
		{
			return false;
		}
		return socketSpecificFemale.CanAccept(this);
	}
}