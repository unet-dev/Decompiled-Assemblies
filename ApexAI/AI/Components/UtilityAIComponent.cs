using Apex;
using Apex.AI;
using Apex.Utilities;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Apex.AI.Components
{
	[AddComponentMenu("")]
	[ApexComponent("AI")]
	public class UtilityAIComponent : ExtendedMonoBehaviour
	{
		[HideInInspector]
		[SerializeField]
		internal UtilityAIConfig[] aiConfigs;

		private IUtilityAIClient[] _clients;

		private int _usedClients;

		public IUtilityAIClient[] clients
		{
			get
			{
				if (this._clients == null)
				{
					IContextProvider contextProvider = this.As<IContextProvider>(false, false);
					if (contextProvider == null)
					{
						Debug.LogWarning(string.Concat(base.gameObject.name, ": No AI context provider was found."));
						this._clients = Empty<IUtilityAIClient>.array;
						this._usedClients = 0;
						return this._clients;
					}
					if (this.aiConfigs == null)
					{
						this._clients = Empty<IUtilityAIClient>.array;
						this._usedClients = 0;
					}
					else
					{
						this._clients = new IUtilityAIClient[(int)this.aiConfigs.Length];
						for (int i = 0; i < (int)this.aiConfigs.Length; i++)
						{
							UtilityAIConfig utilityAIConfig = this.aiConfigs[i];
							IUtilityAI aI = AIManager.GetAI(new Guid(utilityAIConfig.aiId));
							if (aI != null)
							{
								this._clients[i] = new LoadBalancedUtilityAIClient(aI, contextProvider, utilityAIConfig.intervalMin, utilityAIConfig.intervalMax, utilityAIConfig.startDelayMin, utilityAIConfig.startDelayMax);
								this._usedClients++;
							}
							else
							{
								Debug.LogWarning(string.Concat(base.gameObject.name, ": Unable to load AI, no AI with the specified ID exists."));
								this._clients[i] = null;
							}
						}
					}
				}
				return this._clients;
			}
		}

		public UtilityAIComponent()
		{
		}

		public int AddClient(string aiId)
		{
			return this.AddClient(new Guid(aiId));
		}

		public int AddClient(Guid aiId)
		{
			IContextProvider contextProvider = this.As<IContextProvider>(false, false);
			if (contextProvider != null)
			{
				return this.AddClient(aiId, contextProvider);
			}
			Debug.LogWarning(string.Concat(base.gameObject.name, ": No AI context provider was found."));
			return -1;
		}

		public int AddClient(Guid aiId, IContextProvider contextProvider)
		{
			return this.AddClient(aiId, contextProvider, 1f, 1f, 0f, 0f);
		}

		public int AddClient(Guid aiId, IContextProvider contextProvider, float intervalMin, float intervalMax, float startDelayMin, float startDelayMax)
		{
			if (AIManager.GetAI(aiId) == null)
			{
				Debug.LogWarning(string.Concat(base.gameObject.name, ": Unable to load AI, no AI with the specified ID exists."));
				return -1;
			}
			UtilityAIConfig utilityAIConfig = new UtilityAIConfig()
			{
				aiId = aiId.ToString(),
				intervalMin = intervalMin,
				intervalMax = intervalMax,
				startDelayMin = startDelayMin,
				startDelayMax = startDelayMax,
				isActive = true
			};
			if (this._usedClients == (int)this.aiConfigs.Length)
			{
				this.Resize<UtilityAIConfig>(ref this.aiConfigs, Mathf.Max(2, (int)this.aiConfigs.Length * 2));
			}
			this.aiConfigs[this._usedClients] = utilityAIConfig;
			LoadBalancedUtilityAIClient loadBalancedUtilityAIClient = new LoadBalancedUtilityAIClient(aiId, contextProvider, intervalMin, intervalMax, startDelayMin, startDelayMax);
			return this.AddClient(loadBalancedUtilityAIClient);
		}

		private int AddClient(IUtilityAIClient client)
		{
			IUtilityAIClient[] utilityAIClientArray = this.clients;
			if (this._usedClients == (int)utilityAIClientArray.Length)
			{
				this.Resize<IUtilityAIClient>(ref this._clients, Mathf.Max(2, (int)utilityAIClientArray.Length * 2));
			}
			IUtilityAIClient[] utilityAIClientArray1 = this._clients;
			int num = this._usedClients;
			this._usedClients = num + 1;
			utilityAIClientArray1[num] = client;
			if (this.OnNewAI != null)
			{
				this.OnNewAI(client);
			}
			return this._usedClients - 1;
		}

		public IUtilityAIClient GetClient(Guid aiId)
		{
			IUtilityAIClient[] utilityAIClientArray = this.clients;
			for (int i = 0; i < (int)utilityAIClientArray.Length; i++)
			{
				if (utilityAIClientArray[i] != null && utilityAIClientArray[i].ai.id == aiId)
				{
					return utilityAIClientArray[i];
				}
			}
			return null;
		}

		private void OnDisable()
		{
			IUtilityAIClient[] utilityAIClientArray = this.clients;
			for (int i = 0; i < (int)utilityAIClientArray.Length; i++)
			{
				if (utilityAIClientArray[i] != null)
				{
					utilityAIClientArray[i].Stop();
				}
			}
		}

		protected override void OnStartAndEnable()
		{
			IUtilityAIClient[] utilityAIClientArray = this.clients;
			for (int i = 0; i < (int)utilityAIClientArray.Length; i++)
			{
				if (utilityAIClientArray[i] != null && this.aiConfigs[i].isActive)
				{
					utilityAIClientArray[i].Start();
				}
			}
		}

		public void Pause()
		{
			IUtilityAIClient[] utilityAIClientArray = this.clients;
			for (int i = 0; i < (int)utilityAIClientArray.Length; i++)
			{
				if (utilityAIClientArray[i] != null)
				{
					utilityAIClientArray[i].Pause();
				}
			}
		}

		public bool RemoveClient(Guid aiId)
		{
			IUtilityAIClient[] utilityAIClientArray = this.clients;
			for (int i = 0; i < (int)utilityAIClientArray.Length; i++)
			{
				if (utilityAIClientArray[i] != null && utilityAIClientArray[i].ai.id == aiId)
				{
					this.RemoveClientAt(i);
					return true;
				}
			}
			return false;
		}

		public bool RemoveClient(IUtilityAIClient client)
		{
			IUtilityAIClient[] utilityAIClientArray = this.clients;
			for (int i = 0; i < (int)utilityAIClientArray.Length; i++)
			{
				if (utilityAIClientArray[i] != null && utilityAIClientArray[i].Equals(client))
				{
					this.RemoveClientAt(i);
					return true;
				}
			}
			return false;
		}

		public void RemoveClientAt(int index)
		{
			IUtilityAIClient[] utilityAIClientArray = this.clients;
			if (index < 0 || index >= (int)utilityAIClientArray.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int num = this._usedClients - 1;
			if (index < num)
			{
				utilityAIClientArray[index] = utilityAIClientArray[num];
			}
			utilityAIClientArray[num] = null;
			this._usedClients--;
		}

		private void Resize<T>(ref T[] array, int newCapacity)
		{
			T[] tArray = new T[newCapacity];
			Array.Copy(array, 0, tArray, 0, (int)array.Length);
			array = tArray;
		}

		public void Resume()
		{
			IUtilityAIClient[] utilityAIClientArray = this.clients;
			for (int i = 0; i < (int)utilityAIClientArray.Length; i++)
			{
				if (utilityAIClientArray[i] != null)
				{
					utilityAIClientArray[i].Resume();
				}
			}
		}

		internal void ToggleActive(int idx, bool active)
		{
			if (this.aiConfigs[idx].isActive == active)
			{
				return;
			}
			this.aiConfigs[idx].isActive = active;
			if (Application.isPlaying)
			{
				if (active)
				{
					this.clients[idx].Start();
					return;
				}
				this.clients[idx].Stop();
			}
		}

		internal event Action<IUtilityAIClient> OnNewAI;
	}
}