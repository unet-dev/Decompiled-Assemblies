using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Apex.Serialization
{
	public abstract class StageItem
	{
		public string name
		{
			get;
			set;
		}

		internal StageItem next
		{
			get;
			set;
		}

		public StageContainer parent
		{
			get;
			internal set;
		}

		protected StageItem(string name)
		{
			this.name = name;
		}

		public void Remove()
		{
			if (this.parent != null)
			{
				this.parent.Remove(this);
			}
		}

		public override string ToString()
		{
			return string.Concat(this.name, " (", this.GetType().Name, ")");
		}
	}
}