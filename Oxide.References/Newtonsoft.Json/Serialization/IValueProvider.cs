using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public interface IValueProvider
	{
		object GetValue(object target);

		void SetValue(object target, object value);
	}
}