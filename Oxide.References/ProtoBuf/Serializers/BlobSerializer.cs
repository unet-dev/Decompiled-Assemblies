using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class BlobSerializer : IProtoSerializer
	{
		private readonly static Type expectedType;

		private readonly bool overwriteList;

		public Type ExpectedType
		{
			get
			{
				return BlobSerializer.expectedType;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.RequiresOldValue
		{
			get
			{
				return !this.overwriteList;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.ReturnsValue
		{
			get
			{
				return true;
			}
		}

		static BlobSerializer()
		{
			BlobSerializer.expectedType = typeof(byte[]);
		}

		public BlobSerializer(TypeModel model, bool overwriteList)
		{
			this.overwriteList = overwriteList;
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			if (!this.overwriteList)
			{
				ctx.LoadValue(valueFrom);
			}
			else
			{
				ctx.LoadNullRef();
			}
			ctx.LoadReaderWriter();
			ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("AppendBytes"));
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitBasicWrite("WriteBytes", valueFrom);
		}

		public object Read(object value, ProtoReader source)
		{
			byte[] numArray;
			if (this.overwriteList)
			{
				numArray = null;
			}
			else
			{
				numArray = (byte[])value;
			}
			return ProtoReader.AppendBytes(numArray, source);
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteBytes((byte[])value, dest);
		}
	}
}