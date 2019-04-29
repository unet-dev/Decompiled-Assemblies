using Mono.Collections.Generic;
using System;

namespace Mono.Cecil
{
	public sealed class SecurityAttribute : ICustomAttribute
	{
		private TypeReference attribute_type;

		internal Collection<CustomAttributeNamedArgument> fields;

		internal Collection<CustomAttributeNamedArgument> properties;

		public TypeReference AttributeType
		{
			get
			{
				return JustDecompileGenerated_get_AttributeType();
			}
			set
			{
				JustDecompileGenerated_set_AttributeType(value);
			}
		}

		public TypeReference JustDecompileGenerated_get_AttributeType()
		{
			return this.attribute_type;
		}

		public void JustDecompileGenerated_set_AttributeType(TypeReference value)
		{
			this.attribute_type = value;
		}

		public Collection<CustomAttributeNamedArgument> Fields
		{
			get
			{
				Collection<CustomAttributeNamedArgument> customAttributeNamedArguments = this.fields;
				if (customAttributeNamedArguments == null)
				{
					Collection<CustomAttributeNamedArgument> customAttributeNamedArguments1 = new Collection<CustomAttributeNamedArgument>();
					Collection<CustomAttributeNamedArgument> customAttributeNamedArguments2 = customAttributeNamedArguments1;
					this.fields = customAttributeNamedArguments1;
					customAttributeNamedArguments = customAttributeNamedArguments2;
				}
				return customAttributeNamedArguments;
			}
		}

		public bool HasFields
		{
			get
			{
				return !this.fields.IsNullOrEmpty<CustomAttributeNamedArgument>();
			}
		}

		public bool HasProperties
		{
			get
			{
				return !this.properties.IsNullOrEmpty<CustomAttributeNamedArgument>();
			}
		}

		public Collection<CustomAttributeNamedArgument> Properties
		{
			get
			{
				Collection<CustomAttributeNamedArgument> customAttributeNamedArguments = this.properties;
				if (customAttributeNamedArguments == null)
				{
					Collection<CustomAttributeNamedArgument> customAttributeNamedArguments1 = new Collection<CustomAttributeNamedArgument>();
					Collection<CustomAttributeNamedArgument> customAttributeNamedArguments2 = customAttributeNamedArguments1;
					this.properties = customAttributeNamedArguments1;
					customAttributeNamedArguments = customAttributeNamedArguments2;
				}
				return customAttributeNamedArguments;
			}
		}

		public SecurityAttribute(TypeReference attributeType)
		{
			this.attribute_type = attributeType;
		}
	}
}