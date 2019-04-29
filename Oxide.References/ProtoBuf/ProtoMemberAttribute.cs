using System;
using System.Reflection;

namespace ProtoBuf
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false, Inherited=true)]
	public class ProtoMemberAttribute : Attribute, IComparable, IComparable<ProtoMemberAttribute>
	{
		internal MemberInfo Member;

		internal bool TagIsPinned;

		private string name;

		private ProtoBuf.DataFormat dataFormat;

		private int tag;

		private MemberSerializationOptions options;

		public bool AsReference
		{
			get
			{
				return (this.options & MemberSerializationOptions.AsReference) == MemberSerializationOptions.AsReference;
			}
			set
			{
				if (!value)
				{
					ProtoMemberAttribute protoMemberAttribute = this;
					protoMemberAttribute.options = protoMemberAttribute.options & (MemberSerializationOptions.Packed | MemberSerializationOptions.Required | MemberSerializationOptions.DynamicType | MemberSerializationOptions.OverwriteList | MemberSerializationOptions.AsReferenceHasValue);
				}
				else
				{
					this.options |= MemberSerializationOptions.AsReference;
				}
				this.options |= MemberSerializationOptions.AsReferenceHasValue;
			}
		}

		internal bool AsReferenceHasValue
		{
			get
			{
				return (this.options & MemberSerializationOptions.AsReferenceHasValue) == MemberSerializationOptions.AsReferenceHasValue;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.AsReferenceHasValue;
					return;
				}
				ProtoMemberAttribute protoMemberAttribute = this;
				protoMemberAttribute.options = protoMemberAttribute.options & (MemberSerializationOptions.Packed | MemberSerializationOptions.Required | MemberSerializationOptions.AsReference | MemberSerializationOptions.DynamicType | MemberSerializationOptions.OverwriteList);
			}
		}

		public ProtoBuf.DataFormat DataFormat
		{
			get
			{
				return this.dataFormat;
			}
			set
			{
				this.dataFormat = value;
			}
		}

		public bool DynamicType
		{
			get
			{
				return (this.options & MemberSerializationOptions.DynamicType) == MemberSerializationOptions.DynamicType;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.DynamicType;
					return;
				}
				ProtoMemberAttribute protoMemberAttribute = this;
				protoMemberAttribute.options = protoMemberAttribute.options & (MemberSerializationOptions.Packed | MemberSerializationOptions.Required | MemberSerializationOptions.AsReference | MemberSerializationOptions.OverwriteList | MemberSerializationOptions.AsReferenceHasValue);
			}
		}

		public bool IsPacked
		{
			get
			{
				return (this.options & MemberSerializationOptions.Packed) == MemberSerializationOptions.Packed;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.Packed;
					return;
				}
				ProtoMemberAttribute protoMemberAttribute = this;
				protoMemberAttribute.options = protoMemberAttribute.options & (MemberSerializationOptions.Required | MemberSerializationOptions.AsReference | MemberSerializationOptions.DynamicType | MemberSerializationOptions.OverwriteList | MemberSerializationOptions.AsReferenceHasValue);
			}
		}

		public bool IsRequired
		{
			get
			{
				return (this.options & MemberSerializationOptions.Required) == MemberSerializationOptions.Required;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.Required;
					return;
				}
				ProtoMemberAttribute protoMemberAttribute = this;
				protoMemberAttribute.options = protoMemberAttribute.options & (MemberSerializationOptions.Packed | MemberSerializationOptions.AsReference | MemberSerializationOptions.DynamicType | MemberSerializationOptions.OverwriteList | MemberSerializationOptions.AsReferenceHasValue);
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

		public MemberSerializationOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		public bool OverwriteList
		{
			get
			{
				return (this.options & MemberSerializationOptions.OverwriteList) == MemberSerializationOptions.OverwriteList;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.OverwriteList;
					return;
				}
				ProtoMemberAttribute protoMemberAttribute = this;
				protoMemberAttribute.options = protoMemberAttribute.options & (MemberSerializationOptions.Packed | MemberSerializationOptions.Required | MemberSerializationOptions.AsReference | MemberSerializationOptions.DynamicType | MemberSerializationOptions.AsReferenceHasValue);
			}
		}

		public int Tag
		{
			get
			{
				return this.tag;
			}
		}

		public ProtoMemberAttribute(int tag) : this(tag, false)
		{
		}

		internal ProtoMemberAttribute(int tag, bool forced)
		{
			if (tag <= 0 && !forced)
			{
				throw new ArgumentOutOfRangeException("tag");
			}
			this.tag = tag;
		}

		public int CompareTo(object other)
		{
			return this.CompareTo(other as ProtoMemberAttribute);
		}

		public int CompareTo(ProtoMemberAttribute other)
		{
			if (other == null)
			{
				return -1;
			}
			if (this == other)
			{
				return 0;
			}
			int num = this.tag.CompareTo(other.tag);
			if (num == 0)
			{
				num = string.CompareOrdinal(this.name, other.name);
			}
			return num;
		}

		internal void Rebase(int tag)
		{
			this.tag = tag;
		}
	}
}