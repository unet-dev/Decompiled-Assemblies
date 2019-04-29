using Newtonsoft.Json.Shims;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal static class CollectionUtils
	{
		public static bool AddDistinct<T>(this IList<T> list, T value)
		{
			return list.AddDistinct<T>(value, EqualityComparer<T>.Default);
		}

		public static bool AddDistinct<T>(this IList<T> list, T value, IEqualityComparer<T> comparer)
		{
			if (list.ContainsValue<T>(value, comparer))
			{
				return false;
			}
			list.Add(value);
			return true;
		}

		public static void AddRange<T>(this IList<T> initial, IEnumerable<T> collection)
		{
			if (initial == null)
			{
				throw new ArgumentNullException("initial");
			}
			if (collection == null)
			{
				return;
			}
			foreach (T t in collection)
			{
				initial.Add(t);
			}
		}

		public static void AddRange<T>(this IList<T> initial, IEnumerable collection)
		{
			ValidationUtils.ArgumentNotNull(initial, "initial");
			initial.AddRange<T>(collection.Cast<T>());
		}

		public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values, IEqualityComparer<T> comparer)
		{
			bool flag = true;
			foreach (T value in values)
			{
				if (list.AddDistinct<T>(value, comparer))
				{
					continue;
				}
				flag = false;
			}
			return flag;
		}

		public static bool Contains<T>(this List<T> list, T value, IEqualityComparer comparer)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (comparer.Equals(value, list[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool ContainsValue<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			bool flag;
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!comparer.Equals(enumerator.Current, value))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		private static void CopyFromJaggedToMultidimensionalArray(IList values, Array multidimensionalArray, int[] indices)
		{
			int length = (int)indices.Length;
			if (length == multidimensionalArray.Rank)
			{
				multidimensionalArray.SetValue(CollectionUtils.JaggedArrayGetValue(values, indices), indices);
				return;
			}
			int num = multidimensionalArray.GetLength(length);
			if (((IList)CollectionUtils.JaggedArrayGetValue(values, indices)).Count != num)
			{
				throw new Exception("Cannot deserialize non-cubical array as multidimensional array.");
			}
			int[] numArray = new int[length + 1];
			for (int i = 0; i < length; i++)
			{
				numArray[i] = indices[i];
			}
			for (int j = 0; j < multidimensionalArray.GetLength(length); j++)
			{
				numArray[length] = j;
				CollectionUtils.CopyFromJaggedToMultidimensionalArray(values, multidimensionalArray, numArray);
			}
		}

		private static IList<int> GetDimensions(IList values, int dimensionsCount)
		{
			IList<int> nums = new List<int>();
			IList lists = values;
			while (true)
			{
				nums.Add(lists.Count);
				if (nums.Count == dimensionsCount || lists.Count == 0)
				{
					break;
				}
				object item = lists[0];
				if (!(item is IList))
				{
					break;
				}
				lists = (IList)item;
			}
			return nums;
		}

		public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			int num;
			int num1 = 0;
			using (IEnumerator<T> enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!predicate(enumerator.Current))
					{
						num1++;
					}
					else
					{
						num = num1;
						return num;
					}
				}
				return -1;
			}
			return num;
		}

		public static int IndexOfReference<T>(this List<T> list, T item)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if ((object)item == (object)list[i])
				{
					return i;
				}
			}
			return -1;
		}

		public static bool IsDictionaryType(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				return true;
			}
			if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IDictionary<,>)))
			{
				return true;
			}
			return false;
		}

		public static bool IsNullOrEmpty<T>(ICollection<T> collection)
		{
			if (collection == null)
			{
				return true;
			}
			return collection.Count == 0;
		}

		private static object JaggedArrayGetValue(IList values, int[] indices)
		{
			IList item = values;
			for (int i = 0; i < (int)indices.Length; i++)
			{
				int num = indices[i];
				if (i == (int)indices.Length - 1)
				{
					return item[num];
				}
				item = (IList)item[num];
			}
			return item;
		}

		public static ConstructorInfo ResolveEnumerableCollectionConstructor(Type collectionType, Type collectionItemType)
		{
			return CollectionUtils.ResolveEnumerableCollectionConstructor(collectionType, collectionItemType, typeof(IList<>).MakeGenericType(new Type[] { collectionItemType }));
		}

		public static ConstructorInfo ResolveEnumerableCollectionConstructor(Type collectionType, Type collectionItemType, Type constructorArgumentType)
		{
			Type type = typeof(IEnumerable<>).MakeGenericType(new Type[] { collectionItemType });
			ConstructorInfo constructorInfo = null;
			ConstructorInfo[] constructors = collectionType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
			for (int i = 0; i < (int)constructors.Length; i++)
			{
				ConstructorInfo constructorInfo1 = constructors[i];
				IList<ParameterInfo> parameters = constructorInfo1.GetParameters();
				if (parameters.Count == 1)
				{
					Type parameterType = parameters[0].ParameterType;
					if (type == parameterType)
					{
						constructorInfo = constructorInfo1;
						break;
					}
					else if (constructorInfo == null && parameterType.IsAssignableFrom(constructorArgumentType))
					{
						constructorInfo = constructorInfo1;
					}
				}
			}
			return constructorInfo;
		}

		public static Array ToMultidimensionalArray(IList values, Type type, int rank)
		{
			IList<int> dimensions = CollectionUtils.GetDimensions(values, rank);
			while (dimensions.Count < rank)
			{
				dimensions.Add(0);
			}
			Array arrays = Array.CreateInstance(type, dimensions.ToArray<int>());
			CollectionUtils.CopyFromJaggedToMultidimensionalArray(values, arrays, new int[0]);
			return arrays;
		}
	}
}