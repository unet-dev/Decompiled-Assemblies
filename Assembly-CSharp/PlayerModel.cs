using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerModel : ListComponent<PlayerModel>
{
	protected static int speed;

	protected static int acceleration;

	protected static int rotationYaw;

	protected static int forward;

	protected static int right;

	protected static int up;

	protected static int ducked;

	protected static int grounded;

	protected static int waterlevel;

	protected static int attack;

	protected static int attack_alt;

	protected static int deploy;

	protected static int reload;

	protected static int throwWeapon;

	protected static int holster;

	protected static int aiming;

	protected static int onLadder;

	protected static int posing;

	protected static int poseType;

	protected static int relaxGunPose;

	protected static int vehicle_aim_yaw;

	protected static int vehicle_aim_speed;

	protected static int leftFootIK;

	protected static int rightFootIK;

	public BoxCollider collision;

	public GameObject censorshipCube;

	public GameObject censorshipCubeBreasts;

	public GameObject jawBone;

	public GameObject neckBone;

	public GameObject headBone;

	public SkeletonScale skeletonScale;

	public EyeController eyeController;

	public Transform[] SpineBones;

	public Transform leftFootBone;

	public Transform rightFootBone;

	public Vector3 rightHandTarget;

	public Vector3 leftHandTargetPosition;

	public Quaternion leftHandTargetRotation;

	public RuntimeAnimatorController DefaultHoldType;

	public RuntimeAnimatorController SleepGesture;

	public RuntimeAnimatorController WoundedGesture;

	public RuntimeAnimatorController CurrentGesture;

	[Header("Skin")]
	public SkinSetCollection MaleSkin;

	public SkinSetCollection FemaleSkin;

	public SubsurfaceProfile subsurfaceProfile;

	[Header("Parameters")]
	[Range(0f, 1f)]
	public float voiceVolume;

	[Range(0f, 1f)]
	public float skinColor = 1f;

	[Range(0f, 1f)]
	public float skinNumber = 1f;

	[Range(0f, 1f)]
	public float meshNumber;

	[Range(0f, 1f)]
	public float hairNumber;

	[Range(0f, 1f)]
	public int skinType;

	public MovementSounds movementSounds;

	public bool IsFemale
	{
		get
		{
			return this.skinType == 1;
		}
	}

	public Quaternion LookAngles
	{
		get;
		set;
	}

	public SkinSetCollection SkinSet
	{
		get
		{
			if (this.IsFemale)
			{
				return this.FemaleSkin;
			}
			return this.MaleSkin;
		}
	}

	static PlayerModel()
	{
		PlayerModel.speed = Animator.StringToHash("speed");
		PlayerModel.acceleration = Animator.StringToHash("acceleration");
		PlayerModel.rotationYaw = Animator.StringToHash("rotationYaw");
		PlayerModel.forward = Animator.StringToHash("forward");
		PlayerModel.right = Animator.StringToHash("right");
		PlayerModel.up = Animator.StringToHash("up");
		PlayerModel.ducked = Animator.StringToHash("ducked");
		PlayerModel.grounded = Animator.StringToHash("grounded");
		PlayerModel.waterlevel = Animator.StringToHash("waterlevel");
		PlayerModel.attack = Animator.StringToHash("attack");
		PlayerModel.attack_alt = Animator.StringToHash("attack_alt");
		PlayerModel.deploy = Animator.StringToHash("deploy");
		PlayerModel.reload = Animator.StringToHash("reload");
		PlayerModel.throwWeapon = Animator.StringToHash("throw");
		PlayerModel.holster = Animator.StringToHash("holster");
		PlayerModel.aiming = Animator.StringToHash("aiming");
		PlayerModel.onLadder = Animator.StringToHash("onLadder");
		PlayerModel.posing = Animator.StringToHash("posing");
		PlayerModel.poseType = Animator.StringToHash("poseType");
		PlayerModel.relaxGunPose = Animator.StringToHash("relaxGunPose");
		PlayerModel.vehicle_aim_yaw = Animator.StringToHash("vehicleAimYaw");
		PlayerModel.vehicle_aim_speed = Animator.StringToHash("vehicleAimYawSpeed");
		PlayerModel.leftFootIK = Animator.StringToHash("leftFootIK");
		PlayerModel.rightFootIK = Animator.StringToHash("rightFootIK");
	}

	public PlayerModel()
	{
	}

	public enum MountPoses
	{
		Chair = 0,
		Driving = 1,
		Horseback = 2,
		HeliUnarmed = 3,
		HeliArmed = 4,
		HandMotorBoat = 5,
		MotorBoatPassenger = 6,
		SitGeneric = 7,
		SitRaft = 8,
		StandDrive = 9,
		SitShootingGeneric = 10,
		SitMinicopter_Pilot = 11,
		SitMinicopter_Passenger = 12,
		Standing = 128
	}
}