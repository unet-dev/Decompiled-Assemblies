using System;

namespace Steamworks.Data
{
	public struct Image
	{
		public uint Width;

		public uint Height;

		public byte[] Data;

		public Color GetPixel(int x, int y)
		{
			if ((x < 0 ? true : (long)x >= (ulong)this.Width))
			{
				throw new Exception("x out of bounds");
			}
			if ((y < 0 ? true : (long)y >= (ulong)this.Height))
			{
				throw new Exception("y out of bounds");
			}
			Color data = new Color();
			long num = ((long)y * (ulong)this.Width + (long)x) * (long)4;
			data.r = this.Data[checked((IntPtr)num)];
			data.g = this.Data[checked((IntPtr)(num + (long)1))];
			data.b = this.Data[checked((IntPtr)(num + (long)2))];
			data.a = this.Data[checked((IntPtr)(num + (long)3))];
			return data;
		}

		public override string ToString()
		{
			string str = String.Format("{0}x{1} ({2}bytes)", this.Width, this.Height, (int)this.Data.Length);
			return str;
		}
	}
}