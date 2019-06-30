using System;
using TMPro;
using UnityEngine;

public class TeamUI : MonoBehaviour
{
	public static Translate.Phrase invitePhrase;

	public RectTransform MemberPanel;

	public GameObject memberEntryPrefab;

	public TeamMemberElement[] elements;

	public GameObject NoTeamPanel;

	public GameObject TeamPanel;

	public GameObject LeaveTeamButton;

	public GameObject InviteAcceptPanel;

	public TextMeshProUGUI inviteText;

	public static bool dirty;

	[NonSerialized]
	public static ulong pendingTeamID;

	[NonSerialized]
	public static string pendingTeamLeaderName;

	static TeamUI()
	{
		TeamUI.invitePhrase = new Translate.Phrase("team.invited", "{0} has invited you to join a team");
		TeamUI.dirty = true;
	}

	public TeamUI()
	{
	}
}