using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI
{
	public class FileDialog : MonoBehaviour
	{
		[HideInInspector]
		public string result;

		[HideInInspector]
		private FileDialog.FileDialogMode mode;

		[HideInInspector]
		public bool finished;

		[Header("References")]
		public Image windowIcon;

		public Text windowName;

		public InputField currentPath;

		public InputField fileName;

		public Button up;

		public Button commit;

		public Button cancel;

		public GameObject filesScrollRectContent;

		public GameObject drivesScrollRectContent;

		[Header("Lists Prefabs")]
		public GameObject filesScrollRectElement;

		public GameObject drivesScrollRectElement;

		[Header("Lists Icons")]
		public Sprite folderIcon;

		public Sprite fileIcon;

		private string workingPath;

		private string workingFile;

		private string[] allowedExtensions;

		private long maxSize = (long)-1;

		private bool saveLastPath = true;

		public FileDialog()
		{
		}

		public void ClearSelection()
		{
			if (this.mode == FileDialog.FileDialogMode.Open)
			{
				this.workingFile = "";
				this.UpdateFileInfo();
			}
		}

		private string GetFileSizeText(long size)
		{
			float single;
			string str = "#.##";
			if ((float)size / 1024f < 1f)
			{
				return "1 Kb";
			}
			if ((float)size / 1024f < 1024f)
			{
				single = (float)size / 1024f;
				return string.Concat(single.ToString(str), " Kb");
			}
			if ((float)size / 1024f / 1024f < 1024f)
			{
				single = (float)size / 1024f / 1024f;
				return string.Concat(single.ToString(str), " Mb");
			}
			single = (float)size / 1024f / 1024f / 1024f;
			return string.Concat(single.ToString(str), " Gb");
		}

		public void GoTo(string newPath)
		{
			if ((new DirectoryInfo(newPath)).Exists)
			{
				this.OpenDir(string.Concat(newPath, "/"));
				return;
			}
			if (this.mode == FileDialog.FileDialogMode.Open)
			{
				if (!(new FileInfo(newPath)).Exists)
				{
					this.OpenDir(string.Concat(Rust.Application.dataPath, "/../"));
					return;
				}
				this.OpenDir(string.Concat((new FileInfo(newPath)).Directory.FullName, "/"));
				this.SelectFile(newPath);
				return;
			}
			if (!(new DirectoryInfo(string.Concat((new FileInfo(newPath)).Directory.FullName, "/"))).Exists)
			{
				this.OpenDir(string.Concat(Rust.Application.dataPath, "/../"));
				return;
			}
			this.OpenDir(string.Concat((new FileInfo(newPath)).Directory.FullName, "/"));
			this.SelectFile(newPath);
		}

		public void GoUp()
		{
			this.OpenDir(string.Concat(this.workingPath, "/../"));
		}

		private void Hide()
		{
			base.gameObject.SetActive(false);
		}

		public void OnCancelClick()
		{
			this.result = null;
			this.finished = true;
			this.Hide();
		}

		public void OnCommitClick()
		{
			if (this.mode != FileDialog.FileDialogMode.Open)
			{
				this.result = Path.GetFullPath(string.Concat(this.workingPath, "/", this.workingFile));
			}
			else
			{
				this.result = Path.GetFullPath(this.workingFile);
			}
			this.finished = true;
			if (this.saveLastPath)
			{
				PlayerPrefs.SetString("OxOD.lastPath", this.workingPath);
			}
			this.Hide();
		}

		public void OnTypedEnd(string newName)
		{
			if (string.IsNullOrEmpty(newName))
			{
				return;
			}
			if (this.mode == FileDialog.FileDialogMode.Save)
			{
				if (this.allowedExtensions == null)
				{
					this.workingFile = newName;
				}
				else if (!this.allowedExtensions.Contains<string>((new FileInfo(newName)).Extension.ToLower()))
				{
					this.workingFile = string.Concat(newName, this.allowedExtensions[0]);
				}
				else
				{
					this.workingFile = newName;
				}
			}
			this.UpdateFileInfo();
		}

		public void OnTypedFilename(string newName)
		{
			if (this.mode != FileDialog.FileDialogMode.Open)
			{
				this.workingFile = newName;
			}
			else
			{
				this.workingFile = string.Concat(this.workingPath, "/", newName);
			}
			this.UpdateFileInfo();
		}

		public IEnumerator Open(string path = null, string allowedExtensions = null, string windowName = "OPEN FILE", Sprite windowIcon = null, long maxSize = -1L, bool saveLastPath = true)
		{
			FileDialog fileDialog = null;
			string str;
			fileDialog.mode = FileDialog.FileDialogMode.Open;
			fileDialog.commit.GetComponentInChildren<Text>().text = "OPEN";
			fileDialog.fileName.text = "";
			fileDialog.workingPath = "";
			fileDialog.workingFile = "";
			fileDialog.result = null;
			fileDialog.finished = false;
			fileDialog.maxSize = maxSize;
			fileDialog.saveLastPath = saveLastPath;
			if (!string.IsNullOrEmpty(allowedExtensions))
			{
				allowedExtensions = allowedExtensions.ToLower();
				string str1 = allowedExtensions;
				char[] chrArray = new char[] { '|' };
				fileDialog.allowedExtensions = str1.Split(chrArray);
			}
			if (string.IsNullOrEmpty(path))
			{
				if (saveLastPath)
				{
					str = (string.IsNullOrEmpty(PlayerPrefs.GetString("OxOD.lastPath", null)) ? string.Concat(Rust.Application.dataPath, "/../") : PlayerPrefs.GetString("OxOD.lastPath", null));
				}
				else
				{
					str = string.Concat(Rust.Application.dataPath, "/../");
				}
				path = str;
			}
			fileDialog.windowName.text = windowName;
			if (windowIcon)
			{
				fileDialog.windowIcon.sprite = windowIcon;
			}
			fileDialog.GoTo(path);
			fileDialog.gameObject.SetActive(true);
			while (!fileDialog.finished)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}

		public void OpenDir(string path)
		{
			this.ClearSelection();
			this.workingPath = Path.GetFullPath(path);
			this.UpdateElements();
			this.UpdateDrivesList();
			this.UpdateFilesList();
		}

		public IEnumerator Save(string path = null, string allowedExtensions = null, string windowName = "SAVE FILE", Sprite windowIcon = null, bool saveLastPath = true)
		{
			FileDialog fileDialog = null;
			string str;
			fileDialog.mode = FileDialog.FileDialogMode.Save;
			fileDialog.commit.GetComponentInChildren<Text>().text = "SAVE";
			fileDialog.fileName.text = "";
			fileDialog.workingPath = "";
			fileDialog.workingFile = "";
			fileDialog.result = null;
			fileDialog.finished = false;
			fileDialog.maxSize = (long)-1;
			fileDialog.saveLastPath = saveLastPath;
			if (string.IsNullOrEmpty(allowedExtensions))
			{
				fileDialog.allowedExtensions = null;
			}
			else
			{
				allowedExtensions = allowedExtensions.ToLower();
				string str1 = allowedExtensions;
				char[] chrArray = new char[] { '|' };
				fileDialog.allowedExtensions = str1.Split(chrArray);
			}
			if (string.IsNullOrEmpty(path))
			{
				if (saveLastPath)
				{
					str = (string.IsNullOrEmpty(PlayerPrefs.GetString("OxOD.lastPath", null)) ? string.Concat(Rust.Application.dataPath, "/../") : PlayerPrefs.GetString("OxOD.lastPath", null));
				}
				else
				{
					str = string.Concat(Rust.Application.dataPath, "/../");
				}
				path = str;
			}
			fileDialog.windowName.text = windowName;
			if (windowIcon)
			{
				fileDialog.windowIcon.sprite = windowIcon;
			}
			fileDialog.GoTo(path);
			fileDialog.gameObject.SetActive(true);
			while (!fileDialog.finished)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}

		public async Task<string> SaveAsync(string path = null, string allowedExtensions = null, string windowName = "SAVE FILE", Sprite windowIcon = null, bool saveLastPath = true)
		{
			string str;
			this.mode = FileDialog.FileDialogMode.Save;
			this.commit.GetComponentInChildren<Text>().text = "SAVE";
			this.fileName.text = "";
			this.workingPath = "";
			this.workingFile = "";
			this.result = null;
			this.finished = false;
			this.maxSize = (long)-1;
			this.saveLastPath = saveLastPath;
			if (string.IsNullOrEmpty(allowedExtensions))
			{
				this.allowedExtensions = null;
			}
			else
			{
				allowedExtensions = allowedExtensions.ToLower();
				string str1 = allowedExtensions;
				char[] chrArray = new char[] { '|' };
				this.allowedExtensions = str1.Split(chrArray);
			}
			if (string.IsNullOrEmpty(path))
			{
				if (saveLastPath)
				{
					str = (string.IsNullOrEmpty(PlayerPrefs.GetString("OxOD.lastPath", null)) ? string.Concat(Rust.Application.dataPath, "/../") : PlayerPrefs.GetString("OxOD.lastPath", null));
				}
				else
				{
					str = string.Concat(Rust.Application.dataPath, "/../");
				}
				path = str;
			}
			this.windowName.text = windowName;
			if (windowIcon)
			{
				this.windowIcon.sprite = windowIcon;
			}
			this.GoTo(path);
			base.gameObject.SetActive(true);
			while (!this.finished)
			{
				await Task.Delay(100);
			}
			return this.result;
		}

		public void SelectFile(string file)
		{
			if (this.mode != FileDialog.FileDialogMode.Open)
			{
				this.workingFile = (new FileInfo(Path.GetFullPath(file))).Name;
			}
			else
			{
				this.workingFile = Path.GetFullPath(file);
			}
			this.UpdateFileInfo();
		}

		private void UpdateDrivesList()
		{
			GameObject gameObject = this.drivesScrollRectContent;
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				UnityEngine.Object.Destroy(gameObject.transform.GetChild(i).gameObject);
			}
			string[] logicalDrives = Directory.GetLogicalDrives();
			for (int j = 0; j < (int)logicalDrives.Length; j++)
			{
				GameObject vector3 = UnityEngine.Object.Instantiate<GameObject>(this.drivesScrollRectElement, Vector3.zero, Quaternion.identity);
				vector3.transform.SetParent(gameObject.transform, true);
				vector3.transform.localScale = new Vector3(1f, 1f, 1f);
				FileListElement component = vector3.GetComponent<FileListElement>();
				component.instance = this;
				component.data = logicalDrives[j];
				component.elementName.text = logicalDrives[j];
				component.isFile = false;
			}
		}

		private void UpdateElements()
		{
			this.currentPath.text = this.workingPath;
		}

		public void UpdateFileInfo()
		{
			if (this.mode != FileDialog.FileDialogMode.Open)
			{
				if (this.workingFile.Length > 0)
				{
					this.fileName.text = (new FileInfo(this.workingFile)).Name;
				}
				this.commit.interactable = (this.workingFile.Length > 0 ? true : false);
			}
			else
			{
				try
				{
					this.fileName.text = (new FileInfo(this.workingFile)).Name;
					this.commit.interactable = File.Exists(this.workingFile);
				}
				catch (Exception exception)
				{
					this.fileName.text = "";
					this.commit.interactable = false;
				}
			}
		}

		private void UpdateFilesList()
		{
			GameObject gameObject = this.filesScrollRectContent;
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				UnityEngine.Object.Destroy(gameObject.transform.GetChild(i).gameObject);
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(this.workingPath);
			try
			{
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				for (int j = 0; j < (int)directories.Length; j++)
				{
					if (directories[j].Name[0] != '@' && directories[j].Name[0] != '.' && (directories[j].Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
					{
						GameObject vector3 = UnityEngine.Object.Instantiate<GameObject>(this.filesScrollRectElement, Vector3.zero, Quaternion.identity);
						vector3.transform.SetParent(gameObject.transform, true);
						vector3.transform.localScale = new Vector3(1f, 1f, 1f);
						FileListElement component = vector3.GetComponent<FileListElement>();
						component.instance = this;
						component.data = string.Concat(directories[j].FullName, "/");
						component.elementName.text = directories[j].Name;
						component.size.text = "";
						component.icon.sprite = this.folderIcon;
						component.isFile = false;
					}
				}
				if (this.allowedExtensions == null)
				{
					FileInfo[] files = directoryInfo.GetFiles();
					for (int k = 0; k < (int)files.Length; k++)
					{
						if (this.maxSize <= (long)0)
						{
							GameObject vector31 = UnityEngine.Object.Instantiate<GameObject>(this.filesScrollRectElement, Vector3.zero, Quaternion.identity);
							vector31.transform.SetParent(gameObject.transform, true);
							vector31.transform.localScale = new Vector3(1f, 1f, 1f);
							FileListElement fullName = vector31.GetComponent<FileListElement>();
							fullName.instance = this;
							fullName.data = files[k].FullName;
							fullName.size.text = this.GetFileSizeText(files[k].Length);
							fullName.elementName.text = files[k].Name;
							fullName.icon.sprite = this.fileIcon;
							fullName.isFile = true;
						}
						else if (files[k].Length < this.maxSize)
						{
							GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.filesScrollRectElement, Vector3.zero, Quaternion.identity);
							gameObject1.transform.SetParent(gameObject.transform, true);
							gameObject1.transform.localScale = new Vector3(1f, 1f, 1f);
							FileListElement fileSizeText = gameObject1.GetComponent<FileListElement>();
							fileSizeText.instance = this;
							fileSizeText.data = files[k].FullName;
							fileSizeText.size.text = this.GetFileSizeText(files[k].Length);
							fileSizeText.elementName.text = files[k].Name;
							fileSizeText.icon.sprite = this.fileIcon;
							fileSizeText.isFile = true;
						}
					}
				}
				else
				{
					FileInfo[] array = (
						from f in directoryInfo.GetFiles()
						where this.allowedExtensions.Contains<string>(f.Extension.ToLower())
						select f).ToArray<FileInfo>();
					for (int l = 0; l < (int)array.Length; l++)
					{
						if (this.maxSize <= (long)0)
						{
							GameObject vector32 = UnityEngine.Object.Instantiate<GameObject>(this.filesScrollRectElement, Vector3.zero, Quaternion.identity);
							vector32.transform.SetParent(gameObject.transform, true);
							vector32.transform.localScale = new Vector3(1f, 1f, 1f);
							FileListElement name = vector32.GetComponent<FileListElement>();
							name.instance = this;
							name.data = array[l].FullName;
							name.size.text = this.GetFileSizeText(array[l].Length);
							name.elementName.text = array[l].Name;
							name.icon.sprite = this.fileIcon;
							name.isFile = true;
						}
						else if (array[l].Length < this.maxSize)
						{
							GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.filesScrollRectElement, Vector3.zero, Quaternion.identity);
							gameObject2.transform.SetParent(gameObject.transform, true);
							gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
							FileListElement fileListElement = gameObject2.GetComponent<FileListElement>();
							fileListElement.instance = this;
							fileListElement.data = array[l].FullName;
							fileListElement.size.text = this.GetFileSizeText(array[l].Length);
							fileListElement.elementName.text = array[l].Name;
							fileListElement.icon.sprite = this.fileIcon;
							fileListElement.isFile = true;
						}
					}
				}
			}
			catch (Exception exception)
			{
			}
		}

		public enum FileDialogMode
		{
			Open,
			Save
		}
	}
}