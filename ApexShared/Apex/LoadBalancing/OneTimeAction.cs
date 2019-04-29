using Apex.Utilities;
using System;

namespace Apex.LoadBalancing
{
	public class OneTimeAction : ILoadBalanced
	{
		private Action<float> _action;

		public bool repeat
		{
			get
			{
				return false;
			}
		}

		public OneTimeAction(Action<float> action)
		{
			Ensure.ArgumentNotNull(action, "action");
			this._action = action;
		}

		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			this._action(deltaTime);
			return null;
		}
	}
}