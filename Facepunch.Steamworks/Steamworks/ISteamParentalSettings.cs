using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamParentalSettings : SteamInterface
	{
		private ISteamParentalSettings.FBIsParentalLockEnabled _BIsParentalLockEnabled;

		private ISteamParentalSettings.FBIsParentalLockLocked _BIsParentalLockLocked;

		private ISteamParentalSettings.FBIsAppBlocked _BIsAppBlocked;

		private ISteamParentalSettings.FBIsAppInBlockList _BIsAppInBlockList;

		private ISteamParentalSettings.FBIsFeatureBlocked _BIsFeatureBlocked;

		private ISteamParentalSettings.FBIsFeatureInBlockList _BIsFeatureInBlockList;

		public override string InterfaceName
		{
			get
			{
				return "STEAMPARENTALSETTINGS_INTERFACE_VERSION001";
			}
		}

		public ISteamParentalSettings()
		{
		}

		internal bool BIsAppBlocked(AppId nAppID)
		{
			return this._BIsAppBlocked(this.Self, nAppID);
		}

		internal bool BIsAppInBlockList(AppId nAppID)
		{
			return this._BIsAppInBlockList(this.Self, nAppID);
		}

		internal bool BIsFeatureBlocked(ParentalFeature eFeature)
		{
			return this._BIsFeatureBlocked(this.Self, eFeature);
		}

		internal bool BIsFeatureInBlockList(ParentalFeature eFeature)
		{
			return this._BIsFeatureInBlockList(this.Self, eFeature);
		}

		internal bool BIsParentalLockEnabled()
		{
			return this._BIsParentalLockEnabled(this.Self);
		}

		internal bool BIsParentalLockLocked()
		{
			return this._BIsParentalLockLocked(this.Self);
		}

		public override void InitInternals()
		{
			this._BIsParentalLockEnabled = Marshal.GetDelegateForFunctionPointer<ISteamParentalSettings.FBIsParentalLockEnabled>(Marshal.ReadIntPtr(this.VTable, 0));
			this._BIsParentalLockLocked = Marshal.GetDelegateForFunctionPointer<ISteamParentalSettings.FBIsParentalLockLocked>(Marshal.ReadIntPtr(this.VTable, 8));
			this._BIsAppBlocked = Marshal.GetDelegateForFunctionPointer<ISteamParentalSettings.FBIsAppBlocked>(Marshal.ReadIntPtr(this.VTable, 16));
			this._BIsAppInBlockList = Marshal.GetDelegateForFunctionPointer<ISteamParentalSettings.FBIsAppInBlockList>(Marshal.ReadIntPtr(this.VTable, 24));
			this._BIsFeatureBlocked = Marshal.GetDelegateForFunctionPointer<ISteamParentalSettings.FBIsFeatureBlocked>(Marshal.ReadIntPtr(this.VTable, 32));
			this._BIsFeatureInBlockList = Marshal.GetDelegateForFunctionPointer<ISteamParentalSettings.FBIsFeatureInBlockList>(Marshal.ReadIntPtr(this.VTable, 40));
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._BIsParentalLockEnabled = null;
			this._BIsParentalLockLocked = null;
			this._BIsAppBlocked = null;
			this._BIsAppInBlockList = null;
			this._BIsFeatureBlocked = null;
			this._BIsFeatureInBlockList = null;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsAppBlocked(IntPtr self, AppId nAppID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsAppInBlockList(IntPtr self, AppId nAppID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsFeatureBlocked(IntPtr self, ParentalFeature eFeature);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsFeatureInBlockList(IntPtr self, ParentalFeature eFeature);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsParentalLockEnabled(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FBIsParentalLockLocked(IntPtr self);
	}
}