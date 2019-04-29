using System;
using UnityEngine;

public class CreateEffect : MonoBehaviour
{
	public GameObjectRef EffectToCreate;

	public CreateEffect()
	{
	}

	public void OnEnable()
	{
		Effect.client.Run(this.EffectToCreate.resourcePath, base.transform.position, base.transform.up, base.transform.forward);
	}
}