using Oxide.Core;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Libraries
{
	public class Permission : Library
	{
		private readonly Dictionary<Plugin, HashSet<string>> permset;

		private Dictionary<string, UserData> userdata;

		private Dictionary<string, GroupData> groupdata;

		private Func<string, bool> validate;

		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		public bool IsLoaded
		{
			get;
			private set;
		}

		public Permission()
		{
			this.permset = new Dictionary<Plugin, HashSet<string>>();
			this.LoadFromDatafile();
		}

		[LibraryFunction("AddUserGroup")]
		public void AddUserGroup(string id, string name)
		{
			if (!this.GroupExists(name))
			{
				return;
			}
			if (!this.GetUserData(id).Groups.Add(name.ToLower()))
			{
				return;
			}
			Interface.Call("OnUserGroupAdded", new object[] { id, name });
		}

		public void CleanUp()
		{
			if (!this.IsLoaded || this.validate == null)
			{
				return;
			}
			string[] array = (
				from k in this.userdata.Keys
				where !this.validate(k)
				select k).ToArray<string>();
			if (array.Length == 0)
			{
				return;
			}
			string[] strArrays = array;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				this.userdata.Remove(str);
			}
		}

		[LibraryFunction("CreateGroup")]
		public bool CreateGroup(string group, string title, int rank)
		{
			if (this.GroupExists(group) || string.IsNullOrEmpty(group))
			{
				return false;
			}
			GroupData groupDatum = new GroupData()
			{
				Title = title,
				Rank = rank
			};
			group = group.ToLower();
			this.groupdata.Add(group, groupDatum);
			Interface.CallHook("OnGroupCreated", group, title, rank);
			return true;
		}

		[LibraryFunction("Export")]
		public void Export(string prefix = "auth")
		{
			if (!this.IsLoaded)
			{
				return;
			}
			Interface.Oxide.DataFileSystem.WriteObject<Dictionary<string, GroupData>>(string.Concat(prefix, ".groups"), this.groupdata, false);
			Interface.Oxide.DataFileSystem.WriteObject<Dictionary<string, UserData>>(string.Concat(prefix, ".users"), this.userdata, false);
		}

		[LibraryFunction("GetGroupParent")]
		public string GetGroupParent(string group)
		{
			GroupData groupDatum;
			if (!this.GroupExists(group))
			{
				return string.Empty;
			}
			group = group.ToLower();
			if (!this.groupdata.TryGetValue(group, out groupDatum))
			{
				return string.Empty;
			}
			return groupDatum.ParentGroup;
		}

		[LibraryFunction("GetGroupPermissions")]
		public string[] GetGroupPermissions(string name, bool parents = false)
		{
			GroupData groupDatum;
			if (!this.GroupExists(name))
			{
				return new string[0];
			}
			if (!this.groupdata.TryGetValue(name.ToLower(), out groupDatum))
			{
				return new string[0];
			}
			List<string> list = groupDatum.Perms.ToList<string>();
			if (parents)
			{
				list.AddRange(this.GetGroupPermissions(groupDatum.ParentGroup, false));
			}
			return (new HashSet<string>(list)).ToArray<string>();
		}

		[LibraryFunction("GetGroupRank")]
		public int GetGroupRank(string group)
		{
			GroupData groupDatum;
			if (!this.GroupExists(group))
			{
				return 0;
			}
			if (!this.groupdata.TryGetValue(group.ToLower(), out groupDatum))
			{
				return 0;
			}
			return groupDatum.Rank;
		}

		[LibraryFunction("GetGroups")]
		public string[] GetGroups()
		{
			return this.groupdata.Keys.ToArray<string>();
		}

		[LibraryFunction("GetGroupTitle")]
		public string GetGroupTitle(string group)
		{
			GroupData groupDatum;
			if (!this.GroupExists(group))
			{
				return string.Empty;
			}
			if (!this.groupdata.TryGetValue(group.ToLower(), out groupDatum))
			{
				return string.Empty;
			}
			return groupDatum.Title;
		}

		[LibraryFunction("GetPermissionGroups")]
		public string[] GetPermissionGroups(string perm)
		{
			if (string.IsNullOrEmpty(perm))
			{
				return new string[0];
			}
			perm = perm.ToLower();
			HashSet<string> strs = new HashSet<string>();
			foreach (KeyValuePair<string, GroupData> groupdatum in this.groupdata)
			{
				if (!groupdatum.Value.Perms.Contains(perm))
				{
					continue;
				}
				strs.Add(groupdatum.Key);
			}
			return strs.ToArray<string>();
		}

		[LibraryFunction("GetPermissions")]
		public string[] GetPermissions()
		{
			return (new HashSet<string>(this.permset.Values.SelectMany<HashSet<string>, string>((HashSet<string> v) => v))).ToArray<string>();
		}

		[LibraryFunction("GetPermissionUsers")]
		public string[] GetPermissionUsers(string perm)
		{
			if (string.IsNullOrEmpty(perm))
			{
				return new string[0];
			}
			perm = perm.ToLower();
			HashSet<string> strs = new HashSet<string>();
			foreach (KeyValuePair<string, UserData> userdatum in this.userdata)
			{
				if (!userdatum.Value.Perms.Contains(perm))
				{
					continue;
				}
				strs.Add(string.Concat(userdatum.Key, "(", userdatum.Value.LastSeenNickname, ")"));
			}
			return strs.ToArray<string>();
		}

		public UserData GetUserData(string id)
		{
			UserData userDatum;
			if (!this.userdata.TryGetValue(id, out userDatum))
			{
				Dictionary<string, UserData> strs = this.userdata;
				UserData userDatum1 = new UserData();
				userDatum = userDatum1;
				strs.Add(id, userDatum1);
			}
			return userDatum;
		}

		[LibraryFunction("GetUserGroups")]
		public string[] GetUserGroups(string id)
		{
			return this.GetUserData(id).Groups.ToArray<string>();
		}

		[LibraryFunction("GetUserPermissions")]
		public string[] GetUserPermissions(string id)
		{
			UserData userData = this.GetUserData(id);
			List<string> list = userData.Perms.ToList<string>();
			foreach (string group in userData.Groups)
			{
				list.AddRange(this.GetGroupPermissions(group, false));
			}
			return (new HashSet<string>(list)).ToArray<string>();
		}

		[LibraryFunction("GetUsersInGroup")]
		public string[] GetUsersInGroup(string group)
		{
			string lower = group;
			if (!this.GroupExists(lower))
			{
				return new string[0];
			}
			lower = lower.ToLower();
			return (
				from u in this.userdata
				where u.Value.Groups.Contains(lower)
				select string.Concat(u.Key, " (", u.Value.LastSeenNickname, ")")).ToArray<string>();
		}

		[LibraryFunction("GrantGroupPermission")]
		public void GrantGroupPermission(string name, string perm, Plugin owner)
		{
			HashSet<string> strs;
			GroupData groupDatum;
			string lower = perm;
			if (!this.PermissionExists(lower, owner) || !this.GroupExists(name))
			{
				return;
			}
			if (!this.groupdata.TryGetValue(name.ToLower(), out groupDatum))
			{
				return;
			}
			lower = lower.ToLower();
			if (!lower.EndsWith("*"))
			{
				if (!groupDatum.Perms.Add(lower))
				{
					return;
				}
				Interface.Call("OnGroupPermissionGranted", new object[] { name, lower });
				return;
			}
			if (owner == null)
			{
				strs = new HashSet<string>(this.permset.Values.SelectMany<HashSet<string>, string>((HashSet<string> v) => v));
			}
			else if (!this.permset.TryGetValue(owner, out strs))
			{
				return;
			}
			if (lower.Equals("*"))
			{
				strs.Aggregate<string, bool>(false, (bool c, string s) => c | groupDatum.Perms.Add(s));
				return;
			}
			lower = lower.TrimEnd(new char[] { '*' }).ToLower();
			(
				from s in strs
				where s.StartsWith(lower)
				select s).Aggregate<string, bool>(false, (bool c, string s) => c | groupDatum.Perms.Add(s));
		}

		[LibraryFunction("GrantUserPermission")]
		public void GrantUserPermission(string id, string perm, Plugin owner)
		{
			HashSet<string> strs;
			string lower = perm;
			if (!this.PermissionExists(lower, owner))
			{
				return;
			}
			UserData userData = this.GetUserData(id);
			lower = lower.ToLower();
			if (!lower.EndsWith("*"))
			{
				if (!userData.Perms.Add(lower))
				{
					return;
				}
				Interface.Call("OnUserPermissionGranted", new object[] { id, lower });
				return;
			}
			if (owner == null)
			{
				strs = new HashSet<string>(this.permset.Values.SelectMany<HashSet<string>, string>((HashSet<string> v) => v));
			}
			else if (!this.permset.TryGetValue(owner, out strs))
			{
				return;
			}
			if (lower.Equals("*"))
			{
				strs.Aggregate<string, bool>(false, (bool c, string s) => c | userData.Perms.Add(s));
				return;
			}
			lower = lower.TrimEnd(new char[] { '*' });
			(
				from s in strs
				where s.StartsWith(lower)
				select s).Aggregate<string, bool>(false, (bool c, string s) => c | userData.Perms.Add(s));
		}

		[LibraryFunction("GroupExists")]
		public bool GroupExists(string group)
		{
			if (string.IsNullOrEmpty(group))
			{
				return false;
			}
			if (group.Equals("*"))
			{
				return true;
			}
			return this.groupdata.ContainsKey(group.ToLower());
		}

		[LibraryFunction("GroupHasPermission")]
		public bool GroupHasPermission(string name, string perm)
		{
			GroupData groupDatum;
			if (!this.GroupExists(name) || string.IsNullOrEmpty(perm))
			{
				return false;
			}
			if (!this.groupdata.TryGetValue(name.ToLower(), out groupDatum))
			{
				return false;
			}
			if (groupDatum.Perms.Contains(perm.ToLower()))
			{
				return true;
			}
			return this.GroupHasPermission(groupDatum.ParentGroup, perm);
		}

		[LibraryFunction("GroupsHavePermission")]
		public bool GroupsHavePermission(HashSet<string> groups, string perm)
		{
			return groups.Any<string>((string group) => this.GroupHasPermission(group, perm));
		}

		private bool HasCircularParent(string group, string parent)
		{
			GroupData groupDatum;
			if (!this.groupdata.TryGetValue(parent, out groupDatum))
			{
				return false;
			}
			HashSet<string> strs = new HashSet<string>();
			strs.Add(group);
			strs.Add(parent);
			HashSet<string> strs1 = strs;
			while (!string.IsNullOrEmpty(groupDatum.ParentGroup))
			{
				if (!strs1.Add(groupDatum.ParentGroup))
				{
					return true;
				}
				if (this.groupdata.TryGetValue(groupDatum.ParentGroup, out groupDatum))
				{
					continue;
				}
				return false;
			}
			return false;
		}

		private void LoadFromDatafile()
		{
			Utility.DatafileToProto<Dictionary<string, UserData>>("oxide.users", true);
			Utility.DatafileToProto<Dictionary<string, GroupData>>("oxide.groups", true);
			this.userdata = ProtoStorage.Load<Dictionary<string, UserData>>(new string[] { "oxide.users" }) ?? new Dictionary<string, UserData>();
			this.groupdata = ProtoStorage.Load<Dictionary<string, GroupData>>(new string[] { "oxide.groups" }) ?? new Dictionary<string, GroupData>();
			foreach (KeyValuePair<string, GroupData> groupdatum in this.groupdata)
			{
				if (string.IsNullOrEmpty(groupdatum.Value.ParentGroup) || !this.HasCircularParent(groupdatum.Key, groupdatum.Value.ParentGroup))
				{
					continue;
				}
				Interface.Oxide.LogWarning("Detected circular parent group for '{0}'! Removing parent '{1}'", new object[] { groupdatum.Key, groupdatum.Value.ParentGroup });
				groupdatum.Value.ParentGroup = null;
			}
			this.IsLoaded = true;
		}

		public void MigrateGroup(string oldGroup, string newGroup)
		{
			if (!this.IsLoaded)
			{
				return;
			}
			if (this.GroupExists(oldGroup))
			{
				string fileDataPath = ProtoStorage.GetFileDataPath("oxide.groups.data");
				File.Copy(fileDataPath, string.Concat(fileDataPath, ".old"), true);
				string[] groupPermissions = this.GetGroupPermissions(oldGroup, false);
				for (int i = 0; i < (int)groupPermissions.Length; i++)
				{
					this.GrantGroupPermission(newGroup, groupPermissions[i], null);
				}
				if (this.GetUsersInGroup(oldGroup).Length == 0)
				{
					this.RemoveGroup(oldGroup);
				}
			}
		}

		private void owner_OnRemovedFromManager(Plugin sender, PluginManager manager)
		{
			this.permset.Remove(sender);
		}

		[LibraryFunction("PermissionExists")]
		public bool PermissionExists(string name, Plugin owner = null)
		{
			HashSet<string> strs;
			string lower = name;
			if (string.IsNullOrEmpty(lower))
			{
				return false;
			}
			lower = lower.ToLower();
			if (owner != null)
			{
				if (!this.permset.TryGetValue(owner, out strs))
				{
					return false;
				}
				if (strs.Count > 0)
				{
					if (lower.Equals("*"))
					{
						return true;
					}
					if (lower.EndsWith("*"))
					{
						lower = lower.TrimEnd(new char[] { '*' });
						return strs.Any<string>((string p) => p.StartsWith(lower));
					}
				}
				return strs.Contains(lower);
			}
			if (this.permset.Count > 0)
			{
				if (lower.Equals("*"))
				{
					return true;
				}
				if (lower.EndsWith("*"))
				{
					lower = lower.TrimEnd(new char[] { '*' });
					return this.permset.Values.SelectMany<HashSet<string>, string>((HashSet<string> v) => v).Any<string>((string p) => p.StartsWith(lower));
				}
			}
			return this.permset.Values.Any<HashSet<string>>((HashSet<string> v) => v.Contains(lower));
		}

		[LibraryFunction("RegisterPermission")]
		public void RegisterPermission(string name, Plugin owner)
		{
			HashSet<string> strs;
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			name = name.ToLower();
			if (this.PermissionExists(name, null))
			{
				Interface.Oxide.LogWarning("Duplicate permission registered '{0}' (by plugin '{1}')", new object[] { name, owner.Title });
				return;
			}
			if (!this.permset.TryGetValue(owner, out strs))
			{
				strs = new HashSet<string>();
				this.permset.Add(owner, strs);
				owner.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.owner_OnRemovedFromManager));
			}
			strs.Add(name);
			Interface.CallHook("OnPermissionRegistered", name, owner);
			string str = string.Concat(owner.Name.ToLower(), ".");
			if (!name.StartsWith(str) && !owner.IsCorePlugin)
			{
				Interface.Oxide.LogWarning("Missing plugin name prefix '{0}' for permission '{1}' (by plugin '{2}')", new object[] { str, name, owner.Title });
			}
		}

		public void RegisterValidate(Func<string, bool> val)
		{
			this.validate = val;
		}

		[LibraryFunction("RemoveGroup")]
		public bool RemoveGroup(string group)
		{
			string lower = group;
			if (!this.GroupExists(lower))
			{
				return false;
			}
			lower = lower.ToLower();
			bool flag = this.groupdata.Remove(lower);
			if (this.userdata.Values.Aggregate<UserData, bool>(false, (bool current, UserData userData) => current | userData.Groups.Remove(lower)))
			{
				this.SaveUsers();
			}
			if (flag)
			{
				Interface.CallHook("OnGroupDeleted", lower);
			}
			return true;
		}

		[LibraryFunction("RemoveUserGroup")]
		public void RemoveUserGroup(string id, string name)
		{
			if (!this.GroupExists(name))
			{
				return;
			}
			UserData userData = this.GetUserData(id);
			if (name.Equals("*"))
			{
				if (userData.Groups.Count <= 0)
				{
					return;
				}
				userData.Groups.Clear();
				return;
			}
			if (!userData.Groups.Remove(name.ToLower()))
			{
				return;
			}
			Interface.Call("OnUserGroupRemoved", new object[] { id, name });
		}

		[LibraryFunction("RevokeGroupPermission")]
		public void RevokeGroupPermission(string name, string perm)
		{
			GroupData groupDatum;
			string lower = perm;
			if (!this.GroupExists(name) || string.IsNullOrEmpty(lower))
			{
				return;
			}
			if (!this.groupdata.TryGetValue(name.ToLower(), out groupDatum))
			{
				return;
			}
			lower = lower.ToLower();
			if (!lower.EndsWith("*"))
			{
				if (!groupDatum.Perms.Remove(lower))
				{
					return;
				}
				Interface.Call("OnGroupPermissionRevoked", new object[] { name, lower });
				return;
			}
			if (lower.Equals("*"))
			{
				if (groupDatum.Perms.Count <= 0)
				{
					return;
				}
				groupDatum.Perms.Clear();
				return;
			}
			lower = lower.TrimEnd(new char[] { '*' }).ToLower();
			groupDatum.Perms.RemoveWhere((string s) => s.StartsWith(lower));
		}

		[LibraryFunction("RevokeUserPermission")]
		public void RevokeUserPermission(string id, string perm)
		{
			string lower = perm;
			if (string.IsNullOrEmpty(lower))
			{
				return;
			}
			UserData userData = this.GetUserData(id);
			lower = lower.ToLower();
			if (!lower.EndsWith("*"))
			{
				if (!userData.Perms.Remove(lower))
				{
					return;
				}
				Interface.Call("OnUserPermissionRevoked", new object[] { id, lower });
				return;
			}
			if (lower.Equals("*"))
			{
				if (userData.Perms.Count <= 0)
				{
					return;
				}
				userData.Perms.Clear();
				return;
			}
			lower = lower.TrimEnd(new char[] { '*' });
			userData.Perms.RemoveWhere((string s) => s.StartsWith(lower));
		}

		public void SaveData()
		{
			this.SaveUsers();
			this.SaveGroups();
		}

		public void SaveGroups()
		{
			ProtoStorage.Save<Dictionary<string, GroupData>>(this.groupdata, new string[] { "oxide.groups" });
		}

		public void SaveUsers()
		{
			ProtoStorage.Save<Dictionary<string, UserData>>(this.userdata, new string[] { "oxide.users" });
		}

		[LibraryFunction("SetGroupParent")]
		public bool SetGroupParent(string group, string parent)
		{
			GroupData groupDatum;
			if (!this.GroupExists(group))
			{
				return false;
			}
			group = group.ToLower();
			if (!this.groupdata.TryGetValue(group, out groupDatum))
			{
				return false;
			}
			if (string.IsNullOrEmpty(parent))
			{
				groupDatum.ParentGroup = null;
				return true;
			}
			if (!this.GroupExists(parent) || group.Equals(parent.ToLower()))
			{
				return false;
			}
			parent = parent.ToLower();
			if (!string.IsNullOrEmpty(groupDatum.ParentGroup) && groupDatum.ParentGroup.Equals(parent))
			{
				return true;
			}
			if (this.HasCircularParent(group, parent))
			{
				return false;
			}
			groupDatum.ParentGroup = parent;
			Interface.CallHook("OnGroupParentSet", group, parent);
			return true;
		}

		[LibraryFunction("SetGroupRank")]
		public bool SetGroupRank(string group, int rank)
		{
			GroupData groupDatum;
			if (!this.GroupExists(group))
			{
				return false;
			}
			group = group.ToLower();
			if (!this.groupdata.TryGetValue(group, out groupDatum))
			{
				return false;
			}
			if (groupDatum.Rank == rank)
			{
				return true;
			}
			groupDatum.Rank = rank;
			Interface.CallHook("OnGroupRankSet", group, rank);
			return true;
		}

		[LibraryFunction("SetGroupTitle")]
		public bool SetGroupTitle(string group, string title)
		{
			GroupData groupDatum;
			if (!this.GroupExists(group))
			{
				return false;
			}
			group = group.ToLower();
			if (!this.groupdata.TryGetValue(group, out groupDatum))
			{
				return false;
			}
			if (groupDatum.Title == title)
			{
				return true;
			}
			groupDatum.Title = title;
			Interface.CallHook("OnGroupTitleSet", group, title);
			return true;
		}

		[LibraryFunction("UpdateNickname")]
		public void UpdateNickname(string id, string nickname)
		{
			if (this.UserExists(id))
			{
				UserData userData = this.GetUserData(id);
				string lastSeenNickname = userData.LastSeenNickname;
				string str = nickname.Sanitize();
				userData.LastSeenNickname = nickname.Sanitize();
				Interface.CallHook("OnUserNameUpdated", id, lastSeenNickname, str);
			}
		}

		[LibraryFunction("UserExists")]
		public bool UserExists(string id)
		{
			return this.userdata.ContainsKey(id);
		}

		[LibraryFunction("UserHasAnyGroup")]
		public bool UserHasAnyGroup(string id)
		{
			if (!this.UserExists(id))
			{
				return false;
			}
			return this.GetUserData(id).Groups.Count > 0;
		}

		[LibraryFunction("UserHasGroup")]
		public bool UserHasGroup(string id, string name)
		{
			if (!this.GroupExists(name))
			{
				return false;
			}
			return this.GetUserData(id).Groups.Contains(name.ToLower());
		}

		[LibraryFunction("UserHasPermission")]
		public bool UserHasPermission(string id, string perm)
		{
			if (string.IsNullOrEmpty(perm))
			{
				return false;
			}
			perm = perm.ToLower();
			UserData userData = this.GetUserData(id);
			if (userData.Perms.Contains(perm))
			{
				return true;
			}
			return this.GroupsHavePermission(userData.Groups, perm);
		}

		[LibraryFunction("UserIdValid")]
		public bool UserIdValid(string id)
		{
			if (this.validate == null)
			{
				return true;
			}
			return this.validate(id);
		}
	}
}