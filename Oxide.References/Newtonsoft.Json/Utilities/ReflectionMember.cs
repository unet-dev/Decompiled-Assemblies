using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal class ReflectionMember
	{
		public Func<object, object> Getter
		{
			get;
			set;
		}

		public Type MemberType
		{
			get;
			set;
		}

		public Action<object, object> Setter
		{
			get;
			set;
		}

		public ReflectionMember()
		{
		}
	}
}