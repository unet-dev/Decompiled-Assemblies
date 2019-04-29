using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class ByteMap
{
	[SerializeField]
	private int size;

	[SerializeField]
	private int bytes;

	[SerializeField]
	private byte[] values;

	public uint this[int x, int y]
	{
		get
		{
			uint num;
			uint num1;
			int num2 = y * this.bytes * this.size + x * this.bytes;
			switch (this.bytes)
			{
				case 1:
				{
					return this.values[num2];
				}
				case 2:
				{
					byte num3 = this.values[num2];
					num = this.values[num2 + 1];
					return num3 << 8 | num;
				}
				case 3:
				{
					byte num4 = this.values[num2];
					num = this.values[num2 + 1];
					num1 = this.values[num2 + 2];
					return num4 << 16 | num << 8 | num1;
				}
			}
			byte num5 = this.values[num2];
			num = this.values[num2 + 1];
			num1 = this.values[num2 + 2];
			uint num6 = this.values[num2 + 3];
			return num5 << 24 | num << 16 | num1 << 8 | num6;
		}
		set
		{
			int num = y * this.bytes * this.size + x * this.bytes;
			switch (this.bytes)
			{
				case 1:
				{
					this.values[num] = (byte)(value & 255);
					return;
				}
				case 2:
				{
					this.values[num] = (byte)(value >> 8 & 255);
					this.values[num + 1] = (byte)(value & 255);
					return;
				}
				case 3:
				{
					this.values[num] = (byte)(value >> 16 & 255);
					this.values[num + 1] = (byte)(value >> 8 & 255);
					this.values[num + 2] = (byte)(value & 255);
					return;
				}
			}
			this.values[num] = (byte)(value >> 24 & 255);
			this.values[num + 1] = (byte)(value >> 16 & 255);
			this.values[num + 2] = (byte)(value >> 8 & 255);
			this.values[num + 3] = (byte)(value & 255);
		}
	}

	public int Size
	{
		get
		{
			return this.size;
		}
	}

	public ByteMap(int size, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		this.values = new byte[bytes * size * size];
	}

	public ByteMap(int size, byte[] values, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		this.values = values;
	}
}