using System;

namespace Apex.Serialization
{
	public interface ISerializer
	{
		StageItem Deserialize(string data);

		string Serialize(StageItem item, bool pretty);
	}
}