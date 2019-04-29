using System;

namespace Mono.Cecil
{
	public class FieldReference : MemberReference
	{
		private TypeReference field_type;

		public override bool ContainsGenericParameter
		{
			get
			{
				if (this.field_type.ContainsGenericParameter)
				{
					return true;
				}
				return base.ContainsGenericParameter;
			}
		}

		public TypeReference FieldType
		{
			get
			{
				return this.field_type;
			}
			set
			{
				this.field_type = value;
			}
		}

		public override string FullName
		{
			get
			{
				return string.Concat(this.field_type.FullName, " ", base.MemberFullName());
			}
		}

		internal FieldReference()
		{
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.MemberRef);
		}

		public FieldReference(string name, TypeReference fieldType) : base(name)
		{
			if (fieldType == null)
			{
				throw new ArgumentNullException("fieldType");
			}
			this.field_type = fieldType;
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.MemberRef);
		}

		public FieldReference(string name, TypeReference fieldType, TypeReference declaringType) : this(name, fieldType)
		{
			if (declaringType == null)
			{
				throw new ArgumentNullException("declaringType");
			}
			this.DeclaringType = declaringType;
		}

		public virtual FieldDefinition Resolve()
		{
			ModuleDefinition module = this.Module;
			if (module == null)
			{
				throw new NotSupportedException();
			}
			return module.Resolve(this);
		}
	}
}