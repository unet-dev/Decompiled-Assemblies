using System;
using UnityEngine;

public class LTSeq
{
	public LTSeq previous;

	public LTSeq current;

	public LTDescr tween;

	public float totalDelay;

	public float timeScale;

	private int debugIter;

	public uint counter;

	public bool toggle;

	private uint _id;

	public int id
	{
		get
		{
			return (int)(this._id | this.counter << 16);
		}
	}

	public LTSeq()
	{
	}

	private LTSeq addOn()
	{
		this.current.toggle = true;
		LTSeq lTSeq = this.current;
		this.current = LeanTween.sequence(false);
		this.current.previous = lTSeq;
		lTSeq.toggle = false;
		this.current.totalDelay = lTSeq.totalDelay;
		this.current.debugIter = lTSeq.debugIter + 1;
		return this.current;
	}

	private float addPreviousDelays()
	{
		LTSeq lTSeq = this.current.previous;
		if (lTSeq == null || lTSeq.tween == null)
		{
			return this.current.totalDelay;
		}
		return this.current.totalDelay + lTSeq.tween.time;
	}

	public LTSeq append(float delay)
	{
		this.current.totalDelay += delay;
		return this.current;
	}

	public LTSeq append(Action callback)
	{
		this.append(LeanTween.delayedCall(0f, callback));
		return this.addOn();
	}

	public LTSeq append(Action<object> callback, object obj)
	{
		this.append(LeanTween.delayedCall(0f, callback).setOnCompleteParam(obj));
		return this.addOn();
	}

	public LTSeq append(GameObject gameObject, Action callback)
	{
		this.append(LeanTween.delayedCall(gameObject, 0f, callback));
		return this.addOn();
	}

	public LTSeq append(GameObject gameObject, Action<object> callback, object obj)
	{
		this.append(LeanTween.delayedCall(gameObject, 0f, callback).setOnCompleteParam(obj));
		return this.addOn();
	}

	public LTSeq append(LTDescr tween)
	{
		this.current.tween = tween;
		this.current.totalDelay = this.addPreviousDelays();
		tween.setDelay(this.current.totalDelay);
		return this.addOn();
	}

	public void init(uint id, uint global_counter)
	{
		this.reset();
		this._id = id;
		this.counter = global_counter;
		this.current = this;
	}

	public LTSeq insert(LTDescr tween)
	{
		this.current.tween = tween;
		tween.setDelay(this.addPreviousDelays());
		return this.addOn();
	}

	public void reset()
	{
		this.previous = null;
		this.tween = null;
		this.totalDelay = 0f;
	}

	public LTSeq reverse()
	{
		return this.addOn();
	}

	public LTSeq setScale(float timeScale)
	{
		this.setScaleRecursive(this.current, timeScale, 500);
		return this.addOn();
	}

	private void setScaleRecursive(LTSeq seq, float timeScale, int count)
	{
		if (count > 0)
		{
			this.timeScale = timeScale;
			seq.totalDelay *= timeScale;
			if (seq.tween != null)
			{
				if (seq.tween.time != 0f)
				{
					seq.tween.setTime(seq.tween.time * timeScale);
				}
				seq.tween.setDelay(seq.tween.delay * timeScale);
			}
			if (seq.previous != null)
			{
				this.setScaleRecursive(seq.previous, timeScale, count - 1);
			}
		}
	}
}