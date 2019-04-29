using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class UVTextureAnimator : MonoBehaviour
{
	public int Rows = 4;

	public int Columns = 4;

	public float Fps = 20f;

	public int OffsetMat;

	public bool IsLoop = true;

	public float StartDelay;

	private bool isInizialised;

	private int index;

	private int count;

	private int allCount;

	private float deltaFps;

	private bool isVisible;

	private bool isCorutineStarted;

	private Renderer currentRenderer;

	private Material instanceMaterial;

	public UVTextureAnimator()
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
		this.deltaFps = 1f / this.Fps;
		this.count = this.Rows * this.Columns;
		this.index = this.Columns - 1;
		Vector3 vector3 = Vector3.zero;
		this.OffsetMat = this.OffsetMat - this.OffsetMat / this.count * this.count;
		Vector2 vector2 = new Vector2(1f / (float)this.Columns, 1f / (float)this.Rows);
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

	private IEnumerator UpdateCorutine()
	{
		UVTextureAnimator uVTextureAnimator = null;
		while (uVTextureAnimator.isVisible && (uVTextureAnimator.IsLoop || uVTextureAnimator.allCount != uVTextureAnimator.count))
		{
			uVTextureAnimator.UpdateCorutineFrame();
			if (!uVTextureAnimator.IsLoop && uVTextureAnimator.allCount == uVTextureAnimator.count)
			{
				break;
			}
			yield return new WaitForSeconds(uVTextureAnimator.deltaFps);
		}
		uVTextureAnimator.isCorutineStarted = false;
		uVTextureAnimator.currentRenderer.enabled = false;
	}

	private void UpdateCorutineFrame()
	{
		this.allCount++;
		this.index++;
		if (this.index >= this.count)
		{
			this.index = 0;
		}
		Vector2 vector2 = new Vector2((float)this.index / (float)this.Columns - (float)(this.index / this.Columns), 1f - (float)(this.index / this.Columns) / (float)this.Rows);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetTextureOffset("_MainTex", vector2);
		}
	}
}