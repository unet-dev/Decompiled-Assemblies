using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class ExplosionsSpriteSheetAnimation : MonoBehaviour
{
	public int TilesX = 4;

	public int TilesY = 4;

	public float AnimationFPS = 30f;

	public bool IsInterpolateFrames;

	public int StartFrameOffset;

	public bool IsLoop = true;

	public float StartDelay;

	public AnimationCurve FrameOverTime = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	private bool isInizialised;

	private int index;

	private int count;

	private int allCount;

	private float animationLifeTime;

	private bool isVisible;

	private bool isCorutineStarted;

	private Renderer currentRenderer;

	private Material instanceMaterial;

	private float currentInterpolatedTime;

	private float animationStartTime;

	private bool animationStoped;

	public ExplosionsSpriteSheetAnimation()
	{
	}

	private void InitDefaultVariables()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		if (this.currentRenderer == null)
		{
			throw new Exception("UvTextureAnimator can't get renderer");
		}
		if (!this.currentRenderer.enabled)
		{
			this.currentRenderer.enabled = true;
		}
		this.allCount = 0;
		this.animationStoped = false;
		this.animationLifeTime = (float)(this.TilesX * this.TilesY) / this.AnimationFPS;
		this.count = this.TilesY * this.TilesX;
		this.index = this.TilesX - 1;
		Vector3 vector3 = Vector3.zero;
		this.StartFrameOffset = this.StartFrameOffset - this.StartFrameOffset / this.count * this.count;
		Vector2 vector2 = new Vector2(1f / (float)this.TilesX, 1f / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial = this.currentRenderer.material;
			this.instanceMaterial.SetTextureScale("_MainTex", vector2);
			this.instanceMaterial.SetTextureOffset("_MainTex", vector3);
		}
	}

	private void OnDestroy()
	{
		if (this.instanceMaterial != null)
		{
			UnityEngine.Object.Destroy(this.instanceMaterial);
			this.instanceMaterial = null;
		}
	}

	private void OnDisable()
	{
		this.isCorutineStarted = false;
		this.isVisible = false;
		base.StopAllCoroutines();
		base.CancelInvoke("PlayDelay");
	}

	private void OnEnable()
	{
		if (!this.isInizialised)
		{
			return;
		}
		this.InitDefaultVariables();
		this.isVisible = true;
		this.Play();
	}

	private void Play()
	{
		if (this.isCorutineStarted)
		{
			return;
		}
		if (this.StartDelay <= 0.0001f)
		{
			base.StartCoroutine(this.UpdateCorutine());
		}
		else
		{
			base.Invoke("PlayDelay", this.StartDelay);
		}
		this.isCorutineStarted = true;
	}

	private void PlayDelay()
	{
		base.StartCoroutine(this.UpdateCorutine());
	}

	private void Start()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		this.InitDefaultVariables();
		this.isInizialised = true;
		this.isVisible = true;
		this.Play();
	}

	private void Update()
	{
		if (!this.IsInterpolateFrames)
		{
			return;
		}
		this.currentInterpolatedTime += Time.deltaTime;
		int num = this.index + 1;
		if (this.allCount == 0)
		{
			num = this.index;
		}
		Vector4 vector4 = new Vector4(1f / (float)this.TilesX, 1f / (float)this.TilesY, (float)num / (float)this.TilesX - (float)(num / this.TilesX), 1f - (float)(num / this.TilesX) / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetVector("_MainTex_NextFrame", vector4);
			float single = (Time.time - this.animationStartTime) / this.animationLifeTime;
			float single1 = this.FrameOverTime.Evaluate(Mathf.Clamp01(single));
			this.instanceMaterial.SetFloat("InterpolationValue", Mathf.Clamp01(this.currentInterpolatedTime * this.AnimationFPS * single1));
		}
	}

	private IEnumerator UpdateCorutine()
	{
		ExplosionsSpriteSheetAnimation explosionsSpriteSheetAnimation = null;
		explosionsSpriteSheetAnimation.animationStartTime = Time.time;
		while (explosionsSpriteSheetAnimation.isVisible && (explosionsSpriteSheetAnimation.IsLoop || !explosionsSpriteSheetAnimation.animationStoped))
		{
			explosionsSpriteSheetAnimation.UpdateFrame();
			if (!explosionsSpriteSheetAnimation.IsLoop && explosionsSpriteSheetAnimation.animationStoped)
			{
				break;
			}
			float single = (Time.time - explosionsSpriteSheetAnimation.animationStartTime) / explosionsSpriteSheetAnimation.animationLifeTime;
			float single1 = explosionsSpriteSheetAnimation.FrameOverTime.Evaluate(Mathf.Clamp01(single));
			yield return new WaitForSeconds(1f / (explosionsSpriteSheetAnimation.AnimationFPS * single1));
		}
		explosionsSpriteSheetAnimation.isCorutineStarted = false;
		explosionsSpriteSheetAnimation.currentRenderer.enabled = false;
	}

	private void UpdateFrame()
	{
		this.allCount++;
		this.index++;
		if (this.index >= this.count)
		{
			this.index = 0;
		}
		if (this.count == this.allCount)
		{
			this.animationStartTime = Time.time;
			this.allCount = 0;
			this.animationStoped = true;
		}
		Vector2 vector2 = new Vector2((float)this.index / (float)this.TilesX - (float)(this.index / this.TilesX), 1f - (float)(this.index / this.TilesX) / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetTextureOffset("_MainTex", vector2);
		}
		if (this.IsInterpolateFrames)
		{
			this.currentInterpolatedTime = 0f;
		}
	}
}