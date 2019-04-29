using System;

namespace Mono.Cecil.Cil
{
	public struct InstructionSymbol
	{
		public readonly int Offset;

		public readonly Mono.Cecil.Cil.SequencePoint SequencePoint;

		public InstructionSymbol(int offset, Mono.Cecil.Cil.SequencePoint sequencePoint)
		{
			this.Offset = offset;
			this.SequencePoint = sequencePoint;
		}
	}
}