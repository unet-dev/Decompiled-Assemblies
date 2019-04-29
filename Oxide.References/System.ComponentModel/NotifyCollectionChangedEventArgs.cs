using Newtonsoft.Json.Shims;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.ComponentModel
{
	[Preserve]
	public class NotifyCollectionChangedEventArgs
	{
		internal NotifyCollectionChangedAction Action
		{
			get;
			set;
		}

		internal IList NewItems
		{
			get;
			set;
		}

		internal int NewStartingIndex
		{
			get;
			set;
		}

		internal IList OldItems
		{
			get;
			set;
		}

		internal int OldStartingIndex
		{
			get;
			set;
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
		{
			this.Action = action;
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems) : this(action)
		{
			this.NewItems = changedItems;
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem) : this(action)
		{
			this.NewItems = new List<object>()
			{
				changedItem
			};
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems) : this(action, newItems)
		{
			this.OldItems = oldItems;
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex) : this(action, changedItems)
		{
			this.NewStartingIndex = startingIndex;
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index) : this(action, changedItem)
		{
			this.NewStartingIndex = index;
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem) : this(action, newItem)
		{
			this.OldItems = new List<object>()
			{
				oldItem
			};
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex) : this(action, newItems, oldItems)
		{
			this.NewStartingIndex = startingIndex;
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex) : this(action, changedItems, index)
		{
			this.OldStartingIndex = oldIndex;
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex) : this(action, changedItem, index)
		{
			this.OldStartingIndex = oldIndex;
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index) : this(action, newItem, oldItem)
		{
			this.NewStartingIndex = index;
		}
	}
}