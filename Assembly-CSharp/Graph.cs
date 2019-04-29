using System;
using UnityEngine;

public abstract class Graph : MonoBehaviour
{
	public UnityEngine.Material Material;

	public int Resolution = 128;

	public Vector2 ScreenFill = new Vector2(0f, 0f);

	public Vector2 ScreenOrigin = new Vector2(0f, 0f);

	public Vector2 Pivot = new Vector2(0f, 0f);

	public Rect Area = new Rect(0f, 0f, 128f, 32f);

	internal float CurrentValue;

	private int index;

	private float[] values;

	private float max;

	protected Graph()
	{
	}

	protected abstract Color GetColor(float value);

	protected abstract float GetValue();

	protected Vector3 GetVertex(float x, float y)
	{
		return new Vector3(x, y, 0f);
	}

	protected void OnGUI()
	{
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		if (this.values == null || this.values.Length == 0)
		{
			return;
		}
		float single = Mathf.Max(this.Area.width, this.ScreenFill.x * (float)Screen.width);
		float single1 = Mathf.Max(this.Area.height, this.ScreenFill.y * (float)Screen.height);
		float area = this.Area.x - this.Pivot.x * single + this.ScreenOrigin.x * (float)Screen.width;
		float area1 = this.Area.y - this.Pivot.y * single1 + this.ScreenOrigin.y * (float)Screen.height;
		GL.PushMatrix();
		this.Material.SetPass(0);
		GL.LoadPixelMatrix();
		GL.Begin(7);
		for (int i = 0; i < (int)this.values.Length; i++)
		{
			float single2 = this.values[i];
			float length = single / (float)((int)this.values.Length);
			float single3 = single1 * single2 / this.max;
			float single4 = area + (float)i * length;
			float single5 = area1;
			GL.Color(this.GetColor(single2));
			GL.Vertex(this.GetVertex(single4 + 0f, single5 + single3));
			GL.Vertex(this.GetVertex(single4 + length, single5 + single3));
			GL.Vertex(this.GetVertex(single4 + length, single5 + 0f));
			GL.Vertex(this.GetVertex(single4 + 0f, single5 + 0f));
		}
		GL.End();
		GL.PopMatrix();
	}

	protected void Update()
	{
		float single;
		if (this.values == null || (int)this.values.Length != this.Resolution)
		{
			this.values = new float[this.Resolution];
		}
		this.max = 0f;
		for (int i = 0; i < (int)this.values.Length - 1; i++)
		{
			float single1 = this.max;
			float[] singleArray = this.values;
			float single2 = this.values[i + 1];
			single = single2;
			singleArray[i] = single2;
			this.max = Mathf.Max(single1, single);
		}
		float single3 = this.max;
		float[] singleArray1 = this.values;
		float value = this.GetValue();
		single = value;
		singleArray1[(int)this.values.Length - 1] = value;
		float single4 = single;
		single = single4;
		this.CurrentValue = single4;
		this.max = Mathf.Max(single3, single);
	}
}