using System;

namespace Apex.AI.Components
{
	[Serializable]
	internal class UtilityAIConfig
	{
		public string aiId;

		public float intervalMin;

		public float intervalMax;

		public float startDelayMin;

		public float startDelayMax;

		public bool isActive;

		public UtilityAIConfig()
		{
		}
	}
}