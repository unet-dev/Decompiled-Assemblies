using Painting;
using System;
using UnityEngine;
using UnityEngine.Events;

public class UIPaintBox : MonoBehaviour
{
	public UIPaintBox.OnBrushChanged onBrushChanged = new UIPaintBox.OnBrushChanged();

	public Brush brush;

	public UIPaintBox()
	{
	}

	private void OnChanged()
	{
		this.onBrushChanged.Invoke(this.brush);
	}

	public void UpdateBrushAlpha(float a)
	{
		this.brush.color.a = a;
		this.OnChanged();
	}

	public void UpdateBrushColor(Color col)
	{
		this.brush.color.r = col.r;
		this.brush.color.g = col.g;
		this.brush.color.b = col.b;
		this.OnChanged();
	}

	public void UpdateBrushEraser(bool b)
	{
		this.brush.erase = b;
	}

	public void UpdateBrushSize(int size)
	{
		this.brush.brushSize = Vector2.one * (float)size;
		this.brush.spacing = Mathf.Clamp((float)size * 0.1f, 1f, 3f);
		this.OnChanged();
	}

	public void UpdateBrushTexture(Texture2D tex)
	{
		this.brush.texture = tex;
		this.OnChanged();
	}

	[Serializable]
	public class OnBrushChanged : UnityEvent<Brush>
	{
		public OnBrushChanged()
		{
		}
	}
}