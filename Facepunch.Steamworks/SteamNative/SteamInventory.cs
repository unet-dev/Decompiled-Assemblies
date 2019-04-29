using Facepunch.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SteamNative
{
	internal class SteamInventory : IDisposable
	{
		internal Platform.Interface platform;

		internal BaseSteamworks steamworks;

		public bool IsValid
		{
			get
			{
				if (this.platform == null)
				{
					return false;
				}
				return this.platform.IsValid;
			}
		}

		internal SteamInventory(BaseSteamworks steamworks, IntPtr pointer)
		{
			this.steamworks = steamworks;
			if (Platform.IsWindows64)
			{
				this.platform = new Platform.Win64(pointer);
				return;
			}
			if (Platform.IsWindows32)
			{
				this.platform = new Platform.Win32(pointer);
				return;
			}
			if (Platform.IsLinux32)
			{
				this.platform = new Platform.Linux32(pointer);
				return;
			}
			if (Platform.IsLinux64)
			{
				this.platform = new Platform.Linux64(pointer);
				return;
			}
			if (Platform.IsOsx)
			{
				this.platform = new Platform.Mac(pointer);
			}
		}

		public bool AddPromoItem(ref SteamInventoryResult_t pResultHandle, SteamItemDef_t itemDef)
		{
			return this.platform.ISteamInventory_AddPromoItem(ref pResultHandle.Value, itemDef.Value);
		}

		public bool AddPromoItems(ref SteamInventoryResult_t pResultHandle, SteamItemDef_t[] pArrayItemDefs, uint unArrayLength)
		{
			return this.platform.ISteamInventory_AddPromoItems(ref pResultHandle.Value, (
				from x in (IEnumerable<SteamItemDef_t>)pArrayItemDefs
				select x.Value).ToArray<int>(), unArrayLength);
		}

		public bool CheckResultSteamID(SteamInventoryResult_t resultHandle, CSteamID steamIDExpected)
		{
			return this.platform.ISteamInventory_CheckResultSteamID(resultHandle.Value, steamIDExpected.Value);
		}

		public bool ConsumeItem(ref SteamInventoryResult_t pResultHandle, SteamItemInstanceID_t itemConsume, uint unQuantity)
		{
			return this.platform.ISteamInventory_ConsumeItem(ref pResultHandle.Value, itemConsume.Value, unQuantity);
		}

		public bool DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE)
		{
			return this.platform.ISteamInventory_DeserializeResult(ref pOutResultHandle.Value, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
		}

		public void DestroyResult(SteamInventoryResult_t resultHandle)
		{
			this.platform.ISteamInventory_DestroyResult(resultHandle.Value);
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public bool ExchangeItems(ref SteamInventoryResult_t pResultHandle, SteamItemDef_t[] pArrayGenerate, uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, SteamItemInstanceID_t[] pArrayDestroy, uint[] punArrayDestroyQuantity, uint unArrayDestroyLength)
		{
			Platform.Interface @interface = this.platform;
			int[] array = (
				from x in (IEnumerable<SteamItemDef_t>)pArrayGenerate
				select x.Value).ToArray<int>();
			return @interface.ISteamInventory_ExchangeItems(ref pResultHandle.Value, array, punArrayGenerateQuantity, unArrayGenerateLength, (
				from x in (IEnumerable<SteamItemInstanceID_t>)pArrayDestroy
				select x.Value).ToArray<ulong>(), punArrayDestroyQuantity, unArrayDestroyLength);
		}

		public bool GenerateItems(ref SteamInventoryResult_t pResultHandle, SteamItemDef_t[] pArrayItemDefs, uint[] punArrayQuantity, uint unArrayLength)
		{
			return this.platform.ISteamInventory_GenerateItems(ref pResultHandle.Value, (
				from x in (IEnumerable<SteamItemDef_t>)pArrayItemDefs
				select x.Value).ToArray<int>(), punArrayQuantity, unArrayLength);
		}

		public bool GetAllItems(ref SteamInventoryResult_t pResultHandle)
		{
			return this.platform.ISteamInventory_GetAllItems(ref pResultHandle.Value);
		}

		public unsafe SteamItemDef_t[] GetEligiblePromoItemDefinitionIDs(CSteamID steamID)
		{
			// 
			// Current member / type: SteamNative.SteamItemDef_t[] SteamNative.SteamInventory::GetEligiblePromoItemDefinitionIDs(SteamNative.CSteamID)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: SteamNative.SteamItemDef_t[] GetEligiblePromoItemDefinitionIDs(SteamNative.CSteamID)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public unsafe SteamItemDef_t[] GetItemDefinitionIDs()
		{
			// 
			// Current member / type: SteamNative.SteamItemDef_t[] SteamNative.SteamInventory::GetItemDefinitionIDs()
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: SteamNative.SteamItemDef_t[] GetItemDefinitionIDs()
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public bool GetItemDefinitionProperty(SteamItemDef_t iDefinition, string pchPropertyName, out string pchValueBuffer)
		{
			bool flag = false;
			pchValueBuffer = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			uint num = 4096;
			flag = this.platform.ISteamInventory_GetItemDefinitionProperty(iDefinition.Value, pchPropertyName, stringBuilder, out num);
			if (!flag)
			{
				return flag;
			}
			pchValueBuffer = stringBuilder.ToString();
			return flag;
		}

		public bool GetItemPrice(SteamItemDef_t iDefinition, out ulong pPrice)
		{
			return this.platform.ISteamInventory_GetItemPrice(iDefinition.Value, out pPrice);
		}

		public bool GetItemsByID(ref SteamInventoryResult_t pResultHandle, SteamItemInstanceID_t[] pInstanceIDs, uint unCountInstanceIDs)
		{
			return this.platform.ISteamInventory_GetItemsByID(ref pResultHandle.Value, (
				from x in (IEnumerable<SteamItemInstanceID_t>)pInstanceIDs
				select x.Value).ToArray<ulong>(), unCountInstanceIDs);
		}

		public bool GetItemsWithPrices(IntPtr pArrayItemDefs, IntPtr pPrices, uint unArrayLength)
		{
			return this.platform.ISteamInventory_GetItemsWithPrices(pArrayItemDefs, pPrices, unArrayLength);
		}

		public uint GetNumItemsWithPrices()
		{
			return this.platform.ISteamInventory_GetNumItemsWithPrices();
		}

		public bool GetResultItemProperty(SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, out string pchValueBuffer)
		{
			bool flag = false;
			pchValueBuffer = string.Empty;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			uint num = 4096;
			flag = this.platform.ISteamInventory_GetResultItemProperty(resultHandle.Value, unItemIndex, pchPropertyName, stringBuilder, out num);
			if (!flag)
			{
				return flag;
			}
			pchValueBuffer = stringBuilder.ToString();
			return flag;
		}

		public unsafe SteamItemDetails_t[] GetResultItems(SteamInventoryResult_t resultHandle)
		{
			// 
			// Current member / type: SteamNative.SteamItemDetails_t[] SteamNative.SteamInventory::GetResultItems(SteamNative.SteamInventoryResult_t)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: SteamNative.SteamItemDetails_t[] GetResultItems(SteamNative.SteamInventoryResult_t)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public Result GetResultStatus(SteamInventoryResult_t resultHandle)
		{
			return this.platform.ISteamInventory_GetResultStatus(resultHandle.Value);
		}

		public uint GetResultTimestamp(SteamInventoryResult_t resultHandle)
		{
			return this.platform.ISteamInventory_GetResultTimestamp(resultHandle.Value);
		}

		public bool GrantPromoItems(ref SteamInventoryResult_t pResultHandle)
		{
			return this.platform.ISteamInventory_GrantPromoItems(ref pResultHandle.Value);
		}

		public bool LoadItemDefinitions()
		{
			return this.platform.ISteamInventory_LoadItemDefinitions();
		}

		public bool RemoveProperty(SteamInventoryUpdateHandle_t handle, SteamItemInstanceID_t nItemID, string pchPropertyName)
		{
			return this.platform.ISteamInventory_RemoveProperty(handle.Value, nItemID.Value, pchPropertyName);
		}

		public CallbackHandle RequestEligiblePromoItemDefinitionsIDs(CSteamID steamID, Action<SteamInventoryEligiblePromoItemDefIDs_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamInventory_RequestEligiblePromoItemDefinitionsIDs(steamID.Value);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return SteamInventoryEligiblePromoItemDefIDs_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public CallbackHandle RequestPrices(Action<SteamInventoryRequestPricesResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamInventory_RequestPrices();
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return SteamInventoryRequestPricesResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public void SendItemDropHeartbeat()
		{
			this.platform.ISteamInventory_SendItemDropHeartbeat();
		}

		public bool SerializeResult(SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, out uint punOutBufferSize)
		{
			return this.platform.ISteamInventory_SerializeResult(resultHandle.Value, pOutBuffer, out punOutBufferSize);
		}

		public bool SetProperty(SteamInventoryUpdateHandle_t handle, SteamItemInstanceID_t nItemID, string pchPropertyName, string pchPropertyValue)
		{
			return this.platform.ISteamInventory_SetProperty(handle.Value, nItemID.Value, pchPropertyName, pchPropertyValue);
		}

		public bool SetProperty0(SteamInventoryUpdateHandle_t handle, SteamItemInstanceID_t nItemID, string pchPropertyName, bool bValue)
		{
			return this.platform.ISteamInventory_SetProperty0(handle.Value, nItemID.Value, pchPropertyName, bValue);
		}

		public bool SetProperty1(SteamInventoryUpdateHandle_t handle, SteamItemInstanceID_t nItemID, string pchPropertyName, long nValue)
		{
			return this.platform.ISteamInventory_SetProperty0(handle.Value, nItemID.Value, pchPropertyName, nValue);
		}

		public bool SetProperty2(SteamInventoryUpdateHandle_t handle, SteamItemInstanceID_t nItemID, string pchPropertyName, float flValue)
		{
			return this.platform.ISteamInventory_SetProperty0(handle.Value, nItemID.Value, pchPropertyName, flValue);
		}

		public CallbackHandle StartPurchase(SteamItemDef_t[] pArrayItemDefs, uint[] punArrayQuantity, uint unArrayLength, Action<SteamInventoryStartPurchaseResult_t, bool> CallbackFunction = null)
		{
			SteamAPICall_t steamAPICallT = (long)0;
			steamAPICallT = this.platform.ISteamInventory_StartPurchase((
				from x in (IEnumerable<SteamItemDef_t>)pArrayItemDefs
				select x.Value).ToArray<int>(), punArrayQuantity, unArrayLength);
			if (CallbackFunction == null)
			{
				return null;
			}
			if (steamAPICallT == 0)
			{
				return null;
			}
			return SteamInventoryStartPurchaseResult_t.CallResult(this.steamworks, steamAPICallT, CallbackFunction);
		}

		public SteamInventoryUpdateHandle_t StartUpdateProperties()
		{
			return this.platform.ISteamInventory_StartUpdateProperties();
		}

		public bool SubmitUpdateProperties(SteamInventoryUpdateHandle_t handle, ref SteamInventoryResult_t pResultHandle)
		{
			return this.platform.ISteamInventory_SubmitUpdateProperties(handle.Value, ref pResultHandle.Value);
		}

		public bool TradeItems(ref SteamInventoryResult_t pResultHandle, CSteamID steamIDTradePartner, SteamItemInstanceID_t[] pArrayGive, uint[] pArrayGiveQuantity, uint nArrayGiveLength, SteamItemInstanceID_t[] pArrayGet, uint[] pArrayGetQuantity, uint nArrayGetLength)
		{
			Platform.Interface @interface = this.platform;
			ulong value = steamIDTradePartner.Value;
			ulong[] array = (
				from x in (IEnumerable<SteamItemInstanceID_t>)pArrayGive
				select x.Value).ToArray<ulong>();
			return @interface.ISteamInventory_TradeItems(ref pResultHandle.Value, value, array, pArrayGiveQuantity, nArrayGiveLength, (
				from x in (IEnumerable<SteamItemInstanceID_t>)pArrayGet
				select x.Value).ToArray<ulong>(), pArrayGetQuantity, nArrayGetLength);
		}

		public bool TransferItemQuantity(ref SteamInventoryResult_t pResultHandle, SteamItemInstanceID_t itemIdSource, uint unQuantity, SteamItemInstanceID_t itemIdDest)
		{
			return this.platform.ISteamInventory_TransferItemQuantity(ref pResultHandle.Value, itemIdSource.Value, unQuantity, itemIdDest.Value);
		}

		public bool TriggerItemDrop(ref SteamInventoryResult_t pResultHandle, SteamItemDef_t dropListDefinition)
		{
			return this.platform.ISteamInventory_TriggerItemDrop(ref pResultHandle.Value, dropListDefinition.Value);
		}
	}
}