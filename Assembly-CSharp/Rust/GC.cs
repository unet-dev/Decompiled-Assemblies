using System;
using UnityEngine;

namespace Rust
{
	public class GC : MonoBehaviour, IClientComponent
	{
		public static bool Enabled
		{
			get
			{
				return true;
			}
		}

		public GC()
		{
		}

		public static void Collect()
		{
			System.GC.Collect();
		}
	}
}