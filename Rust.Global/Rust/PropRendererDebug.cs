using System;
using UnityEngine;

namespace Rust
{
	[ExecuteInEditMode]
	public class PropRendererDebug : MonoBehaviour
	{
		public PropRendererDebug()
		{
		}

		public void Update()
		{
			PropRenderer[] propRendererArray = UnityEngine.Object.FindObjectsOfType<PropRenderer>();
			for (int i = 0; i < (int)propRendererArray.Length; i++)
			{
				propRendererArray[i].DebugAlign();
			}
		}
	}
}