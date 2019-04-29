using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Plugins
{
	public class HookMethod
	{
		public string Name;

		public MethodInfo Method;

		public bool IsBaseHook
		{
			get;
			set;
		}

		public ParameterInfo[] Parameters
		{
			get;
			set;
		}

		public HookMethod(MethodInfo method)
		{
			this.Method = method;
			this.Name = method.Name;
			this.Parameters = this.Method.GetParameters();
			if (this.Parameters.Length != 0)
			{
				this.Name = string.Concat(this.Name, "(", string.Join(", ", (
					from x in (IEnumerable<ParameterInfo>)this.Parameters
					select x.ParameterType.ToString()).ToArray<string>()), ")");
			}
			this.IsBaseHook = this.Name.StartsWith("base_");
		}

		private bool CanAssignNull(Type type)
		{
			if (!type.IsValueType)
			{
				return true;
			}
			return Nullable.GetUnderlyingType(type) != null;
		}

		private bool CanConvertNumber(object value, Type type)
		{
			if (!this.IsNumber(value) || !this.IsNumber(type))
			{
				return false;
			}
			return TypeDescriptor.GetConverter(type).IsValid(value);
		}

		public bool HasMatchingSignature(object[] args, out bool exact)
		{
			exact = true;
			if (this.Parameters.Length == 0 && (args == null || args.Length == 0))
			{
				return true;
			}
			for (int i = 0; i < (int)args.Length; i++)
			{
				if (args[i] != null)
				{
					if (exact && args[i].GetType() != this.Parameters[i].ParameterType && args[i].GetType().MakeByRefType() != this.Parameters[i].ParameterType && !this.CanConvertNumber(args[i], this.Parameters[i].ParameterType))
					{
						exact = false;
					}
					if (!exact && !(args[i].GetType() == this.Parameters[i].ParameterType) && !(args[i].GetType().MakeByRefType() == this.Parameters[i].ParameterType) && !(this.Parameters[i].ParameterType.FullName == "System.Object"))
					{
						if (args[i].GetType().IsValueType)
						{
							if (!TypeDescriptor.GetConverter(this.Parameters[i].ParameterType).CanConvertFrom(args[i].GetType()) && !this.CanConvertNumber(args[i], this.Parameters[i].ParameterType))
							{
								return false;
							}
						}
						else if (!this.Parameters[i].ParameterType.IsInstanceOfType(args[i]))
						{
							return false;
						}
					}
				}
				else if (!this.CanAssignNull(this.Parameters[i].ParameterType))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsNumber(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			return this.IsNumber(Nullable.GetUnderlyingType(obj.GetType()) ?? obj.GetType());
		}

		private bool IsNumber(Type type)
		{
			if (!type.IsPrimitive)
			{
				return type == typeof(decimal);
			}
			if (!(type != typeof(bool)) || !(type != typeof(char)) || !(type != typeof(IntPtr)))
			{
				return false;
			}
			return type != typeof(UIntPtr);
		}
	}
}