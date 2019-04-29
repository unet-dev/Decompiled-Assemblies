using Rust;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Scroll Rect Ex", 37)]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[SelectionBase]
	public class ScrollRectEx : UIBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutGroup, ILayoutController
	{
		public PointerEventData.InputButton scrollButton;

		public PointerEventData.InputButton altScrollButton;

		[SerializeField]
		private RectTransform m_Content;

		[SerializeField]
		private bool m_Horizontal = true;

		[SerializeField]
		private bool m_Vertical = true;

		[SerializeField]
		private ScrollRectEx.MovementType m_MovementType = ScrollRectEx.MovementType.Elastic;

		[SerializeField]
		private float m_Elasticity = 0.1f;

		[SerializeField]
		private bool m_Inertia = true;

		[SerializeField]
		private float m_DecelerationRate = 0.135f;

		[SerializeField]
		private float m_ScrollSensitivity = 1f;

		[SerializeField]
		private RectTransform m_Viewport;

		[SerializeField]
		private Scrollbar m_HorizontalScrollbar;

		[SerializeField]
		private Scrollbar m_VerticalScrollbar;

		[SerializeField]
		private ScrollRectEx.ScrollbarVisibility m_HorizontalScrollbarVisibility;

		[SerializeField]
		private ScrollRectEx.ScrollbarVisibility m_VerticalScrollbarVisibility;

		[SerializeField]
		private float m_HorizontalScrollbarSpacing;

		[SerializeField]
		private float m_VerticalScrollbarSpacing;

		[SerializeField]
		private ScrollRectEx.ScrollRectEvent m_OnValueChanged = new ScrollRectEx.ScrollRectEvent();

		private Vector2 m_PointerStartLocalCursor = Vector2.zero;

		private Vector2 m_ContentStartPosition = Vector2.zero;

		private RectTransform m_ViewRect;

		private Bounds m_ContentBounds;

		private Bounds m_ViewBounds;

		private Vector2 m_Velocity;

		private bool m_Dragging;

		private Vector2 m_PrevPosition = Vector2.zero;

		private Bounds m_PrevContentBounds;

		private Bounds m_PrevViewBounds;

		[NonSerialized]
		private bool m_HasRebuiltLayout;

		private bool m_HSliderExpand;

		private bool m_VSliderExpand;

		private float m_HSliderHeight;

		private float m_VSliderWidth;

		[NonSerialized]
		private RectTransform m_Rect;

		private RectTransform m_HorizontalScrollbarRect;

		private RectTransform m_VerticalScrollbarRect;

		private DrivenRectTransformTracker m_Tracker;

		private readonly Vector3[] m_Corners = new Vector3[4];

		public RectTransform content
		{
			get
			{
				return this.m_Content;
			}
			set
			{
				this.m_Content = value;
			}
		}

		public float decelerationRate
		{
			get
			{
				return this.m_DecelerationRate;
			}
			set
			{
				this.m_DecelerationRate = value;
			}
		}

		public float elasticity
		{
			get
			{
				return this.m_Elasticity;
			}
			set
			{
				this.m_Elasticity = value;
			}
		}

		public bool horizontal
		{
			get
			{
				return this.m_Horizontal;
			}
			set
			{
				this.m_Horizontal = value;
			}
		}

		public float horizontalNormalizedPosition
		{
			get
			{
				object obj;
				this.UpdateBounds();
				if (this.m_ContentBounds.size.x <= this.m_ViewBounds.size.x)
				{
					if (this.m_ViewBounds.min.x > this.m_ContentBounds.min.x)
					{
						obj = 1;
					}
					else
					{
						obj = null;
					}
					return (float)obj;
				}
				return (this.m_ViewBounds.min.x - this.m_ContentBounds.min.x) / (this.m_ContentBounds.size.x - this.m_ViewBounds.size.x);
			}
			set
			{
				this.SetNormalizedPosition(value, 0);
			}
		}

		public Scrollbar horizontalScrollbar
		{
			get
			{
				return this.m_HorizontalScrollbar;
			}
			set
			{
				if (this.m_HorizontalScrollbar)
				{
					this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
				this.m_HorizontalScrollbar = value;
				if (this.m_HorizontalScrollbar)
				{
					this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
				this.SetDirtyCaching();
			}
		}

		public float horizontalScrollbarSpacing
		{
			get
			{
				return this.m_HorizontalScrollbarSpacing;
			}
			set
			{
				this.m_HorizontalScrollbarSpacing = value;
				this.SetDirty();
			}
		}

		public ScrollRectEx.ScrollbarVisibility horizontalScrollbarVisibility
		{
			get
			{
				return this.m_HorizontalScrollbarVisibility;
			}
			set
			{
				this.m_HorizontalScrollbarVisibility = value;
				this.SetDirtyCaching();
			}
		}

		private bool hScrollingNeeded
		{
			get
			{
				if (!UnityEngine.Application.isPlaying)
				{
					return true;
				}
				return this.m_ContentBounds.size.x > this.m_ViewBounds.size.x + 0.01f;
			}
		}

		public bool inertia
		{
			get
			{
				return this.m_Inertia;
			}
			set
			{
				this.m_Inertia = value;
			}
		}

		public ScrollRectEx.MovementType movementType
		{
			get
			{
				return this.m_MovementType;
			}
			set
			{
				this.m_MovementType = value;
			}
		}

		public Vector2 normalizedPosition
		{
			get
			{
				return new Vector2(this.horizontalNormalizedPosition, this.verticalNormalizedPosition);
			}
			set
			{
				this.SetNormalizedPosition(value.x, 0);
				this.SetNormalizedPosition(value.y, 1);
			}
		}

		public ScrollRectEx.ScrollRectEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		private RectTransform rectTransform
		{
			get
			{
				if (this.m_Rect == null)
				{
					this.m_Rect = base.GetComponent<RectTransform>();
				}
				return this.m_Rect;
			}
		}

		public float scrollSensitivity
		{
			get
			{
				return this.m_ScrollSensitivity;
			}
			set
			{
				this.m_ScrollSensitivity = value;
			}
		}

		public Vector2 velocity
		{
			get
			{
				return this.m_Velocity;
			}
			set
			{
				this.m_Velocity = value;
			}
		}

		public bool vertical
		{
			get
			{
				return this.m_Vertical;
			}
			set
			{
				this.m_Vertical = value;
			}
		}

		public float verticalNormalizedPosition
		{
			get
			{
				object obj;
				this.UpdateBounds();
				if (this.m_ContentBounds.size.y <= this.m_ViewBounds.size.y)
				{
					if (this.m_ViewBounds.min.y > this.m_ContentBounds.min.y)
					{
						obj = 1;
					}
					else
					{
						obj = null;
					}
					return (float)obj;
				}
				return (this.m_ViewBounds.min.y - this.m_ContentBounds.min.y) / (this.m_ContentBounds.size.y - this.m_ViewBounds.size.y);
			}
			set
			{
				this.SetNormalizedPosition(value, 1);
			}
		}

		public Scrollbar verticalScrollbar
		{
			get
			{
				return this.m_VerticalScrollbar;
			}
			set
			{
				if (this.m_VerticalScrollbar)
				{
					this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
				this.m_VerticalScrollbar = value;
				if (this.m_VerticalScrollbar)
				{
					this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
				this.SetDirtyCaching();
			}
		}

		public float verticalScrollbarSpacing
		{
			get
			{
				return this.m_VerticalScrollbarSpacing;
			}
			set
			{
				this.m_VerticalScrollbarSpacing = value;
				this.SetDirty();
			}
		}

		public ScrollRectEx.ScrollbarVisibility verticalScrollbarVisibility
		{
			get
			{
				return this.m_VerticalScrollbarVisibility;
			}
			set
			{
				this.m_VerticalScrollbarVisibility = value;
				this.SetDirtyCaching();
			}
		}

		public RectTransform viewport
		{
			get
			{
				return this.m_Viewport;
			}
			set
			{
				this.m_Viewport = value;
				this.SetDirtyCaching();
			}
		}

		protected RectTransform viewRect
		{
			get
			{
				if (this.m_ViewRect == null)
				{
					this.m_ViewRect = this.m_Viewport;
				}
				if (this.m_ViewRect == null)
				{
					this.m_ViewRect = (RectTransform)base.transform;
				}
				return this.m_ViewRect;
			}
		}

		private bool vScrollingNeeded
		{
			get
			{
				if (!UnityEngine.Application.isPlaying)
				{
					return true;
				}
				return this.m_ContentBounds.size.y > this.m_ViewBounds.size.y + 0.01f;
			}
		}

		protected ScrollRectEx()
		{
		}

		private Vector2 CalculateOffset(Vector2 delta)
		{
			Vector2 mViewBounds = Vector2.zero;
			if (this.m_MovementType == ScrollRectEx.MovementType.Unrestricted)
			{
				return mViewBounds;
			}
			Vector2 mContentBounds = this.m_ContentBounds.min;
			Vector2 vector2 = this.m_ContentBounds.max;
			if (this.m_Horizontal)
			{
				mContentBounds.x += delta.x;
				vector2.x += delta.x;
				if (mContentBounds.x > this.m_ViewBounds.min.x)
				{
					mViewBounds.x = this.m_ViewBounds.min.x - mContentBounds.x;
				}
				else if (vector2.x < this.m_ViewBounds.max.x)
				{
					mViewBounds.x = this.m_ViewBounds.max.x - vector2.x;
				}
			}
			if (this.m_Vertical)
			{
				mContentBounds.y += delta.y;
				vector2.y += delta.y;
				if (vector2.y < this.m_ViewBounds.max.y)
				{
					mViewBounds.y = this.m_ViewBounds.max.y - vector2.y;
				}
				else if (mContentBounds.y > this.m_ViewBounds.min.y)
				{
					mViewBounds.y = this.m_ViewBounds.min.y - mContentBounds.y;
				}
			}
			return mViewBounds;
		}

		public void CenterOnPosition(Vector2 pos)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			Vector2 vector2 = new Vector2(this.content.localScale.x, this.content.localScale.y);
			pos.x *= vector2.x;
			pos.y *= vector2.y;
			Rect rect = this.content.rect;
			float single = rect.width * vector2.x;
			rect = rectTransform.rect;
			float single1 = single - rect.width;
			rect = this.content.rect;
			float single2 = rect.height * vector2.y;
			rect = rectTransform.rect;
			Vector2 vector21 = new Vector2(single1, single2 - rect.height);
			pos.x = pos.x / vector21.x + this.content.pivot.x;
			pos.y = pos.y / vector21.y + this.content.pivot.y;
			if (this.movementType != ScrollRectEx.MovementType.Unrestricted)
			{
				pos.x = Mathf.Clamp(pos.x, 0f, 1f);
				pos.y = Mathf.Clamp(pos.y, 0f, 1f);
			}
			this.normalizedPosition = pos;
		}

		private void EnsureLayoutHasRebuilt()
		{
			if (!this.m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
			{
				Canvas.ForceUpdateCanvases();
			}
		}

		private Bounds GetBounds()
		{
			if (this.m_Content == null)
			{
				return new Bounds();
			}
			Vector3 vector3 = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
			Vector3 vector31 = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);
			Matrix4x4 matrix4x4 = this.viewRect.worldToLocalMatrix;
			this.m_Content.GetWorldCorners(this.m_Corners);
			for (int i = 0; i < 4; i++)
			{
				Vector3 vector32 = matrix4x4.MultiplyPoint3x4(this.m_Corners[i]);
				vector3 = Vector3.Min(vector32, vector3);
				vector31 = Vector3.Max(vector32, vector31);
			}
			Bounds bound = new Bounds(vector3, Vector3.zero);
			bound.Encapsulate(vector31);
			return bound;
		}

		public void GraphicUpdateComplete()
		{
		}

		public override bool IsActive()
		{
			if (!base.IsActive())
			{
				return false;
			}
			return this.m_Content != null;
		}

		protected virtual void LateUpdate()
		{
			if (!this.m_Content)
			{
				return;
			}
			this.EnsureLayoutHasRebuilt();
			this.UpdateScrollbarVisibility();
			this.UpdateBounds();
			float single = Time.unscaledDeltaTime;
			Vector2 vector2 = this.CalculateOffset(Vector2.zero);
			if (!this.m_Dragging && (vector2 != Vector2.zero || this.m_Velocity != Vector2.zero))
			{
				Vector2 mContent = this.m_Content.anchoredPosition;
				for (int i = 0; i < 2; i++)
				{
					if (this.m_MovementType == ScrollRectEx.MovementType.Elastic && vector2[i] != 0f)
					{
						float item = this.m_Velocity[i];
						Vector2 mContent1 = this.m_Content.anchoredPosition;
						float item1 = mContent1[i];
						mContent1 = this.m_Content.anchoredPosition;
						mContent[i] = Mathf.SmoothDamp(item1, mContent1[i] + vector2[i], ref item, this.m_Elasticity, Single.PositiveInfinity, single);
						this.m_Velocity[i] = item;
					}
					else if (!this.m_Inertia)
					{
						this.m_Velocity[i] = 0f;
					}
					else
					{
						&mContent = &this.m_Velocity;
						int num = i;
						mContent[num] = mContent[num] * Mathf.Pow(this.m_DecelerationRate, single);
						if (Mathf.Abs(this.m_Velocity[i]) < 1f)
						{
							this.m_Velocity[i] = 0f;
						}
						num = i;
						mContent[num] = mContent[num] + this.m_Velocity[i] * single;
					}
				}
				if (this.m_Velocity != Vector2.zero)
				{
					if (this.m_MovementType == ScrollRectEx.MovementType.Clamped)
					{
						vector2 = this.CalculateOffset(mContent - this.m_Content.anchoredPosition);
						mContent += vector2;
					}
					this.SetContentAnchoredPosition(mContent);
				}
			}
			if (this.m_Dragging && this.m_Inertia)
			{
				Vector3 vector3 = (this.m_Content.anchoredPosition - this.m_PrevPosition) / single;
				this.m_Velocity = Vector3.Lerp(this.m_Velocity, vector3, single * 10f);
			}
			if (this.m_ViewBounds != this.m_PrevViewBounds || this.m_ContentBounds != this.m_PrevContentBounds || this.m_Content.anchoredPosition != this.m_PrevPosition)
			{
				this.UpdateScrollbars(vector2);
				this.m_OnValueChanged.Invoke(this.normalizedPosition);
				this.UpdatePrevData();
			}
		}

		public void LayoutComplete()
		{
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			if (!this.IsActive())
			{
				return;
			}
			this.UpdateBounds();
			this.m_PointerStartLocalCursor = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, out this.m_PointerStartLocalCursor);
			this.m_ContentStartPosition = this.m_Content.anchoredPosition;
			this.m_Dragging = true;
		}

		protected override void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_HorizontalScrollbar)
			{
				this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			this.m_HasRebuiltLayout = false;
			this.m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			base.OnDisable();
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			Vector2 vector2;
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			if (!this.IsActive())
			{
				return;
			}
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, out vector2))
			{
				return;
			}
			this.UpdateBounds();
			Vector2 mPointerStartLocalCursor = vector2 - this.m_PointerStartLocalCursor;
			Vector2 mContentStartPosition = this.m_ContentStartPosition + mPointerStartLocalCursor;
			Vector2 vector21 = this.CalculateOffset(mContentStartPosition - this.m_Content.anchoredPosition);
			mContentStartPosition += vector21;
			if (this.m_MovementType == ScrollRectEx.MovementType.Elastic)
			{
				if (vector21.x != 0f)
				{
					mContentStartPosition.x -= ScrollRectEx.RubberDelta(vector21.x, this.m_ViewBounds.size.x);
				}
				if (vector21.y != 0f)
				{
					mContentStartPosition.y -= ScrollRectEx.RubberDelta(vector21.y, this.m_ViewBounds.size.y);
				}
			}
			this.SetContentAnchoredPosition(mContentStartPosition);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_HorizontalScrollbar)
			{
				this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			this.m_Dragging = false;
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			this.m_Velocity = Vector2.zero;
		}

		protected override void OnRectTransformDimensionsChange()
		{
			this.SetDirty();
		}

		public virtual void OnScroll(PointerEventData data)
		{
			if (!this.IsActive())
			{
				return;
			}
			this.EnsureLayoutHasRebuilt();
			this.UpdateBounds();
			Vector2 vector2 = data.scrollDelta;
			vector2.y *= -1f;
			if (this.vertical && !this.horizontal)
			{
				if (Mathf.Abs(vector2.x) > Mathf.Abs(vector2.y))
				{
					vector2.y = vector2.x;
				}
				vector2.x = 0f;
			}
			if (this.horizontal && !this.vertical)
			{
				if (Mathf.Abs(vector2.y) > Mathf.Abs(vector2.x))
				{
					vector2.x = vector2.y;
				}
				vector2.y = 0f;
			}
			Vector2 mContent = this.m_Content.anchoredPosition;
			mContent = mContent + (vector2 * this.m_ScrollSensitivity);
			if (this.m_MovementType == ScrollRectEx.MovementType.Clamped)
			{
				mContent += this.CalculateOffset(mContent - this.m_Content.anchoredPosition);
			}
			this.SetContentAnchoredPosition(mContent);
			this.UpdateBounds();
		}

		public virtual void Rebuild(CanvasUpdate executing)
		{
			if (executing == CanvasUpdate.Prelayout)
			{
				this.UpdateCachedData();
			}
			if (executing == CanvasUpdate.PostLayout)
			{
				this.UpdateBounds();
				this.UpdateScrollbars(Vector2.zero);
				this.UpdatePrevData();
				this.m_HasRebuiltLayout = true;
			}
		}

		private static float RubberDelta(float overStretching, float viewSize)
		{
			return (1f - 1f / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1f)) * viewSize * Mathf.Sign(overStretching);
		}

		protected virtual void SetContentAnchoredPosition(Vector2 position)
		{
			if (!this.m_Horizontal)
			{
				position.x = this.m_Content.anchoredPosition.x;
			}
			if (!this.m_Vertical)
			{
				position.y = this.m_Content.anchoredPosition.y;
			}
			if (position != this.m_Content.anchoredPosition)
			{
				this.m_Content.anchoredPosition = position;
				this.UpdateBounds();
			}
		}

		protected void SetDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
		}

		protected void SetDirtyCaching()
		{
			if (!this.IsActive())
			{
				return;
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
		}

		private void SetHorizontalNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 0);
		}

		public virtual void SetLayoutHorizontal()
		{
			Rect rect;
			this.m_Tracker.Clear();
			if (this.m_HSliderExpand || this.m_VSliderExpand)
			{
				this.m_Tracker.Add(this, this.viewRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.AnchorMin | DrivenTransformProperties.AnchorMax | DrivenTransformProperties.Anchors | DrivenTransformProperties.SizeDelta);
				this.viewRect.anchorMin = Vector2.zero;
				this.viewRect.anchorMax = Vector2.one;
				this.viewRect.sizeDelta = Vector2.zero;
				this.viewRect.anchoredPosition = Vector2.zero;
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
				rect = this.viewRect.rect;
				Vector3 vector3 = rect.center;
				rect = this.viewRect.rect;
				this.m_ViewBounds = new Bounds(vector3, rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_VSliderExpand && this.vScrollingNeeded)
			{
				this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
				rect = this.viewRect.rect;
				Vector3 vector31 = rect.center;
				rect = this.viewRect.rect;
				this.m_ViewBounds = new Bounds(vector31, rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_HSliderExpand && this.hScrollingNeeded)
			{
				this.viewRect.sizeDelta = new Vector2(this.viewRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
				rect = this.viewRect.rect;
				Vector3 vector32 = rect.center;
				rect = this.viewRect.rect;
				this.m_ViewBounds = new Bounds(vector32, rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_VSliderExpand && this.vScrollingNeeded && this.viewRect.sizeDelta.x == 0f && this.viewRect.sizeDelta.y < 0f)
			{
				this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
			}
		}

		public virtual void SetLayoutVertical()
		{
			this.UpdateScrollbarLayout();
			Rect rect = this.viewRect.rect;
			Vector3 vector3 = rect.center;
			rect = this.viewRect.rect;
			this.m_ViewBounds = new Bounds(vector3, rect.size);
			this.m_ContentBounds = this.GetBounds();
		}

		private void SetNormalizedPosition(float value, int axis)
		{
			this.EnsureLayoutHasRebuilt();
			this.UpdateBounds();
			Vector3 mContentBounds = this.m_ContentBounds.size;
			float item = mContentBounds[axis];
			mContentBounds = this.m_ViewBounds.size;
			float single = item - mContentBounds[axis];
			mContentBounds = this.m_ViewBounds.min;
			float item1 = mContentBounds[axis] - value * single;
			mContentBounds = this.m_Content.localPosition;
			float single1 = mContentBounds[axis] + item1;
			mContentBounds = this.m_ContentBounds.min;
			float item2 = single1 - mContentBounds[axis];
			Vector3 mContent = this.m_Content.localPosition;
			if (Mathf.Abs(mContent[axis] - item2) > 0.01f)
			{
				mContent[axis] = item2;
				this.m_Content.localPosition = mContent;
				this.m_Velocity[axis] = 0f;
				this.UpdateBounds();
			}
		}

		private void SetVerticalNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 1);
		}

		public virtual void StopMovement()
		{
			this.m_Velocity = Vector2.zero;
		}

		Transform UnityEngine.UI.ICanvasElement.get_transform()
		{
			return base.transform;
		}

		private void UpdateBounds()
		{
			Rect rect = this.viewRect.rect;
			Vector3 vector3 = rect.center;
			rect = this.viewRect.rect;
			this.m_ViewBounds = new Bounds(vector3, rect.size);
			this.m_ContentBounds = this.GetBounds();
			if (this.m_Content == null)
			{
				return;
			}
			Vector3 mContentBounds = this.m_ContentBounds.size;
			Vector3 mContentBounds1 = this.m_ContentBounds.center;
			Vector3 mViewBounds = this.m_ViewBounds.size - mContentBounds;
			if (mViewBounds.x > 0f)
			{
				ref float mContent = ref mContentBounds1.x;
				mContent = mContent - mViewBounds.x * (this.m_Content.pivot.x - 0.5f);
				mContentBounds.x = this.m_ViewBounds.size.x;
			}
			if (mViewBounds.y > 0f)
			{
				ref float singlePointer = ref mContentBounds1.y;
				singlePointer = singlePointer - mViewBounds.y * (this.m_Content.pivot.y - 0.5f);
				mContentBounds.y = this.m_ViewBounds.size.y;
			}
			this.m_ContentBounds.size = mContentBounds;
			this.m_ContentBounds.center = mContentBounds1;
		}

		private void UpdateCachedData()
		{
			RectTransform mHorizontalScrollbar;
			RectTransform mVerticalScrollbar;
			Transform transforms = base.transform;
			if (this.m_HorizontalScrollbar == null)
			{
				mHorizontalScrollbar = null;
			}
			else
			{
				mHorizontalScrollbar = this.m_HorizontalScrollbar.transform as RectTransform;
			}
			this.m_HorizontalScrollbarRect = mHorizontalScrollbar;
			if (this.m_VerticalScrollbar == null)
			{
				mVerticalScrollbar = null;
			}
			else
			{
				mVerticalScrollbar = this.m_VerticalScrollbar.transform as RectTransform;
			}
			this.m_VerticalScrollbarRect = mVerticalScrollbar;
			bool flag = this.viewRect.parent == transforms;
			bool flag1 = (!this.m_HorizontalScrollbarRect ? true : this.m_HorizontalScrollbarRect.parent == transforms);
			bool flag2 = flag & flag1 & (!this.m_VerticalScrollbarRect ? true : this.m_VerticalScrollbarRect.parent == transforms);
			this.m_HSliderExpand = (!flag2 || !this.m_HorizontalScrollbarRect ? false : this.horizontalScrollbarVisibility == ScrollRectEx.ScrollbarVisibility.AutoHideAndExpandViewport);
			this.m_VSliderExpand = (!flag2 || !this.m_VerticalScrollbarRect ? false : this.verticalScrollbarVisibility == ScrollRectEx.ScrollbarVisibility.AutoHideAndExpandViewport);
			this.m_HSliderHeight = (this.m_HorizontalScrollbarRect == null ? 0f : this.m_HorizontalScrollbarRect.rect.height);
			this.m_VSliderWidth = (this.m_VerticalScrollbarRect == null ? 0f : this.m_VerticalScrollbarRect.rect.width);
		}

		private void UpdatePrevData()
		{
			if (this.m_Content != null)
			{
				this.m_PrevPosition = this.m_Content.anchoredPosition;
			}
			else
			{
				this.m_PrevPosition = Vector2.zero;
			}
			this.m_PrevViewBounds = this.m_ViewBounds;
			this.m_PrevContentBounds = this.m_ContentBounds;
		}

		private void UpdateScrollbarLayout()
		{
			if (this.m_VSliderExpand && this.m_HorizontalScrollbar)
			{
				this.m_Tracker.Add(this, this.m_HorizontalScrollbarRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.SizeDeltaX);
				this.m_HorizontalScrollbarRect.anchorMin = new Vector2(0f, this.m_HorizontalScrollbarRect.anchorMin.y);
				this.m_HorizontalScrollbarRect.anchorMax = new Vector2(1f, this.m_HorizontalScrollbarRect.anchorMax.y);
				this.m_HorizontalScrollbarRect.anchoredPosition = new Vector2(0f, this.m_HorizontalScrollbarRect.anchoredPosition.y);
				if (!this.vScrollingNeeded)
				{
					this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(0f, this.m_HorizontalScrollbarRect.sizeDelta.y);
				}
				else
				{
					this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.m_HorizontalScrollbarRect.sizeDelta.y);
				}
			}
			if (this.m_HSliderExpand && this.m_VerticalScrollbar)
			{
				this.m_Tracker.Add(this, this.m_VerticalScrollbarRect, DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaY);
				this.m_VerticalScrollbarRect.anchorMin = new Vector2(this.m_VerticalScrollbarRect.anchorMin.x, 0f);
				this.m_VerticalScrollbarRect.anchorMax = new Vector2(this.m_VerticalScrollbarRect.anchorMax.x, 1f);
				this.m_VerticalScrollbarRect.anchoredPosition = new Vector2(this.m_VerticalScrollbarRect.anchoredPosition.x, 0f);
				if (this.hScrollingNeeded)
				{
					this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
					return;
				}
				this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, 0f);
			}
		}

		private void UpdateScrollbars(Vector2 offset)
		{
			if (this.m_HorizontalScrollbar)
			{
				if (this.m_ContentBounds.size.x <= 0f)
				{
					this.m_HorizontalScrollbar.size = 1f;
				}
				else
				{
					this.m_HorizontalScrollbar.size = Mathf.Clamp01((this.m_ViewBounds.size.x - Mathf.Abs(offset.x)) / this.m_ContentBounds.size.x);
				}
				this.m_HorizontalScrollbar.@value = this.horizontalNormalizedPosition;
			}
			if (this.m_VerticalScrollbar)
			{
				if (this.m_ContentBounds.size.y <= 0f)
				{
					this.m_VerticalScrollbar.size = 1f;
				}
				else
				{
					this.m_VerticalScrollbar.size = Mathf.Clamp01((this.m_ViewBounds.size.y - Mathf.Abs(offset.y)) / this.m_ContentBounds.size.y);
				}
				this.m_VerticalScrollbar.@value = this.verticalNormalizedPosition;
			}
		}

		private void UpdateScrollbarVisibility()
		{
			if (this.m_VerticalScrollbar && this.m_VerticalScrollbarVisibility != ScrollRectEx.ScrollbarVisibility.Permanent && this.m_VerticalScrollbar.gameObject.activeSelf != this.vScrollingNeeded)
			{
				this.m_VerticalScrollbar.gameObject.SetActive(this.vScrollingNeeded);
			}
			if (this.m_HorizontalScrollbar && this.m_HorizontalScrollbarVisibility != ScrollRectEx.ScrollbarVisibility.Permanent && this.m_HorizontalScrollbar.gameObject.activeSelf != this.hScrollingNeeded)
			{
				this.m_HorizontalScrollbar.gameObject.SetActive(this.hScrollingNeeded);
			}
		}

		public enum MovementType
		{
			Unrestricted,
			Elastic,
			Clamped
		}

		public enum ScrollbarVisibility
		{
			Permanent,
			AutoHide,
			AutoHideAndExpandViewport
		}

		[Serializable]
		public class ScrollRectEvent : UnityEvent<Vector2>
		{
			public ScrollRectEvent()
			{
			}
		}
	}
}