using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Facepunch.Steamworks
{
	public class Achievements : IDisposable
	{
		internal Client client;

		private List<Achievement> unlockedRecently = new List<Achievement>();

		public Achievement[] All
		{
			get;
			private set;
		}

		internal Achievements(Client c)
		{
			this.client = c;
			this.All = new Achievement[0];
			c.RegisterCallback<UserStatsReceived_t>(new Action<UserStatsReceived_t>(this.UserStatsReceived));
			c.RegisterCallback<UserStatsStored_t>(new Action<UserStatsStored_t>(this.UserStatsStored));
			this.Refresh();
		}

		public void Dispose()
		{
			this.client = null;
		}

		public Achievement Find(string identifier)
		{
			return this.All.FirstOrDefault<Achievement>((Achievement x) => x.Id == identifier);
		}

		internal void OnUnlocked(Achievement a)
		{
			Action<Achievement> action = this.OnAchievementStateChanged;
			if (action == null)
			{
				return;
			}
			action(a);
		}

		public void Refresh()
		{
			Achievement[] all = this.All;
			this.All = Enumerable.Range(0, (int)this.client.native.userstats.GetNumAchievements()).Select<int, Achievement>((int x) => {
				if (all != null)
				{
					string achievementName = this.client.native.userstats.GetAchievementName((uint)x);
					Achievement achievement = all.FirstOrDefault<Achievement>((Achievement y) => y.Id == achievementName);
					if (achievement != null)
					{
						if (achievement.Refresh())
						{
							this.unlockedRecently.Add(achievement);
						}
						return achievement;
					}
				}
				return new Achievement(this.client, x);
			}).ToArray<Achievement>();
			foreach (Achievement achievement1 in this.unlockedRecently)
			{
				this.OnUnlocked(achievement1);
			}
			this.unlockedRecently.Clear();
		}

		public bool Reset(string identifier)
		{
			return this.client.native.userstats.ClearAchievement(identifier);
		}

		public bool Trigger(string identifier, bool apply = true)
		{
			Achievement achievement = this.Find(identifier);
			if (achievement == null)
			{
				return false;
			}
			return achievement.Trigger(apply);
		}

		private void UserStatsReceived(UserStatsReceived_t stats)
		{
			if (stats.GameID != (ulong)this.client.AppId)
			{
				return;
			}
			this.Refresh();
			Action action = this.OnUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		private void UserStatsStored(UserStatsStored_t stats)
		{
			if (stats.GameID != (ulong)this.client.AppId)
			{
				return;
			}
			this.Refresh();
			Action action = this.OnUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		public event Action<Achievement> OnAchievementStateChanged;

		public event Action OnUpdated;
	}
}