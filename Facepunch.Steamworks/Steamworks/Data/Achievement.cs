using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	public struct Achievement
	{
		internal string Value;

		public string Description
		{
			get
			{
				return SteamUserStats.Internal.GetAchievementDisplayAttribute(this.Value, "desc");
			}
		}

		public float GlobalUnlocked
		{
			get
			{
				float single;
				float single1 = 0f;
				single = (SteamUserStats.Internal.GetAchievementAchievedPercent(this.Value, ref single1) ? single1 / 100f : -1f);
				return single;
			}
		}

		public string Identifier
		{
			get
			{
				return this.Value;
			}
		}

		public string Name
		{
			get
			{
				return SteamUserStats.Internal.GetAchievementDisplayAttribute(this.Value, "name");
			}
		}

		public bool State
		{
			get
			{
				bool flag = false;
				SteamUserStats.Internal.GetAchievement(this.Value, ref flag);
				return flag;
			}
		}

		public DateTime? UnlockTime
		{
			get
			{
				DateTime? nullable;
				bool flag = false;
				uint num = 0;
				if ((!SteamUserStats.Internal.GetAchievementAndUnlockTime(this.Value, ref flag, ref num) ? false : flag))
				{
					nullable = new DateTime?(Epoch.ToDateTime(num));
				}
				else
				{
					nullable = null;
				}
				return nullable;
			}
		}

		public Achievement(string name)
		{
			this.Value = name;
		}

		public bool Clear()
		{
			return SteamUserStats.Internal.ClearAchievement(this.Value);
		}

		public Image? GetIcon()
		{
			Image? image = SteamUtils.GetImage(SteamUserStats.Internal.GetAchievementIcon(this.Value));
			return image;
		}

		public async Task<Image?> GetIconAsync(int timeout = 5000)
		{
			Achievement.<GetIconAsync>d__14 variable = null;
			AsyncTaskMethodBuilder<Image?> asyncTaskMethodBuilder = AsyncTaskMethodBuilder<Image?>.Create();
			asyncTaskMethodBuilder.Start<Achievement.<GetIconAsync>d__14>(ref variable);
			return asyncTaskMethodBuilder.Task;
		}

		public override string ToString()
		{
			return this.Value;
		}

		public bool Trigger(bool apply = true)
		{
			bool flag = SteamUserStats.Internal.SetAchievement(this.Value);
			if (apply & flag)
			{
				SteamUserStats.Internal.StoreStats();
			}
			return flag;
		}
	}
}