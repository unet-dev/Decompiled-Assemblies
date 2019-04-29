using System;
using UnityEngine;

namespace Facepunch
{
	public abstract class ListComponent<T> : ListComponent
	where T : MonoBehaviour
	{
		private static ListHashSet<T> instanceList;

		public static ListHashSet<T> InstanceList
		{
			get
			{
				return ListComponent<T>.instanceList;
			}
		}

		static ListComponent()
		{
			ListComponent<T>.instanceList = new ListHashSet<T>(8);
		}

		protected ListComponent()
		{
		}

		public override void Clear()
		{
			ListComponent<T>.instanceList.Remove((T)(this as T));
		}

		public override void Setup()
		{
			if (!ListComponent<T>.instanceList.Contains((T)(this as T)))
			{
				ListComponent<T>.instanceList.Add((T)(this as T));
			}
		}
	}
}