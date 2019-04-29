using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal delegate T Creator<T>();
}