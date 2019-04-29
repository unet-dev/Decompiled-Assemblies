using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Facepunch.Utility
{
	public static class Mesh
	{
		public static void Export(this UnityEngine.Mesh mesh, string filename)
		{
			int i;
			StringBuilder stringBuilder = new StringBuilder();
			Vector3[] vector3Array = mesh.vertices;
			Vector3[] vector3Array1 = mesh.vertices;
			for (i = 0; i < (int)vector3Array1.Length; i++)
			{
				Vector3 vector3 = vector3Array1[i];
				stringBuilder.AppendLine(string.Format("v {0} {1} {2}", vector3.x, vector3.y, vector3.z));
			}
			Vector2[] vector2Array = mesh.uv;
			for (i = 0; i < (int)vector2Array.Length; i++)
			{
				Vector2 vector2 = vector2Array[i];
				stringBuilder.AppendLine(string.Format("vt {0} {1}", vector2.x, vector2.y));
			}
			vector3Array1 = mesh.normals;
			for (i = 0; i < (int)vector3Array1.Length; i++)
			{
				Vector3 vector31 = vector3Array1[i];
				stringBuilder.AppendLine(string.Format("vn {0} {1} {2}", vector31.x, vector31.y, vector31.z));
			}
			for (int j = 0; j < mesh.subMeshCount; j++)
			{
				int[] indices = mesh.GetIndices(j);
				for (int k = 0; k < (int)indices.Length; k += 3)
				{
					stringBuilder.AppendLine(string.Format("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}", new object[] { indices[k] + 1, indices[k] + 1, indices[k] + 1, indices[k + 1] + 1, indices[k + 1] + 1, indices[k + 1] + 1, indices[k + 2] + 1, indices[k + 2] + 1, indices[k + 2] + 1 }));
				}
			}
			if (File.Exists(filename))
			{
				File.Delete(filename);
			}
			File.WriteAllText(filename, stringBuilder.ToString());
		}
	}
}