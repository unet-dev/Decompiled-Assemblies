using System;
using System.Collections.Generic;
using UnityEngine;

public class TransformInterpolator
{
	public List<TransformInterpolator.Entry> list = new List<TransformInterpolator.Entry>(32);

	public TransformInterpolator.Entry last;

	public TransformInterpolator()
	{
	}

	public void Add(TransformInterpolator.Entry tick)
	{
		this.last = tick;
		this.list.Add(tick);
	}

	public void Clear()
	{
		this.list.Clear();
	}

	public void Cull(float beforeTime)
	{
		for (int i = 0; i < this.list.Count; i++)
		{
			if (this.list[i].time < beforeTime)
			{
				this.list.RemoveAt(i);
				i--;
			}
		}
	}

	public TransformInterpolator.Segment Query(float time, float interpolation, float extrapolation, float smoothing)
	{
		TransformInterpolator.Segment segment = new TransformInterpolator.Segment();
		if (this.list.Count == 0)
		{
			segment.prev = this.last;
			segment.next = this.last;
			segment.tick = this.last;
			return segment;
		}
		float single = time - interpolation - smoothing * 0.5f;
		float single1 = Mathf.Min(time - interpolation, this.last.time);
		float single2 = single1 - smoothing;
		TransformInterpolator.Entry item = this.list[0];
		TransformInterpolator.Entry entry = this.last;
		TransformInterpolator.Entry item1 = this.list[0];
		TransformInterpolator.Entry entry1 = this.last;
		foreach (TransformInterpolator.Entry entry2 in this.list)
		{
			if (entry2.time < single2)
			{
				item = entry2;
			}
			else if (entry.time >= entry2.time)
			{
				entry = entry2;
			}
			if (entry2.time >= single1)
			{
				if (entry1.time < entry2.time)
				{
					continue;
				}
				entry1 = entry2;
			}
			else
			{
				item1 = entry2;
			}
		}
		TransformInterpolator.Entry entry3 = new TransformInterpolator.Entry();
		if (entry.time - item.time >= Mathf.Epsilon)
		{
			float single3 = (single2 - item.time) / (entry.time - item.time);
			entry3.time = single2;
			entry3.pos = Vector3.LerpUnclamped(item.pos, entry.pos, single3);
			entry3.rot = Quaternion.SlerpUnclamped(item.rot, entry.rot, single3);
		}
		else
		{
			entry3.time = single2;
			entry3.pos = entry.pos;
			entry3.rot = entry.rot;
		}
		segment.prev = entry3;
		TransformInterpolator.Entry entry4 = new TransformInterpolator.Entry();
		if (entry1.time - item1.time >= Mathf.Epsilon)
		{
			float single4 = (single1 - item1.time) / (entry1.time - item1.time);
			entry4.time = single1;
			entry4.pos = Vector3.LerpUnclamped(item1.pos, entry1.pos, single4);
			entry4.rot = Quaternion.SlerpUnclamped(item1.rot, entry1.rot, single4);
		}
		else
		{
			entry4.time = single1;
			entry4.pos = entry1.pos;
			entry4.rot = entry1.rot;
		}
		segment.next = entry4;
		if (entry4.time - entry3.time < Mathf.Epsilon)
		{
			segment.prev = entry4;
			segment.tick = entry4;
			return segment;
		}
		if (single - entry4.time > extrapolation)
		{
			segment.prev = entry4;
			segment.tick = entry4;
			return segment;
		}
		TransformInterpolator.Entry entry5 = new TransformInterpolator.Entry();
		float single5 = Mathf.Min(single - entry3.time, entry4.time + extrapolation - entry3.time) / (entry4.time - entry3.time);
		entry5.time = single;
		entry5.pos = Vector3.LerpUnclamped(entry3.pos, entry4.pos, single5);
		entry5.rot = Quaternion.SlerpUnclamped(entry3.rot, entry4.rot, single5);
		segment.tick = entry5;
		return segment;
	}

	public struct Entry
	{
		public float time;

		public Vector3 pos;

		public Quaternion rot;
	}

	public struct Segment
	{
		public TransformInterpolator.Entry tick;

		public TransformInterpolator.Entry prev;

		public TransformInterpolator.Entry next;
	}
}