using System;

namespace ProtoBuf
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, AllowMultiple=false, Inherited=false)]
	public sealed class ProtoContractAttribute : Attribute
	{
		private const byte OPTIONS_InferTagFromName = 1;

		private const byte OPTIONS_InferTagFromNameHasValue = 2;

		private const byte OPTIONS_UseProtoMembersOnly = 4;

		private const byte OPTIONS_SkipConstructor = 8;

		private const byte OPTIONS_IgnoreListHandling = 16;

		private const byte OPTIONS_AsReferenceDefault = 32;

		private const byte OPTIONS_EnumPassthru = 64;

		private const byte OPTIONS_EnumPassthruHasValue = 128;

		private string name;

		private int implicitFirstTag;

		private ProtoBuf.ImplicitFields implicitFields;

		private int dataMemberOffset;

		private byte flags;

		public bool AsReferenceDefault
		{
			get
			{
				return this.HasFlag(32);
			}
			set
			{
				this.SetFlag(32, value);
			}
		}

		public int DataMemberOffset
		{
			get
			{
				return this.dataMemberOffset;
			}
			set
			{
				this.dataMemberOffset = value;
			}
		}

		public bool EnumPassthru
		{
			get
			{
				return this.HasFlag(64);
			}
			set
			{
				this.SetFlag(64, value);
				this.SetFlag(128, true);
			}
		}

		internal bool EnumPassthruHasValue
		{
			get
			{
				return this.HasFlag(128);
			}
		}

		public bool IgnoreListHandling
		{
			get
			{
				return this.HasFlag(16);
			}
			set
			{
				this.SetFlag(16, value);
			}
		}

		public ProtoBuf.ImplicitFields ImplicitFields
		{
			get
			{
				return this.implicitFields;
			}
			set
			{
				this.implicitFields = value;
			}
		}

		public int ImplicitFirstTag
		{
			get
			{
				return this.implicitFirstTag;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("ImplicitFirstTag");
				}
				this.implicitFirstTag = value;
			}
		}

		public bool InferTagFromName
		{
			get
			{
				return this.HasFlag(1);
			}
			set
			{
				this.SetFlag(1, value);
				this.SetFlag(2, true);
			}
		}

		internal bool InferTagFromNameHasValue
		{
			get
			{
				return this.HasFlag(2);
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

		public bool SkipConstructor
		{
			get
			{
				return this.HasFlag(8);
			}
			set
			{
				this.SetFlag(8, value);
			}
		}

		public bool UseProtoMembersOnly
		{
			get
			{
				return this.HasFlag(4);
			}
			set
			{
				this.SetFlag(4, value);
			}
		}

		public ProtoContractAttribute()
		{
		}

		private bool HasFlag(byte flag)
		{
			return (this.flags & flag) == flag;
		}

		private void SetFlag(byte flag, bool value)
		{
			if (value)
			{
				ProtoContractAttribute protoContractAttribute = this;
				protoContractAttribute.flags = (byte)(protoContractAttribute.flags | flag);
				return;
			}
			this.flags = (byte)(this.flags & ~flag);
		}
	}
}