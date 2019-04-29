using Mono.Cecil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Cil
{
	internal sealed class CodeWriter : ByteBuffer
	{
		private readonly uint code_base;

		internal readonly MetadataBuilder metadata;

		private readonly Dictionary<uint, MetadataToken> standalone_signatures;

		private uint current;

		private MethodBody body;

		public CodeWriter(MetadataBuilder metadata) : base(0)
		{
			this.code_base = metadata.text_map.GetNextRVA(TextSegment.CLIHeader);
			this.current = this.code_base;
			this.metadata = metadata;
			this.standalone_signatures = new Dictionary<uint, MetadataToken>();
		}

		private static void AddExceptionStackSize(Instruction handler_start, ref Dictionary<Instruction, int> stack_sizes)
		{
			if (handler_start == null)
			{
				return;
			}
			if (stack_sizes == null)
			{
				stack_sizes = new Dictionary<Instruction, int>();
			}
			stack_sizes[handler_start] = 1;
		}

		private void Align(int align)
		{
			align--;
			base.WriteBytes((this.position + align & ~align) - this.position);
		}

		private uint BeginMethod()
		{
			return this.current;
		}

		private void ComputeExceptionHandlerStackSize(ref Dictionary<Instruction, int> stack_sizes)
		{
			Collection<ExceptionHandler> exceptionHandlers = this.body.ExceptionHandlers;
			for (int i = 0; i < exceptionHandlers.Count; i++)
			{
				ExceptionHandler item = exceptionHandlers[i];
				ExceptionHandlerType handlerType = item.HandlerType;
				if (handlerType == ExceptionHandlerType.Catch)
				{
					CodeWriter.AddExceptionStackSize(item.HandlerStart, ref stack_sizes);
				}
				else if (handlerType == ExceptionHandlerType.Filter)
				{
					CodeWriter.AddExceptionStackSize(item.FilterStart, ref stack_sizes);
					CodeWriter.AddExceptionStackSize(item.HandlerStart, ref stack_sizes);
				}
			}
		}

		private void ComputeHeader()
		{
			int size = 0;
			Collection<Instruction> instructions = this.body.instructions;
			Instruction[] instructionArray = instructions.items;
			int num = instructions.size;
			int num1 = 0;
			int num2 = 0;
			Dictionary<Instruction, int> instructions1 = null;
			if (this.body.HasExceptionHandlers)
			{
				this.ComputeExceptionHandlerStackSize(ref instructions1);
			}
			for (int i = 0; i < num; i++)
			{
				Instruction instruction = instructionArray[i];
				instruction.offset = size;
				size += instruction.GetSize();
				CodeWriter.ComputeStackSize(instruction, ref instructions1, ref num1, ref num2);
			}
			this.body.code_size = size;
			this.body.max_stack_size = num2;
		}

		private static void ComputePopDelta(StackBehaviour pop_behavior, ref int stack_size)
		{
			switch (pop_behavior)
			{
				case StackBehaviour.Pop1:
				case StackBehaviour.Popi:
				case StackBehaviour.Popref:
				{
					stack_size--;
					return;
				}
				case StackBehaviour.Pop1_pop1:
				case StackBehaviour.Popi_pop1:
				case StackBehaviour.Popi_popi:
				case StackBehaviour.Popi_popi8:
				case StackBehaviour.Popi_popr4:
				case StackBehaviour.Popi_popr8:
				case StackBehaviour.Popref_pop1:
				case StackBehaviour.Popref_popi:
				{
					stack_size -= 2;
					return;
				}
				case StackBehaviour.Popi_popi_popi:
				case StackBehaviour.Popref_popi_popi:
				case StackBehaviour.Popref_popi_popi8:
				case StackBehaviour.Popref_popi_popr4:
				case StackBehaviour.Popref_popi_popr8:
				case StackBehaviour.Popref_popi_popref:
				{
					stack_size -= 3;
					return;
				}
				case StackBehaviour.PopAll:
				{
					stack_size = 0;
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private static void ComputePushDelta(StackBehaviour push_behaviour, ref int stack_size)
		{
			switch (push_behaviour)
			{
				case StackBehaviour.Push1:
				case StackBehaviour.Pushi:
				case StackBehaviour.Pushi8:
				case StackBehaviour.Pushr4:
				case StackBehaviour.Pushr8:
				case StackBehaviour.Pushref:
				{
					stack_size++;
					return;
				}
				case StackBehaviour.Push1_push1:
				{
					stack_size += 2;
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private static void ComputeStackDelta(Instruction instruction, ref int stack_size)
		{
			if (instruction.opcode.FlowControl != FlowControl.Call)
			{
				CodeWriter.ComputePopDelta(instruction.opcode.StackBehaviourPop, ref stack_size);
				CodeWriter.ComputePushDelta(instruction.opcode.StackBehaviourPush, ref stack_size);
			}
			else
			{
				IMethodSignature methodSignature = (IMethodSignature)instruction.operand;
				if (methodSignature.HasImplicitThis() && instruction.opcode.Code != Code.Newobj)
				{
					stack_size--;
				}
				if (methodSignature.HasParameters)
				{
					stack_size -= methodSignature.Parameters.Count;
				}
				if (instruction.opcode.Code == Code.Calli)
				{
					stack_size--;
				}
				if (methodSignature.ReturnType.etype != ElementType.Void || instruction.opcode.Code == Code.Newobj)
				{
					stack_size++;
					return;
				}
			}
		}

		private static void ComputeStackSize(Instruction instruction, ref Dictionary<Instruction, int> stack_sizes, ref int stack_size, ref int max_stack)
		{
			int num;
			if (stack_sizes != null && stack_sizes.TryGetValue(instruction, out num))
			{
				stack_size = num;
			}
			max_stack = System.Math.Max(max_stack, stack_size);
			CodeWriter.ComputeStackDelta(instruction, ref stack_size);
			max_stack = System.Math.Max(max_stack, stack_size);
			CodeWriter.CopyBranchStackSize(instruction, ref stack_sizes, stack_size);
			CodeWriter.ComputeStackSize(instruction, ref stack_size);
		}

		private static void ComputeStackSize(Instruction instruction, ref int stack_size)
		{
			FlowControl flowControl = instruction.opcode.FlowControl;
			if (flowControl <= FlowControl.Break)
			{
				if (flowControl != FlowControl.Branch && flowControl != FlowControl.Break)
				{
					return;
				}
			}
			else if (flowControl != FlowControl.Return && flowControl != FlowControl.Throw)
			{
				return;
			}
			stack_size = 0;
		}

		private static void CopyBranchStackSize(Instruction instruction, ref Dictionary<Instruction, int> stack_sizes, int stack_size)
		{
			if (stack_size == 0)
			{
				return;
			}
			OperandType operandType = instruction.opcode.OperandType;
			if (operandType != OperandType.InlineBrTarget)
			{
				if (operandType == OperandType.InlineSwitch)
				{
					Instruction[] instructionArray = (Instruction[])instruction.operand;
					for (int i = 0; i < (int)instructionArray.Length; i++)
					{
						CodeWriter.CopyBranchStackSize(ref stack_sizes, instructionArray[i], stack_size);
					}
				}
				else if (operandType == OperandType.ShortInlineBrTarget)
				{
					CodeWriter.CopyBranchStackSize(ref stack_sizes, (Instruction)instruction.operand, stack_size);
					return;
				}
				return;
			}
			CodeWriter.CopyBranchStackSize(ref stack_sizes, (Instruction)instruction.operand, stack_size);
		}

		private static void CopyBranchStackSize(ref Dictionary<Instruction, int> stack_sizes, Instruction target, int stack_size)
		{
			int num;
			if (stack_sizes == null)
			{
				stack_sizes = new Dictionary<Instruction, int>();
			}
			int stackSize = stack_size;
			if (stack_sizes.TryGetValue(target, out num))
			{
				stackSize = System.Math.Max(stackSize, num);
			}
			stack_sizes[target] = stackSize;
		}

		private void EndMethod()
		{
			this.current = (uint)((ulong)this.code_base + (long)this.position);
		}

		private static MetadataToken GetLocalVarToken(ByteBuffer buffer, MethodSymbols symbols)
		{
			if (symbols.variables.IsNullOrEmpty<VariableDefinition>())
			{
				return MetadataToken.Zero;
			}
			buffer.position = 8;
			return new MetadataToken(buffer.ReadUInt32());
		}

		private int GetParameterIndex(ParameterDefinition parameter)
		{
			if (!this.body.method.HasThis)
			{
				return parameter.Index;
			}
			if (parameter == this.body.this_parameter)
			{
				return 0;
			}
			return parameter.Index + 1;
		}

		public MetadataToken GetStandAloneSignature(Collection<VariableDefinition> variables)
		{
			return this.GetStandAloneSignatureToken(this.metadata.GetLocalVariableBlobIndex(variables));
		}

		public MetadataToken GetStandAloneSignature(Mono.Cecil.CallSite call_site)
		{
			MetadataToken standAloneSignatureToken = this.GetStandAloneSignatureToken(this.metadata.GetCallSiteBlobIndex(call_site));
			call_site.MetadataToken = standAloneSignatureToken;
			return standAloneSignatureToken;
		}

		private MetadataToken GetStandAloneSignatureToken(uint signature)
		{
			MetadataToken metadataToken;
			if (this.standalone_signatures.TryGetValue(signature, out metadataToken))
			{
				return metadataToken;
			}
			metadataToken = new MetadataToken(Mono.Cecil.TokenType.Signature, this.metadata.AddStandAloneSignature(signature));
			this.standalone_signatures.Add(signature, metadataToken);
			return metadataToken;
		}

		private int GetTargetOffset(Instruction instruction)
		{
			if (instruction != null)
			{
				return instruction.offset;
			}
			Instruction item = this.body.instructions[this.body.instructions.size - 1];
			return item.offset + item.GetSize();
		}

		private uint GetUserStringIndex(string @string)
		{
			if (@string == null)
			{
				return (uint)0;
			}
			return this.metadata.user_string_heap.GetStringIndex(@string);
		}

		private static int GetVariableIndex(VariableDefinition variable)
		{
			return variable.Index;
		}

		private static bool IsEmptyMethodBody(MethodBody body)
		{
			if (!body.instructions.IsNullOrEmpty<Instruction>())
			{
				return false;
			}
			return body.variables.IsNullOrEmpty<VariableDefinition>();
		}

		private static bool IsFatRange(Instruction start, Instruction end)
		{
			if (start == null)
			{
				throw new ArgumentException();
			}
			if (end == null)
			{
				return true;
			}
			if (end.Offset - start.Offset > 255)
			{
				return true;
			}
			return start.Offset > 65535;
		}

		private static bool IsUnresolved(MethodDefinition method)
		{
			if (!method.HasBody || !method.HasImage)
			{
				return false;
			}
			return method.body == null;
		}

		private bool RequiresFatHeader()
		{
			MethodBody methodBody = this.body;
			if (methodBody.CodeSize >= 64 || methodBody.InitLocals || methodBody.HasVariables || methodBody.HasExceptionHandlers)
			{
				return true;
			}
			return methodBody.MaxStackSize > 8;
		}

		private static bool RequiresFatSection(Collection<ExceptionHandler> handlers)
		{
			for (int i = 0; i < handlers.Count; i++)
			{
				ExceptionHandler item = handlers[i];
				if (CodeWriter.IsFatRange(item.TryStart, item.TryEnd))
				{
					return true;
				}
				if (CodeWriter.IsFatRange(item.HandlerStart, item.HandlerEnd))
				{
					return true;
				}
				if (item.HandlerType == ExceptionHandlerType.Filter && CodeWriter.IsFatRange(item.FilterStart, item.HandlerStart))
				{
					return true;
				}
			}
			return false;
		}

		private void WriteExceptionHandlers()
		{
			this.Align(4);
			Collection<ExceptionHandler> exceptionHandlers = this.body.ExceptionHandlers;
			if (exceptionHandlers.Count < 21 && !CodeWriter.RequiresFatSection(exceptionHandlers))
			{
				this.WriteSmallSection(exceptionHandlers);
				return;
			}
			this.WriteFatSection(exceptionHandlers);
		}

		private void WriteExceptionHandlers(Collection<ExceptionHandler> handlers, Action<int> write_entry, Action<int> write_length)
		{
			for (int i = 0; i < handlers.Count; i++)
			{
				ExceptionHandler item = handlers[i];
				write_entry(item.HandlerType);
				write_entry(item.TryStart.Offset);
				write_length(this.GetTargetOffset(item.TryEnd) - item.TryStart.Offset);
				write_entry(item.HandlerStart.Offset);
				write_length(this.GetTargetOffset(item.HandlerEnd) - item.HandlerStart.Offset);
				this.WriteExceptionHandlerSpecific(item);
			}
		}

		private void WriteExceptionHandlerSpecific(ExceptionHandler handler)
		{
			ExceptionHandlerType handlerType = handler.HandlerType;
			if (handlerType == ExceptionHandlerType.Catch)
			{
				this.WriteMetadataToken(this.metadata.LookupToken(handler.CatchType));
				return;
			}
			if (handlerType != ExceptionHandlerType.Filter)
			{
				base.WriteInt32(0);
				return;
			}
			base.WriteInt32(handler.FilterStart.Offset);
		}

		private void WriteFatHeader()
		{
			MethodBody methodBody = this.body;
			byte num = 3;
			if (methodBody.InitLocals)
			{
				num = (byte)(num | 16);
			}
			if (methodBody.HasExceptionHandlers)
			{
				num = (byte)(num | 8);
			}
			base.WriteByte(num);
			base.WriteByte(48);
			base.WriteInt16((short)methodBody.max_stack_size);
			base.WriteInt32(methodBody.code_size);
			methodBody.local_var_token = (methodBody.HasVariables ? this.GetStandAloneSignature(methodBody.Variables) : MetadataToken.Zero);
			this.WriteMetadataToken(methodBody.local_var_token);
		}

		private void WriteFatSection(Collection<ExceptionHandler> handlers)
		{
			base.WriteByte(65);
			int count = handlers.Count * 24 + 4;
			base.WriteByte((byte)(count & 255));
			base.WriteByte((byte)(count >> 8 & 255));
			base.WriteByte((byte)(count >> 16 & 255));
			this.WriteExceptionHandlers(handlers, new Action<int>(this.WriteInt32), new Action<int>(this.WriteInt32));
		}

		private void WriteInstructions()
		{
			Collection<Instruction> instructions = this.body.Instructions;
			Instruction[] instructionArray = instructions.items;
			int num = instructions.size;
			for (int i = 0; i < num; i++)
			{
				Instruction instruction = instructionArray[i];
				this.WriteOpCode(instruction.opcode);
				this.WriteOperand(instruction);
			}
		}

		private void WriteMetadataToken(MetadataToken token)
		{
			base.WriteUInt32(token.ToUInt32());
		}

		public uint WriteMethodBody(MethodDefinition method)
		{
			uint num = this.BeginMethod();
			if (!CodeWriter.IsUnresolved(method))
			{
				if (CodeWriter.IsEmptyMethodBody(method.Body))
				{
					return (uint)0;
				}
				this.WriteResolvedMethodBody(method);
			}
			else
			{
				if (method.rva == 0)
				{
					return (uint)0;
				}
				this.WriteUnresolvedMethodBody(method);
			}
			this.Align(4);
			this.EndMethod();
			return num;
		}

		private void WriteOpCode(OpCode opcode)
		{
			if (opcode.Size == 1)
			{
				base.WriteByte(opcode.Op2);
				return;
			}
			base.WriteByte(opcode.Op1);
			base.WriteByte(opcode.Op2);
		}

		private void WriteOperand(Instruction instruction)
		{
			OpCode opCode = instruction.opcode;
			OperandType operandType = opCode.OperandType;
			if (operandType == OperandType.InlineNone)
			{
				return;
			}
			object obj = instruction.operand;
			if (obj == null)
			{
				throw new ArgumentException();
			}
			switch (operandType)
			{
				case OperandType.InlineBrTarget:
				{
					Instruction instruction1 = (Instruction)obj;
					base.WriteInt32(this.GetTargetOffset(instruction1) - (instruction.Offset + opCode.Size + 4));
					return;
				}
				case OperandType.InlineField:
				case OperandType.InlineMethod:
				case OperandType.InlineTok:
				case OperandType.InlineType:
				{
					this.WriteMetadataToken(this.metadata.LookupToken((IMetadataTokenProvider)obj));
					return;
				}
				case OperandType.InlineI:
				{
					base.WriteInt32((int)obj);
					return;
				}
				case OperandType.InlineI8:
				{
					base.WriteInt64((long)obj);
					return;
				}
				case OperandType.InlineNone:
				case OperandType.InlinePhi:
				{
					throw new ArgumentException();
				}
				case OperandType.InlineR:
				{
					base.WriteDouble((double)obj);
					return;
				}
				case OperandType.InlineSig:
				{
					this.WriteMetadataToken(this.GetStandAloneSignature((Mono.Cecil.CallSite)obj));
					return;
				}
				case OperandType.InlineString:
				{
					this.WriteMetadataToken(new MetadataToken(Mono.Cecil.TokenType.String, this.GetUserStringIndex((string)obj)));
					return;
				}
				case OperandType.InlineSwitch:
				{
					Instruction[] instructionArray = (Instruction[])obj;
					base.WriteInt32((int)instructionArray.Length);
					int offset = instruction.Offset + opCode.Size + 4 * ((int)instructionArray.Length + 1);
					for (int i = 0; i < (int)instructionArray.Length; i++)
					{
						base.WriteInt32(this.GetTargetOffset(instructionArray[i]) - offset);
					}
					return;
				}
				case OperandType.InlineVar:
				{
					base.WriteInt16((short)CodeWriter.GetVariableIndex((VariableDefinition)obj));
					return;
				}
				case OperandType.InlineArg:
				{
					base.WriteInt16((short)this.GetParameterIndex((ParameterDefinition)obj));
					return;
				}
				case OperandType.ShortInlineBrTarget:
				{
					Instruction instruction2 = (Instruction)obj;
					base.WriteSByte((sbyte)(this.GetTargetOffset(instruction2) - (instruction.Offset + opCode.Size + 1)));
					return;
				}
				case OperandType.ShortInlineI:
				{
					if (opCode == OpCodes.Ldc_I4_S)
					{
						base.WriteSByte((sbyte)obj);
						return;
					}
					base.WriteByte((byte)obj);
					return;
				}
				case OperandType.ShortInlineR:
				{
					base.WriteSingle((float)obj);
					return;
				}
				case OperandType.ShortInlineVar:
				{
					base.WriteByte((byte)CodeWriter.GetVariableIndex((VariableDefinition)obj));
					return;
				}
				case OperandType.ShortInlineArg:
				{
					base.WriteByte((byte)this.GetParameterIndex((ParameterDefinition)obj));
					return;
				}
				default:
				{
					throw new ArgumentException();
				}
			}
		}

		private void WriteResolvedMethodBody(MethodDefinition method)
		{
			this.body = method.Body;
			this.ComputeHeader();
			if (!this.RequiresFatHeader())
			{
				base.WriteByte((byte)(2 | this.body.CodeSize << 2));
			}
			else
			{
				this.WriteFatHeader();
			}
			this.WriteInstructions();
			if (this.body.HasExceptionHandlers)
			{
				this.WriteExceptionHandlers();
			}
			ISymbolWriter symbolWriter = this.metadata.symbol_writer;
			if (symbolWriter != null)
			{
				symbolWriter.Write(this.body);
			}
		}

		private void WriteSmallSection(Collection<ExceptionHandler> handlers)
		{
			base.WriteByte(1);
			base.WriteByte((byte)(handlers.Count * 12 + 4));
			base.WriteBytes(2);
			this.WriteExceptionHandlers(handlers, (int i) => base.WriteUInt16((ushort)i), (int i) => base.WriteByte((byte)i));
		}

		private void WriteUnresolvedMethodBody(MethodDefinition method)
		{
			MethodSymbols localVarToken;
			ByteBuffer byteBuffer = this.metadata.module.Read<MethodDefinition, CodeReader>(method, (MethodDefinition _, MetadataReader reader) => reader.code).PatchRawMethodBody(method, this, out localVarToken);
			base.WriteBytes(byteBuffer);
			if (localVarToken.instructions.IsNullOrEmpty<InstructionSymbol>())
			{
				return;
			}
			localVarToken.method_token = method.token;
			localVarToken.local_var_token = CodeWriter.GetLocalVarToken(byteBuffer, localVarToken);
			ISymbolWriter symbolWriter = this.metadata.symbol_writer;
			if (symbolWriter != null)
			{
				symbolWriter.Write(localVarToken);
			}
		}
	}
}