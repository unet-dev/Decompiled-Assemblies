using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectZoom : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	public ScrollRectEx scrollRect;

	public float zoom = 1f;

	public bool smooth = true;

	public float max = 1.5f;

	public float min = 0.5f;

	public float scrollAmount = 0.2f;

	public RectTransform rectTransform
	{
		get
		{
			return this.scrollRect.transform as RectTransform;
		}
	}

	public ScrollRectZoom()
	{
	}

	private void OnEnable()
	{
		this.SetZoom(this.zoom);
	}

	public void OnScroll(PointerEventData data)
	{
		this.SetZoom(this.zoom + this.scrollAmount * data.scrollDelta.y);
	}

	private void SetZoom(float z)
	{
		z = Mathf.Clamp(z, this.min, this.max);
		this.zoom = z;
		Rect rect = this.scrollRect.content.rect;
		Vector2 vector2 = rect.size * this.zoom;
		Vector2 vector21 = this.scrollRect.normalizedPosition;
		this.scrollRect.content.localScale = Vector3.one * Mathf.Exp(this.zoom);
		this.scrollRect.normalizedPosition = vector21;
	}
}