using System;
using System.Runtime.CompilerServices;

namespace SilentOrbit.ProtocolBuffers
{
	public class KeyValue
	{
		public SilentOrbit.ProtocolBuffers.Key Key
		{
			get;
			set;
		}

		public byte[] Value
		{
			get;
			set;
		}

		public KeyValue(SilentOrbit.ProtocolBuffers.Key key, byte[] value)
		{
			this.Key = key;
			this.Value = value;
		}

		public override string ToString()
		{
			object field = this.Key.Field;
			SilentOrbit.ProtocolBuffers.Key key = this.Key;
			return string.Format("[KeyValue: {0}, {1}, {2} bytes]", field, key.WireType, (int)this.Value.Length);
		}
	}
}