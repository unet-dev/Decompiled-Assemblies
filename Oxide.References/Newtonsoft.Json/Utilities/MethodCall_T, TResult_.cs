using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal delegate TResult MethodCall<T, TResult>(T target, params object[] args);
}