using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class OccludeeState : OcclusionCulling.SmartListValue
{
	public int slot;

	public bool isStatic;

	public int layer;

	public OcclusionCulling.OnVisibilityChanged onVisibilityChanged;

	public OcclusionCulling.Cell cell;

	public OcclusionCulling.SimpleList<OccludeeState.State> states;

	public bool isVisible
	{
		get
		{
			return this.states[this.slot].isVisible != 0;
		}
	}

	public OccludeeState()
	{
	}

	public OccludeeState Initialize(OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.BufferSet set, int slot, Vector4 sphereBounds, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged)
	{
		OcclusionCulling.SimpleList<OccludeeState.State> simpleList = states;
		int num = slot;
		OccludeeState.State state = new OccludeeState.State()
		{
			sphereBounds = sphereBounds,
			minTimeVisible = minTimeVisible,
			waitTime = (isVisible ? Time.time + minTimeVisible : 0f),
			waitFrame = (uint)(Time.frameCount + 1),
			isVisible = (byte)((isVisible ? 1 : 0)),
			active = 1,
			callback = (byte)((onVisibilityChanged != null ? 1 : 0))
		};
		simpleList[num] = state;
		this.slot = slot;
		this.isStatic = isStatic;
		this.layer = layer;
		this.onVisibilityChanged = onVisibilityChanged;
		this.cell = null;
		this.states = states;
		return this;
	}

	public void Invalidate()
	{
		this.states[this.slot] = OccludeeState.State.Unused;
		this.slot = -1;
		this.onVisibilityChanged = null;
		this.cell = null;
	}

	public void MakeVisible()
	{
		this.states.array[this.slot].waitTime = Time.time + this.states[this.slot].minTimeVisible;
		this.states.array[this.slot].isVisible = 1;
		if (this.onVisibilityChanged != null)
		{
			this.onVisibilityChanged(true);
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct State
	{
		[FieldOffset(0)]
		public Vector4 sphereBounds;

		[FieldOffset(16)]
		public float minTimeVisible;

		[FieldOffset(20)]
		public float waitTime;

		[FieldOffset(24)]
		public uint waitFrame;

		[FieldOffset(28)]
		public byte isVisible;

		[FieldOffset(29)]
		public byte active;

		[FieldOffset(30)]
		public byte callback;

		[FieldOffset(31)]
		public byte pad1;

		[FieldOffset(-1)]
		public static OccludeeState.State Unused;

		static State()
		{
			OccludeeState.State.Unused = new OccludeeState.State()
			{
				active = 0
			};
		}
	}
}