using System;
using System.Runtime.Serialization;

namespace Oxide.Core.Libraries.Covalence
{
	[Serializable]
	public class CommandAlreadyExistsException : Exception
	{
		public CommandAlreadyExistsException()
		{
		}

		public CommandAlreadyExistsException(string cmd) : base(string.Concat("Command ", cmd, " already exists"))
		{
		}

		public CommandAlreadyExistsException(string message, Exception inner) : base(message, inner)
		{
		}

		protected CommandAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}