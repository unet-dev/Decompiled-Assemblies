using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class UriDecorator : ProtoDecoratorBase
	{
		private readonly static Type expectedType;

		public override Type ExpectedType
		{
			get
			{
				return UriDecorator.expectedType;
			}
		}

		public override bool RequiresOldValue
		{
			get
			{
				return false;
			}
		}

		public override bool ReturnsValue
		{
			get
			{
				return true;
			}
		}

		static UriDecorator()
		{
			UriDecorator.expectedType = typeof(Uri);
		}

		public UriDecorator(TypeModel model, IProtoSerializer tail) : base(tail)
		{
		}

		protected override void EmitRead(CompilerContext ctx, Local valueFrom)
		{
			this.Tail.EmitRead(ctx, valueFrom);
			ctx.CopyValue();
			CodeLabel codeLabel = ctx.DefineLabel();
			CodeLabel codeLabel1 = ctx.DefineLabel();
			ctx.LoadValue(typeof(string).GetProperty("Length"));
			ctx.BranchIfTrue(codeLabel, true);
			ctx.DiscardValue();
			ctx.LoadNullRef();
			ctx.Branch(codeLabel1, true);
			ctx.MarkLabel(codeLabel);
			Type type = ctx.MapType(typeof(Uri));
			Type[] typeArray = new Type[] { ctx.MapType(typeof(string)) };
			ctx.EmitCtor(type, typeArray);
			ctx.MarkLabel(codeLabel1);
		}

		protected override void EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ctx.LoadValue(valueFrom);
			ctx.LoadValue(typeof(Uri).GetProperty("AbsoluteUri"));
			this.Tail.EmitWrite(ctx, null);
		}

		public override object Read(object value, ProtoReader source)
		{
			string str = (string)this.Tail.Read(null, source);
			if (str.Length == 0)
			{
				return null;
			}
			return new Uri(str);
		}

		public override void Write(object value, ProtoWriter dest)
		{
			this.Tail.Write(((Uri)value).AbsoluteUri, dest);
		}
	}
}