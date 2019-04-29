using System;

namespace Mono.Cecil
{
	public sealed class LinkedResource : Resource
	{
		internal byte[] hash;

		private string file;

		public string File
		{
			get
			{
				return this.file;
			}
			set
			{
				this.file = value;
			}
		}

		public byte[] Hash
		{
			get
			{
				return this.hash;
			}
		}

		public override Mono.Cecil.ResourceType ResourceType
		{
			get
			{
				return Mono.Cecil.ResourceType.Linked;
			}
		}

		public LinkedResource(string name, ManifestResourceAttributes flags) : base(name, flags)
		{
		}

		public LinkedResource(string name, ManifestResourceAttributes flags, string file) : base(name, flags)
		{
			this.file = file;
		}
	}
}