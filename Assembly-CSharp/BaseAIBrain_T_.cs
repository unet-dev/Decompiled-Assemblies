using System;
using UnityEngine;

public class BaseAIBrain<T> : EntityComponent<T>
where T : BaseEntity
{
	public BaseAIBrain<T>.BasicAIState[] AIStates;

	public const int AIStateIndex_UNSET = 0;

	public int _currentState;

	public Vector3 mainInterestPoint;

	public BaseAIBrain()
	{
	}

	public virtual void AddState(BaseAIBrain<T>.BasicAIState newState, int newIndex)
	{
		newState.SetIndex(newIndex);
		this.AIStates[newIndex] = newState;
		newState.brain = this;
		newState.Reset();
	}

	public virtual void AIThink(float delta)
	{
		BaseAIBrain<T>.BasicAIState currentState = this.GetCurrentState();
		if (currentState != null)
		{
			currentState.StateThink(delta);
		}
		if (currentState == null || currentState.CanInterrupt())
		{
			float single = 0f;
			int num = 0;
			BaseAIBrain<T>.BasicAIState basicAIState = null;
			for (int i = 0; i < (int)this.AIStates.Length; i++)
			{
				BaseAIBrain<T>.BasicAIState aIStates = this.AIStates[i];
				if (aIStates != null)
				{
					float weight = aIStates.GetWeight();
					if (weight > single)
					{
						single = weight;
						num = i;
						basicAIState = aIStates;
					}
				}
			}
			if (basicAIState != currentState)
			{
				this.SwitchToState(num);
			}
		}
	}

	public void Awake()
	{
		this.InitializeAI();
	}

	public virtual void DoThink()
	{
	}

	public BaseAIBrain<T>.BasicAIState GetCurrentState()
	{
		if (this.AIStates == null)
		{
			return null;
		}
		return this.AIStates[this._currentState];
	}

	public T GetEntity()
	{
		return base.baseEntity;
	}

	public BaseAIBrain<T>.BasicAIState GetState(int index)
	{
		return this.AIStates[index];
	}

	public virtual void InitializeAI()
	{
	}

	public virtual bool ShouldThink()
	{
		return true;
	}

	public void SwitchToState(int newState)
	{
		BaseAIBrain<T>.BasicAIState currentState = this.GetCurrentState();
		BaseAIBrain<T>.BasicAIState state = this.GetState(newState);
		if (currentState != null)
		{
			if (currentState == state)
			{
				return;
			}
			if (!currentState.CanInterrupt())
			{
				return;
			}
			currentState.StateLeave();
		}
		this._currentState = newState;
		state.StateEnter();
	}

	public class BasicAIState
	{
		private int myIndex;

		public BaseAIBrain<T> brain;

		protected float _timeInState;

		protected float _lastStateExitTime;

		public BasicAIState()
		{
		}

		public virtual bool CanInterrupt()
		{
			return true;
		}

		public virtual void DrawGizmos()
		{
		}

		public T GetEntity()
		{
			return this.brain.GetEntity();
		}

		public virtual float GetWeight()
		{
			return 0f;
		}

		public bool IsInState()
		{
			if (this.brain == null)
			{
				return false;
			}
			return this.brain.GetCurrentState() == this;
		}

		public void Reset()
		{
			this._timeInState = 0f;
		}

		public void SetIndex(int newIndex)
		{
			if (this.myIndex == 0)
			{
				this.myIndex = newIndex;
			}
		}

		public virtual void StateEnter()
		{
			this._timeInState = 0f;
		}

		public virtual void StateLeave()
		{
			this._timeInState = 0f;
			this._lastStateExitTime = Time.time;
		}

		public virtual void StateThink(float delta)
		{
			this._timeInState += delta;
		}

		public float TimeInState()
		{
			return this._timeInState;
		}

		public float TimeSinceState()
		{
			return Time.time - this._lastStateExitTime;
		}
	}
}