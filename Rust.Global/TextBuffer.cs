using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TextBuffer
{
	private Queue<string> buffer;

	private StringBuilder builder;

	private string text = string.Empty;

	private bool dirty;

	private int curlines;

	private int maxlines;

	private int curchars;

	private int maxchars;

	public int Count
	{
		get
		{
			return this.curlines;
		}
	}

	public TextBuffer(int maxlines, int maxchars = 2147483647)
	{
		this.buffer = new Queue<string>(maxlines + 1);
		this.builder = new StringBuilder();
		this.maxlines = maxlines;
		this.maxchars = maxchars;
	}

	public void Add(string text)
	{
		foreach (string line in StringEx.SplitToLines(text))
		{
			this.buffer.Enqueue(line);
			this.curlines++;
			this.curchars += line.Length;
			while (this.curlines > this.maxlines || this.curchars > this.maxchars)
			{
				this.Remove();
			}
		}
		this.dirty = true;
	}

	public void Clear()
	{
		this.buffer.Clear();
		this.curlines = 0;
		this.curchars = 0;
		this.text = string.Empty;
		this.dirty = true;
	}

	public string Get(int index)
	{
		if (index < 0 || index > this.buffer.Count - 1)
		{
			return string.Empty;
		}
		return this.buffer.ElementAt<string>(this.buffer.Count - 1 - index);
	}

	public void Remove()
	{
		if (this.buffer.Count == 0)
		{
			return;
		}
		string str = this.buffer.Dequeue();
		this.curlines--;
		this.curchars -= str.Length;
	}

	public override string ToString()
	{
		if (this.dirty)
		{
			this.builder.Clear();
			foreach (string str in this.buffer)
			{
				this.builder.AppendLine(str);
			}
			this.text = this.builder.ToString();
			this.dirty = false;
		}
		return this.text;
	}
}