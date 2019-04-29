using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class ErrorEventArgs : EventArgs
	{
		public object CurrentObject
		{
			get;
			private set;
		}

		public Newtonsoft.Json.Serialization.ErrorContext ErrorContext
		{
			get;
			private set;
		}

		public ErrorEventArgs(object currentObject, Newtonsoft.Json.Serialization.ErrorContext errorContext)
		{
			this.CurrentObject = currentObject;
			this.ErrorContext = errorContext;
		}
	}
}