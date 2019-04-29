using Facepunch.Steamworks;
using System;
using UnityEngine;

public class ServerHistory : MonoBehaviour
{
	public ServerHistoryItem prefab;

	public GameObject panelList;

	internal ServerList.Request Request;

	public ServerHistory()
	{
	}
}