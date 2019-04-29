using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	public static class BinaryWriterExtensions
	{
		public static void Write(this BinaryWriter o, Vector3 vec)
		{
			o.Write(vec.x);
			o.Write(vec.y);
			o.Write(vec.z);
		}
	}
}