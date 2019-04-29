using Facepunch.Steamworks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ServerHistoryItem : MonoBehaviour
{
	private ServerList.Server serverInfo;

	public Text serverName;

	public Text players;

	public Text lastJoinDate;

	public uint order;

	public ServerHistoryItem()
	{
	}
}