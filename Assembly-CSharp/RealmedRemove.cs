using System;
using System.Linq;
using UnityEngine;

public class RealmedRemove : MonoBehaviour, IPrefabPreProcess
{
	public GameObject[] removedFromClient;

	public Component[] removedComponentFromClient;

	public GameObject[] removedFromServer;

	public Component[] removedComponentFromServer;

	public Component[] doNotRemoveFromServer;

	public Component[] doNotRemoveFromClient;

	public RealmedRemove()
	{
	}

	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		GameObject[] gameObjectArray;
		int i;
		Component[] componentArray;
		if (clientside)
		{
			gameObjectArray = this.removedFromClient;
			for (i = 0; i < (int)gameObjectArray.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(gameObjectArray[i], true);
			}
			componentArray = this.removedComponentFromClient;
			for (i = 0; i < (int)componentArray.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(componentArray[i], true);
			}
		}
		if (serverside)
		{
			gameObjectArray = this.removedFromServer;
			for (i = 0; i < (int)gameObjectArray.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(gameObjectArray[i], true);
			}
			componentArray = this.removedComponentFromServer;
			for (i = 0; i < (int)componentArray.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(componentArray[i], true);
			}
		}
		if (!bundling)
		{
			process.RemoveComponent(this);
		}
	}

	public bool ShouldDelete(Component comp, bool client, bool server)
	{
		if (client && this.doNotRemoveFromClient != null && this.doNotRemoveFromClient.Contains<Component>(comp))
		{
			return false;
		}
		if (server && this.doNotRemoveFromServer != null && this.doNotRemoveFromServer.Contains<Component>(comp))
		{
			return false;
		}
		return true;
	}
}