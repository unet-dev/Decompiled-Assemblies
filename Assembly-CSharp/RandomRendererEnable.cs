using System;
using UnityEngine;

public class RandomRendererEnable : MonoBehaviour
{
	public Renderer[] randoms;

	public RandomRendererEnable()
	{
	}

	public void OnEnable()
	{
		int num = UnityEngine.Random.Range(0, (int)this.randoms.Length);
		this.randoms[num].enabled = true;
		Gibbable component = this.randoms[num].GetComponent<Gibbable>();
		if (component)
		{
			component.enabled = true;
		}
	}
}