using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class AchievementGroup
{
	public Translate.Phrase groupTitle = new Translate.Phrase("", "");

	public static AchievementGroup[] All;

	public AchievementGroup.AchievementItem[] Items;

	public bool Unlocked
	{
		get
		{
			return ((IEnumerable<AchievementGroup.AchievementItem>)this.Items).All<AchievementGroup.AchievementItem>((AchievementGroup.AchievementItem x) => x.Unlocked);
		}
	}

	static AchievementGroup()
	{
		AchievementGroup[] achievementGroupArray = new AchievementGroup[14];
		AchievementGroup achievementGroup = new AchievementGroup("list_getting_started", "Getting Started")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("COLLECT_100_WOOD", "Harvest 100 Wood"), new AchievementGroup.AchievementItem("CRAFT_CAMPFIRE", "Craft a camp fire"), new AchievementGroup.AchievementItem("PLACE_CAMPFIRE", "Place a camp fire") }
		};
		achievementGroupArray[0] = achievementGroup;
		achievementGroup = new AchievementGroup("list_tools_weaps", "Craft Tools & Weapons")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("COLLECT_700_WOOD", "Harvest 700 Wood"), new AchievementGroup.AchievementItem("COLLECT_200_STONE", "Harvest 200 Stone"), new AchievementGroup.AchievementItem("CRAFT_STONE_HATCHET", "Craft a Stone Hatchet"), new AchievementGroup.AchievementItem("CRAFT_STONE_PICKAXE", "Craft a Stone Pickaxe"), new AchievementGroup.AchievementItem("CRAFT_SPEAR", "Craft a Wooden Spear") }
		};
		achievementGroupArray[1] = achievementGroup;
		achievementGroup = new AchievementGroup("list_respawn_point", "Create a respawn point")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("COLLECT_30_CLOTH", "Collect 30 Cloth"), new AchievementGroup.AchievementItem("CRAFT_SLEEPINGBAG", "Craft a sleeping bag"), new AchievementGroup.AchievementItem("PLACE_SLEEPINGBAG", "Place a sleeping bag") }
		};
		achievementGroupArray[2] = achievementGroup;
		achievementGroup = new AchievementGroup("list_base_building", "Build a Base")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("CRAFT_BUILDING_PLAN", "Craft a Building Plan"), new AchievementGroup.AchievementItem("CRAFT_HAMMER", "Craft a hammer"), new AchievementGroup.AchievementItem("CONSTRUCT_BASE", "Construct a Base"), new AchievementGroup.AchievementItem("UPGRADE_BASE", "Upgrade your base") }
		};
		achievementGroupArray[3] = achievementGroup;
		achievementGroup = new AchievementGroup("list_secure_base", "Secure your Base")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("CRAFT_WOODEN_DOOR", "Craft a Wooden Door"), new AchievementGroup.AchievementItem("CRAFT_LOCK", "Craft a lock"), new AchievementGroup.AchievementItem("PLACE_WOODEN_DOOR", "Place Wooden Door"), new AchievementGroup.AchievementItem("PLACE_LOCK", "Place lock on Door"), new AchievementGroup.AchievementItem("LOCK_LOCK", "Lock the Lock") }
		};
		achievementGroupArray[4] = achievementGroup;
		achievementGroup = new AchievementGroup("list_create_storage", "Create Storage")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("CRAFT_WOODEN_BOX", "Craft a Wooden Box"), new AchievementGroup.AchievementItem("PLACE_WOODEN_BOX", "Place Wooden Box in Base") }
		};
		achievementGroupArray[5] = achievementGroup;
		achievementGroup = new AchievementGroup("list_craft_toolcupboard", "Claim an Area")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("CRAFT_TOOL_CUPBOARD", "Craft a Tool Cupboard"), new AchievementGroup.AchievementItem("PLACE_TOOL_CUPBOARD", "Place tool cupboard in base") }
		};
		achievementGroupArray[6] = achievementGroup;
		achievementGroup = new AchievementGroup("list_hunt", "Going Hunting")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("COLLECT_50_CLOTH", "Gather 50 Cloth"), new AchievementGroup.AchievementItem("CRAFT_HUNTING_BOW", "Craft a Hunting Bow"), new AchievementGroup.AchievementItem("CRAFT_ARROWS", "Craft some Arrows"), new AchievementGroup.AchievementItem("KILL_ANIMAL", "Kill an Animal"), new AchievementGroup.AchievementItem("SKIN_ANIMAL", "Harvest an Animal") }
		};
		achievementGroupArray[7] = achievementGroup;
		achievementGroup = new AchievementGroup("list_gear_up", "Craft & Equip Clothing")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("CRAFT_BURLAP_HEADWRAP", "Craft a Burlap Headwrap"), new AchievementGroup.AchievementItem("CRAFT_BURLAP_SHIRT", "Craft a Burlap Shirt"), new AchievementGroup.AchievementItem("CRAFT_BURLAP_PANTS", "Craft Burlap Pants"), new AchievementGroup.AchievementItem("EQUIP_CLOTHING", "Equip Clothing") }
		};
		achievementGroupArray[8] = achievementGroup;
		achievementGroup = new AchievementGroup("list_furnace", "Create a Furnace")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("COLLECT_50_LGF", "Collect or Craft 50 Low Grade Fuel"), new AchievementGroup.AchievementItem("CRAFT_FURNACE", "Craft a Furnace"), new AchievementGroup.AchievementItem("PLACE_FURNACE", "Place a Furnace") }
		};
		achievementGroupArray[9] = achievementGroup;
		achievementGroup = new AchievementGroup("list_machete", "Craft a Metal Weapon")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("COLLECT_300_METAL_ORE", "Collect 300 Metal Ore"), new AchievementGroup.AchievementItem("CRAFT_MACHETE", "Craft a Machete") }
		};
		achievementGroupArray[10] = achievementGroup;
		achievementGroup = new AchievementGroup("list_explore_1", "Exploring")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("VISIT_ROAD", "Visit a Road"), new AchievementGroup.AchievementItem("DESTROY_10_BARRELS", "Destroy 10 Barrels"), new AchievementGroup.AchievementItem("COLLECT_65_SCRAP", "Collect 65 Scrap") }
		};
		achievementGroupArray[11] = achievementGroup;
		achievementGroup = new AchievementGroup("list_workbench", "Workbenches")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("CRAFT_WORKBENCH", "Craft a Workbench"), new AchievementGroup.AchievementItem("PLACE_WORKBENCH", "Place Workbench in base"), new AchievementGroup.AchievementItem("CRAFT_NAILGUN", "Craft a Nailgun"), new AchievementGroup.AchievementItem("CRAFT_NAILGUN_NAILS", "Craft Nailgun Nails") }
		};
		achievementGroupArray[12] = achievementGroup;
		achievementGroup = new AchievementGroup("list_research", "Researching")
		{
			Items = new AchievementGroup.AchievementItem[] { new AchievementGroup.AchievementItem("CRAFT_RESEARCH_TABLE", "Craft a Research Table"), new AchievementGroup.AchievementItem("PLACE_RESEARCH_TABLE", "Place Research Table in base"), new AchievementGroup.AchievementItem("RESEARCH_ITEM", "Research an Item") }
		};
		achievementGroupArray[13] = achievementGroup;
		AchievementGroup.All = achievementGroupArray;
	}

	public AchievementGroup(string token = "", string english = "")
	{
		this.groupTitle.token = token;
		this.groupTitle.english = english;
	}

	public class AchievementItem
	{
		public string Name;

		public Translate.Phrase Phrase;

		public Achievement Achievement
		{
			get
			{
				return new Achievement(this.Name);
			}
		}

		public bool Unlocked
		{
			get
			{
				return this.Achievement.State;
			}
		}

		public AchievementItem(string name, string phrase)
		{
			this.Name = name;
			this.Phrase = new Translate.Phrase(string.Concat("achievement_", name).ToLower(), phrase);
		}
	}
}