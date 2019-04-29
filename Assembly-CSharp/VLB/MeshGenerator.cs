using System;
using UnityEngine;

namespace VLB
{
	public static class MeshGenerator
	{
		private const float kMinTruncatedRadius = 0.001f;

		private static bool duplicateBackFaces
		{
			get
			{
				return Config.Instance.forceSinglePass;
			}
		}

		public static Mesh GenerateConeZ_Angle(float lengthZ, float coneAngle, int numSides, int numSegments, bool cap)
		{
			return MeshGenerator.GenerateConeZ_RadiusAndAngle(lengthZ, 0f, coneAngle, numSides, numSegments, cap);
		}

		public static Mesh GenerateConeZ_Radius(float lengthZ, float radiusStart, float radiusEnd, int numSides, int numSegments, bool cap)
		{
			Debug.Assert(lengthZ > 0f);
			Debug.Assert(radiusStart >= 0f);
			Debug.Assert(numSides >= 3);
			Debug.Assert(numSegments >= 0);
			Mesh mesh = new Mesh();
			bool flag = false;
			flag = (!cap ? false : radiusStart > 0f);
			radiusStart = Mathf.Max(radiusStart, 0.001f);
			int num = numSides * (numSegments + 2);
			int num1 = num;
			if (flag)
			{
				num1 = num1 + numSides + 1;
			}
			Vector3[] vector3 = new Vector3[num1];
			for (int i = 0; i < numSides; i++)
			{
				float single = 6.28318548f * (float)i / (float)numSides;
				float single1 = Mathf.Cos(single);
				float single2 = Mathf.Sin(single);
				for (int j = 0; j < numSegments + 2; j++)
				{
					float single3 = (float)j / (float)(numSegments + 1);
					Debug.Assert((single3 < 0f ? false : single3 <= 1f));
					float single4 = Mathf.Lerp(radiusStart, radiusEnd, single3);
					vector3[i + j * numSides] = new Vector3(single4 * single1, single4 * single2, single3 * lengthZ);
				}
			}
			if (flag)
			{
				int num2 = num;
				vector3[num2] = Vector3.zero;
				num2++;
				for (int k = 0; k < numSides; k++)
				{
					float single5 = 6.28318548f * (float)k / (float)numSides;
					float single6 = Mathf.Cos(single5);
					float single7 = Mathf.Sin(single5);
					vector3[num2] = new Vector3(radiusStart * single6, radiusStart * single7, 0f);
					num2++;
				}
				Debug.Assert(num2 == (int)vector3.Length);
			}
			if (MeshGenerator.duplicateBackFaces)
			{
				Vector3[] vector3Array = new Vector3[(int)vector3.Length * 2];
				vector3.CopyTo(vector3Array, 0);
				vector3.CopyTo(vector3Array, (int)vector3.Length);
				mesh.vertices = vector3Array;
			}
			else
			{
				mesh.vertices = vector3;
			}
			Vector2[] vector2 = new Vector2[num1];
			int num3 = 0;
			for (int l = 0; l < num; l++)
			{
				int num4 = num3;
				num3 = num4 + 1;
				vector2[num4] = Vector2.zero;
			}
			if (flag)
			{
				for (int m = 0; m < numSides + 1; m++)
				{
					int num5 = num3;
					num3 = num5 + 1;
					vector2[num5] = new Vector2(1f, 0f);
				}
			}
			Debug.Assert(num3 == (int)vector2.Length);
			if (MeshGenerator.duplicateBackFaces)
			{
				Vector2[] vector2Array = new Vector2[(int)vector2.Length * 2];
				vector2.CopyTo(vector2Array, 0);
				vector2.CopyTo(vector2Array, (int)vector2.Length);
				for (int n = 0; n < (int)vector2.Length; n++)
				{
					Vector2 vector21 = vector2Array[n + (int)vector2.Length];
					vector2Array[n + (int)vector2.Length] = new Vector2(vector21.x, 1f);
				}
				mesh.uv = vector2Array;
			}
			else
			{
				mesh.uv = vector2;
			}
			int num6 = numSides * 2 * Mathf.Max(numSegments + 1, 1) * 3;
			if (flag)
			{
				num6 = num6 + numSides * 3;
			}
			int[] numArray = new int[num6];
			int num7 = 0;
			for (int o = 0; o < numSides; o++)
			{
				int num8 = o + 1;
				if (num8 == numSides)
				{
					num8 = 0;
				}
				for (int p = 0; p < numSegments + 1; p++)
				{
					int num9 = p * numSides;
					int num10 = num7;
					num7 = num10 + 1;
					numArray[num10] = num9 + o;
					int num11 = num7;
					num7 = num11 + 1;
					numArray[num11] = num9 + num8;
					int num12 = num7;
					num7 = num12 + 1;
					numArray[num12] = num9 + o + numSides;
					int num13 = num7;
					num7 = num13 + 1;
					numArray[num13] = num9 + num8 + numSides;
					int num14 = num7;
					num7 = num14 + 1;
					numArray[num14] = num9 + o + numSides;
					int num15 = num7;
					num7 = num15 + 1;
					numArray[num15] = num9 + num8;
				}
			}
			if (flag)
			{
				for (int q = 0; q < numSides - 1; q++)
				{
					int num16 = num7;
					num7 = num16 + 1;
					numArray[num16] = num;
					int num17 = num7;
					num7 = num17 + 1;
					numArray[num17] = num + q + 2;
					int num18 = num7;
					num7 = num18 + 1;
					numArray[num18] = num + q + 1;
				}
				int num19 = num7;
				num7 = num19 + 1;
				numArray[num19] = num;
				int num20 = num7;
				num7 = num20 + 1;
				numArray[num20] = num + 1;
				int num21 = num7;
				num7 = num21 + 1;
				numArray[num21] = num + numSides;
			}
			Debug.Assert(num7 == (int)numArray.Length);
			if (MeshGenerator.duplicateBackFaces)
			{
				int[] numArray1 = new int[(int)numArray.Length * 2];
				numArray.CopyTo(numArray1, 0);
				for (int r = 0; r < (int)numArray.Length; r += 3)
				{
					numArray1[(int)numArray.Length + r] = numArray[r] + num1;
					numArray1[(int)numArray.Length + r + 1] = numArray[r + 2] + num1;
					numArray1[(int)numArray.Length + r + 2] = numArray[r + 1] + num1;
				}
				mesh.triangles = numArray1;
			}
			else
			{
				mesh.triangles = numArray;
			}
			Bounds bound = new Bounds(new Vector3(0f, 0f, lengthZ * 0.5f), new Vector3(Mathf.Max(radiusStart, radiusEnd) * 2f, Mathf.Max(radiusStart, radiusEnd) * 2f, lengthZ));
			mesh.bounds = bound;
			Debug.Assert(mesh.vertexCount == MeshGenerator.GetVertexCount(numSides, numSegments, flag));
			Debug.Assert((int)mesh.triangles.Length == MeshGenerator.GetIndicesCount(numSides, numSegments, flag));
			return mesh;
		}

		public static Mesh GenerateConeZ_RadiusAndAngle(float lengthZ, float radiusStart, float coneAngle, int numSides, int numSegments, bool cap)
		{
			Debug.Assert(lengthZ > 0f);
			Debug.Assert((coneAngle <= 0f ? false : coneAngle < 180f));
			float single = lengthZ * Mathf.Tan(coneAngle * 0.0174532924f * 0.5f);
			return MeshGenerator.GenerateConeZ_Radius(lengthZ, radiusStart, single, numSides, numSegments, cap);
		}

		public static int GetIndicesCount(int numSides, int numSegments, bool geomCap)
		{
			Debug.Assert(numSides >= 2);
			Debug.Assert(numSegments >= 0);
			int num = numSides * (numSegments + 1) * 2 * 3;
			if (geomCap)
			{
				num = num + numSides * 3;
			}
			if (MeshGenerator.duplicateBackFaces)
			{
				num *= 2;
			}
			return num;
		}

		public static int GetSharedMeshIndicesCount()
		{
			return MeshGenerator.GetIndicesCount(Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
		}

		public static int GetSharedMeshVertexCount()
		{
			return MeshGenerator.GetVertexCount(Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
		}

		public static int GetVertexCount(int numSides, int numSegments, bool geomCap)
		{
			Debug.Assert(numSides >= 2);
			Debug.Assert(numSegments >= 0);
			int num = numSides * (numSegments + 2);
			if (geomCap)
			{
				num = num + numSides + 1;
			}
			if (MeshGenerator.duplicateBackFaces)
			{
				num *= 2;
			}
			return num;
		}
	}
}