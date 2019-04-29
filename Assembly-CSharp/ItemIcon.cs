using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : BaseMonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IDraggable, IInventoryChanged, IItemAmountChanged, IItemIconChanged
{
	public static Color defaultBackgroundColor;

	public static Color selectedBackgroundColor;

	public ItemContainerSource containerSource;

	public int slotOffset;

	[Range(0f, 64f)]
	public int slot;

	public bool setSlotFromSiblingIndex = true;

	public GameObject slots;

	public CanvasGroup iconContents;

	public Image iconImage;

	public Image underlayImage;

	public Text amountText;

	public Image hoverOutline;

	public Image cornerIcon;

	public Image lockedImage;

	public Image progressImage;

	public Image backgroundImage;

	public CanvasGroup conditionObject;

	public Image conditionFill;

	public Image maxConditionFill;

	public bool allowSelection = true;

	public bool allowDropping = true;

	[NonSerialized]
	public Item item;

	[NonSerialized]
	public bool invalidSlot;

	public SoundDefinition hoverSound;

	static ItemIcon()
	{
		ItemIcon.defaultBackgroundColor = new Color(0.968627453f, 0.921568632f, 0.882352948f, 0.03529412f);
		ItemIcon.selectedBackgroundColor = new Color(0.121568628f, 0.419607848f, 0.627451f, 0.784313738f);
	}

	public ItemIcon()
	{
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}
}