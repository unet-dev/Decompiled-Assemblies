using Painting;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ImagePainter : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IInitializePotentialDragHandler
{
	public ImagePainter.OnDrawingEvent onDrawing = new ImagePainter.OnDrawingEvent();

	public MonoBehaviour redirectRightClick;

	[Tooltip("Spacing scale will depend on your texel size, tweak to what's right.")]
	public float spacingScale = 1f;

	internal Brush brush;

	internal ImagePainter.PointerState[] pointerState = new ImagePainter.PointerState[] { new ImagePainter.PointerState(), new ImagePainter.PointerState(), new ImagePainter.PointerState() };

	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	public ImagePainter()
	{
	}

	private void DrawAt(Vector2 position, PointerEventData.InputButton button)
	{
		if (this.brush == null)
		{
			return;
		}
		ImagePainter.PointerState pointerState = this.pointerState[(int)button];
		Vector2 vector2 = this.rectTransform.Unpivot(position);
		if (!pointerState.isDown)
		{
			this.onDrawing.Invoke(vector2, this.brush);
			pointerState.lastPos = vector2;
			return;
		}
		Vector2 vector21 = pointerState.lastPos - vector2;
		Vector2 vector22 = vector21.normalized;
		for (float i = 0f; i < vector21.magnitude; i = i + Mathf.Max(this.brush.spacing, 1f) * Mathf.Max(this.spacingScale, 0.1f))
		{
			this.onDrawing.Invoke(vector2 + (i * vector22), this.brush);
		}
		pointerState.lastPos = vector2;
	}

	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Right)
		{
			return;
		}
		if (this.redirectRightClick)
		{
			this.redirectRightClick.SendMessage("OnBeginDrag", eventData);
		}
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		Vector2 vector2;
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnDrag", eventData);
			}
			return;
		}
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, eventData.pressEventCamera, out vector2);
		this.DrawAt(vector2, eventData.button);
	}

	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Right)
		{
			return;
		}
		if (this.redirectRightClick)
		{
			this.redirectRightClick.SendMessage("OnEndDrag", eventData);
		}
	}

	public virtual void OnInitializePotentialDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Right)
		{
			return;
		}
		if (this.redirectRightClick)
		{
			this.redirectRightClick.SendMessage("OnInitializePotentialDrag", eventData);
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		Vector2 vector2;
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			return;
		}
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, eventData.pressEventCamera, out vector2);
		this.DrawAt(vector2, eventData.button);
		this.pointerState[(int)eventData.button].isDown = true;
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		this.pointerState[(int)eventData.button].isDown = false;
	}

	private void Start()
	{
	}

	public void UpdateBrush(Brush brush)
	{
		this.brush = brush;
	}

	[Serializable]
	public class OnDrawingEvent : UnityEvent<Vector2, Brush>
	{
		public OnDrawingEvent()
		{
		}
	}

	internal class PointerState
	{
		public Vector2 lastPos;

		public bool isDown;

		public PointerState()
		{
		}
	}
}