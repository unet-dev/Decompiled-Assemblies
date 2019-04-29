using Apex.Utilities;
using System;
using System.Runtime.CompilerServices;

namespace Apex.Serialization
{
	public class StageValue : StageItem
	{
		private string _value;

		public bool isText
		{
			get;
			private set;
		}

		public string @value
		{
			get
			{
				return this._value;
			}
			set
			{
				Ensure.ArgumentNotNull(value, "value");
				this._value = value;
			}
		}

		internal StageValue(string name, string value, bool isText) : base(name)
		{
			this.@value = value;
			this.isText = isText;
		}
	}
}