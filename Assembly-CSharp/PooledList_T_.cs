using Facepunch;
using System;
using System.Collections.Generic;

public class PooledList<T>
{
	public List<T> data;

	public PooledList()
	{
	}

	public void Alloc()
	{
		if (this.data == null)
		{
			this.data = Pool.GetList<T>();
		}
	}

	public void Clear()
	{
		if (this.data != null)
		{
			this.data.Clear();
		}
	}

	public void Free()
	{
		if (this.data != null)
		{
			Pool.FreeList<T>(ref this.data);
		}
	}
}