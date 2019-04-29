using Apex.Serialization;
using System;

namespace Apex.Serialization.Json
{
	internal class JsonSerializer : ISerializer
	{
		public JsonSerializer()
		{
		}

		public StageItem Deserialize(string data)
		{
			return (new JsonParser()).Parse(data);
		}

		public string Serialize(StageItem item, bool pretty)
		{
			StageElement stageElement = item as StageElement;
			if (stageElement == null)
			{
				throw new ArgumentException("Only StageElements can serve as the root of a serialized graph.");
			}
			return (new StagedToJson(pretty)).Serialize(stageElement);
		}
	}
}