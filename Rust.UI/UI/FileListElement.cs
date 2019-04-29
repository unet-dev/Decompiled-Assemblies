using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI
{
	public class FileListElement : MonoBehaviour
	{
		public Image icon;

		public Text elementName;

		public Text size;

		public FileDialog instance;

		public bool isFile;

		public string data;

		public FileListElement()
		{
		}

		public void OnClick()
		{
			if (!this.isFile)
			{
				this.instance.OpenDir(this.data);
				return;
			}
			this.instance.SelectFile(this.data);
		}
	}
}