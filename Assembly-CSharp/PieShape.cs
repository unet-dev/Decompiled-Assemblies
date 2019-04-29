using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PieShape : Graphic
{
	[Range(0f, 1f)]
	public float outerSize = 1f;

	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	public float startRadius = -45f;

	public float endRadius = 45f;

	public float border;

	public bool debugDrawing;

	public PieShape()
	{
	}

	protected override void OnPopulateMesh(VertexHelper vbo)
	{
		vbo.Clear();
		UIVertex vector2 = UIVertex.simpleVert;
		float single = this.startRadius;
		float single1 = this.endRadius;
		if (this.startRadius > this.endRadius)
		{
			single1 = this.endRadius + 360f;
		}
		float single2 = Mathf.Floor((single1 - single) / 6f);
		if (single2 <= 1f)
		{
			return;
		}
		float single3 = (single1 - single) / (float)single2;
		float single4 = single + (single1 - single) * 0.5f;
		Color color = this.color;
		float single5 = base.rectTransform.rect.height * 0.5f;
		Vector2 vector21 = new Vector2(Mathf.Sin(single4 * 0.0174532924f), Mathf.Cos(single4 * 0.0174532924f)) * this.border;
		int num = 0;
		for (float i = single; i < single1; i += single3)
		{
			if (this.debugDrawing)
			{
				color = (color != Color.red ? Color.red : Color.white);
			}
			vector2.color = color;
			float single6 = Mathf.Sin(i * 0.0174532924f);
			float single7 = Mathf.Cos(i * 0.0174532924f);
			float single8 = i + single3;
			if (single8 > single1)
			{
				single8 = single1;
			}
			float single9 = Mathf.Sin(single8 * 0.0174532924f);
			float single10 = Mathf.Cos(single8 * 0.0174532924f);
			vector2.position = new Vector2(single6 * this.outerSize * single5, single7 * this.outerSize * single5) + vector21;
			vbo.AddVert(vector2);
			vector2.position = new Vector2(single9 * this.outerSize * single5, single10 * this.outerSize * single5) + vector21;
			vbo.AddVert(vector2);
			vector2.position = new Vector2(single9 * this.innerSize * single5, single10 * this.innerSize * single5) + vector21;
			vbo.AddVert(vector2);
			vector2.position = new Vector2(single6 * this.innerSize * single5, single7 * this.innerSize * single5) + vector21;
			vbo.AddVert(vector2);
			vbo.AddTriangle(num, num + 1, num + 2);
			vbo.AddTriangle(num + 2, num + 3, num);
			num += 4;
		}
	}
}