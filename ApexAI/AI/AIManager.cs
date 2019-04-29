using Apex.AI.Components;
using Apex.AI.Serialization;
using Apex.AI.Visualization;
using Apex.Serialization;
using Apex.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.AI
{
	public static class AIManager
	{
		public const string StorageFolder = "ApexAIStorage";

		private readonly static object initLock;

		private static Dictionary<Guid, AIManager.AIData> _aiLookup;

		private static Dictionary<Guid, List<IUtilityAIClient>> _aiClients;

		public static AIManager.AIClientResolver GetAIClient;

		public static IEnumerable<IUtilityAIClient> allClients
		{
			get
			{
				// 
				// Current member / type: System.Collections.Generic.IEnumerable`1<Apex.AI.Components.IUtilityAIClient> Apex.AI.AIManager::get_allClients()
				// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\ApexAI.dll
				// 
				// Product version: 2019.1.118.0
				// Exception in: System.Collections.Generic.IEnumerable<Apex.AI.Components.IUtilityAIClient> get_allClients()
				// 
				// Invalid state value
				//    at ..( , Queue`1 , ILogicalConstruct ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 203
				//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 187
				//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 129
				//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 76
				//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\LogicalFlowBuilderStep.cs:line 126
				//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\LogicalFlowBuilderStep.cs:line 51
				//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
				//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
				//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , DecompilationContext ,  , Func`2 , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 104
				//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , DecompilationContext , & ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 139
				//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildYieldStatementsStep.cs:line 134
				//    at ..Match( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildYieldStatementsStep.cs:line 49
				//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildYieldStatementsStep.cs:line 20
				//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
				//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\BlockDecompilationPipeline.cs:line 30
				//    at ..( , DecompilationContext , & ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:line 422
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

			}
		}

		static AIManager()
		{
			AIManager.initLock = new object();
			AIManager.GetAIClient = (GameObject host, Guid aiId) => {
				UtilityAIComponent component = host.GetComponent<UtilityAIComponent>();
				if (component == null)
				{
					return null;
				}
				return component.GetClient(aiId);
			};
		}

		public static void EagerLoadAll()
		{
			AIManager.EnsureLookup(true);
		}

		private static void EnsureLookup(bool init)
		{
			if (AIManager._aiLookup != null)
			{
				return;
			}
			lock (AIManager.initLock)
			{
				if (AIManager._aiLookup == null)
				{
					AIManager._aiLookup = new Dictionary<Guid, AIManager.AIData>();
				}
				else
				{
					return;
				}
			}
			AIStorage[] aIStorageArray = Resources.LoadAll<AIStorage>("ApexAIStorage");
			for (int i = 0; i < (int)aIStorageArray.Length; i++)
			{
				AIManager.AIData aIDatum = new AIManager.AIData()
				{
					storedData = aIStorageArray[i]
				};
				AIManager._aiLookup.Add(new Guid(aIDatum.storedData.aiId), aIDatum);
				if (init)
				{
					AIManager.ReadAndInit(aIDatum);
				}
			}
		}

		public static bool ExecuteAI(Guid id, IAIContext context)
		{
			IUtilityAI aI = AIManager.GetAI(id);
			if (aI == null)
			{
				return false;
			}
			return aI.ExecuteOnce(context);
		}

		public static IUtilityAI GetAI(Guid id)
		{
			AIManager.AIData aIDatum;
			AIManager.EnsureLookup(false);
			if (!AIManager._aiLookup.TryGetValue(id, out aIDatum))
			{
				return null;
			}
			if (aIDatum.ai == null)
			{
				lock (AIManager.initLock)
				{
					if (aIDatum.ai == null)
					{
						AIManager.ReadAndInit(aIDatum);
					}
				}
			}
			return aIDatum.ai;
		}

		public static IList<IUtilityAIClient> GetAIClients(Guid aiId)
		{
			List<IUtilityAIClient> utilityAIClients;
			if (AIManager._aiClients != null && AIManager._aiClients.TryGetValue(aiId, out utilityAIClients))
			{
				return utilityAIClients;
			}
			return Empty<IUtilityAIClient>.array;
		}

		private static void ReadAndInit(AIManager.AIData data)
		{
			List<IInitializeAfterDeserialization> initializeAfterDeserializations = new List<IInitializeAfterDeserialization>();
			try
			{
				data.ai = SerializationMaster.Deserialize<UtilityAI>(data.storedData.configuration, initializeAfterDeserializations);
				data.ai.name = data.storedData.name;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				UnityEngine.Debug.LogWarning(string.Format("Unable to load the AI: {0}. Additional details: {1}\n{2}", data.storedData.name, exception.Message, exception.StackTrace));
				return;
			}
			int count = initializeAfterDeserializations.Count;
			for (int i = 0; i < count; i++)
			{
				initializeAfterDeserializations[i].Initialize(data.ai);
			}
		}

		public static void Register(IUtilityAIClient client)
		{
			List<IUtilityAIClient> utilityAIClients;
			Guid guid = client.ai.id;
			if (AIManager._aiClients == null)
			{
				AIManager._aiClients = new Dictionary<Guid, List<IUtilityAIClient>>();
			}
			if (!AIManager._aiClients.TryGetValue(guid, out utilityAIClients))
			{
				utilityAIClients = new List<IUtilityAIClient>(1);
				AIManager._aiClients.Add(guid, utilityAIClients);
			}
			utilityAIClients.Add(client);
			if (Application.isEditor && VisualizationManager.isVisualizing && !(client.ai is UtilityAIVisualizer))
			{
				client.ai = new UtilityAIVisualizer(client.ai);
			}
		}

		public static void Unregister(IUtilityAIClient client)
		{
			List<IUtilityAIClient> utilityAIClients;
			if (AIManager._aiClients == null)
			{
				return;
			}
			Guid guid = client.ai.id;
			if (!AIManager._aiClients.TryGetValue(guid, out utilityAIClients))
			{
				return;
			}
			utilityAIClients.Remove(client);
			if (Application.isEditor && VisualizationManager.isVisualizing && client.ai is UtilityAIVisualizer)
			{
				((UtilityAIVisualizer)client.ai).Reset();
			}
		}

		public delegate IUtilityAIClient AIClientResolver(GameObject host, Guid aiId);

		private class AIData
		{
			public IUtilityAI ai;

			public AIStorage storedData;

			public AIData()
			{
			}
		}
	}
}