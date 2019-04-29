using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace System.ComponentModel
{
	[Preserve]
	public interface INotifyCollectionChanged
	{
		event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}