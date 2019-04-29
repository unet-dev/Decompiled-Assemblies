using Apex.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Apex.Serialization.Stagers
{
	public sealed class ListStager : IStager
	{
		public Type[] handledTypes
		{
			get
			{
				return new Type[] { typeof(List<>), typeof(Array) };
			}
		}

		public ListStager()
		{
		}

		public StageItem StageValue(string name, object value)
		{
			IList lists = value as IList;
			int count = lists.Count;
			StageList stageList = new StageList(name);
			for (int i = 0; i < count; i++)
			{
				StageItem stageItem = SerializationMaster.Stage("Item", lists[i]);
				stageList.Add(stageItem);
			}
			return stageList;
		}

		public object UnstageValue(StageItem item, Type targetType)
		{
			Type elementType;
			StageItem[] array = ((StageList)item).Items().ToArray<StageItem>();
			if (targetType.IsArray)
			{
				elementType = targetType.GetElementType();
				Array arrays = Array.CreateInstance(elementType, (int)array.Length);
				for (int i = 0; i < (int)array.Length; i++)
				{
					object obj = SerializationMaster.Unstage(array[i], elementType);
					arrays.SetValue(obj, i);
				}
				return arrays;
			}
			elementType = (!targetType.IsGenericType ? typeof(object) : targetType.GetGenericArguments()[0]);
			IList lists = Activator.CreateInstance(targetType, new object[] { (int)array.Length }) as IList;
			for (int j = 0; j < (int)array.Length; j++)
			{
				object obj1 = SerializationMaster.Unstage(array[j], elementType);
				lists.Add(obj1);
			}
			return lists;
		}
	}
}