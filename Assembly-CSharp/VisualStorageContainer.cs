using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VisualStorageContainer : LootContainer
{
	public VisualStorageContainerNode[] displayNodes;

	public VisualStorageContainer.DisplayModel[] displayModels;

	public Transform nodeParent;

	public GameObject defaultDisplayModel;

	public VisualStorageContainer()
	{
	}

	public void ClearRigidBodies()
	{
		if (this.displayModels == null)
		{
			return;
		}
		VisualStorageContainer.DisplayModel[] displayModelArray = this.displayModels;
		for (int i = 0; i < (int)displayModelArray.Length; i++)
		{
			VisualStorageContainer.DisplayModel displayModel = displayModelArray[i];
			if (displayModel != null)
			{
				UnityEngine.Object.Destroy(displayModel.displayModel.GetComponentInChildren<Rigidbody>());
			}
		}
	}

	public void ItemUpdateComplete()
	{
		this.ClearRigidBodies();
		this.SetItemsVisible(true);
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
	}

	public override void PopulateLoot()
	{
		base.PopulateLoot();
		for (int i = 0; i < this.inventorySlots; i++)
		{
			Item slot = this.inventory.GetSlot(i);
			if (slot != null)
			{
				DroppedItem component = slot.Drop(this.displayNodes[i].transform.position + new Vector3(0f, 0.25f, 0f), Vector3.zero, this.displayNodes[i].transform.rotation).GetComponent<DroppedItem>();
				if (component)
				{
					base.ReceiveCollisionMessages(false);
					base.CancelInvoke(new Action(component.IdleDestroy));
					Rigidbody componentInChildren = component.GetComponentInChildren<Rigidbody>();
					if (componentInChildren)
					{
						componentInChildren.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
					}
				}
			}
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
	}

	public void SetItemsVisible(bool vis)
	{
		if (this.displayModels == null)
		{
			return;
		}
		VisualStorageContainer.DisplayModel[] displayModelArray = this.displayModels;
		for (int i = 0; i < (int)displayModelArray.Length; i++)
		{
			VisualStorageContainer.DisplayModel displayModel = displayModelArray[i];
			if (displayModel != null)
			{
				LODGroup componentInChildren = displayModel.displayModel.GetComponentInChildren<LODGroup>();
				if (!componentInChildren)
				{
					Debug.Log(string.Concat("VisualStorageContainer item missing LODGroup", displayModel.displayModel.gameObject.name));
				}
				else
				{
					componentInChildren.localReferencePoint = (vis ? Vector3.zero : new Vector3(10000f, 10000f, 10000f));
				}
			}
		}
	}

	public void UpdateVisibleItems(ProtoBuf.ItemContainer msg)
	{
		for (int i = 0; i < (int)this.displayModels.Length; i++)
		{
			VisualStorageContainer.DisplayModel displayModel = this.displayModels[i];
			if (displayModel != null)
			{
				UnityEngine.Object.Destroy(displayModel.displayModel);
				this.displayModels[i] = null;
			}
		}
		if (msg == null)
		{
			return;
		}
		foreach (ProtoBuf.Item content in msg.contents)
		{
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(content.itemid);
			GameObject vector3 = null;
			vector3 = (itemDefinition.worldModelPrefab == null || !itemDefinition.worldModelPrefab.isValid ? UnityEngine.Object.Instantiate<GameObject>(this.defaultDisplayModel) : itemDefinition.worldModelPrefab.Instantiate(null));
			if (!vector3)
			{
				continue;
			}
			vector3.transform.position = this.displayNodes[content.slot].transform.position + new Vector3(0f, 0.25f, 0f);
			vector3.transform.rotation = this.displayNodes[content.slot].transform.rotation;
			Rigidbody rigidbody = vector3.AddComponent<Rigidbody>();
			rigidbody.mass = 1f;
			rigidbody.drag = 0.1f;
			rigidbody.angularDrag = 0.1f;
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
			this.displayModels[content.slot].displayModel = vector3;
			this.displayModels[content.slot].slot = content.slot;
			this.displayModels[content.slot].def = itemDefinition;
			vector3.SetActive(true);
		}
		this.SetItemsVisible(false);
		base.CancelInvoke(new Action(this.ItemUpdateComplete));
		base.Invoke(new Action(this.ItemUpdateComplete), 1f);
	}

	[Serializable]
	public class DisplayModel
	{
		public GameObject displayModel;

		public ItemDefinition def;

		public int slot;

		public DisplayModel()
		{
		}
	}
}