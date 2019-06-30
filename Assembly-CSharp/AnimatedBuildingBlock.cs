using Rust;
using System;
using UnityEngine;

public class AnimatedBuildingBlock : StabilityEntity
{
	private bool animatorNeedsInitializing = true;

	private bool animatorIsOpen = true;

	private bool isAnimating;

	public AnimatedBuildingBlock()
	{
	}

	protected virtual void OnAnimatorDisabled()
	{
	}

	protected void OnAnimatorFinished()
	{
		if (!this.isAnimating)
		{
			this.PutAnimatorToSleep();
		}
		this.isAnimating = false;
	}

	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		this.UpdateAnimationParameters(false);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateAnimationParameters(true);
	}

	private void PutAnimatorToSleep()
	{
		if (!this.model || !this.model.animator)
		{
			Debug.LogWarning(string.Concat(base.transform.GetRecursiveName(""), " has missing model/animator"), base.gameObject);
			return;
		}
		this.model.animator.enabled = false;
		if (base.isServer)
		{
			base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
		}
		this.OnAnimatorDisabled();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.UpdateAnimationParameters(true);
		}
	}

	public override bool SupportsChildDeployables()
	{
		return false;
	}

	protected void UpdateAnimationParameters(bool init)
	{
		bool flag;
		if (!this.model)
		{
			return;
		}
		if (!this.model.animator)
		{
			return;
		}
		if (!this.model.animator.isInitialized)
		{
			return;
		}
		if (this.animatorNeedsInitializing || this.animatorIsOpen != base.IsOpen())
		{
			flag = true;
		}
		else
		{
			flag = (!init ? false : this.isAnimating);
		}
		bool flag1 = this.animatorNeedsInitializing | init;
		if (flag)
		{
			this.isAnimating = true;
			this.model.animator.enabled = true;
			Animator animator = this.model.animator;
			bool flag2 = base.IsOpen();
			bool flag3 = flag2;
			this.animatorIsOpen = flag2;
			animator.SetBool("open", flag3);
			if (!flag1)
			{
				this.model.animator.fireEvents = base.isClient;
				if (base.isServer)
				{
					base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
				}
			}
			else
			{
				this.model.animator.fireEvents = false;
				if (this.model.animator.isActiveAndEnabled)
				{
					for (int i = 0; (float)i < 20f; i++)
					{
						this.model.animator.Update(1f);
					}
				}
				this.PutAnimatorToSleep();
			}
		}
		else if (flag1)
		{
			this.PutAnimatorToSleep();
		}
		this.animatorNeedsInitializing = false;
	}
}