using System;
using System.Text;
using UnityEngine;

namespace ConVar
{
	[Factory("batching")]
	public class Batching : ConsoleSystem
	{
		[ClientVar]
		[ServerVar]
		public static bool colliders;

		[ClientVar]
		[ServerVar]
		public static bool collider_threading;

		[ClientVar]
		[ServerVar]
		public static int collider_capacity;

		[ClientVar]
		[ServerVar]
		public static int collider_vertices;

		[ClientVar]
		[ServerVar]
		public static int collider_submeshes;

		[ClientVar]
		public static bool renderers;

		[ClientVar]
		public static bool renderer_threading;

		[ClientVar]
		public static int renderer_capacity;

		[ClientVar]
		public static int renderer_vertices;

		[ClientVar]
		public static int renderer_submeshes;

		[ClientVar]
		[ServerVar]
		public static int verbose;

		static Batching()
		{
			Batching.colliders = false;
			Batching.collider_threading = true;
			Batching.collider_capacity = 30000;
			Batching.collider_vertices = 1000;
			Batching.collider_submeshes = 1;
			Batching.renderers = true;
			Batching.renderer_threading = true;
			Batching.renderer_capacity = 30000;
			Batching.renderer_vertices = 1000;
			Batching.renderer_submeshes = 1;
			Batching.verbose = 0;
		}

		public Batching()
		{
		}

		[ServerVar]
		public static void print_colliders(ConsoleSystem.Arg args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (SingletonComponent<ColliderGrid>.Instance)
			{
				stringBuilder.AppendFormat("Mesh Collider Batching: {0:N0}/{0:N0}", SingletonComponent<ColliderGrid>.Instance.BatchedMeshCount(), SingletonComponent<ColliderGrid>.Instance.MeshCount());
				stringBuilder.AppendLine();
			}
			args.ReplyWith(stringBuilder.ToString());
		}

		[ServerVar]
		public static void refresh_colliders(ConsoleSystem.Arg args)
		{
			ColliderBatch[] colliderBatchArray = UnityEngine.Object.FindObjectsOfType<ColliderBatch>();
			for (int i = 0; i < (int)colliderBatchArray.Length; i++)
			{
				colliderBatchArray[i].Refresh();
			}
			if (SingletonComponent<ColliderGrid>.Instance)
			{
				SingletonComponent<ColliderGrid>.Instance.Refresh();
			}
		}
	}
}