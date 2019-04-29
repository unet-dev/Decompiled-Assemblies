using System;
using UnityEngine;

public class BitUtility
{
	private const float float2byte = 255f;

	private const float byte2float = 0.003921569f;

	private const float float2short = 32766f;

	private const float short2float = 3.051944E-05f;

	public BitUtility()
	{
	}

	public static float Byte2Float(int b)
	{
		return (float)b * 0.003921569f;
	}

	public static float DecodeFloat(Color32 c)
	{
		Union32 union32 = new Union32()
		{
			b1 = c.r,
			b2 = c.g,
			b3 = c.b,
			b4 = c.a
		};
		return union32.f;
	}

	public static int DecodeInt(Color32 c)
	{
		Union32 union32 = new Union32()
		{
			b1 = c.r,
			b2 = c.g,
			b3 = c.b,
			b4 = c.a
		};
		return union32.i;
	}

	public static Vector3 DecodeNormal(Color c)
	{
		float single = c.a * 2f - 1f;
		float single1 = c.g * 2f - 1f;
		float single2 = Mathf.Sqrt(1f - Mathf.Clamp01(single * single + single1 * single1));
		return new Vector3(single, single2, single1);
	}

	public static short DecodeShort(Color32 c)
	{
		Union16 union16 = new Union16()
		{
			b1 = c.r,
			b2 = c.b
		};
		return union16.i;
	}

	public static Vector4 DecodeVector(Color32 c)
	{
		return new Vector4(BitUtility.Byte2Float((int)c.r), BitUtility.Byte2Float((int)c.g), BitUtility.Byte2Float((int)c.b), BitUtility.Byte2Float((int)c.a));
	}

	public static Vector2i DecodeVector2i(Color32 c)
	{
		return new Vector2i((int)(c.r - c.g), (int)(c.b - c.a));
	}

	public static Color32 EncodeFloat(float f)
	{
		Union32 union32 = new Union32()
		{
			f = f
		};
		return new Color32(union32.b1, union32.b2, union32.b3, union32.b4);
	}

	public static Color32 EncodeInt(int i)
	{
		Union32 union32 = new Union32()
		{
			i = i
		};
		return new Color32(union32.b1, union32.b2, union32.b3, union32.b4);
	}

	public static Color EncodeNormal(Vector3 n)
	{
		n = (n + Vector3.one) * 0.5f;
		return new Color(n.z, n.z, n.z, n.x);
	}

	public static Color32 EncodeShort(short i)
	{
		Union16 union16 = new Union16()
		{
			i = i
		};
		return new Color32(union16.b1, 0, union16.b2, 1);
	}

	public static Color32 EncodeVector(Vector4 v)
	{
		return new Color32(BitUtility.Float2Byte(v.x), BitUtility.Float2Byte(v.y), BitUtility.Float2Byte(v.z), BitUtility.Float2Byte(v.w));
	}

	public static Color32 EncodeVector2i(Vector2i v)
	{
		byte num = (byte)Mathf.Clamp(v.x, 0, 255);
		byte num1 = (byte)Mathf.Clamp(-v.x, 0, 255);
		byte num2 = (byte)Mathf.Clamp(v.y, 0, 255);
		byte num3 = (byte)Mathf.Clamp(-v.y, 0, 255);
		return new Color32(num, num1, num2, num3);
	}

	public static byte Float2Byte(float f)
	{
		Union32 union32 = new Union32()
		{
			f = f,
			b1 = 0
		};
		return (byte)(union32.f * 255f + 0.5f);
	}

	public static short Float2Short(float f)
	{
		Union32 union32 = new Union32()
		{
			f = f,
			b1 = 0
		};
		return (short)(union32.f * 32766f + 0.5f);
	}

	public static float Short2Float(int b)
	{
		return (float)b * 3.051944E-05f;
	}
}