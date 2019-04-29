using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class SetFactBoolean : BaseAction
	{
		[ApexSerialization]
		public BaseNpc.Facts fact;

		[ApexSerialization(defaultValue=false)]
		public bool @value;

		public SetFactBoolean()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			object obj;
			BaseContext baseContext = c;
			BaseNpc.Facts fact = this.fact;
			if (this.@value)
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			baseContext.SetFact(fact, (byte)obj);
		}
	}
}