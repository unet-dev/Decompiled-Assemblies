using System;

namespace Apex.Serialization
{
	public sealed class StageList : StageContainer
	{
		public StageList(string name) : base(name)
		{
		}

		public StageList(string name, params StageItem[] items) : base(name)
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
			if (this._tailChild != null)
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
				StageItem stageItem = this._tailChild;
				while (stageItem.next != item)
				{
					stageItem = stageItem.next;
				}
				stageItem.next = item.next;
				if (item == this._tailChild)
				{
					this._tailChild = stageItem;
				}
			}
			else
			{
				this._tailChild = null;
			}
			item.next = null;
			item.parent = null;
		}
	}
}