using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropMe : MonoBehaviour, IDropHandler, IEventSystemHandler
{
	public string[] droppableTypes;

	public DropMe()
	{
	}

	public virtual void OnDrop(PointerEventData eventData)
	{
	}
}