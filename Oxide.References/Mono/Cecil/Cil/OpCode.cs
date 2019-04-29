using System;

namespace Mono.Cecil.Cil
{
	public struct OpCode
	{
		private readonly byte op1;

		private readonly byte op2;

		private readonly byte code;

		private readonly byte flow_control;

		private readonly byte opcode_type;

		private readonly byte operand_type;

		private readonly byte stack_behavior_pop;

		private readonly byte stack_behavior_push;

		public Mono.Cecil.Cil.Code Code
		{
			get
			{
				return (Mono.Cecil.Cil.Code)this.code;
			}
		}

		public Mono.Cecil.Cil.FlowControl FlowControl
		{
			get
			{
				return (Mono.Cecil.Cil.FlowControl)this.flow_control;
			}
		}

		public string Name
		{
			get
			{
				return OpCodeNames.names[(int)this.Code];
			}
		}

		public byte Op1
		{
			get
			{
				return this.op1;
			}
		}

		public byte Op2
		{
			get
			{
				return this.op2;
			}
		}

		public Mono.Cecil.Cil.OpCodeType OpCodeType
		{
			get
			{
				return (Mono.Cecil.Cil.OpCodeType)this.opcode_type;
			}
		}

		public Mono.Cecil.Cil.OperandType OperandType
		{
			get
			{
				return (Mono.Cecil.Cil.OperandType)this.operand_type;
			}
		}

		public int Size
		{
			get
			{
				if (this.op1 != 255)
				{
					return 2;
				}
				return 1;
			}
		}

		public StackBehaviour StackBehaviourPop
		{
			get
			{
				return (StackBehaviour)this.stack_behavior_pop;
			}
		}

		public StackBehaviour StackBehaviourPush
		{
			get
			{
				return (StackBehaviour)this.stack_behavior_push;
			}
		}

		public short Value
		{
			get
			{
				if (this.op1 == 255)
				{
					return this.op2;
				}
				return (short)(this.op1 << 8 | this.op2);
			}
		}

		internal OpCode(int x, int y)
		{
			this.op1 = (byte)(x & 255);
			this.op2 = (byte)(x >> 8 & 255);
			this.code = (byte)(x >> 16 & 255);
			this.flow_control = (byte)(x >> 24 & 255);
			this.opcode_type = (byte)(y & 255);
			this.operand_type = (byte)(y >> 8 & 255);
			this.stack_behavior_pop = (byte)(y >> 16 & 255);
			this.stack_behavior_push = (byte)(y >> 24 & 255);
			if (this.op1 == 255)
			{
				OpCodes.OneByteOpCode[this.op2] = this;
				return;
			}
			OpCodes.TwoBytesOpCode[this.op2] = this;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is OpCode))
			{
				return false;
			}
			OpCode opCode = (OpCode)obj;
			if (this.op1 != opCode.op1)
			{
				return false;
			}
			return this.op2 == opCode.op2;
		}

		public bool Equals(OpCode opcode)
		{
			if (this.op1 != opcode.op1)
			{
				return false;
			}
			return this.op2 == opcode.op2;
		}

		public override int GetHashCode()
		{
			return this.Value;
		}

		public static bool operator ==(OpCode one, OpCode other)
		{
			if (one.op1 != other.op1)
			{
				return false;
			}
			return one.op2 == other.op2;
		}

		public static bool operator !=(OpCode one, OpCode other)
		{
			if (one.op1 != other.op1)
			{
				return true;
			}
			return one.op2 != other.op2;
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}