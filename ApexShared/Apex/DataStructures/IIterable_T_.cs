using System.Collections;
using System.Collections.Generic;

namespace Apex.DataStructures
{
	public interface IIterable<T> : IEnumerable<T>, IEnumerable, IIndexable<T>
	{

	}
}