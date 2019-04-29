using Apex.Utilities;
using System;
using System.Runtime.CompilerServices;

namespace Apex.LoadBalancing
{
	public class RepeatableAction : ILoadBalanced
	{
		private Func<float, bool> _action;

		private int _repetitions;

		private int _repetitionCount;

		public bool repeat
		{
			get
			{
				return JustDecompileGenerated_get_repeat();
			}
			set
			{
				JustDecompileGenerated_set_repeat(value);
			}
		}

		private bool JustDecompileGenerated_repeat_k__BackingField;

		public bool JustDecompileGenerated_get_repeat()
		{
			return this.JustDecompileGenerated_repeat_k__BackingField;
		}

		private void JustDecompileGenerated_set_repeat(bool value)
		{
			this.JustDecompileGenerated_repeat_k__BackingField = value;
		}

		public RepeatableAction(Func<float, bool> action) : this(action, -1)
		{
		}

		public RepeatableAction(Func<float, bool> action, int repetitions)
		{
			Ensure.ArgumentNotNull(action, "action");
			this._action = action;
			this._repetitions = repetitions;
		}

		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			this.repeat = this._action(deltaTime);
			if (this.repeat && this._repetitions > -1)
			{
				int num = this._repetitionCount;
				this._repetitionCount = num + 1;
				this.repeat = num < this._repetitions;
			}
			return null;
		}
	}
}