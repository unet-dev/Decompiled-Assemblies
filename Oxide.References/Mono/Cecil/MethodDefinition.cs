using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil
{
	public sealed class MethodDefinition : MethodReference, IMemberDefinition, ICustomAttributeProvider, IMetadataTokenProvider, ISecurityDeclarationProvider
	{
		private ushort attributes;

		private ushort impl_attributes;

		internal volatile bool sem_attrs_ready;

		internal MethodSemanticsAttributes sem_attrs;

		private Collection<CustomAttribute> custom_attributes;

		private Collection<SecurityDeclaration> security_declarations;

		internal uint rva;

		internal Mono.Cecil.PInvokeInfo pinvoke;

		private Collection<MethodReference> overrides;

		internal MethodBody body;

		public MethodAttributes Attributes
		{
			get
			{
				return (MethodAttributes)this.attributes;
			}
			set
			{
				this.attributes = (ushort)value;
			}
		}

		public MethodBody Body
		{
			get
			{
				MethodBody methodBody = this.body;
				if (methodBody != null)
				{
					return methodBody;
				}
				if (!this.HasBody)
				{
					return null;
				}
				if (!base.HasImage || this.rva == 0)
				{
					MethodBody methodBody1 = new MethodBody(this);
					MethodBody methodBody2 = methodBody1;
					this.body = methodBody1;
					return methodBody2;
				}
				return this.Module.Read<MethodDefinition, MethodBody>(ref this.body, this, (MethodDefinition method, MetadataReader reader) => reader.ReadMethodBody(method));
			}
			set
			{
				ModuleDefinition module = this.Module;
				if (module == null)
				{
					this.body = value;
					return;
				}
				lock (module.SyncRoot)
				{
					this.body = value;
				}
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

		public override Collection<GenericParameter> GenericParameters
		{
			get
			{
				return this.generic_parameters ?? this.GetGenericParameters(ref this.generic_parameters, this.Module);
			}
		}

		public bool HasBody
		{
			get
			{
				if ((this.attributes & 1024) != 0 || (this.attributes & 8192) != 0 || (this.impl_attributes & 4096) != 0 || (this.impl_attributes & 1) != 0 || (this.impl_attributes & 4) != 0)
				{
					return false;
				}
				return (this.impl_attributes & 3) == 0;
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

		public override bool HasGenericParameters
		{
			get
			{
				if (this.generic_parameters == null)
				{
					return this.GetHasGenericParameters(this.Module);
				}
				return this.generic_parameters.Count > 0;
			}
		}

		public bool HasOverrides
		{
			get
			{
				if (this.overrides != null)
				{
					return this.overrides.Count > 0;
				}
				if (!base.HasImage)
				{
					return false;
				}
				return this.Module.Read<MethodDefinition, bool>(this, (MethodDefinition method, MetadataReader reader) => reader.HasOverrides(method));
			}
		}

		public bool HasPInvokeInfo
		{
			get
			{
				if (this.pinvoke != null)
				{
					return true;
				}
				return this.IsPInvokeImpl;
			}
		}

		public bool HasSecurity
		{
			get
			{
				return this.attributes.GetAttributes(16384);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(16384, value);
			}
		}

		public bool HasSecurityDeclarations
		{
			get
			{
				if (this.security_declarations == null)
				{
					return this.GetHasSecurityDeclarations(this.Module);
				}
				return this.security_declarations.Count > 0;
			}
		}

		public MethodImplAttributes ImplAttributes
		{
			get
			{
				return (MethodImplAttributes)this.impl_attributes;
			}
			set
			{
				this.impl_attributes = (ushort)value;
			}
		}

		public bool IsAbstract
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

		public bool IsAddOn
		{
			get
			{
				return this.GetSemantics(MethodSemanticsAttributes.AddOn);
			}
			set
			{
				this.SetSemantics(MethodSemanticsAttributes.AddOn, value);
			}
		}

		public bool IsAssembly
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 3);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 3, value);
			}
		}

		public bool IsCheckAccessOnOverride
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

		public bool IsCompilerControlled
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 0);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 0, value);
			}
		}

		public bool IsConstructor
		{
			get
			{
				if (!this.IsRuntimeSpecialName || !this.IsSpecialName)
				{
					return false;
				}
				if (this.Name == ".cctor")
				{
					return true;
				}
				return this.Name == ".ctor";
			}
		}

		public override bool IsDefinition
		{
			get
			{
				return true;
			}
		}

		public bool IsFamily
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 4);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 4, value);
			}
		}

		public bool IsFamilyAndAssembly
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 2);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 2, value);
			}
		}

		public bool IsFamilyOrAssembly
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 5);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 5, value);
			}
		}

		public bool IsFinal
		{
			get
			{
				return this.attributes.GetAttributes(32);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(32, value);
			}
		}

		public bool IsFire
		{
			get
			{
				return this.GetSemantics(MethodSemanticsAttributes.Fire);
			}
			set
			{
				this.SetSemantics(MethodSemanticsAttributes.Fire, value);
			}
		}

		public bool IsForwardRef
		{
			get
			{
				return this.impl_attributes.GetAttributes(16);
			}
			set
			{
				this.impl_attributes = this.impl_attributes.SetAttributes(16, value);
			}
		}

		public bool IsGetter
		{
			get
			{
				return this.GetSemantics(MethodSemanticsAttributes.Getter);
			}
			set
			{
				this.SetSemantics(MethodSemanticsAttributes.Getter, value);
			}
		}

		public bool IsHideBySig
		{
			get
			{
				return this.attributes.GetAttributes(128);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(128, value);
			}
		}

		public bool IsIL
		{
			get
			{
				return this.impl_attributes.GetMaskedAttributes(3, 0);
			}
			set
			{
				this.impl_attributes = this.impl_attributes.SetMaskedAttributes(3, 0, value);
			}
		}

		public bool IsInternalCall
		{
			get
			{
				return this.impl_attributes.GetAttributes(4096);
			}
			set
			{
				this.impl_attributes = this.impl_attributes.SetAttributes(4096, value);
			}
		}

		public bool IsManaged
		{
			get
			{
				return this.impl_attributes.GetMaskedAttributes(4, 0);
			}
			set
			{
				this.impl_attributes = this.impl_attributes.SetMaskedAttributes(4, 0, value);
			}
		}

		public bool IsNative
		{
			get
			{
				return this.impl_attributes.GetMaskedAttributes(3, 1);
			}
			set
			{
				this.impl_attributes = this.impl_attributes.SetMaskedAttributes(3, 1, value);
			}
		}

		public bool IsNewSlot
		{
			get
			{
				return this.attributes.GetMaskedAttributes(256, 256);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(256, 256, value);
			}
		}

		public bool IsOther
		{
			get
			{
				return this.GetSemantics(MethodSemanticsAttributes.Other);
			}
			set
			{
				this.SetSemantics(MethodSemanticsAttributes.Other, value);
			}
		}

		public bool IsPInvokeImpl
		{
			get
			{
				return this.attributes.GetAttributes(8192);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(8192, value);
			}
		}

		public bool IsPreserveSig
		{
			get
			{
				return this.impl_attributes.GetAttributes(128);
			}
			set
			{
				this.impl_attributes = this.impl_attributes.SetAttributes(128, value);
			}
		}

		public bool IsPrivate
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 1);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 1, value);
			}
		}

		public bool IsPublic
		{
			get
			{
				return this.attributes.GetMaskedAttributes(7, 6);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(7, 6, value);
			}
		}

		public bool IsRemoveOn
		{
			get
			{
				return this.GetSemantics(MethodSemanticsAttributes.RemoveOn);
			}
			set
			{
				this.SetSemantics(MethodSemanticsAttributes.RemoveOn, value);
			}
		}

		public bool IsReuseSlot
		{
			get
			{
				return this.attributes.GetMaskedAttributes(256, 0);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(256, 0, value);
			}
		}

		public bool IsRuntime
		{
			get
			{
				return this.impl_attributes.GetMaskedAttributes(3, 3);
			}
			set
			{
				this.impl_attributes = this.impl_attributes.SetMaskedAttributes(3, 3, value);
			}
		}

		public bool IsRuntimeSpecialName
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

		public bool IsSetter
		{
			get
			{
				return this.GetSemantics(MethodSemanticsAttributes.Setter);
			}
			set
			{
				this.SetSemantics(MethodSemanticsAttributes.Setter, value);
			}
		}

		public bool IsSpecialName
		{
			get
			{
				return this.attributes.GetAttributes(2048);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(2048, value);
			}
		}

		public bool IsStatic
		{
			get
			{
				return this.attributes.GetAttributes(16);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(16, value);
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.impl_attributes.GetAttributes(32);
			}
			set
			{
				this.impl_attributes = this.impl_attributes.SetAttributes(32, value);
			}
		}

		public bool IsUnmanaged
		{
			get
			{
				return this.impl_attributes.GetMaskedAttributes(4, 4);
			}
			set
			{
				this.impl_attributes = this.impl_attributes.SetMaskedAttributes(4, 4, value);
			}
		}

		public bool IsUnmanagedExport
		{
			get
			{
				return this.attributes.GetAttributes(8);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(8, value);
			}
		}

		public bool IsVirtual
		{
			get
			{
				return this.attributes.GetAttributes(64);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(64, value);
			}
		}

		public bool NoInlining
		{
			get
			{
				return this.impl_attributes.GetAttributes(8);
			}
			set
			{
				this.impl_attributes = this.impl_attributes.SetAttributes(8, value);
			}
		}

		public bool NoOptimization
		{
			get
			{
				return this.impl_attributes.GetAttributes(64);
			}
			set
			{
				this.impl_attributes = this.impl_attributes.SetAttributes(64, value);
			}
		}

		public Collection<MethodReference> Overrides
		{
			get
			{
				if (this.overrides != null)
				{
					return this.overrides;
				}
				if (!base.HasImage)
				{
					Collection<MethodReference> methodReferences = new Collection<MethodReference>();
					Collection<MethodReference> methodReferences1 = methodReferences;
					this.overrides = methodReferences;
					return methodReferences1;
				}
				return this.Module.Read<MethodDefinition, Collection<MethodReference>>(ref this.overrides, this, (MethodDefinition method, MetadataReader reader) => reader.ReadOverrides(method));
			}
		}

		public Mono.Cecil.PInvokeInfo PInvokeInfo
		{
			get
			{
				if (this.pinvoke != null)
				{
					return this.pinvoke;
				}
				if (!base.HasImage || !this.IsPInvokeImpl)
				{
					return null;
				}
				return this.Module.Read<MethodDefinition, Mono.Cecil.PInvokeInfo>(ref this.pinvoke, this, (MethodDefinition method, MetadataReader reader) => reader.ReadPInvokeInfo(method));
			}
			set
			{
				this.IsPInvokeImpl = true;
				this.pinvoke = value;
			}
		}

		public int RVA
		{
			get
			{
				return (int)this.rva;
			}
		}

		public Collection<SecurityDeclaration> SecurityDeclarations
		{
			get
			{
				return this.security_declarations ?? this.GetSecurityDeclarations(ref this.security_declarations, this.Module);
			}
		}

		public MethodSemanticsAttributes SemanticsAttributes
		{
			get
			{
				if (this.sem_attrs_ready)
				{
					return this.sem_attrs;
				}
				if (base.HasImage)
				{
					this.ReadSemantics();
					return this.sem_attrs;
				}
				this.sem_attrs = MethodSemanticsAttributes.None;
				this.sem_attrs_ready = true;
				return this.sem_attrs;
			}
			set
			{
				this.sem_attrs = value;
			}
		}

		internal MethodDefinition()
		{
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.Method);
		}

		public MethodDefinition(string name, MethodAttributes attributes, TypeReference returnType) : base(name, returnType)
		{
			this.attributes = (ushort)attributes;
			this.HasThis = !this.IsStatic;
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.Method);
		}

		internal void ReadSemantics()
		{
			if (this.sem_attrs_ready)
			{
				return;
			}
			ModuleDefinition module = this.Module;
			if (module == null)
			{
				return;
			}
			if (!module.HasImage)
			{
				return;
			}
			module.Read<MethodDefinition, MethodSemanticsAttributes>(this, (MethodDefinition method, MetadataReader reader) => reader.ReadAllSemantics(method));
		}

		public override MethodDefinition Resolve()
		{
			return this;
		}
	}
}