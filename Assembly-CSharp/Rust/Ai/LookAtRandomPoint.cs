using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class LookAtRandomPoint : BaseAction
	{
		[ApexSerialization]
		public float MinTimeout = 5f;

		[ApexSerialization]
		public float MaxTimeout = 20f;

		public LookAtRandomPoint()
		{
		}

		public override void DoExecute(BaseContext context)
		{
			NPCHumanContext nPCHumanContext = context as NPCHumanContext;
			if (nPCHumanContext != null)
			{
				nPCHumanContext.Human.LookAtRandomPoint(UnityEngine.Random.Range(this.MinTimeout, this.MaxTimeout));
			}
		}
	}
}