using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Apex.Serialization
{
	public abstract class StageContainer : StageItem
	{
		internal StageItem _tailChild;

		protected StageContainer(string name) : base(name)
		{
		}

		public abstract void Add(StageItem item);

		public IEnumerable<StageItem> Descendants()
		{
			StageContainer stageContainer = null;
			StageItem stageItem = stageContainer;
			StageContainer stageContainer1 = stageContainer;
			while (true)
			{
				if (stageContainer1 != null)
				{
					if (stageContainer1._tailChild == null)
					{
						goto Label2;
					}
					stageItem = stageContainer1._tailChild.next;
					goto Label0;
				}
			Label2:
				while (stageItem != stageContainer && stageItem == stageItem.parent._tailChild)
				{
					stageItem = stageItem.parent;
				}
				if (stageItem == stageContainer)
				{
					break;
				}
				stageItem = stageItem.next;
			Label0:
				yield return stageItem;
				stageContainer1 = stageItem as StageContainer;
			}
			yield break;
			goto Label2;
		}

		public IEnumerable<T> Descendants<T>()
		where T : StageItem
		{
			StageContainer stageContainer = null;
			StageItem stageItem = stageContainer;
			StageContainer stageContainer1 = stageContainer;
			while (true)
			{
				if (stageContainer1 != null)
				{
					if (stageContainer1._tailChild == null)
					{
						goto Label2;
					}
					stageItem = stageContainer1._tailChild.next;
					goto Label0;
				}
			Label2:
				while (stageItem != stageContainer && stageItem == stageItem.parent._tailChild)
				{
					stageItem = stageItem.parent;
				}
				if (stageItem == stageContainer)
				{
					break;
				}
				stageItem = stageItem.next;
			Label0:
				T t = (T)(stageItem as T);
				if (t != null)
				{
					yield return t;
				}
				stageContainer1 = stageItem as StageContainer;
			}
			yield break;
			goto Label2;
		}

		public IEnumerable<StageElement> Elements()
		{
			StageContainer stageContainer = null;
			if (stageContainer._tailChild == null)
			{
				yield break;
			}
			StageItem stageItem = stageContainer._tailChild;
			do
			{
				stageItem = stageItem.next;
				StageElement stageElement = stageItem as StageElement;
				if (stageElement == null)
				{
					continue;
				}
				yield return stageElement;
			}
			while (stageItem != stageContainer._tailChild);
		}

		public IEnumerable<StageItem> Items()
		{
			StageContainer stageContainer = null;
			if (stageContainer._tailChild == null)
			{
				yield break;
			}
			StageItem stageItem = stageContainer._tailChild;
			do
			{
				stageItem = stageItem.next;
				yield return stageItem;
			}
			while (stageItem != stageContainer._tailChild);
		}

		internal abstract void Remove(StageItem item);
	}
}