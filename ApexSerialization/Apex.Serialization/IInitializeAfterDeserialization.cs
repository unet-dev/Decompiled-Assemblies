using System;

namespace Apex.Serialization
{
	public interface IInitializeAfterDeserialization
	{
		void Initialize(object rootObject);
	}
}