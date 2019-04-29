using System;
using UnityEngine;

public class LTGUI
{
	public static int RECT_LEVELS;

	public static int RECTS_PER_LEVEL;

	public static int BUTTONS_MAX;

	private static LTRect[] levels;

	private static int[] levelDepths;

	private static Rect[] buttons;

	private static int[] buttonLevels;

	private static int[] buttonLastFrame;

	private static LTRect r;

	private static Color color;

	private static bool isGUIEnabled;

	private static int global_counter;

	static LTGUI()
	{
		LTGUI.RECT_LEVELS = 5;
		LTGUI.RECTS_PER_LEVEL = 10;
		LTGUI.BUTTONS_MAX = 24;
		LTGUI.color = Color.white;
		LTGUI.isGUIEnabled = false;
		LTGUI.global_counter = 0;
	}

	public LTGUI()
	{
	}

	public static bool checkOnScreen(Rect rect)
	{
		bool flag = rect.x + rect.width < 0f;
		bool flag1 = rect.x > (float)Screen.width;
		bool flag2 = rect.y > (float)Screen.height;
		bool flag3 = rect.y + rect.height < 0f;
		return !(flag | flag1 | flag2 | flag3);
	}

	public static bool checkWithinRect(Vector2 vec2, Rect rect)
	{
		vec2.y = (float)Screen.height - vec2.y;
		if (vec2.x <= rect.x || vec2.x >= rect.x + rect.width || vec2.y <= rect.y)
		{
			return false;
		}
		return vec2.y < rect.y + rect.height;
	}

	public static void destroy(int id)
	{
		int num = id & 65535;
		int num1 = id >> 16;
		if (id >= 0 && LTGUI.levels[num] != null && LTGUI.levels[num].hasInitiliazed && LTGUI.levels[num].counter == num1)
		{
			LTGUI.levels[num] = null;
		}
	}

	public static void destroyAll(int depth)
	{
		int num = depth * LTGUI.RECTS_PER_LEVEL + LTGUI.RECTS_PER_LEVEL;
		for (int i = depth * LTGUI.RECTS_PER_LEVEL; LTGUI.levels != null && i < num; i++)
		{
			LTGUI.levels[i] = null;
		}
	}

	public static LTRect element(LTRect rect, int depth)
	{
		LTGUI.isGUIEnabled = true;
		LTGUI.init();
		int num = depth * LTGUI.RECTS_PER_LEVEL + LTGUI.RECTS_PER_LEVEL;
		int num1 = 0;
		if (rect != null)
		{
			LTGUI.destroy(rect.id);
		}
		if (rect.type == LTGUI.Element_Type.Label && rect.style != null && rect.style.normal.textColor.a <= 0f)
		{
			Debug.LogWarning("Your GUI normal color has an alpha of zero, and will not be rendered.");
		}
		if (rect.relativeRect.width == Single.PositiveInfinity)
		{
			rect.relativeRect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		}
		for (int i = depth * LTGUI.RECTS_PER_LEVEL; i < num; i++)
		{
			LTGUI.r = LTGUI.levels[i];
			if (LTGUI.r == null)
			{
				LTGUI.r = rect;
				LTGUI.r.rotateEnabled = true;
				LTGUI.r.alphaEnabled = true;
				LTGUI.r.setId(i, LTGUI.global_counter);
				LTGUI.levels[i] = LTGUI.r;
				if (num1 >= LTGUI.levelDepths[depth])
				{
					LTGUI.levelDepths[depth] = num1 + 1;
				}
				LTGUI.global_counter++;
				return LTGUI.r;
			}
			num1++;
		}
		Debug.LogError("You ran out of GUI Element spaces");
		return null;
	}

	public static Vector2 firstTouch()
	{
		if (Input.touchCount > 0)
		{
			return Input.touches[0].position;
		}
		if (Input.GetMouseButton(0))
		{
			return Input.mousePosition;
		}
		return new Vector2(Single.NegativeInfinity, Single.NegativeInfinity);
	}

	public static bool hasNoOverlap(Rect rect, int depth)
	{
		LTGUI.initRectCheck();
		bool flag = true;
		bool flag1 = false;
		for (int i = 0; i < (int)LTGUI.buttonLevels.Length; i++)
		{
			if (LTGUI.buttonLevels[i] >= 0)
			{
				if (LTGUI.buttonLastFrame[i] + 1 < Time.frameCount)
				{
					LTGUI.buttonLevels[i] = -1;
				}
				else if (LTGUI.buttonLevels[i] > depth && LTGUI.pressedWithinRect(LTGUI.buttons[i]))
				{
					flag = false;
				}
			}
			if (!flag1 && LTGUI.buttonLevels[i] < 0)
			{
				flag1 = true;
				LTGUI.buttonLevels[i] = depth;
				LTGUI.buttons[i] = rect;
				LTGUI.buttonLastFrame[i] = Time.frameCount;
			}
		}
		return flag;
	}

	public static void init()
	{
		if (LTGUI.levels == null)
		{
			LTGUI.levels = new LTRect[LTGUI.RECT_LEVELS * LTGUI.RECTS_PER_LEVEL];
			LTGUI.levelDepths = new int[LTGUI.RECT_LEVELS];
		}
	}

	public static void initRectCheck()
	{
		if (LTGUI.buttons == null)
		{
			LTGUI.buttons = new Rect[LTGUI.BUTTONS_MAX];
			LTGUI.buttonLevels = new int[LTGUI.BUTTONS_MAX];
			LTGUI.buttonLastFrame = new int[LTGUI.BUTTONS_MAX];
			for (int i = 0; i < (int)LTGUI.buttonLevels.Length; i++)
			{
				LTGUI.buttonLevels[i] = -1;
			}
		}
	}

	public static LTRect label(Rect rect, string label, int depth)
	{
		return LTGUI.label(new LTRect(rect), label, depth);
	}

	public static LTRect label(LTRect rect, string label, int depth)
	{
		rect.type = LTGUI.Element_Type.Label;
		rect.labelStr = label;
		return LTGUI.element(rect, depth);
	}

	public static bool pressedWithinRect(Rect rect)
	{
		Vector2 vector2 = LTGUI.firstTouch();
		if (vector2.x < 0f)
		{
			return false;
		}
		float single = (float)Screen.height - vector2.y;
		if (vector2.x <= rect.x || vector2.x >= rect.x + rect.width || single <= rect.y)
		{
			return false;
		}
		return single < rect.y + rect.height;
	}

	public static void reset()
	{
		if (LTGUI.isGUIEnabled)
		{
			LTGUI.isGUIEnabled = false;
			for (int i = 0; i < (int)LTGUI.levels.Length; i++)
			{
				LTGUI.levels[i] = null;
			}
			for (int j = 0; j < (int)LTGUI.levelDepths.Length; j++)
			{
				LTGUI.levelDepths[j] = 0;
			}
		}
	}

	public static LTRect texture(Rect rect, Texture texture, int depth)
	{
		return LTGUI.texture(new LTRect(rect), texture, depth);
	}

	public static LTRect texture(LTRect rect, Texture texture, int depth)
	{
		rect.type = LTGUI.Element_Type.Texture;
		rect.texture = texture;
		return LTGUI.element(rect, depth);
	}

	public static void update(int updateLevel)
	{
		Rect rect;
		Vector2 vector2;
		if (LTGUI.isGUIEnabled)
		{
			LTGUI.init();
			if (LTGUI.levelDepths[updateLevel] > 0)
			{
				LTGUI.color = GUI.color;
				int num = updateLevel * LTGUI.RECTS_PER_LEVEL;
				int num1 = num + LTGUI.levelDepths[updateLevel];
				for (int i = num; i < num1; i++)
				{
					LTGUI.r = LTGUI.levels[i];
					if (LTGUI.r != null)
					{
						if (LTGUI.r.useColor)
						{
							GUI.color = LTGUI.r.color;
						}
						if (LTGUI.r.type == LTGUI.Element_Type.Label)
						{
							if (LTGUI.r.style != null)
							{
								GUI.skin.label = LTGUI.r.style;
							}
							if (!LTGUI.r.useSimpleScale)
							{
								rect = LTGUI.r.rect;
								float single = rect.x + LTGUI.r.margin.x;
								rect = LTGUI.r.rect;
								float single1 = rect.y + LTGUI.r.margin.y;
								float single2 = LTGUI.r.rect.width;
								rect = LTGUI.r.rect;
								GUI.Label(new Rect(single, single1, single2, rect.height), LTGUI.r.labelStr);
							}
							else
							{
								rect = LTGUI.r.rect;
								float single3 = (rect.x + LTGUI.r.margin.x + LTGUI.r.relativeRect.x) * LTGUI.r.relativeRect.width;
								rect = LTGUI.r.rect;
								float single4 = (rect.y + LTGUI.r.margin.y + LTGUI.r.relativeRect.y) * LTGUI.r.relativeRect.height;
								rect = LTGUI.r.rect;
								float single5 = rect.width * LTGUI.r.relativeRect.width;
								rect = LTGUI.r.rect;
								GUI.Label(new Rect(single3, single4, single5, rect.height * LTGUI.r.relativeRect.height), LTGUI.r.labelStr);
							}
						}
						else if (LTGUI.r.type == LTGUI.Element_Type.Texture && LTGUI.r.texture != null)
						{
							if (LTGUI.r.useSimpleScale)
							{
								rect = LTGUI.r.rect;
								vector2 = new Vector2(0f, rect.height * LTGUI.r.relativeRect.height);
							}
							else
							{
								float single6 = LTGUI.r.rect.width;
								rect = LTGUI.r.rect;
								vector2 = new Vector2(single6, rect.height);
							}
							Vector2 vector21 = vector2;
							if (LTGUI.r.sizeByHeight)
							{
								vector21.x = (float)LTGUI.r.texture.width / (float)LTGUI.r.texture.height * vector21.y;
							}
							if (!LTGUI.r.useSimpleScale)
							{
								rect = LTGUI.r.rect;
								float single7 = rect.x + LTGUI.r.margin.x;
								rect = LTGUI.r.rect;
								GUI.DrawTexture(new Rect(single7, rect.y + LTGUI.r.margin.y, vector21.x, vector21.y), LTGUI.r.texture);
							}
							else
							{
								rect = LTGUI.r.rect;
								float single8 = (rect.x + LTGUI.r.margin.x + LTGUI.r.relativeRect.x) * LTGUI.r.relativeRect.width;
								rect = LTGUI.r.rect;
								GUI.DrawTexture(new Rect(single8, (rect.y + LTGUI.r.margin.y + LTGUI.r.relativeRect.y) * LTGUI.r.relativeRect.height, vector21.x, vector21.y), LTGUI.r.texture);
							}
						}
					}
				}
				GUI.color = LTGUI.color;
			}
		}
	}

	public enum Element_Type
	{
		Texture,
		Label
	}
}