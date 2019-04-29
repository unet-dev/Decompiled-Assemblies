using System;

namespace Mono.Cecil
{
	public class ModuleReference : IMetadataScope, IMetadataTokenProvider
	{
		private string name;

		internal Mono.Cecil.MetadataToken token;

		public virtual Mono.Cecil.MetadataScopeType MetadataScopeType
		{
			get
			{
				return Mono.Cecil.MetadataScopeType.ModuleReference;
			}
		}

		public Mono.Cecil.MetadataToken MetadataToken
		{
			get
			{
				return this.token;
			}
			set
			{
				this.token = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal ModuleReference()
		{
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.ModuleRef);
		}

		public ModuleReference(string name) : this()
		{
			this.name = name;
		}

		public override string ToString()
		{
			return this.name;
		}
	}
}