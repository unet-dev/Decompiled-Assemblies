using System;
using UnityEngine;

[Serializable]
public class LTRect
{
	public Rect _rect;

	public float alpha = 1f;

	public float rotation;

	public Vector2 pivot;

	public Vector2 margin;

	public Rect relativeRect = new Rect(0f, 0f, Single.PositiveInfinity, Single.PositiveInfinity);

	public bool rotateEnabled;

	[HideInInspector]
	public bool rotateFinished;

	public bool alphaEnabled;

	public string labelStr;

	public LTGUI.Element_Type type;

	public GUIStyle style;

	public bool useColor;

	public Color color = Color.white;

	public bool fontScaleToFit;

	public bool useSimpleScale;

	public bool sizeByHeight;

	public Texture texture;

	private int _id = -1;

	[HideInInspector]
	public int counter;

	public static bool colorTouched;

	public bool hasInitiliazed
	{
		get
		{
			return this._id != -1;
		}
	}

	public float height
	{
		get
		{
			return this._rect.height;
		}
		set
		{
			this._rect.height = value;
		}
	}

	public int id
	{
		get
		{
			return this._id | this.counter << 16;
		}
	}

	public Rect rect
	{
		get
		{
			if (LTRect.colorTouched)
			{
				LTRect.colorTouched = false;
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1f);
			}
			if (this.rotateEnabled)
			{
				if (!this.rotateFinished)
				{
					GUIUtility.RotateAroundPivot(this.rotation, this.pivot);
				}
				else
				{
					this.rotateFinished = false;
					this.rotateEnabled = false;
					this.pivot = Vector2.zero;
				}
			}
			if (this.alphaEnabled)
			{
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.alpha);
				LTRect.colorTouched = true;
			}
			if (this.fontScaleToFit)
			{
				if (!this.useSimpleScale)
				{
					this.style.fontSize = (int)this._rect.height;
				}
				else
				{
					this.style.fontSize = (int)(this._rect.height * this.relativeRect.height);
				}
			}
			return this._rect;
		}
		set
		{
			this._rect = value;
		}
	}

	public float width
	{
		get
		{
			return this._rect.width;
		}
		set
		{
			this._rect.width = value;
		}
	}

	public float x
	{
		get
		{
			return this._rect.x;
		}
		set
		{
			this._rect.x = value;
		}
	}

	public float y
	{
		get
		{
			return this._rect.y;
		}
		set
		{
			this._rect.y = value;
		}
	}

	public LTRect()
	{
		this.reset();
		int num = 1;
		bool flag = (bool)num;
		this.alphaEnabled = (bool)num;
		this.rotateEnabled = flag;
		this._rect = new Rect(0f, 0f, 1f, 1f);
	}

	public LTRect(Rect rect)
	{
		this._rect = rect;
		this.reset();
	}

	public LTRect(float x, float y, float width, float height)
	{
		this._rect = new Rect(x, y, width, height);
		this.alpha = 1f;
		this.rotation = 0f;
		int num = 0;
		bool flag = (bool)num;
		this.alphaEnabled = (bool)num;
		this.rotateEnabled = flag;
	}

	public LTRect(float x, float y, float width, float height, float alpha)
	{
		this._rect = new Rect(x, y, width, height);
		this.alpha = alpha;
		this.rotation = 0f;
		int num = 0;
		bool flag = (bool)num;
		this.alphaEnabled = (bool)num;
		this.rotateEnabled = flag;
	}

	public LTRect(float x, float y, float width, float height, float alpha, float rotation)
	{
		this._rect = new Rect(x, y, width, height);
		this.alpha = alpha;
		this.rotation = rotation;
		int num = 0;
		bool flag = (bool)num;
		this.alphaEnabled = (bool)num;
		this.rotateEnabled = flag;
		if (rotation != 0f)
		{
			this.rotateEnabled = true;
			this.resetForRotation();
		}
	}

	public void reset()
	{
		this.alpha = 1f;
		this.rotation = 0f;
		int num = 0;
		bool flag = (bool)num;
		this.alphaEnabled = (bool)num;
		this.rotateEnabled = flag;
		this.margin = Vector2.zero;
		this.sizeByHeight = false;
		this.useColor = false;
	}

	public void resetForRotation()
	{
		Matrix4x4 matrix4x4 = GUI.matrix;
		float item = matrix4x4[0, 0];
		matrix4x4 = GUI.matrix;
		float single = matrix4x4[1, 1];
		matrix4x4 = GUI.matrix;
		Vector3 vector3 = new Vector3(item, single, matrix4x4[2, 2]);
		if (this.pivot == Vector2.zero)
		{
			float single1 = (this._rect.x + this._rect.width * 0.5f) * vector3.x;
			matrix4x4 = GUI.matrix;
			float item1 = single1 + matrix4x4[0, 3];
			float single2 = (this._rect.y + this._rect.height * 0.5f) * vector3.y;
			matrix4x4 = GUI.matrix;
			this.pivot = new Vector2(item1, single2 + matrix4x4[1, 3]);
		}
	}

	public LTRect setAlpha(float alpha)
	{
		this.alpha = alpha;
		return this;
	}

	public LTRect setColor(Color color)
	{
		this.color = color;
		this.useColor = true;
		return this;
	}

	public LTRect setFontScaleToFit(bool fontScaleToFit)
	{
		this.fontScaleToFit = fontScaleToFit;
		return this;
	}

	public void setId(int id, int counter)
	{
		this._id = id;
		this.counter = counter;
	}

	public LTRect setLabel(string str)
	{
		this.labelStr = str;
		return this;
	}

	public LTRect setSizeByHeight(bool sizeByHeight)
	{
		this.sizeByHeight = sizeByHeight;
		return this;
	}

	public LTRect setStyle(GUIStyle style)
	{
		this.style = style;
		return this;
	}

	public LTRect setUseSimpleScale(bool useSimpleScale, Rect relativeRect)
	{
		this.useSimpleScale = useSimpleScale;
		this.relativeRect = relativeRect;
		return this;
	}

	public LTRect setUseSimpleScale(bool useSimpleScale)
	{
		this.useSimpleScale = useSimpleScale;
		this.relativeRect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		return this;
	}

	public override string ToString()
	{
		return string.Concat(new object[] { "x:", this._rect.x, " y:", this._rect.y, " width:", this._rect.width, " height:", this._rect.height });
	}
}