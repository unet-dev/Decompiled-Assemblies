using System;
using UnityEngine;
using UnityEngine.UI;

public class GridLayoutGroupNeat : GridLayoutGroup
{
	public GridLayoutGroupNeat()
	{
	}

	private float IdealCellWidth(float cellSize)
	{
		Rect rect = base.rectTransform.rect;
		float single = rect.x + (float)(base.padding.left + base.padding.right) * 0.5f;
		float single1 = Mathf.Floor(single / cellSize);
		return single / single1 - this.m_Spacing.x;
	}

	public override void SetLayoutHorizontal()
	{
		Vector2 mCellSize = this.m_CellSize;
		this.m_CellSize.x = this.IdealCellWidth(mCellSize.x);
		base.SetLayoutHorizontal();
		this.m_CellSize = mCellSize;
	}

	public override void SetLayoutVertical()
	{
		Vector2 mCellSize = this.m_CellSize;
		this.m_CellSize.x = this.IdealCellWidth(mCellSize.x);
		base.SetLayoutVertical();
		this.m_CellSize = mCellSize;
	}
}