using System;
using UnityEngine;

public class m2bradleyAnimator : MonoBehaviour
{
	public Animator m2Animator;

	public Material treadLeftMaterial;

	public Material treadRightMaterial;

	private Rigidbody mainRigidbody;

	[Header("GunBones")]
	public Transform turret;

	public Transform mainCannon;

	public Transform coaxGun;

	public Transform rocketsPitch;

	public Transform spotLightYaw;

	public Transform spotLightPitch;

	public Transform sideMG;

	public Transform[] sideguns;

	[Header("WheelBones")]
	public Transform[] ShocksBones;

	public Transform[] ShockTraceLineBegin;

	public Vector3[] vecShocksOffsetPosition;

	[Header("Targeting")]
	public Transform targetTurret;

	public Transform targetSpotLight;

	public Transform[] targetSideguns;

	private Vector3 vecTurret = new Vector3(0f, 0f, 0f);

	private Vector3 vecMainCannon = new Vector3(0f, 0f, 0f);

	private Vector3 vecCoaxGun = new Vector3(0f, 0f, 0f);

	private Vector3 vecRocketsPitch = new Vector3(0f, 0f, 0f);

	private Vector3 vecSpotLightBase = new Vector3(0f, 0f, 0f);

	private Vector3 vecSpotLight = new Vector3(0f, 0f, 0f);

	private float sideMGPitchValue;

	[Header("MuzzleFlash locations")]
	public GameObject muzzleflashCannon;

	public GameObject muzzleflashCoaxGun;

	public GameObject muzzleflashSideMG;

	public GameObject[] muzzleflashRockets;

	public GameObject spotLightHaloSawnpoint;

	public GameObject[] muzzleflashSideguns;

	[Header("MuzzleFlash Particle Systems")]
	public GameObjectRef machineGunMuzzleFlashFX;

	public GameObjectRef mainCannonFireFX;

	public GameObjectRef rocketLaunchFX;

	[Header("Misc")]
	public bool rocketsOpen;

	public Vector3[] vecSideGunRotation;

	public float treadConstant = 0.14f;

	public float wheelSpinConstant = 80f;

	[Header("Gun Movement speeds")]
	public float sidegunsTurnSpeed = 30f;

	public float turretTurnSpeed = 6f;

	public float cannonPitchSpeed = 10f;

	public float rocketPitchSpeed = 20f;

	public float spotLightTurnSpeed = 60f;

	public float machineGunSpeed = 20f;

	private float wheelAngle;

	public m2bradleyAnimator()
	{
	}

	private void AdjustShocksHeight()
	{
		RaycastHit raycastHit;
		Ray shockTraceLineBegin = new Ray();
		int mask = LayerMask.GetMask(new string[] { "Terrain", "World", "Construction" });
		int length = (int)this.ShocksBones.Length;
		float single = 0.55f;
		float single1 = 0.79f;
		float single2 = 0.26f;
		for (int i = 0; i < length; i++)
		{
			shockTraceLineBegin.origin = this.ShockTraceLineBegin[i].position;
			shockTraceLineBegin.direction = base.transform.up * -1f;
			single2 = (!Physics.SphereCast(shockTraceLineBegin, 0.15f, out raycastHit, single1, mask) ? 0.26f : raycastHit.distance - single);
			this.vecShocksOffsetPosition[i].y = Mathf.Lerp(this.vecShocksOffsetPosition[i].y, Mathf.Clamp(single2 * -1f, -0.26f, 0f), Time.deltaTime * 5f);
			this.ShocksBones[i].localPosition = this.vecShocksOffsetPosition[i];
		}
	}

	private void AnimateWheelsTreads()
	{
		float single = 0f;
		if (this.mainRigidbody != null)
		{
			single = Vector3.Dot(this.mainRigidbody.velocity, base.transform.forward);
		}
		float single1 = Time.time * -1f * single * this.treadConstant % 1f;
		this.treadLeftMaterial.SetTextureOffset("_MainTex", new Vector2(single1, 0f));
		this.treadLeftMaterial.SetTextureOffset("_BumpMap", new Vector2(single1, 0f));
		this.treadLeftMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(single1, 0f));
		this.treadRightMaterial.SetTextureOffset("_MainTex", new Vector2(single1, 0f));
		this.treadRightMaterial.SetTextureOffset("_BumpMap", new Vector2(single1, 0f));
		this.treadRightMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(single1, 0f));
		if (single < 0f)
		{
			this.wheelAngle = this.wheelAngle + Time.deltaTime * single * this.wheelSpinConstant;
			if (this.wheelAngle <= 0f)
			{
				this.wheelAngle = 360f;
			}
		}
		else
		{
			this.wheelAngle = (this.wheelAngle + Time.deltaTime * single * this.wheelSpinConstant) % 360f;
		}
		this.m2Animator.SetFloat("wheel_spin", this.wheelAngle);
		this.m2Animator.SetFloat("speed", single);
	}

	public void CalculateYawPitchOffset(Transform objectTransform, Vector3 vecStart, Vector3 vecEnd, out float yaw, out float pitch)
	{
		Vector3 vector3 = objectTransform.InverseTransformDirection(vecEnd - vecStart);
		float single = Mathf.Sqrt(vector3.x * vector3.x + vector3.z * vector3.z);
		pitch = -Mathf.Atan2(vector3.y, single) * 57.2957764f;
		vector3 = (vecEnd - vecStart).normalized;
		Vector3 vector31 = objectTransform.forward;
		vector31.y = 0f;
		vector31.Normalize();
		float single1 = Vector3.Dot(vector3, vector31);
		float single2 = Vector3.Dot(vector3, objectTransform.right);
		float single3 = 360f * single2;
		float single4 = 360f * -single1;
		yaw = (Mathf.Atan2(single3, single4) + 3.14159274f) * 57.2957764f;
	}

	public float NormalizeYaw(float flYaw)
	{
		float single;
		single = (flYaw <= 180f ? flYaw * -1f : 360f - flYaw);
		return single;
	}

	private void Start()
	{
		this.mainRigidbody = base.GetComponent<Rigidbody>();
		for (int i = 0; i < (int)this.ShocksBones.Length; i++)
		{
			this.vecShocksOffsetPosition[i] = this.ShocksBones[i].localPosition;
		}
	}

	private void TrackSideGuns()
	{
		float single;
		float single1;
		for (int i = 0; i < (int)this.sideguns.Length; i++)
		{
			if (this.targetSideguns[i] != null)
			{
				Vector3 vector3 = this.targetSideguns[i].position - this.sideguns[i].position;
				Vector3 vector31 = vector3.normalized;
				this.CalculateYawPitchOffset(this.sideguns[i], this.sideguns[i].position, this.targetSideguns[i].position, out single, out single1);
				single = this.NormalizeYaw(single);
				float single2 = Time.deltaTime * this.sidegunsTurnSpeed;
				if (single < -0.5f)
				{
					this.vecSideGunRotation[i].y -= single2;
				}
				else if (single > 0.5f)
				{
					this.vecSideGunRotation[i].y += single2;
				}
				if (single1 < -0.5f)
				{
					this.vecSideGunRotation[i].x -= single2;
				}
				else if (single1 > 0.5f)
				{
					this.vecSideGunRotation[i].x += single2;
				}
				this.vecSideGunRotation[i].x = Mathf.Clamp(this.vecSideGunRotation[i].x, -45f, 45f);
				this.vecSideGunRotation[i].y = Mathf.Clamp(this.vecSideGunRotation[i].y, -45f, 45f);
				this.sideguns[i].localEulerAngles = this.vecSideGunRotation[i];
			}
		}
	}

	private void TrackSpotLight()
	{
		float single;
		float single1;
		if (this.targetSpotLight != null)
		{
			Vector3 vector3 = this.targetSpotLight.position - this.spotLightYaw.position;
			Vector3 vector31 = vector3.normalized;
			this.CalculateYawPitchOffset(this.spotLightYaw, this.spotLightYaw.position, this.targetSpotLight.position, out single, out single1);
			single = this.NormalizeYaw(single);
			float single2 = Time.deltaTime * this.spotLightTurnSpeed;
			if (single < -0.5f)
			{
				this.vecSpotLightBase.y = (this.vecSpotLightBase.y - single2) % 360f;
			}
			else if (single > 0.5f)
			{
				this.vecSpotLightBase.y = (this.vecSpotLightBase.y + single2) % 360f;
			}
			this.spotLightYaw.localEulerAngles = this.vecSpotLightBase;
			this.CalculateYawPitchOffset(this.spotLightPitch, this.spotLightPitch.position, this.targetSpotLight.position, out single, out single1);
			if (single1 < -0.5f)
			{
				this.vecSpotLight.x -= single2;
			}
			else if (single1 > 0.5f)
			{
				this.vecSpotLight.x += single2;
			}
			this.vecSpotLight.x = Mathf.Clamp(this.vecSpotLight.x, -50f, 50f);
			this.spotLightPitch.localEulerAngles = this.vecSpotLight;
			this.m2Animator.SetFloat("sideMG_pitch", this.vecSpotLight.x, 0.5f, Time.deltaTime);
		}
	}

	private void TrackTurret()
	{
		float single;
		float single1;
		if (this.targetTurret != null)
		{
			Vector3 vector3 = this.targetTurret.position - this.turret.position;
			Vector3 vector31 = vector3.normalized;
			this.CalculateYawPitchOffset(this.turret, this.turret.position, this.targetTurret.position, out single, out single1);
			single = this.NormalizeYaw(single);
			float single2 = Time.deltaTime * this.turretTurnSpeed;
			if (single < -0.5f)
			{
				this.vecTurret.y = (this.vecTurret.y - single2) % 360f;
			}
			else if (single > 0.5f)
			{
				this.vecTurret.y = (this.vecTurret.y + single2) % 360f;
			}
			this.turret.localEulerAngles = this.vecTurret;
			float single3 = Time.deltaTime * this.cannonPitchSpeed;
			this.CalculateYawPitchOffset(this.mainCannon, this.mainCannon.position, this.targetTurret.position, out single, out single1);
			if (single1 < -0.5f)
			{
				this.vecMainCannon.x -= single3;
			}
			else if (single1 > 0.5f)
			{
				this.vecMainCannon.x += single3;
			}
			this.vecMainCannon.x = Mathf.Clamp(this.vecMainCannon.x, -55f, 5f);
			this.mainCannon.localEulerAngles = this.vecMainCannon;
			if (single1 < -0.5f)
			{
				this.vecCoaxGun.x -= single3;
			}
			else if (single1 > 0.5f)
			{
				this.vecCoaxGun.x += single3;
			}
			this.vecCoaxGun.x = Mathf.Clamp(this.vecCoaxGun.x, -65f, 15f);
			this.coaxGun.localEulerAngles = this.vecCoaxGun;
			if (!this.rocketsOpen)
			{
				this.vecRocketsPitch.x = Mathf.Lerp(this.vecRocketsPitch.x, 0f, Time.deltaTime * 1.7f);
			}
			else
			{
				single3 = Time.deltaTime * this.rocketPitchSpeed;
				this.CalculateYawPitchOffset(this.rocketsPitch, this.rocketsPitch.position, this.targetTurret.position, out single, out single1);
				if (single1 < -0.5f)
				{
					this.vecRocketsPitch.x -= single3;
				}
				else if (single1 > 0.5f)
				{
					this.vecRocketsPitch.x += single3;
				}
				this.vecRocketsPitch.x = Mathf.Clamp(this.vecRocketsPitch.x, -45f, 45f);
			}
			this.rocketsPitch.localEulerAngles = this.vecRocketsPitch;
		}
	}

	private void Update()
	{
		this.TrackTurret();
		this.TrackSpotLight();
		this.TrackSideGuns();
		this.AnimateWheelsTreads();
		this.AdjustShocksHeight();
		this.m2Animator.SetBool("rocketpods", this.rocketsOpen);
	}
}