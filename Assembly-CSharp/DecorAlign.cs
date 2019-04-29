using System;
using UnityEngine;

public class DecorAlign : DecorComponent
{
	public float NormalAlignment = 1f;

	public float GradientAlignment = 1f;

	public Vector3 SlopeOffset = Vector3.zero;

	public Vector3 SlopeScale = Vector3.one;

	public DecorAlign()
	{
	}

	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		Vector3 normal = TerrainMeta.HeightMap.GetNormal(pos);
		Vector3 vector3 = (normal == Vector3.up ? Vector3.forward : Vector3.Cross(normal, Vector3.up));
		Vector3 vector31 = Vector3.Cross(normal, vector3);
		if (this.SlopeOffset != Vector3.zero || this.SlopeScale != Vector3.one)
		{
			float slope01 = TerrainMeta.HeightMap.GetSlope01(pos);
			if (this.SlopeOffset != Vector3.zero)
			{
				Vector3 slopeOffset = this.SlopeOffset * slope01;
				pos = pos + (slopeOffset.x * vector3);
				pos = pos + (slopeOffset.y * normal);
				pos = pos - (slopeOffset.z * vector31);
			}
			if (this.SlopeScale != Vector3.one)
			{
				Vector3 vector32 = Vector3.Lerp(Vector3.one, Vector3.one + (Quaternion.Inverse(rot) * (this.SlopeScale - Vector3.one)), slope01);
				scale.x *= vector32.x;
				scale.y *= vector32.y;
				scale.z *= vector32.z;
			}
		}
		Vector3 vector33 = Vector3.Lerp(rot * Vector3.up, normal, this.NormalAlignment);
		Quaternion quaternion = QuaternionEx.LookRotationForcedUp(Vector3.Lerp(rot * Vector3.forward, vector31, this.GradientAlignment), vector33);
		rot = quaternion * rot;
	}
}