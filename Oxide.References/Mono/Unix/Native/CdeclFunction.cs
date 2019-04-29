using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Mono.Unix.Native
{
	public sealed class CdeclFunction
	{
		private readonly string library;

		private readonly string method;

		private readonly Type returnType;

		private readonly AssemblyName assemblyName;

		private readonly AssemblyBuilder assemblyBuilder;

		private readonly ModuleBuilder moduleBuilder;

		private Hashtable overloads;

		public CdeclFunction(string library, string method) : this(library, method, typeof(void))
		{
		}

		public CdeclFunction(string library, string method, Type returnType)
		{
			this.library = library;
			this.method = method;
			this.returnType = returnType;
			this.overloads = new Hashtable();
			this.assemblyName = new AssemblyName()
			{
				Name = string.Concat("Mono.Posix.Imports.", library)
			};
			this.assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(this.assemblyName, AssemblyBuilderAccess.Run);
			this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(this.assemblyName.Name);
		}

		private MethodInfo CreateMethod(Type[] parameterTypes)
		{
			MethodInfo methodInfo;
			string typeName = this.GetTypeName(parameterTypes);
			Hashtable hashtables = this.overloads;
			Monitor.Enter(hashtables);
			try
			{
				MethodInfo item = (MethodInfo)this.overloads[typeName];
				if (item == null)
				{
					TypeBuilder typeBuilder = this.CreateType(typeName);
					typeBuilder.DefinePInvokeMethod(this.method, this.library, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PinvokeImpl, CallingConventions.Standard, this.returnType, parameterTypes, CallingConvention.Cdecl, CharSet.Ansi);
					item = typeBuilder.CreateType().GetMethod(this.method);
					this.overloads.Add(typeName, item);
					methodInfo = item;
				}
				else
				{
					methodInfo = item;
				}
			}
			finally
			{
				Monitor.Exit(hashtables);
			}
			return methodInfo;
		}

		private TypeBuilder CreateType(string typeName)
		{
			return this.moduleBuilder.DefineType(typeName, TypeAttributes.Public);
		}

		private static Type GetMarshalType(Type t)
		{
			switch (Type.GetTypeCode(t))
			{
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				{
					return typeof(int);
				}
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				{
					return typeof(uint);
				}
				case TypeCode.Int64:
				{
					return typeof(long);
				}
				case TypeCode.UInt64:
				{
					return typeof(ulong);
				}
				case TypeCode.Single:
				case TypeCode.Double:
				{
					return typeof(double);
				}
			}
			return t;
		}

		private static Type[] GetParameterTypes(object[] parameters)
		{
			Type[] marshalType = new Type[(int)parameters.Length];
			for (int i = 0; i < (int)parameters.Length; i++)
			{
				marshalType[i] = CdeclFunction.GetMarshalType(parameters[i].GetType());
			}
			return marshalType;
		}

		private string GetTypeName(Type[] parameterTypes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[").Append(this.library).Append("] ").Append(this.method);
			stringBuilder.Append("(");
			if ((int)parameterTypes.Length > 0)
			{
				stringBuilder.Append(parameterTypes[0]);
			}
			for (int i = 1; i < (int)parameterTypes.Length; i++)
			{
				stringBuilder.Append(",").Append(parameterTypes[i]);
			}
			stringBuilder.Append(") : ").Append(this.returnType.FullName);
			return stringBuilder.ToString();
		}

		public object Invoke(object[] parameters)
		{
			Type[] parameterTypes = CdeclFunction.GetParameterTypes(parameters);
			return this.CreateMethod(parameterTypes).Invoke(null, parameters);
		}
	}
}