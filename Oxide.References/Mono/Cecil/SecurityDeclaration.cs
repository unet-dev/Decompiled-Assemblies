using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil
{
	public sealed class SecurityDeclaration
	{
		internal readonly uint signature;

		private byte[] blob;

		private readonly ModuleDefinition module;

		internal bool resolved;

		private SecurityAction action;

		internal Collection<SecurityAttribute> security_attributes;

		public SecurityAction Action
		{
			get
			{
				return this.action;
			}
			set
			{
				this.action = value;
			}
		}

		internal bool HasImage
		{
			get
			{
				if (this.module == null)
				{
					return false;
				}
				return this.module.HasImage;
			}
		}

		public bool HasSecurityAttributes
		{
			get
			{
				this.Resolve();
				return !this.security_attributes.IsNullOrEmpty<SecurityAttribute>();
			}
		}

		public Collection<SecurityAttribute> SecurityAttributes
		{
			get
			{
				this.Resolve();
				Collection<SecurityAttribute> securityAttributes = this.security_attributes;
				if (securityAttributes == null)
				{
					Collection<SecurityAttribute> securityAttributes1 = new Collection<SecurityAttribute>();
					Collection<SecurityAttribute> securityAttributes2 = securityAttributes1;
					this.security_attributes = securityAttributes1;
					securityAttributes = securityAttributes2;
				}
				return securityAttributes;
			}
		}

		internal SecurityDeclaration(SecurityAction action, uint signature, ModuleDefinition module)
		{
			this.action = action;
			this.signature = signature;
			this.module = module;
		}

		public SecurityDeclaration(SecurityAction action)
		{
			this.action = action;
			this.resolved = true;
		}

		public SecurityDeclaration(SecurityAction action, byte[] blob)
		{
			this.action = action;
			this.resolved = false;
			this.blob = blob;
		}

		public byte[] GetBlob()
		{
			if (this.blob != null)
			{
				return this.blob;
			}
			if (!this.HasImage || this.signature == 0)
			{
				throw new NotSupportedException();
			}
			byte[] numArray = this.module.Read<SecurityDeclaration, byte[]>(this, (SecurityDeclaration declaration, MetadataReader reader) => reader.ReadSecurityDeclarationBlob(declaration.signature));
			byte[] numArray1 = numArray;
			this.blob = numArray;
			return numArray1;
		}

		private void Resolve()
		{
			if (this.resolved || !this.HasImage)
			{
				return;
			}
			this.module.Read<SecurityDeclaration, SecurityDeclaration>(this, (SecurityDeclaration declaration, MetadataReader reader) => {
				reader.ReadSecurityDeclarationSignature(declaration);
				return this;
			});
			this.resolved = true;
		}
	}
}