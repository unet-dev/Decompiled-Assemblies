using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace System.ComponentModel
{
	[Preserve]
	public class PropertyChangingEventArgs : EventArgs
	{
		public virtual string PropertyName
		{
			get;
			set;
		}

		public PropertyChangingEventArgs(string propertyName)
		{
			this.PropertyName = propertyName;
		}
	}
}