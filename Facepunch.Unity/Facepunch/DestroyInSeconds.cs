using System;
using UnityEngine;

namespace Facepunch
{
	public class DestroyInSeconds : MonoBehaviour
	{
		public float TimeToDie = 5f;

		public float TimeToDieVariance;

		public DestroyInSeconds()
		{
		}

		private void Start()
		{
			UnityEngine.Object.Destroy(base.gameObject, this.TimeToDie + UnityEngine.Random.Range(this.TimeToDieVariance * -0.5f, this.TimeToDieVariance * 0.5f));
		}
	}
}