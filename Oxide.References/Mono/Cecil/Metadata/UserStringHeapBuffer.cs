using Mono.Cecil.PE;
using System;

namespace Mono.Cecil.Metadata
{
	internal sealed class UserStringHeapBuffer : StringHeapBuffer
	{
		public UserStringHeapBuffer()
		{
		}

		protected override void WriteString(string @string)
		{
			base.WriteCompressedUInt32((uint)(@string.Length * 2 + 1));
			byte num = 0;
			for (int i = 0; i < @string.Length; i++)
			{
				char str = @string[i];
				base.WriteUInt16(str);
				if (num != 1 && (str < ' ' || str > '~') && (str > '~' || str >= '\u0001' && str <= '\b' || str >= '\u000E' && str <= '\u001F' || str == '\'' || str == '-'))
				{
					num = 1;
				}
			}
			base.WriteByte(num);
		}
	}
}