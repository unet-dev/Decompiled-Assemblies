using Facepunch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PieMenu : UIBehaviour
{
	public static PieMenu Instance;

	public Image middleBox;

	public PieShape pieBackgroundBlur;

	public PieShape pieBackground;

	public PieShape pieSelection;

	public GameObject pieOptionPrefab;

	public GameObject optionsCanvas;

	public PieMenu.MenuOption[] options;

	public GameObject scaleTarget;

	public float sliceGaps = 10f;

	[Range(0f, 1f)]
	public float outerSize = 1f;

	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	[Range(0f, 1f)]
	public float iconSize = 0.8f;

	[Range(0f, 360f)]
	public float startRadius;

	[Range(0f, 360f)]
	public float radiusSize = 360f;

	public Image middleImage;

	public Text middleTitle;

	public Text middleDesc;

	public Text middleRequired;

	public Color colorIconActive;

	public Color colorIconHovered;

	public Color colorIconDisabled;

	public Color colorBackgroundDisabled;

	public SoundDefinition clipOpen;

	public SoundDefinition clipCancel;

	public SoundDefinition clipChanged;

	public SoundDefinition clipSelected;

	public PieMenu.MenuOption defaultOption;

	private bool isClosing;

	private CanvasGroup canvasGroup;

	public bool IsOpen;

	internal PieMenu.MenuOption selectedOption;

	private static AnimationCurve easePunch;

	static PieMenu()
	{
		PieMenu.easePunch = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.112586f, 0.9976035f), new Keyframe(0.3120486f, 0.01720615f), new Keyframe(0.4316337f, 0.170306817f), new Keyframe(0.5524869f, 0.03141804f), new Keyframe(0.6549395f, 0.002909959f), new Keyframe(0.770987f, 0.009817753f), new Keyframe(0.8838775f, 0.001939224f), new Keyframe(1f, 0f) });
	}

	public PieMenu()
	{
	}

	public void AddOption(PieMenu.MenuOption option)
	{
		List<PieMenu.MenuOption> list = this.options.ToList<PieMenu.MenuOption>();
		list.Add(option);
		this.options = list.ToArray();
	}

	public void Clear()
	{
		this.options = new PieMenu.MenuOption[0];
	}

	public void Close(bool success = false)
	{
		if (this.isClosing)
		{
			return;
		}
		this.isClosing = true;
		NeedsCursor component = base.GetComponent<NeedsCursor>();
		if (component != null)
		{
			component.enabled = false;
		}
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setEase(LeanTweenType.easeOutCirc);
		LeanTween.scale(this.scaleTarget, Vector3.one * (success ? 1.5f : 0.5f), 0.2f).setEase(LeanTweenType.easeOutCirc);
		this.IsOpen = false;
	}

	public bool DoSelect()
	{
		return true;
	}

	public void FinishAndOpen()
	{
		this.IsOpen = true;
		this.isClosing = false;
		this.SetDefaultOption();
		this.Rebuild();
		this.UpdateInteraction(false);
		this.PlayOpenSound();
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f).setEase(LeanTweenType.easeOutCirc);
		this.scaleTarget.transform.localScale = Vector3.one * 1.5f;
		LeanTween.scale(this.scaleTarget, Vector3.one, 0.1f).setEase(LeanTweenType.easeOutBounce);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		this.Rebuild();
	}

	public void PlayCancelSound()
	{
	}

	public void PlayOpenSound()
	{
	}

	public void Rebuild()
	{
		this.options = (
			from x in (IEnumerable<PieMenu.MenuOption>)this.options
			orderby x.order
			select x).ToArray<PieMenu.MenuOption>();
		while (this.optionsCanvas.transform.childCount > 0)
		{
			GameManager.DestroyImmediate(this.optionsCanvas.transform.GetChild(0).gameObject, true);
		}
		float length = this.radiusSize / (float)((int)this.options.Length);
		for (int i = 0; i < (int)this.options.Length; i++)
		{
			GameObject gameObject = Instantiate.GameObject(this.pieOptionPrefab, null);
			gameObject.transform.SetParent(this.optionsCanvas.transform, false);
			this.options[i].option = gameObject.GetComponent<PieOption>();
			this.options[i].option.UpdateOption(this.startRadius + (float)i * length - length * 0.25f, length, this.sliceGaps, this.options[i].name, this.outerSize, this.innerSize, this.iconSize, this.options[i].sprite);
		}
		this.selectedOption = null;
	}

	public void SetDefaultOption()
	{
		this.defaultOption = null;
		for (int i = 0; i < (int)this.options.Length; i++)
		{
			if (!this.options[i].disabled)
			{
				if (this.defaultOption == null)
				{
					this.defaultOption = this.options[i];
				}
				if (this.options[i].selected)
				{
					this.defaultOption = this.options[i];
					return;
				}
			}
		}
	}

	protected override void Start()
	{
		base.Start();
		PieMenu.Instance = this;
		this.canvasGroup = base.GetComponentInChildren<CanvasGroup>();
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.interactable = false;
		this.canvasGroup.blocksRaycasts = false;
		this.IsOpen = false;
		this.isClosing = true;
	}

	private void Update()
	{
		if (!UnityEngine.Application.isPlaying)
		{
			this.Rebuild();
		}
		if (this.pieBackground.innerSize != this.innerSize || this.pieBackground.outerSize != this.outerSize || this.pieBackground.startRadius != this.startRadius || this.pieBackground.endRadius != this.startRadius + this.radiusSize)
		{
			this.pieBackground.startRadius = this.startRadius;
			this.pieBackground.endRadius = this.startRadius + this.radiusSize;
			this.pieBackground.innerSize = this.innerSize;
			this.pieBackground.outerSize = this.outerSize;
			this.pieBackground.SetVerticesDirty();
		}
		this.UpdateInteraction(true);
		if (this.IsOpen)
		{
			CursorManager.HoldOpen(false);
			IngameMenuBackground.Enabled = true;
		}
	}

	public void UpdateInteraction(bool allowLerp = true)
	{
		if (this.isClosing)
		{
			return;
		}
		Vector3 vector3 = UnityEngine.Input.mousePosition - new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
		float single = Mathf.Atan2(vector3.x, vector3.y) * 57.29578f;
		if (single < 0f)
		{
			single += 360f;
		}
		float length = this.radiusSize / (float)((int)this.options.Length);
		for (int i = 0; i < (int)this.options.Length; i++)
		{
			float single1 = this.startRadius + (float)i * length + length * 0.5f - length * 0.25f;
			if ((vector3.magnitude >= 32f || this.options[i] != this.defaultOption) && (vector3.magnitude < 32f || Mathf.Abs(Mathf.DeltaAngle(single, single1)) >= length * 0.5f))
			{
				this.options[i].option.imageIcon.color = this.colorIconActive;
			}
			else
			{
				if (!allowLerp)
				{
					this.pieSelection.startRadius = this.options[i].option.background.startRadius;
					this.pieSelection.endRadius = this.options[i].option.background.endRadius;
				}
				else
				{
					this.pieSelection.startRadius = Mathf.MoveTowardsAngle(this.pieSelection.startRadius, this.options[i].option.background.startRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(this.pieSelection.startRadius, this.options[i].option.background.startRadius) * 30f + 10f));
					this.pieSelection.endRadius = Mathf.MoveTowardsAngle(this.pieSelection.endRadius, this.options[i].option.background.endRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(this.pieSelection.endRadius, this.options[i].option.background.endRadius) * 30f + 10f));
				}
				this.pieSelection.SetVerticesDirty();
				this.middleImage.sprite = this.options[i].sprite;
				this.middleTitle.text = this.options[i].name;
				this.middleDesc.text = this.options[i].desc;
				this.middleRequired.text = "";
				string str = this.options[i].requirements;
				if (str != null)
				{
					str = str.Replace("[e]", "<color=#CD412B>");
					str = str.Replace("[/e]", "</color>");
					this.middleRequired.text = str;
				}
				this.options[i].option.imageIcon.color = this.colorIconHovered;
				if (this.selectedOption != this.options[i])
				{
					if (this.selectedOption != null && !this.options[i].disabled)
					{
						this.scaleTarget.transform.localScale = Vector3.one;
						LeanTween.scale(this.scaleTarget, Vector3.one * 1.03f, 0.2f).setEase(PieMenu.easePunch);
					}
					this.selectedOption = this.options[i];
				}
			}
			if (this.options[i].disabled)
			{
				this.options[i].option.imageIcon.color = this.colorIconDisabled;
				this.options[i].option.background.color = this.colorBackgroundDisabled;
			}
		}
	}

	[Serializable]
	public class MenuOption
	{
		public string name;

		public string desc;

		public string requirements;

		public Sprite sprite;

		public bool disabled;

		public int order;

		[NonSerialized]
		public Action<BasePlayer> action;

		[NonSerialized]
		public PieOption option;

		[NonSerialized]
		public bool selected;

		public MenuOption()
		{
		}
	}
}