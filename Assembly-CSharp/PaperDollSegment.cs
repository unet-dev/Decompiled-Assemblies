using System;
using UnityEngine.UI;

public class PaperDollSegment : BaseMonoBehaviour
{
	public static HitArea selectedAreas;

	[InspectorFlags]
	public HitArea area;

	public Image overlayImg;

	public PaperDollSegment()
	{
	}
}