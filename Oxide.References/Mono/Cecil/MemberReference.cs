using System;

namespace Mono.Cecil
{
	public abstract class MemberReference : IMetadataTokenProvider
	{
		private string name;

		private TypeReference declaring_type;

		internal Mono.Cecil.MetadataToken token;

		public virtual bool ContainsGenericParameter
		{
			get
			{
				if (this.declaring_type == null)
				{
					return false;
				}
				return this.declaring_type.ContainsGenericParameter;
			}
		}

		public virtual TypeReference DeclaringType
		{
			get
			{
				return this.declaring_type;
			}
			set
			{
				this.declaring_type = value;
			}
		}

		public abstract string FullName
		{
			get;
		}

		internal bool HasImage
		{
			get
			{
				ModuleDefinition module = this.Module;
				if (module == null)
				{
					return false;
				}
				return module.HasImage;
			}
		}

		public virtual bool IsDefinition
		{
			get
			{
				return false;
			}
		}

		public Mono.Cecil.MetadataToken MetadataToken
		{
			get
			{
				return this.token;
			}
			set
			{
				this.token = value;
			}
		}

		public virtual ModuleDefinition Module
		{
			get
			{
				if (this.declaring_type == null)
				{
					return null;
				}
				return this.declaring_type.Module;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal MemberReference()
		{
		}

		internal MemberReference(string name)
		{
			this.name = name ?? string.Empty;
		}

		internal string MemberFullName()
		{
			if (this.declaring_type == null)
			{
				return this.name;
			}
			return string.Concat(this.declaring_type.FullName, "::", this.name);
		}

		public override string ToString()
		{
			return this.FullName;
		}
	}
}