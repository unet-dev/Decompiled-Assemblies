using Apex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Apex.Utilities
{
	public static class ApexReflection
	{
		public static IEnumerable<Type> GetRelevantTypes()
		{
			return ((IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies()).Where<Assembly>((Assembly a) => {
				if (a.FullName.IndexOf("Assembly-CSharp", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
				return a.IsDefined(typeof(ApexRelevantAssemblyAttribute), false);
			}).SelectMany<Assembly, Type, Type>((Assembly a) => a.GetTypes(), (Assembly a, Type t) => t);
		}

		public static string ProperName(this Type t, bool fullName)
		{
			if (!t.IsGenericType)
			{
				if (!fullName)
				{
					return t.Name;
				}
				return t.FullName;
			}
			StringBuilder stringBuilder = new StringBuilder();
			ApexReflection.ProperName(t, fullName, stringBuilder);
			return stringBuilder.ToString();
		}

		private static void ProperName(Type t, bool fullName, StringBuilder b)
		{
			string str = (fullName ? t.FullName : t.Name);
			if (!t.IsGenericType)
			{
				b.Append(str);
				return;
			}
			Type[] genericArguments = t.GetGenericArguments();
			str = str.Substring(0, str.IndexOf('\u0060'));
			b.Append(str);
			b.Append('<');
			if (!t.IsGenericTypeDefinition)
			{
				ApexReflection.ProperName(genericArguments[0], fullName, b);
				for (int i = 1; i < (int)genericArguments.Length; i++)
				{
					b.Append(',');
					ApexReflection.ProperName(genericArguments[i], fullName, b);
				}
			}
			else
			{
				b.Append(',', (int)genericArguments.Length);
			}
			b.Append('>');
		}
	}
}