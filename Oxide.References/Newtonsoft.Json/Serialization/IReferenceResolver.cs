using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public interface IReferenceResolver
	{
		void AddReference(object context, string reference, object value);

		string GetReference(object context, object value);

		bool IsReferenced(object context, object value);

		object ResolveReference(object context, string reference);
	}
}