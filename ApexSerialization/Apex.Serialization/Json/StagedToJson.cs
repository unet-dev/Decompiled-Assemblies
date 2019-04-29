using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Apex.Serialization.Json
{
	internal struct StagedToJson
	{
		private IJsonWriter _json;

		internal StagedToJson(bool pretty)
		{
			if (pretty)
			{
				this._json = new JsonPrettyWriter();
				return;
			}
			this._json = new JsonCompactWriter();
		}

		internal string Serialize(StageElement element)
		{
			this.WriteElement(element);
			return this._json.ToString();
		}

		private void WriteElement(StageElement element)
		{
			bool flag = false;
			this._json.WriteElementStart();
			foreach (StageAttribute stageAttribute in element.Attributes())
			{
				if (!flag)
				{
					flag = true;
				}
				else
				{
					this._json.WriteSeparator();
				}
				this._json.WriteAttributeLabel(stageAttribute);
				this._json.WriteValue(stageAttribute);
			}
			foreach (StageItem stageItem in element.Items())
			{
				if (!flag)
				{
					flag = true;
				}
				else
				{
					this._json.WriteSeparator();
				}
				this._json.WriteLabel(stageItem);
				if (stageItem is StageValue)
				{
					this._json.WriteValue((StageValue)stageItem);
				}
				else if (stageItem is StageElement)
				{
					this.WriteElement((StageElement)stageItem);
				}
				else if (!(stageItem is StageList))
				{
					if (!(stageItem is StageNull))
					{
						continue;
					}
					this._json.WriteNull((StageNull)stageItem);
				}
				else
				{
					this.WriteList((StageList)stageItem);
				}
			}
			this._json.WriteElementEnd();
		}

		private void WriteList(StageList list)
		{
			this._json.WriteListStart();
			bool flag = false;
			foreach (StageItem stageItem in list.Items())
			{
				if (!flag)
				{
					flag = true;
				}
				else
				{
					this._json.WriteSeparator();
				}
				if (stageItem is StageValue)
				{
					this._json.WriteValue((StageValue)stageItem);
				}
				else if (stageItem is StageElement)
				{
					this.WriteElement((StageElement)stageItem);
				}
				else if (!(stageItem is StageList))
				{
					if (!(stageItem is StageNull))
					{
						continue;
					}
					this._json.WriteNull((StageNull)stageItem);
				}
				else
				{
					this.WriteList((StageList)stageItem);
				}
			}
			this._json.WriteListEnd();
		}
	}
}