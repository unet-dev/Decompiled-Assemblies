using System;
using UnityEngine;

namespace Rust.Ai
{
	public static class Sense
	{
		private static BaseEntity[] query;

		static Sense()
		{
			Sense.query = new BaseEntity[512];
		}

		private static bool IsAbleToBeStimulated(BaseEntity ent)
		{
			if (ent is BasePlayer)
			{
				return true;
			}
			if (ent is BaseNpc)
			{
				return true;
			}
			return false;
		}

		public static void Stimulate(Sensation sensation)
		{
			int inSphere = BaseEntity.Query.Server.GetInSphere(sensation.Position, sensation.Radius, Sense.query, new Func<BaseEntity, bool>(Sense.IsAbleToBeStimulated));
			float radius = sensation.Radius * sensation.Radius;
			for (int i = 0; i < inSphere; i++)
			{
				if ((Sense.query[i].transform.position - sensation.Position).sqrMagnitude <= radius)
				{
					Sense.query[i].OnSensation(sensation);
				}
			}
		}
	}
}