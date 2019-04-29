using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class CompiledSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		private readonly IProtoTypeSerializer head;

		private readonly ProtoSerializer serializer;

		private readonly ProtoDeserializer deserializer;

		Type ProtoBuf.Serializers.IProtoSerializer.ExpectedType
		{
			get
			{
				return this.head.ExpectedType;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.RequiresOldValue
		{
			get
			{
				return this.head.RequiresOldValue;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.ReturnsValue
		{
			get
			{
				return this.head.ReturnsValue;
			}
		}

		private CompiledSerializer(IProtoTypeSerializer head, TypeModel model)
		{
			this.head = head;
			this.serializer = CompilerContext.BuildSerializer(head, model);
			this.deserializer = CompilerContext.BuildDeserializer(head, model);
		}

		public void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
			this.head.Callback(value, callbackType, context);
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			this.head.EmitRead(ctx, valueFrom);
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			this.head.EmitWrite(ctx, valueFrom);
		}

		object ProtoBuf.Serializers.IProtoSerializer.Read(object value, ProtoReader source)
		{
			return this.deserializer(value, source);
		}

		void ProtoBuf.Serializers.IProtoSerializer.Write(object value, ProtoWriter dest)
		{
			this.serializer(value, dest);
		}

		bool ProtoBuf.Serializers.IProtoTypeSerializer.CanCreateInstance()
		{
			return this.head.CanCreateInstance();
		}

		object ProtoBuf.Serializers.IProtoTypeSerializer.CreateInstance(ProtoReader source)
		{
			return this.head.CreateInstance(source);
		}

		void ProtoBuf.Serializers.IProtoTypeSerializer.EmitCallback(CompilerContext ctx, Local valueFrom, TypeModel.CallbackType callbackType)
		{
			this.head.EmitCallback(ctx, valueFrom, callbackType);
		}

		void ProtoBuf.Serializers.IProtoTypeSerializer.EmitCreateInstance(CompilerContext ctx)
		{
			this.head.EmitCreateInstance(ctx);
		}

		bool ProtoBuf.Serializers.IProtoTypeSerializer.HasCallbacks(TypeModel.CallbackType callbackType)
		{
			return this.head.HasCallbacks(callbackType);
		}

		public static CompiledSerializer Wrap(IProtoTypeSerializer head, TypeModel model)
		{
			return head as CompiledSerializer ?? new CompiledSerializer(head, model);
		}
	}
}