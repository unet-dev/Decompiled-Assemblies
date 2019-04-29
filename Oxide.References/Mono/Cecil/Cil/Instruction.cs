using Mono.Cecil;
using System;
using System.Text;

namespace Mono.Cecil.Cil
{
	public sealed class Instruction
	{
		internal int offset;

		internal Mono.Cecil.Cil.OpCode opcode;

		internal object operand;

		internal Instruction previous;

		internal Instruction next;

		private Mono.Cecil.Cil.SequencePoint sequence_point;

		public Instruction Next
		{
			get
			{
				return this.next;
			}
			set
			{
				this.next = value;
			}
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = value;
			}
		}

		public Mono.Cecil.Cil.OpCode OpCode
		{
			get
			{
				return this.opcode;
			}
			set
			{
				this.opcode = value;
			}
		}

		public object Operand
		{
			get
			{
				return this.operand;
			}
			set
			{
				this.operand = value;
			}
		}

		public Instruction Previous
		{
			get
			{
				return this.previous;
			}
			set
			{
				this.previous = value;
			}
		}

		public Mono.Cecil.Cil.SequencePoint SequencePoint
		{
			get
			{
				return this.sequence_point;
			}
			set
			{
				this.sequence_point = value;
			}
		}

		internal Instruction(int offset, Mono.Cecil.Cil.OpCode opCode)
		{
			this.offset = offset;
			this.opcode = opCode;
		}

		internal Instruction(Mono.Cecil.Cil.OpCode opcode, object operand)
		{
			this.opcode = opcode;
			this.operand = operand;
		}

		private static void AppendLabel(StringBuilder builder, Instruction instruction)
		{
			builder.Append("IL_");
			builder.Append(instruction.offset.ToString("x4"));
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode)
		{
			if (opcode.OperandType != OperandType.InlineNone)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, null);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, TypeReference type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (opcode.OperandType != OperandType.InlineType && opcode.OperandType != OperandType.InlineTok)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, type);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, CallSite site)
		{
			if (site == null)
			{
				throw new ArgumentNullException("site");
			}
			if (opcode.Code != Code.Calli)
			{
				throw new ArgumentException("code");
			}
			return new Instruction(opcode, site);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, MethodReference method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (opcode.OperandType != OperandType.InlineMethod && opcode.OperandType != OperandType.InlineTok)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, method);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, FieldReference field)
		{
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}
			if (opcode.OperandType != OperandType.InlineField && opcode.OperandType != OperandType.InlineTok)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, field);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (opcode.OperandType != OperandType.InlineString)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, value);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, sbyte value)
		{
			if (opcode.OperandType != OperandType.ShortInlineI && opcode != OpCodes.Ldc_I4_S)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, (object)value);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, byte value)
		{
			if (opcode.OperandType != OperandType.ShortInlineI || opcode == OpCodes.Ldc_I4_S)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, (object)value);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, int value)
		{
			if (opcode.OperandType != OperandType.InlineI)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, (object)value);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, long value)
		{
			if (opcode.OperandType != OperandType.InlineI8)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, (object)value);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, float value)
		{
			if (opcode.OperandType != OperandType.ShortInlineR)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, (object)value);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, double value)
		{
			if (opcode.OperandType != OperandType.InlineR)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, (object)value);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, Instruction target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (opcode.OperandType != OperandType.InlineBrTarget && opcode.OperandType != OperandType.ShortInlineBrTarget)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, target);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, Instruction[] targets)
		{
			if (targets == null)
			{
				throw new ArgumentNullException("targets");
			}
			if (opcode.OperandType != OperandType.InlineSwitch)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, targets);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, VariableDefinition variable)
		{
			if (variable == null)
			{
				throw new ArgumentNullException("variable");
			}
			if (opcode.OperandType != OperandType.ShortInlineVar && opcode.OperandType != OperandType.InlineVar)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, variable);
		}

		public static Instruction Create(Mono.Cecil.Cil.OpCode opcode, ParameterDefinition parameter)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException("parameter");
			}
			if (opcode.OperandType != OperandType.ShortInlineArg && opcode.OperandType != OperandType.InlineArg)
			{
				throw new ArgumentException("opcode");
			}
			return new Instruction(opcode, parameter);
		}

		public int GetSize()
		{
			int size = this.opcode.Size;
			switch (this.opcode.OperandType)
			{
				case OperandType.InlineBrTarget:
				case OperandType.InlineField:
				case OperandType.InlineI:
				case OperandType.InlineMethod:
				case OperandType.InlineSig:
				case OperandType.InlineString:
				case OperandType.InlineTok:
				case OperandType.InlineType:
				case OperandType.ShortInlineR:
				{
					return size + 4;
				}
				case OperandType.InlineI8:
				case OperandType.InlineR:
				{
					return size + 8;
				}
				case OperandType.InlineNone:
				case OperandType.InlinePhi:
				{
					return size;
				}
				case OperandType.InlineSwitch:
				{
					return size + (1 + (int)((Instruction[])this.operand).Length) * 4;
				}
				case OperandType.InlineVar:
				case OperandType.InlineArg:
				{
					return size + 2;
				}
				case OperandType.ShortInlineBrTarget:
				case OperandType.ShortInlineI:
				case OperandType.ShortInlineVar:
				case OperandType.ShortInlineArg:
				{
					return size + 1;
				}
				default:
				{
					return size;
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder;
			StringBuilder stringBuilder1 = new StringBuilder();
			Instruction.AppendLabel(stringBuilder1, this);
			stringBuilder1.Append(':');
			stringBuilder1.Append(' ');
			stringBuilder1.Append(this.opcode.Name);
			if (this.operand == null)
			{
				return stringBuilder1.ToString();
			}
			stringBuilder1.Append(' ');
			OperandType operandType = this.opcode.OperandType;
			if (operandType > OperandType.InlineString)
			{
				if (operandType != OperandType.InlineSwitch)
				{
					goto Label2;
				}
				Instruction[] instructionArray = (Instruction[])this.operand;
				for (int i = 0; i < (int)instructionArray.Length; i++)
				{
					if (i > 0)
					{
						stringBuilder1.Append(',');
					}
					Instruction.AppendLabel(stringBuilder1, instructionArray[i]);
				}
				return stringBuilder1.ToString();
			}
			else
			{
				if (operandType == OperandType.InlineBrTarget)
				{
					Instruction.AppendLabel(stringBuilder1, (Instruction)this.operand);
					return stringBuilder1.ToString();
				}
				if (operandType != OperandType.InlineString)
				{
					stringBuilder = stringBuilder1.Append(this.operand);
					return stringBuilder1.ToString();
				}
				stringBuilder1.Append('\"');
				stringBuilder1.Append(this.operand);
				stringBuilder1.Append('\"');
				return stringBuilder1.ToString();
			}
			stringBuilder = stringBuilder1.Append(this.operand);
			return stringBuilder1.ToString();
		Label2:
			if (operandType != OperandType.ShortInlineBrTarget)
			{
				stringBuilder = stringBuilder1.Append(this.operand);
				return stringBuilder1.ToString();
			}
			else
			{
				Instruction.AppendLabel(stringBuilder1, (Instruction)this.operand);
				return stringBuilder1.ToString();
			}
		}
	}
}