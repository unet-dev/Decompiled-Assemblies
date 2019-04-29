using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace System.ComponentModel
{
	[Preserve]
	public class AddingNewEventArgs
	{
		public object NewObject
		{
			get;
			set;
		}

		public AddingNewEventArgs()
		{
		}

		public AddingNewEventArgs(object newObject)
		{
			this.NewObject = newObject;
		}
	}
}