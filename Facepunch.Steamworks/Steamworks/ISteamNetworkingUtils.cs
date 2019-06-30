using Steamworks.Data;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	internal class ISteamNetworkingUtils : SteamInterface
	{
		private ISteamNetworkingUtils.FGetLocalPingLocation _GetLocalPingLocation;

		private ISteamNetworkingUtils.FEstimatePingTimeBetweenTwoLocations _EstimatePingTimeBetweenTwoLocations;

		private ISteamNetworkingUtils.FEstimatePingTimeFromLocalHost _EstimatePingTimeFromLocalHost;

		private ISteamNetworkingUtils.FConvertPingLocationToString _ConvertPingLocationToString;

		private ISteamNetworkingUtils.FParsePingLocationString _ParsePingLocationString;

		private ISteamNetworkingUtils.FCheckPingDataUpToDate _CheckPingDataUpToDate;

		private ISteamNetworkingUtils.FIsPingMeasurementInProgress _IsPingMeasurementInProgress;

		private ISteamNetworkingUtils.FGetPingToDataCenter _GetPingToDataCenter;

		private ISteamNetworkingUtils.FGetDirectPingToPOP _GetDirectPingToPOP;

		private ISteamNetworkingUtils.FGetPOPCount _GetPOPCount;

		private ISteamNetworkingUtils.FGetPOPList _GetPOPList;

		private ISteamNetworkingUtils.FGetLocalTimestamp _GetLocalTimestamp;

		private ISteamNetworkingUtils.FSetDebugOutputFunction _SetDebugOutputFunction;

		private ISteamNetworkingUtils.FSetConfigValue _SetConfigValue;

		private ISteamNetworkingUtils.FGetConfigValue _GetConfigValue;

		private ISteamNetworkingUtils.FGetConfigValueInfo _GetConfigValueInfo;

		private ISteamNetworkingUtils.FGetFirstConfigValue _GetFirstConfigValue;

		public override string InterfaceName
		{
			get
			{
				return "SteamNetworkingUtils001";
			}
		}

		public ISteamNetworkingUtils()
		{
		}

		internal bool CheckPingDataUpToDate(float flMaxAgeSeconds)
		{
			return this._CheckPingDataUpToDate(this.Self, flMaxAgeSeconds);
		}

		internal void ConvertPingLocationToString(ref PingLocation location, StringBuilder pszBuf, int cchBufSize)
		{
			this._ConvertPingLocationToString(this.Self, ref location, pszBuf, cchBufSize);
		}

		internal int EstimatePingTimeBetweenTwoLocations(ref PingLocation location1, ref PingLocation location2)
		{
			return this._EstimatePingTimeBetweenTwoLocations(this.Self, ref location1, ref location2);
		}

		internal int EstimatePingTimeFromLocalHost(ref PingLocation remoteLocation)
		{
			return this._EstimatePingTimeFromLocalHost(this.Self, ref remoteLocation);
		}

		internal NetConfigResult GetConfigValue(NetConfig eValue, NetScope eScopeType, long scopeObj, ref NetConfigType pOutDataType, IntPtr pResult, ref ulong cbResult)
		{
			NetConfigResult self = this._GetConfigValue(this.Self, eValue, eScopeType, scopeObj, ref pOutDataType, pResult, ref cbResult);
			return self;
		}

		internal bool GetConfigValueInfo(NetConfig eValue, [In][Out] string[] pOutName, ref NetConfigType pOutDataType, [In][Out] NetScope[] pOutScope, [In][Out] NetConfig[] pOutNextValue)
		{
			bool self = this._GetConfigValueInfo(this.Self, eValue, pOutName, ref pOutDataType, pOutScope, pOutNextValue);
			return self;
		}

		internal int GetDirectPingToPOP(SteamNetworkingPOPID popID)
		{
			return this._GetDirectPingToPOP(this.Self, popID);
		}

		internal NetConfig GetFirstConfigValue()
		{
			return this._GetFirstConfigValue(this.Self);
		}

		internal float GetLocalPingLocation(ref PingLocation result)
		{
			return this._GetLocalPingLocation(this.Self, ref result);
		}

		internal long GetLocalTimestamp()
		{
			return this._GetLocalTimestamp(this.Self);
		}

		internal int GetPingToDataCenter(SteamNetworkingPOPID popID, ref SteamNetworkingPOPID pViaRelayPoP)
		{
			return this._GetPingToDataCenter(this.Self, popID, ref pViaRelayPoP);
		}

		internal int GetPOPCount()
		{
			return this._GetPOPCount(this.Self);
		}

		internal int GetPOPList(ref SteamNetworkingPOPID list, int nListSz)
		{
			return this._GetPOPList(this.Self, ref list, nListSz);
		}

		public override void InitInternals()
		{
			this._GetLocalPingLocation = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FGetLocalPingLocation>(Marshal.ReadIntPtr(this.VTable, 0));
			this._EstimatePingTimeBetweenTwoLocations = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FEstimatePingTimeBetweenTwoLocations>(Marshal.ReadIntPtr(this.VTable, 8));
			this._EstimatePingTimeFromLocalHost = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FEstimatePingTimeFromLocalHost>(Marshal.ReadIntPtr(this.VTable, 16));
			this._ConvertPingLocationToString = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FConvertPingLocationToString>(Marshal.ReadIntPtr(this.VTable, 24));
			this._ParsePingLocationString = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FParsePingLocationString>(Marshal.ReadIntPtr(this.VTable, 32));
			this._CheckPingDataUpToDate = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FCheckPingDataUpToDate>(Marshal.ReadIntPtr(this.VTable, 40));
			this._IsPingMeasurementInProgress = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FIsPingMeasurementInProgress>(Marshal.ReadIntPtr(this.VTable, 48));
			this._GetPingToDataCenter = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FGetPingToDataCenter>(Marshal.ReadIntPtr(this.VTable, 56));
			this._GetDirectPingToPOP = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FGetDirectPingToPOP>(Marshal.ReadIntPtr(this.VTable, 64));
			this._GetPOPCount = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FGetPOPCount>(Marshal.ReadIntPtr(this.VTable, 72));
			this._GetPOPList = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FGetPOPList>(Marshal.ReadIntPtr(this.VTable, 80));
			this._GetLocalTimestamp = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FGetLocalTimestamp>(Marshal.ReadIntPtr(this.VTable, 88));
			this._SetDebugOutputFunction = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FSetDebugOutputFunction>(Marshal.ReadIntPtr(this.VTable, 96));
			this._SetConfigValue = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FSetConfigValue>(Marshal.ReadIntPtr(this.VTable, 104));
			this._GetConfigValue = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FGetConfigValue>(Marshal.ReadIntPtr(this.VTable, 112));
			this._GetConfigValueInfo = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FGetConfigValueInfo>(Marshal.ReadIntPtr(this.VTable, 120));
			this._GetFirstConfigValue = Marshal.GetDelegateForFunctionPointer<ISteamNetworkingUtils.FGetFirstConfigValue>(Marshal.ReadIntPtr(this.VTable, 128));
		}

		internal bool IsPingMeasurementInProgress()
		{
			return this._IsPingMeasurementInProgress(this.Self);
		}

		internal bool ParsePingLocationString(string pszString, ref PingLocation result)
		{
			return this._ParsePingLocationString(this.Self, pszString, ref result);
		}

		internal bool SetConfigValue(NetConfig eValue, NetScope eScopeType, long scopeObj, NetConfigType eDataType, IntPtr pArg)
		{
			bool self = this._SetConfigValue(this.Self, eValue, eScopeType, scopeObj, eDataType, pArg);
			return self;
		}

		internal void SetDebugOutputFunction(DebugOutputType eDetailLevel, FSteamNetworkingSocketsDebugOutput pfnFunc)
		{
			this._SetDebugOutputFunction(this.Self, eDetailLevel, pfnFunc);
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._GetLocalPingLocation = null;
			this._EstimatePingTimeBetweenTwoLocations = null;
			this._EstimatePingTimeFromLocalHost = null;
			this._ConvertPingLocationToString = null;
			this._ParsePingLocationString = null;
			this._CheckPingDataUpToDate = null;
			this._IsPingMeasurementInProgress = null;
			this._GetPingToDataCenter = null;
			this._GetDirectPingToPOP = null;
			this._GetPOPCount = null;
			this._GetPOPList = null;
			this._GetLocalTimestamp = null;
			this._SetDebugOutputFunction = null;
			this._SetConfigValue = null;
			this._GetConfigValue = null;
			this._GetConfigValueInfo = null;
			this._GetFirstConfigValue = null;
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FCheckPingDataUpToDate(IntPtr self, float flMaxAgeSeconds);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FConvertPingLocationToString(IntPtr self, ref PingLocation location, StringBuilder pszBuf, int cchBufSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FEstimatePingTimeBetweenTwoLocations(IntPtr self, ref PingLocation location1, ref PingLocation location2);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FEstimatePingTimeFromLocalHost(IntPtr self, ref PingLocation remoteLocation);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate NetConfigResult FGetConfigValue(IntPtr self, NetConfig eValue, NetScope eScopeType, long scopeObj, ref NetConfigType pOutDataType, IntPtr pResult, ref ulong cbResult);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetConfigValueInfo(IntPtr self, NetConfig eValue, [In][Out] string[] pOutName, ref NetConfigType pOutDataType, [In][Out] NetScope[] pOutScope, [In][Out] NetConfig[] pOutNextValue);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetDirectPingToPOP(IntPtr self, SteamNetworkingPOPID popID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate NetConfig FGetFirstConfigValue(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate float FGetLocalPingLocation(IntPtr self, ref PingLocation result);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate long FGetLocalTimestamp(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetPingToDataCenter(IntPtr self, SteamNetworkingPOPID popID, ref SteamNetworkingPOPID pViaRelayPoP);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetPOPCount(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int FGetPOPList(IntPtr self, ref SteamNetworkingPOPID list, int nListSz);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FIsPingMeasurementInProgress(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FParsePingLocationString(IntPtr self, string pszString, ref PingLocation result);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetConfigValue(IntPtr self, NetConfig eValue, NetScope eScopeType, long scopeObj, NetConfigType eDataType, IntPtr pArg);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSetDebugOutputFunction(IntPtr self, DebugOutputType eDetailLevel, FSteamNetworkingSocketsDebugOutput pfnFunc);
	}
}