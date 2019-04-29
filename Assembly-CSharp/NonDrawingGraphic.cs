using System;
using UnityEngine.UI;

public class NonDrawingGraphic : Graphic
{
	public NonDrawingGraphic()
	{
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}

	public override void SetMaterialDirty()
	{
	}

	public override void SetVerticesDirty()
	{
	}
}