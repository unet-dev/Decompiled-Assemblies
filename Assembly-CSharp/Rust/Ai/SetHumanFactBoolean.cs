using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class SetHumanFactBoolean : BaseAction
	{
		[ApexSerialization]
		public NPCPlayerApex.Facts fact;

		[ApexSerialization(defaultValue=false)]
		public bool @value;

		public SetHumanFactBoolean()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			object obj;
			BaseContext baseContext = c;
			NPCPlayerApex.Facts fact = this.fact;
			if (this.@value)
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			baseContext.SetFact(fact, (byte)obj, true, true);
		}
	}
}