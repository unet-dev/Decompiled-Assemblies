using Mono.Cecil.Metadata;
using System;

namespace Mono.Cecil
{
	public sealed class PointerType : TypeSpecification
	{
		public override string FullName
		{
			get
			{
				return string.Concat(base.FullName, "*");
			}
		}

		public override bool IsPointer
		{
			get
			{
				return true;
			}
		}

		public override bool IsValueType
		{
			get
			{
				return false;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override string Name
		{
			get
			{
				return string.Concat(base.Name, "*");
			}
		}

		public PointerType(TypeReference type) : base(type)
		{
			Mixin.CheckType(type);
			this.etype = Mono.Cecil.Metadata.ElementType.Ptr;
		}
	}
}