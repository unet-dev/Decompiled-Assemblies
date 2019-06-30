using Steamworks.Data;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Steamworks
{
	public static class SteamParental
	{
		private static ISteamParentalSettings _internal;

		internal static ISteamParentalSettings Internal
		{
			get
			{
				if (SteamParental._internal == null)
				{
					SteamParental._internal = new ISteamParentalSettings();
					SteamParental._internal.Init();
				}
				return SteamParental._internal;
			}
		}

		public static bool IsParentalLockEnabled
		{
			get
			{
				return SteamParental.Internal.BIsParentalLockEnabled();
			}
		}

		public static bool IsParentalLockLocked
		{
			get
			{
				return SteamParental.Internal.BIsParentalLockLocked();
			}
		}

		public static bool BIsAppInBlockList(AppId app)
		{
			return SteamParental.Internal.BIsAppInBlockList(app.Value);
		}

		public static bool BIsFeatureInBlockList(ParentalFeature feature)
		{
			return SteamParental.Internal.BIsFeatureInBlockList(feature);
		}

		internal static void InstallEvents()
		{
			SteamParentalSettingsChanged_t.Install((SteamParentalSettingsChanged_t x) => {
				Action onSettingsChanged = SteamParental.OnSettingsChanged;
				if (onSettingsChanged != null)
				{
					onSettingsChanged();
				}
				else
				{
				}
			}, false);
		}

		public static bool IsAppBlocked(AppId app)
		{
			return SteamParental.Internal.BIsAppBlocked(app.Value);
		}

		public static bool IsFeatureBlocked(ParentalFeature feature)
		{
			return SteamParental.Internal.BIsFeatureBlocked(feature);
		}

		internal static void Shutdown()
		{
			SteamParental._internal = null;
		}

		public static event Action OnSettingsChanged;
	}
}