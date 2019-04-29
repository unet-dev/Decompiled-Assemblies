using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Monument : TerrainPlacement
{
	public float Radius;

	public float Fade = 10f;

	public Monument()
	{
	}

	protected override void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		TextureData textureDatum = new TextureData(this.alphamap);
		Vector3 vector32 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 vector33 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 vector34 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 vector35 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.AlphaMap.ForEachParallel(vector32, vector33, vector34, vector35, (int x, int z) => {
			float single = TerrainMeta.AlphaMap.Coordinate(z);
			Vector3 vector3 = new Vector3(TerrainMeta.DenormalizeX(TerrainMeta.AlphaMap.Coordinate(x)), 0f, TerrainMeta.DenormalizeZ(single));
			Vector3 vector31 = worldToLocal.MultiplyPoint3x4(vector3) - this.offset;
			float single1 = Noise.Billow(vector3.x, vector3.z, 4, 0.005f, 0.25f * this.Fade, 2f, 0.5f);
			float single2 = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade + single1, vector31.Magnitude2D());
			if (single2 == 0f)
			{
				return;
			}
			float interpolatedVector = textureDatum.GetInterpolatedVector((vector31.x + this.extents.x) / this.size.x, (vector31.z + this.extents.z) / this.size.z).w;
			TerrainMeta.AlphaMap.SetAlpha(x, z, interpolatedVector, single2);
		});
	}

	protected override void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool flag = base.ShouldBiome(1);
		bool flag1 = base.ShouldBiome(2);
		bool flag2 = base.ShouldBiome(4);
		bool flag3 = base.ShouldBiome(8);
		if (!flag && !flag1 && !flag2 && !flag3)
		{
			return;
		}
		TextureData textureDatum = new TextureData(this.biomemap);
		Vector3 vector32 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 vector33 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 vector34 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 vector35 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.BiomeMap.ForEachParallel(vector32, vector33, vector34, vector35, (int x, int z) => {
			float single = TerrainMeta.BiomeMap.Coordinate(z);
			Vector3 vector3 = new Vector3(TerrainMeta.DenormalizeX(TerrainMeta.BiomeMap.Coordinate(x)), 0f, TerrainMeta.DenormalizeZ(single));
			Vector3 vector31 = worldToLocal.MultiplyPoint3x4(vector3) - this.offset;
			float single1 = Noise.Billow(vector3.x, vector3.z, 4, 0.005f, 0.25f * this.Fade, 2f, 0.5f);
			float single2 = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade + single1, vector31.Magnitude2D());
			if (single2 == 0f)
			{
				return;
			}
			Vector4 interpolatedVector = textureDatum.GetInterpolatedVector((vector31.x + this.extents.x) / this.size.x, (vector31.z + this.extents.z) / this.size.z);
			if (!flag)
			{
				interpolatedVector.x = 0f;
			}
			if (!flag1)
			{
				interpolatedVector.y = 0f;
			}
			if (!flag2)
			{
				interpolatedVector.z = 0f;
			}
			if (!flag3)
			{
				interpolatedVector.w = 0f;
			}
			TerrainMeta.BiomeMap.SetBiomeRaw(x, z, interpolatedVector, single2);
		});
	}

	protected override void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool flag = this.blendmap != null;
		Vector3 vector32 = localToWorld.MultiplyPoint3x4(Vector3.zero);
		TextureData textureDatum = new TextureData(this.heightmap);
		TextureData textureDatum1 = (flag ? new TextureData(this.blendmap) : new TextureData());
		Vector3 vector33 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 vector34 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 vector35 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 vector36 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.HeightMap.ForEachParallel(vector33, vector34, vector35, vector36, (int x, int z) => {
			float single = TerrainMeta.HeightMap.Coordinate(z);
			Vector3 vector3 = new Vector3(TerrainMeta.DenormalizeX(TerrainMeta.HeightMap.Coordinate(x)), 0f, TerrainMeta.DenormalizeZ(single));
			Vector3 vector31 = worldToLocal.MultiplyPoint3x4(vector3) - this.offset;
			float interpolatedVector = 1f;
			if (!flag)
			{
				float single1 = Noise.Billow(vector3.x, vector3.z, 4, 0.005f, 0.25f * this.Fade, 2f, 0.5f);
				interpolatedVector = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade + single1, vector31.Magnitude2D());
			}
			else
			{
				interpolatedVector = textureDatum1.GetInterpolatedVector((vector31.x + this.extents.x) / this.size.x, (vector31.z + this.extents.z) / this.size.z).w;
			}
			if (interpolatedVector == 0f)
			{
				return;
			}
			float single2 = TerrainMeta.NormalizeY(vector32.y + this.offset.y + textureDatum.GetInterpolatedHalf((vector31.x + this.extents.x) / this.size.x, (vector31.z + this.extents.z) / this.size.z) * this.size.y);
			single2 = Mathf.SmoothStep(TerrainMeta.HeightMap.GetHeight01(x, z), single2, interpolatedVector);
			TerrainMeta.HeightMap.SetHeight(x, z, single2);
		});
	}

	protected override void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool flag = base.ShouldSplat(1);
		bool flag1 = base.ShouldSplat(2);
		bool flag2 = base.ShouldSplat(4);
		bool flag3 = base.ShouldSplat(8);
		bool flag4 = base.ShouldSplat(16);
		bool flag5 = base.ShouldSplat(32);
		bool flag6 = base.ShouldSplat(64);
		bool flag7 = base.ShouldSplat(128);
		if (!flag && !flag1 && !flag2 && !flag3 && !flag4 && !flag5 && !flag6 && !flag7)
		{
			return;
		}
		TextureData textureDatum = new TextureData(this.splatmap0);
		TextureData textureDatum1 = new TextureData(this.splatmap1);
		Vector3 vector32 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 vector33 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 vector34 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 vector35 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.SplatMap.ForEachParallel(vector32, vector33, vector34, vector35, (int x, int z) => {
			GenerateCliffSplat.Process(x, z);
			float single = TerrainMeta.SplatMap.Coordinate(z);
			Vector3 vector3 = new Vector3(TerrainMeta.DenormalizeX(TerrainMeta.SplatMap.Coordinate(x)), 0f, TerrainMeta.DenormalizeZ(single));
			Vector3 vector31 = worldToLocal.MultiplyPoint3x4(vector3) - this.offset;
			float single1 = Noise.Billow(vector3.x, vector3.z, 4, 0.005f, 0.25f * this.Fade, 2f, 0.5f);
			float single2 = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade + single1, vector31.Magnitude2D());
			if (single2 == 0f)
			{
				return;
			}
			Vector4 interpolatedVector = textureDatum.GetInterpolatedVector((vector31.x + this.extents.x) / this.size.x, (vector31.z + this.extents.z) / this.size.z);
			Vector4 vector4 = textureDatum1.GetInterpolatedVector((vector31.x + this.extents.x) / this.size.x, (vector31.z + this.extents.z) / this.size.z);
			if (!flag)
			{
				interpolatedVector.x = 0f;
			}
			if (!flag1)
			{
				interpolatedVector.y = 0f;
			}
			if (!flag2)
			{
				interpolatedVector.z = 0f;
			}
			if (!flag3)
			{
				interpolatedVector.w = 0f;
			}
			if (!flag4)
			{
				vector4.x = 0f;
			}
			if (!flag5)
			{
				vector4.y = 0f;
			}
			if (!flag6)
			{
				vector4.z = 0f;
			}
			if (!flag7)
			{
				vector4.w = 0f;
			}
			TerrainMeta.SplatMap.SetSplatRaw(x, z, interpolatedVector, vector4, single2);
		});
	}

	protected override void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		TextureData textureDatum = new TextureData(this.topologymap);
		Vector3 vector32 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 vector33 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 vector34 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 vector35 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.TopologyMap.ForEachParallel(vector32, vector33, vector34, vector35, (int x, int z) => {
			GenerateCliffTopology.Process(x, z);
			float single = TerrainMeta.TopologyMap.Coordinate(z);
			Vector3 vector3 = new Vector3(TerrainMeta.DenormalizeX(TerrainMeta.TopologyMap.Coordinate(x)), 0f, TerrainMeta.DenormalizeZ(single));
			Vector3 vector31 = worldToLocal.MultiplyPoint3x4(vector3) - this.offset;
			int interpolatedInt = textureDatum.GetInterpolatedInt((vector31.x + this.extents.x) / this.size.x, (vector31.z + this.extents.z) / this.size.z);
			if (base.ShouldTopology(interpolatedInt))
			{
				TerrainMeta.TopologyMap.AddTopology(x, z, interpolatedInt & (int)this.TopologyMask);
			}
		});
	}

	protected override void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}

	protected void OnDrawGizmosSelected()
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.Radius);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.Radius - this.Fade);
	}
}