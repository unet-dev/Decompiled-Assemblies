using System;
using UnityEngine.SceneManagement;

namespace Rust
{
	public static class Generic
	{
		private static Scene _batchingScene;

		public static Scene BatchingScene
		{
			get
			{
				if (!Generic._batchingScene.IsValid())
				{
					Generic._batchingScene = SceneManager.CreateScene("Batching");
				}
				return Generic._batchingScene;
			}
		}
	}
}