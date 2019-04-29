using System;
using UnityEngine;

public class Wearable : MonoBehaviour, IItemSetup, IPrefabPreProcess
{
	[InspectorFlags]
	public Wearable.RemoveSkin removeSkin;

	[InspectorFlags]
	public Wearable.RemoveHair removeHair;

	public Wearable.DeformHair deformHair;

	public bool showCensorshipCube;

	public bool showCensorshipCubeBreasts;

	public bool forceHideCensorshipBreasts;

	[InspectorFlags]
	public Wearable.OccupationSlots occupationUnder;

	[InspectorFlags]
	public Wearable.OccupationSlots occupationOver;

	public string followBone;

	private static LOD[] emptyLOD;

	static Wearable()
	{
		Wearable.emptyLOD = new LOD[1];
	}

	public Wearable()
	{
	}

	public void OnItemSetup(Item item)
	{
	}

	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		LODGroup[] componentsInChildren = base.GetComponentsInChildren<LODGroup>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			LODGroup lODGroup = componentsInChildren[i];
			lODGroup.SetLODs(Wearable.emptyLOD);
			preProcess.RemoveComponent(lODGroup);
		}
	}

	public void SetupRendererCache(IPrefabProcessor preProcess)
	{
	}

	[Flags]
	public enum DeformHair
	{
		None,
		BaseballCap,
		BoonieHat,
		CandleHat,
		MinersHat,
		WoodHelmet
	}

	[Flags]
	public enum OccupationSlots
	{
		HeadTop = 1,
		Face = 2,
		HeadBack = 4,
		TorsoFront = 8,
		TorsoBack = 16,
		LeftShoulder = 32,
		RightShoulder = 64,
		LeftArm = 128,
		RightArm = 256,
		LeftHand = 512,
		RightHand = 1024,
		Groin = 2048,
		Bum = 4096,
		LeftKnee = 8192,
		RightKnee = 16384,
		LeftLeg = 32768,
		RightLeg = 65536,
		LeftFoot = 131072,
		RightFoot = 262144
	}

	[Flags]
	public enum RemoveHair
	{
		Head = 1,
		Eyebrow = 2,
		Facial = 4,
		Armpit = 8,
		Pubic = 16
	}

	[Flags]
	public enum RemoveSkin
	{
		Torso = 1,
		Feet = 2,
		Hands = 4,
		Legs = 8,
		Head = 16
	}
}