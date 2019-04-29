using Apex.AI;
using Apex.Utilities;
using System;
using System.Runtime.CompilerServices;

namespace Apex.AI.Components
{
	public abstract class UtilityAIClient : IUtilityAIClient
	{
		private IUtilityAI _ai;

		private IContextProvider _contextProvider;

		private IRequireTermination _activeAction;

		public IUtilityAI ai
		{
			get
			{
				return this._ai;
			}
			set
			{
				this._ai = value;
			}
		}

		public UtilityAIClientState state
		{
			get
			{
				return JustDecompileGenerated_get_state();
			}
			set
			{
				JustDecompileGenerated_set_state(value);
			}
		}

		private UtilityAIClientState JustDecompileGenerated_state_k__BackingField;

		public UtilityAIClientState JustDecompileGenerated_get_state()
		{
			return this.JustDecompileGenerated_state_k__BackingField;
		}

		protected void JustDecompileGenerated_set_state(UtilityAIClientState value)
		{
			this.JustDecompileGenerated_state_k__BackingField = value;
		}

		protected UtilityAIClient(Guid aiId, IContextProvider contextProvider)
		{
			Ensure.ArgumentNotNull(contextProvider, "contextProvider");
			this._ai = AIManager.GetAI(aiId);
			if (this._ai == null)
			{
				throw new ArgumentException("Unable to load associated AI.", "aiId");
			}
			this._contextProvider = contextProvider;
			this.state = UtilityAIClientState.Stopped;
		}

		protected UtilityAIClient(IUtilityAI ai, IContextProvider contextProvider)
		{
			Ensure.ArgumentNotNull(ai, "ai");
			Ensure.ArgumentNotNull(contextProvider, "contextProvider");
			this._ai = ai;
			this._contextProvider = contextProvider;
			this.state = UtilityAIClientState.Stopped;
		}

		public void Execute()
		{
			IAIContext context = this._contextProvider.GetContext(this._ai.id);
			IAction action = this._ai.Select(context);
			bool flag = false;
			while (!flag)
			{
				ICompositeAction compositeAction = action as ICompositeAction;
				if (compositeAction == null)
				{
					IConnectorAction connectorAction = action as IConnectorAction;
					if (connectorAction != null)
					{
						action = connectorAction.Select(context);
					}
					else
					{
						flag = true;
					}
				}
				else if (!compositeAction.isConnector)
				{
					flag = true;
				}
				else
				{
					action.Execute(context);
					action = compositeAction.Select(context);
				}
			}
			if (this._activeAction != null && this._activeAction != action)
			{
				this._activeAction.Terminate(context);
				this._activeAction = action as IRequireTermination;
			}
			else if (this._activeAction == null)
			{
				this._activeAction = action as IRequireTermination;
			}
			if (action != null)
			{
				action.Execute(context);
			}
		}

		protected abstract void OnPause();

		protected abstract void OnResume();

		protected abstract void OnStart();

		protected abstract void OnStop();

		public void Pause()
		{
			if (this.state != UtilityAIClientState.Running)
			{
				return;
			}
			this.state = UtilityAIClientState.Paused;
			this.OnPause();
		}

		public void Resume()
		{
			if (this.state != UtilityAIClientState.Paused)
			{
				return;
			}
			this.state = UtilityAIClientState.Running;
			this.OnResume();
		}

		public void Start()
		{
			if (this.state != UtilityAIClientState.Stopped)
			{
				return;
			}
			AIManager.Register(this);
			this.state = UtilityAIClientState.Running;
			this.OnStart();
		}

		public void Stop()
		{
			if (this.state == UtilityAIClientState.Stopped)
			{
				return;
			}
			AIManager.Unregister(this);
			this.state = UtilityAIClientState.Stopped;
			this.OnStop();
		}
	}
}