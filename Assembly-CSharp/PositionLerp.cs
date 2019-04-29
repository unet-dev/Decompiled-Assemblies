using System;
using System.Collections.Generic;
using UnityEngine;

public class PositionLerp : ListComponent<PositionLerp>
{
	public static bool DebugLog;

	public static bool DebugDraw;

	private Action idleDisable;

	private TransformInterpolator interpolator = new TransformInterpolator();

	private ILerpTarget target;

	private float timeOffset0 = Single.MaxValue;

	private float timeOffset1 = Single.MaxValue;

	private float timeOffset2 = Single.MaxValue;

	private float timeOffset3 = Single.MaxValue;

	private float lastClientTime;

	private float lastServerTime;

	private float extrapolatedTime;

	static PositionLerp()
	{
	}

	public PositionLerp()
	{
	}

	public static void Cycle()
	{
		PositionLerp[] buffer = ListComponent<PositionLerp>.InstanceList.Values.Buffer;
		int count = ListComponent<PositionLerp>.InstanceList.Count;
		for (int i = 0; i < count; i++)
		{
			buffer[i].DoCycle();
		}
	}

	protected void DoCycle()
	{
		if (this.target == null)
		{
			return;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		TransformInterpolator.Segment segment = this.interpolator.Query(Time.time, interpolationDelay, extrapolationTime, interpolationSmoothing);
		if (segment.next.time < this.interpolator.last.time)
		{
			this.extrapolatedTime = Mathf.Max(this.extrapolatedTime - Time.deltaTime, 0f);
		}
		else
		{
			this.extrapolatedTime = Mathf.Min(this.extrapolatedTime + Time.deltaTime, extrapolationTime);
		}
		if (this.extrapolatedTime > 0f && extrapolationTime > 0f && interpolationSmoothing > 0f)
		{
			float single = Time.deltaTime / (this.extrapolatedTime / extrapolationTime * interpolationSmoothing);
			segment.tick.pos = Vector3.Lerp(this.target.GetNetworkPosition(), segment.tick.pos, single);
			segment.tick.rot = Quaternion.Slerp(this.target.GetNetworkRotation(), segment.tick.rot, single);
		}
		this.target.SetNetworkPosition(segment.tick.pos);
		this.target.SetNetworkRotation(segment.tick.rot);
		if (PositionLerp.DebugDraw)
		{
			this.target.DrawInterpolationState(segment, this.interpolator.list);
		}
		if (Time.time - this.lastClientTime > 10f)
		{
			if (this.idleDisable == null)
			{
				this.idleDisable = new Action(this.IdleDisable);
			}
			InvokeHandler.Invoke(this, this.idleDisable, 0f);
		}
	}

	public Quaternion GetEstimatedAngularVelocity()
	{
		if (this.target == null)
		{
			return Quaternion.identity;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		TransformInterpolator.Segment segment = this.interpolator.Query(Time.time, interpolationDelay, extrapolationTime, interpolationSmoothing);
		TransformInterpolator.Entry entry = segment.next;
		TransformInterpolator.Entry entry1 = segment.prev;
		if (entry.time == entry1.time)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler((entry1.rot.eulerAngles - entry.rot.eulerAngles) / (entry1.time - entry.time));
	}

	public Vector3 GetEstimatedVelocity()
	{
		if (this.target == null)
		{
			return Vector3.zero;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		TransformInterpolator.Segment segment = this.interpolator.Query(Time.time, interpolationDelay, extrapolationTime, interpolationSmoothing);
		TransformInterpolator.Entry entry = segment.next;
		TransformInterpolator.Entry entry1 = segment.prev;
		if (entry.time == entry1.time)
		{
			return Vector3.zero;
		}
		return (entry1.pos - entry.pos) / (entry1.time - entry.time);
	}

	private void IdleDisable()
	{
		base.enabled = false;
	}

	public void Initialize(ILerpTarget target)
	{
		this.target = target;
	}

	public void Snapshot(Vector3 position, Quaternion rotation, float serverTime)
	{
		float interpolationDelay = this.target.GetInterpolationDelay() + this.target.GetInterpolationSmoothing() + 1f;
		float single = Time.time;
		this.timeOffset0 = this.timeOffset1;
		this.timeOffset1 = this.timeOffset2;
		this.timeOffset2 = this.timeOffset3;
		this.timeOffset3 = single - serverTime;
		float single1 = Mathx.Min(this.timeOffset0, this.timeOffset1, this.timeOffset2, this.timeOffset3);
		single = serverTime + single1;
		if (PositionLerp.DebugLog && this.interpolator.list.Count > 0 && serverTime < this.lastServerTime)
		{
			Debug.LogWarning(string.Concat(new object[] { this.target.ToString(), " adding tick from the past: server time ", serverTime, " < ", this.lastServerTime }));
		}
		else if (!PositionLerp.DebugLog || this.interpolator.list.Count <= 0 || single >= this.lastClientTime)
		{
			this.lastClientTime = single;
			this.lastServerTime = serverTime;
			TransformInterpolator transformInterpolator = this.interpolator;
			TransformInterpolator.Entry entry = new TransformInterpolator.Entry()
			{
				time = single,
				pos = position,
				rot = rotation
			};
			transformInterpolator.Add(entry);
		}
		else
		{
			Debug.LogWarning(string.Concat(new object[] { this.target.ToString(), " adding tick from the past: client time ", single, " < ", this.lastClientTime }));
		}
		this.interpolator.Cull(single - interpolationDelay);
	}

	public void SnapTo(Vector3 position, Quaternion rotation, float serverTime)
	{
		this.interpolator.Clear();
		this.Snapshot(position, rotation, serverTime);
		this.target.SetNetworkPosition(position);
		this.target.SetNetworkRotation(rotation);
	}

	public void SnapToEnd()
	{
		float interpolationDelay = this.target.GetInterpolationDelay();
		TransformInterpolator.Segment segment = this.interpolator.Query(Time.time, interpolationDelay, 0f, 0f);
		this.target.SetNetworkPosition(segment.tick.pos);
		this.target.SetNetworkRotation(segment.tick.rot);
		this.interpolator.Clear();
	}

	public void TransformEntries(Matrix4x4 matrix)
	{
		Quaternion quaternion = matrix.rotation;
		for (int i = 0; i < this.interpolator.list.Count; i++)
		{
			TransformInterpolator.Entry item = this.interpolator.list[i];
			item.pos = matrix.MultiplyPoint3x4(item.pos);
			item.rot = quaternion * item.rot;
			this.interpolator.list[i] = item;
		}
		this.interpolator.last.pos = matrix.MultiplyPoint3x4(this.interpolator.last.pos);
		this.interpolator.last.rot = quaternion * this.interpolator.last.rot;
	}
}