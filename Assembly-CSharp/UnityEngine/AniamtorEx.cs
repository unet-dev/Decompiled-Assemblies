using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class AniamtorEx
	{
		public static void SetFloatFixed(this Animator animator, int id, float value, float dampTime, float deltaTime)
		{
			if (value == 0f)
			{
				float num = animator.GetFloat(id);
				if (num == 0f)
				{
					return;
				}
				if (Mathf.Max(new float[] { num }) < 1.401298E-45f)
				{
					animator.SetFloat(id, 0f);
					return;
				}
			}
			animator.SetFloat(id, value, deltaTime, deltaTime);
		}
	}
}