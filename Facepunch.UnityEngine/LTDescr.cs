using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class LTDescr
{
	public bool toggle;

	public bool useEstimatedTime;

	public bool useFrames;

	public bool useManualTime;

	public bool usesNormalDt;

	public bool hasInitiliazed;

	public bool hasExtraOnCompletes;

	public bool hasPhysics;

	public bool onCompleteOnRepeat;

	public bool onCompleteOnStart;

	public bool useRecursion;

	public float ratioPassed;

	public float passed;

	public float delay;

	public float time;

	public float speed;

	public float lastVal;

	private uint _id;

	public int loopCount;

	public uint counter;

	public float direction;

	public float directionLast;

	public float overshoot;

	public float period;

	public float scale;

	public bool destroyOnComplete;

	public Transform trans;

	public LTRect ltRect;

	internal Vector3 fromInternal;

	internal Vector3 toInternal;

	internal Vector3 diff;

	internal Vector3 diffDiv2;

	public TweenAction type;

	private LeanTweenType easeType;

	public LeanTweenType loopType;

	public bool hasUpdateCallback;

	public LTDescr.EaseTypeDelegate easeMethod;

	public SpriteRenderer spriteRen;

	public RectTransform rectTransform;

	public Text uiText;

	public Image uiImage;

	public RawImage rawImage;

	public Sprite[] sprites;

	public LTDescrOptional _optional = new LTDescrOptional();

	public static float val;

	public static float dt;

	public static Vector3 newVect;

	public LTDescr.ActionMethodDelegate easeInternal
	{
		get;
		set;
	}

	public Vector3 @from
	{
		get
		{
			return this.fromInternal;
		}
		set
		{
			this.fromInternal = value;
		}
	}

	public int id
	{
		get
		{
			return this.uniqueId;
		}
	}

	public LTDescr.ActionMethodDelegate initInternal
	{
		get;
		set;
	}

	public LTDescrOptional optional
	{
		get
		{
			return this._optional;
		}
		set
		{
			this._optional = this.optional;
		}
	}

	public Vector3 to
	{
		get
		{
			return this.toInternal;
		}
		set
		{
			this.toInternal = value;
		}
	}

	public int uniqueId
	{
		get
		{
			return (int)(this._id | this.counter << 16);
		}
	}

	public LTDescr()
	{
	}

	private static void alphaRecursive(Transform transform, float val, bool useRecursion = true)
	{
		Renderer component = transform.gameObject.GetComponent<Renderer>();
		if (component != null)
		{
			Material[] materialArray = component.materials;
			for (int i = 0; i < (int)materialArray.Length; i++)
			{
				Material color = materialArray[i];
				if (color.HasProperty("_Color"))
				{
					color.color = new Color(color.color.r, color.color.g, color.color.b, val);
				}
				else if (color.HasProperty("_TintColor"))
				{
					Color color1 = color.GetColor("_TintColor");
					color.SetColor("_TintColor", new Color(color1.r, color1.g, color1.b, val));
				}
			}
		}
		if (useRecursion && transform.childCount > 0)
		{
			foreach (object obj in transform)
			{
				LTDescr.alphaRecursive((Transform)obj, val, true);
			}
		}
	}

	private static void alphaRecursive(RectTransform rectTransform, float val, int recursiveLevel = 0)
	{
		if (rectTransform.childCount > 0)
		{
			foreach (RectTransform rectTransform1 in rectTransform)
			{
				MaskableGraphic component = rectTransform1.GetComponent<Image>();
				if (component == null)
				{
					component = rectTransform1.GetComponent<RawImage>();
					if (component != null)
					{
						Color color = component.color;
						color.a = val;
						component.color = color;
					}
				}
				else
				{
					Color color1 = component.color;
					color1.a = val;
					component.color = color1;
				}
				LTDescr.alphaRecursive(rectTransform1, val, recursiveLevel + 1);
			}
		}
	}

	private static void alphaRecursiveSprite(Transform transform, float val)
	{
		if (transform.childCount > 0)
		{
			foreach (Transform transforms in transform)
			{
				SpriteRenderer component = transforms.GetComponent<SpriteRenderer>();
				if (component != null)
				{
					component.color = new Color(component.color.r, component.color.g, component.color.b, val);
				}
				LTDescr.alphaRecursiveSprite(transforms, val);
			}
		}
	}

	private void callback()
	{
		LTDescr.newVect = this.easeMethod();
		LTDescr.val = LTDescr.newVect.x;
	}

	public void callOnCompletes()
	{
		if (this.type == TweenAction.GUI_ROTATE)
		{
			this._optional.ltRect.rotateFinished = true;
		}
		if (this.type == TweenAction.DELAYED_SOUND)
		{
			AudioSource.PlayClipAtPoint((AudioClip)this._optional.onCompleteParam, this.to, this.@from.x);
		}
		if (this._optional.onComplete != null)
		{
			this._optional.onComplete();
			return;
		}
		if (this._optional.onCompleteObject != null)
		{
			this._optional.onCompleteObject(this._optional.onCompleteParam);
		}
	}

	[Obsolete("Use 'LeanTween.cancel( id )' instead")]
	public LTDescr cancel(GameObject gameObject)
	{
		if (gameObject == this.trans.gameObject)
		{
			LeanTween.removeTween((int)this._id, this.uniqueId);
		}
		return this;
	}

	private static void colorRecursive(Transform transform, Color toColor, bool useRecursion = true)
	{
		Renderer component = transform.gameObject.GetComponent<Renderer>();
		if (component != null)
		{
			Material[] materialArray = component.materials;
			for (int i = 0; i < (int)materialArray.Length; i++)
			{
				materialArray[i].color = toColor;
			}
		}
		if (useRecursion && transform.childCount > 0)
		{
			foreach (object obj in transform)
			{
				LTDescr.colorRecursive((Transform)obj, toColor, true);
			}
		}
	}

	private static void colorRecursive(RectTransform rectTransform, Color toColor)
	{
		if (rectTransform.childCount > 0)
		{
			foreach (RectTransform rectTransform1 in rectTransform)
			{
				MaskableGraphic component = rectTransform1.GetComponent<Image>();
				if (component == null)
				{
					component = rectTransform1.GetComponent<RawImage>();
					if (component != null)
					{
						component.color = toColor;
					}
				}
				else
				{
					component.color = toColor;
				}
				LTDescr.colorRecursive(rectTransform1, toColor);
			}
		}
	}

	private static void colorRecursiveSprite(Transform transform, Color toColor)
	{
		if (transform.childCount > 0)
		{
			foreach (Transform transforms in transform)
			{
				SpriteRenderer component = transform.gameObject.GetComponent<SpriteRenderer>();
				if (component != null)
				{
					component.color = toColor;
				}
				LTDescr.colorRecursiveSprite(transforms, toColor);
			}
		}
	}

	private Vector3 easeInBack()
	{
		LTDescr.val = this.ratioPassed;
		LTDescr.val /= 1f;
		float single = 1.70158f * this.overshoot;
		return (((this.diff * LTDescr.val) * LTDescr.val) * ((single + 1f) * LTDescr.val - single)) + this.@from;
	}

	private Vector3 easeInBounce()
	{
		LTDescr.val = this.ratioPassed;
		LTDescr.val = 1f - LTDescr.val;
		return new Vector3(this.diff.x - LeanTween.easeOutBounce(0f, this.diff.x, LTDescr.val) + this.@from.x, this.diff.y - LeanTween.easeOutBounce(0f, this.diff.y, LTDescr.val) + this.@from.y, this.diff.z - LeanTween.easeOutBounce(0f, this.diff.z, LTDescr.val) + this.@from.z);
	}

	private Vector3 easeInCirc()
	{
		LTDescr.val = -(Mathf.Sqrt(1f - this.ratioPassed * this.ratioPassed) - 1f);
		return new Vector3(this.diff.x * LTDescr.val + this.@from.x, this.diff.y * LTDescr.val + this.@from.y, this.diff.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeInCubic()
	{
		LTDescr.val = this.ratioPassed * this.ratioPassed * this.ratioPassed;
		return new Vector3(this.diff.x * LTDescr.val + this.@from.x, this.diff.y * LTDescr.val + this.@from.y, this.diff.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeInElastic()
	{
		return new Vector3(LeanTween.easeInElastic(this.@from.x, this.to.x, this.ratioPassed, this.overshoot, this.period), LeanTween.easeInElastic(this.@from.y, this.to.y, this.ratioPassed, this.overshoot, this.period), LeanTween.easeInElastic(this.@from.z, this.to.z, this.ratioPassed, this.overshoot, this.period));
	}

	private Vector3 easeInExpo()
	{
		LTDescr.val = Mathf.Pow(2f, 10f * (this.ratioPassed - 1f));
		return new Vector3(this.diff.x * LTDescr.val + this.@from.x, this.diff.y * LTDescr.val + this.@from.y, this.diff.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeInOutBack()
	{
		float single = 1.70158f * this.overshoot;
		LTDescr.val = this.ratioPassed * 2f;
		if (LTDescr.val < 1f)
		{
			single = single * (1.525f * this.overshoot);
			return (this.diffDiv2 * (LTDescr.val * LTDescr.val * ((single + 1f) * LTDescr.val - single))) + this.@from;
		}
		LTDescr.val -= 2f;
		single = single * (1.525f * this.overshoot);
		LTDescr.val = LTDescr.val * LTDescr.val * ((single + 1f) * LTDescr.val + single) + 2f;
		return (this.diffDiv2 * LTDescr.val) + this.@from;
	}

	private Vector3 easeInOutBounce()
	{
		LTDescr.val = this.ratioPassed * 2f;
		if (LTDescr.val < 1f)
		{
			return new Vector3(LeanTween.easeInBounce(0f, this.diff.x, LTDescr.val) * 0.5f + this.@from.x, LeanTween.easeInBounce(0f, this.diff.y, LTDescr.val) * 0.5f + this.@from.y, LeanTween.easeInBounce(0f, this.diff.z, LTDescr.val) * 0.5f + this.@from.z);
		}
		LTDescr.val -= 1f;
		return new Vector3(LeanTween.easeOutBounce(0f, this.diff.x, LTDescr.val) * 0.5f + this.diffDiv2.x + this.@from.x, LeanTween.easeOutBounce(0f, this.diff.y, LTDescr.val) * 0.5f + this.diffDiv2.y + this.@from.y, LeanTween.easeOutBounce(0f, this.diff.z, LTDescr.val) * 0.5f + this.diffDiv2.z + this.@from.z);
	}

	private Vector3 easeInOutCirc()
	{
		LTDescr.val = this.ratioPassed * 2f;
		if (LTDescr.val < 1f)
		{
			LTDescr.val = -(Mathf.Sqrt(1f - LTDescr.val * LTDescr.val) - 1f);
			return new Vector3(this.diffDiv2.x * LTDescr.val + this.@from.x, this.diffDiv2.y * LTDescr.val + this.@from.y, this.diffDiv2.z * LTDescr.val + this.@from.z);
		}
		LTDescr.val -= 2f;
		LTDescr.val = Mathf.Sqrt(1f - LTDescr.val * LTDescr.val) + 1f;
		return new Vector3(this.diffDiv2.x * LTDescr.val + this.@from.x, this.diffDiv2.y * LTDescr.val + this.@from.y, this.diffDiv2.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeInOutCubic()
	{
		LTDescr.val = this.ratioPassed * 2f;
		if (LTDescr.val < 1f)
		{
			LTDescr.val = LTDescr.val * LTDescr.val * LTDescr.val;
			return new Vector3(this.diffDiv2.x * LTDescr.val + this.@from.x, this.diffDiv2.y * LTDescr.val + this.@from.y, this.diffDiv2.z * LTDescr.val + this.@from.z);
		}
		LTDescr.val -= 2f;
		LTDescr.val = LTDescr.val * LTDescr.val * LTDescr.val + 2f;
		return new Vector3(this.diffDiv2.x * LTDescr.val + this.@from.x, this.diffDiv2.y * LTDescr.val + this.@from.y, this.diffDiv2.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeInOutElastic()
	{
		return new Vector3(LeanTween.easeInOutElastic(this.@from.x, this.to.x, this.ratioPassed, this.overshoot, this.period), LeanTween.easeInOutElastic(this.@from.y, this.to.y, this.ratioPassed, this.overshoot, this.period), LeanTween.easeInOutElastic(this.@from.z, this.to.z, this.ratioPassed, this.overshoot, this.period));
	}

	private Vector3 easeInOutExpo()
	{
		LTDescr.val = this.ratioPassed * 2f;
		if (LTDescr.val < 1f)
		{
			return (this.diffDiv2 * Mathf.Pow(2f, 10f * (LTDescr.val - 1f))) + this.@from;
		}
		LTDescr.val -= 1f;
		return (this.diffDiv2 * (-Mathf.Pow(2f, -10f * LTDescr.val) + 2f)) + this.@from;
	}

	private Vector3 easeInOutQuad()
	{
		LTDescr.val = this.ratioPassed * 2f;
		if (LTDescr.val < 1f)
		{
			LTDescr.val *= LTDescr.val;
			return new Vector3(this.diffDiv2.x * LTDescr.val + this.@from.x, this.diffDiv2.y * LTDescr.val + this.@from.y, this.diffDiv2.z * LTDescr.val + this.@from.z);
		}
		LTDescr.val = (1f - LTDescr.val) * (LTDescr.val - 3f) + 1f;
		return new Vector3(this.diffDiv2.x * LTDescr.val + this.@from.x, this.diffDiv2.y * LTDescr.val + this.@from.y, this.diffDiv2.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeInOutQuart()
	{
		LTDescr.val = this.ratioPassed * 2f;
		if (LTDescr.val >= 1f)
		{
			LTDescr.val -= 2f;
			return (-this.diffDiv2 * (LTDescr.val * LTDescr.val * LTDescr.val * LTDescr.val - 2f)) + this.@from;
		}
		LTDescr.val = LTDescr.val * LTDescr.val * LTDescr.val * LTDescr.val;
		return new Vector3(this.diffDiv2.x * LTDescr.val + this.@from.x, this.diffDiv2.y * LTDescr.val + this.@from.y, this.diffDiv2.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeInOutQuint()
	{
		LTDescr.val = this.ratioPassed * 2f;
		if (LTDescr.val < 1f)
		{
			LTDescr.val = LTDescr.val * LTDescr.val * LTDescr.val * LTDescr.val * LTDescr.val;
			return new Vector3(this.diffDiv2.x * LTDescr.val + this.@from.x, this.diffDiv2.y * LTDescr.val + this.@from.y, this.diffDiv2.z * LTDescr.val + this.@from.z);
		}
		LTDescr.val -= 2f;
		LTDescr.val = LTDescr.val * LTDescr.val * LTDescr.val * LTDescr.val * LTDescr.val + 2f;
		return new Vector3(this.diffDiv2.x * LTDescr.val + this.@from.x, this.diffDiv2.y * LTDescr.val + this.@from.y, this.diffDiv2.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeInOutSine()
	{
		LTDescr.val = -(Mathf.Cos(3.14159274f * this.ratioPassed) - 1f);
		return new Vector3(this.diffDiv2.x * LTDescr.val + this.@from.x, this.diffDiv2.y * LTDescr.val + this.@from.y, this.diffDiv2.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeInQuad()
	{
		LTDescr.val = this.ratioPassed * this.ratioPassed;
		return new Vector3(this.diff.x * LTDescr.val + this.@from.x, this.diff.y * LTDescr.val + this.@from.y, this.diff.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeInQuart()
	{
		LTDescr.val = this.ratioPassed * this.ratioPassed * this.ratioPassed * this.ratioPassed;
		return (this.diff * LTDescr.val) + this.@from;
	}

	private Vector3 easeInQuint()
	{
		LTDescr.val = this.ratioPassed;
		LTDescr.val = LTDescr.val * LTDescr.val * LTDescr.val * LTDescr.val * LTDescr.val;
		return new Vector3(this.diff.x * LTDescr.val + this.@from.x, this.diff.y * LTDescr.val + this.@from.y, this.diff.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeInSine()
	{
		LTDescr.val = -Mathf.Cos(this.ratioPassed * LeanTween.PI_DIV2);
		return new Vector3(this.diff.x * LTDescr.val + this.diff.x + this.@from.x, this.diff.y * LTDescr.val + this.diff.y + this.@from.y, this.diff.z * LTDescr.val + this.diff.z + this.@from.z);
	}

	private Vector3 easeLinear()
	{
		LTDescr.val = this.ratioPassed;
		return new Vector3(this.@from.x + this.diff.x * LTDescr.val, this.@from.y + this.diff.y * LTDescr.val, this.@from.z + this.diff.z * LTDescr.val);
	}

	private Vector3 easeOutBack()
	{
		float single = 1.70158f * this.overshoot;
		LTDescr.val = this.ratioPassed / 1f - 1f;
		LTDescr.val = LTDescr.val * LTDescr.val * ((single + 1f) * LTDescr.val + single) + 1f;
		return (this.diff * LTDescr.val) + this.@from;
	}

	private Vector3 easeOutBounce()
	{
		LTDescr.val = this.ratioPassed;
		float single = LTDescr.val;
		float single1 = 1f - 1.75f * this.overshoot / 2.75f;
		float single2 = single1;
		if (single >= single1)
		{
			float single3 = LTDescr.val;
			float single4 = 1f - 0.75f * this.overshoot / 2.75f;
			float single5 = single4;
			if (single3 >= single4)
			{
				float single6 = LTDescr.val;
				float single7 = 1f - 0.25f * this.overshoot / 2.75f;
				single2 = single7;
				if (single6 >= single7)
				{
					LTDescr.val = LTDescr.val - (single2 + 1f) / 2f;
					LTDescr.val = 7.5625f * LTDescr.val * LTDescr.val + 1f - 0.015625f * this.overshoot * this.overshoot;
				}
				else
				{
					LTDescr.val = LTDescr.val - (single2 + single5) / 2f;
					LTDescr.val = 7.5625f * LTDescr.val * LTDescr.val + 1f - 0.0625f * this.overshoot * this.overshoot;
				}
			}
			else
			{
				LTDescr.val = LTDescr.val - (single2 + single5) / 2f;
				LTDescr.val = 7.5625f * LTDescr.val * LTDescr.val + 1f - 0.25f * this.overshoot * this.overshoot;
			}
		}
		else
		{
			LTDescr.val = 1f / single2 / single2 * LTDescr.val * LTDescr.val;
		}
		return (this.diff * LTDescr.val) + this.@from;
	}

	private Vector3 easeOutCirc()
	{
		LTDescr.val = this.ratioPassed - 1f;
		LTDescr.val = Mathf.Sqrt(1f - LTDescr.val * LTDescr.val);
		return new Vector3(this.diff.x * LTDescr.val + this.@from.x, this.diff.y * LTDescr.val + this.@from.y, this.diff.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeOutCubic()
	{
		LTDescr.val = this.ratioPassed - 1f;
		LTDescr.val = LTDescr.val * LTDescr.val * LTDescr.val + 1f;
		return new Vector3(this.diff.x * LTDescr.val + this.@from.x, this.diff.y * LTDescr.val + this.@from.y, this.diff.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeOutElastic()
	{
		return new Vector3(LeanTween.easeOutElastic(this.@from.x, this.to.x, this.ratioPassed, this.overshoot, this.period), LeanTween.easeOutElastic(this.@from.y, this.to.y, this.ratioPassed, this.overshoot, this.period), LeanTween.easeOutElastic(this.@from.z, this.to.z, this.ratioPassed, this.overshoot, this.period));
	}

	private Vector3 easeOutExpo()
	{
		LTDescr.val = -Mathf.Pow(2f, -10f * this.ratioPassed) + 1f;
		return new Vector3(this.diff.x * LTDescr.val + this.@from.x, this.diff.y * LTDescr.val + this.@from.y, this.diff.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeOutQuad()
	{
		LTDescr.val = this.ratioPassed;
		LTDescr.val = -LTDescr.val * (LTDescr.val - 2f);
		return (this.diff * LTDescr.val) + this.@from;
	}

	private Vector3 easeOutQuart()
	{
		LTDescr.val = this.ratioPassed - 1f;
		LTDescr.val = -(LTDescr.val * LTDescr.val * LTDescr.val * LTDescr.val - 1f);
		return new Vector3(this.diff.x * LTDescr.val + this.@from.x, this.diff.y * LTDescr.val + this.@from.y, this.diff.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeOutQuint()
	{
		LTDescr.val = this.ratioPassed - 1f;
		LTDescr.val = LTDescr.val * LTDescr.val * LTDescr.val * LTDescr.val * LTDescr.val + 1f;
		return new Vector3(this.diff.x * LTDescr.val + this.@from.x, this.diff.y * LTDescr.val + this.@from.y, this.diff.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeOutSine()
	{
		LTDescr.val = Mathf.Sin(this.ratioPassed * LeanTween.PI_DIV2);
		return new Vector3(this.diff.x * LTDescr.val + this.@from.x, this.diff.y * LTDescr.val + this.@from.y, this.diff.z * LTDescr.val + this.@from.z);
	}

	private Vector3 easeSpring()
	{
		LTDescr.val = Mathf.Clamp01(this.ratioPassed);
		LTDescr.val = (Mathf.Sin(LTDescr.val * 3.14159274f * (0.2f + 2.5f * LTDescr.val * LTDescr.val * LTDescr.val)) * Mathf.Pow(1f - LTDescr.val, 2.2f) + LTDescr.val) * (1f + 1.2f * (1f - LTDescr.val));
		return this.@from + (this.diff * LTDescr.val);
	}

	private void init()
	{
		this.hasInitiliazed = true;
		this.usesNormalDt = (this.useEstimatedTime || this.useManualTime ? false : !this.useFrames);
		if (this.useFrames)
		{
			this.optional.initFrameCount = Time.frameCount;
		}
		if (this.time <= 0f)
		{
			this.time = Mathf.Epsilon;
		}
		this.initInternal();
		this.diff = this.to - this.@from;
		this.diffDiv2 = this.diff * 0.5f;
		if (this._optional.onStart != null)
		{
			this._optional.onStart();
		}
		if (this.onCompleteOnStart)
		{
			this.callOnCompletes();
		}
		if (this.speed >= 0f)
		{
			this.initSpeed();
		}
	}

	private void initCanvasRotateAround()
	{
		this.lastVal = 0f;
		this.fromInternal.x = 0f;
		this._optional.origRotation = this.rectTransform.rotation;
	}

	private void initFromInternal()
	{
		this.fromInternal.x = 0f;
	}

	private void initSpeed()
	{
		if (this.type == TweenAction.MOVE_CURVED || this.type == TweenAction.MOVE_CURVED_LOCAL)
		{
			this.time = this._optional.path.distance / this.speed;
			return;
		}
		if (this.type == TweenAction.MOVE_SPLINE || this.type == TweenAction.MOVE_SPLINE_LOCAL)
		{
			this.time = this._optional.spline.distance / this.speed;
			return;
		}
		Vector3 vector3 = this.to - this.@from;
		this.time = vector3.magnitude / this.speed;
	}

	public LTDescr pause()
	{
		if (this.direction != 0f)
		{
			this.directionLast = this.direction;
			this.direction = 0f;
		}
		return this;
	}

	public void reset()
	{
		int num = 1;
		bool flag = (bool)num;
		this.usesNormalDt = (bool)num;
		bool flag1 = flag;
		flag = flag1;
		this.useRecursion = flag1;
		this.toggle = flag;
		this.trans = null;
		this.spriteRen = null;
		float single = 0f;
		float single1 = single;
		this.lastVal = single;
		float single2 = single1;
		single1 = single2;
		this.delay = single2;
		this.passed = single1;
		int num1 = 0;
		flag = (bool)num1;
		this.hasExtraOnCompletes = (bool)num1;
		bool flag2 = flag;
		flag = flag2;
		this.useManualTime = flag2;
		bool flag3 = flag;
		flag = flag3;
		this.onCompleteOnStart = flag3;
		bool flag4 = flag;
		flag = flag4;
		this.destroyOnComplete = flag4;
		bool flag5 = flag;
		flag = flag5;
		this.onCompleteOnRepeat = flag5;
		bool flag6 = flag;
		flag = flag6;
		this.hasInitiliazed = flag6;
		bool flag7 = flag;
		flag = flag7;
		this.useFrames = flag7;
		bool flag8 = flag;
		flag = flag8;
		this.useEstimatedTime = flag8;
		this.hasUpdateCallback = flag;
		this.easeType = LeanTweenType.linear;
		this.loopType = LeanTweenType.once;
		this.loopCount = 0;
		float single3 = 1f;
		single1 = single3;
		this.scale = single3;
		float single4 = single1;
		single1 = single4;
		this.overshoot = single4;
		float single5 = single1;
		single1 = single5;
		this.directionLast = single5;
		this.direction = single1;
		this.period = 0.3f;
		this.speed = -1f;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeLinear);
		Vector3 vector3 = Vector3.zero;
		Vector3 vector31 = vector3;
		this.to = vector3;
		this.@from = vector31;
		this._optional.reset();
	}

	public LTDescr resume()
	{
		this.direction = this.directionLast;
		return this;
	}

	public LTDescr setAlpha()
	{
		this.type = TweenAction.ALPHA;
		this.initInternal = () => {
			SpriteRenderer component = this.trans.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				this.fromInternal.x = component.color.a;
			}
			else if (this.trans.GetComponent<Renderer>() != null && this.trans.GetComponent<Renderer>().material.HasProperty("_Color"))
			{
				this.fromInternal.x = this.trans.GetComponent<Renderer>().material.color.a;
			}
			else if (this.trans.GetComponent<Renderer>() != null && this.trans.GetComponent<Renderer>().material.HasProperty("_TintColor"))
			{
				Color color = this.trans.GetComponent<Renderer>().material.GetColor("_TintColor");
				this.fromInternal.x = color.a;
			}
			else if (this.trans.childCount > 0)
			{
				foreach (Transform tran in this.trans)
				{
					if (tran.gameObject.GetComponent<Renderer>() == null)
					{
						continue;
					}
					Color component1 = tran.gameObject.GetComponent<Renderer>().material.color;
					this.fromInternal.x = component1.a;
					this.easeInternal = () => {
						LTDescr.val = this.easeMethod().x;
						if (this.spriteRen == null)
						{
							LTDescr.alphaRecursive(this.trans, LTDescr.val, this.useRecursion);
							return;
						}
						this.spriteRen.color = new Color(this.spriteRen.color.r, this.spriteRen.color.g, this.spriteRen.color.b, LTDescr.val);
						LTDescr.alphaRecursiveSprite(this.trans, LTDescr.val);
					};
					return;
				}
			}
			this.easeInternal = () => {
				LTDescr.val = this.easeMethod().x;
				if (this.spriteRen == null)
				{
					LTDescr.alphaRecursive(this.trans, LTDescr.val, this.useRecursion);
					return;
				}
				this.spriteRen.color = new Color(this.spriteRen.color.r, this.spriteRen.color.g, this.spriteRen.color.b, LTDescr.val);
				LTDescr.alphaRecursiveSprite(this.trans, LTDescr.val);
			};
		};
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			if (this.spriteRen == null)
			{
				LTDescr.alphaRecursive(this.trans, LTDescr.val, this.useRecursion);
				return;
			}
			this.spriteRen.color = new Color(this.spriteRen.color.r, this.spriteRen.color.g, this.spriteRen.color.b, LTDescr.val);
			LTDescr.alphaRecursiveSprite(this.trans, LTDescr.val);
		};
		return this;
	}

	public LTDescr setAlphaVertex()
	{
		this.type = TweenAction.ALPHA_VERTEX;
		this.initInternal = () => this.fromInternal.x = (float)this.trans.GetComponent<MeshFilter>().mesh.colors32[0].a;
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			Mesh component = this.trans.GetComponent<MeshFilter>().mesh;
			Vector3[] vector3Array = component.vertices;
			Color32[] color32Array = new Color32[(int)vector3Array.Length];
			if (color32Array.Length == 0)
			{
				Color32 color32 = new Color32(255, 255, 255, 0);
				color32Array = new Color32[(int)component.vertices.Length];
				for (int i = 0; i < (int)color32Array.Length; i++)
				{
					color32Array[i] = color32;
				}
				component.colors32 = color32Array;
			}
			Color32 color = component.colors32[0];
			color = new Color((float)color.r, (float)color.g, (float)color.b, LTDescr.val);
			for (int j = 0; j < (int)vector3Array.Length; j++)
			{
				color32Array[j] = color;
			}
			component.colors32 = color32Array;
		};
		return this;
	}

	public LTDescr setAudio(object audio)
	{
		this._optional.onCompleteParam = audio;
		return this;
	}

	public LTDescr setAxis(Vector3 axis)
	{
		this._optional.axis = axis;
		return this;
	}

	public LTDescr setCallback()
	{
		this.type = TweenAction.CALLBACK;
		this.initInternal = () => {
		};
		this.easeInternal = new LTDescr.ActionMethodDelegate(this.callback);
		return this;
	}

	public LTDescr setCallbackColor()
	{
		this.type = TweenAction.CALLBACK_COLOR;
		this.initInternal = () => this.diff = new Vector3(1f, 0f, 0f);
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			Color color = LTDescr.tweenColor(this, LTDescr.val);
			if (this.spriteRen != null)
			{
				this.spriteRen.color = color;
				LTDescr.colorRecursiveSprite(this.trans, color);
			}
			else if (this.type == TweenAction.COLOR)
			{
				LTDescr.colorRecursive(this.trans, color, this.useRecursion);
			}
			if (LTDescr.dt != 0f && this._optional.onUpdateColor != null)
			{
				this._optional.onUpdateColor(color);
				return;
			}
			if (LTDescr.dt != 0f && this._optional.onUpdateColorObject != null)
			{
				this._optional.onUpdateColorObject(color, this._optional.onUpdateParam);
			}
		};
		return this;
	}

	public LTDescr setCanvasAlpha()
	{
		this.type = TweenAction.CANVAS_ALPHA;
		this.initInternal = () => {
			this.uiImage = this.trans.GetComponent<Image>();
			if (this.uiImage != null)
			{
				this.fromInternal.x = this.uiImage.color.a;
				return;
			}
			this.rawImage = this.trans.GetComponent<RawImage>();
			if (this.rawImage == null)
			{
				this.fromInternal.x = 1f;
				return;
			}
			this.fromInternal.x = this.rawImage.color.a;
		};
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			if (this.uiImage != null)
			{
				Color color = this.uiImage.color;
				color.a = LTDescr.val;
				this.uiImage.color = color;
			}
			else if (this.rawImage != null)
			{
				Color color1 = this.rawImage.color;
				color1.a = LTDescr.val;
				this.rawImage.color = color1;
			}
			if (this.useRecursion)
			{
				LTDescr.alphaRecursive(this.rectTransform, LTDescr.val, 0);
				LTDescr.textAlphaChildrenRecursive(this.rectTransform, LTDescr.val, true);
			}
		};
		return this;
	}

	public LTDescr setCanvasColor()
	{
		this.type = TweenAction.CANVAS_COLOR;
		this.initInternal = () => {
			this.uiImage = this.trans.GetComponent<Image>();
			if (this.uiImage != null)
			{
				this.setFromColor(this.uiImage.color);
				return;
			}
			this.rawImage = this.trans.GetComponent<RawImage>();
			this.setFromColor((this.rawImage != null ? this.rawImage.color : Color.white));
		};
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			Color color = LTDescr.tweenColor(this, LTDescr.val);
			if (this.uiImage != null)
			{
				this.uiImage.color = color;
			}
			else if (this.rawImage != null)
			{
				this.rawImage.color = color;
			}
			if (LTDescr.dt != 0f && this._optional.onUpdateColor != null)
			{
				this._optional.onUpdateColor(color);
			}
			if (this.useRecursion)
			{
				LTDescr.colorRecursive(this.rectTransform, color);
			}
		};
		return this;
	}

	public LTDescr setCanvasGroupAlpha()
	{
		this.type = TweenAction.CANVASGROUP_ALPHA;
		this.initInternal = () => this.fromInternal.x = this.trans.GetComponent<CanvasGroup>().alpha;
		this.easeInternal = () => this.trans.GetComponent<CanvasGroup>().alpha = this.easeMethod().x;
		return this;
	}

	public LTDescr setCanvasMove()
	{
		this.type = TweenAction.CANVAS_MOVE;
		this.initInternal = () => this.fromInternal = this.rectTransform.anchoredPosition3D;
		this.easeInternal = () => this.rectTransform.anchoredPosition3D = this.easeMethod();
		return this;
	}

	public LTDescr setCanvasMoveX()
	{
		this.type = TweenAction.CANVAS_MOVE_X;
		this.initInternal = () => this.fromInternal.x = this.rectTransform.anchoredPosition3D.x;
		this.easeInternal = () => {
			Vector3 vector3 = this.rectTransform.anchoredPosition3D;
			this.rectTransform.anchoredPosition3D = new Vector3(this.easeMethod().x, vector3.y, vector3.z);
		};
		return this;
	}

	public LTDescr setCanvasMoveY()
	{
		this.type = TweenAction.CANVAS_MOVE_Y;
		this.initInternal = () => this.fromInternal.x = this.rectTransform.anchoredPosition3D.y;
		this.easeInternal = () => {
			Vector3 vector3 = this.rectTransform.anchoredPosition3D;
			this.rectTransform.anchoredPosition3D = new Vector3(vector3.x, this.easeMethod().x, vector3.z);
		};
		return this;
	}

	public LTDescr setCanvasMoveZ()
	{
		this.type = TweenAction.CANVAS_MOVE_Z;
		this.initInternal = () => this.fromInternal.x = this.rectTransform.anchoredPosition3D.z;
		this.easeInternal = () => {
			Vector3 vector3 = this.rectTransform.anchoredPosition3D;
			this.rectTransform.anchoredPosition3D = new Vector3(vector3.x, vector3.y, this.easeMethod().x);
		};
		return this;
	}

	public LTDescr setCanvasPlaySprite()
	{
		this.type = TweenAction.CANVAS_PLAYSPRITE;
		this.initInternal = () => {
			this.uiImage = this.trans.GetComponent<Image>();
			this.fromInternal.x = 0f;
		};
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			int num = (int)Mathf.Round(LTDescr.val);
			this.uiImage.sprite = this.sprites[num];
		};
		return this;
	}

	public LTDescr setCanvasRotateAround()
	{
		this.type = TweenAction.CANVAS_ROTATEAROUND;
		this.initInternal = new LTDescr.ActionMethodDelegate(this.initCanvasRotateAround);
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			RectTransform rectTransform = this.rectTransform;
			Vector3 vector3 = rectTransform.localPosition;
			rectTransform.RotateAround(rectTransform.TransformPoint(this._optional.point), this._optional.axis, -LTDescr.val);
			rectTransform.localPosition = vector3 - (vector3 - rectTransform.localPosition);
			rectTransform.rotation = this._optional.origRotation;
			rectTransform.RotateAround(rectTransform.TransformPoint(this._optional.point), this._optional.axis, LTDescr.val);
		};
		return this;
	}

	public LTDescr setCanvasRotateAroundLocal()
	{
		this.type = TweenAction.CANVAS_ROTATEAROUND_LOCAL;
		this.initInternal = new LTDescr.ActionMethodDelegate(this.initCanvasRotateAround);
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			RectTransform rectTransform = this.rectTransform;
			Vector3 vector3 = rectTransform.localPosition;
			rectTransform.RotateAround(rectTransform.TransformPoint(this._optional.point), rectTransform.TransformDirection(this._optional.axis), -LTDescr.val);
			rectTransform.localPosition = vector3 - (vector3 - rectTransform.localPosition);
			rectTransform.rotation = this._optional.origRotation;
			rectTransform.RotateAround(rectTransform.TransformPoint(this._optional.point), rectTransform.TransformDirection(this._optional.axis), LTDescr.val);
		};
		return this;
	}

	public LTDescr setCanvasScale()
	{
		this.type = TweenAction.CANVAS_SCALE;
		this.initInternal = () => this.@from = this.rectTransform.localScale;
		this.easeInternal = () => this.rectTransform.localScale = this.easeMethod();
		return this;
	}

	public LTDescr setCanvasSizeDelta()
	{
		this.type = TweenAction.CANVAS_SIZEDELTA;
		this.initInternal = () => this.@from = this.rectTransform.sizeDelta;
		this.easeInternal = () => this.rectTransform.sizeDelta = this.easeMethod();
		return this;
	}

	public LTDescr setColor()
	{
		this.type = TweenAction.COLOR;
		this.initInternal = () => {
			SpriteRenderer component = this.trans.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				this.setFromColor(component.color);
				return;
			}
			if (this.trans.GetComponent<Renderer>() != null && this.trans.GetComponent<Renderer>().material.HasProperty("_Color"))
			{
				this.setFromColor(this.trans.GetComponent<Renderer>().material.color);
				return;
			}
			if (this.trans.GetComponent<Renderer>() != null && this.trans.GetComponent<Renderer>().material.HasProperty("_TintColor"))
			{
				this.setFromColor(this.trans.GetComponent<Renderer>().material.GetColor("_TintColor"));
				return;
			}
			if (this.trans.childCount > 0)
			{
				foreach (Transform tran in this.trans)
				{
					if (tran.gameObject.GetComponent<Renderer>() == null)
					{
						continue;
					}
					this.setFromColor(tran.gameObject.GetComponent<Renderer>().material.color);
					return;
				}
			}
		};
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			Color color = LTDescr.tweenColor(this, LTDescr.val);
			if (this.spriteRen != null)
			{
				this.spriteRen.color = color;
				LTDescr.colorRecursiveSprite(this.trans, color);
			}
			else if (this.type == TweenAction.COLOR)
			{
				LTDescr.colorRecursive(this.trans, color, this.useRecursion);
			}
			if (LTDescr.dt != 0f && this._optional.onUpdateColor != null)
			{
				this._optional.onUpdateColor(color);
				return;
			}
			if (LTDescr.dt != 0f && this._optional.onUpdateColorObject != null)
			{
				this._optional.onUpdateColorObject(color, this._optional.onUpdateParam);
			}
		};
		return this;
	}

	public LTDescr setDelay(float delay)
	{
		this.delay = delay;
		return this;
	}

	public LTDescr setDelayedSound()
	{
		this.type = TweenAction.DELAYED_SOUND;
		this.initInternal = () => this.hasExtraOnCompletes = true;
		this.easeInternal = new LTDescr.ActionMethodDelegate(this.callback);
		return this;
	}

	public LTDescr setDestroyOnComplete(bool doesDestroy)
	{
		this.destroyOnComplete = doesDestroy;
		return this;
	}

	public LTDescr setDiff(Vector3 diff)
	{
		this.diff = diff;
		return this;
	}

	public LTDescr setDirection(float direction)
	{
		if (this.direction != -1f && this.direction != 1f)
		{
			Debug.LogWarning(string.Concat("You have passed an incorrect direction of '", direction, "', direction must be -1f or 1f"));
			return this;
		}
		if (this.direction != direction)
		{
			if (this.hasInitiliazed)
			{
				this.direction = direction;
			}
			else if (this._optional.path != null)
			{
				this._optional.path = new LTBezierPath(LTUtility.reverse(this._optional.path.pts));
			}
			else if (this._optional.spline != null)
			{
				this._optional.spline = new LTSpline(LTUtility.reverse(this._optional.spline.pts));
			}
		}
		return this;
	}

	public LTDescr setEase(LeanTweenType easeType)
	{
		switch (easeType)
		{
			case LeanTweenType.linear:
			{
				this.setEaseLinear();
				break;
			}
			case LeanTweenType.easeOutQuad:
			{
				this.setEaseOutQuad();
				break;
			}
			case LeanTweenType.easeInQuad:
			{
				this.setEaseInQuad();
				break;
			}
			case LeanTweenType.easeInOutQuad:
			{
				this.setEaseInOutQuad();
				break;
			}
			case LeanTweenType.easeInCubic:
			{
				this.setEaseInCubic();
				break;
			}
			case LeanTweenType.easeOutCubic:
			{
				this.setEaseOutCubic();
				break;
			}
			case LeanTweenType.easeInOutCubic:
			{
				this.setEaseInOutCubic();
				break;
			}
			case LeanTweenType.easeInQuart:
			{
				this.setEaseInQuart();
				break;
			}
			case LeanTweenType.easeOutQuart:
			{
				this.setEaseOutQuart();
				break;
			}
			case LeanTweenType.easeInOutQuart:
			{
				this.setEaseInOutQuart();
				break;
			}
			case LeanTweenType.easeInQuint:
			{
				this.setEaseInQuint();
				break;
			}
			case LeanTweenType.easeOutQuint:
			{
				this.setEaseOutQuint();
				break;
			}
			case LeanTweenType.easeInOutQuint:
			{
				this.setEaseInOutQuint();
				break;
			}
			case LeanTweenType.easeInSine:
			{
				this.setEaseInSine();
				break;
			}
			case LeanTweenType.easeOutSine:
			{
				this.setEaseOutSine();
				break;
			}
			case LeanTweenType.easeInOutSine:
			{
				this.setEaseInOutSine();
				break;
			}
			case LeanTweenType.easeInExpo:
			{
				this.setEaseInExpo();
				break;
			}
			case LeanTweenType.easeOutExpo:
			{
				this.setEaseOutExpo();
				break;
			}
			case LeanTweenType.easeInOutExpo:
			{
				this.setEaseInOutExpo();
				break;
			}
			case LeanTweenType.easeInCirc:
			{
				this.setEaseInCirc();
				break;
			}
			case LeanTweenType.easeOutCirc:
			{
				this.setEaseOutCirc();
				break;
			}
			case LeanTweenType.easeInOutCirc:
			{
				this.setEaseInOutCirc();
				break;
			}
			case LeanTweenType.easeInBounce:
			{
				this.setEaseInBounce();
				break;
			}
			case LeanTweenType.easeOutBounce:
			{
				this.setEaseOutBounce();
				break;
			}
			case LeanTweenType.easeInOutBounce:
			{
				this.setEaseInOutBounce();
				break;
			}
			case LeanTweenType.easeInBack:
			{
				this.setEaseInBack();
				break;
			}
			case LeanTweenType.easeOutBack:
			{
				this.setEaseOutBack();
				break;
			}
			case LeanTweenType.easeInOutBack:
			{
				this.setEaseInOutBack();
				break;
			}
			case LeanTweenType.easeInElastic:
			{
				this.setEaseInElastic();
				break;
			}
			case LeanTweenType.easeOutElastic:
			{
				this.setEaseOutElastic();
				break;
			}
			case LeanTweenType.easeInOutElastic:
			{
				this.setEaseInOutElastic();
				break;
			}
			case LeanTweenType.easeSpring:
			{
				this.setEaseSpring();
				break;
			}
			case LeanTweenType.easeShake:
			{
				this.setEaseShake();
				break;
			}
			case LeanTweenType.punch:
			{
				this.setEasePunch();
				break;
			}
			default:
			{
				this.setEaseLinear();
				break;
			}
		}
		return this;
	}

	public LTDescr setEase(AnimationCurve easeCurve)
	{
		this._optional.animationCurve = easeCurve;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.tweenOnCurve);
		this.easeType = LeanTweenType.animationCurve;
		return this;
	}

	public LTDescr setEaseInBack()
	{
		this.easeType = LeanTweenType.easeInBack;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInBack);
		return this;
	}

	public LTDescr setEaseInBounce()
	{
		this.easeType = LeanTweenType.easeInBounce;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInBounce);
		return this;
	}

	public LTDescr setEaseInCirc()
	{
		this.easeType = LeanTweenType.easeInCirc;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInCirc);
		return this;
	}

	public LTDescr setEaseInCubic()
	{
		this.easeType = LeanTweenType.easeInCubic;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInCubic);
		return this;
	}

	public LTDescr setEaseInElastic()
	{
		this.easeType = LeanTweenType.easeInElastic;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInElastic);
		return this;
	}

	public LTDescr setEaseInExpo()
	{
		this.easeType = LeanTweenType.easeInExpo;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInExpo);
		return this;
	}

	public LTDescr setEaseInOutBack()
	{
		this.easeType = LeanTweenType.easeInOutBack;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInOutBack);
		return this;
	}

	public LTDescr setEaseInOutBounce()
	{
		this.easeType = LeanTweenType.easeInOutBounce;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInOutBounce);
		return this;
	}

	public LTDescr setEaseInOutCirc()
	{
		this.easeType = LeanTweenType.easeInOutCirc;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInOutCirc);
		return this;
	}

	public LTDescr setEaseInOutCubic()
	{
		this.easeType = LeanTweenType.easeInOutCubic;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInOutCubic);
		return this;
	}

	public LTDescr setEaseInOutElastic()
	{
		this.easeType = LeanTweenType.easeInOutElastic;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInOutElastic);
		return this;
	}

	public LTDescr setEaseInOutExpo()
	{
		this.easeType = LeanTweenType.easeInOutExpo;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInOutExpo);
		return this;
	}

	public LTDescr setEaseInOutQuad()
	{
		this.easeType = LeanTweenType.easeInOutQuad;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInOutQuad);
		return this;
	}

	public LTDescr setEaseInOutQuart()
	{
		this.easeType = LeanTweenType.easeInOutQuart;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInOutQuart);
		return this;
	}

	public LTDescr setEaseInOutQuint()
	{
		this.easeType = LeanTweenType.easeInOutQuint;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInOutQuint);
		return this;
	}

	public LTDescr setEaseInOutSine()
	{
		this.easeType = LeanTweenType.easeInOutSine;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInOutSine);
		return this;
	}

	public LTDescr setEaseInQuad()
	{
		this.easeType = LeanTweenType.easeInQuad;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInQuad);
		return this;
	}

	public LTDescr setEaseInQuart()
	{
		this.easeType = LeanTweenType.easeInQuart;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInQuart);
		return this;
	}

	public LTDescr setEaseInQuint()
	{
		this.easeType = LeanTweenType.easeInQuint;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInQuint);
		return this;
	}

	public LTDescr setEaseInSine()
	{
		this.easeType = LeanTweenType.easeInSine;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeInSine);
		return this;
	}

	public LTDescr setEaseLinear()
	{
		this.easeType = LeanTweenType.linear;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeLinear);
		return this;
	}

	public LTDescr setEaseOutBack()
	{
		this.easeType = LeanTweenType.easeOutBack;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeOutBack);
		return this;
	}

	public LTDescr setEaseOutBounce()
	{
		this.easeType = LeanTweenType.easeOutBounce;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeOutBounce);
		return this;
	}

	public LTDescr setEaseOutCirc()
	{
		this.easeType = LeanTweenType.easeOutCirc;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeOutCirc);
		return this;
	}

	public LTDescr setEaseOutCubic()
	{
		this.easeType = LeanTweenType.easeOutCubic;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeOutCubic);
		return this;
	}

	public LTDescr setEaseOutElastic()
	{
		this.easeType = LeanTweenType.easeOutElastic;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeOutElastic);
		return this;
	}

	public LTDescr setEaseOutExpo()
	{
		this.easeType = LeanTweenType.easeOutExpo;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeOutExpo);
		return this;
	}

	public LTDescr setEaseOutQuad()
	{
		this.easeType = LeanTweenType.easeOutQuad;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeOutQuad);
		return this;
	}

	public LTDescr setEaseOutQuart()
	{
		this.easeType = LeanTweenType.easeOutQuart;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeOutQuart);
		return this;
	}

	public LTDescr setEaseOutQuint()
	{
		this.easeType = LeanTweenType.easeOutQuint;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeOutQuint);
		return this;
	}

	public LTDescr setEaseOutSine()
	{
		this.easeType = LeanTweenType.easeOutSine;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeOutSine);
		return this;
	}

	public LTDescr setEasePunch()
	{
		this._optional.animationCurve = LeanTween.punch;
		this.toInternal.x = this.@from.x + this.to.x;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.tweenOnCurve);
		return this;
	}

	public LTDescr setEaseShake()
	{
		this._optional.animationCurve = LeanTween.shake;
		this.toInternal.x = this.@from.x + this.to.x;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.tweenOnCurve);
		return this;
	}

	public LTDescr setEaseSpring()
	{
		this.easeType = LeanTweenType.easeSpring;
		this.easeMethod = new LTDescr.EaseTypeDelegate(this.easeSpring);
		return this;
	}

	public LTDescr setFrameRate(float frameRate)
	{
		this.time = (float)((int)this.sprites.Length) / frameRate;
		return this;
	}

	public LTDescr setFrom(Vector3 from)
	{
		if (this.trans)
		{
			this.init();
		}
		this.@from = from;
		this.diff = this.to - this.@from;
		this.diffDiv2 = this.diff * 0.5f;
		return this;
	}

	public LTDescr setFrom(float from)
	{
		return this.setFrom(new Vector3(from, 0f, 0f));
	}

	public LTDescr setFromColor(Color col)
	{
		this.@from = new Vector3(0f, col.a, 0f);
		this.diff = new Vector3(1f, 0f, 0f);
		this._optional.axis = new Vector3(col.r, col.g, col.b);
		return this;
	}

	public LTDescr setGUIAlpha()
	{
		this.type = TweenAction.GUI_ALPHA;
		this.initInternal = () => this.fromInternal.x = this._optional.ltRect.alpha;
		this.easeInternal = () => this._optional.ltRect.alpha = this.easeMethod().x;
		return this;
	}

	public LTDescr setGUIMove()
	{
		this.type = TweenAction.GUI_MOVE;
		this.initInternal = () => {
			Rect rect = this._optional.ltRect.rect;
			float single = rect.x;
			rect = this._optional.ltRect.rect;
			this.@from = new Vector3(single, rect.y, 0f);
		};
		this.easeInternal = () => {
			Vector3 vector3 = this.easeMethod();
			LTRect rect = this._optional.ltRect;
			float single = vector3.x;
			float single1 = vector3.y;
			Rect rect1 = this._optional.ltRect.rect;
			float single2 = rect1.width;
			rect1 = this._optional.ltRect.rect;
			rect.rect = new Rect(single, single1, single2, rect1.height);
		};
		return this;
	}

	public LTDescr setGUIMoveMargin()
	{
		this.type = TweenAction.GUI_MOVE_MARGIN;
		this.initInternal = () => this.@from = new Vector2(this._optional.ltRect.margin.x, this._optional.ltRect.margin.y);
		this.easeInternal = () => {
			Vector3 vector3 = this.easeMethod();
			this._optional.ltRect.margin = new Vector2(vector3.x, vector3.y);
		};
		return this;
	}

	public LTDescr setGUIRotate()
	{
		this.type = TweenAction.GUI_ROTATE;
		this.initInternal = () => {
			if (!this._optional.ltRect.rotateEnabled)
			{
				this._optional.ltRect.rotateEnabled = true;
				this._optional.ltRect.resetForRotation();
			}
			this.fromInternal.x = this._optional.ltRect.rotation;
		};
		this.easeInternal = () => this._optional.ltRect.rotation = this.easeMethod().x;
		return this;
	}

	public LTDescr setGUIScale()
	{
		this.type = TweenAction.GUI_SCALE;
		this.initInternal = () => {
			Rect rect = this._optional.ltRect.rect;
			float single = rect.width;
			rect = this._optional.ltRect.rect;
			this.@from = new Vector3(single, rect.height, 0f);
		};
		this.easeInternal = () => {
			Vector3 vector3 = this.easeMethod();
			LTRect rect = this._optional.ltRect;
			Rect rect1 = this._optional.ltRect.rect;
			float single = rect1.x;
			rect1 = this._optional.ltRect.rect;
			rect.rect = new Rect(single, rect1.y, vector3.x, vector3.y);
		};
		return this;
	}

	public LTDescr setHasInitialized(bool has)
	{
		this.hasInitiliazed = has;
		return this;
	}

	public LTDescr setId(uint id, uint global_counter)
	{
		this._id = id;
		this.counter = global_counter;
		return this;
	}

	public LTDescr setIgnoreTimeScale(bool useUnScaledTime)
	{
		this.useEstimatedTime = useUnScaledTime;
		this.usesNormalDt = false;
		return this;
	}

	public LTDescr setLoopClamp()
	{
		this.loopType = LeanTweenType.clamp;
		if (this.loopCount == 0)
		{
			this.loopCount = -1;
		}
		return this;
	}

	public LTDescr setLoopClamp(int loops)
	{
		this.loopCount = loops;
		return this;
	}

	public LTDescr setLoopCount(int loopCount)
	{
		this.loopType = LeanTweenType.clamp;
		this.loopCount = loopCount;
		return this;
	}

	public LTDescr setLoopOnce()
	{
		this.loopType = LeanTweenType.once;
		return this;
	}

	public LTDescr setLoopPingPong()
	{
		this.loopType = LeanTweenType.pingPong;
		if (this.loopCount == 0)
		{
			this.loopCount = -1;
		}
		return this;
	}

	public LTDescr setLoopPingPong(int loops)
	{
		this.loopType = LeanTweenType.pingPong;
		this.loopCount = (loops == -1 ? loops : loops * 2);
		return this;
	}

	public LTDescr setLoopType(LeanTweenType loopType)
	{
		this.loopType = loopType;
		return this;
	}

	public LTDescr setMove()
	{
		this.type = TweenAction.MOVE;
		this.initInternal = () => this.@from = this.trans.position;
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			this.trans.position = LTDescr.newVect;
		};
		return this;
	}

	public LTDescr setMoveCurved()
	{
		this.type = TweenAction.MOVE_CURVED;
		this.initInternal = new LTDescr.ActionMethodDelegate(this.initFromInternal);
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			if (!this._optional.path.orientToPath)
			{
				this.trans.position = this._optional.path.point(LTDescr.val);
				return;
			}
			if (this._optional.path.orientToPath2d)
			{
				this._optional.path.place2d(this.trans, LTDescr.val);
				return;
			}
			this._optional.path.place(this.trans, LTDescr.val);
		};
		return this;
	}

	public LTDescr setMoveCurvedLocal()
	{
		this.type = TweenAction.MOVE_CURVED_LOCAL;
		this.initInternal = new LTDescr.ActionMethodDelegate(this.initFromInternal);
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			if (!this._optional.path.orientToPath)
			{
				this.trans.localPosition = this._optional.path.point(LTDescr.val);
				return;
			}
			if (this._optional.path.orientToPath2d)
			{
				this._optional.path.placeLocal2d(this.trans, LTDescr.val);
				return;
			}
			this._optional.path.placeLocal(this.trans, LTDescr.val);
		};
		return this;
	}

	public LTDescr setMoveLocal()
	{
		this.type = TweenAction.MOVE_LOCAL;
		this.initInternal = () => this.@from = this.trans.localPosition;
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			this.trans.localPosition = LTDescr.newVect;
		};
		return this;
	}

	public LTDescr setMoveLocalX()
	{
		this.type = TweenAction.MOVE_LOCAL_X;
		this.initInternal = () => this.fromInternal.x = this.trans.localPosition.x;
		this.easeInternal = () => this.trans.localPosition = new Vector3(this.easeMethod().x, this.trans.localPosition.y, this.trans.localPosition.z);
		return this;
	}

	public LTDescr setMoveLocalY()
	{
		this.type = TweenAction.MOVE_LOCAL_Y;
		this.initInternal = () => this.fromInternal.x = this.trans.localPosition.y;
		this.easeInternal = () => this.trans.localPosition = new Vector3(this.trans.localPosition.x, this.easeMethod().x, this.trans.localPosition.z);
		return this;
	}

	public LTDescr setMoveLocalZ()
	{
		this.type = TweenAction.MOVE_LOCAL_Z;
		this.initInternal = () => this.fromInternal.x = this.trans.localPosition.z;
		this.easeInternal = () => this.trans.localPosition = new Vector3(this.trans.localPosition.x, this.trans.localPosition.y, this.easeMethod().x);
		return this;
	}

	public LTDescr setMoveSpline()
	{
		this.type = TweenAction.MOVE_SPLINE;
		this.initInternal = new LTDescr.ActionMethodDelegate(this.initFromInternal);
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			if (!this._optional.spline.orientToPath)
			{
				this.trans.position = this._optional.spline.point(LTDescr.val);
				return;
			}
			if (this._optional.spline.orientToPath2d)
			{
				this._optional.spline.place2d(this.trans, LTDescr.val);
				return;
			}
			this._optional.spline.place(this.trans, LTDescr.val);
		};
		return this;
	}

	public LTDescr setMoveSplineLocal()
	{
		this.type = TweenAction.MOVE_SPLINE_LOCAL;
		this.initInternal = new LTDescr.ActionMethodDelegate(this.initFromInternal);
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			if (!this._optional.spline.orientToPath)
			{
				this.trans.localPosition = this._optional.spline.point(LTDescr.val);
				return;
			}
			if (this._optional.spline.orientToPath2d)
			{
				this._optional.spline.placeLocal2d(this.trans, LTDescr.val);
				return;
			}
			this._optional.spline.placeLocal(this.trans, LTDescr.val);
		};
		return this;
	}

	public LTDescr setMoveToTransform()
	{
		this.type = TweenAction.MOVE_TO_TRANSFORM;
		this.initInternal = () => this.@from = this.trans.position;
		this.easeInternal = () => {
			this.to = this._optional.toTrans.position;
			this.diff = this.to - this.@from;
			this.diffDiv2 = this.diff * 0.5f;
			LTDescr.newVect = this.easeMethod();
			this.trans.position = LTDescr.newVect;
		};
		return this;
	}

	public LTDescr setMoveX()
	{
		this.type = TweenAction.MOVE_X;
		this.initInternal = () => this.fromInternal.x = this.trans.position.x;
		this.easeInternal = () => this.trans.position = new Vector3(this.easeMethod().x, this.trans.position.y, this.trans.position.z);
		return this;
	}

	public LTDescr setMoveY()
	{
		this.type = TweenAction.MOVE_Y;
		this.initInternal = () => this.fromInternal.x = this.trans.position.y;
		this.easeInternal = () => this.trans.position = new Vector3(this.trans.position.x, this.easeMethod().x, this.trans.position.z);
		return this;
	}

	public LTDescr setMoveZ()
	{
		this.type = TweenAction.MOVE_Z;
		this.initInternal = () => this.fromInternal.x = this.trans.position.z;
		this.easeInternal = () => this.trans.position = new Vector3(this.trans.position.x, this.trans.position.y, this.easeMethod().x);
		return this;
	}

	public LTDescr setOnComplete(Action onComplete)
	{
		this._optional.onComplete = onComplete;
		this.hasExtraOnCompletes = true;
		return this;
	}

	public LTDescr setOnComplete(Action<object> onComplete)
	{
		this._optional.onCompleteObject = onComplete;
		this.hasExtraOnCompletes = true;
		return this;
	}

	public LTDescr setOnComplete(Action<object> onComplete, object onCompleteParam)
	{
		this._optional.onCompleteObject = onComplete;
		this.hasExtraOnCompletes = true;
		if (onCompleteParam != null)
		{
			this._optional.onCompleteParam = onCompleteParam;
		}
		return this;
	}

	public LTDescr setOnCompleteOnRepeat(bool isOn)
	{
		this.onCompleteOnRepeat = isOn;
		return this;
	}

	public LTDescr setOnCompleteOnStart(bool isOn)
	{
		this.onCompleteOnStart = isOn;
		return this;
	}

	public LTDescr setOnCompleteParam(object onCompleteParam)
	{
		this._optional.onCompleteParam = onCompleteParam;
		this.hasExtraOnCompletes = true;
		return this;
	}

	public LTDescr setOnStart(Action onStart)
	{
		this._optional.onStart = onStart;
		return this;
	}

	public LTDescr setOnUpdate(Action<float> onUpdate)
	{
		this._optional.onUpdateFloat = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdate(Action<Color> onUpdate)
	{
		this._optional.onUpdateColor = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdate(Action<Color, object> onUpdate)
	{
		this._optional.onUpdateColorObject = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdate(Action<float, object> onUpdate, object onUpdateParam = null)
	{
		this._optional.onUpdateFloatObject = onUpdate;
		this.hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this._optional.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdate(Action<Vector3, object> onUpdate, object onUpdateParam = null)
	{
		this._optional.onUpdateVector3Object = onUpdate;
		this.hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this._optional.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdate(Action<Vector2> onUpdate, object onUpdateParam = null)
	{
		this._optional.onUpdateVector2 = onUpdate;
		this.hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this._optional.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdate(Action<Vector3> onUpdate, object onUpdateParam = null)
	{
		this._optional.onUpdateVector3 = onUpdate;
		this.hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this._optional.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdateColor(Action<Color> onUpdate)
	{
		this._optional.onUpdateColor = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateColor(Action<Color, object> onUpdate)
	{
		this._optional.onUpdateColorObject = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateObject(Action<float, object> onUpdate)
	{
		this._optional.onUpdateFloatObject = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateParam(object onUpdateParam)
	{
		this._optional.onUpdateParam = onUpdateParam;
		return this;
	}

	public LTDescr setOnUpdateRatio(Action<float, float> onUpdate)
	{
		this._optional.onUpdateFloatRatio = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateVector2(Action<Vector2> onUpdate)
	{
		this._optional.onUpdateVector2 = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateVector3(Action<Vector3> onUpdate)
	{
		this._optional.onUpdateVector3 = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOrientToPath(bool doesOrient)
	{
		if (this.type == TweenAction.MOVE_CURVED || this.type == TweenAction.MOVE_CURVED_LOCAL)
		{
			if (this._optional.path == null)
			{
				this._optional.path = new LTBezierPath();
			}
			this._optional.path.orientToPath = doesOrient;
		}
		else
		{
			this._optional.spline.orientToPath = doesOrient;
		}
		return this;
	}

	public LTDescr setOrientToPath2d(bool doesOrient2d)
	{
		this.setOrientToPath(doesOrient2d);
		if (this.type == TweenAction.MOVE_CURVED || this.type == TweenAction.MOVE_CURVED_LOCAL)
		{
			this._optional.path.orientToPath2d = doesOrient2d;
		}
		else
		{
			this._optional.spline.orientToPath2d = doesOrient2d;
		}
		return this;
	}

	public LTDescr setOvershoot(float overshoot)
	{
		this.overshoot = overshoot;
		return this;
	}

	public LTDescr setPassed(float passed)
	{
		this.passed = passed;
		return this;
	}

	public LTDescr setPath(LTBezierPath path)
	{
		this._optional.path = path;
		return this;
	}

	public LTDescr setPeriod(float period)
	{
		this.period = period;
		return this;
	}

	public LTDescr setPoint(Vector3 point)
	{
		this._optional.point = point;
		return this;
	}

	public LTDescr setRect(LTRect rect)
	{
		this._optional.ltRect = rect;
		return this;
	}

	public LTDescr setRect(Rect rect)
	{
		this._optional.ltRect = new LTRect(rect);
		return this;
	}

	public LTDescr setRect(RectTransform rect)
	{
		this.rectTransform = rect;
		return this;
	}

	public LTDescr setRecursive(bool useRecursion)
	{
		this.useRecursion = useRecursion;
		return this;
	}

	public LTDescr setRepeat(int repeat)
	{
		this.loopCount = repeat;
		if (repeat > 1 && this.loopType == LeanTweenType.once || repeat < 0 && this.loopType == LeanTweenType.once)
		{
			this.loopType = LeanTweenType.clamp;
		}
		if (this.type == TweenAction.CALLBACK || this.type == TweenAction.CALLBACK_COLOR)
		{
			this.setOnCompleteOnRepeat(true);
		}
		return this;
	}

	public LTDescr setRotate()
	{
		this.type = TweenAction.ROTATE;
		this.initInternal = () => {
			this.@from = this.trans.eulerAngles;
			this.to = new Vector3(LeanTween.closestRot(this.fromInternal.x, this.toInternal.x), LeanTween.closestRot(this.@from.y, this.to.y), LeanTween.closestRot(this.@from.z, this.to.z));
		};
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			this.trans.eulerAngles = LTDescr.newVect;
		};
		return this;
	}

	public LTDescr setRotateAround()
	{
		this.type = TweenAction.ROTATE_AROUND;
		this.initInternal = () => {
			this.fromInternal.x = 0f;
			this._optional.origRotation = this.trans.rotation;
		};
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			Vector3 vector3 = this.trans.localPosition;
			Vector3 vector31 = this.trans.TransformPoint(this._optional.point);
			this.trans.RotateAround(vector31, this._optional.axis, -this._optional.lastVal);
			Vector3 vector32 = vector3 - this.trans.localPosition;
			this.trans.localPosition = vector3 - vector32;
			this.trans.rotation = this._optional.origRotation;
			vector31 = this.trans.TransformPoint(this._optional.point);
			this.trans.RotateAround(vector31, this._optional.axis, LTDescr.val);
			this._optional.lastVal = LTDescr.val;
		};
		return this;
	}

	public LTDescr setRotateAroundLocal()
	{
		this.type = TweenAction.ROTATE_AROUND_LOCAL;
		this.initInternal = () => {
			this.fromInternal.x = 0f;
			this._optional.origRotation = this.trans.localRotation;
		};
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			Vector3 vector3 = this.trans.localPosition;
			this.trans.RotateAround(this.trans.TransformPoint(this._optional.point), this.trans.TransformDirection(this._optional.axis), -this._optional.lastVal);
			Vector3 vector31 = vector3 - this.trans.localPosition;
			this.trans.localPosition = vector3 - vector31;
			this.trans.localRotation = this._optional.origRotation;
			Vector3 vector32 = this.trans.TransformPoint(this._optional.point);
			this.trans.RotateAround(vector32, this.trans.TransformDirection(this._optional.axis), LTDescr.val);
			this._optional.lastVal = LTDescr.val;
		};
		return this;
	}

	public LTDescr setRotateLocal()
	{
		this.type = TweenAction.ROTATE_LOCAL;
		this.initInternal = () => {
			this.@from = this.trans.localEulerAngles;
			this.to = new Vector3(LeanTween.closestRot(this.fromInternal.x, this.toInternal.x), LeanTween.closestRot(this.@from.y, this.to.y), LeanTween.closestRot(this.@from.z, this.to.z));
		};
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			this.trans.localEulerAngles = LTDescr.newVect;
		};
		return this;
	}

	public LTDescr setRotateX()
	{
		this.type = TweenAction.ROTATE_X;
		this.initInternal = () => {
			this.fromInternal.x = this.trans.eulerAngles.x;
			this.toInternal.x = LeanTween.closestRot(this.fromInternal.x, this.toInternal.x);
		};
		this.easeInternal = () => this.trans.eulerAngles = new Vector3(this.easeMethod().x, this.trans.eulerAngles.y, this.trans.eulerAngles.z);
		return this;
	}

	public LTDescr setRotateY()
	{
		this.type = TweenAction.ROTATE_Y;
		this.initInternal = () => {
			this.fromInternal.x = this.trans.eulerAngles.y;
			this.toInternal.x = LeanTween.closestRot(this.fromInternal.x, this.toInternal.x);
		};
		this.easeInternal = () => this.trans.eulerAngles = new Vector3(this.trans.eulerAngles.x, this.easeMethod().x, this.trans.eulerAngles.z);
		return this;
	}

	public LTDescr setRotateZ()
	{
		this.type = TweenAction.ROTATE_Z;
		this.initInternal = () => {
			this.fromInternal.x = this.trans.eulerAngles.z;
			this.toInternal.x = LeanTween.closestRot(this.fromInternal.x, this.toInternal.x);
		};
		this.easeInternal = () => this.trans.eulerAngles = new Vector3(this.trans.eulerAngles.x, this.trans.eulerAngles.y, this.easeMethod().x);
		return this;
	}

	public LTDescr setScale()
	{
		this.type = TweenAction.SCALE;
		this.initInternal = () => this.@from = this.trans.localScale;
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			this.trans.localScale = LTDescr.newVect;
		};
		return this;
	}

	public LTDescr setScale(float scale)
	{
		this.scale = scale;
		return this;
	}

	public LTDescr setScaleX()
	{
		this.type = TweenAction.SCALE_X;
		this.initInternal = () => this.fromInternal.x = this.trans.localScale.x;
		this.easeInternal = () => this.trans.localScale = new Vector3(this.easeMethod().x, this.trans.localScale.y, this.trans.localScale.z);
		return this;
	}

	public LTDescr setScaleY()
	{
		this.type = TweenAction.SCALE_Y;
		this.initInternal = () => this.fromInternal.x = this.trans.localScale.y;
		this.easeInternal = () => this.trans.localScale = new Vector3(this.trans.localScale.x, this.easeMethod().x, this.trans.localScale.z);
		return this;
	}

	public LTDescr setScaleZ()
	{
		this.type = TweenAction.SCALE_Z;
		this.initInternal = () => this.fromInternal.x = this.trans.localScale.z;
		this.easeInternal = () => this.trans.localScale = new Vector3(this.trans.localScale.x, this.trans.localScale.y, this.easeMethod().x);
		return this;
	}

	public LTDescr setSpeed(float speed)
	{
		this.speed = speed;
		if (this.hasInitiliazed)
		{
			this.initSpeed();
		}
		return this;
	}

	public LTDescr setSprites(Sprite[] sprites)
	{
		this.sprites = sprites;
		return this;
	}

	public LTDescr setTextAlpha()
	{
		this.type = TweenAction.TEXT_ALPHA;
		this.initInternal = () => {
			this.uiText = this.trans.GetComponent<Text>();
			this.fromInternal.x = (this.uiText != null ? this.uiText.color.a : 1f);
		};
		this.easeInternal = () => LTDescr.textAlphaRecursive(this.trans, this.easeMethod().x, this.useRecursion);
		return this;
	}

	public LTDescr setTextColor()
	{
		this.type = TweenAction.TEXT_COLOR;
		this.initInternal = () => {
			this.uiText = this.trans.GetComponent<Text>();
			this.setFromColor((this.uiText != null ? this.uiText.color : Color.white));
		};
		this.easeInternal = () => {
			LTDescr.newVect = this.easeMethod();
			LTDescr.val = LTDescr.newVect.x;
			Color color = LTDescr.tweenColor(this, LTDescr.val);
			this.uiText.color = color;
			if (LTDescr.dt != 0f && this._optional.onUpdateColor != null)
			{
				this._optional.onUpdateColor(color);
			}
			if (this.useRecursion && this.trans.childCount > 0)
			{
				LTDescr.textColorRecursive(this.trans, color);
			}
		};
		return this;
	}

	public LTDescr setTime(float time)
	{
		this.passed = time * (this.passed / this.time);
		this.time = time;
		return this;
	}

	public LTDescr setTo(Vector3 to)
	{
		if (!this.hasInitiliazed)
		{
			this.to = to;
		}
		else
		{
			this.to = to;
			this.diff = to - this.@from;
		}
		return this;
	}

	public LTDescr setTo(Transform to)
	{
		this._optional.toTrans = to;
		return this;
	}

	public LTDescr setUseEstimatedTime(bool useEstimatedTime)
	{
		this.useEstimatedTime = useEstimatedTime;
		this.usesNormalDt = false;
		return this;
	}

	public LTDescr setUseFrames(bool useFrames)
	{
		this.useFrames = useFrames;
		this.usesNormalDt = false;
		return this;
	}

	public LTDescr setUseManualTime(bool useManualTime)
	{
		this.useManualTime = useManualTime;
		this.usesNormalDt = false;
		return this;
	}

	public LTDescr setValue3()
	{
		this.type = TweenAction.VALUE3;
		this.initInternal = () => {
		};
		this.easeInternal = new LTDescr.ActionMethodDelegate(this.callback);
		return this;
	}

	private static void textAlphaChildrenRecursive(Transform trans, float val, bool useRecursion = true)
	{
		if (useRecursion && trans.childCount > 0)
		{
			foreach (Transform tran in trans)
			{
				Text component = tran.GetComponent<Text>();
				if (component != null)
				{
					Color color = component.color;
					color.a = val;
					component.color = color;
				}
				LTDescr.textAlphaChildrenRecursive(tran, val, true);
			}
		}
	}

	private static void textAlphaRecursive(Transform trans, float val, bool useRecursion = true)
	{
		Text component = trans.GetComponent<Text>();
		if (component != null)
		{
			Color color = component.color;
			color.a = val;
			component.color = color;
		}
		if (useRecursion && trans.childCount > 0)
		{
			foreach (object tran in trans)
			{
				LTDescr.textAlphaRecursive((Transform)tran, val, true);
			}
		}
	}

	private static void textColorRecursive(Transform trans, Color toColor)
	{
		if (trans.childCount > 0)
		{
			foreach (Transform tran in trans)
			{
				Text component = tran.GetComponent<Text>();
				if (component != null)
				{
					component.color = toColor;
				}
				LTDescr.textColorRecursive(tran, toColor);
			}
		}
	}

	public override string ToString()
	{
		return string.Concat(new object[] { (this.trans != null ? string.Concat("name:", this.trans.gameObject.name) : "gameObject:null"), " toggle:", this.toggle.ToString(), " passed:", this.passed, " time:", this.time, " delay:", this.delay, " direction:", this.direction, " from:", this.@from, " to:", this.to, " diff:", this.diff, " type:", this.type, " ease:", this.easeType, " useEstimatedTime:", this.useEstimatedTime.ToString(), " id:", this.id, " hasInitiliazed:", this.hasInitiliazed.ToString() });
	}

	private static Color tweenColor(LTDescr tween, float val)
	{
		Vector3 vector3 = tween._optional.point - tween._optional.axis;
		float single = tween.to.y - tween.@from.y;
		return new Color(tween._optional.axis.x + vector3.x * val, tween._optional.axis.y + vector3.y * val, tween._optional.axis.z + vector3.z * val, tween.@from.y + single * val);
	}

	private Vector3 tweenOnCurve()
	{
		return new Vector3(this.@from.x + this.diff.x * this._optional.animationCurve.Evaluate(this.ratioPassed), this.@from.y + this.diff.y * this._optional.animationCurve.Evaluate(this.ratioPassed), this.@from.z + this.diff.z * this._optional.animationCurve.Evaluate(this.ratioPassed));
	}

	public bool updateInternal()
	{
		bool flag;
		object obj;
		float single = this.direction;
		if (this.usesNormalDt)
		{
			LTDescr.dt = LeanTween.dtActual;
		}
		else if (this.useEstimatedTime)
		{
			LTDescr.dt = LeanTween.dtEstimated;
		}
		else if (this.useFrames)
		{
			if (this.optional.initFrameCount == 0)
			{
				obj = null;
			}
			else
			{
				obj = 1;
			}
			LTDescr.dt = (float)obj;
			this.optional.initFrameCount = Time.frameCount;
		}
		else if (this.useManualTime)
		{
			LTDescr.dt = LeanTween.dtManual;
		}
		if (this.delay > 0f || single == 0f)
		{
			this.delay -= LTDescr.dt;
		}
		else
		{
			if (this.trans == null)
			{
				return true;
			}
			if (!this.hasInitiliazed)
			{
				this.init();
			}
			LTDescr.dt *= single;
			this.passed += LTDescr.dt;
			if (this.passed > this.time)
			{
				this.passed = this.time;
			}
			this.ratioPassed = this.passed / this.time;
			this.easeInternal();
			if (this.hasUpdateCallback)
			{
				this._optional.callOnUpdate(LTDescr.val, this.ratioPassed);
			}
			if ((single > 0f ? this.passed >= this.time : this.passed <= 0f))
			{
				this.loopCount--;
				if (this.loopType != LeanTweenType.pingPong)
				{
					this.passed = Mathf.Epsilon;
				}
				else
				{
					this.direction = 0f - single;
				}
				flag = (this.loopCount == 0 ? true : this.loopType == LeanTweenType.once);
				if (!flag && this.onCompleteOnRepeat && this.hasExtraOnCompletes)
				{
					this.callOnCompletes();
				}
				return flag;
			}
		}
		return false;
	}

	public LTDescr updateNow()
	{
		this.updateInternal();
		return this;
	}

	public delegate void ActionMethodDelegate();

	public delegate Vector3 EaseTypeDelegate();
}