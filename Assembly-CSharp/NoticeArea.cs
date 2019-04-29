using System;
using UnityEngine;
using UnityEngine.UI;

public class NoticeArea : SingletonComponent<NoticeArea>
{
	public GameObject itemPickupPrefab;

	public GameObject itemDroppedPrefab;

	public NoticeArea()
	{
	}

	public static void ItemPickUp(ItemDefinition def, int amount, string nameOverride)
	{
		if (SingletonComponent<NoticeArea>.Instance == null)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((amount > 0 ? SingletonComponent<NoticeArea>.Instance.itemPickupPrefab : SingletonComponent<NoticeArea>.Instance.itemDroppedPrefab));
		if (gameObject == null)
		{
			return;
		}
		gameObject.transform.SetParent(SingletonComponent<NoticeArea>.Instance.transform, false);
		ItemPickupNotice component = gameObject.GetComponent<ItemPickupNotice>();
		if (component == null)
		{
			return;
		}
		component.itemInfo = def;
		component.amount = amount;
		if (!string.IsNullOrEmpty(nameOverride))
		{
			component.Text.text = nameOverride;
		}
	}
}