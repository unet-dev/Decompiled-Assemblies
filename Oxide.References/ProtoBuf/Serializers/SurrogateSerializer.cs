using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class SurrogateSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		private readonly Type forType;

		private readonly Type declaredType;

		private readonly MethodInfo toTail;

		private readonly MethodInfo fromTail;

		private IProtoTypeSerializer rootTail;

		public Type ExpectedType
		{
			get
			{
				return this.forType;
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
				return false;
			}
		}

		public SurrogateSerializer(Type forType, Type declaredType, IProtoTypeSerializer rootTail)
		{
			this.forType = forType;
			this.declaredType = declaredType;
			this.rootTail = rootTail;
			this.toTail = this.GetConversion(true);
			this.fromTail = this.GetConversion(false);
		}

		public MethodInfo GetConversion(bool toTail)
		{
			MethodInfo methodInfo;
			Type type = (toTail ? this.declaredType : this.forType);
			Type type1 = (toTail ? this.forType : this.declaredType);
			if (!SurrogateSerializer.HasCast(this.declaredType, type1, type, out methodInfo) && !SurrogateSerializer.HasCast(this.forType, type1, type, out methodInfo))
			{
				throw new InvalidOperationException(string.Concat("No suitable conversion operator found for surrogate: ", this.forType.FullName, " / ", this.declaredType.FullName));
			}
			return methodInfo;
		}

		private static bool HasCast(Type type, Type from, Type to, out MethodInfo op)
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < (int)methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				if ((!(methodInfo.Name != "op_Implicit") || !(methodInfo.Name != "op_Explicit")) && methodInfo.ReturnType == to)
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if ((int)parameters.Length == 1 && parameters[0].ParameterType == from)
					{
						op = methodInfo;
						return true;
					}
				}
			}
			op = null;
			return false;
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			using (Local local = new Local(ctx, this.declaredType))
			{
				ctx.LoadValue(valueFrom);
				ctx.EmitCall(this.toTail);
				ctx.StoreValue(local);
				this.rootTail.EmitRead(ctx, local);
				ctx.LoadValue(local);
				ctx.EmitCall(this.fromTail);
				ctx.StoreValue(valueFrom);
			}
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.LoadValue(valueFrom);
			ctx.EmitCall(this.toTail);
			this.rootTail.EmitWrite(ctx, null);
		}

		void ProtoBuf.Serializers.IProtoTypeSerializer.Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
		}

		bool ProtoBuf.Serializers.IProtoTypeSerializer.CanCreateInstance()
		{
			return false;
		}

		object ProtoBuf.Serializers.IProtoTypeSerializer.CreateInstance(ProtoReader source)
		{
			throw new NotSupportedException();
		}

		void ProtoBuf.Serializers.IProtoTypeSerializer.EmitCallback(CompilerContext ctx, Local valueFrom, TypeModel.CallbackType callbackType)
		{
		}

		void ProtoBuf.Serializers.IProtoTypeSerializer.EmitCreateInstance(CompilerContext ctx)
		{
			throw new NotSupportedException();
		}

		bool ProtoBuf.Serializers.IProtoTypeSerializer.HasCallbacks(TypeModel.CallbackType callbackType)
		{
			return false;
		}

		public object Read(object value, ProtoReader source)
		{
			object[] objArray = new object[] { value };
			value = this.toTail.Invoke(null, objArray);
			objArray[0] = this.rootTail.Read(value, source);
			return this.fromTail.Invoke(null, objArray);
		}

		public void Write(object value, ProtoWriter writer)
		{
			IProtoTypeSerializer protoTypeSerializer = this.rootTail;
			MethodInfo methodInfo = this.toTail;
			object[] objArray = new object[] { value };
			protoTypeSerializer.Write(methodInfo.Invoke(null, objArray), writer);
		}
	}
}