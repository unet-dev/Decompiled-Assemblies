using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Mono.Cecil.Rocks
{
	public interface IILVisitor
	{
		void OnInlineArgument(OpCode opcode, ParameterDefinition parameter);

		void OnInlineBranch(OpCode opcode, int offset);

		void OnInlineByte(OpCode opcode, byte value);

		void OnInlineDouble(OpCode opcode, double value);

		void OnInlineField(OpCode opcode, FieldReference field);

		void OnInlineInt32(OpCode opcode, int value);

		void OnInlineInt64(OpCode opcode, long value);

		void OnInlineMethod(OpCode opcode, MethodReference method);

		void OnInlineNone(OpCode opcode);

		void OnInlineSByte(OpCode opcode, sbyte value);

		void OnInlineSignature(OpCode opcode, CallSite callSite);

		void OnInlineSingle(OpCode opcode, float value);

		void OnInlineString(OpCode opcode, string value);

		void OnInlineSwitch(OpCode opcode, int[] offsets);

		void OnInlineType(OpCode opcode, TypeReference type);

		void OnInlineVariable(OpCode opcode, VariableDefinition variable);
	}
}