using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class Checksum
{
	private List<byte> values = new List<byte>();

	public Checksum()
	{
	}

	public void Add(float f, int bytes)
	{
		Union32 union32 = new Union32()
		{
			f = f
		};
		if (bytes >= 4)
		{
			this.values.Add(union32.b1);
		}
		if (bytes >= 3)
		{
			this.values.Add(union32.b2);
		}
		if (bytes >= 2)
		{
			this.values.Add(union32.b3);
		}
		if (bytes >= 1)
		{
			this.values.Add(union32.b4);
		}
	}

	public void Add(float f)
	{
		Union32 union32 = new Union32()
		{
			f = f
		};
		this.values.Add(union32.b1);
		this.values.Add(union32.b2);
		this.values.Add(union32.b3);
		this.values.Add(union32.b4);
	}

	public void Add(int i)
	{
		Union32 union32 = new Union32()
		{
			i = i
		};
		this.values.Add(union32.b1);
		this.values.Add(union32.b2);
		this.values.Add(union32.b3);
		this.values.Add(union32.b4);
	}

	public void Add(uint u)
	{
		Union32 union32 = new Union32()
		{
			u = u
		};
		this.values.Add(union32.b1);
		this.values.Add(union32.b2);
		this.values.Add(union32.b3);
		this.values.Add(union32.b4);
	}

	public void Add(short i)
	{
		Union16 union16 = new Union16()
		{
			i = i
		};
		this.values.Add(union16.b1);
		this.values.Add(union16.b2);
	}

	public void Add(ushort u)
	{
		Union16 union16 = new Union16()
		{
			u = u
		};
		this.values.Add(union16.b1);
		this.values.Add(union16.b2);
	}

	public void Add(byte b)
	{
		this.values.Add(b);
	}

	private string BytesToString(byte[] bytes)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < (int)bytes.Length; i++)
		{
			stringBuilder.Append(bytes[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	public void Clear()
	{
		this.values.Clear();
	}

	public string MD5()
	{
		byte[] numArray = (new MD5CryptoServiceProvider()).ComputeHash(this.values.ToArray());
		return this.BytesToString(numArray);
	}

	public string SHA1()
	{
		byte[] numArray = (new SHA1CryptoServiceProvider()).ComputeHash(this.values.ToArray());
		return this.BytesToString(numArray);
	}

	public override string ToString()
	{
		return this.BytesToString(this.values.ToArray());
	}
}