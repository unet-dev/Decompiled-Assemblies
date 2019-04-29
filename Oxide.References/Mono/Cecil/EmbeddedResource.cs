using System;
using System.IO;

namespace Mono.Cecil
{
	public sealed class EmbeddedResource : Resource
	{
		private readonly MetadataReader reader;

		private uint? offset;

		private byte[] data;

		private Stream stream;

		public override Mono.Cecil.ResourceType ResourceType
		{
			get
			{
				return Mono.Cecil.ResourceType.Embedded;
			}
		}

		public EmbeddedResource(string name, ManifestResourceAttributes attributes, byte[] data) : base(name, attributes)
		{
			this.data = data;
		}

		public EmbeddedResource(string name, ManifestResourceAttributes attributes, Stream stream) : base(name, attributes)
		{
			this.stream = stream;
		}

		internal EmbeddedResource(string name, ManifestResourceAttributes attributes, uint offset, MetadataReader reader) : base(name, attributes)
		{
			this.offset = new uint?(offset);
			this.reader = reader;
		}

		public byte[] GetResourceData()
		{
			if (this.stream != null)
			{
				return EmbeddedResource.ReadStream(this.stream);
			}
			if (this.data != null)
			{
				return this.data;
			}
			if (!this.offset.HasValue)
			{
				throw new InvalidOperationException();
			}
			return this.reader.GetManagedResourceStream(this.offset.Value).ToArray();
		}

		public Stream GetResourceStream()
		{
			if (this.stream != null)
			{
				return this.stream;
			}
			if (this.data != null)
			{
				return new MemoryStream(this.data);
			}
			if (!this.offset.HasValue)
			{
				throw new InvalidOperationException();
			}
			return this.reader.GetManagedResourceStream(this.offset.Value);
		}

		private static byte[] ReadStream(Stream stream)
		{
			int num;
			if (!stream.CanSeek)
			{
				byte[] numArray = new byte[8192];
				MemoryStream memoryStream = new MemoryStream();
				while (true)
				{
					int num1 = stream.Read(numArray, 0, (int)numArray.Length);
					num = num1;
					if (num1 <= 0)
					{
						break;
					}
					memoryStream.Write(numArray, 0, num);
				}
				return memoryStream.ToArray();
			}
			int length = (int)stream.Length;
			byte[] numArray1 = new byte[length];
			int num2 = 0;
			while (true)
			{
				int num3 = stream.Read(numArray1, num2, length - num2);
				num = num3;
				if (num3 <= 0)
				{
					break;
				}
				num2 += num;
			}
			return numArray1;
		}
	}
}