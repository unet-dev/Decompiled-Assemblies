using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mono.Cecil
{
	public sealed class PropertyDefinition : PropertyReference, IMemberDefinition, ICustomAttributeProvider, IMetadataTokenProvider, IConstantProvider
	{
		private bool? has_this;

		private ushort attributes;

		private Collection<CustomAttribute> custom_attributes;

		internal MethodDefinition get_method;

		internal MethodDefinition set_method;

		internal Collection<MethodDefinition> other_methods;

		private object constant = Mixin.NotResolved;

		public PropertyAttributes Attributes
		{
			get
			{
				return (PropertyAttributes)this.attributes;
			}
			set
			{
				this.attributes = (ushort)value;
			}
		}

		public object Constant
		{
			get
			{
				if (!this.HasConstant)
				{
					return null;
				}
				return this.constant;
			}
			set
			{
				this.constant = value;
			}
		}

		public Collection<CustomAttribute> CustomAttributes
		{
			get
			{
				return this.custom_attributes ?? this.GetCustomAttributes(ref this.custom_attributes, this.Module);
			}
		}

		public new TypeDefinition DeclaringType
		{
			get
			{
				return (TypeDefinition)base.DeclaringType;
			}
			set
			{
				base.DeclaringType = value;
			}
		}

		public override string FullName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(base.PropertyType.ToString());
				stringBuilder.Append(' ');
				stringBuilder.Append(base.MemberFullName());
				stringBuilder.Append('(');
				if (this.HasParameters)
				{
					Collection<ParameterDefinition> parameters = this.Parameters;
					for (int i = 0; i < parameters.Count; i++)
					{
						if (i > 0)
						{
							stringBuilder.Append(',');
						}
						stringBuilder.Append(parameters[i].ParameterType.FullName);
					}
				}
				stringBuilder.Append(')');
				return stringBuilder.ToString();
			}
		}

		public MethodDefinition GetMethod
		{
			get
			{
				if (this.get_method != null)
				{
					return this.get_method;
				}
				this.InitializeMethods();
				return this.get_method;
			}
			set
			{
				this.get_method = value;
			}
		}

		public bool HasConstant
		{
			get
			{
				this.ResolveConstant(ref this.constant, this.Module);
				return this.constant != Mixin.NoValue;
			}
			set
			{
				if (!value)
				{
					this.constant = Mixin.NoValue;
				}
			}
		}

		public bool HasCustomAttributes
		{
			get
			{
				if (this.custom_attributes == null)
				{
					return this.GetHasCustomAttributes(this.Module);
				}
				return this.custom_attributes.Count > 0;
			}
		}

		public bool HasDefault
		{
			get
			{
				return this.attributes.GetAttributes(4096);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(4096, value);
			}
		}

		public bool HasOtherMethods
		{
			get
			{
				if (this.other_methods != null)
				{
					return this.other_methods.Count > 0;
				}
				this.InitializeMethods();
				return !this.other_methods.IsNullOrEmpty<MethodDefinition>();
			}
		}

		public bool HasParameters
		{
			get
			{
				this.InitializeMethods();
				if (this.get_method != null)
				{
					return this.get_method.HasParameters;
				}
				if (this.set_method == null)
				{
					return false;
				}
				if (!this.set_method.HasParameters)
				{
					return false;
				}
				return this.set_method.Parameters.Count > 1;
			}
		}

		public bool HasThis
		{
			get
			{
				if (this.has_this.HasValue)
				{
					return this.has_this.Value;
				}
				if (this.GetMethod != null)
				{
					return this.get_method.HasThis;
				}
				if (this.SetMethod == null)
				{
					return false;
				}
				return this.set_method.HasThis;
			}
			set
			{
				this.has_this = new bool?(value);
			}
		}

		public override bool IsDefinition
		{
			get
			{
				return true;
			}
		}

		public bool IsRuntimeSpecialName
		{
			get
			{
				return this.attributes.GetAttributes(1024);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(1024, value);
			}
		}

		public bool IsSpecialName
		{
			get
			{
				return this.attributes.GetAttributes(512);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(512, value);
			}
		}

		public Collection<MethodDefinition> OtherMethods
		{
			get
			{
				if (this.other_methods != null)
				{
					return this.other_methods;
				}
				this.InitializeMethods();
				if (this.other_methods != null)
				{
					return this.other_methods;
				}
				Collection<MethodDefinition> methodDefinitions = new Collection<MethodDefinition>();
				Collection<MethodDefinition> methodDefinitions1 = methodDefinitions;
				this.other_methods = methodDefinitions;
				return methodDefinitions1;
			}
		}

		public override Collection<ParameterDefinition> Parameters
		{
			get
			{
				this.InitializeMethods();
				if (this.get_method != null)
				{
					return PropertyDefinition.MirrorParameters(this.get_method, 0);
				}
				if (this.set_method == null)
				{
					return new Collection<ParameterDefinition>();
				}
				return PropertyDefinition.MirrorParameters(this.set_method, 1);
			}
		}

		public MethodDefinition SetMethod
		{
			get
			{
				if (this.set_method != null)
				{
					return this.set_method;
				}
				this.InitializeMethods();
				return this.set_method;
			}
			set
			{
				this.set_method = value;
			}
		}

		public PropertyDefinition(string name, PropertyAttributes attributes, TypeReference propertyType) : base(name, propertyType)
		{
			this.attributes = (ushort)attributes;
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.Property);
		}

		private void InitializeMethods()
		{
			ModuleDefinition module = this.Module;
			if (module == null)
			{
				return;
			}
			lock (module.SyncRoot)
			{
				if (this.get_method == null && this.set_method == null)
				{
					if (module.HasImage())
					{
						module.Read<PropertyDefinition, PropertyDefinition>(this, (PropertyDefinition property, MetadataReader reader) => reader.ReadMethods(property));
					}
				}
			}
		}

		private static Collection<ParameterDefinition> MirrorParameters(MethodDefinition method, int bound)
		{
			Collection<ParameterDefinition> parameterDefinitions = new Collection<ParameterDefinition>();
			if (!method.HasParameters)
			{
				return parameterDefinitions;
			}
			Collection<ParameterDefinition> parameters = method.Parameters;
			int count = parameters.Count - bound;
			for (int i = 0; i < count; i++)
			{
				parameterDefinitions.Add(parameters[i]);
			}
			return parameterDefinitions;
		}

		public override PropertyDefinition Resolve()
		{
			return this;
		}
	}
}