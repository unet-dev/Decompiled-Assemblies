using Rust;
using System;
using UnityEngine;

public class DoorAnimEvents : MonoBehaviour, IClientComponent
{
	public GameObjectRef openStart;

	public GameObjectRef openEnd;

	public GameObjectRef closeStart;

	public GameObjectRef closeEnd;

	public Animator animator
	{
		get
		{
			return base.GetComponent<Animator>();
		}
	}

	public DoorAnimEvents()
	{
	}

	private void DoorCloseEnd()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.closeEnd.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
		{
			return;
		}
		Effect.client.Run(this.closeEnd.resourcePath, base.gameObject);
	}

	private void DoorCloseStart()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.closeStart.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
		{
			return;
		}
		Effect.client.Run(this.closeStart.resourcePath, base.gameObject);
	}

	private void DoorOpenEnd()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.openEnd.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
		{
			return;
		}
		Effect.client.Run(this.openEnd.resourcePath, base.gameObject);
	}

	private void DoorOpenStart()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.openStart.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
		{
			return;
		}
		Effect.client.Run(this.openStart.resourcePath, base.gameObject);
	}
}