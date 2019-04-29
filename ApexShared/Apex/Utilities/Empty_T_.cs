using System;

namespace Apex.Utilities
{
	public static class Empty<T>
	{
		public readonly static T[] array;

		static Empty()
		{
			Empty<T>.array = new T[0];
		}
	}
}