using Mono.Cecil.Metadata;
using System;

namespace Mono.Cecil
{
	public sealed class RequiredModifierType : TypeSpecification, IModifierType
	{
		private TypeReference modifier_type;

		public override bool ContainsGenericParameter
		{
			get
			{
				if (this.modifier_type.ContainsGenericParameter)
				{
					return true;
				}
				return base.ContainsGenericParameter;
			}
		}

		public override string FullName
		{
			get
			{
				return string.Concat(base.FullName, this.Suffix);
			}
		}

		public override bool IsRequiredModifier
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

		public TypeReference ModifierType
		{
			get
			{
				return JustDecompileGenerated_get_ModifierType();
			}
			set
			{
				JustDecompileGenerated_set_ModifierType(value);
			}
		}

		public TypeReference JustDecompileGenerated_get_ModifierType()
		{
			return this.modifier_type;
		}

		public void JustDecompileGenerated_set_ModifierType(TypeReference value)
		{
			this.modifier_type = value;
		}

		public override string Name
		{
			get
			{
				return string.Concat(base.Name, this.Suffix);
			}
		}

		private string Suffix
		{
			get
			{
				return string.Concat(" modreq(", this.modifier_type, ")");
			}
		}

		public RequiredModifierType(TypeReference modifierType, TypeReference type) : base(type)
		{
			Mixin.CheckModifier(modifierType, type);
			this.modifier_type = modifierType;
			this.etype = Mono.Cecil.Metadata.ElementType.CModReqD;
		}
	}
}