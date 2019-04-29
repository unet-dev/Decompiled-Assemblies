using System;
using UnityEngine;

public class ConstructionSocket : Socket_Base
{
	public ConstructionSocket.Type socketType;

	public int rotationDegrees;

	public int rotationOffset;

	public bool restrictPlacementAngle;

	public float faceAngle;

	public float angleAllowed = 150f;

	[Range(0f, 1f)]
	public float support = 1f;

	public ConstructionSocket()
	{
	}

	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		Matrix4x4 matrix4x4 = Matrix4x4.TRS(position, rotation, Vector3.one);
		Matrix4x4 matrix4x41 = Matrix4x4.TRS(socketPosition, socketRotation, Vector3.one);
		if (Vector3.Distance(matrix4x4.MultiplyPoint3x4(this.worldPosition), matrix4x41.MultiplyPoint3x4(socket.worldPosition)) > 0.01f)
		{
			return false;
		}
		Vector3 vector3 = matrix4x4.MultiplyVector(this.worldRotation * Vector3.forward);
		Vector3 vector31 = matrix4x41.MultiplyVector(socket.worldRotation * Vector3.forward);
		float single = Vector3.Angle(vector3, vector31);
		if (this.male && this.female)
		{
			single = Mathf.Min(single, Vector3.Angle(-vector3, vector31));
		}
		if (socket.male && socket.female)
		{
			single = Mathf.Min(single, Vector3.Angle(vector3, -vector31));
		}
		if (single > 1f)
		{
			return false;
		}
		return true;
	}

	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		if (!target.entity || !target.entity.transform)
		{
			return null;
		}
		Vector3 worldPosition = target.GetWorldPosition();
		Quaternion worldRotation = target.GetWorldRotation(true);
		if (this.rotationDegrees > 0)
		{
			Construction.Placement placement = new Construction.Placement();
			float single = Single.MaxValue;
			float single1 = 0f;
			for (int i = 0; i < 360; i += this.rotationDegrees)
			{
				Quaternion quaternion = Quaternion.Euler(0f, (float)(this.rotationOffset + i), 0f);
				Vector3 vector3 = target.ray.direction;
				Vector3 vector31 = (quaternion * worldRotation) * Vector3.up;
				float single2 = Vector3.Angle(vector3, vector31);
				if (single2 < single)
				{
					single = single2;
					single1 = (float)i;
				}
			}
			for (int j = 0; j < 360; j += this.rotationDegrees)
			{
				Quaternion quaternion1 = worldRotation * Quaternion.Inverse(this.rotation);
				Quaternion quaternion2 = Quaternion.Euler(target.rotation);
				Quaternion quaternion3 = Quaternion.Euler(0f, (float)(this.rotationOffset + j) + single1, 0f);
				Quaternion quaternion4 = (quaternion2 * quaternion3) * quaternion1;
				placement.position = worldPosition - (quaternion4 * this.position);
				placement.rotation = quaternion4;
				if (this.CheckSocketMods(placement))
				{
					return placement;
				}
			}
		}
		Construction.Placement placement1 = new Construction.Placement();
		Quaternion quaternion5 = worldRotation * Quaternion.Inverse(this.rotation);
		placement1.position = worldPosition - (quaternion5 * this.position);
		placement1.rotation = quaternion5;
		if (!this.TestRestrictedAngles(worldPosition, worldRotation, target))
		{
			return null;
		}
		return placement1;
	}

	public override bool IsCompatible(Socket_Base socket)
	{
		if (!base.IsCompatible(socket))
		{
			return false;
		}
		ConstructionSocket constructionSocket = socket as ConstructionSocket;
		if (constructionSocket == null)
		{
			return false;
		}
		if (constructionSocket.socketType == ConstructionSocket.Type.None || this.socketType == ConstructionSocket.Type.None)
		{
			return false;
		}
		if (constructionSocket.socketType != this.socketType)
		{
			return false;
		}
		return true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.6f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	private void OnDrawGizmosSelected()
	{
		if (this.female)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
		}
	}

	public bool TestRestrictedAngles(Vector3 suggestedPos, Quaternion suggestedAng, Construction.Target target)
	{
		if (this.restrictPlacementAngle)
		{
			Quaternion quaternion = Quaternion.Euler(0f, this.faceAngle, 0f) * suggestedAng;
			float single = target.ray.direction.XZ3D().DotDegrees(quaternion * Vector3.forward);
			if (single > this.angleAllowed * 0.5f)
			{
				return false;
			}
			if (single < this.angleAllowed * -0.5f)
			{
				return false;
			}
		}
		return true;
	}

	public override bool TestTarget(Construction.Target target)
	{
		if (!base.TestTarget(target))
		{
			return false;
		}
		return this.IsCompatible(target.socket);
	}

	public enum Type
	{
		None = 0,
		Foundation = 1,
		Floor = 2,
		Doorway = 4,
		Wall = 5,
		Block = 6,
		Window = 11,
		Shutters = 12,
		WallFrame = 13,
		FloorFrame = 14,
		WindowDressing = 15,
		DoorDressing = 16
	}
}