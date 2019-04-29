using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal class TypeInformation
	{
		public System.Type Type
		{
			get;
			set;
		}

		public PrimitiveTypeCode TypeCode
		{
			get;
			set;
		}

		public TypeInformation()
		{
		}
	}
}