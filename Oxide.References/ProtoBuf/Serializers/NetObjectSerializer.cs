using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class NetObjectSerializer : IProtoSerializer
	{
		private readonly int key;

		private readonly Type type;

		private readonly BclHelpers.NetObjectOptions options;

		public Type ExpectedType
		{
			get
			{
				return this.type;
			}
		}

		public bool RequiresOldValue
		{
			get
			{
				return true;
			}
		}

		public bool ReturnsValue
		{
			get
			{
				return true;
			}
		}

		public NetObjectSerializer(TypeModel model, Type type, int key, BclHelpers.NetObjectOptions options)
		{
			bool flag = (byte)(options & BclHelpers.NetObjectOptions.DynamicType) != 0;
			this.key = (flag ? -1 : key);
			this.type = (flag ? model.MapType(typeof(object)) : type);
			this.options = options;
		}

		public void EmitRead(CompilerContext ctx, Local valueFrom)
		{
			ctx.LoadValue(valueFrom);
			ctx.CastToObject(this.type);
			ctx.LoadReaderWriter();
			ctx.LoadValue(ctx.MapMetaKeyToCompiledKey(this.key));
			if (this.type != ctx.MapType(typeof(object)))
			{
				ctx.LoadValue(this.type);
			}
			else
			{
				ctx.LoadNullRef();
			}
			ctx.LoadValue((int)this.options);
			ctx.EmitCall(ctx.MapType(typeof(BclHelpers)).GetMethod("ReadNetObject"));
			ctx.CastFromObject(this.type);
		}

		public void EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.LoadValue(valueFrom);
			ctx.CastToObject(this.type);
			ctx.LoadReaderWriter();
			ctx.LoadValue(ctx.MapMetaKeyToCompiledKey(this.key));
			ctx.LoadValue((int)this.options);
			ctx.EmitCall(ctx.MapType(typeof(BclHelpers)).GetMethod("WriteNetObject"));
		}

		public object Read(object value, ProtoReader source)
		{
			Type type;
			object obj = value;
			ProtoReader protoReader = source;
			int num = this.key;
			if (this.type == typeof(object))
			{
				type = null;
			}
			else
			{
				type = this.type;
			}
			return BclHelpers.ReadNetObject(obj, protoReader, num, type, this.options);
		}

		public void Write(object value, ProtoWriter dest)
		{
			BclHelpers.WriteNetObject(value, dest, this.key, this.options);
		}
	}
}