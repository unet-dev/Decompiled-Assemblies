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

	public float velocity;

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
		this.velocity = this.velocity + data.scrollDelta.y * 0.001f;
		this.velocity = Mathf.Clamp(this.velocity, -1f, 1f);
	}

	private void SetZoom(float z)
	{
		z = Mathf.Clamp(z, this.min, this.max);
		if (z == this.zoom)
		{
			return;
		}
		this.zoom = z;
		Rect rect = (this.scrollRect.transform as RectTransform).rect;
		Rect rect1 = this.scrollRect.content.rect;
		Vector2 vector2 = rect1.size * this.zoom;
		Vector2 vector21 = this.scrollRect.normalizedPosition;
		if (vector2.x < rect.width)
		{
			float single = rect.width;
			rect1 = this.scrollRect.content.rect;
			this.zoom = single / rect1.size.x;
		}
		if (vector2.y < rect.height)
		{
			float single1 = rect.height;
			rect1 = this.scrollRect.content.rect;
			this.zoom = single1 / rect1.size.y;
		}
		this.scrollRect.content.localScale = Vector3.one * this.zoom;
		this.scrollRect.normalizedPosition = vector21;
	}

	private void Update()
	{
		this.velocity = Mathf.Lerp(this.velocity, 0f, Time.deltaTime * 10f);
		this.SetZoom(this.zoom + this.velocity);
	}
}