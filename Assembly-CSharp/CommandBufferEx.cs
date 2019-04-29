using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public static class CommandBufferEx
{
	public static void BlitArray(this CommandBuffer cb, Mesh blitMesh, RenderTargetIdentifier source, Material mat, int slice, int pass = 0)
	{
		cb.SetGlobalTexture("_Source", source);
		cb.SetGlobalFloat("_SourceMip", 0f);
		if (slice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)slice);
			cb.SetGlobalInt("_TargetSlice", slice);
		}
		cb.DrawMesh(blitMesh, Matrix4x4.identity, mat, 0, pass);
	}

	public static void BlitArray(this CommandBuffer cb, Mesh blitMesh, RenderTargetIdentifier source, Texture target, Material mat, int slice, int pass = 0)
	{
		cb.SetRenderTarget(target, 0, CubemapFace.PositiveX, -1);
		cb.SetGlobalTexture("_Source", source);
		cb.SetGlobalFloat("_SourceMip", 0f);
		if (slice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)slice);
			cb.SetGlobalInt("_TargetSlice", slice);
		}
		cb.DrawMesh(blitMesh, Matrix4x4.identity, mat, 0, pass);
	}

	public static void BlitArrayMip(this CommandBuffer cb, Mesh blitMesh, Texture source, int sourceMip, int sourceSlice, Texture target, int targetMip, int targetSlice, Material mat, int pass = 0)
	{
		int num = source.width >> (sourceMip & 31);
		int num1 = source.height >> (sourceMip & 31);
		Vector4 vector4 = new Vector4(1f / (float)num, 1f / (float)num1, (float)num, (float)num1);
		int num2 = target.width >> (targetMip & 31);
		int num3 = target.height >> (targetMip & 31);
		Vector4 vector41 = new Vector4(1f / (float)num2, 1f / (float)num3, (float)num2, (float)num3);
		cb.SetGlobalTexture("_Source", source);
		cb.SetGlobalVector("_Source_TexelSize", vector4);
		cb.SetGlobalVector("_Target_TexelSize", vector41);
		cb.SetGlobalFloat("_SourceMip", (float)sourceMip);
		if (sourceSlice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)sourceSlice);
			cb.SetGlobalInt("_TargetSlice", targetSlice);
		}
		cb.SetRenderTarget(target, targetMip, CubemapFace.PositiveX, -1);
		cb.DrawMesh(blitMesh, Matrix4x4.identity, mat, 0, pass);
	}

	public static void BlitMip(this CommandBuffer cb, Mesh blitMesh, Texture source, Texture target, int mip, int slice, Material mat, int pass = 0)
	{
		cb.BlitArrayMip(blitMesh, source, mip, slice, target, mip, slice, mat, pass);
	}
}