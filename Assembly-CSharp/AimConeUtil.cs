using System;
using UnityEngine;

public class AimConeUtil
{
	public AimConeUtil()
	{
	}

	public static Quaternion GetAimConeQuat(float aimCone)
	{
		Vector3 vector3 = UnityEngine.Random.insideUnitSphere;
		return Quaternion.Euler(vector3.x * aimCone * 0.5f, vector3.y * aimCone * 0.5f, 0f);
	}

	public static Vector3 GetModifiedAimConeDirection(float aimCone, Vector3 inputVec, bool anywhereInside = true)
	{
		Quaternion quaternion = Quaternion.LookRotation(inputVec);
		Vector2 vector2 = (anywhereInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized);
		return (quaternion * Quaternion.Euler(vector2.x * aimCone * 0.5f, vector2.y * aimCone * 0.5f, 0f)) * Vector3.forward;
	}
}