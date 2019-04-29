using Newtonsoft.Json.Shims;
using System;

namespace System.ComponentModel
{
	[Preserve]
	public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);
}