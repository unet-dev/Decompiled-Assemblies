using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks
{
	public struct InventoryResult : IDisposable
	{
		internal SteamInventoryResult_t _id;

		public bool Expired
		{
			get;
			internal set;
		}

		public int ItemCount
		{
			get
			{
				int num;
				uint num1 = 0;
				if (SteamInventory.Internal.GetResultItems(this._id, null, ref num1))
				{
					num = (int)num1;
				}
				else
				{
					num = 0;
				}
				return num;
			}
		}

		internal InventoryResult(SteamInventoryResult_t id, bool expired)
		{
			this._id = id;
			this.Expired = expired;
		}

		public bool BelongsTo(SteamId steamId)
		{
			return SteamInventory.Internal.CheckResultSteamID(this._id, steamId);
		}

		public void Dispose()
		{
			if (this._id.Value != -1)
			{
				SteamInventory.Internal.DestroyResult(this._id);
			}
		}

		internal static async Task<InventoryResult?> GetAsync(SteamInventoryResult_t sresult)
		{
			InventoryResult? nullable;
			Result resultStatus = Result.Pending;
			while (resultStatus == Result.Pending)
			{
				resultStatus = SteamInventory.Internal.GetResultStatus(sresult);
				await Task.Delay(10);
			}
			if ((resultStatus == Result.OK ? true : resultStatus == Result.Expired))
			{
				nullable = new InventoryResult?(new InventoryResult(sresult, resultStatus == Result.Expired));
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public InventoryItem[] GetItems(bool includeProperties = false)
		{
			InventoryItem[] inventoryItemArray;
			uint itemCount = (uint)this.ItemCount;
			if (itemCount != 0)
			{
				SteamItemDetails_t[] steamItemDetailsTArray = new SteamItemDetails_t[itemCount];
				if (SteamInventory.Internal.GetResultItems(this._id, steamItemDetailsTArray, ref itemCount))
				{
					InventoryItem[] inventoryItemArray1 = new InventoryItem[itemCount];
					for (int i = 0; (long)i < (ulong)itemCount; i++)
					{
						InventoryItem properties = InventoryItem.From(steamItemDetailsTArray[i]);
						if (includeProperties)
						{
							properties._properties = InventoryItem.GetProperties(this._id, i);
						}
						inventoryItemArray1[i] = properties;
					}
					inventoryItemArray = inventoryItemArray1;
				}
				else
				{
					inventoryItemArray = null;
				}
			}
			else
			{
				inventoryItemArray = null;
			}
			return inventoryItemArray;
		}

		public unsafe byte[] Serialize()
		{
			// 
			// Current member / type: System.Byte[] Steamworks.InventoryResult::Serialize()
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Byte[] Serialize()
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
	}
}