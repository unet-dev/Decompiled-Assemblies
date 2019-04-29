using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TextTable
{
	private List<TextTable.Row> rows = new List<TextTable.Row>();

	private List<TextTable.Column> columns = new List<TextTable.Column>();

	private StringBuilder builder = new StringBuilder();

	private string text = string.Empty;

	private bool dirty;

	public TextTable()
	{
	}

	public void AddColumn(string title)
	{
		this.columns.Add(new TextTable.Column(title));
		this.dirty = true;
	}

	public void AddColumns(params string[] values)
	{
		for (int i = 0; i < (int)values.Length; i++)
		{
			this.columns.Add(new TextTable.Column(values[i]));
		}
		this.dirty = true;
	}

	public void AddRow(params string[] values)
	{
		int num = Mathf.Min(this.columns.Count, (int)values.Length);
		for (int i = 0; i < num; i++)
		{
			this.columns[i].width = Mathf.Max(this.columns[i].width, values[i].Length);
		}
		this.rows.Add(new TextTable.Row(values));
		this.dirty = true;
	}

	public void Clear()
	{
		this.rows.Clear();
		this.columns.Clear();
		this.dirty = true;
	}

	public override string ToString()
	{
		if (this.dirty)
		{
			this.builder.Clear();
			for (int i = 0; i < this.columns.Count; i++)
			{
				this.builder.Append(this.columns[i].title.PadRight(this.columns[i].width + 1));
			}
			this.builder.AppendLine();
			for (int j = 0; j < this.rows.Count; j++)
			{
				TextTable.Row item = this.rows[j];
				int num = Mathf.Min(this.columns.Count, (int)item.values.Length);
				for (int k = 0; k < num; k++)
				{
					this.builder.Append(item.values[k].PadRight(this.columns[k].width + 1));
				}
				this.builder.AppendLine();
			}
			this.text = this.builder.ToString();
			this.dirty = false;
		}
		return this.text;
	}

	private class Column
	{
		public string title;

		public int width;

		public Column(string title)
		{
			this.title = title;
			this.width = title.Length;
		}
	}

	private class Row
	{
		public string[] values;

		public Row(string[] values)
		{
			this.values = values;
		}
	}
}