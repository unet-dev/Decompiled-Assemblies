using Oxide.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Libraries
{
	public abstract class Library
	{
		private IDictionary<string, MethodInfo> functions;

		private IDictionary<string, PropertyInfo> properties;

		public virtual bool IsGlobal
		{
			get;
		}

		public Exception LastException
		{
			get;
			protected set;
		}

		public Library()
		{
			int i;
			LibraryFunction libraryFunction;
			LibraryProperty libraryProperty;
			this.functions = new Dictionary<string, MethodInfo>();
			this.properties = new Dictionary<string, PropertyInfo>();
			Type type = this.GetType();
			MethodInfo[] methods = type.GetMethods();
			for (i = 0; i < (int)methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				try
				{
					libraryFunction = methodInfo.GetCustomAttributes(typeof(LibraryFunction), true).SingleOrDefault<object>() as LibraryFunction;
					if (libraryFunction == null)
					{
						goto Label0;
					}
				}
				catch (TypeLoadException typeLoadException)
				{
					goto Label0;
				}
				string name = libraryFunction.Name ?? methodInfo.Name;
				if (!this.functions.ContainsKey(name))
				{
					this.functions[name] = methodInfo;
				}
				else
				{
					Interface.Oxide.LogError(string.Concat(type.FullName, " library tried to register an already registered function: ", name), Array.Empty<object>());
				}
			Label0:
			}
			PropertyInfo[] properties = type.GetProperties();
			for (i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				try
				{
					libraryProperty = propertyInfo.GetCustomAttributes(typeof(LibraryProperty), true).SingleOrDefault<object>() as LibraryProperty;
					if (libraryProperty == null)
					{
						goto Label1;
					}
				}
				catch (TypeLoadException typeLoadException1)
				{
					goto Label1;
				}
				string str = libraryProperty.Name ?? propertyInfo.Name;
				if (!this.properties.ContainsKey(str))
				{
					this.properties[str] = propertyInfo;
				}
				else
				{
					Interface.Oxide.LogError("{0} library tried to register an already registered property: {1}", new object[] { type.FullName, str });
				}
			Label1:
			}
		}

		public MethodInfo GetFunction(string name)
		{
			MethodInfo methodInfo;
			if (!this.functions.TryGetValue(name, out methodInfo))
			{
				return null;
			}
			return methodInfo;
		}

		public IEnumerable<string> GetFunctionNames()
		{
			return this.functions.Keys;
		}

		public PropertyInfo GetProperty(string name)
		{
			PropertyInfo propertyInfo;
			if (!this.properties.TryGetValue(name, out propertyInfo))
			{
				return null;
			}
			return propertyInfo;
		}

		public IEnumerable<string> GetPropertyNames()
		{
			return this.properties.Keys;
		}

		public static implicit operator Boolean(Library library)
		{
			return library != null;
		}

		public static bool operator !(Library library)
		{
			return !library;
		}

		public virtual void Shutdown()
		{
		}
	}
}