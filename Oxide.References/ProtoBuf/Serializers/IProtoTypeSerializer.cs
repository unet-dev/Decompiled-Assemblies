using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal interface IProtoTypeSerializer : IProtoSerializer
	{
		void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context);

		bool CanCreateInstance();

		object CreateInstance(ProtoReader source);

		void EmitCallback(CompilerContext ctx, Local valueFrom, TypeModel.CallbackType callbackType);

		void EmitCreateInstance(CompilerContext ctx);

		bool HasCallbacks(TypeModel.CallbackType callbackType);
	}
}