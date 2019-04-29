using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName="Scriptable Object/Workshop Skin Meta", fileName="meta.asset")]
public class WorkshopSkinBase : WorkshopBase
{
	public static string[] itemName;

	[Header("Skin Setup")]
	public WorkshopSkinBase.SkinType skinType;

	[FormerlySerializedAs("skinMaterial")]
	public Material skinMaterial0;

	public Material skinMaterial1;

	public Material skinMaterial2;

	public Material skinMaterial3;

	static WorkshopSkinBase()
	{
		WorkshopSkinBase.itemName = new string[] { "tshirt", "pants", "sleepingbag", "hoodie", "tshirt.long", "hat.cap", "hat.beenie", "shoes.boots", "jacket", "mask.balaclava", "hat.boonie", "jacket.snow", "mask.bandana", "rifle.ak", "rifle.bolt", "pistol.revolver", "rock", "hammer", "shotgun.waterpipe", "shotgun.pump", "pistol.semiauto", "smg.thompson", "box.wooden.large", "bucket.helmet", "burlap.gloves", "burlap.trousers", "burlap.shirt" };
	}

	public WorkshopSkinBase()
	{
	}

	public enum SkinType
	{
		TShirt,
		Pants,
		SleepingBag,
		Hoodie,
		LongTShirt,
		Cap,
		Beenie,
		Boots,
		Jacket,
		Balaclava,
		Boonie,
		SnowJacket,
		Bandana,
		AK47,
		BoltRifle,
		Revolver,
		Rock,
		Hammer,
		PipeShotgun,
		PumpShotgun,
		SemiAutoPistol,
		Thompson,
		WoodStorage,
		BucketHat,
		BurlapGloves,
		BurlapPants,
		BurlapShirt
	}
}