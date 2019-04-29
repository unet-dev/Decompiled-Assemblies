using Apex.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Apex.Serialization.Stagers
{
	public sealed class DictionaryStager : IStager
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(Dictionary<,>) };
			}
		}

		public DictionaryStager()
		{
		}

		public StageItem StageValue(string name, object value)
		{
			IDictionary dictionaries = value as IDictionary;
			int count = dictionaries.Count;
			StageList stageList = new StageList(name);
			foreach (object key in dictionaries.Keys)
			{
				stageList.Add(new StageElement(string.Empty, new StageItem[] { SerializationMaster.Stage("key", key), SerializationMaster.Stage("value", dictionaries[key]) }));
			}
			return stageList;
		}

		public object UnstageValue(StageItem item, Type targetType)
		{
			StageElement[] array = ((StageList)item).Elements().ToArray<StageElement>();
			Type[] genericArguments = targetType.GetGenericArguments();
			Type type = genericArguments[0];
			Type type1 = genericArguments[1];
			IDictionary dictionaries = Activator.CreateInstance(targetType, new object[] { (int)array.Length }) as IDictionary;
			for (int i = 0; i < (int)array.Length; i++)
			{
				StageElement stageElement = array[i];
				object obj = SerializationMaster.Unstage(stageElement.Item("key"), type);
				object obj1 = SerializationMaster.Unstage(stageElement.Item("value"), type1);
				dictionaries.Add(obj, obj1);
			}
			return dictionaries;
		}
	}
}