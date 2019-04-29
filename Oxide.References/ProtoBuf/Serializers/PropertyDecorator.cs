using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class PropertyDecorator : ProtoDecoratorBase
	{
		private readonly PropertyInfo property;

		private readonly Type forType;

		private readonly bool readOptionsWriteValue;

		private readonly MethodInfo shadowSetter;

		public override Type ExpectedType
		{
			get
			{
				return this.forType;
			}
		}

		public override bool RequiresOldValue
		{
			get
			{
				return true;
			}
		}

		public override bool ReturnsValue
		{
			get
			{
				return false;
			}
		}

		public PropertyDecorator(TypeModel model, Type forType, PropertyInfo property, IProtoSerializer tail) : base(tail)
		{
			this.forType = forType;
			this.property = property;
			PropertyDecorator.SanityCheck(model, property, tail, out this.readOptionsWriteValue, true, true);
			this.shadowSetter = PropertyDecorator.GetShadowSetter(model, property);
		}

		internal static bool CanWrite(TypeModel model, MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			PropertyInfo propertyInfo = member as PropertyInfo;
			if (propertyInfo == null)
			{
				return member is FieldInfo;
			}
			if (propertyInfo.CanWrite)
			{
				return true;
			}
			return PropertyDecorator.GetShadowSetter(model, propertyInfo) != null;
		}

		protected override void EmitRead(CompilerContext ctx, Local valueFrom)
		{
			bool flag;
			PropertyDecorator.SanityCheck(ctx.Model, this.property, this.Tail, out flag, ctx.NonPublic, ctx.AllowInternal(this.property));
			if (this.ExpectedType.IsValueType && valueFrom == null)
			{
				throw new InvalidOperationException("Attempt to mutate struct on the head of the stack; changes would be lost");
			}
			using (Local localWithValue = ctx.GetLocalWithValue(this.ExpectedType, valueFrom))
			{
				if (this.Tail.RequiresOldValue)
				{
					ctx.LoadAddress(localWithValue, this.ExpectedType);
					ctx.LoadValue(this.property);
				}
				Type propertyType = this.property.PropertyType;
				ctx.ReadNullCheckedTail(propertyType, this.Tail, null);
				if (flag)
				{
					using (Local local = new Local(ctx, this.property.PropertyType))
					{
						ctx.StoreValue(local);
						CodeLabel codeLabel = new CodeLabel();
						if (!propertyType.IsValueType)
						{
							codeLabel = ctx.DefineLabel();
							ctx.LoadValue(local);
							ctx.BranchIfFalse(codeLabel, true);
						}
						ctx.LoadAddress(localWithValue, this.ExpectedType);
						ctx.LoadValue(local);
						if (this.shadowSetter != null)
						{
							ctx.EmitCall(this.shadowSetter);
						}
						else
						{
							ctx.StoreValue(this.property);
						}
						if (!propertyType.IsValueType)
						{
							ctx.MarkLabel(codeLabel);
						}
					}
				}
				else if (this.Tail.ReturnsValue)
				{
					ctx.DiscardValue();
				}
			}
		}

		protected override void EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.LoadAddress(valueFrom, this.ExpectedType);
			ctx.LoadValue(this.property);
			ctx.WriteNullCheckedTail(this.property.PropertyType, this.Tail, null);
		}

		private static MethodInfo GetShadowSetter(TypeModel model, PropertyInfo property)
		{
			Type reflectedType = property.ReflectedType;
			string str = string.Concat("Set", property.Name);
			Type[] propertyType = new Type[] { property.PropertyType };
			MethodInfo instanceMethod = Helpers.GetInstanceMethod(reflectedType, str, propertyType);
			if (instanceMethod != null && instanceMethod.IsPublic && instanceMethod.ReturnType == model.MapType(typeof(void)))
			{
				return instanceMethod;
			}
			return null;
		}

		public override object Read(object value, ProtoReader source)
		{
			object obj = this.Tail.Read((this.Tail.RequiresOldValue ? this.property.GetValue(value, null) : null), source);
			if (this.readOptionsWriteValue && obj != null)
			{
				if (this.shadowSetter != null)
				{
					MethodInfo methodInfo = this.shadowSetter;
					object[] objArray = new object[] { obj };
					methodInfo.Invoke(value, objArray);
				}
				else
				{
					this.property.SetValue(value, obj, null);
				}
			}
			return null;
		}

		private static void SanityCheck(TypeModel model, PropertyInfo property, IProtoSerializer tail, out bool writeValue, bool nonPublic, bool allowInternal)
		{
			bool flag;
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (!tail.ReturnsValue)
			{
				flag = false;
			}
			else if (PropertyDecorator.GetShadowSetter(model, property) != null)
			{
				flag = true;
			}
			else
			{
				flag = (!property.CanWrite ? false : Helpers.GetSetMethod(property, nonPublic, allowInternal) != null);
			}
			writeValue = flag;
			if (!property.CanRead || Helpers.GetGetMethod(property, nonPublic, allowInternal) == null)
			{
				throw new InvalidOperationException("Cannot serialize property without a get accessor");
			}
			if (!writeValue && (!tail.RequiresOldValue || Helpers.IsValueType(tail.ExpectedType)))
			{
				throw new InvalidOperationException(string.Concat("Cannot apply changes to property ", property.DeclaringType.FullName, ".", property.Name));
			}
		}

		public override void Write(object value, ProtoWriter dest)
		{
			value = this.property.GetValue(value, null);
			if (value != null)
			{
				this.Tail.Write(value, dest);
			}
		}
	}
}