using Facepunch;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RelationshipManager : BaseEntity
{
	[ServerVar]
	public static int maxTeamSize;

	public static RelationshipManager _instance;

	public Dictionary<ulong, BasePlayer> cachedPlayers = new Dictionary<ulong, BasePlayer>();

	public Dictionary<ulong, RelationshipManager.PlayerTeam> playerTeams = new Dictionary<ulong, RelationshipManager.PlayerTeam>();

	private ulong lastTeamIndex = (long)1;

	public static RelationshipManager Instance
	{
		get
		{
			return RelationshipManager._instance;
		}
	}

	static RelationshipManager()
	{
		RelationshipManager.maxTeamSize = 8;
	}

	public RelationshipManager()
	{
	}

	[ServerUserVar]
	public static void acceptinvite(ConsoleSystem.Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (basePlayer.currentTeam != 0)
		{
			return;
		}
		ulong num = arg.GetULong(0, (ulong)0);
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.Instance.FindTeam(num);
		if (playerTeam == null)
		{
			basePlayer.ClearPendingInvite();
			return;
		}
		if (Interface.CallHook("OnTeamAcceptInvite", playerTeam, basePlayer) != null)
		{
			return;
		}
		playerTeam.AcceptInvite(basePlayer);
	}

	[ServerVar]
	public static void addtoteam(ConsoleSystem.Arg arg)
	{
		RaycastHit raycastHit;
		BasePlayer basePlayer = arg.Player();
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.Instance.FindTeam(basePlayer.currentTeam);
		if (playerTeam == null)
		{
			return;
		}
		if (playerTeam.GetLeader() == null)
		{
			return;
		}
		if (playerTeam.GetLeader() != basePlayer)
		{
			return;
		}
		if (Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out raycastHit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			BaseEntity entity = raycastHit.GetEntity();
			if (entity)
			{
				BasePlayer component = entity.GetComponent<BasePlayer>();
				if (component && component != basePlayer && !component.IsNpc)
				{
					playerTeam.AddPlayer(component);
				}
			}
		}
	}

	public RelationshipManager.PlayerTeam CreateTeam()
	{
		RelationshipManager.PlayerTeam playerTeam = Pool.Get<RelationshipManager.PlayerTeam>();
		playerTeam.teamID = this.lastTeamIndex;
		this.playerTeams.Add(this.lastTeamIndex, playerTeam);
		this.lastTeamIndex += (long)1;
		return playerTeam;
	}

	public void DisbandTeam(RelationshipManager.PlayerTeam teamToDisband)
	{
		if (Interface.CallHook("OnTeamDisband", teamToDisband) != null)
		{
			return;
		}
		this.playerTeams.Remove(teamToDisband.teamID);
		Interface.CallHook("OnTeamDisbanded", teamToDisband);
		Pool.Free<RelationshipManager.PlayerTeam>(ref teamToDisband);
	}

	[ServerVar]
	public static void fakeinvite(ConsoleSystem.Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		ulong num = arg.GetULong(0, (ulong)0);
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.Instance.FindTeam(num);
		if (playerTeam == null)
		{
			return;
		}
		if (basePlayer.currentTeam != 0)
		{
			Debug.Log("already in team");
		}
		playerTeam.SendInvite(basePlayer);
		Debug.Log("sent bot invite");
	}

	public static BasePlayer FindByID(ulong userID)
	{
		BasePlayer basePlayer = null;
		if (RelationshipManager.Instance.cachedPlayers.TryGetValue(userID, out basePlayer))
		{
			if (basePlayer != null)
			{
				return basePlayer;
			}
			RelationshipManager.Instance.cachedPlayers.Remove(userID);
		}
		BasePlayer basePlayer1 = BasePlayer.activePlayerList.Find((BasePlayer x) => x.userID == userID);
		if (!basePlayer1)
		{
			basePlayer1 = BasePlayer.sleepingPlayerList.Find((BasePlayer x) => x.userID == userID);
		}
		if (basePlayer1 != null)
		{
			RelationshipManager.Instance.cachedPlayers.Add(userID, basePlayer1);
		}
		return basePlayer1;
	}

	public RelationshipManager.PlayerTeam FindTeam(ulong TeamID)
	{
		if (!this.playerTeams.ContainsKey(TeamID))
		{
			return null;
		}
		return this.playerTeams[TeamID];
	}

	public static BasePlayer GetLookingAtPlayer(BasePlayer source)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(source.eyes.position, source.eyes.HeadForward(), out raycastHit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			BaseEntity entity = raycastHit.GetEntity();
			if (entity)
			{
				return entity.GetComponent<BasePlayer>();
			}
		}
		return null;
	}

	[ServerUserVar]
	public static void kickmember(ConsoleSystem.Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.Instance.FindTeam(basePlayer.currentTeam);
		if (playerTeam == null)
		{
			return;
		}
		if (playerTeam.GetLeader() != basePlayer)
		{
			return;
		}
		ulong num = arg.GetULong(0, (ulong)0);
		if (basePlayer.userID == num)
		{
			return;
		}
		if (Interface.CallHook("OnTeamKick", playerTeam, basePlayer) != null)
		{
			return;
		}
		playerTeam.RemovePlayer(num);
	}

	[ServerUserVar]
	public static void leaveteam(ConsoleSystem.Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (basePlayer.currentTeam == 0)
		{
			return;
		}
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.Instance.FindTeam(basePlayer.currentTeam);
		if (playerTeam != null)
		{
			if (Interface.CallHook("OnTeamLeave", playerTeam, basePlayer) != null)
			{
				return;
			}
			playerTeam.RemovePlayer(basePlayer.userID);
			basePlayer.ClearTeam();
		}
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.relationshipManager != null)
		{
			this.lastTeamIndex = info.msg.relationshipManager.lastTeamIndex;
			foreach (ProtoBuf.PlayerTeam playerTeam in info.msg.relationshipManager.teamList)
			{
				RelationshipManager.PlayerTeam nums = Pool.Get<RelationshipManager.PlayerTeam>();
				nums.teamLeader = playerTeam.teamLeader;
				nums.teamID = playerTeam.teamID;
				nums.teamName = playerTeam.teamName;
				nums.members = new List<ulong>();
				foreach (ProtoBuf.PlayerTeam.TeamMember member in playerTeam.members)
				{
					nums.members.Add(member.userID);
				}
				this.playerTeams[nums.teamID] = nums;
			}
		}
	}

	public void OnDestroy()
	{
		RelationshipManager._instance = null;
	}

	public void OnEnable()
	{
		if (base.isClient)
		{
			return;
		}
		if (RelationshipManager._instance == null)
		{
			RelationshipManager._instance = this;
			return;
		}
		Debug.LogError("Major fuckup! RelationshipManager spawned twice, Contact Developers!");
		UnityEngine.Object.Destroy(base.gameObject);
	}

	[ServerUserVar]
	public static void promote(ConsoleSystem.Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (basePlayer.currentTeam == 0)
		{
			return;
		}
		BasePlayer lookingAtPlayer = RelationshipManager.GetLookingAtPlayer(basePlayer);
		if (lookingAtPlayer == null)
		{
			return;
		}
		if (lookingAtPlayer.IsDead())
		{
			return;
		}
		if (lookingAtPlayer == basePlayer)
		{
			return;
		}
		if (lookingAtPlayer.currentTeam == basePlayer.currentTeam)
		{
			RelationshipManager.PlayerTeam item = RelationshipManager.Instance.playerTeams[basePlayer.currentTeam];
			if (item != null && item.teamLeader == basePlayer.userID)
			{
				if (Interface.CallHook("OnTeamPromote", item, lookingAtPlayer) != null)
				{
					return;
				}
				item.SetTeamLeader(lookingAtPlayer.userID);
			}
		}
	}

	[ServerUserVar]
	public static void rejectinvite(ConsoleSystem.Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (basePlayer.currentTeam != 0)
		{
			return;
		}
		ulong num = arg.GetULong(0, (ulong)0);
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.Instance.FindTeam(num);
		if (playerTeam == null)
		{
			basePlayer.ClearPendingInvite();
			return;
		}
		if (Interface.CallHook("OnTeamRejectInvite", basePlayer, playerTeam) != null)
		{
			return;
		}
		playerTeam.RejectInvite(basePlayer);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.relationshipManager = Pool.Get<ProtoBuf.RelationshipManager>();
		info.msg.relationshipManager.maxTeamSize = RelationshipManager.maxTeamSize;
		if (info.forDisk)
		{
			info.msg.relationshipManager.lastTeamIndex = this.lastTeamIndex;
			info.msg.relationshipManager.teamList = Pool.GetList<ProtoBuf.PlayerTeam>();
			foreach (KeyValuePair<ulong, RelationshipManager.PlayerTeam> playerTeam in this.playerTeams)
			{
				RelationshipManager.PlayerTeam value = playerTeam.Value;
				if (value == null)
				{
					continue;
				}
				ProtoBuf.PlayerTeam list = Pool.Get<ProtoBuf.PlayerTeam>();
				list.teamLeader = value.teamLeader;
				list.teamID = value.teamID;
				list.teamName = value.teamName;
				list.members = Pool.GetList<ProtoBuf.PlayerTeam.TeamMember>();
				foreach (ulong member in value.members)
				{
					ProtoBuf.PlayerTeam.TeamMember teamMember = Pool.Get<ProtoBuf.PlayerTeam.TeamMember>();
					BasePlayer basePlayer = RelationshipManager.FindByID(member);
					teamMember.displayName = (basePlayer != null ? basePlayer.displayName : "DEAD");
					teamMember.userID = member;
					list.members.Add(teamMember);
				}
				info.msg.relationshipManager.teamList.Add(list);
			}
		}
	}

	[ServerUserVar]
	public static void sendinvite(ConsoleSystem.Arg arg)
	{
		RaycastHit raycastHit;
		BasePlayer basePlayer = arg.Player();
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.Instance.FindTeam(basePlayer.currentTeam);
		if (playerTeam == null)
		{
			return;
		}
		if (playerTeam.GetLeader() == null)
		{
			return;
		}
		if (playerTeam.GetLeader() != basePlayer)
		{
			return;
		}
		if (Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out raycastHit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			BaseEntity entity = raycastHit.GetEntity();
			if (entity)
			{
				BasePlayer component = entity.GetComponent<BasePlayer>();
				if (component && component != basePlayer && !component.IsNpc && component.currentTeam == 0)
				{
					if (Interface.CallHook("OnTeamInvite", basePlayer, component) != null)
					{
						return;
					}
					playerTeam.SendInvite(component);
				}
			}
		}
	}

	[ServerVar]
	public static void sleeptoggle(ConsoleSystem.Arg arg)
	{
		RaycastHit raycastHit;
		BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out raycastHit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			BaseEntity entity = raycastHit.GetEntity();
			if (entity)
			{
				BasePlayer component = entity.GetComponent<BasePlayer>();
				if (component && component != basePlayer && !component.IsNpc)
				{
					if (component.IsSleeping())
					{
						component.EndSleeping();
						return;
					}
					component.StartSleeping();
				}
			}
		}
	}

	public static bool TeamsEnabled()
	{
		return RelationshipManager.maxTeamSize > 0;
	}

	[ServerUserVar]
	public static void trycreateteam(ConsoleSystem.Arg arg)
	{
		if (RelationshipManager.maxTeamSize == 0)
		{
			arg.ReplyWith("Teams are disabled on this server");
			return;
		}
		BasePlayer basePlayer = arg.Player();
		if (basePlayer.currentTeam != 0)
		{
			return;
		}
		if (Interface.CallHook("OnTeamCreate", basePlayer) != null)
		{
			return;
		}
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.Instance.CreateTeam();
		playerTeam.teamLeader = basePlayer.userID;
		playerTeam.AddPlayer(basePlayer);
	}

	public class PlayerTeam
	{
		public ulong teamID;

		public string teamName;

		public ulong teamLeader;

		public List<ulong> members;

		public List<ulong> invites;

		public PlayerTeam()
		{
		}

		public void AcceptInvite(BasePlayer player)
		{
			if (!this.invites.Contains(player.userID))
			{
				return;
			}
			this.invites.Remove(player.userID);
			this.AddPlayer(player);
			player.ClearPendingInvite();
		}

		public bool AddPlayer(BasePlayer player)
		{
			ulong num = player.userID;
			if (this.members.Contains(num))
			{
				return false;
			}
			if (player.currentTeam != 0)
			{
				return false;
			}
			if (this.members.Count >= RelationshipManager.maxTeamSize)
			{
				return false;
			}
			player.currentTeam = this.teamID;
			this.members.Add(num);
			this.MarkDirty();
			player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return true;
		}

		public void Cleanup()
		{
			for (int i = this.members.Count - 1; i >= 0; i--)
			{
				ulong item = this.members[i];
				if (RelationshipManager.FindByID(item) == null)
				{
					this.members.Remove(item);
				}
			}
		}

		public void Disband()
		{
			RelationshipManager.Instance.DisbandTeam(this);
		}

		public BasePlayer GetLeader()
		{
			return RelationshipManager.FindByID(this.teamLeader);
		}

		public BasePlayer GetMember(ulong userID)
		{
			return RelationshipManager.FindByID(userID);
		}

		public BasePlayer GetMember(int index)
		{
			if (index >= this.members.Count)
			{
				return null;
			}
			return this.GetMember(this.members[index]);
		}

		public void MarkDirty()
		{
			foreach (ulong member in this.members)
			{
				BasePlayer basePlayer = RelationshipManager.FindByID(member);
				if (basePlayer == null)
				{
					continue;
				}
				basePlayer.UpdateTeam(this.teamID);
			}
		}

		public void RejectInvite(BasePlayer player)
		{
			player.ClearPendingInvite();
			this.invites.Remove(player.userID);
		}

		public bool RemovePlayer(ulong playerID)
		{
			if (!this.members.Contains(playerID))
			{
				return false;
			}
			this.members.Remove(playerID);
			BasePlayer basePlayer = RelationshipManager.FindByID(playerID);
			if (basePlayer != null)
			{
				basePlayer.ClearTeam();
			}
			if (this.teamLeader == playerID)
			{
				if (this.members.Count <= 0)
				{
					this.Disband();
				}
				else
				{
					this.SetTeamLeader(this.members[0]);
				}
			}
			this.MarkDirty();
			return true;
		}

		public void SendInvite(BasePlayer player)
		{
			if (this.invites.Count > 8)
			{
				this.invites.RemoveRange(0, 1);
			}
			BasePlayer basePlayer = RelationshipManager.FindByID(this.teamLeader);
			if (basePlayer == null)
			{
				return;
			}
			this.invites.Add(player.userID);
			player.ClientRPCPlayer<string, ulong>(null, player, "CLIENT_PendingInvite", basePlayer.displayName, this.teamID);
		}

		public void SetTeamLeader(ulong newTeamLeader)
		{
			this.teamLeader = newTeamLeader;
			this.MarkDirty();
		}
	}
}