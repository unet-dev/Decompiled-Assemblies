using System;

public class RadixSorter
{
	private uint[] histogram;

	private uint[] offset;

	public RadixSorter()
	{
		this.histogram = new uint[768];
		this.offset = new uint[768];
	}

	public void SortU24(uint[] values, uint[] remap, uint[] remapTemp, uint num)
	{
		uint num1;
		for (int i = 0; i < 768; i++)
		{
			this.histogram[i] = 0;
		}
		for (uint j = 0; j < num; j++)
		{
			uint num2 = values[j];
			this.histogram[num2 & 255]++;
			this.histogram[256 + (num2 >> 8 & 255)]++;
			this.histogram[512 + (num2 >> 16 & 255)]++;
		}
		int num3 = 0;
		uint num4 = (uint)num3;
		this.offset[512] = (uint)num3;
		uint num5 = num4;
		num4 = num5;
		this.offset[256] = num5;
		this.offset[0] = num4;
		uint num6 = 0;
		uint num7 = 256;
		uint num8 = 512;
		while (num6 < 255)
		{
			this.offset[num6 + 1] = this.offset[num6] + this.histogram[num6];
			this.offset[num7 + 1] = this.offset[num7] + this.histogram[num7];
			this.offset[num8 + 1] = this.offset[num8] + this.histogram[num8];
			num6++;
			num7++;
			num8++;
		}
		for (uint k = 0; k < num; k++)
		{
			ref uint numPointer = ref this.offset[values[k] & 255];
			num4 = numPointer;
			numPointer = num4 + 1;
			remapTemp[num4] = k;
		}
		for (uint l = 0; l < num; l++)
		{
			num1 = remapTemp[l];
			ref uint numPointer1 = ref this.offset[256 + (values[num1] >> 8 & 255)];
			num4 = numPointer1;
			numPointer1 = num4 + 1;
			remap[num4] = num1;
		}
		for (uint m = 0; m < num; m++)
		{
			num1 = remap[m];
			ref uint numPointer2 = ref this.offset[512 + (values[num1] >> 16 & 255)];
			num4 = numPointer2;
			numPointer2 = num4 + 1;
			remapTemp[num4] = num1;
		}
		for (uint n = 0; n < num; n++)
		{
			remap[n] = remapTemp[n];
		}
	}

	public void SortU8(uint[] values, uint[] remap, uint num)
	{
		for (int i = 0; i < 256; i++)
		{
			this.histogram[i] = 0;
		}
		for (uint j = 0; j < num; j++)
		{
			this.histogram[values[j] & 255]++;
		}
		this.offset[0] = 0;
		for (uint k = 0; k < 255; k++)
		{
			this.offset[k + 1] = this.offset[k] + this.histogram[k];
		}
		for (uint l = 0; l < num; l++)
		{
			ref uint numPointer = ref this.offset[values[l] & 255];
			uint num1 = numPointer;
			numPointer = num1 + 1;
			remap[num1] = l;
		}
	}
}