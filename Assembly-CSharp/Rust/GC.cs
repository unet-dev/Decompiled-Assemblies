using System;
using UnityEngine;

namespace Rust
{
	public class GC : MonoBehaviour, IClientComponent
	{
		public GC()
		{
		}

		public static void Collect()
		{
			System.GC.Collect();
		}
	}
}