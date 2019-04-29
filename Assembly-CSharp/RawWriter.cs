using System;
using System.Collections.Generic;
using System.IO;

public static class RawWriter
{
	public static void Write(IEnumerable<byte> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (byte datum in data)
				{
					binaryWriter.Write(datum);
				}
			}
		}
	}

	public static void Write(IEnumerable<int> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (int datum in data)
				{
					binaryWriter.Write(datum);
				}
			}
		}
	}

	public static void Write(IEnumerable<short> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (short datum in data)
				{
					binaryWriter.Write(datum);
				}
			}
		}
	}

	public static void Write(IEnumerable<float> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (float datum in data)
				{
					binaryWriter.Write(datum);
				}
			}
		}
	}
}