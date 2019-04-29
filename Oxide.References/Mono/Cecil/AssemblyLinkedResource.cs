using System;

namespace Mono.Cecil
{
	public sealed class AssemblyLinkedResource : Resource
	{
		private AssemblyNameReference reference;

		public AssemblyNameReference Assembly
		{
			get
			{
				return this.reference;
			}
			set
			{
				this.reference = value;
			}
		}

		public override Mono.Cecil.ResourceType ResourceType
		{
			get
			{
				return Mono.Cecil.ResourceType.AssemblyLinked;
			}
		}

		public AssemblyLinkedResource(string name, ManifestResourceAttributes flags) : base(name, flags)
		{
		}

		public AssemblyLinkedResource(string name, ManifestResourceAttributes flags, AssemblyNameReference reference) : base(name, flags)
		{
			this.reference = reference;
		}
	}
}