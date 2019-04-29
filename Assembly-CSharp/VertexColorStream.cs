using System;
using UnityEngine;

[ExecuteInEditMode]
public class VertexColorStream : MonoBehaviour
{
	[HideInInspector]
	public Mesh originalMesh;

	[HideInInspector]
	public Mesh paintedMesh;

	[HideInInspector]
	public MeshHolder meshHold;

	[HideInInspector]
	public Vector3[] _vertices;

	[HideInInspector]
	public Vector3[] _normals;

	[HideInInspector]
	public int[] _triangles;

	[HideInInspector]
	public int[][] _Subtriangles;

	[HideInInspector]
	public Matrix4x4[] _bindPoses;

	[HideInInspector]
	public BoneWeight[] _boneWeights;

	[HideInInspector]
	public Bounds _bounds;

	[HideInInspector]
	public int _subMeshCount;

	[HideInInspector]
	public Vector4[] _tangents;

	[HideInInspector]
	public Vector2[] _uv;

	[HideInInspector]
	public Vector2[] _uv2;

	[HideInInspector]
	public Vector2[] _uv3;

	[HideInInspector]
	public Color[] _colors;

	[HideInInspector]
	public Vector2[] _uv4;

	public VertexColorStream()
	{
	}

	public Color[] getColors()
	{
		return this.paintedMesh.colors;
	}

	public Vector3[] getNormals()
	{
		return this.paintedMesh.normals;
	}

	public Vector4[] getTangents()
	{
		return this.paintedMesh.tangents;
	}

	public int[] getTriangles()
	{
		return this.paintedMesh.triangles;
	}

	public Vector2[] getUV4s()
	{
		return this.paintedMesh.uv4;
	}

	public Vector2[] getUVs()
	{
		return this.paintedMesh.uv;
	}

	public Vector3[] getVertices()
	{
		return this.paintedMesh.vertices;
	}

	public void init(Mesh origMesh, bool destroyOld)
	{
		this.originalMesh = origMesh;
		this.paintedMesh = UnityEngine.Object.Instantiate<Mesh>(origMesh);
		if (destroyOld)
		{
			UnityEngine.Object.DestroyImmediate(origMesh);
		}
		this.paintedMesh.hideFlags = HideFlags.None;
		this.paintedMesh.name = string.Concat("vpp_", base.gameObject.name);
		this.meshHold = new MeshHolder()
		{
			_vertices = this.paintedMesh.vertices,
			_normals = this.paintedMesh.normals,
			_triangles = this.paintedMesh.triangles,
			_TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount]
		};
		for (int i = 0; i < this.paintedMesh.subMeshCount; i++)
		{
			this.meshHold._TrianglesOfSubs[i] = new trisPerSubmesh();
			this.meshHold._TrianglesOfSubs[i].triangles = this.paintedMesh.GetTriangles(i);
		}
		this.meshHold._bindPoses = this.paintedMesh.bindposes;
		this.meshHold._boneWeights = this.paintedMesh.boneWeights;
		this.meshHold._bounds = this.paintedMesh.bounds;
		this.meshHold._subMeshCount = this.paintedMesh.subMeshCount;
		this.meshHold._tangents = this.paintedMesh.tangents;
		this.meshHold._uv = this.paintedMesh.uv;
		this.meshHold._uv2 = this.paintedMesh.uv2;
		this.meshHold._uv3 = this.paintedMesh.uv3;
		this.meshHold._colors = this.paintedMesh.colors;
		this.meshHold._uv4 = this.paintedMesh.uv4;
		base.GetComponent<MeshFilter>().sharedMesh = this.paintedMesh;
		if (base.GetComponent<MeshCollider>())
		{
			base.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
		}
	}

	private void OnDidApplyAnimationProperties()
	{
	}

	public void rebuild()
	{
		if (!base.GetComponent<MeshFilter>())
		{
			return;
		}
		this.paintedMesh = new Mesh()
		{
			hideFlags = HideFlags.HideAndDontSave,
			name = string.Concat("vpp_", base.gameObject.name)
		};
		if (this.meshHold != null && this.meshHold._vertices.Length != 0 && this.meshHold._TrianglesOfSubs.Length != 0)
		{
			this.paintedMesh.subMeshCount = this.meshHold._subMeshCount;
			this.paintedMesh.vertices = this.meshHold._vertices;
			this.paintedMesh.normals = this.meshHold._normals;
			for (int i = 0; i < this.meshHold._subMeshCount; i++)
			{
				this.paintedMesh.SetTriangles(this.meshHold._TrianglesOfSubs[i].triangles, i);
			}
			this.paintedMesh.bindposes = this.meshHold._bindPoses;
			this.paintedMesh.boneWeights = this.meshHold._boneWeights;
			this.paintedMesh.bounds = this.meshHold._bounds;
			this.paintedMesh.tangents = this.meshHold._tangents;
			this.paintedMesh.uv = this.meshHold._uv;
			this.paintedMesh.uv2 = this.meshHold._uv2;
			this.paintedMesh.uv3 = this.meshHold._uv3;
			this.paintedMesh.colors = this.meshHold._colors;
			this.paintedMesh.uv4 = this.meshHold._uv4;
			this.init(this.paintedMesh, true);
			return;
		}
		this.paintedMesh.subMeshCount = this._subMeshCount;
		this.paintedMesh.vertices = this._vertices;
		this.paintedMesh.normals = this._normals;
		this.paintedMesh.triangles = this._triangles;
		this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount];
		for (int j = 0; j < this.paintedMesh.subMeshCount; j++)
		{
			this.meshHold._TrianglesOfSubs[j] = new trisPerSubmesh();
			this.meshHold._TrianglesOfSubs[j].triangles = this.paintedMesh.GetTriangles(j);
		}
		this.paintedMesh.bindposes = this._bindPoses;
		this.paintedMesh.boneWeights = this._boneWeights;
		this.paintedMesh.bounds = this._bounds;
		this.paintedMesh.tangents = this._tangents;
		this.paintedMesh.uv = this._uv;
		this.paintedMesh.uv2 = this._uv2;
		this.paintedMesh.uv3 = this._uv3;
		this.paintedMesh.colors = this._colors;
		this.paintedMesh.uv4 = this._uv4;
		this.init(this.paintedMesh, true);
	}

	public void setColors(Color[] _vertexColors)
	{
		this.paintedMesh.colors = _vertexColors;
		this.meshHold._colors = _vertexColors;
	}

	public void setTangents(Vector4[] _meshTangents)
	{
		this.paintedMesh.tangents = _meshTangents;
		this.meshHold._tangents = _meshTangents;
	}

	public void setUV4s(Vector2[] _uv4s)
	{
		this.paintedMesh.uv4 = _uv4s;
		this.meshHold._uv4 = _uv4s;
	}

	public Vector3[] setVertices(Vector3[] _deformedVertices)
	{
		this.paintedMesh.vertices = _deformedVertices;
		this.meshHold._vertices = _deformedVertices;
		this.paintedMesh.RecalculateNormals();
		this.paintedMesh.RecalculateBounds();
		this.meshHold._normals = this.paintedMesh.normals;
		this.meshHold._bounds = this.paintedMesh.bounds;
		base.GetComponent<MeshCollider>().sharedMesh = null;
		if (base.GetComponent<MeshCollider>())
		{
			base.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
		}
		return this.meshHold._normals;
	}

	public void setWholeMesh(Mesh tmpMesh)
	{
		this.paintedMesh.vertices = tmpMesh.vertices;
		this.paintedMesh.triangles = tmpMesh.triangles;
		this.paintedMesh.normals = tmpMesh.normals;
		this.paintedMesh.colors = tmpMesh.colors;
		this.paintedMesh.uv = tmpMesh.uv;
		this.paintedMesh.uv2 = tmpMesh.uv2;
		this.paintedMesh.uv3 = tmpMesh.uv3;
		this.meshHold._vertices = tmpMesh.vertices;
		this.meshHold._triangles = tmpMesh.triangles;
		this.meshHold._normals = tmpMesh.normals;
		this.meshHold._colors = tmpMesh.colors;
		this.meshHold._uv = tmpMesh.uv;
		this.meshHold._uv2 = tmpMesh.uv2;
		this.meshHold._uv3 = tmpMesh.uv3;
	}

	private void Start()
	{
		if (!this.paintedMesh || this.meshHold == null)
		{
			this.rebuild();
		}
	}

	public void unlink()
	{
		this.init(this.paintedMesh, false);
	}
}