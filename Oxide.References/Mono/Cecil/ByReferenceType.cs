using Mono.Cecil.Metadata;
using System;

namespace Mono.Cecil
{
	public sealed class ByReferenceType : TypeSpecification
	{
		public override string FullName
		{
			get
			{
				return string.Concat(base.FullName, "&");
			}
		}

		public override bool IsByReference
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
				return string.Concat(base.Name, "&");
			}
		}

		public ByReferenceType(TypeReference type) : base(type)
		{
			Mixin.CheckType(type);
			this.etype = Mono.Cecil.Metadata.ElementType.ByRef;
		}
	}
}