using System;
using UnityEngine;

public class FlameJet : MonoBehaviour
{
	public LineRenderer line;

	public float tesselation = 0.025f;

	private float length;

	public float maxLength = 2f;

	public float drag;

	private int numSegments;

	public bool on;

	private Vector3[] lastWorldSegments;

	private Vector3[] currentSegments;

	public Color startColor;

	public Color endColor;

	public Color currentColor;

	public FlameJet()
	{
	}

	private void Awake()
	{
		this.Initialize();
	}

	private float curve(float x)
	{
		return x * x;
	}

	private void Initialize()
	{
		this.currentColor = this.startColor;
	}

	public void LateUpdate()
	{
		this.UpdateLine();
	}

	public void SetOn(bool isOn)
	{
		this.on = isOn;
	}

	private void UpdateLine()
	{
		float single = this.currentColor.a;
		this.currentColor.a = Mathf.Lerp(single, (this.on ? 1f : 0f), Time.deltaTime * 40f);
		this.line.SetColors(this.currentColor, this.endColor);
		this.tesselation = 0.1f;
		this.numSegments = Mathf.CeilToInt(this.maxLength / this.tesselation);
		float single1 = this.maxLength / (float)this.numSegments;
		Vector3[] vector3Array = new Vector3[this.numSegments];
		for (int i = 0; i < (int)vector3Array.Length; i++)
		{
			float single2 = 0f;
			float single3 = 0f;
			if (this.lastWorldSegments != null && this.lastWorldSegments[i] != Vector3.zero)
			{
				Vector3 vector3 = base.transform.InverseTransformPoint(this.lastWorldSegments[i]);
				float length = (float)i / (float)((int)vector3Array.Length);
				Vector3 vector31 = Vector3.Lerp(vector3, Vector3.zero, Time.deltaTime * this.drag);
				vector31 = Vector3.Lerp(Vector3.zero, vector31, Mathf.Sqrt(length));
				single2 = vector31.x;
				single3 = vector31.y;
			}
			if (i == 0)
			{
				float single4 = 0f;
				single3 = single4;
				single2 = single4;
			}
			Vector3 vector32 = new Vector3(single2, single3, (float)i * single1);
			vector3Array[i] = vector32;
			if (this.lastWorldSegments == null)
			{
				this.lastWorldSegments = new Vector3[this.numSegments];
			}
			this.lastWorldSegments[i] = base.transform.TransformPoint(vector32);
		}
		this.line.SetVertexCount(this.numSegments);
		this.line.SetPositions(vector3Array);
	}
}