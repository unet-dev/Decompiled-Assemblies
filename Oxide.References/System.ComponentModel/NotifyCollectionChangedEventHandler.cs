using Newtonsoft.Json.Shims;
using System;

namespace System.ComponentModel
{
	[Preserve]
	public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e);
}