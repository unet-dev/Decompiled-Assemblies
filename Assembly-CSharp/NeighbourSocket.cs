using System;
using UnityEngine;

public class NeighbourSocket : Socket_Base
{
	public NeighbourSocket()
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