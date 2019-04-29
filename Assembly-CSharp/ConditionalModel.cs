using System;
using UnityEngine;

public class ConditionalModel : PrefabAttribute
{
	public GameObjectRef prefab;

	public bool onClient = true;

	public bool onServer = true;

	[NonSerialized]
	public ModelConditionTest[] conditions;

	public ConditionalModel()
	{
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.conditions = base.GetComponentsInChildren<ModelConditionTest>(true);
	}

	protected override Type GetIndexedType()
	{
		return typeof(ConditionalModel);
	}

	public GameObject InstantiateSkin(BaseEntity parent)
	{
		if (!this.onServer && this.isServer)
		{
			return null;
		}
		GameObject gameObject = this.gameManager.CreatePrefab(this.prefab.resourcePath, parent.transform, false);
		if (gameObject)
		{
			gameObject.transform.localPosition = this.worldPosition;
			gameObject.transform.localRotation = this.worldRotation;
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	public bool RunTests(BaseEntity parent)
	{
		for (int i = 0; i < (int)this.conditions.Length; i++)
		{
			if (!this.conditions[i].DoTest(parent))
			{
				return false;
			}
		}
		return true;
	}
}