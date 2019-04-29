using System;
using UnityEngine;

public class StabilitySocket : Socket_Base
{
	[Range(0f, 1f)]
	public float support = 1f;

	public StabilitySocket()
	{
	}

	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		OBB selectBounds = base.GetSelectBounds(position, rotation);
		return selectBounds.Intersects(socket.GetSelectBounds(socketPosition, socketRotation));
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	public override bool TestTarget(Construction.Target target)
	{
		return false;
	}
}