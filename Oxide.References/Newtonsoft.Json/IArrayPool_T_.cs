using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Preserve]
	public interface IArrayPool<T>
	{
		T[] Rent(int minimumLength);

		void Return(T[] array);
	}
}