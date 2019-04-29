using System;
using System.Runtime.InteropServices;
using System.Security;

[SuppressUnmanagedCodeSecurity]
public static class NativeNoise
{
	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="billow_32", ExactSpelling=false)]
	public static extern float Billow(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="billow_iq_32", ExactSpelling=false)]
	public static extern float BillowIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="billow_warp_32", ExactSpelling=false)]
	public static extern float BillowWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="jordan_32", ExactSpelling=false)]
	public static extern float Jordan(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp, float damp, float damp_scale);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="ridge_32", ExactSpelling=false)]
	public static extern float Ridge(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="ridge_iq_32", ExactSpelling=false)]
	public static extern float RidgeIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="ridge_warp_32", ExactSpelling=false)]
	public static extern float RidgeWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="sharp_32", ExactSpelling=false)]
	public static extern float Sharp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="sharp_iq_32", ExactSpelling=false)]
	public static extern float SharpIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="sharp_warp_32", ExactSpelling=false)]
	public static extern float SharpWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="snoise1_32", ExactSpelling=false)]
	public static extern float Simplex1D(float x);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="sdnoise1_32", ExactSpelling=false)]
	public static extern float Simplex1D(float x, out float dx);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="snoise2_32", ExactSpelling=false)]
	public static extern float Simplex2D(float x, float y);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="sdnoise2_32", ExactSpelling=false)]
	public static extern float Simplex2D(float x, float y, out float dx, out float dy);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="turbulence_32", ExactSpelling=false)]
	public static extern float Turbulence(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="turbulence_iq_32", ExactSpelling=false)]
	public static extern float TurbulenceIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="turbulence_warp_32", ExactSpelling=false)]
	public static extern float TurbulenceWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);
}