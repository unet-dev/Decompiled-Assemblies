using System;
using System.Runtime.Serialization;

namespace ProtoBuf
{
	public sealed class SerializationContext
	{
		private bool frozen;

		private object context;

		private readonly static SerializationContext @default;

		private StreamingContextStates state = StreamingContextStates.Persistence;

		public object Context
		{
			get
			{
				return this.context;
			}
			set
			{
				if (this.context != value)
				{
					this.ThrowIfFrozen();
					this.context = value;
				}
			}
		}

		internal static SerializationContext Default
		{
			get
			{
				return SerializationContext.@default;
			}
		}

		public StreamingContextStates State
		{
			get
			{
				return this.state;
			}
			set
			{
				if (this.state != value)
				{
					this.ThrowIfFrozen();
					this.state = value;
				}
			}
		}

		static SerializationContext()
		{
			SerializationContext.@default = new SerializationContext();
			SerializationContext.@default.Freeze();
		}

		public SerializationContext()
		{
		}

		internal void Freeze()
		{
			this.frozen = true;
		}

		public static implicit operator StreamingContext(SerializationContext ctx)
		{
			if (ctx == null)
			{
				return new StreamingContext(StreamingContextStates.Persistence);
			}
			return new StreamingContext(ctx.state, ctx.context);
		}

		public static implicit operator SerializationContext(StreamingContext ctx)
		{
			SerializationContext serializationContext = new SerializationContext()
			{
				Context = ctx.Context,
				State = ctx.State
			};
			return serializationContext;
		}

		private void ThrowIfFrozen()
		{
			if (this.frozen)
			{
				throw new InvalidOperationException("The serialization-context cannot be changed once it is in use");
			}
		}
	}
}