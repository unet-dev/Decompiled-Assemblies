using System;

namespace Apex.AI
{
	public static class AINameMap
	{
		public readonly static Guid AnimalAction;

		public readonly static Guid AnimalMovement;

		public readonly static Guid AnimalThink;

		public readonly static Guid HTNAllShoot;

		public readonly static Guid HTNDomainAnimalBear;

		public readonly static Guid HTNDomainMurderer;

		public readonly static Guid HTNDomainNPCTurret;

		public readonly static Guid HTNDomainScientistAStar;

		public readonly static Guid HTNDomainScientistAStarTestCargoShip;

		public readonly static Guid HTNDomainScientistJunkpile;

		public readonly static Guid HTNDomainScientistMilitaryTunnel;

		public readonly static Guid HTNOneAttackFromCover;

		public readonly static Guid HTNOneIdle;

		public readonly static Guid HTNOneKillPrimaryThreat;

		public readonly static Guid HTNOneReloadOrSwitchWeapon;

		public readonly static Guid HTNRootScientist;

		public readonly static Guid MurdererAction;

		public readonly static Guid MurdererMove;

		public readonly static Guid MurdererThink;

		public readonly static Guid NpcAnimalGeneric;

		public readonly static Guid NpcHumanBanditGuard;

		public readonly static Guid NpcHumanScientist;

		public readonly static Guid NpcHumanScientistCH47;

		public readonly static Guid NpcHumanScientistJunkpile;

		public readonly static Guid NpcHumanScientistMelee;

		public readonly static Guid NpcHumanScientistTactical;

		public readonly static Guid NpcHumanScientistTacticalMountable;

		public readonly static Guid NpcHumanScientistTargetSelectorCover;

		public readonly static Guid NpcHumanScientistTargetSelectorEnemyHideout;

		public readonly static Guid NpcHumanScientistTargetSelectorOtherEntities;

		public readonly static Guid NpcHumanScientistTargetSelectorPlayer;

		public readonly static Guid NpcHumanScientistTargetSelectorPlayerMounted;

		public readonly static Guid NPCPlayerAction;

		public readonly static Guid NPCPlayerIdle;

		public readonly static Guid NPCPlayerMove;

		public readonly static Guid NPCPlayerThink;

		public readonly static Guid ZombieAction;

		static AINameMap()
		{
			AINameMap.AnimalAction = new Guid("fbbca198-2370-403a-a9ff-46ebd266cc4c");
			AINameMap.AnimalMovement = new Guid("6dca0a5b-d417-4ee1-9780-86ef5721fa04");
			AINameMap.AnimalThink = new Guid("288b4d89-ac3d-4e3c-b927-b3193af53d9e");
			AINameMap.HTNAllShoot = new Guid("fd7e613e-149b-428f-ae72-1483cbe5b29c");
			AINameMap.HTNDomainAnimalBear = new Guid("f7700d92-e54e-4a7f-9759-341bf82097f4");
			AINameMap.HTNDomainMurderer = new Guid("097b2c2f-0a52-47a0-902e-b4b96c601353");
			AINameMap.HTNDomainNPCTurret = new Guid("87cb4764-1606-4df9-b8fd-25770ac218c6");
			AINameMap.HTNDomainScientistAStar = new Guid("cade7ac7-b198-4454-8974-819023cd328a");
			AINameMap.HTNDomainScientistAStarTestCargoShip = new Guid("4085fb83-40de-43e8-bb2f-e978ba371c35");
			AINameMap.HTNDomainScientistJunkpile = new Guid("f402b4f6-2401-4ac6-9784-687e6ae8b27c");
			AINameMap.HTNDomainScientistMilitaryTunnel = new Guid("9dad0a1f-e2e2-49a2-8f47-4e9a667ab2f1");
			AINameMap.HTNOneAttackFromCover = new Guid("a641deaf-34cc-4427-81d8-ec2ecd4e55bb");
			AINameMap.HTNOneIdle = new Guid("ed311b16-a8c6-46f1-9fe2-9badc411ae95");
			AINameMap.HTNOneKillPrimaryThreat = new Guid("d0494bf8-f36f-4854-b2b2-92b0e4665036");
			AINameMap.HTNOneReloadOrSwitchWeapon = new Guid("75bf2577-4f66-4cdc-89c2-c358816af226");
			AINameMap.HTNRootScientist = new Guid("ffc9820f-5a80-4c46-8ecf-d38d3d45be2f");
			AINameMap.MurdererAction = new Guid("6e8a0667-bed9-424a-9128-6f49e9196ea9");
			AINameMap.MurdererMove = new Guid("84177090-95b1-4434-a4b5-c480785487f4");
			AINameMap.MurdererThink = new Guid("21161e69-f640-4c69-b9c8-0e061807ff5e");
			AINameMap.NpcAnimalGeneric = new Guid("0ce6945b-b709-487d-b69d-047f13447107");
			AINameMap.NpcHumanBanditGuard = new Guid("bc0a8fd4-cd5f-4d6e-8f36-27c30a6032e7");
			AINameMap.NpcHumanScientist = new Guid("157ef1da-b58a-4092-9267-4458c850fc48");
			AINameMap.NpcHumanScientistCH47 = new Guid("b1348460-0648-4c6a-98b0-59a9da4a639c");
			AINameMap.NpcHumanScientistJunkpile = new Guid("3b746da6-6aef-403e-9806-365d11a9ff48");
			AINameMap.NpcHumanScientistMelee = new Guid("96f12289-c968-4d53-afc7-d64897dd85c6");
			AINameMap.NpcHumanScientistTactical = new Guid("fba3b625-d850-4312-b328-ef9ae20f44bc");
			AINameMap.NpcHumanScientistTacticalMountable = new Guid("5c3b0c47-07ef-4b8b-811d-d3881c226772");
			AINameMap.NpcHumanScientistTargetSelectorCover = new Guid("fc26fe11-9167-451f-b718-3bc6cdccd84a");
			AINameMap.NpcHumanScientistTargetSelectorEnemyHideout = new Guid("436adfad-b17b-4f81-a0e8-213e660b5d63");
			AINameMap.NpcHumanScientistTargetSelectorOtherEntities = new Guid("e9048c4d-d032-4ffb-9aac-71846f2f1742");
			AINameMap.NpcHumanScientistTargetSelectorPlayer = new Guid("e8bae2d4-b6ff-40cc-911e-632c739ac441");
			AINameMap.NpcHumanScientistTargetSelectorPlayerMounted = new Guid("eac40400-12da-4905-9f5f-281ca2a8ba88");
			AINameMap.NPCPlayerAction = new Guid("8d135d51-8591-4977-a0ea-34be0e175ab0");
			AINameMap.NPCPlayerIdle = new Guid("d9c089f7-6067-42b6-bb7a-49c1d1687867");
			AINameMap.NPCPlayerMove = new Guid("594ce0f7-e8f7-4fcc-bb7b-f5619fb6c36e");
			AINameMap.NPCPlayerThink = new Guid("fc6aef4e-0128-4c29-8082-9f8098dade72");
			AINameMap.ZombieAction = new Guid("0b91e09e-b490-4412-898f-62235e69aabe");
		}
	}
}