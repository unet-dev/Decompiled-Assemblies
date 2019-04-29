using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using Oxide.Core;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Oxide.Core.CSharp
{
	public class DirectCallMethod
	{
		private ModuleDefinition module;

		private TypeDefinition type;

		private MethodDefinition method;

		private MethodBody body;

		private Instruction endInstruction;

		private Dictionary<Instruction, DirectCallMethod.Node> jumpToEdgePlaceholderTargets = new Dictionary<Instruction, DirectCallMethod.Node>();

		private List<Instruction> jumpToEndPlaceholders = new List<Instruction>();

		private Dictionary<string, MethodDefinition> hookMethods = new Dictionary<string, MethodDefinition>();

		private MethodReference getLength;

		private MethodReference getChars;

		private MethodReference isNullOrEmpty;

		private MethodReference stringEquals;

		private string hook_attribute = typeof(HookMethodAttribute).FullName;

		public DirectCallMethod(ModuleDefinition module, TypeDefinition type)
		{
			DirectCallMethod.Node node;
			Func<MethodDefinition, bool> func = null;
			DirectCallMethod directCallMethod = this;
			this.module = module;
			this.type = type;
			this.getLength = module.Import(typeof(string).GetMethod("get_Length", new Type[0]));
			this.getChars = module.Import(typeof(string).GetMethod("get_Chars", new Type[] { typeof(int) }));
			this.isNullOrEmpty = module.Import(typeof(string).GetMethod("IsNullOrEmpty", new Type[] { typeof(string) }));
			this.stringEquals = module.Import(typeof(string).GetMethod("Equals", new Type[] { typeof(string) }));
			AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(Path.Combine(Interface.Oxide.ExtensionDirectory, "Oxide.CSharp.dll"));
			ModuleDefinition mainModule = assemblyDefinition.MainModule;
			TypeDefinition typeDefinition = module.Import(assemblyDefinition.MainModule.GetType("Oxide.Plugins.CSharpPlugin")).Resolve();
			MethodDefinition methodDefinition = module.Import(typeDefinition.Methods.First<MethodDefinition>((MethodDefinition method) => method.Name == "DirectCallHook")).Resolve();
			this.method = new MethodDefinition(methodDefinition.Name, methodDefinition.Attributes, mainModule.Import(methodDefinition.ReturnType))
			{
				DeclaringType = type
			};
			foreach (ParameterDefinition parameter in methodDefinition.Parameters)
			{
				ParameterDefinition parameterDefinition = new ParameterDefinition(parameter.Name, parameter.Attributes, mainModule.Import(parameter.ParameterType))
				{
					IsOut = parameter.IsOut,
					Constant = parameter.Constant,
					MarshalInfo = parameter.MarshalInfo,
					IsReturnValue = parameter.IsReturnValue
				};
				foreach (CustomAttribute customAttribute in parameter.CustomAttributes)
				{
					parameterDefinition.CustomAttributes.Add(new CustomAttribute(module.Import(customAttribute.Constructor)));
				}
				this.method.Parameters.Add(parameterDefinition);
			}
			foreach (CustomAttribute customAttribute1 in methodDefinition.CustomAttributes)
			{
				this.method.CustomAttributes.Add(new CustomAttribute(module.Import(customAttribute1.Constructor)));
			}
			this.method.ImplAttributes = methodDefinition.ImplAttributes;
			this.method.SemanticsAttributes = methodDefinition.SemanticsAttributes;
			MethodDefinition attributes = this.method;
			attributes.Attributes = attributes.Attributes & (MethodAttributes.MemberAccessMask | MethodAttributes.Private | MethodAttributes.FamANDAssem | MethodAttributes.Assembly | MethodAttributes.Family | MethodAttributes.FamORAssem | MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Abstract | MethodAttributes.SpecialName | MethodAttributes.PInvokeImpl | MethodAttributes.UnmanagedExport | MethodAttributes.RTSpecialName | MethodAttributes.HasSecurity | MethodAttributes.RequireSecObject);
			MethodDefinition attributes1 = this.method;
			attributes1.Attributes = attributes1.Attributes | MethodAttributes.CompilerControlled;
			this.body = new MethodBody(this.method);
			this.body.SimplifyMacros();
			this.method.Body = this.body;
			type.Methods.Add(this.method);
			this.body.Variables.Add(new VariableDefinition("name_size", module.TypeSystem.Int32));
			this.body.Variables.Add(new VariableDefinition("i", module.TypeSystem.Int32));
			this.AddInstruction(OpCodes.Ldarg_2);
			this.AddInstruction(OpCodes.Ldnull);
			this.AddInstruction(OpCodes.Stind_Ref);
			this.AddInstruction(OpCodes.Ldarg_1);
			this.AddInstruction(OpCodes.Call, this.isNullOrEmpty);
			Instruction instruction = this.AddInstruction(OpCodes.Brfalse, this.body.Instructions[0]);
			this.Return(false);
			instruction.Operand = this.AddInstruction(OpCodes.Ldarg_1);
			this.AddInstruction(OpCodes.Callvirt, this.getLength);
			this.AddInstruction(OpCodes.Stloc_0);
			this.AddInstruction(OpCodes.Ldc_I4_0);
			this.AddInstruction(OpCodes.Stloc_1);
			Collection<MethodDefinition> methods = type.Methods;
			Func<MethodDefinition, bool> func1 = func;
			if (func1 == null)
			{
				Func<MethodDefinition, bool> isStatic = (MethodDefinition m) => {
					if (m.IsStatic || !m.IsPrivate && !directCallMethod.IsHookMethod(m) || m.HasGenericParameters || m.ReturnType.IsGenericParameter || m.DeclaringType != type || m.IsSetter)
					{
						return false;
					}
					return !m.IsGetter;
				};
				Func<MethodDefinition, bool> func2 = isStatic;
				func = isStatic;
				func1 = func2;
			}
			foreach (MethodDefinition methodDefinition1 in methods.Where<MethodDefinition>(func1))
			{
				if (methodDefinition1.Name.Contains("<"))
				{
					continue;
				}
				string name = methodDefinition1.Name;
				if (methodDefinition1.Parameters.Count > 0)
				{
					name = string.Concat(name, "(", string.Join(", ", (
						from x in methodDefinition1.Parameters
						select x.ParameterType.ToString().Replace("/", "+").Replace("<", "[").Replace(">", "]")).ToArray<string>()), ")");
				}
				if (this.hookMethods.ContainsKey(name))
				{
					continue;
				}
				this.hookMethods[name] = methodDefinition1;
			}
			DirectCallMethod.Node node1 = new DirectCallMethod.Node();
			foreach (string key in this.hookMethods.Keys)
			{
				DirectCallMethod.Node node2 = node1;
				for (int i = 1; i <= key.Length; i++)
				{
					char chr = key[i - 1];
					if (!node2.Edges.TryGetValue(chr, out node))
					{
						node = new DirectCallMethod.Node()
						{
							Parent = node2,
							Char = chr
						};
						node2.Edges[chr] = node;
					}
					if (i == key.Length)
					{
						node.Name = key;
					}
					node2 = node;
				}
			}
			int num = 1;
			foreach (char key1 in node1.Edges.Keys)
			{
				int num1 = num;
				num = num1 + 1;
				this.BuildNode(node1.Edges[key1], num1);
			}
			this.endInstruction = this.Return(false);
			foreach (Instruction firstInstruction in this.jumpToEdgePlaceholderTargets.Keys)
			{
				firstInstruction.Operand = this.jumpToEdgePlaceholderTargets[firstInstruction].FirstInstruction;
			}
			foreach (Instruction jumpToEndPlaceholder in this.jumpToEndPlaceholders)
			{
				jumpToEndPlaceholder.Operand = this.endInstruction;
			}
			this.body.OptimizeMacros();
		}

		private Instruction AddInstruction(OpCode opcode)
		{
			return this.AddInstruction(Instruction.Create(opcode));
		}

		private Instruction AddInstruction(OpCode opcode, Instruction instruction)
		{
			return this.AddInstruction(Instruction.Create(opcode, instruction));
		}

		private Instruction AddInstruction(OpCode opcode, MethodReference method_reference)
		{
			return this.AddInstruction(Instruction.Create(opcode, method_reference));
		}

		private Instruction AddInstruction(OpCode opcode, TypeReference type_reference)
		{
			return this.AddInstruction(Instruction.Create(opcode, type_reference));
		}

		private Instruction AddInstruction(OpCode opcode, int value)
		{
			return this.AddInstruction(Instruction.Create(opcode, value));
		}

		private Instruction AddInstruction(OpCode opcode, VariableDefinition value)
		{
			return this.AddInstruction(Instruction.Create(opcode, value));
		}

		private Instruction AddInstruction(Instruction instruction)
		{
			this.body.Instructions.Add(instruction);
			return instruction;
		}

		public VariableDefinition AddVariable(TypeReference typeRef, string name = "")
		{
			VariableDefinition variableDefinition = new VariableDefinition(name, typeRef);
			this.body.Variables.Add(variableDefinition);
			return variableDefinition;
		}

		private void BuildNode(DirectCallMethod.Node node, int edge_number)
		{
			if (edge_number == 1)
			{
				node.FirstInstruction = this.AddInstruction(OpCodes.Ldloc_1);
				this.AddInstruction(OpCodes.Ldloc_0);
				this.jumpToEndPlaceholders.Add(this.AddInstruction(OpCodes.Bge, this.body.Instructions[0]));
			}
			if (edge_number != 1)
			{
				node.FirstInstruction = this.AddInstruction(OpCodes.Ldarg_1);
			}
			else
			{
				this.AddInstruction(OpCodes.Ldarg_1);
			}
			this.AddInstruction(OpCodes.Ldloc_1);
			this.AddInstruction(OpCodes.Callvirt, this.getChars);
			this.AddInstruction(this.Ldc_I4_n(node.Char));
			if (node.Parent.Edges.Count <= edge_number)
			{
				this.JumpToEnd();
			}
			else
			{
				this.JumpToEdge(node.Parent.Edges.Values.ElementAt<DirectCallMethod.Node>(edge_number));
			}
			if (node.Edges.Count == 1 && node.Name == null)
			{
				DirectCallMethod.Node node1 = node;
				while (node1.Edges.Count == 1 && node1.Name == null)
				{
					node1 = node1.Edges.Values.First<DirectCallMethod.Node>();
				}
				if (node1.Edges.Count == 0 && node1.Name != null)
				{
					this.AddInstruction(OpCodes.Ldarg_1);
					this.AddInstruction(Instruction.Create(OpCodes.Ldstr, node1.Name));
					this.AddInstruction(OpCodes.Callvirt, this.stringEquals);
					this.jumpToEndPlaceholders.Add(this.AddInstruction(OpCodes.Brfalse, this.body.Instructions[0]));
					this.CallMethod(this.hookMethods[node1.Name]);
					this.Return(true);
					return;
				}
			}
			this.AddInstruction(OpCodes.Ldloc_1);
			this.AddInstruction(OpCodes.Ldc_I4_1);
			this.AddInstruction(OpCodes.Add);
			this.AddInstruction(OpCodes.Stloc_1);
			if (node.Name != null)
			{
				this.AddInstruction(OpCodes.Ldloc_1);
				this.AddInstruction(OpCodes.Ldloc_0);
				if (node.Edges.Count <= 0)
				{
					this.JumpToEnd();
				}
				else
				{
					this.JumpToEdge(node.Edges.Values.First<DirectCallMethod.Node>());
				}
				this.CallMethod(this.hookMethods[node.Name]);
				this.Return(true);
			}
			int num = 1;
			foreach (char key in node.Edges.Keys)
			{
				int num1 = num;
				num = num1 + 1;
				this.BuildNode(node.Edges[key], num1);
			}
		}

		private void CallMethod(MethodDefinition method)
		{
			Dictionary<ParameterDefinition, VariableDefinition> parameterDefinitions = new Dictionary<ParameterDefinition, VariableDefinition>();
			for (int i = 0; i < method.Parameters.Count; i++)
			{
				ParameterDefinition item = method.Parameters[i];
				ByReferenceType parameterType = item.ParameterType as ByReferenceType;
				if (parameterType != null)
				{
					VariableDefinition variableDefinition = this.AddVariable(this.module.Import(parameterType.ElementType), "");
					this.AddInstruction(OpCodes.Ldarg_3);
					this.AddInstruction(this.Ldc_I4_n(i));
					this.AddInstruction(OpCodes.Ldelem_Ref);
					this.AddInstruction(OpCodes.Unbox_Any, this.module.Import(parameterType.ElementType));
					this.AddInstruction(OpCodes.Stloc_S, variableDefinition);
					parameterDefinitions[item] = variableDefinition;
				}
			}
			if (method.ReturnType.Name != "Void")
			{
				this.AddInstruction(OpCodes.Ldarg_2);
			}
			this.AddInstruction(OpCodes.Ldarg_0);
			for (int j = 0; j < method.Parameters.Count; j++)
			{
				ParameterDefinition parameterDefinition = method.Parameters[j];
				if (!(parameterDefinition.ParameterType is ByReferenceType))
				{
					this.AddInstruction(OpCodes.Ldarg_3);
					this.AddInstruction(this.Ldc_I4_n(j));
					this.AddInstruction(OpCodes.Ldelem_Ref);
					this.AddInstruction(OpCodes.Unbox_Any, this.module.Import(parameterDefinition.ParameterType));
				}
				else
				{
					this.AddInstruction(OpCodes.Ldloca, parameterDefinitions[parameterDefinition]);
				}
			}
			this.AddInstruction(OpCodes.Call, this.module.Import(method));
			for (int k = 0; k < method.Parameters.Count; k++)
			{
				ParameterDefinition item1 = method.Parameters[k];
				ByReferenceType byReferenceType = item1.ParameterType as ByReferenceType;
				if (byReferenceType != null)
				{
					this.AddInstruction(OpCodes.Ldarg_3);
					this.AddInstruction(this.Ldc_I4_n(k));
					this.AddInstruction(OpCodes.Ldloc_S, parameterDefinitions[item1]);
					this.AddInstruction(OpCodes.Box, this.module.Import(byReferenceType.ElementType));
					this.AddInstruction(OpCodes.Stelem_Ref);
				}
			}
			if (method.ReturnType.Name != "Void")
			{
				if (method.ReturnType.Name != "Object")
				{
					this.AddInstruction(OpCodes.Box, this.module.Import(method.ReturnType));
				}
				this.AddInstruction(OpCodes.Stind_Ref);
			}
		}

		private bool IsHookMethod(MethodDefinition method)
		{
			bool flag;
			Collection<CustomAttribute>.Enumerator enumerator = method.CustomAttributes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.AttributeType.FullName != this.hook_attribute)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private void JumpToEdge(DirectCallMethod.Node node)
		{
			Instruction instruction = this.AddInstruction(OpCodes.Bne_Un, this.body.Instructions[1]);
			this.jumpToEdgePlaceholderTargets[instruction] = node;
		}

		private void JumpToEnd()
		{
			this.jumpToEndPlaceholders.Add(this.AddInstruction(OpCodes.Bne_Un, this.body.Instructions[0]));
		}

		private Instruction Ldc_I4_n(int n)
		{
			if (n == 0)
			{
				return Instruction.Create(OpCodes.Ldc_I4_0);
			}
			if (n == 1)
			{
				return Instruction.Create(OpCodes.Ldc_I4_1);
			}
			if (n == 2)
			{
				return Instruction.Create(OpCodes.Ldc_I4_2);
			}
			if (n == 3)
			{
				return Instruction.Create(OpCodes.Ldc_I4_3);
			}
			if (n == 4)
			{
				return Instruction.Create(OpCodes.Ldc_I4_4);
			}
			if (n == 5)
			{
				return Instruction.Create(OpCodes.Ldc_I4_5);
			}
			if (n == 6)
			{
				return Instruction.Create(OpCodes.Ldc_I4_6);
			}
			if (n == 7)
			{
				return Instruction.Create(OpCodes.Ldc_I4_7);
			}
			if (n == 8)
			{
				return Instruction.Create(OpCodes.Ldc_I4_8);
			}
			return Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)n);
		}

		private Instruction Return(bool value)
		{
			Instruction instruction = this.AddInstruction(this.Ldc_I4_n((value ? 1 : 0)));
			this.AddInstruction(OpCodes.Ret);
			return instruction;
		}

		public class Node
		{
			public char Char;

			public string Name;

			public Dictionary<char, DirectCallMethod.Node> Edges;

			public DirectCallMethod.Node Parent;

			public Instruction FirstInstruction;

			public Node()
			{
			}
		}
	}
}