using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	internal class ISteamInventory : SteamInterface
	{
		private ISteamInventory.FGetResultStatus _GetResultStatus;

		private ISteamInventory.FGetResultItems _GetResultItems;

		private ISteamInventory.FGetResultItems_Windows _GetResultItems_Windows;

		private ISteamInventory.FGetResultItemProperty _GetResultItemProperty;

		private ISteamInventory.FGetResultTimestamp _GetResultTimestamp;

		private ISteamInventory.FCheckResultSteamID _CheckResultSteamID;

		private ISteamInventory.FDestroyResult _DestroyResult;

		private ISteamInventory.FGetAllItems _GetAllItems;

		private ISteamInventory.FGetItemsByID _GetItemsByID;

		private ISteamInventory.FSerializeResult _SerializeResult;

		private ISteamInventory.FDeserializeResult _DeserializeResult;

		private ISteamInventory.FGenerateItems _GenerateItems;

		private ISteamInventory.FGrantPromoItems _GrantPromoItems;

		private ISteamInventory.FAddPromoItem _AddPromoItem;

		private ISteamInventory.FAddPromoItems _AddPromoItems;

		private ISteamInventory.FConsumeItem _ConsumeItem;

		private ISteamInventory.FExchangeItems _ExchangeItems;

		private ISteamInventory.FTransferItemQuantity _TransferItemQuantity;

		private ISteamInventory.FSendItemDropHeartbeat _SendItemDropHeartbeat;

		private ISteamInventory.FTriggerItemDrop _TriggerItemDrop;

		private ISteamInventory.FTradeItems _TradeItems;

		private ISteamInventory.FLoadItemDefinitions _LoadItemDefinitions;

		private ISteamInventory.FGetItemDefinitionIDs _GetItemDefinitionIDs;

		private ISteamInventory.FGetItemDefinitionProperty _GetItemDefinitionProperty;

		private ISteamInventory.FRequestEligiblePromoItemDefinitionsIDs _RequestEligiblePromoItemDefinitionsIDs;

		private ISteamInventory.FGetEligiblePromoItemDefinitionIDs _GetEligiblePromoItemDefinitionIDs;

		private ISteamInventory.FStartPurchase _StartPurchase;

		private ISteamInventory.FRequestPrices _RequestPrices;

		private ISteamInventory.FGetNumItemsWithPrices _GetNumItemsWithPrices;

		private ISteamInventory.FGetItemsWithPrices _GetItemsWithPrices;

		private ISteamInventory.FGetItemPrice _GetItemPrice;

		private ISteamInventory.FStartUpdateProperties _StartUpdateProperties;

		private ISteamInventory.FRemoveProperty _RemoveProperty;

		private ISteamInventory.FSetProperty1 _SetProperty1;

		private ISteamInventory.FSetProperty2 _SetProperty2;

		private ISteamInventory.FSetProperty3 _SetProperty3;

		private ISteamInventory.FSetProperty4 _SetProperty4;

		private ISteamInventory.FSubmitUpdateProperties _SubmitUpdateProperties;

		public override string InterfaceName
		{
			get
			{
				return "STEAMINVENTORY_INTERFACE_V003";
			}
		}

		public ISteamInventory()
		{
		}

		internal bool AddPromoItem(ref SteamInventoryResult_t pResultHandle, InventoryDefId itemDef)
		{
			return this._AddPromoItem(this.Self, ref pResultHandle, itemDef);
		}

		internal bool AddPromoItems(ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayItemDefs, uint unArrayLength)
		{
			return this._AddPromoItems(this.Self, ref pResultHandle, pArrayItemDefs, unArrayLength);
		}

		internal bool CheckResultSteamID(SteamInventoryResult_t resultHandle, SteamId steamIDExpected)
		{
			return this._CheckResultSteamID(this.Self, resultHandle, steamIDExpected);
		}

		internal bool ConsumeItem(ref SteamInventoryResult_t pResultHandle, InventoryItemId itemConsume, uint unQuantity)
		{
			return this._ConsumeItem(this.Self, ref pResultHandle, itemConsume, unQuantity);
		}

		internal bool DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE)
		{
			bool self = this._DeserializeResult(this.Self, ref pOutResultHandle, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
			return self;
		}

		internal void DestroyResult(SteamInventoryResult_t resultHandle)
		{
			this._DestroyResult(this.Self, resultHandle);
		}

		internal bool ExchangeItems(ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayGenerate, [In][Out] uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, [In][Out] InventoryItemId[] pArrayDestroy, [In][Out] uint[] punArrayDestroyQuantity, uint unArrayDestroyLength)
		{
			bool self = this._ExchangeItems(this.Self, ref pResultHandle, pArrayGenerate, punArrayGenerateQuantity, unArrayGenerateLength, pArrayDestroy, punArrayDestroyQuantity, unArrayDestroyLength);
			return self;
		}

		internal bool GenerateItems(ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] uint[] punArrayQuantity, uint unArrayLength)
		{
			bool self = this._GenerateItems(this.Self, ref pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
			return self;
		}

		internal bool GetAllItems(ref SteamInventoryResult_t pResultHandle)
		{
			return this._GetAllItems(this.Self, ref pResultHandle);
		}

		internal bool GetEligiblePromoItemDefinitionIDs(SteamId steamID, [In][Out] InventoryDefId[] pItemDefIDs, ref uint punItemDefIDsArraySize)
		{
			return this._GetEligiblePromoItemDefinitionIDs(this.Self, steamID, pItemDefIDs, ref punItemDefIDsArraySize);
		}

		internal bool GetItemDefinitionIDs([In][Out] InventoryDefId[] pItemDefIDs, ref uint punItemDefIDsArraySize)
		{
			return this._GetItemDefinitionIDs(this.Self, pItemDefIDs, ref punItemDefIDsArraySize);
		}

		internal bool GetItemDefinitionProperty(InventoryDefId iDefinition, string pchPropertyName, StringBuilder pchValueBuffer, ref uint punValueBufferSizeOut)
		{
			bool self = this._GetItemDefinitionProperty(this.Self, iDefinition, pchPropertyName, pchValueBuffer, ref punValueBufferSizeOut);
			return self;
		}

		internal bool GetItemPrice(InventoryDefId iDefinition, ref ulong pCurrentPrice, ref ulong pBasePrice)
		{
			return this._GetItemPrice(this.Self, iDefinition, ref pCurrentPrice, ref pBasePrice);
		}

		internal bool GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref InventoryItemId pInstanceIDs, uint unCountInstanceIDs)
		{
			return this._GetItemsByID(this.Self, ref pResultHandle, ref pInstanceIDs, unCountInstanceIDs);
		}

		internal bool GetItemsWithPrices([In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] ulong[] pCurrentPrices, [In][Out] ulong[] pBasePrices, uint unArrayLength)
		{
			bool self = this._GetItemsWithPrices(this.Self, pArrayItemDefs, pCurrentPrices, pBasePrices, unArrayLength);
			return self;
		}

		internal uint GetNumItemsWithPrices()
		{
			return this._GetNumItemsWithPrices(this.Self);
		}

		internal bool GetResultItemProperty(SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, StringBuilder pchValueBuffer, ref uint punValueBufferSizeOut)
		{
			bool self = this._GetResultItemProperty(this.Self, resultHandle, unItemIndex, pchPropertyName, pchValueBuffer, ref punValueBufferSizeOut);
			return self;
		}

		internal bool GetResultItems(SteamInventoryResult_t resultHandle, [In][Out] SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize)
		{
			bool self;
			SteamItemDetails_t.Pack8[] pack8Array;
			if (Config.Os != OsType.Windows)
			{
				self = this._GetResultItems(this.Self, resultHandle, pOutItemsArray, ref punOutItemsArraySize);
			}
			else
			{
				if (pOutItemsArray == null)
				{
					pack8Array = null;
				}
				else
				{
					pack8Array = new SteamItemDetails_t.Pack8[(int)pOutItemsArray.Length];
				}
				SteamItemDetails_t.Pack8[] pack8Array1 = pack8Array;
				if (pack8Array1 != null)
				{
					for (int i = 0; i < (int)pOutItemsArray.Length; i++)
					{
						pack8Array1[i] = pOutItemsArray[i];
					}
				}
				bool _GetResultItemsWindows = this._GetResultItems_Windows(this.Self, resultHandle, pack8Array1, ref punOutItemsArraySize);
				if (pack8Array1 != null)
				{
					for (int j = 0; j < (int)pOutItemsArray.Length; j++)
					{
						pOutItemsArray[j] = pack8Array1[j];
					}
				}
				self = _GetResultItemsWindows;
			}
			return self;
		}

		internal Result GetResultStatus(SteamInventoryResult_t resultHandle)
		{
			return this._GetResultStatus(this.Self, resultHandle);
		}

		internal uint GetResultTimestamp(SteamInventoryResult_t resultHandle)
		{
			return this._GetResultTimestamp(this.Self, resultHandle);
		}

		internal bool GrantPromoItems(ref SteamInventoryResult_t pResultHandle)
		{
			return this._GrantPromoItems(this.Self, ref pResultHandle);
		}

		public override void InitInternals()
		{
			this._GetResultStatus = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetResultStatus>(Marshal.ReadIntPtr(this.VTable, 0));
			this._GetResultItems = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetResultItems>(Marshal.ReadIntPtr(this.VTable, 8));
			this._GetResultItems_Windows = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetResultItems_Windows>(Marshal.ReadIntPtr(this.VTable, 8));
			this._GetResultItemProperty = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetResultItemProperty>(Marshal.ReadIntPtr(this.VTable, 16));
			this._GetResultTimestamp = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetResultTimestamp>(Marshal.ReadIntPtr(this.VTable, 24));
			this._CheckResultSteamID = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FCheckResultSteamID>(Marshal.ReadIntPtr(this.VTable, 32));
			this._DestroyResult = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FDestroyResult>(Marshal.ReadIntPtr(this.VTable, 40));
			this._GetAllItems = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetAllItems>(Marshal.ReadIntPtr(this.VTable, 48));
			this._GetItemsByID = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetItemsByID>(Marshal.ReadIntPtr(this.VTable, 56));
			this._SerializeResult = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FSerializeResult>(Marshal.ReadIntPtr(this.VTable, 64));
			this._DeserializeResult = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FDeserializeResult>(Marshal.ReadIntPtr(this.VTable, 72));
			this._GenerateItems = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGenerateItems>(Marshal.ReadIntPtr(this.VTable, 80));
			this._GrantPromoItems = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGrantPromoItems>(Marshal.ReadIntPtr(this.VTable, 88));
			this._AddPromoItem = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FAddPromoItem>(Marshal.ReadIntPtr(this.VTable, 96));
			this._AddPromoItems = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FAddPromoItems>(Marshal.ReadIntPtr(this.VTable, 104));
			this._ConsumeItem = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FConsumeItem>(Marshal.ReadIntPtr(this.VTable, 112));
			this._ExchangeItems = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FExchangeItems>(Marshal.ReadIntPtr(this.VTable, 120));
			this._TransferItemQuantity = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FTransferItemQuantity>(Marshal.ReadIntPtr(this.VTable, 128));
			this._SendItemDropHeartbeat = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FSendItemDropHeartbeat>(Marshal.ReadIntPtr(this.VTable, 136));
			this._TriggerItemDrop = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FTriggerItemDrop>(Marshal.ReadIntPtr(this.VTable, 144));
			this._TradeItems = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FTradeItems>(Marshal.ReadIntPtr(this.VTable, 152));
			this._LoadItemDefinitions = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FLoadItemDefinitions>(Marshal.ReadIntPtr(this.VTable, 160));
			this._GetItemDefinitionIDs = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetItemDefinitionIDs>(Marshal.ReadIntPtr(this.VTable, 168));
			this._GetItemDefinitionProperty = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetItemDefinitionProperty>(Marshal.ReadIntPtr(this.VTable, 176));
			this._RequestEligiblePromoItemDefinitionsIDs = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FRequestEligiblePromoItemDefinitionsIDs>(Marshal.ReadIntPtr(this.VTable, 184));
			this._GetEligiblePromoItemDefinitionIDs = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetEligiblePromoItemDefinitionIDs>(Marshal.ReadIntPtr(this.VTable, 192));
			this._StartPurchase = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FStartPurchase>(Marshal.ReadIntPtr(this.VTable, 200));
			this._RequestPrices = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FRequestPrices>(Marshal.ReadIntPtr(this.VTable, 208));
			this._GetNumItemsWithPrices = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetNumItemsWithPrices>(Marshal.ReadIntPtr(this.VTable, 216));
			this._GetItemsWithPrices = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetItemsWithPrices>(Marshal.ReadIntPtr(this.VTable, 224));
			this._GetItemPrice = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FGetItemPrice>(Marshal.ReadIntPtr(this.VTable, 232));
			this._StartUpdateProperties = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FStartUpdateProperties>(Marshal.ReadIntPtr(this.VTable, 240));
			this._RemoveProperty = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FRemoveProperty>(Marshal.ReadIntPtr(this.VTable, 248));
			this._SetProperty1 = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FSetProperty1>(Marshal.ReadIntPtr(this.VTable, 256));
			this._SetProperty2 = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FSetProperty2>(Marshal.ReadIntPtr(this.VTable, 264));
			this._SetProperty3 = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FSetProperty3>(Marshal.ReadIntPtr(this.VTable, 272));
			this._SetProperty4 = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FSetProperty4>(Marshal.ReadIntPtr(this.VTable, 280));
			this._SubmitUpdateProperties = Marshal.GetDelegateForFunctionPointer<ISteamInventory.FSubmitUpdateProperties>(Marshal.ReadIntPtr(this.VTable, 288));
		}

		internal bool LoadItemDefinitions()
		{
			return this._LoadItemDefinitions(this.Self);
		}

		internal bool RemoveProperty(SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, string pchPropertyName)
		{
			return this._RemoveProperty(this.Self, handle, nItemID, pchPropertyName);
		}

		internal async Task<SteamInventoryEligiblePromoItemDefIDs_t?> RequestEligiblePromoItemDefinitionsIDs(SteamId steamID)
		{
			SteamInventoryEligiblePromoItemDefIDs_t? resultAsync = await SteamInventoryEligiblePromoItemDefIDs_t.GetResultAsync(this._RequestEligiblePromoItemDefinitionsIDs(this.Self, steamID));
			return resultAsync;
		}

		internal async Task<SteamInventoryRequestPricesResult_t?> RequestPrices()
		{
			SteamInventoryRequestPricesResult_t? resultAsync = await SteamInventoryRequestPricesResult_t.GetResultAsync(this._RequestPrices(this.Self));
			return resultAsync;
		}

		internal void SendItemDropHeartbeat()
		{
			this._SendItemDropHeartbeat(this.Self);
		}

		internal bool SerializeResult(SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, ref uint punOutBufferSize)
		{
			return this._SerializeResult(this.Self, resultHandle, pOutBuffer, ref punOutBufferSize);
		}

		internal bool SetProperty1(SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, string pchPropertyName, string pchPropertyValue)
		{
			bool self = this._SetProperty1(this.Self, handle, nItemID, pchPropertyName, pchPropertyValue);
			return self;
		}

		internal bool SetProperty2(SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, string pchPropertyName, bool bValue)
		{
			bool self = this._SetProperty2(this.Self, handle, nItemID, pchPropertyName, bValue);
			return self;
		}

		internal bool SetProperty3(SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, string pchPropertyName, long nValue)
		{
			bool self = this._SetProperty3(this.Self, handle, nItemID, pchPropertyName, nValue);
			return self;
		}

		internal bool SetProperty4(SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, string pchPropertyName, float flValue)
		{
			bool self = this._SetProperty4(this.Self, handle, nItemID, pchPropertyName, flValue);
			return self;
		}

		internal override void Shutdown()
		{
			base.Shutdown();
			this._GetResultStatus = null;
			this._GetResultItems = null;
			this._GetResultItems_Windows = null;
			this._GetResultItemProperty = null;
			this._GetResultTimestamp = null;
			this._CheckResultSteamID = null;
			this._DestroyResult = null;
			this._GetAllItems = null;
			this._GetItemsByID = null;
			this._SerializeResult = null;
			this._DeserializeResult = null;
			this._GenerateItems = null;
			this._GrantPromoItems = null;
			this._AddPromoItem = null;
			this._AddPromoItems = null;
			this._ConsumeItem = null;
			this._ExchangeItems = null;
			this._TransferItemQuantity = null;
			this._SendItemDropHeartbeat = null;
			this._TriggerItemDrop = null;
			this._TradeItems = null;
			this._LoadItemDefinitions = null;
			this._GetItemDefinitionIDs = null;
			this._GetItemDefinitionProperty = null;
			this._RequestEligiblePromoItemDefinitionsIDs = null;
			this._GetEligiblePromoItemDefinitionIDs = null;
			this._StartPurchase = null;
			this._RequestPrices = null;
			this._GetNumItemsWithPrices = null;
			this._GetItemsWithPrices = null;
			this._GetItemPrice = null;
			this._StartUpdateProperties = null;
			this._RemoveProperty = null;
			this._SetProperty1 = null;
			this._SetProperty2 = null;
			this._SetProperty3 = null;
			this._SetProperty4 = null;
			this._SubmitUpdateProperties = null;
		}

		internal async Task<SteamInventoryStartPurchaseResult_t?> StartPurchase([In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] uint[] punArrayQuantity, uint unArrayLength)
		{
			SteamInventoryStartPurchaseResult_t? resultAsync = await SteamInventoryStartPurchaseResult_t.GetResultAsync(this._StartPurchase(this.Self, pArrayItemDefs, punArrayQuantity, unArrayLength));
			return resultAsync;
		}

		internal SteamInventoryUpdateHandle_t StartUpdateProperties()
		{
			return this._StartUpdateProperties(this.Self);
		}

		internal bool SubmitUpdateProperties(SteamInventoryUpdateHandle_t handle, ref SteamInventoryResult_t pResultHandle)
		{
			return this._SubmitUpdateProperties(this.Self, handle, ref pResultHandle);
		}

		internal bool TradeItems(ref SteamInventoryResult_t pResultHandle, SteamId steamIDTradePartner, [In][Out] InventoryItemId[] pArrayGive, [In][Out] uint[] pArrayGiveQuantity, uint nArrayGiveLength, [In][Out] InventoryItemId[] pArrayGet, [In][Out] uint[] pArrayGetQuantity, uint nArrayGetLength)
		{
			bool self = this._TradeItems(this.Self, ref pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
			return self;
		}

		internal bool TransferItemQuantity(ref SteamInventoryResult_t pResultHandle, InventoryItemId itemIdSource, uint unQuantity, InventoryItemId itemIdDest)
		{
			bool self = this._TransferItemQuantity(this.Self, ref pResultHandle, itemIdSource, unQuantity, itemIdDest);
			return self;
		}

		internal bool TriggerItemDrop(ref SteamInventoryResult_t pResultHandle, InventoryDefId dropListDefinition)
		{
			return this._TriggerItemDrop(this.Self, ref pResultHandle, dropListDefinition);
		}

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FAddPromoItem(IntPtr self, ref SteamInventoryResult_t pResultHandle, InventoryDefId itemDef);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FAddPromoItems(IntPtr self, ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayItemDefs, uint unArrayLength);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FCheckResultSteamID(IntPtr self, SteamInventoryResult_t resultHandle, SteamId steamIDExpected);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FConsumeItem(IntPtr self, ref SteamInventoryResult_t pResultHandle, InventoryItemId itemConsume, uint unQuantity);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FDeserializeResult(IntPtr self, ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FDestroyResult(IntPtr self, SteamInventoryResult_t resultHandle);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FExchangeItems(IntPtr self, ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayGenerate, [In][Out] uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, [In][Out] InventoryItemId[] pArrayDestroy, [In][Out] uint[] punArrayDestroyQuantity, uint unArrayDestroyLength);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGenerateItems(IntPtr self, ref SteamInventoryResult_t pResultHandle, [In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] uint[] punArrayQuantity, uint unArrayLength);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetAllItems(IntPtr self, ref SteamInventoryResult_t pResultHandle);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetEligiblePromoItemDefinitionIDs(IntPtr self, SteamId steamID, [In][Out] InventoryDefId[] pItemDefIDs, ref uint punItemDefIDsArraySize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetItemDefinitionIDs(IntPtr self, [In][Out] InventoryDefId[] pItemDefIDs, ref uint punItemDefIDsArraySize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetItemDefinitionProperty(IntPtr self, InventoryDefId iDefinition, string pchPropertyName, StringBuilder pchValueBuffer, ref uint punValueBufferSizeOut);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetItemPrice(IntPtr self, InventoryDefId iDefinition, ref ulong pCurrentPrice, ref ulong pBasePrice);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetItemsByID(IntPtr self, ref SteamInventoryResult_t pResultHandle, ref InventoryItemId pInstanceIDs, uint unCountInstanceIDs);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetItemsWithPrices(IntPtr self, [In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] ulong[] pCurrentPrices, [In][Out] ulong[] pBasePrices, uint unArrayLength);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetNumItemsWithPrices(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetResultItemProperty(IntPtr self, SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, StringBuilder pchValueBuffer, ref uint punValueBufferSizeOut);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetResultItems(IntPtr self, SteamInventoryResult_t resultHandle, [In][Out] SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGetResultItems_Windows(IntPtr self, SteamInventoryResult_t resultHandle, [In][Out] SteamItemDetails_t.Pack8[] pOutItemsArray, ref uint punOutItemsArraySize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate Result FGetResultStatus(IntPtr self, SteamInventoryResult_t resultHandle);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint FGetResultTimestamp(IntPtr self, SteamInventoryResult_t resultHandle);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FGrantPromoItems(IntPtr self, ref SteamInventoryResult_t pResultHandle);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FLoadItemDefinitions(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FRemoveProperty(IntPtr self, SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, string pchPropertyName);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRequestEligiblePromoItemDefinitionsIDs(IntPtr self, SteamId steamID);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FRequestPrices(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void FSendItemDropHeartbeat(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSerializeResult(IntPtr self, SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, ref uint punOutBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetProperty1(IntPtr self, SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, string pchPropertyName, string pchPropertyValue);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetProperty2(IntPtr self, SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, string pchPropertyName, bool bValue);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetProperty3(IntPtr self, SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, string pchPropertyName, long nValue);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSetProperty4(IntPtr self, SteamInventoryUpdateHandle_t handle, InventoryItemId nItemID, string pchPropertyName, float flValue);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamAPICall_t FStartPurchase(IntPtr self, [In][Out] InventoryDefId[] pArrayItemDefs, [In][Out] uint[] punArrayQuantity, uint unArrayLength);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate SteamInventoryUpdateHandle_t FStartUpdateProperties(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FSubmitUpdateProperties(IntPtr self, SteamInventoryUpdateHandle_t handle, ref SteamInventoryResult_t pResultHandle);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FTradeItems(IntPtr self, ref SteamInventoryResult_t pResultHandle, SteamId steamIDTradePartner, [In][Out] InventoryItemId[] pArrayGive, [In][Out] uint[] pArrayGiveQuantity, uint nArrayGiveLength, [In][Out] InventoryItemId[] pArrayGet, [In][Out] uint[] pArrayGetQuantity, uint nArrayGetLength);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FTransferItemQuantity(IntPtr self, ref SteamInventoryResult_t pResultHandle, InventoryItemId itemIdSource, uint unQuantity, InventoryItemId itemIdDest);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate bool FTriggerItemDrop(IntPtr self, ref SteamInventoryResult_t pResultHandle, InventoryDefId dropListDefinition);
	}
}