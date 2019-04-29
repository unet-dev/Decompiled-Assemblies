using Apex.DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Apex
{
	public static class SharedExtensions
	{
		public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
		{
			foreach (T item in items)
			{
				list.Add(item);
			}
		}

		public static void AddRangeUnique<T>(this IList<T> list, IEnumerable<T> items)
		{
			foreach (T item in items)
			{
				list.AddUnique<T>(item);
			}
		}

		public static bool AddUnique<T>(this IList<T> list, T item)
		{
			if (list.Contains(item))
			{
				return false;
			}
			list.Add(item);
			return true;
		}

		public static void Apply<T>(this IEnumerable<T> list, Action<T> action)
		{
			IEnumerator<T> enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				action(enumerator.Current);
			}
		}

		public static void Apply<T>(this IList<T> list, Action<T> action)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				action(list[i]);
			}
		}

		public static void Apply<T>(this IIndexable<T> list, Action<T> action)
		{
			int num = list.count;
			for (int i = 0; i < num; i++)
			{
				action(list[i]);
			}
		}

		public static void EnsureCapacity<T>(this List<T> list, int capacity)
		{
			if (list.Capacity >= capacity)
			{
				return;
			}
			list.Capacity = capacity;
		}

		public static string ExpandFromPascal(this string pascalString)
		{
			if (pascalString == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(pascalString.Length);
			pascalString = pascalString.TrimStart(new char[] { '\u005F' });
			int length = pascalString.Length;
			if (length > 0)
			{
				stringBuilder.Append(char.ToUpper(pascalString[0]));
				for (int i = 1; i < length; i++)
				{
					if (char.IsUpper(pascalString, i) && i + 1 < length && (!char.IsUpper(pascalString, i - 1) || !char.IsUpper(pascalString, i + 1)))
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(pascalString[i]);
				}
			}
			return stringBuilder.ToString();
		}

		public static T[] Fuse<T>(this T[] arrOne, T[] arrTwo)
		{
			if (arrOne == null)
			{
				return arrTwo;
			}
			if (arrTwo == null)
			{
				return arrOne;
			}
			T[] tArray = new T[(int)arrOne.Length + (int)arrTwo.Length];
			Array.Copy(arrOne, tArray, (int)arrOne.Length);
			Array.Copy(arrTwo, 0, tArray, (int)arrOne.Length, (int)arrTwo.Length);
			return tArray;
		}

		public static T GetAttribute<T>(this ICustomAttributeProvider inf, bool inherit)
		where T : Attribute
		{
			T t;
			if (inf == null)
			{
				t = default(T);
				return t;
			}
			object[] customAttributes = inf.GetCustomAttributes(typeof(T), inherit);
			if (customAttributes == null || customAttributes.Length == 0)
			{
				t = default(T);
				return t;
			}
			return (T)customAttributes[0];
		}

		public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider inf, bool inherit)
		where T : Attribute
		{
			if (inf == null)
			{
				return null;
			}
			object[] customAttributes = inf.GetCustomAttributes(typeof(T), inherit);
			if (customAttributes == null)
			{
				return null;
			}
			return customAttributes.Cast<T>();
		}

		public static int IndexOf<T>(this T[] array, T value)
		where T : IEquatable<T>
		{
			for (int i = 0; i < (int)array.Length; i++)
			{
				if (array[i].Equals(value))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool IsDefined<T>(this ICustomAttributeProvider inf, bool inherit)
		where T : Attribute
		{
			if (inf == null)
			{
				return false;
			}
			return inf.IsDefined(typeof(T), inherit);
		}

		public static string PrettyName(this Type t)
		{
			return ((t.IsGenericType ? t.Name.Substring(0, t.Name.IndexOf('\u0060')) : t.Name)).ExpandFromPascal();
		}

		public static void Reorder(this IList list, int fromIdx, int toIdx)
		{
			object item = list[fromIdx];
			if (fromIdx >= toIdx)
			{
				for (int i = fromIdx - 1; i >= toIdx; i--)
				{
					list[i + 1] = list[i];
				}
			}
			else
			{
				for (int j = fromIdx + 1; j <= toIdx; j++)
				{
					list[j - 1] = list[j];
				}
			}
			list[toIdx] = item;
		}

		public static void ReorderList<T>(this IList<T> list, int fromIdx, int toIdx)
		{
			T item = list[fromIdx];
			if (fromIdx >= toIdx)
			{
				for (int i = fromIdx - 1; i >= toIdx; i--)
				{
					list[i + 1] = list[i];
				}
			}
			else
			{
				for (int j = fromIdx + 1; j <= toIdx; j++)
				{
					list[j - 1] = list[j];
				}
			}
			list[toIdx] = item;
		}

		public static string SafeTrim(this string s)
		{
			if (s == null)
			{
				return null;
			}
			return s.Trim();
		}

		public static T[] ToArray<T>(this IIndexable<T> list)
		{
			int num = list.count;
			T[] item = new T[num];
			for (int i = 0; i < num; i++)
			{
				item[i] = list[i];
			}
			return item;
		}

		public static T Value<TKey, T>(this Dictionary<TKey, T> dict, TKey key)
		{
			T t;
			if (dict.TryGetValue(key, out t))
			{
				return t;
			}
			return default(T);
		}
	}
}