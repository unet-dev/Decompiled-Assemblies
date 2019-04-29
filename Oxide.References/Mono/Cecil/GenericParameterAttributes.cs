using System;

namespace Mono.Cecil
{
	[Flags]
	public enum GenericParameterAttributes : ushort
	{
		NonVariant = 0,
		Covariant = 1,
		Contravariant = 2,
		VarianceMask = 3,
		ReferenceTypeConstraint = 4,
		NotNullableValueTypeConstraint = 8,
		DefaultConstructorConstraint = 16,
		SpecialConstraintMask = 28
	}
}