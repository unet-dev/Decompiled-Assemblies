using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class ObjWriter
{
	public static string MeshToString(Mesh mesh)
	{
		int i;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("g ").Append(mesh.name).Append("\n");
		Vector3[] vector3Array = mesh.vertices;
		for (i = 0; i < (int)vector3Array.Length; i++)
		{
			Vector3 vector3 = vector3Array[i];
			stringBuilder.Append(string.Format("v {0} {1} {2}\n", -vector3.x, vector3.y, vector3.z));
		}
		stringBuilder.Append("\n");
		vector3Array = mesh.normals;
		for (i = 0; i < (int)vector3Array.Length; i++)
		{
			Vector3 vector31 = vector3Array[i];
			stringBuilder.Append(string.Format("vn {0} {1} {2}\n", -vector31.x, vector31.y, vector31.z));
		}
		stringBuilder.Append("\n");
		Vector2[] vector2Array = mesh.uv;
		for (i = 0; i < (int)vector2Array.Length; i++)
		{
			Vector3 vector32 = vector2Array[i];
			stringBuilder.Append(string.Format("vt {0} {1}\n", vector32.x, vector32.y));
		}
		stringBuilder.Append("\n");
		int[] numArray = mesh.triangles;
		for (int j = 0; j < (int)numArray.Length; j += 3)
		{
			int num = numArray[j] + 1;
			int num1 = numArray[j + 1] + 1;
			int num2 = numArray[j + 2] + 1;
			stringBuilder.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n", num, num1, num2));
		}
		return stringBuilder.ToString();
	}

	public static void Write(Mesh mesh, string path)
	{
		using (StreamWriter streamWriter = new StreamWriter(path))
		{
			streamWriter.Write(ObjWriter.MeshToString(mesh));
		}
	}
}