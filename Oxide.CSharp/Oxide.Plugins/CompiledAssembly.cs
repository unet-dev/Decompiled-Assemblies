using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using Oxide.Core;
using Oxide.Core.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Oxide.Plugins
{
	public class CompiledAssembly
	{
		public CompilablePlugin[] CompilablePlugins;

		public string[] PluginNames;

		public string Name;

		public DateTime CompiledAt;

		public byte[] RawAssembly;

		public byte[] PatchedAssembly;

		public float Duration;

		public Assembly LoadedAssembly;

		public bool IsLoading;

		private List<Action<bool>> loadCallbacks = new List<Action<bool>>();

		private bool isPatching;

		private bool isLoaded;

		private static IEnumerable<string> BlacklistedNamespaces
		{
			get
			{
				return new string[] { "Oxide.Core.ServerConsole", "System.IO", "System.Net", "System.Xml", "System.Reflection.Assembly", "System.Reflection.Emit", "System.Threading", "System.Runtime.InteropServices", "System.Diagnostics", "System.Security", "System.Timers", "Mono.CSharp", "Mono.Cecil", "ServerFileSystem" };
			}
		}

		public bool IsBatch
		{
			get
			{
				return (int)this.CompilablePlugins.Length > 1;
			}
		}

		private static IEnumerable<string> WhitelistedNamespaces
		{
			get
			{
				return new string[] { "System.Diagnostics.Stopwatch", "System.IO.MemoryStream", "System.IO.Stream", "System.IO.BinaryReader", "System.IO.BinaryWriter", "System.Net.Dns", "System.Net.Dns.GetHostEntry", "System.Net.IPAddress", "System.Net.IPEndPoint", "System.Net.NetworkInformation", "System.Net.Sockets.SocketFlags", "System.Security.Cryptography", "System.Threading.Interlocked" };
			}
		}

		public CompiledAssembly(string name, CompilablePlugin[] plugins, byte[] rawAssembly, float duration)
		{
			this.Name = name;
			this.CompilablePlugins = plugins;
			this.RawAssembly = rawAssembly;
			this.Duration = duration;
			this.PluginNames = (
				from pl in (IEnumerable<CompilablePlugin>)this.CompilablePlugins
				select pl.Name).ToArray<string>();
		}

		private bool IsCompilerGenerated(TypeDefinition type)
		{
			return type.CustomAttributes.Any<CustomAttribute>((CustomAttribute attr) => attr.Constructor.DeclaringType.ToString().Contains("CompilerGeneratedAttribute"));
		}

		private static bool IsNamespaceBlacklisted(string fullNamespace)
		{
			bool flag;
			using (IEnumerator<string> enumerator = CompiledAssembly.BlacklistedNamespaces.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					if (!fullNamespace.StartsWith(current) || CompiledAssembly.WhitelistedNamespaces.Any<string>(new Func<string, bool>(fullNamespace.StartsWith)))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public bool IsOutdated()
		{
			return this.CompilablePlugins.Any<CompilablePlugin>((CompilablePlugin pl) => pl.GetLastModificationTime() != this.CompiledAt);
		}

		public void LoadAssembly(Action<bool> callback)
		{
			if (this.isLoaded)
			{
				callback(true);
				return;
			}
			this.IsLoading = true;
			this.loadCallbacks.Add(callback);
			if (this.isPatching)
			{
				return;
			}
			this.PatchAssembly((byte[] rawAssembly) => {
				if (rawAssembly == null)
				{
					foreach (Action<bool> loadCallback in this.loadCallbacks)
					{
						loadCallback(true);
					}
					this.loadCallbacks.Clear();
					this.IsLoading = false;
					return;
				}
				this.LoadedAssembly = Assembly.Load(rawAssembly);
				this.isLoaded = true;
				foreach (Action<bool> action in this.loadCallbacks)
				{
					action(true);
				}
				this.loadCallbacks.Clear();
				this.IsLoading = false;
			});
		}

		private void PatchAssembly(Action<byte[]> callback)
		{
			Action action1 = null;
			if (this.isPatching)
			{
				Interface.Oxide.LogWarning("Already patching plugin assembly: {0} (ignoring)", new object[] { this.PluginNames.ToSentence<string>() });
				return;
			}
			float now = Interface.Oxide.Now;
			this.isPatching = true;
			ThreadPool.QueueUserWorkItem((object _) => {
				AssemblyDefinition assemblyDefinition;
				try
				{
					using (MemoryStream memoryStream = new MemoryStream(this.RawAssembly))
					{
						assemblyDefinition = AssemblyDefinition.ReadAssembly(memoryStream);
					}
					ConstructorInfo constructor = typeof(UnauthorizedAccessException).GetConstructor(new Type[] { typeof(string) });
					MethodReference methodReference1 = assemblyDefinition.MainModule.Import(constructor);
					Action<TypeDefinition> methods = null;
					methods = (TypeDefinition type) => {
						string fullName;
						string str;
						foreach (MethodDefinition method in type.Methods)
						{
							bool flag = false;
							if (method.Body != null)
							{
								bool flag1 = false;
								foreach (VariableDefinition variable in method.Body.Variables)
								{
									if (!CompiledAssembly.IsNamespaceBlacklisted(variable.VariableType.FullName))
									{
										continue;
									}
									Mono.Cecil.Cil.MethodBody methodBody = new Mono.Cecil.Cil.MethodBody(method);
									methodBody.Instructions.Add(Instruction.Create(OpCodes.Ldstr, string.Concat("System access is restricted, you are not allowed to use ", variable.VariableType.FullName)));
									methodBody.Instructions.Add(Instruction.Create(OpCodes.Newobj, methodReference1));
									methodBody.Instructions.Add(Instruction.Create(OpCodes.Throw));
									method.Body = methodBody;
									flag1 = true;
									break;
								}
								if (flag1)
								{
									continue;
								}
								Collection<Instruction> instructions = method.Body.Instructions;
								ILProcessor lProcessor = method.Body.GetILProcessor();
								Instruction instruction = instructions.First<Instruction>();
								for (int i = 0; i < instructions.Count && !flag; i++)
								{
									Instruction item = instructions[i];
									if (item.OpCode == OpCodes.Ldtoken)
									{
										IMetadataTokenProvider operand = item.Operand as IMetadataTokenProvider;
										str = (operand != null ? operand.ToString() : null);
										string str1 = str;
										if (CompiledAssembly.IsNamespaceBlacklisted(str1))
										{
											lProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, string.Concat("System access is restricted, you are not allowed to use ", str1)));
											lProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Newobj, methodReference1));
											lProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Throw));
											flag = true;
										}
									}
									else if (item.OpCode == OpCodes.Call || item.OpCode == OpCodes.Calli || item.OpCode == OpCodes.Callvirt || item.OpCode == OpCodes.Ldftn)
									{
										MethodReference methodReference = item.Operand as MethodReference;
										string str2 = (methodReference != null ? methodReference.DeclaringType.FullName : null);
										if (str2 == "System.Type" && methodReference.Name == "GetType" || CompiledAssembly.IsNamespaceBlacklisted(str2))
										{
											lProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, string.Concat("System access is restricted, you are not allowed to use ", str2)));
											lProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Newobj, methodReference1));
											lProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Throw));
											flag = true;
										}
									}
									else if (item.OpCode == OpCodes.Ldfld)
									{
										FieldReference fieldReference = item.Operand as FieldReference;
										fullName = (fieldReference != null ? fieldReference.FieldType.FullName : null);
										string str3 = fullName;
										if (CompiledAssembly.IsNamespaceBlacklisted(str3))
										{
											lProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, string.Concat("System access is restricted, you are not allowed to use ", str3)));
											lProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Newobj, methodReference1));
											lProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Throw));
											flag = true;
										}
									}
								}
							}
							else if (method.HasPInvokeInfo)
							{
								MethodDefinition attributes = method;
								attributes.Attributes = attributes.Attributes & (Mono.Cecil.MethodAttributes.MemberAccessMask | Mono.Cecil.MethodAttributes.Private | Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Assembly | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.FamORAssem | Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static | Mono.Cecil.MethodAttributes.Final | Mono.Cecil.MethodAttributes.Virtual | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.VtableLayoutMask | Mono.Cecil.MethodAttributes.NewSlot | Mono.Cecil.MethodAttributes.CheckAccessOnOverride | Mono.Cecil.MethodAttributes.Abstract | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.UnmanagedExport | Mono.Cecil.MethodAttributes.RTSpecialName | Mono.Cecil.MethodAttributes.HasSecurity | Mono.Cecil.MethodAttributes.RequireSecObject);
								Mono.Cecil.Cil.MethodBody methodBody1 = new Mono.Cecil.Cil.MethodBody(method);
								methodBody1.Instructions.Add(Instruction.Create(OpCodes.Ldstr, "PInvoke access is restricted, you are not allowed to use PInvoke"));
								methodBody1.Instructions.Add(Instruction.Create(OpCodes.Newobj, methodReference1));
								methodBody1.Instructions.Add(Instruction.Create(OpCodes.Throw));
								method.Body = methodBody1;
							}
							if (!flag)
							{
								continue;
							}
							Mono.Cecil.Cil.MethodBody body = method.Body;
							if (body != null)
							{
								body.OptimizeMacros();
							}
							else
							{
							}
						}
						foreach (TypeDefinition nestedType in type.NestedTypes)
						{
							methods(nestedType);
						}
					};
					foreach (TypeDefinition typeDefinition in assemblyDefinition.MainModule.Types)
					{
						methods(typeDefinition);
						if (this.IsCompilerGenerated(typeDefinition))
						{
							continue;
						}
						if (typeDefinition.Namespace != "Oxide.Plugins")
						{
							if (!(typeDefinition.FullName != "<Module>") || this.PluginNames.Any<string>((string plugin) => typeDefinition.FullName.StartsWith(string.Concat("Oxide.Plugins.", plugin))))
							{
								continue;
							}
							Interface.Oxide.LogWarning(((int)this.PluginNames.Length == 1 ? string.Concat(this.PluginNames[0], " has polluted the global namespace by defining ", typeDefinition.FullName) : string.Concat("A plugin has polluted the global namespace by defining ", typeDefinition.FullName)), Array.Empty<object>());
						}
						else if (!this.PluginNames.Contains<string>(typeDefinition.Name))
						{
							Interface.Oxide.LogWarning(((int)this.PluginNames.Length == 1 ? string.Concat(this.PluginNames[0], " has polluted the global namespace by defining ", typeDefinition.Name) : string.Concat("A plugin has polluted the global namespace by defining ", typeDefinition.Name)), Array.Empty<object>());
						}
						else
						{
							Collection<MethodDefinition> methodDefinitions = typeDefinition.Methods;
							Func<MethodDefinition, bool> u003cu003e9_204 = CompiledAssembly.<>c.<>9__20_4;
							if (u003cu003e9_204 == null)
							{
								u003cu003e9_204 = (MethodDefinition m) => {
									if (m.IsStatic || !m.IsConstructor || m.HasParameters)
									{
										return false;
									}
									return !m.IsPublic;
								};
								CompiledAssembly.<>c.<>9__20_4 = u003cu003e9_204;
							}
							if (methodDefinitions.FirstOrDefault<MethodDefinition>(u003cu003e9_204) == null)
							{
								DirectCallMethod directCallMethod = new DirectCallMethod(assemblyDefinition.MainModule, typeDefinition);
							}
							else
							{
								CompilablePlugin compilablePlugin = this.CompilablePlugins.SingleOrDefault<CompilablePlugin>((CompilablePlugin p) => p.Name == typeDefinition.Name);
								if (compilablePlugin == null)
								{
									continue;
								}
								compilablePlugin.CompilerErrors = "Primary constructor in main class must be public";
							}
						}
					}
					foreach (TypeDefinition typeDefinition1 in assemblyDefinition.MainModule.Types)
					{
						if (typeDefinition1.Namespace != "Oxide.Plugins" || !this.PluginNames.Contains<string>(typeDefinition1.Name))
						{
							continue;
						}
						Collection<MethodDefinition> methods1 = typeDefinition1.Methods;
						Func<MethodDefinition, bool> u003cu003e9_206 = CompiledAssembly.<>c.<>9__20_6;
						if (u003cu003e9_206 == null)
						{
							u003cu003e9_206 = (MethodDefinition m) => {
								if (m.IsStatic || m.HasGenericParameters || m.ReturnType.IsGenericParameter || m.IsSetter)
								{
									return false;
								}
								return !m.IsGetter;
							};
							CompiledAssembly.<>c.<>9__20_6 = u003cu003e9_206;
						}
						foreach (MethodDefinition methodDefinition in methods1.Where<MethodDefinition>(u003cu003e9_206))
						{
							foreach (ParameterDefinition parameter in methodDefinition.Parameters)
							{
								foreach (CustomAttribute customAttribute in parameter.CustomAttributes)
								{
								}
							}
						}
					}
					using (MemoryStream memoryStream1 = new MemoryStream())
					{
						assemblyDefinition.Write(memoryStream1);
						this.PatchedAssembly = memoryStream1.ToArray();
					}
					OxideMod oxide = Interface.Oxide;
					Action u003cu003e9_2 = action1;
					if (u003cu003e9_2 == null)
					{
						Action u003cu003e4_this = () => {
							this.isPatching = false;
							callback(this.PatchedAssembly);
						};
						Action action = u003cu003e4_this;
						action1 = u003cu003e4_this;
						u003cu003e9_2 = action;
					}
					oxide.NextTick(u003cu003e9_2);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					Interface.Oxide.NextTick(() => {
						this.isPatching = false;
						Interface.Oxide.LogException(string.Concat("Exception while patching: ", this.PluginNames.ToSentence<string>()), exception);
						callback(null);
					});
				}
			});
		}
	}
}