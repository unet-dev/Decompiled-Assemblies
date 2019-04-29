using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ProtoBuf.Compiler
{
	internal sealed class Local : IDisposable
	{
		private LocalBuilder @value;

		private CompilerContext ctx;

		private readonly System.Type type;

		public System.Type Type
		{
			get
			{
				return this.type;
			}
		}

		internal LocalBuilder Value
		{
			get
			{
				if (this.@value == null)
				{
					throw new ObjectDisposedException(this.GetType().Name);
				}
				return this.@value;
			}
		}

		private Local(LocalBuilder value, System.Type type)
		{
			this.@value = value;
			this.type = type;
		}

		internal Local(CompilerContext ctx, System.Type type)
		{
			this.ctx = ctx;
			if (ctx != null)
			{
				this.@value = ctx.GetFromPool(type);
			}
			this.type = type;
		}

		public Local AsCopy()
		{
			if (this.ctx == null)
			{
				return this;
			}
			return new Local(this.@value, this.type);
		}

		public void Dispose()
		{
			if (this.ctx != null)
			{
				this.ctx.ReleaseToPool(this.@value);
				this.@value = null;
				this.ctx = null;
			}
		}

		internal bool IsSame(Local other)
		{
			if (this == other)
			{
				return true;
			}
			object obj = this.@value;
			if (other == null)
			{
				return false;
			}
			return obj == other.@value;
		}
	}
}