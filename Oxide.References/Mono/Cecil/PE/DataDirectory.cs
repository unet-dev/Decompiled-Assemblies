using System;

namespace Mono.Cecil.PE
{
	internal struct DataDirectory
	{
		public readonly uint VirtualAddress;

		public readonly uint Size;

		public bool IsZero
		{
			get
			{
				if (this.VirtualAddress != 0)
				{
					return false;
				}
				return this.Size == 0;
			}
		}

		public DataDirectory(uint rva, uint size)
		{
			this.VirtualAddress = rva;
			this.Size = size;
		}
	}
}