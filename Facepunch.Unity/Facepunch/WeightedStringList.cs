using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	[Serializable]
	public class WeightedStringList
	{
		public List<WeightedStringList.Container> Objects = new List<WeightedStringList.Container>();

		private float _total;

		private float Total
		{
			get
			{
				if (this._total == 0f)
				{
					this._total = this.Objects.Sum<WeightedStringList.Container>((WeightedStringList.Container x) => x.Weight);
				}
				return this._total;
			}
		}

		public WeightedStringList()
		{
		}

		public string Get(float f)
		{
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

		public string GetRandom()
		{
			return this.Get(UnityEngine.Random.Range(0f, 1f));
		}

		[Serializable]
		public struct Container
		{
			public float Weight;

			public string Object;
		}
	}
}