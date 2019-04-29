using System;

namespace Mono
{
	internal static class Empty<T>
	{
		public readonly static T[] Array;

		static Empty()
		{
			Empty<T>.Array = new T[0];
		}
	}
}