using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	[Serializable]
	public class WeightedGameObjectList
	{
		public List<WeightedGameObjectList.Container> Objects = new List<WeightedGameObjectList.Container>();

		private float _total;

		private float Total
		{
			get
			{
				if (this._total == 0f)
				{
					this._total = this.Objects.Sum<WeightedGameObjectList.Container>((WeightedGameObjectList.Container x) => x.Weight);
				}
				return this._total;
			}
		}

		public WeightedGameObjectList()
		{
		}

		public GameObject Get(float f)
		{
			if (this.Objects.Count == 0)
			{
				return null;
			}
			f *= this.Total;
			float weight = 0f;
			for (int i = 0; i < this.Objects.Count; i++)
			{
				weight += this.Objects[i].Weight;
				if (f <= weight)
				{
					return this.Objects[i].Object;
				}
			}
			return this.Objects[this.Objects.Count - 1].Object;
		}

		public GameObject GetRandom()
		{
			return this.Get(UnityEngine.Random.Range(0f, 1f));
		}

		[Serializable]
		public struct Container
		{
			public float Weight;

			public GameObject Object;
		}
	}
}