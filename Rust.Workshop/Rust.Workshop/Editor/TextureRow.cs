using Facepunch.Extend;
using Facepunch.Utility;
using Rust;
using Rust.UI;
using Rust.Workshop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.Workshop.Editor
{
	public class TextureRow : MaterialRow
	{
		private string Filename;

		public bool IsNormalMap;

		public RawImage TargetImage;

		public Text FilenameLabel;

		public Button Reset;

		public Button Clear;

		public bool HasChanges;

		private UnityEngine.Texture Default;

		private FileSystemWatcher watcher;

		public bool IsClear
		{
			get
			{
				return this.TargetImage.texture == null;
			}
		}

		public bool IsDefault
		{
			get
			{
				return this.Default == this.TargetImage.texture;
			}
		}

		public TextureRow()
		{
		}

		public IEnumerator BrowseForTexture()
		{
			TextureRow textureRow = null;
			yield return textureRow.StartCoroutine(textureRow.Editor.FileDialog.Open(null, ".png|.jpg", "OPEN FILE", null, (long)-1, true));
			if (textureRow.Editor.FileDialog.result == null)
			{
				yield break;
			}
			FileInfo fileInfo = new FileInfo(textureRow.Editor.FileDialog.result);
			try
			{
				textureRow.Load(fileInfo.FullName);
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogWarning(string.Concat("Couldn't load texture: ", exception.Message));
			}
		}

		private IEnumerator DoSaveFile()
		{
			TextureRow textureRow = null;
			yield return textureRow.StartCoroutine(textureRow.Editor.FileDialog.Save(null, ".png", "SAVE FILE", null, true));
			if (string.IsNullOrEmpty(textureRow.Editor.FileDialog.result))
			{
				yield break;
			}
			UnityEngine.Debug.Log(string.Concat("Save Png to ", textureRow.Editor.FileDialog.result));
			UnityEngine.Texture targetImage = textureRow.TargetImage.texture;
			if (textureRow.IsNormalMap)
			{
				targetImage = Facepunch.Utility.Texture.CreateReadableCopy(targetImage as Texture2D, 0, 0);
				(targetImage as Texture2D).DecompressNormals();
			}
			targetImage.SaveAsPng(textureRow.Editor.FileDialog.result);
			if (textureRow.IsNormalMap)
			{
				UnityEngine.Object.Destroy(targetImage);
			}
		}

		public void FileChanged(string name)
		{
			lock (this)
			{
				this.HasChanges = true;
			}
		}

		public void Load(string fullname)
		{
			this.StopWatching();
			FileInfo fileInfo = new FileInfo(fullname);
			if (!fileInfo.Exists)
			{
				return;
			}
			Texture2D texture2D = base.Editor.SetTexture(this.ParamName, fileInfo.FullName, this.IsNormalMap);
			if (texture2D)
			{
				this.SetTexture(texture2D);
				this.StartWatching();
			}
		}

		public void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			this.StopWatching();
		}

		public void OpenFileBrowser()
		{
			base.StartCoroutine(this.BrowseForTexture());
		}

		public void OpenFileLocation()
		{
			if (this.IsDefault)
			{
				return;
			}
			Os.OpenFolder(this.Filename);
		}

		public override void Read(Material source, Material def)
		{
			this.StopWatching();
			UnityEngine.Texture texture = source.GetTexture(this.ParamName);
			this.Default = def.GetTexture(this.ParamName);
			this.SetTexture(texture);
		}

		public void ResetToClear()
		{
			this.StopWatching();
			this.SetTexture(null);
			base.Editor.SetTexture(this.ParamName, null);
		}

		public void ResetToDefault()
		{
			this.StopWatching();
			this.SetTexture(this.Default);
			base.Editor.SetTexture(this.ParamName, this.Default);
		}

		public void SaveFile()
		{
			if (this.TargetImage.texture == null)
			{
				return;
			}
			base.StartCoroutine(this.DoSaveFile());
		}

		public void SetFilenameText(string filename)
		{
			this.FilenameLabel.text = filename.TruncateFilename(48, null);
		}

		public void SetTexture(UnityEngine.Texture tex)
		{
			this.TargetImage.texture = tex;
			if (tex != null)
			{
				this.Filename = tex.name;
				this.SetFilenameText(this.Filename);
			}
			if (this.IsDefault)
			{
				this.Filename = "Default";
				this.SetFilenameText(this.Filename);
			}
			if (tex == null)
			{
				this.Filename = "None";
				this.SetFilenameText(this.Filename);
			}
		}

		public void StartWatching()
		{
			this.StopWatching();
			FileInfo fileInfo = new FileInfo(this.Filename);
			if (!fileInfo.Exists)
			{
				return;
			}
			this.watcher = new FileSystemWatcher()
			{
				Path = fileInfo.Directory.FullName,
				Filter = fileInfo.Name,
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
			};
			this.watcher.Changed += new FileSystemEventHandler((object a, FileSystemEventArgs e) => this.FileChanged(e.FullPath));
			this.watcher.EnableRaisingEvents = true;
		}

		public void StopWatching()
		{
			if (this.watcher == null)
			{
				return;
			}
			this.watcher.Dispose();
			this.watcher = null;
		}

		public void Update()
		{
			this.Reset.gameObject.SetActive(!this.IsDefault);
			this.Clear.gameObject.SetActive(!this.IsClear);
			lock (this)
			{
				if (this.HasChanges)
				{
					try
					{
						this.Load(this.Filename);
						this.HasChanges = false;
					}
					catch
					{
						Thread.Sleep(10);
					}
				}
			}
		}
	}
}