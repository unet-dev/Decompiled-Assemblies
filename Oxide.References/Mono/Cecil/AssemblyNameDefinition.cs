using Mono;
using System;

namespace Mono.Cecil
{
	public sealed class AssemblyNameDefinition : AssemblyNameReference
	{
		public override byte[] Hash
		{
			get
			{
				return Empty<byte>.Array;
			}
		}

		internal AssemblyNameDefinition()
		{
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.Assembly, 1);
		}

		public AssemblyNameDefinition(string name, System.Version version) : base(name, version)
		{
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.Assembly, 1);
		}
	}
}