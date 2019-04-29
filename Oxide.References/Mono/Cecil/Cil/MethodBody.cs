using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Threading;

namespace Mono.Cecil.Cil
{
	public sealed class MethodBody : IVariableDefinitionProvider
	{
		internal readonly MethodDefinition method;

		internal ParameterDefinition this_parameter;

		internal int max_stack_size;

		internal int code_size;

		internal bool init_locals;

		internal MetadataToken local_var_token;

		internal Collection<Instruction> instructions;

		internal Collection<ExceptionHandler> exceptions;

		internal Collection<VariableDefinition> variables;

		private Mono.Cecil.Cil.Scope scope;

		public int CodeSize
		{
			get
			{
				return this.code_size;
			}
		}

		public Collection<ExceptionHandler> ExceptionHandlers
		{
			get
			{
				Collection<ExceptionHandler> exceptionHandlers = this.exceptions;
				if (exceptionHandlers == null)
				{
					Collection<ExceptionHandler> exceptionHandlers1 = new Collection<ExceptionHandler>();
					Collection<ExceptionHandler> exceptionHandlers2 = exceptionHandlers1;
					this.exceptions = exceptionHandlers1;
					exceptionHandlers = exceptionHandlers2;
				}
				return exceptionHandlers;
			}
		}

		public bool HasExceptionHandlers
		{
			get
			{
				return !this.exceptions.IsNullOrEmpty<ExceptionHandler>();
			}
		}

		public bool HasVariables
		{
			get
			{
				return !this.variables.IsNullOrEmpty<VariableDefinition>();
			}
		}

		public bool InitLocals
		{
			get
			{
				return this.init_locals;
			}
			set
			{
				this.init_locals = value;
			}
		}

		public Collection<Instruction> Instructions
		{
			get
			{
				Collection<Instruction> instructions = this.instructions;
				if (instructions == null)
				{
					InstructionCollection instructionCollection = new InstructionCollection();
					Collection<Instruction> instructions1 = instructionCollection;
					this.instructions = instructionCollection;
					instructions = instructions1;
				}
				return instructions;
			}
		}

		public MetadataToken LocalVarToken
		{
			get
			{
				return this.local_var_token;
			}
			set
			{
				this.local_var_token = value;
			}
		}

		public int MaxStackSize
		{
			get
			{
				return this.max_stack_size;
			}
			set
			{
				this.max_stack_size = value;
			}
		}

		public MethodDefinition Method
		{
			get
			{
				return this.method;
			}
		}

		public Mono.Cecil.Cil.Scope Scope
		{
			get
			{
				return this.scope;
			}
			set
			{
				this.scope = value;
			}
		}

		public ParameterDefinition ThisParameter
		{
			get
			{
				if (this.method == null || this.method.DeclaringType == null)
				{
					throw new NotSupportedException();
				}
				if (!this.method.HasThis)
				{
					return null;
				}
				if (this.this_parameter == null)
				{
					Interlocked.CompareExchange<ParameterDefinition>(ref this.this_parameter, MethodBody.CreateThisParameter(this.method), null);
				}
				return this.this_parameter;
			}
		}

		public Collection<VariableDefinition> Variables
		{
			get
			{
				Collection<VariableDefinition> variableDefinitions = this.variables;
				if (variableDefinitions == null)
				{
					VariableDefinitionCollection variableDefinitionCollection = new VariableDefinitionCollection();
					Collection<VariableDefinition> variableDefinitions1 = variableDefinitionCollection;
					this.variables = variableDefinitionCollection;
					variableDefinitions = variableDefinitions1;
				}
				return variableDefinitions;
			}
		}

		public MethodBody(MethodDefinition method)
		{
			this.method = method;
		}

		private static ParameterDefinition CreateThisParameter(MethodDefinition method)
		{
			TypeReference pointerType;
			TypeDefinition declaringType = method.DeclaringType;
			if (declaringType.IsValueType || declaringType.IsPrimitive)
			{
				pointerType = new PointerType(declaringType);
			}
			else
			{
				pointerType = declaringType;
			}
			return new ParameterDefinition(pointerType, method);
		}

		public ILProcessor GetILProcessor()
		{
			return new ILProcessor(this);
		}
	}
}