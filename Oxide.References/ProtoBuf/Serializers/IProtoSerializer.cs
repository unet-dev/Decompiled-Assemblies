using ProtoBuf;
using ProtoBuf.Compiler;
using System;

namespace ProtoBuf.Serializers
{
	internal interface IProtoSerializer
	{
		Type ExpectedType
		{
			get;
		}

		bool RequiresOldValue
		{
			get;
		}

		bool ReturnsValue
		{
			get;
		}

		void EmitRead(CompilerContext ctx, Local entity);

		void EmitWrite(CompilerContext ctx, Local valueFrom);

		object Read(object value, ProtoReader source);

		void Write(object value, ProtoWriter dest);
	}
}