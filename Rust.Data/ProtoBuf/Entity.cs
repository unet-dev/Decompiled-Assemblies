using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Entity : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public BaseNetworkable baseNetworkable;

		[NonSerialized]
		public BaseEntity baseEntity;

		[NonSerialized]
		public BasePlayer basePlayer;

		[NonSerialized]
		public WorldItem worldItem;

		[NonSerialized]
		public BaseResource resource;

		[NonSerialized]
		public BuildingBlock buildingBlock;

		[NonSerialized]
		public ProtoBuf.Environment environment;

		[NonSerialized]
		public Corpse corpse;

		[NonSerialized]
		public ParentInfo parent;

		[NonSerialized]
		public KeyLock keyLock;

		[NonSerialized]
		public CodeLock codeLock;

		[NonSerialized]
		public EntitySlots entitySlots;

		[NonSerialized]
		public BuildingPrivilege buildingPrivilege;

		[NonSerialized]
		public StorageBox storageBox;

		[NonSerialized]
		public HeldEntity heldEntity;

		[NonSerialized]
		public BaseProjectile baseProjectile;

		[NonSerialized]
		public BaseNPC baseNPC;

		[NonSerialized]
		public Loot loot;

		[NonSerialized]
		public GenericSpawner genericSpawner;

		[NonSerialized]
		public SleepingBag sleepingBag;

		[NonSerialized]
		public LootableCorpse lootableCorpse;

		[NonSerialized]
		public Sign sign;

		[NonSerialized]
		public BaseCombat baseCombat;

		[NonSerialized]
		public MapEntity mapEntity;

		[NonSerialized]
		public ResearchTable researchTable;

		[NonSerialized]
		public DudExplosive dudExplosive;

		[NonSerialized]
		public MiningQuarry miningQuarry;

		[NonSerialized]
		public PlantEntity plantEntity;

		[NonSerialized]
		public Helicopter helicopter;

		[NonSerialized]
		public Landmine landmine;

		[NonSerialized]
		public AutoTurret autoturret;

		[NonSerialized]
		public SphereEntity sphereEntity;

		[NonSerialized]
		public StabilityEntity stabilityEntity;

		[NonSerialized]
		public OwnerInfo ownerInfo;

		[NonSerialized]
		public DecayEntity decayEntity;

		[NonSerialized]
		public Spawnable spawnable;

		[NonSerialized]
		public ServerGib servergib;

		[NonSerialized]
		public VendingMachine vendingMachine;

		[NonSerialized]
		public SpinnerWheel spinnerWheel;

		[NonSerialized]
		public Lift lift;

		[NonSerialized]
		public BradleyAPC bradley;

		[NonSerialized]
		public WaterWell waterwell;

		[NonSerialized]
		public Motorboat motorBoat;

		[NonSerialized]
		public IOEntity ioEntity;

		[NonSerialized]
		public PuzzleReset puzzleReset;

		[NonSerialized]
		public RelationshipManager relationshipManager;

		[NonSerialized]
		public HotAirBalloon hotAirBalloon;

		[NonSerialized]
		public SAMSite samSite;

		[NonSerialized]
		public EggHunt eggHunt;

		[NonSerialized]
		public ArcadeMachine arcadeMachine;

		[NonSerialized]
		public bool createdThisFrame;

		public bool ShouldPool = true;

		private bool _disposed;

		public Entity()
		{
		}

		public Entity Copy()
		{
			Entity entity = Pool.Get<Entity>();
			this.CopyTo(entity);
			return entity;
		}

		public void CopyTo(Entity instance)
		{
			if (this.baseNetworkable == null)
			{
				instance.baseNetworkable = null;
			}
			else if (instance.baseNetworkable != null)
			{
				this.baseNetworkable.CopyTo(instance.baseNetworkable);
			}
			else
			{
				instance.baseNetworkable = this.baseNetworkable.Copy();
			}
			if (this.baseEntity == null)
			{
				instance.baseEntity = null;
			}
			else if (instance.baseEntity != null)
			{
				this.baseEntity.CopyTo(instance.baseEntity);
			}
			else
			{
				instance.baseEntity = this.baseEntity.Copy();
			}
			if (this.basePlayer == null)
			{
				instance.basePlayer = null;
			}
			else if (instance.basePlayer != null)
			{
				this.basePlayer.CopyTo(instance.basePlayer);
			}
			else
			{
				instance.basePlayer = this.basePlayer.Copy();
			}
			if (this.worldItem == null)
			{
				instance.worldItem = null;
			}
			else if (instance.worldItem != null)
			{
				this.worldItem.CopyTo(instance.worldItem);
			}
			else
			{
				instance.worldItem = this.worldItem.Copy();
			}
			if (this.resource == null)
			{
				instance.resource = null;
			}
			else if (instance.resource != null)
			{
				this.resource.CopyTo(instance.resource);
			}
			else
			{
				instance.resource = this.resource.Copy();
			}
			if (this.buildingBlock == null)
			{
				instance.buildingBlock = null;
			}
			else if (instance.buildingBlock != null)
			{
				this.buildingBlock.CopyTo(instance.buildingBlock);
			}
			else
			{
				instance.buildingBlock = this.buildingBlock.Copy();
			}
			if (this.environment == null)
			{
				instance.environment = null;
			}
			else if (instance.environment != null)
			{
				this.environment.CopyTo(instance.environment);
			}
			else
			{
				instance.environment = this.environment.Copy();
			}
			if (this.corpse == null)
			{
				instance.corpse = null;
			}
			else if (instance.corpse != null)
			{
				this.corpse.CopyTo(instance.corpse);
			}
			else
			{
				instance.corpse = this.corpse.Copy();
			}
			if (this.parent == null)
			{
				instance.parent = null;
			}
			else if (instance.parent != null)
			{
				this.parent.CopyTo(instance.parent);
			}
			else
			{
				instance.parent = this.parent.Copy();
			}
			if (this.keyLock == null)
			{
				instance.keyLock = null;
			}
			else if (instance.keyLock != null)
			{
				this.keyLock.CopyTo(instance.keyLock);
			}
			else
			{
				instance.keyLock = this.keyLock.Copy();
			}
			if (this.codeLock == null)
			{
				instance.codeLock = null;
			}
			else if (instance.codeLock != null)
			{
				this.codeLock.CopyTo(instance.codeLock);
			}
			else
			{
				instance.codeLock = this.codeLock.Copy();
			}
			if (this.entitySlots == null)
			{
				instance.entitySlots = null;
			}
			else if (instance.entitySlots != null)
			{
				this.entitySlots.CopyTo(instance.entitySlots);
			}
			else
			{
				instance.entitySlots = this.entitySlots.Copy();
			}
			if (this.buildingPrivilege == null)
			{
				instance.buildingPrivilege = null;
			}
			else if (instance.buildingPrivilege != null)
			{
				this.buildingPrivilege.CopyTo(instance.buildingPrivilege);
			}
			else
			{
				instance.buildingPrivilege = this.buildingPrivilege.Copy();
			}
			if (this.storageBox == null)
			{
				instance.storageBox = null;
			}
			else if (instance.storageBox != null)
			{
				this.storageBox.CopyTo(instance.storageBox);
			}
			else
			{
				instance.storageBox = this.storageBox.Copy();
			}
			if (this.heldEntity == null)
			{
				instance.heldEntity = null;
			}
			else if (instance.heldEntity != null)
			{
				this.heldEntity.CopyTo(instance.heldEntity);
			}
			else
			{
				instance.heldEntity = this.heldEntity.Copy();
			}
			if (this.baseProjectile == null)
			{
				instance.baseProjectile = null;
			}
			else if (instance.baseProjectile != null)
			{
				this.baseProjectile.CopyTo(instance.baseProjectile);
			}
			else
			{
				instance.baseProjectile = this.baseProjectile.Copy();
			}
			if (this.baseNPC == null)
			{
				instance.baseNPC = null;
			}
			else if (instance.baseNPC != null)
			{
				this.baseNPC.CopyTo(instance.baseNPC);
			}
			else
			{
				instance.baseNPC = this.baseNPC.Copy();
			}
			if (this.loot == null)
			{
				instance.loot = null;
			}
			else if (instance.loot != null)
			{
				this.loot.CopyTo(instance.loot);
			}
			else
			{
				instance.loot = this.loot.Copy();
			}
			if (this.genericSpawner == null)
			{
				instance.genericSpawner = null;
			}
			else if (instance.genericSpawner != null)
			{
				this.genericSpawner.CopyTo(instance.genericSpawner);
			}
			else
			{
				instance.genericSpawner = this.genericSpawner.Copy();
			}
			if (this.sleepingBag == null)
			{
				instance.sleepingBag = null;
			}
			else if (instance.sleepingBag != null)
			{
				this.sleepingBag.CopyTo(instance.sleepingBag);
			}
			else
			{
				instance.sleepingBag = this.sleepingBag.Copy();
			}
			if (this.lootableCorpse == null)
			{
				instance.lootableCorpse = null;
			}
			else if (instance.lootableCorpse != null)
			{
				this.lootableCorpse.CopyTo(instance.lootableCorpse);
			}
			else
			{
				instance.lootableCorpse = this.lootableCorpse.Copy();
			}
			if (this.sign == null)
			{
				instance.sign = null;
			}
			else if (instance.sign != null)
			{
				this.sign.CopyTo(instance.sign);
			}
			else
			{
				instance.sign = this.sign.Copy();
			}
			if (this.baseCombat == null)
			{
				instance.baseCombat = null;
			}
			else if (instance.baseCombat != null)
			{
				this.baseCombat.CopyTo(instance.baseCombat);
			}
			else
			{
				instance.baseCombat = this.baseCombat.Copy();
			}
			if (this.mapEntity == null)
			{
				instance.mapEntity = null;
			}
			else if (instance.mapEntity != null)
			{
				this.mapEntity.CopyTo(instance.mapEntity);
			}
			else
			{
				instance.mapEntity = this.mapEntity.Copy();
			}
			if (this.researchTable == null)
			{
				instance.researchTable = null;
			}
			else if (instance.researchTable != null)
			{
				this.researchTable.CopyTo(instance.researchTable);
			}
			else
			{
				instance.researchTable = this.researchTable.Copy();
			}
			if (this.dudExplosive == null)
			{
				instance.dudExplosive = null;
			}
			else if (instance.dudExplosive != null)
			{
				this.dudExplosive.CopyTo(instance.dudExplosive);
			}
			else
			{
				instance.dudExplosive = this.dudExplosive.Copy();
			}
			if (this.miningQuarry == null)
			{
				instance.miningQuarry = null;
			}
			else if (instance.miningQuarry != null)
			{
				this.miningQuarry.CopyTo(instance.miningQuarry);
			}
			else
			{
				instance.miningQuarry = this.miningQuarry.Copy();
			}
			if (this.plantEntity == null)
			{
				instance.plantEntity = null;
			}
			else if (instance.plantEntity != null)
			{
				this.plantEntity.CopyTo(instance.plantEntity);
			}
			else
			{
				instance.plantEntity = this.plantEntity.Copy();
			}
			if (this.helicopter == null)
			{
				instance.helicopter = null;
			}
			else if (instance.helicopter != null)
			{
				this.helicopter.CopyTo(instance.helicopter);
			}
			else
			{
				instance.helicopter = this.helicopter.Copy();
			}
			if (this.landmine == null)
			{
				instance.landmine = null;
			}
			else if (instance.landmine != null)
			{
				this.landmine.CopyTo(instance.landmine);
			}
			else
			{
				instance.landmine = this.landmine.Copy();
			}
			if (this.autoturret == null)
			{
				instance.autoturret = null;
			}
			else if (instance.autoturret != null)
			{
				this.autoturret.CopyTo(instance.autoturret);
			}
			else
			{
				instance.autoturret = this.autoturret.Copy();
			}
			if (this.sphereEntity == null)
			{
				instance.sphereEntity = null;
			}
			else if (instance.sphereEntity != null)
			{
				this.sphereEntity.CopyTo(instance.sphereEntity);
			}
			else
			{
				instance.sphereEntity = this.sphereEntity.Copy();
			}
			if (this.stabilityEntity == null)
			{
				instance.stabilityEntity = null;
			}
			else if (instance.stabilityEntity != null)
			{
				this.stabilityEntity.CopyTo(instance.stabilityEntity);
			}
			else
			{
				instance.stabilityEntity = this.stabilityEntity.Copy();
			}
			if (this.ownerInfo == null)
			{
				instance.ownerInfo = null;
			}
			else if (instance.ownerInfo != null)
			{
				this.ownerInfo.CopyTo(instance.ownerInfo);
			}
			else
			{
				instance.ownerInfo = this.ownerInfo.Copy();
			}
			if (this.decayEntity == null)
			{
				instance.decayEntity = null;
			}
			else if (instance.decayEntity != null)
			{
				this.decayEntity.CopyTo(instance.decayEntity);
			}
			else
			{
				instance.decayEntity = this.decayEntity.Copy();
			}
			if (this.spawnable == null)
			{
				instance.spawnable = null;
			}
			else if (instance.spawnable != null)
			{
				this.spawnable.CopyTo(instance.spawnable);
			}
			else
			{
				instance.spawnable = this.spawnable.Copy();
			}
			if (this.servergib == null)
			{
				instance.servergib = null;
			}
			else if (instance.servergib != null)
			{
				this.servergib.CopyTo(instance.servergib);
			}
			else
			{
				instance.servergib = this.servergib.Copy();
			}
			if (this.vendingMachine == null)
			{
				instance.vendingMachine = null;
			}
			else if (instance.vendingMachine != null)
			{
				this.vendingMachine.CopyTo(instance.vendingMachine);
			}
			else
			{
				instance.vendingMachine = this.vendingMachine.Copy();
			}
			if (this.spinnerWheel == null)
			{
				instance.spinnerWheel = null;
			}
			else if (instance.spinnerWheel != null)
			{
				this.spinnerWheel.CopyTo(instance.spinnerWheel);
			}
			else
			{
				instance.spinnerWheel = this.spinnerWheel.Copy();
			}
			if (this.lift == null)
			{
				instance.lift = null;
			}
			else if (instance.lift != null)
			{
				this.lift.CopyTo(instance.lift);
			}
			else
			{
				instance.lift = this.lift.Copy();
			}
			if (this.bradley == null)
			{
				instance.bradley = null;
			}
			else if (instance.bradley != null)
			{
				this.bradley.CopyTo(instance.bradley);
			}
			else
			{
				instance.bradley = this.bradley.Copy();
			}
			if (this.waterwell == null)
			{
				instance.waterwell = null;
			}
			else if (instance.waterwell != null)
			{
				this.waterwell.CopyTo(instance.waterwell);
			}
			else
			{
				instance.waterwell = this.waterwell.Copy();
			}
			if (this.motorBoat == null)
			{
				instance.motorBoat = null;
			}
			else if (instance.motorBoat != null)
			{
				this.motorBoat.CopyTo(instance.motorBoat);
			}
			else
			{
				instance.motorBoat = this.motorBoat.Copy();
			}
			if (this.ioEntity == null)
			{
				instance.ioEntity = null;
			}
			else if (instance.ioEntity != null)
			{
				this.ioEntity.CopyTo(instance.ioEntity);
			}
			else
			{
				instance.ioEntity = this.ioEntity.Copy();
			}
			if (this.puzzleReset == null)
			{
				instance.puzzleReset = null;
			}
			else if (instance.puzzleReset != null)
			{
				this.puzzleReset.CopyTo(instance.puzzleReset);
			}
			else
			{
				instance.puzzleReset = this.puzzleReset.Copy();
			}
			if (this.relationshipManager == null)
			{
				instance.relationshipManager = null;
			}
			else if (instance.relationshipManager != null)
			{
				this.relationshipManager.CopyTo(instance.relationshipManager);
			}
			else
			{
				instance.relationshipManager = this.relationshipManager.Copy();
			}
			if (this.hotAirBalloon == null)
			{
				instance.hotAirBalloon = null;
			}
			else if (instance.hotAirBalloon != null)
			{
				this.hotAirBalloon.CopyTo(instance.hotAirBalloon);
			}
			else
			{
				instance.hotAirBalloon = this.hotAirBalloon.Copy();
			}
			if (this.samSite == null)
			{
				instance.samSite = null;
			}
			else if (instance.samSite != null)
			{
				this.samSite.CopyTo(instance.samSite);
			}
			else
			{
				instance.samSite = this.samSite.Copy();
			}
			if (this.eggHunt == null)
			{
				instance.eggHunt = null;
			}
			else if (instance.eggHunt != null)
			{
				this.eggHunt.CopyTo(instance.eggHunt);
			}
			else
			{
				instance.eggHunt = this.eggHunt.Copy();
			}
			if (this.arcadeMachine == null)
			{
				instance.arcadeMachine = null;
			}
			else if (instance.arcadeMachine != null)
			{
				this.arcadeMachine.CopyTo(instance.arcadeMachine);
			}
			else
			{
				instance.arcadeMachine = this.arcadeMachine.Copy();
			}
			instance.createdThisFrame = this.createdThisFrame;
		}

		public static Entity Deserialize(Stream stream)
		{
			Entity entity = Pool.Get<Entity>();
			Entity.Deserialize(stream, entity, false);
			return entity;
		}

		public static Entity Deserialize(byte[] buffer)
		{
			Entity entity = Pool.Get<Entity>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Entity.Deserialize(memoryStream, entity, false);
			}
			return entity;
		}

		public static Entity Deserialize(byte[] buffer, Entity instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Entity.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Entity Deserialize(Stream stream, Entity instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 58)
				{
					if (num <= 26)
					{
						if (num == 10)
						{
							if (instance.baseNetworkable != null)
							{
								BaseNetworkable.DeserializeLengthDelimited(stream, instance.baseNetworkable, isDelta);
								continue;
							}
							else
							{
								instance.baseNetworkable = BaseNetworkable.DeserializeLengthDelimited(stream);
								continue;
							}
						}
						else if (num != 18)
						{
							if (num == 26)
							{
								if (instance.basePlayer != null)
								{
									BasePlayer.DeserializeLengthDelimited(stream, instance.basePlayer, isDelta);
									continue;
								}
								else
								{
									instance.basePlayer = BasePlayer.DeserializeLengthDelimited(stream);
									continue;
								}
							}
						}
						else if (instance.baseEntity != null)
						{
							BaseEntity.DeserializeLengthDelimited(stream, instance.baseEntity, isDelta);
							continue;
						}
						else
						{
							instance.baseEntity = BaseEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num <= 42)
					{
						if (num != 34)
						{
							if (num == 42)
							{
								if (instance.resource != null)
								{
									BaseResource.DeserializeLengthDelimited(stream, instance.resource, isDelta);
									continue;
								}
								else
								{
									instance.resource = BaseResource.DeserializeLengthDelimited(stream);
									continue;
								}
							}
						}
						else if (instance.worldItem != null)
						{
							WorldItem.DeserializeLengthDelimited(stream, instance.worldItem, isDelta);
							continue;
						}
						else
						{
							instance.worldItem = WorldItem.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num != 50)
					{
						if (num == 58)
						{
							if (instance.environment != null)
							{
								ProtoBuf.Environment.DeserializeLengthDelimited(stream, instance.environment, isDelta);
								continue;
							}
							else
							{
								instance.environment = ProtoBuf.Environment.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.buildingBlock != null)
					{
						BuildingBlock.DeserializeLengthDelimited(stream, instance.buildingBlock, isDelta);
						continue;
					}
					else
					{
						instance.buildingBlock = BuildingBlock.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num <= 90)
				{
					if (num == 66)
					{
						if (instance.corpse != null)
						{
							Corpse.DeserializeLengthDelimited(stream, instance.corpse, isDelta);
							continue;
						}
						else
						{
							instance.corpse = Corpse.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num != 82)
					{
						if (num == 90)
						{
							if (instance.keyLock != null)
							{
								KeyLock.DeserializeLengthDelimited(stream, instance.keyLock, isDelta);
								continue;
							}
							else
							{
								instance.keyLock = KeyLock.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.parent != null)
					{
						ParentInfo.DeserializeLengthDelimited(stream, instance.parent, isDelta);
						continue;
					}
					else
					{
						instance.parent = ParentInfo.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num <= 106)
				{
					if (num != 98)
					{
						if (num == 106)
						{
							if (instance.entitySlots != null)
							{
								EntitySlots.DeserializeLengthDelimited(stream, instance.entitySlots, isDelta);
								continue;
							}
							else
							{
								instance.entitySlots = EntitySlots.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.codeLock != null)
					{
						CodeLock.DeserializeLengthDelimited(stream, instance.codeLock, isDelta);
						continue;
					}
					else
					{
						instance.codeLock = CodeLock.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num != 114)
				{
					if (num == 122)
					{
						if (instance.storageBox != null)
						{
							StorageBox.DeserializeLengthDelimited(stream, instance.storageBox, isDelta);
							continue;
						}
						else
						{
							instance.storageBox = StorageBox.DeserializeLengthDelimited(stream);
							continue;
						}
					}
				}
				else if (instance.buildingPrivilege != null)
				{
					BuildingPrivilege.DeserializeLengthDelimited(stream, instance.buildingPrivilege, isDelta);
					continue;
				}
				else
				{
					instance.buildingPrivilege = BuildingPrivilege.DeserializeLengthDelimited(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				switch (field)
				{
					case 0:
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 15:
					{
						ProtocolParser.SkipKey(stream, key);
						continue;
					}
					case 16:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.heldEntity != null)
						{
							HeldEntity.DeserializeLengthDelimited(stream, instance.heldEntity, isDelta);
							continue;
						}
						else
						{
							instance.heldEntity = HeldEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 17:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.baseProjectile != null)
						{
							BaseProjectile.DeserializeLengthDelimited(stream, instance.baseProjectile, isDelta);
							continue;
						}
						else
						{
							instance.baseProjectile = BaseProjectile.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 18:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.baseNPC != null)
						{
							BaseNPC.DeserializeLengthDelimited(stream, instance.baseNPC, isDelta);
							continue;
						}
						else
						{
							instance.baseNPC = BaseNPC.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 19:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.loot != null)
						{
							Loot.DeserializeLengthDelimited(stream, instance.loot, isDelta);
							continue;
						}
						else
						{
							instance.loot = Loot.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 20:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.genericSpawner != null)
						{
							GenericSpawner.DeserializeLengthDelimited(stream, instance.genericSpawner, isDelta);
							continue;
						}
						else
						{
							instance.genericSpawner = GenericSpawner.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 21:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.sleepingBag != null)
						{
							SleepingBag.DeserializeLengthDelimited(stream, instance.sleepingBag, isDelta);
							continue;
						}
						else
						{
							instance.sleepingBag = SleepingBag.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 22:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.lootableCorpse != null)
						{
							LootableCorpse.DeserializeLengthDelimited(stream, instance.lootableCorpse, isDelta);
							continue;
						}
						else
						{
							instance.lootableCorpse = LootableCorpse.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 23:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.sign != null)
						{
							Sign.DeserializeLengthDelimited(stream, instance.sign, isDelta);
							continue;
						}
						else
						{
							instance.sign = Sign.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 24:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.baseCombat != null)
						{
							BaseCombat.DeserializeLengthDelimited(stream, instance.baseCombat, isDelta);
							continue;
						}
						else
						{
							instance.baseCombat = BaseCombat.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 25:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.mapEntity != null)
						{
							MapEntity.DeserializeLengthDelimited(stream, instance.mapEntity, isDelta);
							continue;
						}
						else
						{
							instance.mapEntity = MapEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 26:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.researchTable != null)
						{
							ResearchTable.DeserializeLengthDelimited(stream, instance.researchTable, isDelta);
							continue;
						}
						else
						{
							instance.researchTable = ResearchTable.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 27:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.dudExplosive != null)
						{
							DudExplosive.DeserializeLengthDelimited(stream, instance.dudExplosive, isDelta);
							continue;
						}
						else
						{
							instance.dudExplosive = DudExplosive.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 28:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.miningQuarry != null)
						{
							MiningQuarry.DeserializeLengthDelimited(stream, instance.miningQuarry, isDelta);
							continue;
						}
						else
						{
							instance.miningQuarry = MiningQuarry.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 29:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.plantEntity != null)
						{
							PlantEntity.DeserializeLengthDelimited(stream, instance.plantEntity, isDelta);
							continue;
						}
						else
						{
							instance.plantEntity = PlantEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 30:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.helicopter != null)
						{
							Helicopter.DeserializeLengthDelimited(stream, instance.helicopter, isDelta);
							continue;
						}
						else
						{
							instance.helicopter = Helicopter.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 31:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.landmine != null)
						{
							Landmine.DeserializeLengthDelimited(stream, instance.landmine, isDelta);
							continue;
						}
						else
						{
							instance.landmine = Landmine.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 32:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.autoturret != null)
						{
							AutoTurret.DeserializeLengthDelimited(stream, instance.autoturret, isDelta);
							continue;
						}
						else
						{
							instance.autoturret = AutoTurret.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 33:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.sphereEntity != null)
						{
							SphereEntity.DeserializeLengthDelimited(stream, instance.sphereEntity, isDelta);
							continue;
						}
						else
						{
							instance.sphereEntity = SphereEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 34:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.stabilityEntity != null)
						{
							StabilityEntity.DeserializeLengthDelimited(stream, instance.stabilityEntity, isDelta);
							continue;
						}
						else
						{
							instance.stabilityEntity = StabilityEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 35:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.ownerInfo != null)
						{
							OwnerInfo.DeserializeLengthDelimited(stream, instance.ownerInfo, isDelta);
							continue;
						}
						else
						{
							instance.ownerInfo = OwnerInfo.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 36:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.decayEntity != null)
						{
							DecayEntity.DeserializeLengthDelimited(stream, instance.decayEntity, isDelta);
							continue;
						}
						else
						{
							instance.decayEntity = DecayEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 37:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.spawnable != null)
						{
							Spawnable.DeserializeLengthDelimited(stream, instance.spawnable, isDelta);
							continue;
						}
						else
						{
							instance.spawnable = Spawnable.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 38:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.servergib != null)
						{
							ServerGib.DeserializeLengthDelimited(stream, instance.servergib, isDelta);
							continue;
						}
						else
						{
							instance.servergib = ServerGib.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 39:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.vendingMachine != null)
						{
							VendingMachine.DeserializeLengthDelimited(stream, instance.vendingMachine, isDelta);
							continue;
						}
						else
						{
							instance.vendingMachine = VendingMachine.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 40:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.spinnerWheel != null)
						{
							SpinnerWheel.DeserializeLengthDelimited(stream, instance.spinnerWheel, isDelta);
							continue;
						}
						else
						{
							instance.spinnerWheel = SpinnerWheel.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 41:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.lift != null)
						{
							Lift.DeserializeLengthDelimited(stream, instance.lift, isDelta);
							continue;
						}
						else
						{
							instance.lift = Lift.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 42:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.bradley != null)
						{
							BradleyAPC.DeserializeLengthDelimited(stream, instance.bradley, isDelta);
							continue;
						}
						else
						{
							instance.bradley = BradleyAPC.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 43:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.waterwell != null)
						{
							WaterWell.DeserializeLengthDelimited(stream, instance.waterwell, isDelta);
							continue;
						}
						else
						{
							instance.waterwell = WaterWell.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 44:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.motorBoat != null)
						{
							Motorboat.DeserializeLengthDelimited(stream, instance.motorBoat, isDelta);
							continue;
						}
						else
						{
							instance.motorBoat = Motorboat.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 45:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.ioEntity != null)
						{
							IOEntity.DeserializeLengthDelimited(stream, instance.ioEntity, isDelta);
							continue;
						}
						else
						{
							instance.ioEntity = IOEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 46:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.puzzleReset != null)
						{
							PuzzleReset.DeserializeLengthDelimited(stream, instance.puzzleReset, isDelta);
							continue;
						}
						else
						{
							instance.puzzleReset = PuzzleReset.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 47:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.relationshipManager != null)
						{
							RelationshipManager.DeserializeLengthDelimited(stream, instance.relationshipManager, isDelta);
							continue;
						}
						else
						{
							instance.relationshipManager = RelationshipManager.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 48:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.hotAirBalloon != null)
						{
							HotAirBalloon.DeserializeLengthDelimited(stream, instance.hotAirBalloon, isDelta);
							continue;
						}
						else
						{
							instance.hotAirBalloon = HotAirBalloon.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 49:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.samSite != null)
						{
							SAMSite.DeserializeLengthDelimited(stream, instance.samSite, isDelta);
							continue;
						}
						else
						{
							instance.samSite = SAMSite.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 50:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.eggHunt != null)
						{
							EggHunt.DeserializeLengthDelimited(stream, instance.eggHunt, isDelta);
							continue;
						}
						else
						{
							instance.eggHunt = EggHunt.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 51:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.arcadeMachine != null)
						{
							ArcadeMachine.DeserializeLengthDelimited(stream, instance.arcadeMachine, isDelta);
							continue;
						}
						else
						{
							instance.arcadeMachine = ArcadeMachine.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					default:
					{
						if (field == 100)
						{
							if (key.WireType != Wire.Varint)
							{
								continue;
							}
							instance.createdThisFrame = ProtocolParser.ReadBool(stream);
							continue;
						}
						else
						{
							goto case 15;
						}
					}
				}
			}
			return instance;
		}

		public static Entity DeserializeLength(Stream stream, int length)
		{
			Entity entity = Pool.Get<Entity>();
			Entity.DeserializeLength(stream, length, entity, false);
			return entity;
		}

		public static Entity DeserializeLength(Stream stream, int length, Entity instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 58)
				{
					if (num <= 26)
					{
						if (num == 10)
						{
							if (instance.baseNetworkable != null)
							{
								BaseNetworkable.DeserializeLengthDelimited(stream, instance.baseNetworkable, isDelta);
								continue;
							}
							else
							{
								instance.baseNetworkable = BaseNetworkable.DeserializeLengthDelimited(stream);
								continue;
							}
						}
						else if (num != 18)
						{
							if (num == 26)
							{
								if (instance.basePlayer != null)
								{
									BasePlayer.DeserializeLengthDelimited(stream, instance.basePlayer, isDelta);
									continue;
								}
								else
								{
									instance.basePlayer = BasePlayer.DeserializeLengthDelimited(stream);
									continue;
								}
							}
						}
						else if (instance.baseEntity != null)
						{
							BaseEntity.DeserializeLengthDelimited(stream, instance.baseEntity, isDelta);
							continue;
						}
						else
						{
							instance.baseEntity = BaseEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num <= 42)
					{
						if (num != 34)
						{
							if (num == 42)
							{
								if (instance.resource != null)
								{
									BaseResource.DeserializeLengthDelimited(stream, instance.resource, isDelta);
									continue;
								}
								else
								{
									instance.resource = BaseResource.DeserializeLengthDelimited(stream);
									continue;
								}
							}
						}
						else if (instance.worldItem != null)
						{
							WorldItem.DeserializeLengthDelimited(stream, instance.worldItem, isDelta);
							continue;
						}
						else
						{
							instance.worldItem = WorldItem.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num != 50)
					{
						if (num == 58)
						{
							if (instance.environment != null)
							{
								ProtoBuf.Environment.DeserializeLengthDelimited(stream, instance.environment, isDelta);
								continue;
							}
							else
							{
								instance.environment = ProtoBuf.Environment.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.buildingBlock != null)
					{
						BuildingBlock.DeserializeLengthDelimited(stream, instance.buildingBlock, isDelta);
						continue;
					}
					else
					{
						instance.buildingBlock = BuildingBlock.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num <= 90)
				{
					if (num == 66)
					{
						if (instance.corpse != null)
						{
							Corpse.DeserializeLengthDelimited(stream, instance.corpse, isDelta);
							continue;
						}
						else
						{
							instance.corpse = Corpse.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num != 82)
					{
						if (num == 90)
						{
							if (instance.keyLock != null)
							{
								KeyLock.DeserializeLengthDelimited(stream, instance.keyLock, isDelta);
								continue;
							}
							else
							{
								instance.keyLock = KeyLock.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.parent != null)
					{
						ParentInfo.DeserializeLengthDelimited(stream, instance.parent, isDelta);
						continue;
					}
					else
					{
						instance.parent = ParentInfo.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num <= 106)
				{
					if (num != 98)
					{
						if (num == 106)
						{
							if (instance.entitySlots != null)
							{
								EntitySlots.DeserializeLengthDelimited(stream, instance.entitySlots, isDelta);
								continue;
							}
							else
							{
								instance.entitySlots = EntitySlots.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.codeLock != null)
					{
						CodeLock.DeserializeLengthDelimited(stream, instance.codeLock, isDelta);
						continue;
					}
					else
					{
						instance.codeLock = CodeLock.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num != 114)
				{
					if (num == 122)
					{
						if (instance.storageBox != null)
						{
							StorageBox.DeserializeLengthDelimited(stream, instance.storageBox, isDelta);
							continue;
						}
						else
						{
							instance.storageBox = StorageBox.DeserializeLengthDelimited(stream);
							continue;
						}
					}
				}
				else if (instance.buildingPrivilege != null)
				{
					BuildingPrivilege.DeserializeLengthDelimited(stream, instance.buildingPrivilege, isDelta);
					continue;
				}
				else
				{
					instance.buildingPrivilege = BuildingPrivilege.DeserializeLengthDelimited(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				switch (field)
				{
					case 0:
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 15:
					{
						ProtocolParser.SkipKey(stream, key);
						continue;
					}
					case 16:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.heldEntity != null)
						{
							HeldEntity.DeserializeLengthDelimited(stream, instance.heldEntity, isDelta);
							continue;
						}
						else
						{
							instance.heldEntity = HeldEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 17:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.baseProjectile != null)
						{
							BaseProjectile.DeserializeLengthDelimited(stream, instance.baseProjectile, isDelta);
							continue;
						}
						else
						{
							instance.baseProjectile = BaseProjectile.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 18:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.baseNPC != null)
						{
							BaseNPC.DeserializeLengthDelimited(stream, instance.baseNPC, isDelta);
							continue;
						}
						else
						{
							instance.baseNPC = BaseNPC.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 19:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.loot != null)
						{
							Loot.DeserializeLengthDelimited(stream, instance.loot, isDelta);
							continue;
						}
						else
						{
							instance.loot = Loot.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 20:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.genericSpawner != null)
						{
							GenericSpawner.DeserializeLengthDelimited(stream, instance.genericSpawner, isDelta);
							continue;
						}
						else
						{
							instance.genericSpawner = GenericSpawner.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 21:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.sleepingBag != null)
						{
							SleepingBag.DeserializeLengthDelimited(stream, instance.sleepingBag, isDelta);
							continue;
						}
						else
						{
							instance.sleepingBag = SleepingBag.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 22:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.lootableCorpse != null)
						{
							LootableCorpse.DeserializeLengthDelimited(stream, instance.lootableCorpse, isDelta);
							continue;
						}
						else
						{
							instance.lootableCorpse = LootableCorpse.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 23:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.sign != null)
						{
							Sign.DeserializeLengthDelimited(stream, instance.sign, isDelta);
							continue;
						}
						else
						{
							instance.sign = Sign.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 24:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.baseCombat != null)
						{
							BaseCombat.DeserializeLengthDelimited(stream, instance.baseCombat, isDelta);
							continue;
						}
						else
						{
							instance.baseCombat = BaseCombat.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 25:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.mapEntity != null)
						{
							MapEntity.DeserializeLengthDelimited(stream, instance.mapEntity, isDelta);
							continue;
						}
						else
						{
							instance.mapEntity = MapEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 26:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.researchTable != null)
						{
							ResearchTable.DeserializeLengthDelimited(stream, instance.researchTable, isDelta);
							continue;
						}
						else
						{
							instance.researchTable = ResearchTable.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 27:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.dudExplosive != null)
						{
							DudExplosive.DeserializeLengthDelimited(stream, instance.dudExplosive, isDelta);
							continue;
						}
						else
						{
							instance.dudExplosive = DudExplosive.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 28:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.miningQuarry != null)
						{
							MiningQuarry.DeserializeLengthDelimited(stream, instance.miningQuarry, isDelta);
							continue;
						}
						else
						{
							instance.miningQuarry = MiningQuarry.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 29:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.plantEntity != null)
						{
							PlantEntity.DeserializeLengthDelimited(stream, instance.plantEntity, isDelta);
							continue;
						}
						else
						{
							instance.plantEntity = PlantEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 30:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.helicopter != null)
						{
							Helicopter.DeserializeLengthDelimited(stream, instance.helicopter, isDelta);
							continue;
						}
						else
						{
							instance.helicopter = Helicopter.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 31:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.landmine != null)
						{
							Landmine.DeserializeLengthDelimited(stream, instance.landmine, isDelta);
							continue;
						}
						else
						{
							instance.landmine = Landmine.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 32:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.autoturret != null)
						{
							AutoTurret.DeserializeLengthDelimited(stream, instance.autoturret, isDelta);
							continue;
						}
						else
						{
							instance.autoturret = AutoTurret.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 33:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.sphereEntity != null)
						{
							SphereEntity.DeserializeLengthDelimited(stream, instance.sphereEntity, isDelta);
							continue;
						}
						else
						{
							instance.sphereEntity = SphereEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 34:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.stabilityEntity != null)
						{
							StabilityEntity.DeserializeLengthDelimited(stream, instance.stabilityEntity, isDelta);
							continue;
						}
						else
						{
							instance.stabilityEntity = StabilityEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 35:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.ownerInfo != null)
						{
							OwnerInfo.DeserializeLengthDelimited(stream, instance.ownerInfo, isDelta);
							continue;
						}
						else
						{
							instance.ownerInfo = OwnerInfo.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 36:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.decayEntity != null)
						{
							DecayEntity.DeserializeLengthDelimited(stream, instance.decayEntity, isDelta);
							continue;
						}
						else
						{
							instance.decayEntity = DecayEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 37:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.spawnable != null)
						{
							Spawnable.DeserializeLengthDelimited(stream, instance.spawnable, isDelta);
							continue;
						}
						else
						{
							instance.spawnable = Spawnable.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 38:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.servergib != null)
						{
							ServerGib.DeserializeLengthDelimited(stream, instance.servergib, isDelta);
							continue;
						}
						else
						{
							instance.servergib = ServerGib.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 39:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.vendingMachine != null)
						{
							VendingMachine.DeserializeLengthDelimited(stream, instance.vendingMachine, isDelta);
							continue;
						}
						else
						{
							instance.vendingMachine = VendingMachine.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 40:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.spinnerWheel != null)
						{
							SpinnerWheel.DeserializeLengthDelimited(stream, instance.spinnerWheel, isDelta);
							continue;
						}
						else
						{
							instance.spinnerWheel = SpinnerWheel.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 41:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.lift != null)
						{
							Lift.DeserializeLengthDelimited(stream, instance.lift, isDelta);
							continue;
						}
						else
						{
							instance.lift = Lift.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 42:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.bradley != null)
						{
							BradleyAPC.DeserializeLengthDelimited(stream, instance.bradley, isDelta);
							continue;
						}
						else
						{
							instance.bradley = BradleyAPC.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 43:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.waterwell != null)
						{
							WaterWell.DeserializeLengthDelimited(stream, instance.waterwell, isDelta);
							continue;
						}
						else
						{
							instance.waterwell = WaterWell.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 44:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.motorBoat != null)
						{
							Motorboat.DeserializeLengthDelimited(stream, instance.motorBoat, isDelta);
							continue;
						}
						else
						{
							instance.motorBoat = Motorboat.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 45:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.ioEntity != null)
						{
							IOEntity.DeserializeLengthDelimited(stream, instance.ioEntity, isDelta);
							continue;
						}
						else
						{
							instance.ioEntity = IOEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 46:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.puzzleReset != null)
						{
							PuzzleReset.DeserializeLengthDelimited(stream, instance.puzzleReset, isDelta);
							continue;
						}
						else
						{
							instance.puzzleReset = PuzzleReset.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 47:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.relationshipManager != null)
						{
							RelationshipManager.DeserializeLengthDelimited(stream, instance.relationshipManager, isDelta);
							continue;
						}
						else
						{
							instance.relationshipManager = RelationshipManager.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 48:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.hotAirBalloon != null)
						{
							HotAirBalloon.DeserializeLengthDelimited(stream, instance.hotAirBalloon, isDelta);
							continue;
						}
						else
						{
							instance.hotAirBalloon = HotAirBalloon.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 49:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.samSite != null)
						{
							SAMSite.DeserializeLengthDelimited(stream, instance.samSite, isDelta);
							continue;
						}
						else
						{
							instance.samSite = SAMSite.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 50:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.eggHunt != null)
						{
							EggHunt.DeserializeLengthDelimited(stream, instance.eggHunt, isDelta);
							continue;
						}
						else
						{
							instance.eggHunt = EggHunt.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 51:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.arcadeMachine != null)
						{
							ArcadeMachine.DeserializeLengthDelimited(stream, instance.arcadeMachine, isDelta);
							continue;
						}
						else
						{
							instance.arcadeMachine = ArcadeMachine.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					default:
					{
						if (field == 100)
						{
							if (key.WireType != Wire.Varint)
							{
								continue;
							}
							instance.createdThisFrame = ProtocolParser.ReadBool(stream);
							continue;
						}
						else
						{
							goto case 15;
						}
					}
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static Entity DeserializeLengthDelimited(Stream stream)
		{
			Entity entity = Pool.Get<Entity>();
			Entity.DeserializeLengthDelimited(stream, entity, false);
			return entity;
		}

		public static Entity DeserializeLengthDelimited(Stream stream, Entity instance, bool isDelta)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 58)
				{
					if (num <= 26)
					{
						if (num == 10)
						{
							if (instance.baseNetworkable != null)
							{
								BaseNetworkable.DeserializeLengthDelimited(stream, instance.baseNetworkable, isDelta);
								continue;
							}
							else
							{
								instance.baseNetworkable = BaseNetworkable.DeserializeLengthDelimited(stream);
								continue;
							}
						}
						else if (num != 18)
						{
							if (num == 26)
							{
								if (instance.basePlayer != null)
								{
									BasePlayer.DeserializeLengthDelimited(stream, instance.basePlayer, isDelta);
									continue;
								}
								else
								{
									instance.basePlayer = BasePlayer.DeserializeLengthDelimited(stream);
									continue;
								}
							}
						}
						else if (instance.baseEntity != null)
						{
							BaseEntity.DeserializeLengthDelimited(stream, instance.baseEntity, isDelta);
							continue;
						}
						else
						{
							instance.baseEntity = BaseEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num <= 42)
					{
						if (num != 34)
						{
							if (num == 42)
							{
								if (instance.resource != null)
								{
									BaseResource.DeserializeLengthDelimited(stream, instance.resource, isDelta);
									continue;
								}
								else
								{
									instance.resource = BaseResource.DeserializeLengthDelimited(stream);
									continue;
								}
							}
						}
						else if (instance.worldItem != null)
						{
							WorldItem.DeserializeLengthDelimited(stream, instance.worldItem, isDelta);
							continue;
						}
						else
						{
							instance.worldItem = WorldItem.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num != 50)
					{
						if (num == 58)
						{
							if (instance.environment != null)
							{
								ProtoBuf.Environment.DeserializeLengthDelimited(stream, instance.environment, isDelta);
								continue;
							}
							else
							{
								instance.environment = ProtoBuf.Environment.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.buildingBlock != null)
					{
						BuildingBlock.DeserializeLengthDelimited(stream, instance.buildingBlock, isDelta);
						continue;
					}
					else
					{
						instance.buildingBlock = BuildingBlock.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num <= 90)
				{
					if (num == 66)
					{
						if (instance.corpse != null)
						{
							Corpse.DeserializeLengthDelimited(stream, instance.corpse, isDelta);
							continue;
						}
						else
						{
							instance.corpse = Corpse.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num != 82)
					{
						if (num == 90)
						{
							if (instance.keyLock != null)
							{
								KeyLock.DeserializeLengthDelimited(stream, instance.keyLock, isDelta);
								continue;
							}
							else
							{
								instance.keyLock = KeyLock.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.parent != null)
					{
						ParentInfo.DeserializeLengthDelimited(stream, instance.parent, isDelta);
						continue;
					}
					else
					{
						instance.parent = ParentInfo.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num <= 106)
				{
					if (num != 98)
					{
						if (num == 106)
						{
							if (instance.entitySlots != null)
							{
								EntitySlots.DeserializeLengthDelimited(stream, instance.entitySlots, isDelta);
								continue;
							}
							else
							{
								instance.entitySlots = EntitySlots.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.codeLock != null)
					{
						CodeLock.DeserializeLengthDelimited(stream, instance.codeLock, isDelta);
						continue;
					}
					else
					{
						instance.codeLock = CodeLock.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num != 114)
				{
					if (num == 122)
					{
						if (instance.storageBox != null)
						{
							StorageBox.DeserializeLengthDelimited(stream, instance.storageBox, isDelta);
							continue;
						}
						else
						{
							instance.storageBox = StorageBox.DeserializeLengthDelimited(stream);
							continue;
						}
					}
				}
				else if (instance.buildingPrivilege != null)
				{
					BuildingPrivilege.DeserializeLengthDelimited(stream, instance.buildingPrivilege, isDelta);
					continue;
				}
				else
				{
					instance.buildingPrivilege = BuildingPrivilege.DeserializeLengthDelimited(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				switch (field)
				{
					case 0:
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 15:
					{
						ProtocolParser.SkipKey(stream, key);
						continue;
					}
					case 16:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.heldEntity != null)
						{
							HeldEntity.DeserializeLengthDelimited(stream, instance.heldEntity, isDelta);
							continue;
						}
						else
						{
							instance.heldEntity = HeldEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 17:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.baseProjectile != null)
						{
							BaseProjectile.DeserializeLengthDelimited(stream, instance.baseProjectile, isDelta);
							continue;
						}
						else
						{
							instance.baseProjectile = BaseProjectile.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 18:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.baseNPC != null)
						{
							BaseNPC.DeserializeLengthDelimited(stream, instance.baseNPC, isDelta);
							continue;
						}
						else
						{
							instance.baseNPC = BaseNPC.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 19:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.loot != null)
						{
							Loot.DeserializeLengthDelimited(stream, instance.loot, isDelta);
							continue;
						}
						else
						{
							instance.loot = Loot.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 20:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.genericSpawner != null)
						{
							GenericSpawner.DeserializeLengthDelimited(stream, instance.genericSpawner, isDelta);
							continue;
						}
						else
						{
							instance.genericSpawner = GenericSpawner.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 21:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.sleepingBag != null)
						{
							SleepingBag.DeserializeLengthDelimited(stream, instance.sleepingBag, isDelta);
							continue;
						}
						else
						{
							instance.sleepingBag = SleepingBag.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 22:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.lootableCorpse != null)
						{
							LootableCorpse.DeserializeLengthDelimited(stream, instance.lootableCorpse, isDelta);
							continue;
						}
						else
						{
							instance.lootableCorpse = LootableCorpse.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 23:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.sign != null)
						{
							Sign.DeserializeLengthDelimited(stream, instance.sign, isDelta);
							continue;
						}
						else
						{
							instance.sign = Sign.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 24:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.baseCombat != null)
						{
							BaseCombat.DeserializeLengthDelimited(stream, instance.baseCombat, isDelta);
							continue;
						}
						else
						{
							instance.baseCombat = BaseCombat.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 25:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.mapEntity != null)
						{
							MapEntity.DeserializeLengthDelimited(stream, instance.mapEntity, isDelta);
							continue;
						}
						else
						{
							instance.mapEntity = MapEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 26:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.researchTable != null)
						{
							ResearchTable.DeserializeLengthDelimited(stream, instance.researchTable, isDelta);
							continue;
						}
						else
						{
							instance.researchTable = ResearchTable.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 27:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.dudExplosive != null)
						{
							DudExplosive.DeserializeLengthDelimited(stream, instance.dudExplosive, isDelta);
							continue;
						}
						else
						{
							instance.dudExplosive = DudExplosive.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 28:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.miningQuarry != null)
						{
							MiningQuarry.DeserializeLengthDelimited(stream, instance.miningQuarry, isDelta);
							continue;
						}
						else
						{
							instance.miningQuarry = MiningQuarry.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 29:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.plantEntity != null)
						{
							PlantEntity.DeserializeLengthDelimited(stream, instance.plantEntity, isDelta);
							continue;
						}
						else
						{
							instance.plantEntity = PlantEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 30:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.helicopter != null)
						{
							Helicopter.DeserializeLengthDelimited(stream, instance.helicopter, isDelta);
							continue;
						}
						else
						{
							instance.helicopter = Helicopter.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 31:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.landmine != null)
						{
							Landmine.DeserializeLengthDelimited(stream, instance.landmine, isDelta);
							continue;
						}
						else
						{
							instance.landmine = Landmine.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 32:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.autoturret != null)
						{
							AutoTurret.DeserializeLengthDelimited(stream, instance.autoturret, isDelta);
							continue;
						}
						else
						{
							instance.autoturret = AutoTurret.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 33:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.sphereEntity != null)
						{
							SphereEntity.DeserializeLengthDelimited(stream, instance.sphereEntity, isDelta);
							continue;
						}
						else
						{
							instance.sphereEntity = SphereEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 34:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.stabilityEntity != null)
						{
							StabilityEntity.DeserializeLengthDelimited(stream, instance.stabilityEntity, isDelta);
							continue;
						}
						else
						{
							instance.stabilityEntity = StabilityEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 35:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.ownerInfo != null)
						{
							OwnerInfo.DeserializeLengthDelimited(stream, instance.ownerInfo, isDelta);
							continue;
						}
						else
						{
							instance.ownerInfo = OwnerInfo.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 36:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.decayEntity != null)
						{
							DecayEntity.DeserializeLengthDelimited(stream, instance.decayEntity, isDelta);
							continue;
						}
						else
						{
							instance.decayEntity = DecayEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 37:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.spawnable != null)
						{
							Spawnable.DeserializeLengthDelimited(stream, instance.spawnable, isDelta);
							continue;
						}
						else
						{
							instance.spawnable = Spawnable.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 38:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.servergib != null)
						{
							ServerGib.DeserializeLengthDelimited(stream, instance.servergib, isDelta);
							continue;
						}
						else
						{
							instance.servergib = ServerGib.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 39:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.vendingMachine != null)
						{
							VendingMachine.DeserializeLengthDelimited(stream, instance.vendingMachine, isDelta);
							continue;
						}
						else
						{
							instance.vendingMachine = VendingMachine.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 40:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.spinnerWheel != null)
						{
							SpinnerWheel.DeserializeLengthDelimited(stream, instance.spinnerWheel, isDelta);
							continue;
						}
						else
						{
							instance.spinnerWheel = SpinnerWheel.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 41:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.lift != null)
						{
							Lift.DeserializeLengthDelimited(stream, instance.lift, isDelta);
							continue;
						}
						else
						{
							instance.lift = Lift.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 42:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.bradley != null)
						{
							BradleyAPC.DeserializeLengthDelimited(stream, instance.bradley, isDelta);
							continue;
						}
						else
						{
							instance.bradley = BradleyAPC.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 43:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.waterwell != null)
						{
							WaterWell.DeserializeLengthDelimited(stream, instance.waterwell, isDelta);
							continue;
						}
						else
						{
							instance.waterwell = WaterWell.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 44:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.motorBoat != null)
						{
							Motorboat.DeserializeLengthDelimited(stream, instance.motorBoat, isDelta);
							continue;
						}
						else
						{
							instance.motorBoat = Motorboat.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 45:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.ioEntity != null)
						{
							IOEntity.DeserializeLengthDelimited(stream, instance.ioEntity, isDelta);
							continue;
						}
						else
						{
							instance.ioEntity = IOEntity.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 46:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.puzzleReset != null)
						{
							PuzzleReset.DeserializeLengthDelimited(stream, instance.puzzleReset, isDelta);
							continue;
						}
						else
						{
							instance.puzzleReset = PuzzleReset.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 47:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.relationshipManager != null)
						{
							RelationshipManager.DeserializeLengthDelimited(stream, instance.relationshipManager, isDelta);
							continue;
						}
						else
						{
							instance.relationshipManager = RelationshipManager.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 48:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.hotAirBalloon != null)
						{
							HotAirBalloon.DeserializeLengthDelimited(stream, instance.hotAirBalloon, isDelta);
							continue;
						}
						else
						{
							instance.hotAirBalloon = HotAirBalloon.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 49:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.samSite != null)
						{
							SAMSite.DeserializeLengthDelimited(stream, instance.samSite, isDelta);
							continue;
						}
						else
						{
							instance.samSite = SAMSite.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 50:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.eggHunt != null)
						{
							EggHunt.DeserializeLengthDelimited(stream, instance.eggHunt, isDelta);
							continue;
						}
						else
						{
							instance.eggHunt = EggHunt.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 51:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.arcadeMachine != null)
						{
							ArcadeMachine.DeserializeLengthDelimited(stream, instance.arcadeMachine, isDelta);
							continue;
						}
						else
						{
							instance.arcadeMachine = ArcadeMachine.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					default:
					{
						if (field == 100)
						{
							if (key.WireType != Wire.Varint)
							{
								continue;
							}
							instance.createdThisFrame = ProtocolParser.ReadBool(stream);
							continue;
						}
						else
						{
							goto case 15;
						}
					}
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public virtual void Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			this.ResetToPool();
			this._disposed = true;
		}

		public virtual void EnterPool()
		{
			this._disposed = true;
		}

		public void FromProto(Stream stream, bool isDelta = false)
		{
			Entity.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Entity.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Entity instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.baseNetworkable != null)
			{
				instance.baseNetworkable.ResetToPool();
				instance.baseNetworkable = null;
			}
			if (instance.baseEntity != null)
			{
				instance.baseEntity.ResetToPool();
				instance.baseEntity = null;
			}
			if (instance.basePlayer != null)
			{
				instance.basePlayer.ResetToPool();
				instance.basePlayer = null;
			}
			if (instance.worldItem != null)
			{
				instance.worldItem.ResetToPool();
				instance.worldItem = null;
			}
			if (instance.resource != null)
			{
				instance.resource.ResetToPool();
				instance.resource = null;
			}
			if (instance.buildingBlock != null)
			{
				instance.buildingBlock.ResetToPool();
				instance.buildingBlock = null;
			}
			if (instance.environment != null)
			{
				instance.environment.ResetToPool();
				instance.environment = null;
			}
			if (instance.corpse != null)
			{
				instance.corpse.ResetToPool();
				instance.corpse = null;
			}
			if (instance.parent != null)
			{
				instance.parent.ResetToPool();
				instance.parent = null;
			}
			if (instance.keyLock != null)
			{
				instance.keyLock.ResetToPool();
				instance.keyLock = null;
			}
			if (instance.codeLock != null)
			{
				instance.codeLock.ResetToPool();
				instance.codeLock = null;
			}
			if (instance.entitySlots != null)
			{
				instance.entitySlots.ResetToPool();
				instance.entitySlots = null;
			}
			if (instance.buildingPrivilege != null)
			{
				instance.buildingPrivilege.ResetToPool();
				instance.buildingPrivilege = null;
			}
			if (instance.storageBox != null)
			{
				instance.storageBox.ResetToPool();
				instance.storageBox = null;
			}
			if (instance.heldEntity != null)
			{
				instance.heldEntity.ResetToPool();
				instance.heldEntity = null;
			}
			if (instance.baseProjectile != null)
			{
				instance.baseProjectile.ResetToPool();
				instance.baseProjectile = null;
			}
			if (instance.baseNPC != null)
			{
				instance.baseNPC.ResetToPool();
				instance.baseNPC = null;
			}
			if (instance.loot != null)
			{
				instance.loot.ResetToPool();
				instance.loot = null;
			}
			if (instance.genericSpawner != null)
			{
				instance.genericSpawner.ResetToPool();
				instance.genericSpawner = null;
			}
			if (instance.sleepingBag != null)
			{
				instance.sleepingBag.ResetToPool();
				instance.sleepingBag = null;
			}
			if (instance.lootableCorpse != null)
			{
				instance.lootableCorpse.ResetToPool();
				instance.lootableCorpse = null;
			}
			if (instance.sign != null)
			{
				instance.sign.ResetToPool();
				instance.sign = null;
			}
			if (instance.baseCombat != null)
			{
				instance.baseCombat.ResetToPool();
				instance.baseCombat = null;
			}
			if (instance.mapEntity != null)
			{
				instance.mapEntity.ResetToPool();
				instance.mapEntity = null;
			}
			if (instance.researchTable != null)
			{
				instance.researchTable.ResetToPool();
				instance.researchTable = null;
			}
			if (instance.dudExplosive != null)
			{
				instance.dudExplosive.ResetToPool();
				instance.dudExplosive = null;
			}
			if (instance.miningQuarry != null)
			{
				instance.miningQuarry.ResetToPool();
				instance.miningQuarry = null;
			}
			if (instance.plantEntity != null)
			{
				instance.plantEntity.ResetToPool();
				instance.plantEntity = null;
			}
			if (instance.helicopter != null)
			{
				instance.helicopter.ResetToPool();
				instance.helicopter = null;
			}
			if (instance.landmine != null)
			{
				instance.landmine.ResetToPool();
				instance.landmine = null;
			}
			if (instance.autoturret != null)
			{
				instance.autoturret.ResetToPool();
				instance.autoturret = null;
			}
			if (instance.sphereEntity != null)
			{
				instance.sphereEntity.ResetToPool();
				instance.sphereEntity = null;
			}
			if (instance.stabilityEntity != null)
			{
				instance.stabilityEntity.ResetToPool();
				instance.stabilityEntity = null;
			}
			if (instance.ownerInfo != null)
			{
				instance.ownerInfo.ResetToPool();
				instance.ownerInfo = null;
			}
			if (instance.decayEntity != null)
			{
				instance.decayEntity.ResetToPool();
				instance.decayEntity = null;
			}
			if (instance.spawnable != null)
			{
				instance.spawnable.ResetToPool();
				instance.spawnable = null;
			}
			if (instance.servergib != null)
			{
				instance.servergib.ResetToPool();
				instance.servergib = null;
			}
			if (instance.vendingMachine != null)
			{
				instance.vendingMachine.ResetToPool();
				instance.vendingMachine = null;
			}
			if (instance.spinnerWheel != null)
			{
				instance.spinnerWheel.ResetToPool();
				instance.spinnerWheel = null;
			}
			if (instance.lift != null)
			{
				instance.lift.ResetToPool();
				instance.lift = null;
			}
			if (instance.bradley != null)
			{
				instance.bradley.ResetToPool();
				instance.bradley = null;
			}
			if (instance.waterwell != null)
			{
				instance.waterwell.ResetToPool();
				instance.waterwell = null;
			}
			if (instance.motorBoat != null)
			{
				instance.motorBoat.ResetToPool();
				instance.motorBoat = null;
			}
			if (instance.ioEntity != null)
			{
				instance.ioEntity.ResetToPool();
				instance.ioEntity = null;
			}
			if (instance.puzzleReset != null)
			{
				instance.puzzleReset.ResetToPool();
				instance.puzzleReset = null;
			}
			if (instance.relationshipManager != null)
			{
				instance.relationshipManager.ResetToPool();
				instance.relationshipManager = null;
			}
			if (instance.hotAirBalloon != null)
			{
				instance.hotAirBalloon.ResetToPool();
				instance.hotAirBalloon = null;
			}
			if (instance.samSite != null)
			{
				instance.samSite.ResetToPool();
				instance.samSite = null;
			}
			if (instance.eggHunt != null)
			{
				instance.eggHunt.ResetToPool();
				instance.eggHunt = null;
			}
			if (instance.arcadeMachine != null)
			{
				instance.arcadeMachine.ResetToPool();
				instance.arcadeMachine = null;
			}
			instance.createdThisFrame = false;
			Pool.Free<Entity>(ref instance);
		}

		public void ResetToPool()
		{
			Entity.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Entity instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.baseNetworkable != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				BaseNetworkable.Serialize(memoryStream, instance.baseNetworkable);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.baseEntity != null)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				BaseEntity.Serialize(memoryStream, instance.baseEntity);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.basePlayer != null)
			{
				stream.WriteByte(26);
				memoryStream.SetLength((long)0);
				BasePlayer.Serialize(memoryStream, instance.basePlayer);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			if (instance.worldItem != null)
			{
				stream.WriteByte(34);
				memoryStream.SetLength((long)0);
				WorldItem.Serialize(memoryStream, instance.worldItem);
				uint num1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			}
			if (instance.resource != null)
			{
				stream.WriteByte(42);
				memoryStream.SetLength((long)0);
				BaseResource.Serialize(memoryStream, instance.resource);
				uint length2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length2);
			}
			if (instance.buildingBlock != null)
			{
				stream.WriteByte(50);
				memoryStream.SetLength((long)0);
				BuildingBlock.Serialize(memoryStream, instance.buildingBlock);
				uint num2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num2);
			}
			if (instance.environment != null)
			{
				stream.WriteByte(58);
				memoryStream.SetLength((long)0);
				ProtoBuf.Environment.Serialize(memoryStream, instance.environment);
				uint length3 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length3);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length3);
			}
			if (instance.corpse != null)
			{
				stream.WriteByte(66);
				memoryStream.SetLength((long)0);
				Corpse.Serialize(memoryStream, instance.corpse);
				uint num3 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num3);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num3);
			}
			if (instance.parent != null)
			{
				stream.WriteByte(82);
				memoryStream.SetLength((long)0);
				ParentInfo.Serialize(memoryStream, instance.parent);
				uint length4 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length4);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length4);
			}
			if (instance.keyLock != null)
			{
				stream.WriteByte(90);
				memoryStream.SetLength((long)0);
				KeyLock.Serialize(memoryStream, instance.keyLock);
				uint num4 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num4);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num4);
			}
			if (instance.codeLock != null)
			{
				stream.WriteByte(98);
				memoryStream.SetLength((long)0);
				CodeLock.Serialize(memoryStream, instance.codeLock);
				uint length5 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length5);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length5);
			}
			if (instance.entitySlots != null)
			{
				stream.WriteByte(106);
				memoryStream.SetLength((long)0);
				EntitySlots.Serialize(memoryStream, instance.entitySlots);
				uint num5 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num5);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num5);
			}
			if (instance.buildingPrivilege != null)
			{
				stream.WriteByte(114);
				memoryStream.SetLength((long)0);
				BuildingPrivilege.Serialize(memoryStream, instance.buildingPrivilege);
				uint length6 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length6);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length6);
			}
			if (instance.storageBox != null)
			{
				stream.WriteByte(122);
				memoryStream.SetLength((long)0);
				StorageBox.Serialize(memoryStream, instance.storageBox);
				uint num6 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num6);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num6);
			}
			if (instance.heldEntity != null)
			{
				stream.WriteByte(130);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				HeldEntity.Serialize(memoryStream, instance.heldEntity);
				uint length7 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length7);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length7);
			}
			if (instance.baseProjectile != null)
			{
				stream.WriteByte(138);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				BaseProjectile.Serialize(memoryStream, instance.baseProjectile);
				uint num7 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num7);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num7);
			}
			if (instance.baseNPC != null)
			{
				stream.WriteByte(146);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				BaseNPC.Serialize(memoryStream, instance.baseNPC);
				uint length8 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length8);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length8);
			}
			if (instance.loot != null)
			{
				stream.WriteByte(154);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				Loot.Serialize(memoryStream, instance.loot);
				uint num8 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num8);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num8);
			}
			if (instance.genericSpawner != null)
			{
				stream.WriteByte(162);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				GenericSpawner.Serialize(memoryStream, instance.genericSpawner);
				uint length9 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length9);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length9);
			}
			if (instance.sleepingBag != null)
			{
				stream.WriteByte(170);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				SleepingBag.Serialize(memoryStream, instance.sleepingBag);
				uint num9 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num9);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num9);
			}
			if (instance.lootableCorpse != null)
			{
				stream.WriteByte(178);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				LootableCorpse.Serialize(memoryStream, instance.lootableCorpse);
				uint length10 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length10);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length10);
			}
			if (instance.sign != null)
			{
				stream.WriteByte(186);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				Sign.Serialize(memoryStream, instance.sign);
				uint num10 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num10);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num10);
			}
			if (instance.baseCombat != null)
			{
				stream.WriteByte(194);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				BaseCombat.Serialize(memoryStream, instance.baseCombat);
				uint length11 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length11);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length11);
			}
			if (instance.mapEntity != null)
			{
				stream.WriteByte(202);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				MapEntity.Serialize(memoryStream, instance.mapEntity);
				uint num11 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num11);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num11);
			}
			if (instance.researchTable != null)
			{
				stream.WriteByte(210);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				ResearchTable.Serialize(memoryStream, instance.researchTable);
				uint length12 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length12);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length12);
			}
			if (instance.dudExplosive != null)
			{
				stream.WriteByte(218);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				DudExplosive.Serialize(memoryStream, instance.dudExplosive);
				uint num12 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num12);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num12);
			}
			if (instance.miningQuarry != null)
			{
				stream.WriteByte(226);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				MiningQuarry.Serialize(memoryStream, instance.miningQuarry);
				uint length13 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length13);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length13);
			}
			if (instance.plantEntity != null)
			{
				stream.WriteByte(234);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				PlantEntity.Serialize(memoryStream, instance.plantEntity);
				uint num13 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num13);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num13);
			}
			if (instance.helicopter != null)
			{
				stream.WriteByte(242);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				Helicopter.Serialize(memoryStream, instance.helicopter);
				uint length14 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length14);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length14);
			}
			if (instance.landmine != null)
			{
				stream.WriteByte(250);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				Landmine.Serialize(memoryStream, instance.landmine);
				uint num14 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num14);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num14);
			}
			if (instance.autoturret != null)
			{
				stream.WriteByte(130);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				AutoTurret.Serialize(memoryStream, instance.autoturret);
				uint length15 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length15);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length15);
			}
			if (instance.sphereEntity != null)
			{
				stream.WriteByte(138);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				SphereEntity.Serialize(memoryStream, instance.sphereEntity);
				uint num15 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num15);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num15);
			}
			if (instance.stabilityEntity != null)
			{
				stream.WriteByte(146);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				StabilityEntity.Serialize(memoryStream, instance.stabilityEntity);
				uint length16 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length16);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length16);
			}
			if (instance.ownerInfo != null)
			{
				stream.WriteByte(154);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				OwnerInfo.Serialize(memoryStream, instance.ownerInfo);
				uint num16 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num16);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num16);
			}
			if (instance.decayEntity != null)
			{
				stream.WriteByte(162);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				DecayEntity.Serialize(memoryStream, instance.decayEntity);
				uint length17 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length17);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length17);
			}
			if (instance.spawnable != null)
			{
				stream.WriteByte(170);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				Spawnable.Serialize(memoryStream, instance.spawnable);
				uint num17 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num17);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num17);
			}
			if (instance.servergib != null)
			{
				stream.WriteByte(178);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				ServerGib.Serialize(memoryStream, instance.servergib);
				uint length18 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length18);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length18);
			}
			if (instance.vendingMachine != null)
			{
				stream.WriteByte(186);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				VendingMachine.Serialize(memoryStream, instance.vendingMachine);
				uint num18 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num18);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num18);
			}
			if (instance.spinnerWheel != null)
			{
				stream.WriteByte(194);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				SpinnerWheel.Serialize(memoryStream, instance.spinnerWheel);
				uint length19 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length19);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length19);
			}
			if (instance.lift != null)
			{
				stream.WriteByte(202);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				Lift.Serialize(memoryStream, instance.lift);
				uint num19 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num19);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num19);
			}
			if (instance.bradley != null)
			{
				stream.WriteByte(210);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				BradleyAPC.Serialize(memoryStream, instance.bradley);
				uint length20 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length20);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length20);
			}
			if (instance.waterwell != null)
			{
				stream.WriteByte(218);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				WaterWell.Serialize(memoryStream, instance.waterwell);
				uint num20 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num20);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num20);
			}
			if (instance.motorBoat != null)
			{
				stream.WriteByte(226);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				Motorboat.Serialize(memoryStream, instance.motorBoat);
				uint length21 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length21);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length21);
			}
			if (instance.ioEntity != null)
			{
				stream.WriteByte(234);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				IOEntity.Serialize(memoryStream, instance.ioEntity);
				uint num21 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num21);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num21);
			}
			if (instance.puzzleReset != null)
			{
				stream.WriteByte(242);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				PuzzleReset.Serialize(memoryStream, instance.puzzleReset);
				uint length22 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length22);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length22);
			}
			if (instance.relationshipManager != null)
			{
				stream.WriteByte(250);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				RelationshipManager.Serialize(memoryStream, instance.relationshipManager);
				uint num22 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num22);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num22);
			}
			if (instance.hotAirBalloon != null)
			{
				stream.WriteByte(130);
				stream.WriteByte(3);
				memoryStream.SetLength((long)0);
				HotAirBalloon.Serialize(memoryStream, instance.hotAirBalloon);
				uint length23 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length23);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length23);
			}
			if (instance.samSite != null)
			{
				stream.WriteByte(138);
				stream.WriteByte(3);
				memoryStream.SetLength((long)0);
				SAMSite.Serialize(memoryStream, instance.samSite);
				uint num23 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num23);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num23);
			}
			if (instance.eggHunt != null)
			{
				stream.WriteByte(146);
				stream.WriteByte(3);
				memoryStream.SetLength((long)0);
				EggHunt.Serialize(memoryStream, instance.eggHunt);
				uint length24 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length24);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length24);
			}
			if (instance.arcadeMachine != null)
			{
				stream.WriteByte(154);
				stream.WriteByte(3);
				memoryStream.SetLength((long)0);
				ArcadeMachine.Serialize(memoryStream, instance.arcadeMachine);
				uint num24 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num24);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num24);
			}
			stream.WriteByte(160);
			stream.WriteByte(6);
			ProtocolParser.WriteBool(stream, instance.createdThisFrame);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Entity instance, Entity previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.baseNetworkable != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				BaseNetworkable.SerializeDelta(memoryStream, instance.baseNetworkable, previous.baseNetworkable);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.baseEntity != null)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				BaseEntity.SerializeDelta(memoryStream, instance.baseEntity, previous.baseEntity);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.basePlayer != null)
			{
				stream.WriteByte(26);
				memoryStream.SetLength((long)0);
				BasePlayer.SerializeDelta(memoryStream, instance.basePlayer, previous.basePlayer);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			if (instance.worldItem != null)
			{
				stream.WriteByte(34);
				memoryStream.SetLength((long)0);
				WorldItem.SerializeDelta(memoryStream, instance.worldItem, previous.worldItem);
				uint num1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			}
			if (instance.resource != null)
			{
				stream.WriteByte(42);
				memoryStream.SetLength((long)0);
				BaseResource.SerializeDelta(memoryStream, instance.resource, previous.resource);
				uint length2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length2);
			}
			if (instance.buildingBlock != null)
			{
				stream.WriteByte(50);
				memoryStream.SetLength((long)0);
				BuildingBlock.SerializeDelta(memoryStream, instance.buildingBlock, previous.buildingBlock);
				uint num2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num2);
			}
			if (instance.environment != null)
			{
				stream.WriteByte(58);
				memoryStream.SetLength((long)0);
				ProtoBuf.Environment.SerializeDelta(memoryStream, instance.environment, previous.environment);
				uint length3 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length3);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length3);
			}
			if (instance.corpse != null)
			{
				stream.WriteByte(66);
				memoryStream.SetLength((long)0);
				Corpse.SerializeDelta(memoryStream, instance.corpse, previous.corpse);
				uint num3 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num3);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num3);
			}
			if (instance.parent != null)
			{
				stream.WriteByte(82);
				memoryStream.SetLength((long)0);
				ParentInfo.SerializeDelta(memoryStream, instance.parent, previous.parent);
				uint length4 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length4);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length4);
			}
			if (instance.keyLock != null)
			{
				stream.WriteByte(90);
				memoryStream.SetLength((long)0);
				KeyLock.SerializeDelta(memoryStream, instance.keyLock, previous.keyLock);
				uint num4 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num4);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num4);
			}
			if (instance.codeLock != null)
			{
				stream.WriteByte(98);
				memoryStream.SetLength((long)0);
				CodeLock.SerializeDelta(memoryStream, instance.codeLock, previous.codeLock);
				uint length5 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length5);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length5);
			}
			if (instance.entitySlots != null)
			{
				stream.WriteByte(106);
				memoryStream.SetLength((long)0);
				EntitySlots.SerializeDelta(memoryStream, instance.entitySlots, previous.entitySlots);
				uint num5 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num5);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num5);
			}
			if (instance.buildingPrivilege != null)
			{
				stream.WriteByte(114);
				memoryStream.SetLength((long)0);
				BuildingPrivilege.SerializeDelta(memoryStream, instance.buildingPrivilege, previous.buildingPrivilege);
				uint length6 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length6);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length6);
			}
			if (instance.storageBox != null)
			{
				stream.WriteByte(122);
				memoryStream.SetLength((long)0);
				StorageBox.SerializeDelta(memoryStream, instance.storageBox, previous.storageBox);
				uint num6 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num6);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num6);
			}
			if (instance.heldEntity != null)
			{
				stream.WriteByte(130);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				HeldEntity.SerializeDelta(memoryStream, instance.heldEntity, previous.heldEntity);
				uint length7 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length7);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length7);
			}
			if (instance.baseProjectile != null)
			{
				stream.WriteByte(138);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				BaseProjectile.SerializeDelta(memoryStream, instance.baseProjectile, previous.baseProjectile);
				uint num7 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num7);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num7);
			}
			if (instance.baseNPC != null)
			{
				stream.WriteByte(146);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				BaseNPC.SerializeDelta(memoryStream, instance.baseNPC, previous.baseNPC);
				uint length8 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length8);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length8);
			}
			if (instance.loot != null)
			{
				stream.WriteByte(154);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				Loot.SerializeDelta(memoryStream, instance.loot, previous.loot);
				uint num8 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num8);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num8);
			}
			if (instance.genericSpawner != null)
			{
				stream.WriteByte(162);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				GenericSpawner.SerializeDelta(memoryStream, instance.genericSpawner, previous.genericSpawner);
				uint length9 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length9);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length9);
			}
			if (instance.sleepingBag != null)
			{
				stream.WriteByte(170);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				SleepingBag.SerializeDelta(memoryStream, instance.sleepingBag, previous.sleepingBag);
				uint num9 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num9);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num9);
			}
			if (instance.lootableCorpse != null)
			{
				stream.WriteByte(178);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				LootableCorpse.SerializeDelta(memoryStream, instance.lootableCorpse, previous.lootableCorpse);
				uint length10 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length10);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length10);
			}
			if (instance.sign != null)
			{
				stream.WriteByte(186);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				Sign.SerializeDelta(memoryStream, instance.sign, previous.sign);
				uint num10 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num10);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num10);
			}
			if (instance.baseCombat != null)
			{
				stream.WriteByte(194);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				BaseCombat.SerializeDelta(memoryStream, instance.baseCombat, previous.baseCombat);
				uint length11 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length11);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length11);
			}
			if (instance.mapEntity != null)
			{
				stream.WriteByte(202);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				MapEntity.SerializeDelta(memoryStream, instance.mapEntity, previous.mapEntity);
				uint num11 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num11);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num11);
			}
			if (instance.researchTable != null)
			{
				stream.WriteByte(210);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				ResearchTable.SerializeDelta(memoryStream, instance.researchTable, previous.researchTable);
				uint length12 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length12);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length12);
			}
			if (instance.dudExplosive != null)
			{
				stream.WriteByte(218);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				DudExplosive.SerializeDelta(memoryStream, instance.dudExplosive, previous.dudExplosive);
				uint num12 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num12);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num12);
			}
			if (instance.miningQuarry != null)
			{
				stream.WriteByte(226);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				MiningQuarry.SerializeDelta(memoryStream, instance.miningQuarry, previous.miningQuarry);
				uint length13 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length13);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length13);
			}
			if (instance.plantEntity != null)
			{
				stream.WriteByte(234);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				PlantEntity.SerializeDelta(memoryStream, instance.plantEntity, previous.plantEntity);
				uint num13 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num13);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num13);
			}
			if (instance.helicopter != null)
			{
				stream.WriteByte(242);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				Helicopter.SerializeDelta(memoryStream, instance.helicopter, previous.helicopter);
				uint length14 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length14);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length14);
			}
			if (instance.landmine != null)
			{
				stream.WriteByte(250);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				Landmine.SerializeDelta(memoryStream, instance.landmine, previous.landmine);
				uint num14 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num14);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num14);
			}
			if (instance.autoturret != null)
			{
				stream.WriteByte(130);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				AutoTurret.SerializeDelta(memoryStream, instance.autoturret, previous.autoturret);
				uint length15 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length15);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length15);
			}
			if (instance.sphereEntity != null)
			{
				stream.WriteByte(138);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				SphereEntity.SerializeDelta(memoryStream, instance.sphereEntity, previous.sphereEntity);
				uint num15 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num15);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num15);
			}
			if (instance.stabilityEntity != null)
			{
				stream.WriteByte(146);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				StabilityEntity.SerializeDelta(memoryStream, instance.stabilityEntity, previous.stabilityEntity);
				uint length16 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length16);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length16);
			}
			if (instance.ownerInfo != null)
			{
				stream.WriteByte(154);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				OwnerInfo.SerializeDelta(memoryStream, instance.ownerInfo, previous.ownerInfo);
				uint num16 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num16);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num16);
			}
			if (instance.decayEntity != null)
			{
				stream.WriteByte(162);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				DecayEntity.SerializeDelta(memoryStream, instance.decayEntity, previous.decayEntity);
				uint length17 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length17);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length17);
			}
			if (instance.spawnable != null)
			{
				stream.WriteByte(170);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				Spawnable.SerializeDelta(memoryStream, instance.spawnable, previous.spawnable);
				uint num17 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num17);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num17);
			}
			if (instance.servergib != null)
			{
				stream.WriteByte(178);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				ServerGib.SerializeDelta(memoryStream, instance.servergib, previous.servergib);
				uint length18 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length18);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length18);
			}
			if (instance.vendingMachine != null)
			{
				stream.WriteByte(186);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				VendingMachine.SerializeDelta(memoryStream, instance.vendingMachine, previous.vendingMachine);
				uint num18 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num18);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num18);
			}
			if (instance.spinnerWheel != null)
			{
				stream.WriteByte(194);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				SpinnerWheel.SerializeDelta(memoryStream, instance.spinnerWheel, previous.spinnerWheel);
				uint length19 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length19);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length19);
			}
			if (instance.lift != null)
			{
				stream.WriteByte(202);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				Lift.SerializeDelta(memoryStream, instance.lift, previous.lift);
				uint num19 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num19);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num19);
			}
			if (instance.bradley != null)
			{
				stream.WriteByte(210);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				BradleyAPC.SerializeDelta(memoryStream, instance.bradley, previous.bradley);
				uint length20 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length20);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length20);
			}
			if (instance.waterwell != null)
			{
				stream.WriteByte(218);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				WaterWell.SerializeDelta(memoryStream, instance.waterwell, previous.waterwell);
				uint num20 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num20);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num20);
			}
			if (instance.motorBoat != null)
			{
				stream.WriteByte(226);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				Motorboat.SerializeDelta(memoryStream, instance.motorBoat, previous.motorBoat);
				uint length21 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length21);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length21);
			}
			if (instance.ioEntity != null)
			{
				stream.WriteByte(234);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				IOEntity.SerializeDelta(memoryStream, instance.ioEntity, previous.ioEntity);
				uint num21 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num21);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num21);
			}
			if (instance.puzzleReset != null)
			{
				stream.WriteByte(242);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				PuzzleReset.SerializeDelta(memoryStream, instance.puzzleReset, previous.puzzleReset);
				uint length22 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length22);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length22);
			}
			if (instance.relationshipManager != null)
			{
				stream.WriteByte(250);
				stream.WriteByte(2);
				memoryStream.SetLength((long)0);
				RelationshipManager.SerializeDelta(memoryStream, instance.relationshipManager, previous.relationshipManager);
				uint num22 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num22);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num22);
			}
			if (instance.hotAirBalloon != null)
			{
				stream.WriteByte(130);
				stream.WriteByte(3);
				memoryStream.SetLength((long)0);
				HotAirBalloon.SerializeDelta(memoryStream, instance.hotAirBalloon, previous.hotAirBalloon);
				uint length23 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length23);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length23);
			}
			if (instance.samSite != null)
			{
				stream.WriteByte(138);
				stream.WriteByte(3);
				memoryStream.SetLength((long)0);
				SAMSite.SerializeDelta(memoryStream, instance.samSite, previous.samSite);
				uint num23 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num23);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num23);
			}
			if (instance.eggHunt != null)
			{
				stream.WriteByte(146);
				stream.WriteByte(3);
				memoryStream.SetLength((long)0);
				EggHunt.SerializeDelta(memoryStream, instance.eggHunt, previous.eggHunt);
				uint length24 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length24);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length24);
			}
			if (instance.arcadeMachine != null)
			{
				stream.WriteByte(154);
				stream.WriteByte(3);
				memoryStream.SetLength((long)0);
				ArcadeMachine.SerializeDelta(memoryStream, instance.arcadeMachine, previous.arcadeMachine);
				uint num24 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num24);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num24);
			}
			stream.WriteByte(160);
			stream.WriteByte(6);
			ProtocolParser.WriteBool(stream, instance.createdThisFrame);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Entity instance)
		{
			byte[] bytes = Entity.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Entity instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Entity.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Entity.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Entity.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Entity.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Entity previous)
		{
			if (previous == null)
			{
				Entity.Serialize(stream, this);
				return;
			}
			Entity.SerializeDelta(stream, this, previous);
		}
	}
}