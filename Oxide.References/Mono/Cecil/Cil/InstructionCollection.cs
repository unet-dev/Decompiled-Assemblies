using Mono.Collections.Generic;
using System;

namespace Mono.Cecil.Cil
{
	internal class InstructionCollection : Collection<Instruction>
	{
		internal InstructionCollection()
		{
		}

		internal InstructionCollection(int capacity) : base(capacity)
		{
		}

		protected override void OnAdd(Instruction item, int index)
		{
			if (index == 0)
			{
				return;
			}
			Instruction instruction = this.items[index - 1];
			instruction.next = item;
			item.previous = instruction;
		}

		protected override void OnInsert(Instruction item, int index)
		{
			if (this.size == 0)
			{
				return;
			}
			Instruction instruction = this.items[index];
			if (instruction == null)
			{
				Instruction instruction1 = this.items[index - 1];
				instruction1.next = item;
				item.previous = instruction1;
				return;
			}
			Instruction instruction2 = instruction.previous;
			if (instruction2 != null)
			{
				instruction2.next = item;
				item.previous = instruction2;
			}
			instruction.previous = item;
			item.next = instruction;
		}

		protected override void OnRemove(Instruction item, int index)
		{
			Instruction instruction = item.previous;
			if (instruction != null)
			{
				instruction.next = item.next;
			}
			Instruction instruction1 = item.next;
			if (instruction1 != null)
			{
				instruction1.previous = item.previous;
			}
			item.previous = null;
			item.next = null;
		}

		protected override void OnSet(Instruction item, int index)
		{
			Instruction instruction = this.items[index];
			item.previous = instruction.previous;
			item.next = instruction.next;
			instruction.previous = null;
			instruction.next = null;
		}
	}
}