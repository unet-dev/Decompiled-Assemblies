using Mono.Collections.Generic;
using System;

namespace Mono.Cecil
{
	public abstract class PropertyReference : MemberReference
	{
		private TypeReference property_type;

		public abstract Collection<ParameterDefinition> Parameters
		{
			get;
		}

		public TypeReference PropertyType
		{
			get
			{
				return this.property_type;
			}
			set
			{
				this.property_type = value;
			}
		}

		internal PropertyReference(string name, TypeReference propertyType) : base(name)
		{
			if (propertyType == null)
			{
				throw new ArgumentNullException("propertyType");
			}
			this.property_type = propertyType;
		}

		public abstract PropertyDefinition Resolve();
	}
}