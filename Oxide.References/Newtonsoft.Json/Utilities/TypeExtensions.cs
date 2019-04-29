using Newtonsoft.Json.Shims;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal static class TypeExtensions
	{
		public static Assembly Assembly(this Type type)
		{
			return type.Assembly;
		}

		public static bool AssignableToTypeName(this Type type, string fullTypeName, out Type match)
		{
			for (Type i = type; i != null; i = i.BaseType())
			{
				if (string.Equals(i.FullName, fullTypeName, StringComparison.Ordinal))
				{
					match = i;
					return true;
				}
			}
			Type[] interfaces = type.GetInterfaces();
			for (int j = 0; j < (int)interfaces.Length; j++)
			{
				if (string.Equals(interfaces[j].Name, fullTypeName, StringComparison.Ordinal))
				{
					match = type;
					return true;
				}
			}
			match = null;
			return false;
		}

		public static bool AssignableToTypeName(this Type type, string fullTypeName)
		{
			Type type1;
			return type.AssignableToTypeName(fullTypeName, out type1);
		}

		public static Type BaseType(this Type type)
		{
			return type.BaseType;
		}

		public static bool ContainsGenericParameters(this Type type)
		{
			return type.ContainsGenericParameters;
		}

		public static bool ImplementInterface(this Type type, Type interfaceType)
		{
			bool flag;
			Type type1 = type;
		Label2:
			while (type1 != null)
			{
				using (IEnumerator<Type> enumerator = ((IEnumerable<Type>)type1.GetInterfaces()).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Type current = enumerator.Current;
						if (current != interfaceType && (current == null || !current.ImplementInterface(interfaceType)))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					goto Label0;
				}
				return flag;
			}
			return false;
		Label0:
			type1 = type1.BaseType();
			goto Label2;
		}

		public static bool IsAbstract(this Type type)
		{
			return type.IsAbstract;
		}

		public static bool IsClass(this Type type)
		{
			return type.IsClass;
		}

		public static bool IsEnum(this Type type)
		{
			return type.IsEnum;
		}

		public static bool IsGenericType(this Type type)
		{
			return type.IsGenericType;
		}

		public static bool IsGenericTypeDefinition(this Type type)
		{
			return type.IsGenericTypeDefinition;
		}

		public static bool IsInterface(this Type type)
		{
			return type.IsInterface;
		}

		public static bool IsSealed(this Type type)
		{
			return type.IsSealed;
		}

		public static bool IsValueType(this Type type)
		{
			return type.IsValueType;
		}

		public static bool IsVisible(this Type type)
		{
			return type.IsVisible;
		}

		public static MemberTypes MemberType(this MemberInfo memberInfo)
		{
			return memberInfo.MemberType;
		}

		public static MethodInfo Method(this Delegate d)
		{
			return d.Method;
		}
	}
}