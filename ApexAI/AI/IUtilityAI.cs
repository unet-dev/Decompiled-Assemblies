using System;
using System.Reflection;

namespace Apex.AI
{
	public interface IUtilityAI : ISelect
	{
		Selector this[int idx]
		{
			get;
		}

		string name
		{
			get;
			set;
		}

		Selector rootSelector
		{
			get;
			set;
		}

		int selectorCount
		{
			get;
		}

		void AddSelector(Selector s);

		Selector FindSelector(Guid id);

		void RegenerateIds();

		void RemoveSelector(Selector s);

		bool ReplaceSelector(Selector current, Selector replacement);
	}
}