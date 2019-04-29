using System;

namespace Mono.Cecil
{
	public sealed class CustomMarshalInfo : MarshalInfo
	{
		internal System.Guid guid;

		internal string unmanaged_type;

		internal TypeReference managed_type;

		internal string cookie;

		public string Cookie
		{
			get
			{
				return this.cookie;
			}
			set
			{
				this.cookie = value;
			}
		}

		public System.Guid Guid
		{
			get
			{
				return this.guid;
			}
			set
			{
				this.guid = value;
			}
		}

		public TypeReference ManagedType
		{
			get
			{
				return this.managed_type;
			}
			set
			{
				this.managed_type = value;
			}
		}

		public string UnmanagedType
		{
			get
			{
				return this.unmanaged_type;
			}
			set
			{
				this.unmanaged_type = value;
			}
		}

		public CustomMarshalInfo() : base(Mono.Cecil.NativeType.CustomMarshaler)
		{
		}
	}
}