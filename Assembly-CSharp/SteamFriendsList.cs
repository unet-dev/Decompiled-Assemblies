using System;
using UnityEngine;
using UnityEngine.Events;

public class SteamFriendsList : MonoBehaviour
{
	public RectTransform targetPanel;

	public SteamUserButton userButton;

	public bool IncludeFriendsList = true;

	public bool IncludeRecentlySeen;

	public SteamFriendsList.onFriendSelectedEvent onFriendSelected;

	public SteamFriendsList()
	{
	}

	[Serializable]
	public class onFriendSelectedEvent : UnityEvent<ulong>
	{
		public onFriendSelectedEvent()
		{
		}
	}
}