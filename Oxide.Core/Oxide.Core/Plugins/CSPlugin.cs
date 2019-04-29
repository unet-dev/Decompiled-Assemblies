using Oxide.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Oxide.Core.Plugins
{
	public abstract class CSPlugin : Plugin
	{
		protected Dictionary<string, List<HookMethod>> Hooks = new Dictionary<string, List<HookMethod>>();

		protected HookCache HooksCache = new HookCache();

		public CSPlugin()
		{
			string name;
			Type type = base.GetType();
			List<Type> types = new List<Type>()
			{
				type
			};
			while (type != typeof(CSPlugin))
			{
				Type baseType = type.BaseType;
				type = baseType;
				types.Add(baseType);
			}
			for (int i = types.Count - 1; i >= 0; i--)
			{
				MethodInfo[] methods = types[i].GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				for (int j = 0; j < (int)methods.Length; j++)
				{
					MethodInfo methodInfo = methods[j];
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(HookMethodAttribute), true);
					if ((int)customAttributes.Length >= 1)
					{
						HookMethodAttribute hookMethodAttribute = customAttributes[0] as HookMethodAttribute;
						if (hookMethodAttribute != null)
						{
							name = hookMethodAttribute.Name;
						}
						else
						{
							name = null;
						}
						this.AddHookMethod(name, methodInfo);
					}
				}
			}
		}

		protected void AddHookMethod(string name, MethodInfo method)
		{
			List<HookMethod> hookMethods;
			if (!this.Hooks.TryGetValue(name, out hookMethods))
			{
				hookMethods = new List<HookMethod>();
				this.Hooks[name] = hookMethods;
			}
			hookMethods.Add(new HookMethod(method));
		}

		protected List<HookMethod> FindHooks(string name, object[] args)
		{
			HookCache hookCache;
			object[] defaultValue;
			bool flag;
			List<HookMethod> hookMethod = this.HooksCache.GetHookMethod(name, args, out hookCache);
			if (hookMethod != null)
			{
				return hookMethod;
			}
			List<HookMethod> hookMethods = new List<HookMethod>();
			if (!this.Hooks.TryGetValue(name, out hookMethod))
			{
				return hookMethods;
			}
			HookMethod hookMethod1 = null;
			HookMethod hookMethod2 = null;
			foreach (HookMethod hookMethod3 in hookMethod)
			{
				if (!hookMethod3.IsBaseHook)
				{
					int num = (args != null ? (int)args.Length : 0);
					bool flag1 = false;
					if (num == (int)hookMethod3.Parameters.Length)
					{
						defaultValue = args;
					}
					else
					{
						defaultValue = ArrayPool.Get((int)hookMethod3.Parameters.Length);
						flag1 = true;
						if (num > 0 && defaultValue.Length != 0)
						{
							Array.Copy(args, defaultValue, Math.Min(num, (int)defaultValue.Length));
						}
						if ((int)defaultValue.Length > num)
						{
							for (int i = num; i < (int)defaultValue.Length; i++)
							{
								ParameterInfo parameters = hookMethod3.Parameters[i];
								if (parameters.DefaultValue != null && parameters.DefaultValue != DBNull.Value)
								{
									defaultValue[i] = parameters.DefaultValue;
								}
								else if (parameters.ParameterType.IsValueType)
								{
									defaultValue[i] = Activator.CreateInstance(parameters.ParameterType);
								}
							}
						}
					}
					if (hookMethod3.HasMatchingSignature(defaultValue, out flag))
					{
						if (!flag)
						{
							hookMethod2 = hookMethod3;
						}
						else
						{
							hookMethod1 = hookMethod3;
							if (hookMethod1 != null)
							{
								hookMethods.Add(hookMethod1);
							}
							else if (hookMethod2 != null)
							{
								hookMethods.Add(hookMethod2);
							}
							hookCache.SetupMethods(hookMethods);
							return hookMethods;
						}
					}
					if (!flag1)
					{
						continue;
					}
					ArrayPool.Free(defaultValue);
				}
				else
				{
					hookMethods.Add(hookMethod3);
				}
			}
			if (hookMethod1 != null)
			{
				hookMethods.Add(hookMethod1);
			}
			else if (hookMethod2 != null)
			{
				hookMethods.Add(hookMethod2);
			}
			hookCache.SetupMethods(hookMethods);
			return hookMethods;
		}

		public static T GetLibrary<T>(string name = null)
		where T : Library
		{
			return Interface.Oxide.GetLibrary<T>(name);
		}

		public override void HandleAddedToManager(PluginManager manager)
		{
			base.HandleAddedToManager(manager);
			foreach (string key in this.Hooks.Keys)
			{
				base.Subscribe(key);
			}
			try
			{
				this.OnCallHook("Init", null);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Interface.Oxide.LogException(string.Format("Failed to initialize plugin '{0} v{1}'", base.Name, base.Version), exception);
				if (base.Loader != null)
				{
					base.Loader.PluginErrors[base.Name] = exception.Message;
				}
			}
		}

		protected virtual object InvokeMethod(HookMethod method, object[] args)
		{
			return method.Method.Invoke(this, args);
		}

		protected sealed override object OnCallHook(string name, object[] args)
		{
			object[] defaultValue;
			object obj = null;
			bool flag = false;
			foreach (HookMethod hookMethod in this.FindHooks(name, args))
			{
				int num = (args != null ? (int)args.Length : 0);
				if (num == (int)hookMethod.Parameters.Length)
				{
					defaultValue = args;
				}
				else
				{
					defaultValue = ArrayPool.Get((int)hookMethod.Parameters.Length);
					flag = true;
					if (num > 0 && defaultValue.Length != 0)
					{
						Array.Copy(args, defaultValue, Math.Min(num, (int)defaultValue.Length));
					}
					if ((int)defaultValue.Length > num)
					{
						for (int i = num; i < (int)defaultValue.Length; i++)
						{
							ParameterInfo parameters = hookMethod.Parameters[i];
							if (parameters.DefaultValue != null && parameters.DefaultValue != DBNull.Value)
							{
								defaultValue[i] = parameters.DefaultValue;
							}
							else if (parameters.ParameterType.IsValueType)
							{
								defaultValue[i] = Activator.CreateInstance(parameters.ParameterType);
							}
						}
					}
				}
				try
				{
					obj = this.InvokeMethod(hookMethod, defaultValue);
				}
				catch (TargetInvocationException targetInvocationException1)
				{
					TargetInvocationException targetInvocationException = targetInvocationException1;
					if (flag)
					{
						ArrayPool.Free(defaultValue);
					}
					Exception innerException = targetInvocationException.InnerException;
					if (innerException == null)
					{
						innerException = targetInvocationException;
					}
					throw innerException;
				}
				if (num != (int)hookMethod.Parameters.Length)
				{
					for (int j = 0; j < (int)hookMethod.Parameters.Length; j++)
					{
						if (hookMethod.Parameters[j].IsOut || hookMethod.Parameters[j].ParameterType.IsByRef)
						{
							args[j] = defaultValue[j];
						}
					}
				}
				if (!flag)
				{
					continue;
				}
				ArrayPool.Free(defaultValue);
			}
			return obj;
		}
	}
}