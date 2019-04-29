using System;
using System.Collections;

namespace Apex.AI.Visualization
{
	public interface ICompositeVisualizer
	{
		IList children
		{
			get;
		}

		void Add(object item);
	}
}