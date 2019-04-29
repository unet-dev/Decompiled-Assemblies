using Mono.Cecil.PE;
using System;

namespace Mono.Cecil.Metadata
{
	internal sealed class ResourceBuffer : ByteBuffer
	{
		public ResourceBuffer() : base(0)
		{
		}

		public uint AddResource(byte[] resource)
		{
			int num = this.position;
			base.WriteInt32((int)resource.Length);
			base.WriteBytes(resource);
			return (uint)num;
		}
	}
}