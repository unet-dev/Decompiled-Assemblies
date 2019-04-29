using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public abstract class TerrainMap<T> : TerrainMap
where T : struct
{
	internal T[] src;

	internal T[] dst;

	protected TerrainMap()
	{
	}

	public int BytesPerElement()
	{
		return Marshal.SizeOf(typeof(T));
	}

	public void FromByteArray(byte[] dat)
	{
		Buffer.BlockCopy(dat, 0, this.dst, 0, (int)dat.Length);
	}

	public long GetMemoryUsage()
	{
		return (long)this.BytesPerElement() * (long)((int)this.src.Length);
	}

	public void Pop()
	{
		if (this.src == this.dst)
		{
			return;
		}
		Array.Copy(this.dst, this.src, (int)this.src.Length);
		this.dst = this.src;
	}

	public void Push()
	{
		if (this.src != this.dst)
		{
			return;
		}
		this.dst = (T[])this.src.Clone();
	}

	public byte[] ToByteArray()
	{
		byte[] numArray = new byte[this.BytesPerElement() * (int)this.src.Length];
		Buffer.BlockCopy(this.src, 0, numArray, 0, (int)numArray.Length);
		return numArray;
	}

	public IEnumerable<T> ToEnumerable()
	{
		return this.src.Cast<T>();
	}
}