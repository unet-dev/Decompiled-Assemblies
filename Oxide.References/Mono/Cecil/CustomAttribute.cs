using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil
{
	public sealed class CustomAttribute : ICustomAttribute
	{
		internal readonly uint signature;

		internal bool resolved;

		private MethodReference constructor;

		private byte[] blob;

		internal Collection<CustomAttributeArgument> arguments;

		internal Collection<CustomAttributeNamedArgument> fields;

		internal Collection<CustomAttributeNamedArgument> properties;

		public TypeReference AttributeType
		{
			get
			{
				return this.constructor.DeclaringType;
			}
		}

		public MethodReference Constructor
		{
			get
			{
				return this.constructor;
			}
			set
			{
				this.constructor = value;
			}
		}

		public Collection<CustomAttributeArgument> ConstructorArguments
		{
			get
			{
				this.Resolve();
				Collection<CustomAttributeArgument> customAttributeArguments = this.arguments;
				if (customAttributeArguments == null)
				{
					Collection<CustomAttributeArgument> customAttributeArguments1 = new Collection<CustomAttributeArgument>();
					Collection<CustomAttributeArgument> customAttributeArguments2 = customAttributeArguments1;
					this.arguments = customAttributeArguments1;
					customAttributeArguments = customAttributeArguments2;
				}
				return customAttributeArguments;
			}
		}

		public Collection<CustomAttributeNamedArgument> Fields
		{
			get
			{
				this.Resolve();
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

		public bool HasConstructorArguments
		{
			get
			{
				this.Resolve();
				return !this.arguments.IsNullOrEmpty<CustomAttributeArgument>();
			}
		}

		public bool HasFields
		{
			get
			{
				this.Resolve();
				return !this.fields.IsNullOrEmpty<CustomAttributeNamedArgument>();
			}
		}

		internal bool HasImage
		{
			get
			{
				if (this.constructor == null)
				{
					return false;
				}
				return this.constructor.HasImage;
			}
		}

		public bool HasProperties
		{
			get
			{
				this.Resolve();
				return !this.properties.IsNullOrEmpty<CustomAttributeNamedArgument>();
			}
		}

		public bool IsResolved
		{
			get
			{
				return this.resolved;
			}
		}

		internal ModuleDefinition Module
		{
			get
			{
				return this.constructor.Module;
			}
		}

		public Collection<CustomAttributeNamedArgument> Properties
		{
			get
			{
				this.Resolve();
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

		internal CustomAttribute(uint signature, MethodReference constructor)
		{
			this.signature = signature;
			this.constructor = constructor;
			this.resolved = false;
		}

		public CustomAttribute(MethodReference constructor)
		{
			this.constructor = constructor;
			this.resolved = true;
		}

		public CustomAttribute(MethodReference constructor, byte[] blob)
		{
			this.constructor = constructor;
			this.resolved = false;
			this.blob = blob;
		}

		public byte[] GetBlob()
		{
			if (this.blob != null)
			{
				return this.blob;
			}
			if (!this.HasImage)
			{
				throw new NotSupportedException();
			}
			return this.Module.Read<CustomAttribute, byte[]>(ref this.blob, this, (CustomAttribute attribute, MetadataReader reader) => reader.ReadCustomAttributeBlob(attribute.signature));
		}

		private void Resolve()
		{
			if (this.resolved || !this.HasImage)
			{
				return;
			}
			this.Module.Read<CustomAttribute, CustomAttribute>(this, (CustomAttribute attribute, MetadataReader reader) => {
				try
				{
					reader.ReadCustomAttributeSignature(attribute);
					this.resolved = true;
				}
				catch (ResolutionException resolutionException)
				{
					if (this.arguments != null)
					{
						this.arguments.Clear();
					}
					if (this.fields != null)
					{
						this.fields.Clear();
					}
					if (this.properties != null)
					{
						this.properties.Clear();
					}
					this.resolved = false;
				}
				return this;
			});
		}
	}
}