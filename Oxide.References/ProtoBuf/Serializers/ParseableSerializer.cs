using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class ParseableSerializer : IProtoSerializer
	{
		private readonly MethodInfo parse;

		public Type ExpectedType
		{
			get
			{
				return this.parse.DeclaringType;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.RequiresOldValue
		{
			get
			{
				return false;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.ReturnsValue
		{
			get
			{
				return true;
			}
		}

		private ParseableSerializer(MethodInfo parse)
		{
			this.parse = parse;
		}

		private static MethodInfo GetCustomToString(Type type)
		{
			return type.GetMethod("ToString", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, null, Helpers.EmptyTypes, null);
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			ctx.EmitBasicRead("ReadString", ctx.MapType(typeof(string)));
			ctx.EmitCall(this.parse);
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			Type expectedType = this.ExpectedType;
			if (!expectedType.IsValueType)
			{
				ctx.EmitCall(ctx.MapType(typeof(object)).GetMethod("ToString"));
			}
			else
			{
				using (Local localWithValue = ctx.GetLocalWithValue(expectedType, valueFrom))
				{
					ctx.LoadAddress(localWithValue, expectedType);
					ctx.EmitCall(ParseableSerializer.GetCustomToString(expectedType));
				}
			}
			ctx.EmitBasicWrite("WriteString", valueFrom);
		}

		public object Read(object value, ProtoReader source)
		{
			MethodInfo methodInfo = this.parse;
			object[] objArray = new object[] { source.ReadString() };
			return methodInfo.Invoke(null, objArray);
		}

		public static ParseableSerializer TryCreate(Type type, TypeModel model)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Type[] typeArray = new Type[] { model.MapType(typeof(string)) };
			MethodInfo method = type.GetMethod("Parse", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public, null, typeArray, null);
			if (method == null || method.ReturnType != type)
			{
				return null;
			}
			if (Helpers.IsValueType(type))
			{
				MethodInfo customToString = ParseableSerializer.GetCustomToString(type);
				if (customToString == null || customToString.ReturnType != model.MapType(typeof(string)))
				{
					return null;
				}
			}
			return new ParseableSerializer(method);
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteString(value.ToString(), dest);
		}
	}
}