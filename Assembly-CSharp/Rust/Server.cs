using System;
using UnityEngine.SceneManagement;

namespace Rust
{
	public static class Server
	{
		public const float UseDistance = 3f;

		private static Scene _entityScene;

		public static Scene EntityScene
		{
			get
			{
				if (!Server._entityScene.IsValid())
				{
					Server._entityScene = SceneManager.CreateScene("Server Entities");
				}
				return Server._entityScene;
			}
		}
	}
}