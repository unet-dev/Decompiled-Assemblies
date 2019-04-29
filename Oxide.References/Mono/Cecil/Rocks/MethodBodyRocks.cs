using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Rocks
{
	public static class MethodBodyRocks
	{
		private static void ComputeOffsets(MethodBody body)
		{
			int size = 0;
			foreach (Instruction instruction in body.Instructions)
			{
				instruction.Offset = size;
				size += instruction.GetSize();
			}
		}

		private static void ExpandMacro(Instruction instruction, OpCode opcode, object operand)
		{
			instruction.OpCode = opcode;
			instruction.Operand = operand;
		}

		private static void MakeMacro(Instruction instruction, OpCode opcode)
		{
			instruction.OpCode = opcode;
			instruction.Operand = null;
		}

		private static bool OptimizeBranch(Instruction instruction)
		{
			int offset = ((Instruction)instruction.Operand).Offset;
			int num = instruction.Offset;
			OpCode opCode = instruction.OpCode;
			int size = offset - (num + opCode.Size + 4);
			if (size < -128 || size > 127)
			{
				return false;
			}
			Code code = instruction.OpCode.Code;
			switch (code)
			{
				case Code.Br:
				{
					instruction.OpCode = OpCodes.Br_S;
					break;
				}
				case Code.Brfalse:
				{
					instruction.OpCode = OpCodes.Brfalse_S;
					break;
				}
				case Code.Brtrue:
				{
					instruction.OpCode = OpCodes.Brtrue_S;
					break;
				}
				case Code.Beq:
				{
					instruction.OpCode = OpCodes.Beq_S;
					break;
				}
				case Code.Bge:
				{
					instruction.OpCode = OpCodes.Bge_S;
					break;
				}
				case Code.Bgt:
				{
					instruction.OpCode = OpCodes.Bgt_S;
					break;
				}
				case Code.Ble:
				{
					instruction.OpCode = OpCodes.Ble_S;
					break;
				}
				case Code.Blt:
				{
					instruction.OpCode = OpCodes.Blt_S;
					break;
				}
				case Code.Bne_Un:
				{
					instruction.OpCode = OpCodes.Bne_Un_S;
					break;
				}
				case Code.Bge_Un:
				{
					instruction.OpCode = OpCodes.Bge_Un_S;
					break;
				}
				case Code.Bgt_Un:
				{
					instruction.OpCode = OpCodes.Bgt_Un_S;
					break;
				}
				case Code.Ble_Un:
				{
					instruction.OpCode = OpCodes.Ble_Un_S;
					break;
				}
				case Code.Blt_Un:
				{
					instruction.OpCode = OpCodes.Blt_Un_S;
					break;
				}
				default:
				{
					if (code == Code.Leave)
					{
						instruction.OpCode = OpCodes.Leave_S;
						break;
					}
					else
					{
						break;
					}
				}
			}
			return true;
		}

		private static void OptimizeBranches(MethodBody body)
		{
			MethodBodyRocks.ComputeOffsets(body);
			foreach (Instruction instruction in body.Instructions)
			{
				if (instruction.OpCode.OperandType != OperandType.InlineBrTarget || !MethodBodyRocks.OptimizeBranch(instruction))
				{
					continue;
				}
				MethodBodyRocks.ComputeOffsets(body);
			}
		}

		public static void OptimizeMacros(this MethodBody self)
		{
			int index;
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			MethodDefinition method = self.Method;
			foreach (Instruction instruction in self.Instructions)
			{
				Code code = instruction.OpCode.Code;
				if (code == Code.Ldc_I4)
				{
					int operand = (int)instruction.Operand;
					switch (operand)
					{
						case -1:
						{
							MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldc_I4_M1);
							continue;
						}
						case 0:
						{
							MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldc_I4_0);
							continue;
						}
						case 1:
						{
							MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldc_I4_1);
							continue;
						}
						case 2:
						{
							MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldc_I4_2);
							continue;
						}
						case 3:
						{
							MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldc_I4_3);
							continue;
						}
						case 4:
						{
							MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldc_I4_4);
							continue;
						}
						case 5:
						{
							MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldc_I4_5);
							continue;
						}
						case 6:
						{
							MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldc_I4_6);
							continue;
						}
						case 7:
						{
							MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldc_I4_7);
							continue;
						}
						case 8:
						{
							MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldc_I4_8);
							continue;
						}
					}
					if (operand < -128 || operand >= 128)
					{
						continue;
					}
					MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4_S, (sbyte)operand);
				}
				else
				{
					switch (code)
					{
						case Code.Ldarg:
						{
							index = ((ParameterDefinition)instruction.Operand).Index;
							if (index == -1 && instruction.Operand == self.ThisParameter)
							{
								index = 0;
							}
							else if (method.HasThis)
							{
								index++;
							}
							switch (index)
							{
								case 0:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldarg_0);
									continue;
								}
								case 1:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldarg_1);
									continue;
								}
								case 2:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldarg_2);
									continue;
								}
								case 3:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldarg_3);
									continue;
								}
							}
							if (index >= 256)
							{
								continue;
							}
							MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldarg_S, instruction.Operand);
							continue;
						}
						case Code.Ldarga:
						{
							index = ((ParameterDefinition)instruction.Operand).Index;
							if (index == -1 && instruction.Operand == self.ThisParameter)
							{
								index = 0;
							}
							else if (method.HasThis)
							{
								index++;
							}
							if (index >= 256)
							{
								continue;
							}
							MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldarga_S, instruction.Operand);
							continue;
						}
						case Code.Ldloc:
						{
							index = ((VariableDefinition)instruction.Operand).Index;
							switch (index)
							{
								case 0:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldloc_0);
									continue;
								}
								case 1:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldloc_1);
									continue;
								}
								case 2:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldloc_2);
									continue;
								}
								case 3:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Ldloc_3);
									continue;
								}
							}
							if (index >= 256)
							{
								continue;
							}
							MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldloc_S, instruction.Operand);
							continue;
						}
						case Code.Ldloca:
						{
							if (((VariableDefinition)instruction.Operand).Index >= 256)
							{
								continue;
							}
							MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldloca_S, instruction.Operand);
							continue;
						}
						case Code.Stloc:
						{
							index = ((VariableDefinition)instruction.Operand).Index;
							switch (index)
							{
								case 0:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Stloc_0);
									continue;
								}
								case 1:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Stloc_1);
									continue;
								}
								case 2:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Stloc_2);
									continue;
								}
								case 3:
								{
									MethodBodyRocks.MakeMacro(instruction, OpCodes.Stloc_3);
									continue;
								}
							}
							if (index >= 256)
							{
								continue;
							}
							MethodBodyRocks.ExpandMacro(instruction, OpCodes.Stloc_S, instruction.Operand);
							continue;
						}
						default:
						{
							continue;
						}
					}
				}
			}
			MethodBodyRocks.OptimizeBranches(self);
		}

		public static void SimplifyMacros(this MethodBody self)
		{
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
		Label1:
			foreach (Instruction instruction in self.Instructions)
			{
				if (instruction.OpCode.OpCodeType != OpCodeType.Macro)
				{
					continue;
				}
				Code code = instruction.OpCode.Code;
				switch (code)
				{
					case Code.Ldarg_0:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldarg, self.GetParameter(0));
						continue;
					}
					case Code.Ldarg_1:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldarg, self.GetParameter(1));
						continue;
					}
					case Code.Ldarg_2:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldarg, self.GetParameter(2));
						continue;
					}
					case Code.Ldarg_3:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldarg, self.GetParameter(3));
						continue;
					}
					case Code.Ldloc_0:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldloc, self.Variables[0]);
						continue;
					}
					case Code.Ldloc_1:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldloc, self.Variables[1]);
						continue;
					}
					case Code.Ldloc_2:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldloc, self.Variables[2]);
						continue;
					}
					case Code.Ldloc_3:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldloc, self.Variables[3]);
						continue;
					}
					case Code.Stloc_0:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Stloc, self.Variables[0]);
						continue;
					}
					case Code.Stloc_1:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Stloc, self.Variables[1]);
						continue;
					}
					case Code.Stloc_2:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Stloc, self.Variables[2]);
						continue;
					}
					case Code.Stloc_3:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Stloc, self.Variables[3]);
						continue;
					}
					case Code.Ldarg_S:
					{
						instruction.OpCode = OpCodes.Ldarg;
						continue;
					}
					case Code.Ldarga_S:
					{
						instruction.OpCode = OpCodes.Ldarga;
						continue;
					}
					case Code.Starg_S:
					{
						instruction.OpCode = OpCodes.Starg;
						continue;
					}
					case Code.Ldloc_S:
					{
						instruction.OpCode = OpCodes.Ldloc;
						continue;
					}
					case Code.Ldloca_S:
					{
						instruction.OpCode = OpCodes.Ldloca;
						continue;
					}
					case Code.Stloc_S:
					{
						instruction.OpCode = OpCodes.Stloc;
						continue;
					}
					case Code.Ldnull:
					case Code.Ldc_I4:
					case Code.Ldc_I8:
					case Code.Ldc_R4:
					case Code.Ldc_R8:
					case Code.Dup:
					case Code.Pop:
					case Code.Jmp:
					case Code.Call:
					case Code.Calli:
					case Code.Ret:
					{
						continue;
					}
					case Code.Ldc_I4_M1:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4, -1);
						continue;
					}
					case Code.Ldc_I4_0:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4, 0);
						continue;
					}
					case Code.Ldc_I4_1:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4, 1);
						continue;
					}
					case Code.Ldc_I4_2:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4, 2);
						continue;
					}
					case Code.Ldc_I4_3:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4, 3);
						continue;
					}
					case Code.Ldc_I4_4:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4, 4);
						continue;
					}
					case Code.Ldc_I4_5:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4, 5);
						continue;
					}
					case Code.Ldc_I4_6:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4, 6);
						continue;
					}
					case Code.Ldc_I4_7:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4, 7);
						continue;
					}
					case Code.Ldc_I4_8:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4, 8);
						continue;
					}
					case Code.Ldc_I4_S:
					{
						MethodBodyRocks.ExpandMacro(instruction, OpCodes.Ldc_I4, (int)((sbyte)instruction.Operand));
						continue;
					}
					case Code.Br_S:
					{
						instruction.OpCode = OpCodes.Br;
						continue;
					}
					case Code.Brfalse_S:
					{
						instruction.OpCode = OpCodes.Brfalse;
						continue;
					}
					case Code.Brtrue_S:
					{
						instruction.OpCode = OpCodes.Brtrue;
						continue;
					}
					case Code.Beq_S:
					{
						instruction.OpCode = OpCodes.Beq;
						continue;
					}
					case Code.Bge_S:
					{
						instruction.OpCode = OpCodes.Bge;
						continue;
					}
					case Code.Bgt_S:
					{
						instruction.OpCode = OpCodes.Bgt;
						continue;
					}
					case Code.Ble_S:
					{
						instruction.OpCode = OpCodes.Ble;
						continue;
					}
					case Code.Blt_S:
					{
						instruction.OpCode = OpCodes.Blt;
						continue;
					}
					case Code.Bne_Un_S:
					{
						instruction.OpCode = OpCodes.Bne_Un;
						continue;
					}
					case Code.Bge_Un_S:
					{
						instruction.OpCode = OpCodes.Bge_Un;
						continue;
					}
					case Code.Bgt_Un_S:
					{
						instruction.OpCode = OpCodes.Bgt_Un;
						continue;
					}
					case Code.Ble_Un_S:
					{
						instruction.OpCode = OpCodes.Ble_Un;
						continue;
					}
					case Code.Blt_Un_S:
					{
						instruction.OpCode = OpCodes.Blt_Un;
						continue;
					}
					default:
					{
						if (code == Code.Leave_S)
						{
							break;
						}
						else
						{
							goto Label1;
						}
					}
				}
				instruction.OpCode = OpCodes.Leave;
			}
		}
	}
}