using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class SkinnedMeshRendererEx
	{
		public static Transform FindRig(this SkinnedMeshRenderer renderer)
		{
			Transform transforms = renderer.transform.parent;
			Transform transforms1 = renderer.rootBone;
			while (transforms1 != null && transforms1.parent != null && transforms1.parent != transforms)
			{
				transforms1 = transforms1.parent;
			}
			return transforms1;
		}
	}
}