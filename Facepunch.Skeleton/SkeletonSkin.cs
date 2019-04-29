using System;
using UnityEngine;

namespace Facepunch
{
	[AddComponentMenu("Facepunch/SkeletonSkin")]
	public class SkeletonSkin : MonoBehaviour
	{
		public UnityEngine.SkinnedMeshRenderer SkinnedMeshRenderer;

		public Facepunch.SkeletonDefinition SkeletonDefinition;

		public int RootBone;

		public int[] TargetBones;

		public SkeletonSkin()
		{
		}

		public UnityEngine.SkinnedMeshRenderer DuplicateAndRetarget(GameObject host, Skeleton target)
		{
			UnityEngine.SkinnedMeshRenderer skinnedMeshRenderer = host.AddComponent<UnityEngine.SkinnedMeshRenderer>();
			skinnedMeshRenderer.receiveShadows = this.SkinnedMeshRenderer.receiveShadows;
			skinnedMeshRenderer.skinnedMotionVectors = this.SkinnedMeshRenderer.skinnedMotionVectors;
			skinnedMeshRenderer.motionVectorGenerationMode = this.SkinnedMeshRenderer.motionVectorGenerationMode;
			skinnedMeshRenderer.updateWhenOffscreen = this.SkinnedMeshRenderer.updateWhenOffscreen;
			skinnedMeshRenderer.localBounds = this.SkinnedMeshRenderer.localBounds;
			skinnedMeshRenderer.shadowCastingMode = this.SkinnedMeshRenderer.shadowCastingMode;
			skinnedMeshRenderer.sharedMesh = this.SkinnedMeshRenderer.sharedMesh;
			skinnedMeshRenderer.sharedMaterials = this.SkinnedMeshRenderer.sharedMaterials;
			skinnedMeshRenderer.rootBone = target.GetTransform(this.RootBone);
			Transform[] transform = new Transform[(int)this.TargetBones.Length];
			for (int i = 0; i < (int)transform.Length; i++)
			{
				transform[i] = target.GetTransform(this.TargetBones[i]);
			}
			skinnedMeshRenderer.bones = transform;
			return skinnedMeshRenderer;
		}

		public void Retarget(Skeleton target)
		{
			this.SkinnedMeshRenderer.rootBone = target.GetTransform(this.RootBone);
			Transform[] transform = new Transform[(int)this.TargetBones.Length];
			for (int i = 0; i < (int)transform.Length; i++)
			{
				transform[i] = target.GetTransform(this.TargetBones[i]);
			}
			this.SkinnedMeshRenderer.bones = transform;
		}
	}
}