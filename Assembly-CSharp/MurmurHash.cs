using System;
using System.IO;

public static class MurmurHash
{
	private const uint seed = 1337;

	private static uint mix(uint h)
	{
		h = h ^ h >> 16;
		h *= -2048144789;
		h = h ^ h >> 13;
		h *= -1028477387;
		h = h ^ h >> 16;
		return h;
	}

	private static uint rot(uint x, byte r)
	{
		return x << (r & 31) | x >> (32 - r & 31);
	}

	public static int Signed(Stream stream)
	{
		return (int)MurmurHash.Unsigned(stream);
	}

	public static uint Unsigned(Stream stream)
	{
		uint num = 1337;
		uint num1 = 0;
		uint length = 0;
		using (BinaryReader binaryReader = new BinaryReader(stream))
		{
			for (byte[] i = binaryReader.ReadBytes(4); i.Length != 0; i = binaryReader.ReadBytes(4))
			{
				length += (int)i.Length;
				switch ((int)i.Length)
				{
					case 1:
					{
						num1 = i[0];
						num1 = MurmurHash.rot(num1 * -862048943, 15);
						num = num ^ num1 * 461845907;
						break;
					}
					case 2:
					{
						num1 = (uint)(i[0] | i[1] << 8);
						num1 = MurmurHash.rot(num1 * -862048943, 15);
						num = num ^ num1 * 461845907;
						break;
					}
					case 3:
					{
						num1 = (uint)(i[0] | i[1] << 8 | i[2] << 16);
						num1 = MurmurHash.rot(num1 * -862048943, 15);
						num = num ^ num1 * 461845907;
						break;
					}
					case 4:
					{
						num1 = (uint)(i[0] | i[1] << 8 | i[2] << 16 | i[3] << 24);
						num1 = MurmurHash.rot(num1 * -862048943, 15);
						num = num ^ num1 * 461845907;
						num = MurmurHash.rot(num, 13);
						num = num * 5 + -430675100;
						break;
					}
				}
			}
		}
		num ^= length;
		num = MurmurHash.mix(num);
		return num;
	}
}