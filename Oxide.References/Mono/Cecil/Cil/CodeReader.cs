using Mono.Cecil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Cil
{
	internal sealed class CodeReader : ByteBuffer
	{
		internal readonly MetadataReader reader;

		private int start;

		private Section code_section;

		private MethodDefinition method;

		private MethodBody body;

		private int Offset
		{
			get
			{
				return this.position - this.start;
			}
		}

		public CodeReader(Section section, MetadataReader reader) : base(section.Data)
		{
			this.code_section = section;
			this.reader = reader;
		}

		private void Align(int align)
		{
			align--;
			base.Advance((this.position + align & ~align) - this.position);
		}

		public Mono.Cecil.CallSite GetCallSite(MetadataToken token)
		{
			return this.reader.ReadCallSite(token);
		}

		private Instruction GetInstruction(int offset)
		{
			return CodeReader.GetInstruction(this.body.Instructions, offset);
		}

		private static Instruction GetInstruction(Collection<Instruction> instructions, int offset)
		{
			int num = instructions.size;
			Instruction[] instructionArray = instructions.items;
			if (offset < 0 || offset > instructionArray[num - 1].offset)
			{
				return null;
			}
			int num1 = 0;
			int num2 = num - 1;
			while (num1 <= num2)
			{
				int num3 = num1 + (num2 - num1) / 2;
				Instruction instruction = instructionArray[num3];
				int num4 = instruction.offset;
				if (offset == num4)
				{
					return instruction;
				}
				if (offset >= num4)
				{
					num1 = num3 + 1;
				}
				else
				{
					num2 = num3 - 1;
				}
			}
			return null;
		}

		private static MetadataToken GetOriginalToken(MetadataBuilder metadata, MethodDefinition method)
		{
			MetadataToken metadataToken;
			if (metadata.TryGetOriginalMethodToken(method.token, out metadataToken))
			{
				return metadataToken;
			}
			return MetadataToken.Zero;
		}

		public ParameterDefinition GetParameter(int index)
		{
			return this.body.GetParameter(index);
		}

		public string GetString(MetadataToken token)
		{
			return this.reader.image.UserStringHeap.Read(token.RID);
		}

		public VariableDefinition GetVariable(int index)
		{
			return this.body.GetVariable(index);
		}

		private bool IsInSection(int rva)
		{
			if ((ulong)this.code_section.VirtualAddress > (long)rva)
			{
				return false;
			}
			return (long)rva < (ulong)(this.code_section.VirtualAddress + this.code_section.SizeOfRawData);
		}

		public void MoveTo(int rva)
		{
			if (!this.IsInSection(rva))
			{
				this.code_section = this.reader.image.GetSectionAtVirtualAddress((uint)rva);
				base.Reset(this.code_section.Data);
			}
			this.position = (int)(rva - this.code_section.VirtualAddress);
		}

		private void PatchRawCode(ByteBuffer buffer, int code_size, CodeWriter writer)
		{
			OpCode twoBytesOpCode;
			MetadataToken standAloneSignature;
			MetadataBuilder metadataBuilder = writer.metadata;
			buffer.WriteBytes(base.ReadBytes(code_size));
			int num = buffer.position;
			buffer.position -= code_size;
			while (buffer.position < num)
			{
				byte num1 = buffer.ReadByte();
				if (num1 == 254)
				{
					byte num2 = buffer.ReadByte();
					twoBytesOpCode = OpCodes.TwoBytesOpCode[num2];
				}
				else
				{
					twoBytesOpCode = OpCodes.OneByteOpCode[num1];
				}
				switch (twoBytesOpCode.OperandType)
				{
					case OperandType.InlineBrTarget:
					case OperandType.InlineI:
					case OperandType.ShortInlineR:
					{
						buffer.position += 4;
						continue;
					}
					case OperandType.InlineField:
					case OperandType.InlineMethod:
					case OperandType.InlineTok:
					case OperandType.InlineType:
					{
						IMetadataTokenProvider metadataTokenProvider = this.reader.LookupToken(new MetadataToken(buffer.ReadUInt32()));
						buffer.position -= 4;
						standAloneSignature = metadataBuilder.LookupToken(metadataTokenProvider);
						buffer.WriteUInt32(standAloneSignature.ToUInt32());
						continue;
					}
					case OperandType.InlineI8:
					case OperandType.InlineR:
					{
						buffer.position += 8;
						continue;
					}
					case OperandType.InlineSig:
					{
						Mono.Cecil.CallSite callSite = this.GetCallSite(new MetadataToken(buffer.ReadUInt32()));
						buffer.position -= 4;
						standAloneSignature = writer.GetStandAloneSignature(callSite);
						buffer.WriteUInt32(standAloneSignature.ToUInt32());
						continue;
					}
					case OperandType.InlineString:
					{
						string str = this.GetString(new MetadataToken(buffer.ReadUInt32()));
						buffer.position -= 4;
						standAloneSignature = new MetadataToken(Mono.Cecil.TokenType.String, metadataBuilder.user_string_heap.GetStringIndex(str));
						buffer.WriteUInt32(standAloneSignature.ToUInt32());
						continue;
					}
					case OperandType.InlineSwitch:
					{
						int num3 = buffer.ReadInt32();
						ByteBuffer byteBuffer = buffer;
						byteBuffer.position = byteBuffer.position + num3 * 4;
						continue;
					}
					case OperandType.InlineVar:
					case OperandType.InlineArg:
					{
						buffer.position += 2;
						continue;
					}
					case OperandType.ShortInlineBrTarget:
					case OperandType.ShortInlineI:
					case OperandType.ShortInlineVar:
					case OperandType.ShortInlineArg:
					{
						buffer.position++;
						continue;
					}
					default:
					{
						continue;
					}
				}
			}
		}

		private void PatchRawExceptionHandlers(ByteBuffer buffer, MetadataBuilder metadata, int count, bool fat_entry)
		{
			ExceptionHandlerType exceptionHandlerType;
			for (int i = 0; i < count; i++)
			{
				if (!fat_entry)
				{
					ushort num = base.ReadUInt16();
					exceptionHandlerType = (ExceptionHandlerType)(num & 7);
					buffer.WriteUInt16(num);
				}
				else
				{
					uint num1 = base.ReadUInt32();
					exceptionHandlerType = (ExceptionHandlerType)(num1 & 7);
					buffer.WriteUInt32(num1);
				}
				buffer.WriteBytes(base.ReadBytes((fat_entry ? 16 : 6)));
				if (exceptionHandlerType != ExceptionHandlerType.Catch)
				{
					buffer.WriteUInt32(base.ReadUInt32());
				}
				else
				{
					IMetadataTokenProvider metadataTokenProvider = this.reader.LookupToken(this.ReadToken());
					buffer.WriteUInt32(metadata.LookupToken(metadataTokenProvider).ToUInt32());
				}
			}
		}

		private void PatchRawFatMethod(ByteBuffer buffer, MethodSymbols symbols, CodeWriter writer, out MetadataToken local_var_token)
		{
			uint num;
			ushort num1 = base.ReadUInt16();
			buffer.WriteUInt16(num1);
			buffer.WriteUInt16(base.ReadUInt16());
			symbols.code_size = base.ReadInt32();
			buffer.WriteInt32(symbols.code_size);
			local_var_token = this.ReadToken();
			if (local_var_token.RID <= 0)
			{
				buffer.WriteUInt32(0);
			}
			else
			{
				VariableDefinitionCollection variableDefinitionCollection = this.ReadVariables(local_var_token);
				Collection<VariableDefinition> variableDefinitions = variableDefinitionCollection;
				symbols.variables = variableDefinitionCollection;
				Collection<VariableDefinition> variableDefinitions1 = variableDefinitions;
				ByteBuffer byteBuffer = buffer;
				if (variableDefinitions1 != null)
				{
					num = writer.GetStandAloneSignature(symbols.variables).ToUInt32();
				}
				else
				{
					num = 0;
				}
				byteBuffer.WriteUInt32(num);
			}
			this.PatchRawCode(buffer, symbols.code_size, writer);
			if ((num1 & 8) != 0)
			{
				this.PatchRawSection(buffer, writer.metadata);
			}
		}

		private void PatchRawFatSection(ByteBuffer buffer, MetadataBuilder metadata)
		{
			this.position--;
			int num = base.ReadInt32();
			buffer.WriteInt32(num);
			int num1 = (num >> 8) / 24;
			this.PatchRawExceptionHandlers(buffer, metadata, num1, true);
		}

		public ByteBuffer PatchRawMethodBody(MethodDefinition method, CodeWriter writer, out MethodSymbols symbols)
		{
			MetadataToken zero;
			ByteBuffer byteBuffer = new ByteBuffer();
			symbols = new MethodSymbols(method.Name);
			this.method = method;
			this.reader.context = method;
			this.MoveTo(method.RVA);
			byte num = base.ReadByte();
			int num1 = num & 3;
			if (num1 == 2)
			{
				byteBuffer.WriteByte(num);
				zero = MetadataToken.Zero;
				symbols.code_size = num >> 2;
				this.PatchRawCode(byteBuffer, symbols.code_size, writer);
			}
			else
			{
				if (num1 != 3)
				{
					throw new NotSupportedException();
				}
				this.position--;
				this.PatchRawFatMethod(byteBuffer, symbols, writer, out zero);
			}
			ISymbolReader symbolReader = this.reader.module.symbol_reader;
			if (symbolReader != null && writer.metadata.write_symbols)
			{
				symbols.method_token = CodeReader.GetOriginalToken(writer.metadata, method);
				symbols.local_var_token = zero;
				symbolReader.Read(symbols);
			}
			return byteBuffer;
		}

		private void PatchRawSection(ByteBuffer buffer, MetadataBuilder metadata)
		{
			int num = this.position;
			this.Align(4);
			buffer.WriteBytes(this.position - num);
			byte num1 = base.ReadByte();
			if ((num1 & 64) != 0)
			{
				this.PatchRawFatSection(buffer, metadata);
			}
			else
			{
				buffer.WriteByte(num1);
				this.PatchRawSmallSection(buffer, metadata);
			}
			if ((num1 & 128) != 0)
			{
				this.PatchRawSection(buffer, metadata);
			}
		}

		private void PatchRawSmallSection(ByteBuffer buffer, MetadataBuilder metadata)
		{
			byte num = base.ReadByte();
			buffer.WriteByte(num);
			base.Advance(2);
			buffer.WriteUInt16(0);
			this.PatchRawExceptionHandlers(buffer, metadata, num / 12, false);
		}

		private void ReadCode()
		{
			this.start = this.position;
			int codeSize = this.body.code_size;
			if (codeSize < 0 || (long)((int)this.buffer.Length) <= (ulong)(codeSize + this.position))
			{
				codeSize = 0;
			}
			int num = this.start + codeSize;
			MethodBody methodBody = this.body;
			InstructionCollection instructionCollection = new InstructionCollection((codeSize + 1) / 2);
			Collection<Instruction> instructions = instructionCollection;
			methodBody.instructions = instructionCollection;
			Collection<Instruction> instructions1 = instructions;
			while (this.position < num)
			{
				int num1 = this.position - this.start;
				OpCode opCode = this.ReadOpCode();
				Instruction instruction = new Instruction(num1, opCode);
				if (opCode.OperandType != OperandType.InlineNone)
				{
					instruction.operand = this.ReadOperand(instruction);
				}
				instructions1.Add(instruction);
			}
			this.ResolveBranches(instructions1);
		}

		private void ReadExceptionHandlers(int count, Func<int> read_entry, Func<int> read_length)
		{
			for (int i = 0; i < count; i++)
			{
				ExceptionHandler exceptionHandler = new ExceptionHandler((ExceptionHandlerType)(read_entry() & 7))
				{
					TryStart = this.GetInstruction(read_entry()),
					TryEnd = this.GetInstruction(exceptionHandler.TryStart.Offset + read_length()),
					HandlerStart = this.GetInstruction(read_entry()),
					HandlerEnd = this.GetInstruction(exceptionHandler.HandlerStart.Offset + read_length())
				};
				this.ReadExceptionHandlerSpecific(exceptionHandler);
				this.body.ExceptionHandlers.Add(exceptionHandler);
			}
		}

		private void ReadExceptionHandlerSpecific(ExceptionHandler handler)
		{
			ExceptionHandlerType handlerType = handler.HandlerType;
			if (handlerType == ExceptionHandlerType.Catch)
			{
				handler.CatchType = (TypeReference)this.reader.LookupToken(this.ReadToken());
				return;
			}
			if (handlerType != ExceptionHandlerType.Filter)
			{
				base.Advance(4);
				return;
			}
			handler.FilterStart = this.GetInstruction(base.ReadInt32());
		}

		private void ReadFatMethod()
		{
			ushort num = base.ReadUInt16();
			this.body.max_stack_size = base.ReadUInt16();
			this.body.code_size = (int)base.ReadUInt32();
			this.body.local_var_token = new MetadataToken(base.ReadUInt32());
			this.body.init_locals = (num & 16) != 0;
			if (this.body.local_var_token.RID != 0)
			{
				this.body.variables = this.ReadVariables(this.body.local_var_token);
			}
			this.ReadCode();
			if ((num & 8) != 0)
			{
				this.ReadSection();
			}
		}

		private void ReadFatSection()
		{
			this.position--;
			int num = (base.ReadInt32() >> 8) / 24;
			this.ReadExceptionHandlers(num, new Func<int>(this.ReadInt32), new Func<int>(this.ReadInt32));
		}

		public MethodBody ReadMethodBody(MethodDefinition method)
		{
			this.method = method;
			this.body = new MethodBody(method);
			this.reader.context = method;
			this.ReadMethodBody();
			return this.body;
		}

		private void ReadMethodBody()
		{
			this.MoveTo(this.method.RVA);
			byte num = base.ReadByte();
			int num1 = num & 3;
			if (num1 == 2)
			{
				this.body.code_size = num >> 2;
				this.body.MaxStackSize = 8;
				this.ReadCode();
			}
			else
			{
				if (num1 != 3)
				{
					throw new InvalidOperationException();
				}
				this.position--;
				this.ReadFatMethod();
			}
			ISymbolReader symbolReader = this.reader.module.symbol_reader;
			if (symbolReader != null)
			{
				Collection<Instruction> instructions = this.body.Instructions;
				symbolReader.Read(this.body, (int offset) => CodeReader.GetInstruction(instructions, offset));
			}
		}

		private OpCode ReadOpCode()
		{
			byte num = base.ReadByte();
			if (num != 254)
			{
				return OpCodes.OneByteOpCode[num];
			}
			return OpCodes.TwoBytesOpCode[base.ReadByte()];
		}

		private object ReadOperand(Instruction instruction)
		{
			switch (instruction.opcode.OperandType)
			{
				case OperandType.InlineBrTarget:
				{
					return base.ReadInt32() + this.Offset;
				}
				case OperandType.InlineField:
				case OperandType.InlineMethod:
				case OperandType.InlineTok:
				case OperandType.InlineType:
				{
					return this.reader.LookupToken(this.ReadToken());
				}
				case OperandType.InlineI:
				{
					return base.ReadInt32();
				}
				case OperandType.InlineI8:
				{
					return base.ReadInt64();
				}
				case OperandType.InlineNone:
				case OperandType.InlinePhi:
				{
					throw new NotSupportedException();
				}
				case OperandType.InlineR:
				{
					return base.ReadDouble();
				}
				case OperandType.InlineSig:
				{
					return this.GetCallSite(this.ReadToken());
				}
				case OperandType.InlineString:
				{
					return this.GetString(this.ReadToken());
				}
				case OperandType.InlineSwitch:
				{
					int num = base.ReadInt32();
					int offset = this.Offset + 4 * num;
					int[] numArray = new int[num];
					for (int i = 0; i < num; i++)
					{
						numArray[i] = offset + base.ReadInt32();
					}
					return numArray;
				}
				case OperandType.InlineVar:
				{
					return this.GetVariable((int)base.ReadUInt16());
				}
				case OperandType.InlineArg:
				{
					return this.GetParameter((int)base.ReadUInt16());
				}
				case OperandType.ShortInlineBrTarget:
				{
					return base.ReadSByte() + this.Offset;
				}
				case OperandType.ShortInlineI:
				{
					if (instruction.opcode == OpCodes.Ldc_I4_S)
					{
						return base.ReadSByte();
					}
					return base.ReadByte();
				}
				case OperandType.ShortInlineR:
				{
					return base.ReadSingle();
				}
				case OperandType.ShortInlineVar:
				{
					return this.GetVariable((int)base.ReadByte());
				}
				case OperandType.ShortInlineArg:
				{
					return this.GetParameter((int)base.ReadByte());
				}
				default:
				{
					throw new NotSupportedException();
				}
			}
		}

		private void ReadSection()
		{
			this.Align(4);
			byte num = base.ReadByte();
			if ((num & 64) != 0)
			{
				this.ReadFatSection();
			}
			else
			{
				this.ReadSmallSection();
			}
			if ((num & 128) != 0)
			{
				this.ReadSection();
			}
		}

		private void ReadSmallSection()
		{
			int num = base.ReadByte() / 12;
			base.Advance(2);
			this.ReadExceptionHandlers(num, () => base.ReadUInt16(), () => base.ReadByte());
		}

		public MetadataToken ReadToken()
		{
			return new MetadataToken(base.ReadUInt32());
		}

		public VariableDefinitionCollection ReadVariables(MetadataToken local_var_token)
		{
			int num = this.reader.position;
			this.reader.position = num;
			return this.reader.ReadVariables(local_var_token);
		}

		private void ResolveBranches(Collection<Instruction> instructions)
		{
			Instruction[] instructionArray = instructions.items;
			int num = instructions.size;
			for (int i = 0; i < num; i++)
			{
				Instruction instruction = instructionArray[i];
				OperandType operandType = instruction.opcode.OperandType;
				if (operandType != OperandType.InlineBrTarget)
				{
					if (operandType == OperandType.InlineSwitch)
					{
						int[] numArray = (int[])instruction.operand;
						Instruction[] instruction1 = new Instruction[(int)numArray.Length];
						for (int j = 0; j < (int)numArray.Length; j++)
						{
							instruction1[j] = this.GetInstruction(numArray[j]);
						}
						instruction.operand = instruction1;
						goto Label0;
					}
					else if (operandType != OperandType.ShortInlineBrTarget)
					{
						goto Label0;
					}
				}
				instruction.operand = this.GetInstruction((int)instruction.operand);
			Label0:
			}
		}
	}
}