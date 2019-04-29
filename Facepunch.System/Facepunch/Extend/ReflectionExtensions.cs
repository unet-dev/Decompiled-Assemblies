using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Facepunch.Extend
{
	public static class ReflectionExtensions
	{
		public static bool HasAttribute(this MemberInfo method, Type attribute)
		{
			return method.GetCustomAttributes(attribute, true).Length != 0;
		}
	}
}