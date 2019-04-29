using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupNotice : MonoBehaviour
{
	public GameObject objectDeleteOnFinish;

	public UnityEngine.UI.Text Text;

	public UnityEngine.UI.Text Amount;

	public int amount
	{
		set
		{
			this.Amount.text = (value > 0 ? value.ToString("+0") : value.ToString("0"));
		}
	}

	public ItemDefinition itemInfo
	{
		set
		{
			this.Text.text = value.displayName.translated;
		}
	}

	public ItemPickupNotice()
	{
	}

	public void PopupNoticeEnd()
	{
		GameManager.Destroy(this.objectDeleteOnFinish, 0f);
	}
}