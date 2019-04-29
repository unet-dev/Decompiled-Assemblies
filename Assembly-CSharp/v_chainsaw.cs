using System;
using UnityEngine;

public class v_chainsaw : MonoBehaviour
{
	public bool bAttacking;

	public bool bHitMetal;

	public bool bHitWood;

	public bool bHitFlesh;

	public bool bEngineOn;

	public ParticleSystem[] hitMetalFX;

	public ParticleSystem[] hitWoodFX;

	public ParticleSystem[] hitFleshFX;

	public SoundDefinition hitMetalSoundDef;

	public SoundDefinition hitWoodSoundDef;

	public SoundDefinition hitFleshSoundDef;

	public Sound hitSound;

	public GameObject hitSoundTarget;

	public float hitSoundFadeTime = 0.1f;

	public ParticleSystem smokeEffect;

	public Animator chainsawAnimator;

	public Renderer chainRenderer;

	public Material chainlink;

	private MaterialPropertyBlock block;

	private Vector2 saveST;

	private float chainSpeed;

	private float chainAmount;

	public float temp1;

	public float temp2;

	public v_chainsaw()
	{
	}

	private void Awake()
	{
		this.chainlink = this.chainRenderer.sharedMaterial;
	}

	private void DoHitSound(SoundDefinition soundDef)
	{
	}

	public void OnEnable()
	{
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		this.saveST = this.chainRenderer.sharedMaterial.GetVector("_MainTex_ST");
	}

	private void ScrollChainTexture()
	{
		float single = (this.chainAmount + Time.deltaTime * this.chainSpeed) % 1f;
		float single1 = single;
		this.chainAmount = single;
		float single2 = single1;
		this.block.Clear();
		this.block.SetVector("_MainTex_ST", new Vector4(this.saveST.x, this.saveST.y, single2, 0f));
		this.chainRenderer.SetPropertyBlock(this.block);
	}

	private void Start()
	{
	}

	private void Update()
	{
		ParticleSystem[] particleSystemArray;
		int i;
		this.chainsawAnimator.SetBool("attacking", this.bAttacking);
		this.smokeEffect.enableEmission = this.bEngineOn;
		if (this.bHitMetal)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			particleSystemArray = this.hitMetalFX;
			for (i = 0; i < (int)particleSystemArray.Length; i++)
			{
				particleSystemArray[i].enableEmission = true;
			}
			particleSystemArray = this.hitWoodFX;
			for (i = 0; i < (int)particleSystemArray.Length; i++)
			{
				particleSystemArray[i].enableEmission = false;
			}
			particleSystemArray = this.hitFleshFX;
			for (i = 0; i < (int)particleSystemArray.Length; i++)
			{
				particleSystemArray[i].enableEmission = false;
			}
			this.DoHitSound(this.hitMetalSoundDef);
			return;
		}
		if (this.bHitWood)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			particleSystemArray = this.hitMetalFX;
			for (i = 0; i < (int)particleSystemArray.Length; i++)
			{
				particleSystemArray[i].enableEmission = false;
			}
			particleSystemArray = this.hitWoodFX;
			for (i = 0; i < (int)particleSystemArray.Length; i++)
			{
				particleSystemArray[i].enableEmission = true;
			}
			particleSystemArray = this.hitFleshFX;
			for (i = 0; i < (int)particleSystemArray.Length; i++)
			{
				particleSystemArray[i].enableEmission = false;
			}
			this.DoHitSound(this.hitWoodSoundDef);
			return;
		}
		if (!this.bHitFlesh)
		{
			this.chainsawAnimator.SetBool("attackHit", false);
			particleSystemArray = this.hitMetalFX;
			for (i = 0; i < (int)particleSystemArray.Length; i++)
			{
				particleSystemArray[i].enableEmission = false;
			}
			particleSystemArray = this.hitWoodFX;
			for (i = 0; i < (int)particleSystemArray.Length; i++)
			{
				particleSystemArray[i].enableEmission = false;
			}
			particleSystemArray = this.hitFleshFX;
			for (i = 0; i < (int)particleSystemArray.Length; i++)
			{
				particleSystemArray[i].enableEmission = false;
			}
			return;
		}
		this.chainsawAnimator.SetBool("attackHit", true);
		particleSystemArray = this.hitMetalFX;
		for (i = 0; i < (int)particleSystemArray.Length; i++)
		{
			particleSystemArray[i].enableEmission = false;
		}
		particleSystemArray = this.hitWoodFX;
		for (i = 0; i < (int)particleSystemArray.Length; i++)
		{
			particleSystemArray[i].enableEmission = false;
		}
		particleSystemArray = this.hitFleshFX;
		for (i = 0; i < (int)particleSystemArray.Length; i++)
		{
			particleSystemArray[i].enableEmission = true;
		}
		this.DoHitSound(this.hitFleshSoundDef);
	}
}