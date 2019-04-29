using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Apex.Serialization
{
	public sealed class StageElement : StageContainer
	{
		private StageItem _tailAttribute;

		public StageElement(string name) : base(name)
		{
		}

		public StageElement(string name, params StageItem[] items) : base(name)
		{
			if (items != null)
			{
				for (int i = 0; i < (int)items.Length; i++)
				{
					this.Add(items[i]);
				}
			}
		}

		public override void Add(StageItem item)
		{
			if (item == null)
			{
				return;
			}
			if (item is StageAttribute)
			{
				if (this._tailAttribute != null)
				{
					item.next = this._tailAttribute.next;
					this._tailAttribute.next = item;
					this._tailAttribute = item;
				}
				else
				{
					this._tailAttribute = item;
					this._tailAttribute.next = this._tailAttribute;
				}
			}
			else if (this._tailChild != null)
			{
				item.next = this._tailChild.next;
				this._tailChild.next = item;
				this._tailChild = item;
			}
			else
			{
				this._tailChild = item;
				this._tailChild.next = this._tailChild;
			}
			item.parent = this;
		}

		public StageAttribute Attribute(string name)
		{
			if (this._tailAttribute == null)
			{
				return null;
			}
			StageItem stageItem = this._tailAttribute;
			do
			{
				if (stageItem.name == name)
				{
					return (StageAttribute)stageItem;
				}
				stageItem = stageItem.next;
			}
			while (stageItem != this._tailAttribute);
			return null;
		}

		public IEnumerable<StageAttribute> Attributes()
		{
			StageElement stageElement = null;
			StageItem i;
			if (stageElement._tailAttribute == null)
			{
				yield break;
			}
			for (i = stageElement._tailAttribute.next; i != stageElement._tailAttribute; i = i.next)
			{
				yield return (StageAttribute)i;
			}
			yield return (StageAttribute)i;
		}

		public IEnumerable<StageItem> Descendants(string name)
		{
			StageElement stageElement = null;
			StageItem stageItem = stageElement;
			StageContainer stageContainer = stageElement;
			while (true)
			{
				if (stageContainer != null)
				{
					if (stageContainer._tailChild == null)
					{
						goto Label2;
					}
					stageItem = stageContainer._tailChild.next;
					goto Label0;
				}
			Label2:
				while (stageItem != stageElement && stageItem == stageItem.parent._tailChild)
				{
					stageItem = stageItem.parent;
				}
				if (stageItem == stageElement)
				{
					break;
				}
				stageItem = stageItem.next;
			Label0:
				if (stageItem.name == name)
				{
					yield return stageItem;
				}
				stageContainer = stageItem as StageContainer;
			}
			yield break;
			goto Label2;
		}

		public StageElement Element(string name)
		{
			return this.Item(name) as StageElement;
		}

		public IEnumerable<StageElement> Elements(string name)
		{
			StageElement stageElement = null;
			if (stageElement._tailChild == null)
			{
				yield break;
			}
			StageItem stageItem = stageElement._tailChild;
			do
			{
				stageItem = stageItem.next;
				StageElement stageElement1 = stageItem as StageElement;
				if (stageElement1 == null || !(stageElement1.name == name))
				{
					continue;
				}
				yield return stageElement1;
			}
			while (stageItem != stageElement._tailChild);
		}

		public StageItem Item(string name)
		{
			if (this._tailChild == null)
			{
				return null;
			}
			StageItem stageItem = this._tailChild;
			do
			{
				if (stageItem.name == name)
				{
					return stageItem;
				}
				stageItem = stageItem.next;
			}
			while (stageItem != this._tailChild);
			return null;
		}

		public IEnumerable<StageItem> Items(string name)
		{
			StageElement stageElement = null;
			if (stageElement._tailChild == null)
			{
				yield break;
			}
			StageItem stageItem = stageElement._tailChild;
			do
			{
				stageItem = stageItem.next;
				if (stageItem.name != name)
				{
					continue;
				}
				yield return stageItem;
			}
			while (stageItem != stageElement._tailChild);
		}

		public StageList List(string name)
		{
			return this.Item(name) as StageList;
		}

		internal override void Remove(StageItem item)
		{
			if (item == null)
			{
				return;
			}
			if (item.parent != this)
			{
				throw new ArgumentException("Cannot remove item not belonging to this element.", "item");
			}
			if (item.next != item)
			{
				bool flag = item is StageAttribute;
				StageItem stageItem = (flag ? this._tailAttribute : this._tailChild);
				while (stageItem.next != item)
				{
					stageItem = stageItem.next;
				}
				stageItem.next = item.next;
				if (flag && item == this._tailAttribute)
				{
					this._tailAttribute = stageItem;
				}
				else if (item == this._tailChild)
				{
					this._tailChild = stageItem;
				}
			}
			else if (!(item is StageAttribute))
			{
				this._tailChild = null;
			}
			else
			{
				this._tailAttribute = null;
			}
			item.next = null;
			item.parent = null;
		}
	}
}