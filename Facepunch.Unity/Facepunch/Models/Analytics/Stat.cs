using System;
using System.Collections.Generic;

namespace Facepunch.Models.Analytics
{
	internal class Stat : Dictionary<string, Stat.Container>
	{
		public Stat()
		{
		}

		public Stat.Container GetCategory(string name)
		{
			Stat.Container container;
			if (base.TryGetValue(name, out container))
			{
				return container;
			}
			container = new Stat.Container();
			base.Add(name, container);
			return container;
		}

		public class Container
		{
			public Dictionary<string, Stat.Container.Average> avg;

			public Dictionary<string, double> cnt;

			public Container()
			{
			}

			internal void AddAverage(string label, double value)
			{
				if (this.avg == null)
				{
					this.avg = new Dictionary<string, Stat.Container.Average>();
				}
				Stat.Container.Average average = null;
				if (!this.avg.TryGetValue(label, out average))
				{
					average = new Stat.Container.Average();
					this.avg.Add(label, average);
				}
				average.Sum += value;
				average.Cnt++;
				if (average.Cnt == 1)
				{
					average.Min = value;
					average.Max = value;
					return;
				}
				average.Min = System.Math.Min(value, average.Min);
				average.Max = System.Math.Max(value, average.Max);
			}

			internal void AddReplace(string label, double value)
			{
				if (this.cnt == null)
				{
					this.cnt = new Dictionary<string, double>();
				}
				if (this.cnt.ContainsKey(label))
				{
					this.cnt[label] = value;
					return;
				}
				this.cnt.Add(label, value);
			}

			internal void AddSum(string label, double value)
			{
				if (this.cnt == null)
				{
					this.cnt = new Dictionary<string, double>();
				}
				double num = 0;
				if (!this.cnt.TryGetValue(label, out num))
				{
					this.cnt.Add(label, value);
					return;
				}
				this.cnt[label] = num + value;
			}

			public class Average
			{
				public double Sum;

				public int Cnt;

				public double Min;

				public double Max;

				public Average()
				{
				}
			}
		}
	}
}