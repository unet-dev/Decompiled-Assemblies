using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class QuaternionEx
	{
		public static Quaternion AlignToNormal(this Quaternion rot, Vector3 normal)
		{
			return Quaternion.FromToRotation(Vector3.up, normal) * rot;
		}

		public static Quaternion EnsureValid(this Quaternion rot, float epsilon = 1.401298E-45f)
		{
			if (Quaternion.Dot(rot, rot) < epsilon)
			{
				return Quaternion.identity;
			}
			return rot;
		}

		public static Quaternion LookRotationForcedUp(Vector3 forward, Vector3 up)
		{
			if (forward == up)
			{
				return Quaternion.LookRotation(up);
			}
			Vector3 vector3 = Vector3.Cross(forward, up);
			forward = Vector3.Cross(up, vector3);
			return Quaternion.LookRotation(forward, up);
		}

		public static Quaternion LookRotationGradient(Vector3 normal, Vector3 up)
		{
			return QuaternionEx.LookRotationForcedUp(Vector3.Cross(normal, (normal == Vector3.up ? Vector3.forward : Vector3.Cross(normal, Vector3.up))), up);
		}

		public static Quaternion LookRotationNormal(Vector3 normal, Vector3 up = null)
		{
			if (up != Vector3.zero)
			{
				return QuaternionEx.LookRotationForcedUp(up, normal);
			}
			if (normal == Vector3.up)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.forward, normal);
			}
			if (normal == Vector3.down)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.back, normal);
			}
			if (normal.y == 0f)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.up, normal);
			}
			Vector3 vector3 = Vector3.Cross(normal, Vector3.up);
			return QuaternionEx.LookRotationForcedUp(-Vector3.Cross(normal, vector3), normal);
		}

		public static Quaternion LookRotationWithOffset(Vector3 offset, Vector3 forward, Vector3 up)
		{
			return Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Inverse(Quaternion.LookRotation(offset, Vector3.up));
		}
	}
}