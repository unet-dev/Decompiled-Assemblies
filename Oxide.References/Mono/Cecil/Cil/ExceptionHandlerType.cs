using System;

namespace Mono.Cecil.Cil
{
	public enum ExceptionHandlerType
	{
		Catch = 0,
		Filter = 1,
		Finally = 2,
		Fault = 4
	}
}