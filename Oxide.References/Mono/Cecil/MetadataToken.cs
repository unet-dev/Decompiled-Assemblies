using System;

namespace Mono.Cecil
{
	public struct MetadataToken
	{
		private readonly uint token;

		public readonly static MetadataToken Zero;

		public uint RID
		{
			get
			{
				return this.token & 16777215;
			}
		}

		public Mono.Cecil.TokenType TokenType
		{
			get
			{
				return (Mono.Cecil.TokenType)(this.token & -16777216);
			}
		}

		static MetadataToken()
		{
			MetadataToken.Zero = new MetadataToken(0);
		}

		public MetadataToken(uint token)
		{
			this.token = token;
		}

		public MetadataToken(Mono.Cecil.TokenType type) : this(type, 0)
		{
		}

		public MetadataToken(Mono.Cecil.TokenType type, uint rid)
		{
			this.token = (uint)type | rid;
		}

		public MetadataToken(Mono.Cecil.TokenType type, int rid)
		{
			this.token = (uint)((int)type | rid);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is MetadataToken))
			{
				return false;
			}
			return ((MetadataToken)obj).token == this.token;
		}

		public override int GetHashCode()
		{
			return (int)this.token;
		}

		public static bool operator ==(MetadataToken one, MetadataToken other)
		{
			return one.token == other.token;
		}

		public static bool operator !=(MetadataToken one, MetadataToken other)
		{
			return one.token != other.token;
		}

		public int ToInt32()
		{
			return (int)this.token;
		}

		public override string ToString()
		{
			object tokenType = this.TokenType;
			uint rID = this.RID;
			return string.Format("[{0}:0x{1}]", tokenType, rID.ToString("x4"));
		}

		public uint ToUInt32()
		{
			return this.token;
		}
	}
}