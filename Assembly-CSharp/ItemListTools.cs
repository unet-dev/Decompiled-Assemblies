using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemListTools : MonoBehaviour
{
	public GameObject categoryButton;

	public GameObject itemButton;

	internal Button lastCategory;

	public ItemListTools()
	{
	}

	public void OnPanelOpened()
	{
		this.Refresh();
	}

	private void RebuildCategories()
	{
		for (int i = 0; i < this.categoryButton.transform.parent.childCount; i++)
		{
			Transform child = this.categoryButton.transform.parent.GetChild(i);
			if (child != this.categoryButton.transform)
			{
				GameManager.Destroy(child.gameObject, 0f);
			}
		}
		this.categoryButton.SetActive(true);
		foreach (IGrouping<ItemCategory, ItemDefinition> itemCategories in 
			from x in ItemManager.GetItemDefinitions()
			group x by x.category into x
			orderby x.First<ItemDefinition>().category
			select x)
		{
			GameObject str = UnityEngine.Object.Instantiate<GameObject>(this.categoryButton);
			str.transform.SetParent(this.categoryButton.transform.parent, false);
			str.GetComponentInChildren<TextMeshProUGUI>().text = itemCategories.First<ItemDefinition>().category.ToString();
			Button componentInChildren = str.GetComponentInChildren<Button>();
			ItemDefinition[] array = itemCategories.ToArray<ItemDefinition>();
			componentInChildren.onClick.AddListener(() => {
				if (this.lastCategory)
				{
					this.lastCategory.interactable = true;
				}
				this.lastCategory = componentInChildren;
				this.lastCategory.interactable = false;
				this.SwitchItemCategory(array);
			});
			if (this.lastCategory != null)
			{
				continue;
			}
			this.lastCategory = componentInChildren;
			this.lastCategory.interactable = false;
			this.SwitchItemCategory(array);
		}
		this.categoryButton.SetActive(false);
	}

	public void Refresh()
	{
		this.RebuildCategories();
	}

	private void SwitchItemCategory(ItemDefinition[] defs)
	{
		for (int i = 0; i < this.itemButton.transform.parent.childCount; i++)
		{
			Transform child = this.itemButton.transform.parent.GetChild(i);
			if (child != this.itemButton.transform)
			{
				GameManager.Destroy(child.gameObject, 0f);
			}
		}
		this.itemButton.SetActive(true);
		foreach (ItemDefinition itemDefinition in 
			from x in (IEnumerable<ItemDefinition>)defs
			orderby x.displayName.translated
			select x)
		{
			if (itemDefinition.hidden)
			{
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.itemButton);
			gameObject.transform.SetParent(this.itemButton.transform.parent, false);
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = itemDefinition.displayName.translated;
			gameObject.GetComponentInChildren<ItemButtonTools>().itemDef = itemDefinition;
			gameObject.GetComponentInChildren<ItemButtonTools>().image.sprite = itemDefinition.iconSprite;
		}
		this.itemButton.SetActive(false);
	}
}