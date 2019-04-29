using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public class Achievement
	{
		private Client client;

		private int refreshCount;

		private Image _icon;

		public string Description
		{
			get;
			private set;
		}

		public float GlobalUnlockedPercentage
		{
			get
			{
				if (this.State)
				{
					return 1f;
				}
				float single = 0f;
				if (!this.client.native.userstats.GetAchievementAchievedPercent(this.Id, out single))
				{
					return -1f;
				}
				return single;
			}
		}

		public Image Icon
		{
			get
			{
				if (this.iconId <= 0)
				{
					return null;
				}
				if (this._icon == null)
				{
					this._icon = new Image()
					{
						Id = this.iconId
					};
				}
				if (this._icon.IsLoaded)
				{
					return this._icon;
				}
				if (!this._icon.TryLoad(this.client.native.utils))
				{
					return null;
				}
				return this._icon;
			}
		}

		private int iconId { get; set; } = -1;

		public string Id
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public bool State
		{
			get;
			private set;
		}

		public DateTime UnlockTime
		{
			get;
			private set;
		}

		public Achievement(Client client, int index)
		{
			this.client = client;
			this.Id = client.native.userstats.GetAchievementName((uint)index);
			this.Name = client.native.userstats.GetAchievementDisplayAttribute(this.Id, "name");
			this.Description = client.native.userstats.GetAchievementDisplayAttribute(this.Id, "desc");
			this.iconId = client.native.userstats.GetAchievementIcon(this.Id);
			this.Refresh();
		}

		public bool Refresh()
		{
			uint num;
			bool state = this.State;
			bool flag = false;
			this.State = false;
			if (this.client.native.userstats.GetAchievementAndUnlockTime(this.Id, ref flag, out num))
			{
				this.State = flag;
				this.UnlockTime = Utility.Epoch.ToDateTime(num);
			}
			this.refreshCount++;
			if (state != this.State && this.refreshCount > 1)
			{
				return true;
			}
			return false;
		}

		public bool Reset()
		{
			this.State = false;
			this.UnlockTime = DateTime.Now;
			return this.client.native.userstats.ClearAchievement(this.Id);
		}

		public bool Trigger(bool apply = true)
		{
			if (this.State)
			{
				return false;
			}
			this.State = true;
			this.UnlockTime = DateTime.Now;
			bool flag = this.client.native.userstats.SetAchievement(this.Id);
			if (apply)
			{
				this.client.Stats.StoreStats();
			}
			this.client.Achievements.OnUnlocked(this);
			return flag;
		}
	}
}