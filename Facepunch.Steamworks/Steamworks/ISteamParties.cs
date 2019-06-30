using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamParties : SteamInterface
	{
		private ISteamParties.FGetNumActiveBeacons _GetNumActiveBeacons;

		private ISteamParties.FGetBeaconByIndex _GetBeaconByIndex;

		private ISteamParties.FGetBeaconDetails _GetBeaconDetails;

		private ISteamParties.FGetBeaconDetails_Windows _GetBeaconDetails_Windows;

		private ISteamParties.FJoinParty _JoinParty;

		private ISteamParties.FGetNumAvailableBeaconLocations _GetNumAvailableBeaconLocations;

		private ISteamParties.FGetAvailableBeaconLocations _GetAvailableBeaconLocations;

		private ISteamParties.FGetAvailableBeaconLocations_Windows _GetAvailableBeaconLocations_Windows;

		private ISteamParties.FCreateBeacon _CreateBeacon;

		private ISteamParties.FCreateBeacon_Windows _CreateBeacon_Windows;

		private ISteamParties.FOnReservationCompleted _OnReservationCompleted;

		private ISteamParties.FCancelReservation _CancelReservation;

		private ISteamParties.FChangeNumOpenSlots _ChangeNumOpenSlots;

		private ISteamParties.FDestroyBeacon _DestroyBeacon;

		private ISteamParties.FGetBeaconLocationData _GetBeaconLocationData;

		private ISteamParties.FGetBeaconLocationData_Windows _GetBeaconLocationData_Windows;

		public override string InterfaceName
		{
			get
			{
				return "SteamParties002";
			}
		}

		public ISteamParties()
		{
		}

		internal void CancelReservation(PartyBeaconID_t ulBeacon, SteamId steamIDUser)
		{
			this._CancelReservation(this.Self, ulBeacon, steamIDUser);
		}

		internal async Task<ChangeNumOpenSlotsCallback_t?> ChangeNumOpenSlots(PartyBeaconID_t ulBeacon, uint unOpenSlots)
		{
			ChangeNumOpenSlotsCallback_t? resultAsync = await ChangeNumOpenSlotsCallback_t.GetResultAsync(this._ChangeNumOpenSlots(this.Self, ulBeacon, unOpenSlots));
			return resultAsync;
		}

		internal async Task<CreateBeaconCallback_t?> CreateBeacon(uint unOpenSlots, SteamPartyBeaconLocation_t pBeaconLocation, string pchConnectString, string pchMetadata)
		{
			CreateBeaconCallback_t? resultAsync;
			if (Config.Os != OsType.Windows)
			{
				CreateBeaconCallback_t? nullable = await CreateBeaconCallback_t.GetResultAsync(this._CreateBeacon(this.Self, unOpenSlots, ref pBeaconLocation, pchConnectString, pchMetadata));
				resultAsync = nullable;
			}
			else
			{
				SteamPartyBeaconLocation_t.Pack8 pack8 = pBeaconLocation;
				SteamAPICall_t _CreateBeaconWindows = this._CreateBeacon_Windows(this.Self, unOpenSlots, ref pack8, pchConnectString, pchMetadata);
				pBeaconLocation = pack8;
				resultAsync = await CreateBeaconCallback_t.GetResultAsync(_CreateBeaconWindows);
			}
			return resultAsync;
		}

		internal bool DestroyBeacon(PartyBeaconID_t ulBeacon)
		{
			return this._DestroyBeacon(this.Self, ulBeacon);
		}

		internal bool GetAvailableBeaconLocations(ref SteamPartyBeaconLocation_t pLocationList, uint uMaxNumLocations)
		{
			bool self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetAvailableBeaconLocations(this.Self, ref pLocationList, uMaxNumLocations);
			}
			else
			{
				SteamPartyBeaconLocation_t.Pack8 pack8 = pLocationList;
				bool _GetAvailableBeaconLocationsWindows = this._GetAvailableBeaconLocations_Windows(this.Self, ref pack8, uMaxNumLocations);
				pLocationList = pack8;
				self = _GetAvailableBeaconLocationsWindows;
			}
			return self;
		}

		internal PartyBeaconID_t GetBeaconByIndex(uint unIndex)
		{
			return this._GetBeaconByIndex(this.Self, unIndex);
		}

		internal bool GetBeaconDetails(PartyBeaconID_t ulBeaconID, ref SteamId pSteamIDBeaconOwner, ref SteamPartyBeaconLocation_t pLocation, StringBuilder pchMetadata, int cchMetadata)
		{
			bool self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetBeaconDetails(this.Self, ulBeaconID, ref pSteamIDBeaconOwner, ref pLocation, pchMetadata, cchMetadata);
			}
			else
			{
				SteamPartyBeaconLocation_t.Pack8 pack8 = pLocation;
				bool _GetBeaconDetailsWindows = this._GetBeaconDetails_Windows(this.Self, ulBeaconID, ref pSteamIDBeaconOwner, ref pack8, pchMetadata, cchMetadata);
				pLocation = pack8;
				self = _GetBeaconDetailsWindows;
			}
			return self;
		}

		internal bool GetBeaconLocationData(SteamPartyBeaconLocation_t BeaconLocation, SteamPartyBeaconLocationData eData, StringBuilder pchDataStringOut, int cchDataStringOut)
		{
			bool self;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetBeaconLocationData(this.Self, BeaconLocation, eData, pchDataStringOut, cchDataStringOut);
			}
			else
			{
				SteamPartyBeaconLocation_t.Pack8 beaconLocation = BeaconLocation;
				bool _GetBeaconLocationDataWindows = this._GetBeaconLocationData_Windows(this.Self, BeaconLocation, eData, pchDataStringOut, cchDataStringOut);
				BeaconLocation = beaconLocation;
				self = _GetBeaconLocationDataWindows;
			}
			return self;
		}

		internal uint GetNumActiveBeacons()
		{
			return this._GetNumActiveBeacons(this.Self);
		}

		internal bool GetNumAvailableBeaconLocations(ref uint puNumLocations)
		{
			return this._GetNumAvailableBeaconLocations(this.Self, ref puNumLocations);
		}

		public override void InitInternals()
		{
			this._GetNumActiveBeacons = Marshal.GetDelegateForFunctionPointer<ISteamParties.FGetNumActiveBeacons>(Marshal.ReadIntPtr(this.VTable, 0));
			this._GetBeaconByIndex = Marshal.GetDelegateForFunctionPointer<ISteamParties.FGetBeaconByIndex>(Marshal.ReadIntPtr(this.VTable, 8));
			this._GetBeaconDetails = Marshal.GetDelegateForFunctionPointer<ISteamParties.FGetBeaconDetails>(Marshal.ReadIntPtr(this.VTable, 16));
			this._GetBeaconDetails_Windows = Marshal.GetDelegateForFunctionPointer<ISteamParties.FGetBeaconDetails_Windows>(Marshal.ReadIntPtr(this.VTable, 16));
			this._JoinParty = Marshal.GetDelegateForFunctionPointer<ISteamParties.FJoinParty>(Marshal.ReadIntPtr(this.VTable, 24));
			this._GetNumAvailableBeaconLocations = Marshal.GetDelegateForFunctionPointer<ISteamParties.FGetNumAvailableBeaconLocations>(Marshal.ReadIntPtr(this.VTable, 32));
			this._GetAvailableBeaconLocations = Marshal.GetDelegateForFunctionPointer<ISteamParties.FGetAvailableBeaconLocations>(Marshal.ReadIntPtr(this.VTable, 40));
			this._GetAvailableBeaconLocations_Windows = Marshal.GetDelegateForFunctionPointer<ISteamParties.FGetAvailableBeaconLocations_Windows>(Marshal.ReadIntPtr(this.VTable, 40));
			this._CreateBeacon = Marshal.GetDelegateForFunctionPointer<ISteamParties.FCreateBeacon>(Marshal.ReadIntPtr(this.VTable, 48));
			this._CreateBeacon_Windows = Marshal.GetDelegateForFunctionPointer<ISteamParties.FCreateBeacon_Windows>(Marshal.ReadIntPtr(this.VTable, 48));
			this._OnReservationCompleted = Marshal.GetDelegateForFunctionPointer<ISteamParties.FOnReservationCompleted>(Marshal.ReadIntPtr(this.VTable, 56));
			this._CancelReservation = Marshal.GetDelegateForFunctionPointer<ISteamParties.FCancelReservation>(Marshal.ReadIntPtr(this.VTable, 64));
			this._ChangeNumOpenSlots = Marshal.GetDelegateForFunctionPointer<ISteamParties.FChangeNumOpenSlots>(Marshal.ReadIntPtr(this.VTable, 72));
			this._DestroyBeacon = Marshal.GetDelegateForFunctionPointer<ISteamParties.FDestroyBeacon>(Marshal.ReadIntPtr(this.VTable, 80));
			this._GetBeaconLocationData = Marshal.GetDelegateForFunctionPointer<ISteamParties.FGetBeaconLocationData>(Marshal.ReadIntPtr(this.VTable, 88));
			this._GetBeaconLocationData_Windows = Marshal.GetDelegateForFunctionPointer<ISteamParties.FGetBeaconLocationData_Windows>(Marshal.ReadIntPtr(this.VTable, 88));
		}

		internal async Task<JoinPartyCallback_t?> JoinParty(PartyBeaconID_t ulBeaconID)
		{
			JoinPartyCallback_t? resultAsync = await JoinPartyCallback_t.GetResultAsync(this._JoinParty(this.Self, ulBeaconID));
			return resultAsync;
		}

		internal void OnReservationCompleted(PartyBeaconID_t ulBeacon, SteamId steamIDUser)
		{
			this._OnReservationCompleted(this.Self, ulBeacon, steamIDUser);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._GetNumActiveBeacons = null;
			this._GetBeaconByIndex = null;
			this._GetBeaconDetails = null;
			this._GetBeaconDetails_Windows = null;
			this._JoinParty = null;
			this._GetNumAvailableBeaconLocations = null;
			this._GetAvailableBeaconLocations = null;
			this._GetAvailableBeaconLocations_Windows = null;
			this._CreateBeacon = null;
			this._CreateBeacon_Windows = null;
			this._OnReservationCompleted = null;
			this._CancelReservation = null;
			this._ChangeNumOpenSlots = null;
			this._DestroyBeacon = null;
			this._GetBeaconLocationData = null;
			this._GetBeaconLocationData_Windows = null;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FCancelReservation(IntPtr self, PartyBeaconID_t ulBeacon, SteamId steamIDUser);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FChangeNumOpenSlots(IntPtr self, PartyBeaconID_t ulBeacon, uint unOpenSlots);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FCreateBeacon(IntPtr self, uint unOpenSlots, ref SteamPartyBeaconLocation_t pBeaconLocation, string pchConnectString, string pchMetadata);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FCreateBeacon_Windows(IntPtr self, uint unOpenSlots, ref SteamPartyBeaconLocation_t.Pack8 pBeaconLocation, string pchConnectString, string pchMetadata);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FDestroyBeacon(IntPtr self, PartyBeaconID_t ulBeacon);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetAvailableBeaconLocations(IntPtr self, ref SteamPartyBeaconLocation_t pLocationList, uint uMaxNumLocations);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetAvailableBeaconLocations_Windows(IntPtr self, ref SteamPartyBeaconLocation_t.Pack8 pLocationList, uint uMaxNumLocations);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate PartyBeaconID_t FGetBeaconByIndex(IntPtr self, uint unIndex);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetBeaconDetails(IntPtr self, PartyBeaconID_t ulBeaconID, ref SteamId pSteamIDBeaconOwner, ref SteamPartyBeaconLocation_t pLocation, StringBuilder pchMetadata, int cchMetadata);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetBeaconDetails_Windows(IntPtr self, PartyBeaconID_t ulBeaconID, ref SteamId pSteamIDBeaconOwner, ref SteamPartyBeaconLocation_t.Pack8 pLocation, StringBuilder pchMetadata, int cchMetadata);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetBeaconLocationData(IntPtr self, SteamPartyBeaconLocation_t BeaconLocation, SteamPartyBeaconLocationData eData, StringBuilder pchDataStringOut, int cchDataStringOut);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetBeaconLocationData_Windows(IntPtr self, SteamPartyBeaconLocation_t.Pack8 BeaconLocation, SteamPartyBeaconLocationData eData, StringBuilder pchDataStringOut, int cchDataStringOut);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetNumActiveBeacons(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetNumAvailableBeaconLocations(IntPtr self, ref uint puNumLocations);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FJoinParty(IntPtr self, PartyBeaconID_t ulBeaconID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FOnReservationCompleted(IntPtr self, PartyBeaconID_t ulBeacon, SteamId steamIDUser);
	}
}