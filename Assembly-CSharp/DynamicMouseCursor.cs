using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicMouseCursor : MonoBehaviour
{
	public Texture2D RegularCursor;

	public Vector2 RegularCursorPos;

	public Texture2D HoverCursor;

	public Vector2 HoverCursorPos;

	private Texture2D current;

	private PointerEventData pointer;

	private List<RaycastResult> results = new List<RaycastResult>();

	public DynamicMouseCursor()
	{
	}

	private GameObject CurrentlyHoveredItem()
	{
		GameObject current;
		if (this.pointer == null)
		{
			this.pointer = new PointerEventData(EventSystem.current);
		}
		this.pointer.position = Input.mousePosition;
		EventSystem.current.RaycastAll(this.pointer, this.results);
		List<RaycastResult>.Enumerator enumerator = this.results.GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				current = enumerator.Current.gameObject;
			}
			else
			{
				return null;
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return current;
	}

	private void LateUpdate()
	{
		GameObject gameObject = this.CurrentlyHoveredItem();
		if (gameObject != null && gameObject.GetComponentInParent<ISubmitHandler>() != null)
		{
			this.UpdateCursor(this.HoverCursor, this.HoverCursorPos);
			return;
		}
		if (gameObject != null && gameObject.GetComponentInParent<IPointerClickHandler>() != null)
		{
			this.UpdateCursor(this.HoverCursor, this.HoverCursorPos);
			return;
		}
		this.UpdateCursor(this.RegularCursor, this.RegularCursorPos);
	}

	private void UpdateCursor(Texture2D cursor, Vector2 offs)
	{
		if (this.current == cursor)
		{
			return;
		}
		this.current = cursor;
		Cursor.SetCursor(cursor, offs, CursorMode.Auto);
	}
}