using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Core.Plugins.Watchers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Oxide.Plugins
{
	public abstract class CSharpPlugin : CSPlugin
	{
		public FSWatcher Watcher;

		protected Covalence covalence = Interface.Oxide.GetLibrary<Covalence>(null);

		protected Lang lang = Interface.Oxide.GetLibrary<Lang>(null);

		protected Oxide.Core.Libraries.Plugins plugins = Interface.Oxide.GetLibrary<Oxide.Core.Libraries.Plugins>(null);

		protected Permission permission = Interface.Oxide.GetLibrary<Permission>(null);

		protected WebRequests webrequest = Interface.Oxide.GetLibrary<WebRequests>(null);

		protected PluginTimers timer;

		protected HashSet<CSharpPlugin.PluginFieldInfo> onlinePlayerFields = new HashSet<CSharpPlugin.PluginFieldInfo>();

		private Dictionary<string, FieldInfo> pluginReferenceFields = new Dictionary<string, FieldInfo>();

		private bool hookDispatchFallback;

		protected Covalence universal = Interface.Oxide.GetLibrary<Covalence>(null);

		public bool HookedOnFrame
		{
			get;
			private set;
		}

		public CSharpPlugin()
		{
			int i;
			this.timer = new PluginTimers(this);
			Type type = base.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			for (i = 0; i < (int)fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(PluginReferenceAttribute), true);
				if (customAttributes.Length != 0)
				{
					PluginReferenceAttribute pluginReferenceAttribute = customAttributes[0] as PluginReferenceAttribute;
					this.pluginReferenceFields[pluginReferenceAttribute.Name ?? fieldInfo.Name] = fieldInfo;
				}
			}
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
			for (i = 0; i < (int)methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				if (methodInfo.GetCustomAttributes(typeof(HookMethodAttribute), true).Length == 0)
				{
					if (methodInfo.Name.Equals("OnFrame"))
					{
						this.HookedOnFrame = true;
					}
					if (methodInfo.DeclaringType.Name == type.Name)
					{
						base.AddHookMethod(methodInfo.Name, methodInfo);
					}
				}
			}
		}

		[HookMethod("OnPluginLoaded")]
		private void base_OnPluginLoaded(Plugin plugin)
		{
			FieldInfo fieldInfo;
			if (this.pluginReferenceFields.TryGetValue(plugin.Name, out fieldInfo))
			{
				fieldInfo.SetValue(this, plugin);
			}
		}

		[HookMethod("OnPluginUnloaded")]
		private void base_OnPluginUnloaded(Plugin plugin)
		{
			FieldInfo fieldInfo;
			if (this.pluginReferenceFields.TryGetValue(plugin.Name, out fieldInfo))
			{
				fieldInfo.SetValue(this, null);
			}
		}

		public virtual bool DirectCallHook(string name, out object ret, object[] args)
		{
			ret = null;
			return false;
		}

		public override void HandleAddedToManager(PluginManager manager)
		{
			base.HandleAddedToManager(manager);
			if (base.Filename != null)
			{
				this.Watcher.AddMapping(base.Name);
			}
			foreach (string key in this.pluginReferenceFields.Keys)
			{
				this.pluginReferenceFields[key].SetValue(this, manager.GetPlugin(key));
			}
			try
			{
				this.OnCallHook("Loaded", null);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Interface.Oxide.LogException(string.Format("Failed to initialize plugin '{0} v{1}'", base.Name, base.Version), exception);
				base.Loader.PluginErrors[base.Name] = exception.Message;
			}
		}

		public override void HandleRemovedFromManager(PluginManager manager)
		{
			if (base.IsLoaded)
			{
				base.CallHook("Unload", null);
			}
			this.Watcher.RemoveMapping(base.Name);
			foreach (string key in this.pluginReferenceFields.Keys)
			{
				this.pluginReferenceFields[key].SetValue(this, null);
			}
			base.HandleRemovedFromManager(manager);
		}

		protected override object InvokeMethod(HookMethod method, object[] args)
		{
			object obj;
			object obj1;
			bool compiledAssembly;
			if (!this.hookDispatchFallback && !method.IsBaseHook)
			{
				if (args != null && args.Length != 0)
				{
					ParameterInfo[] parameters = method.Parameters;
					for (int i = 0; i < (int)args.Length; i++)
					{
						object obj2 = args[i];
						if (obj2 != null)
						{
							Type parameterType = parameters[i].ParameterType;
							if (parameterType.IsValueType)
							{
								Type type = obj2.GetType();
								if (parameterType != typeof(object) && type != parameterType)
								{
									args[i] = Convert.ChangeType(obj2, parameterType);
								}
							}
						}
					}
				}
				try
				{
					if (!this.DirectCallHook(method.Name, out obj, args))
					{
						this.PrintWarning(string.Concat("Unable to call hook directly: ", method.Name), Array.Empty<object>());
						return method.Method.Invoke(this, args);
					}
					else
					{
						obj1 = obj;
					}
				}
				catch (InvalidProgramException invalidProgramException1)
				{
					InvalidProgramException invalidProgramException = invalidProgramException1;
					Interface.Oxide.LogError(string.Concat("Hook dispatch failure detected, falling back to reflection based dispatch. ", invalidProgramException), Array.Empty<object>());
					CompilablePlugin compilablePlugin = CSharpPluginLoader.GetCompilablePlugin(Interface.Oxide.PluginDirectory, base.Name);
					if (compilablePlugin != null)
					{
						compiledAssembly = compilablePlugin.CompiledAssembly;
					}
					else
					{
						compiledAssembly = false;
					}
					if (compiledAssembly)
					{
						File.WriteAllBytes(string.Concat(Interface.Oxide.PluginDirectory, "\\", base.Name, ".dump"), compilablePlugin.CompiledAssembly.PatchedAssembly);
						Interface.Oxide.LogWarning(string.Concat("The invalid raw assembly has been dumped to Plugins/", base.Name, ".dump"), Array.Empty<object>());
					}
					this.hookDispatchFallback = true;
					return method.Method.Invoke(this, args);
				}
				return obj1;
			}
			return method.Method.Invoke(this, args);
		}

		protected void LogToFile(string filename, string text, Plugin plugin, bool timeStamp = true)
		{
			string str = Path.Combine(Interface.Oxide.LogDirectory, plugin.Name);
			if (!Directory.Exists(str))
			{
				Directory.CreateDirectory(str);
			}
			string[] lower = new string[] { plugin.Name.ToLower(), "_", filename.ToLower(), null, null };
			lower[3] = (timeStamp ? string.Format("-{0:yyyy-MM-dd}", DateTime.Now) : "");
			lower[4] = ".txt";
			filename = string.Concat(lower);
			using (StreamWriter streamWriter = new StreamWriter(Path.Combine(str, Utility.CleanPath(filename)), true))
			{
				streamWriter.WriteLine(text);
			}
		}

		protected void NextFrame(Action callback)
		{
			Interface.Oxide.NextTick(callback);
		}

		protected void NextTick(Action callback)
		{
			Interface.Oxide.NextTick(callback);
		}

		protected void PrintError(string format, params object[] args)
		{
			OxideMod oxide = Interface.Oxide;
			object[] title = new object[] { base.Title, null };
			title[1] = (args.Length != 0 ? string.Format(format, args) : format);
			oxide.LogError("[{0}] {1}", title);
		}

		protected void PrintWarning(string format, params object[] args)
		{
			OxideMod oxide = Interface.Oxide;
			object[] title = new object[] { base.Title, null };
			title[1] = (args.Length != 0 ? string.Format(format, args) : format);
			oxide.LogWarning("[{0}] {1}", title);
		}

		protected void Puts(string format, params object[] args)
		{
			OxideMod oxide = Interface.Oxide;
			object[] title = new object[] { base.Title, null };
			title[1] = (args.Length != 0 ? string.Format(format, args) : format);
			oxide.LogInfo("[{0}] {1}", title);
		}

		protected void QueueWorkerThread(Action<object> callback)
		{
			ThreadPool.QueueUserWorkItem((object context) => {
				try
				{
					callback(context);
				}
				catch (Exception exception)
				{
					base.RaiseError(string.Format("Exception in '{0} v{1}' plugin worker thread: {2}", this.Name, this.Version, exception.ToString()));
				}
			});
		}

		public void SetFailState(string reason)
		{
			throw new PluginLoadFailure(reason);
		}

		public virtual bool SetPluginInfo(string name, string path)
		{
			base.Name = name;
			base.Filename = path;
			object[] customAttributes = base.GetType().GetCustomAttributes(typeof(InfoAttribute), true);
			if (customAttributes.Length == 0)
			{
				Interface.Oxide.LogWarning(string.Concat("Failed to load ", name, ": Info attribute missing"), Array.Empty<object>());
				return false;
			}
			InfoAttribute infoAttribute = customAttributes[0] as InfoAttribute;
			base.Title = infoAttribute.Title;
			base.Author = infoAttribute.Author;
			base.Version = infoAttribute.Version;
			base.ResourceId = infoAttribute.ResourceId;
			object[] objArray = base.GetType().GetCustomAttributes(typeof(DescriptionAttribute), true);
			if (objArray.Length != 0)
			{
				base.Description = (objArray[0] as DescriptionAttribute).Description;
			}
			MethodInfo method = base.GetType().GetMethod("LoadDefaultConfig", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			base.HasConfig = method.DeclaringType != typeof(Plugin);
			MethodInfo methodInfo = base.GetType().GetMethod("LoadDefaultMessages", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			base.HasMessages = methodInfo.DeclaringType != typeof(Plugin);
			return true;
		}

		public class PluginFieldInfo
		{
			public Plugin Plugin;

			public FieldInfo Field;

			public Type FieldType;

			public Type[] GenericArguments;

			public Dictionary<string, MethodInfo> Methods;

			public object Value
			{
				get
				{
					return this.Field.GetValue(this.Plugin);
				}
			}

			public PluginFieldInfo(Plugin plugin, FieldInfo field)
			{
				this.Plugin = plugin;
				this.Field = field;
				this.FieldType = field.FieldType;
				this.GenericArguments = this.FieldType.GetGenericArguments();
			}

			public object Call(string method_name, params object[] args)
			{
				MethodInfo method;
				if (!this.Methods.TryGetValue(method_name, out method))
				{
					method = this.FieldType.GetMethod(method_name, BindingFlags.Instance | BindingFlags.Public);
					this.Methods[method_name] = method;
				}
				if (method == null)
				{
					throw new MissingMethodException(this.FieldType.Name, method_name);
				}
				return method.Invoke(this.Value, args);
			}

			public bool HasValidConstructor(params Type[] argument_types)
			{
				Type genericArguments = this.GenericArguments[1];
				if (genericArguments.GetConstructor(new Type[0]) != null)
				{
					return true;
				}
				return genericArguments.GetConstructor(argument_types) != null;
			}

			public bool LookupMethod(string method_name, params Type[] argument_types)
			{
				MethodInfo method = this.FieldType.GetMethod(method_name, argument_types);
				if (method == null)
				{
					return false;
				}
				this.Methods[method_name] = method;
				return true;
			}
		}
	}
}