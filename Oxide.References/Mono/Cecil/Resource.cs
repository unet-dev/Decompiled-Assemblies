using System;

namespace Mono.Cecil
{
	public abstract class Resource
	{
		private string name;

		private uint attributes;

		public ManifestResourceAttributes Attributes
		{
			get
			{
				return (ManifestResourceAttributes)this.attributes;
			}
			set
			{
				this.attributes = (uint)value;
			}
		}

		public bool IsPrivate
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

		public bool IsPublic
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

		public string Name
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

		public abstract Mono.Cecil.ResourceType ResourceType
		{
			get;
		}

		internal Resource(string name, ManifestResourceAttributes attributes)
		{
			this.name = name;
			this.attributes = (uint)attributes;
		}
	}
}