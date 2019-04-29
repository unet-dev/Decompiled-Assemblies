using Newtonsoft.Json.Shims;
using System;

namespace System.ComponentModel
{
	[Preserve]
	public enum NotifyCollectionChangedAction
	{
		Add,
		Remove,
		Replace,
		Move,
		Reset
	}
}