using System;
using UnityEngine;

public struct TextureData
{
	public int width;

	public int height;

	public Color32[] colors;

	public TextureData(Texture2D tex)
	{
		if (tex == null)
		{
			this.width = 0;
			this.height = 0;
			this.colors = null;
			return;
		}
		this.width = tex.width;
		this.height = tex.height;
		this.colors = tex.GetPixels32();
	}

	public Color32 GetColor(int x, int y)
	{
		return this.colors[y * this.width + x];
	}

	public float GetFloat(int x, int y)
	{
		return BitUtility.DecodeFloat(this.GetColor(x, y));
	}

	public float GetHalf(int x, int y)
	{
		return BitUtility.Short2Float(this.GetShort(x, y));
	}

	public int GetInt(int x, int y)
	{
		return BitUtility.DecodeInt(this.GetColor(x, y));
	}

	public Color32 GetInterpolatedColor(float x, float y)
	{
		float single = x * (float)(this.width - 1);
		float single1 = y * (float)(this.height - 1);
		int num = Mathf.Clamp((int)single, 1, this.width - 2);
		int num1 = Mathf.Clamp((int)single1, 1, this.height - 2);
		int num2 = Mathf.Min(num + 1, this.width - 2);
		int num3 = Mathf.Min(num1 + 1, this.height - 2);
		Color color = this.GetColor(num, num1);
		Color color1 = this.GetColor(num2, num1);
		Color color2 = this.GetColor(num, num3);
		Color color3 = this.GetColor(num2, num3);
		float single2 = single - (float)num;
		float single3 = single1 - (float)num1;
		return Color.Lerp(Color.Lerp(color, color1, single2), Color.Lerp(color2, color3, single2), single3);
	}

	public float GetInterpolatedFloat(float x, float y)
	{
		float single = x * (float)(this.width - 1);
		float single1 = y * (float)(this.height - 1);
		int num = Mathf.Clamp((int)single, 1, this.width - 2);
		int num1 = Mathf.Clamp((int)single1, 1, this.height - 2);
		int num2 = Mathf.Min(num + 1, this.width - 2);
		int num3 = Mathf.Min(num1 + 1, this.height - 2);
		float single2 = this.GetFloat(num, num1);
		float single3 = this.GetFloat(num2, num1);
		float num4 = this.GetFloat(num, num3);
		float single4 = this.GetFloat(num2, num3);
		float single5 = single - (float)num;
		float single6 = single1 - (float)num1;
		float single7 = Mathf.Lerp(single2, single3, single5);
		float single8 = Mathf.Lerp(num4, single4, single5);
		return Mathf.Lerp(single7, single8, single6);
	}

	public float GetInterpolatedHalf(float x, float y)
	{
		float single = x * (float)(this.width - 1);
		float single1 = y * (float)(this.height - 1);
		int num = Mathf.Clamp((int)single, 1, this.width - 2);
		int num1 = Mathf.Clamp((int)single1, 1, this.height - 2);
		int num2 = Mathf.Min(num + 1, this.width - 2);
		int num3 = Mathf.Min(num1 + 1, this.height - 2);
		float half = this.GetHalf(num, num1);
		float half1 = this.GetHalf(num2, num1);
		float half2 = this.GetHalf(num, num3);
		float single2 = this.GetHalf(num2, num3);
		float single3 = single - (float)num;
		float single4 = single1 - (float)num1;
		float single5 = Mathf.Lerp(half, half1, single3);
		float single6 = Mathf.Lerp(half2, single2, single3);
		return Mathf.Lerp(single5, single6, single4);
	}

	public int GetInterpolatedInt(float x, float y)
	{
		float single = x * (float)(this.width - 1);
		float single1 = y * (float)(this.height - 1);
		int num = Mathf.Clamp(Mathf.RoundToInt(single), 1, this.width - 2);
		int num1 = Mathf.Clamp(Mathf.RoundToInt(single1), 1, this.height - 2);
		return this.GetInt(num, num1);
	}

	public Vector3 GetInterpolatedNormal(float x, float y)
	{
		float single = x * (float)(this.width - 1);
		float single1 = y * (float)(this.height - 1);
		int num = Mathf.Clamp((int)single, 1, this.width - 2);
		int num1 = Mathf.Clamp((int)single1, 1, this.height - 2);
		int num2 = Mathf.Min(num + 1, this.width - 2);
		int num3 = Mathf.Min(num1 + 1, this.height - 2);
		Vector3 normal = this.GetNormal(num, num1);
		Vector3 vector3 = this.GetNormal(num2, num1);
		Vector3 normal1 = this.GetNormal(num, num3);
		Vector3 vector31 = this.GetNormal(num2, num3);
		float single2 = single - (float)num;
		float single3 = single1 - (float)num1;
		Vector3 vector32 = Vector3.Lerp(normal, vector3, single2);
		Vector3 vector33 = Vector3.Lerp(normal1, vector31, single2);
		return Vector3.Lerp(vector32, vector33, single3);
	}

	public int GetInterpolatedShort(float x, float y)
	{
		float single = x * (float)(this.width - 1);
		float single1 = y * (float)(this.height - 1);
		int num = Mathf.Clamp(Mathf.RoundToInt(single), 1, this.width - 2);
		int num1 = Mathf.Clamp(Mathf.RoundToInt(single1), 1, this.height - 2);
		return this.GetShort(num, num1);
	}

	public Vector4 GetInterpolatedVector(float x, float y)
	{
		float single = x * (float)(this.width - 1);
		float single1 = y * (float)(this.height - 1);
		int num = Mathf.Clamp((int)single, 1, this.width - 2);
		int num1 = Mathf.Clamp((int)single1, 1, this.height - 2);
		int num2 = Mathf.Min(num + 1, this.width - 2);
		int num3 = Mathf.Min(num1 + 1, this.height - 2);
		Vector4 vector = this.GetVector(num, num1);
		Vector4 vector4 = this.GetVector(num2, num1);
		Vector4 vector1 = this.GetVector(num, num3);
		Vector4 vector41 = this.GetVector(num2, num3);
		float single2 = single - (float)num;
		float single3 = single1 - (float)num1;
		Vector4 vector42 = Vector4.Lerp(vector, vector4, single2);
		Vector4 vector43 = Vector4.Lerp(vector1, vector41, single2);
		return Vector4.Lerp(vector42, vector43, single3);
	}

	public Vector3 GetNormal(int x, int y)
	{
		return BitUtility.DecodeNormal(this.GetColor(x, y));
	}

	public int GetShort(int x, int y)
	{
		return BitUtility.DecodeShort(this.GetColor(x, y));
	}

	public Vector4 GetVector(int x, int y)
	{
		return BitUtility.DecodeVector(this.GetColor(x, y));
	}
}