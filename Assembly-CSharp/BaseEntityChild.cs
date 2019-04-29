using Rust;
using Rust.Registry;
using System;
using UnityEngine;

public class BaseEntityChild : MonoBehaviour
{
	public BaseEntityChild()
	{
	}

	public void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("Registry.Entity.Unregister", 0.1f))
		{
			Entity.Unregister(base.gameObject);
		}
	}

	public static void Setup(GameObject obj, BaseEntity parent)
	{
		using (TimeWarning timeWarning = TimeWarning.New("Registry.Entity.Register", 0.1f))
		{
			Entity.Register(obj, parent);
		}
	}
}