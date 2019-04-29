using System;
using UnityEngine;

public class VTP : MonoBehaviour
{
	public VTP()
	{
	}

	private static Vector4[] calculateMeshTangents(int[] triangles, Vector3[] vertices, Vector2[] uv, Vector3[] normals)
	{
		int length = (int)triangles.Length;
		int num = (int)vertices.Length;
		Vector3[] vector3Array = new Vector3[num];
		Vector3[] vector3Array1 = new Vector3[num];
		Vector4[] vector4Array = new Vector4[num];
		for (long i = (long)0; i < (long)length; i += (long)3)
		{
			long num1 = (long)triangles[checked((IntPtr)i)];
			long num2 = (long)triangles[checked((IntPtr)(i + (long)1))];
			long num3 = (long)triangles[checked((IntPtr)(i + (long)2))];
			Vector3 vector3 = vertices[checked((IntPtr)num1)];
			Vector3 vector31 = vertices[checked((IntPtr)num2)];
			Vector3 vector32 = vertices[checked((IntPtr)num3)];
			Vector2 vector2 = uv[checked((IntPtr)num1)];
			Vector2 vector21 = uv[checked((IntPtr)num2)];
			Vector2 vector22 = uv[checked((IntPtr)num3)];
			float single = vector31.x - vector3.x;
			float single1 = vector32.x - vector3.x;
			float single2 = vector31.y - vector3.y;
			float single3 = vector32.y - vector3.y;
			float single4 = vector31.z - vector3.z;
			float single5 = vector32.z - vector3.z;
			float single6 = vector21.x - vector2.x;
			float single7 = vector22.x - vector2.x;
			float single8 = vector21.y - vector2.y;
			float single9 = vector22.y - vector2.y;
			float single10 = single6 * single9 - single7 * single8;
			float single11 = (single10 == 0f ? 0f : 1f / single10);
			Vector3 vector33 = new Vector3((single9 * single - single8 * single1) * single11, (single9 * single2 - single8 * single3) * single11, (single9 * single4 - single8 * single5) * single11);
			Vector3 vector34 = new Vector3((single6 * single1 - single7 * single) * single11, (single6 * single3 - single7 * single2) * single11, (single6 * single5 - single7 * single4) * single11);
			vector3Array[checked((IntPtr)num1)] += vector33;
			vector3Array[checked((IntPtr)num2)] += vector33;
			vector3Array[checked((IntPtr)num3)] += vector33;
			vector3Array1[checked((IntPtr)num1)] += vector34;
			vector3Array1[checked((IntPtr)num2)] += vector34;
			vector3Array1[checked((IntPtr)num3)] += vector34;
		}
		for (long j = (long)0; j < (long)num; j += (long)1)
		{
			Vector3 vector35 = normals[checked((IntPtr)j)];
			Vector3 vector36 = vector3Array[checked((IntPtr)j)];
			Vector3.OrthoNormalize(ref vector35, ref vector36);
			vector4Array[checked((IntPtr)j)].x = vector36.x;
			vector4Array[checked((IntPtr)j)].y = vector36.y;
			vector4Array[checked((IntPtr)j)].z = vector36.z;
			vector4Array[checked((IntPtr)j)].w = (Vector3.Dot(Vector3.Cross(vector35, vector36), vector3Array1[checked((IntPtr)j)]) < 0f ? -1f : 1f);
		}
		return vector4Array;
	}

	public static void deformFaceVerticesOnHit(Transform transform, RaycastHit hit, bool up, float strength, bool recalculateNormals, bool recalculateCollider, bool recalculateFlow)
	{
		Vector3[] component = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] numArray = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Vector3[] vector3Array = transform.GetComponent<MeshFilter>().sharedMesh.normals;
		int num = hit.triangleIndex;
		int num1 = 0;
		int num2 = 1;
		if (!up)
		{
			num2 = -1;
		}
		for (int i = 0; i < 3; i++)
		{
			num1 = numArray[num * 3 + i];
			ref Vector3 vector3Pointer = ref component[num1];
			vector3Pointer = vector3Pointer + ((float)num2 * 0.1f * strength * vector3Array[num1]);
		}
		transform.GetComponent<MeshFilter>().sharedMesh.vertices = component;
		if (recalculateNormals)
		{
			transform.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
		}
		if (recalculateCollider)
		{
			transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().sharedMesh;
		}
		if (recalculateFlow)
		{
			Vector4[] vector4Array = VTP.calculateMeshTangents(numArray, component, transform.GetComponent<MeshCollider>().sharedMesh.uv, vector3Array);
			transform.GetComponent<MeshCollider>().sharedMesh.tangents = vector4Array;
			VTP.recalculateMeshForFlow(transform, component, vector3Array, vector4Array);
		}
	}

	public static void deformSingleVertexOnHit(Transform transform, RaycastHit hit, bool up, float strength, bool recalculateNormals, bool recalculateCollider, bool recalculateFlow)
	{
		Vector3[] component = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] numArray = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Vector3[] vector3Array = transform.GetComponent<MeshFilter>().sharedMesh.normals;
		int num = hit.triangleIndex;
		float single = Single.PositiveInfinity;
		int num1 = 0;
		for (int i = 0; i < 3; i++)
		{
			float single1 = Vector3.Distance(transform.TransformPoint(component[numArray[num * 3 + i]]), hit.point);
			if (single1 < single)
			{
				num1 = numArray[num * 3 + i];
				single = single1;
			}
		}
		int num2 = 1;
		if (!up)
		{
			num2 = -1;
		}
		ref Vector3 vector3Pointer = ref component[num1];
		vector3Pointer = vector3Pointer + ((float)num2 * 0.1f * strength * vector3Array[num1]);
		transform.GetComponent<MeshFilter>().sharedMesh.vertices = component;
		if (recalculateNormals)
		{
			transform.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
		}
		if (recalculateCollider)
		{
			transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().sharedMesh;
		}
		if (recalculateFlow)
		{
			Vector4[] vector4Array = VTP.calculateMeshTangents(numArray, component, transform.GetComponent<MeshCollider>().sharedMesh.uv, vector3Array);
			transform.GetComponent<MeshCollider>().sharedMesh.tangents = vector4Array;
			VTP.recalculateMeshForFlow(transform, component, vector3Array, vector4Array);
		}
	}

	public static Color getFaceVerticesColorAtHit(Transform transform, RaycastHit hit)
	{
		int[] component = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colorArray = transform.GetComponent<MeshFilter>().sharedMesh.colors;
		int num = component[hit.triangleIndex * 3];
		return ((colorArray[num] + colorArray[num + 1]) + colorArray[num + 2]) / 3f;
	}

	public static Color getSingleVertexColorAtHit(Transform transform, RaycastHit hit)
	{
		Vector3[] component = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] numArray = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colorArray = transform.GetComponent<MeshFilter>().sharedMesh.colors;
		int num = hit.triangleIndex;
		float single = Single.PositiveInfinity;
		int num1 = 0;
		for (int i = 0; i < 3; i++)
		{
			float single1 = Vector3.Distance(transform.TransformPoint(component[numArray[num * 3 + i]]), hit.point);
			if (single1 < single)
			{
				num1 = numArray[num * 3 + i];
				single = single1;
			}
		}
		return colorArray[num1];
	}

	public static void paintFaceVerticesOnHit(Transform transform, RaycastHit hit, Color color, float strength)
	{
		int[] component = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colorArray = transform.GetComponent<MeshFilter>().sharedMesh.colors;
		int num = hit.triangleIndex;
		int num1 = 0;
		for (int i = 0; i < 3; i++)
		{
			num1 = component[num * 3 + i];
			Color color1 = VTP.VertexColorLerp(colorArray[num1], color, strength);
			colorArray[num1] = color1;
		}
		transform.GetComponent<MeshFilter>().sharedMesh.colors = colorArray;
	}

	public static void paintSingleVertexOnHit(Transform transform, RaycastHit hit, Color color, float strength)
	{
		Vector3[] component = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] numArray = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colorArray = transform.GetComponent<MeshFilter>().sharedMesh.colors;
		int num = hit.triangleIndex;
		float single = Single.PositiveInfinity;
		int num1 = 0;
		for (int i = 0; i < 3; i += 3)
		{
			float single1 = Vector3.Distance(transform.TransformPoint(component[numArray[num * 3 + i]]), hit.point);
			if (single1 < single)
			{
				num1 = numArray[num * 3 + i];
				single = single1;
			}
		}
		Color color1 = VTP.VertexColorLerp(colorArray[num1], color, strength);
		colorArray[num1] = color1;
		transform.GetComponent<MeshFilter>().sharedMesh.colors = colorArray;
	}

	private static void recalculateMeshForFlow(Transform transform, Vector3[] currentVertices, Vector3[] currentNormals, Vector4[] currentTangents)
	{
		Vector2[] component = transform.GetComponent<MeshFilter>().sharedMesh.uv4;
		for (int i = 0; i < (int)currentVertices.Length; i++)
		{
			Vector3 vector3 = Vector3.Cross(currentNormals[i], new Vector3(currentTangents[i].x, currentTangents[i].y, currentTangents[i].z));
			Vector3 vector31 = transform.TransformDirection(vector3.normalized * currentTangents[i].w);
			Vector3 vector32 = transform.TransformDirection(currentTangents[i].normalized);
			float single = 0.5f + 0.5f * vector32.y;
			float single1 = 0.5f + 0.5f * vector31.y;
			component[i] = new Vector2(single, single1);
		}
		transform.GetComponent<MeshFilter>().sharedMesh.uv4 = component;
	}

	public static Color VertexColorLerp(Color colorA, Color colorB, float value)
	{
		if (value >= 1f)
		{
			return colorB;
		}
		if (value <= 0f)
		{
			return colorA;
		}
		return new Color(colorA.r + (colorB.r - colorA.r) * value, colorA.g + (colorB.g - colorA.g) * value, colorA.b + (colorB.b - colorA.b) * value, colorA.a + (colorB.a - colorA.a) * value);
	}
}