using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AssetBundleBackend : FileSystemBackend, IDisposable
{
	private AssetBundle rootBundle;

	private AssetBundleManifest manifest;

	public static Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>(StringComparer.OrdinalIgnoreCase);

	public static Dictionary<string, AssetBundle> files = new Dictionary<string, AssetBundle>(StringComparer.OrdinalIgnoreCase);

	private string assetPath;

	public AssetBundleBackend(string assetRoot)
	{
		this.isError = false;
		string directoryName = Path.GetDirectoryName(assetRoot);
		char directorySeparatorChar = Path.DirectorySeparatorChar;
		this.assetPath = string.Concat(directoryName, directorySeparatorChar.ToString());
		this.rootBundle = AssetBundle.LoadFromFile(assetRoot);
		if (this.rootBundle == null)
		{
			base.LoadError(string.Concat("Couldn't load root AssetBundle - ", assetRoot));
			return;
		}
		AssetBundleManifest[] assetBundleManifestArray = this.rootBundle.LoadAllAssets<AssetBundleManifest>();
		if ((int)assetBundleManifestArray.Length != 1)
		{
			base.LoadError(string.Concat("Couldn't find AssetBundleManifest - ", (int)assetBundleManifestArray.Length));
			return;
		}
		this.manifest = assetBundleManifestArray[0];
		string[] allAssetBundles = this.manifest.GetAllAssetBundles();
		for (int i = 0; i < (int)allAssetBundles.Length; i++)
		{
			this.LoadBundle(allAssetBundles[i]);
			if (this.isError)
			{
				return;
			}
		}
		this.BuildFileIndex();
	}

	private void BuildFileIndex()
	{
		this.files.Clear();
		foreach (KeyValuePair<string, AssetBundle> bundle in this.bundles)
		{
			if (bundle.Key.StartsWith("content", StringComparison.InvariantCultureIgnoreCase))
			{
				continue;
			}
			string[] allAssetNames = bundle.Value.GetAllAssetNames();
			for (int i = 0; i < (int)allAssetNames.Length; i++)
			{
				string str = allAssetNames[i];
				this.files.Add(str, bundle.Value);
			}
		}
	}

	public void Dispose()
	{
		this.manifest = null;
		foreach (KeyValuePair<string, AssetBundle> bundle in this.bundles)
		{
			bundle.Value.Unload(false);
			UnityEngine.Object.DestroyImmediate(bundle.Value);
		}
		this.bundles.Clear();
		if (this.rootBundle)
		{
			this.rootBundle.Unload(false);
			UnityEngine.Object.DestroyImmediate(this.rootBundle);
			this.rootBundle = null;
		}
	}

	protected override T LoadAsset<T>(string filePath)
	where T : UnityEngine.Object
	{
		AssetBundle assetBundle = null;
		if (!this.files.TryGetValue(filePath, out assetBundle))
		{
			return default(T);
		}
		return assetBundle.LoadAsset<T>(filePath);
	}

	protected override string[] LoadAssetList(string folder, string search)
	{
		Func<KeyValuePair<string, AssetBundle>, bool> func = null;
		List<string> strs = new List<string>();
		Dictionary<string, AssetBundle> strs1 = this.files;
		Func<KeyValuePair<string, AssetBundle>, bool> func1 = func;
		if (func1 == null)
		{
			Func<KeyValuePair<string, AssetBundle>, bool> func2 = (KeyValuePair<string, AssetBundle> x) => x.Key.StartsWith(folder, StringComparison.InvariantCultureIgnoreCase);
			Func<KeyValuePair<string, AssetBundle>, bool> func3 = func2;
			func = func2;
			func1 = func3;
		}
		foreach (KeyValuePair<string, AssetBundle> keyValuePair in strs1.Where<KeyValuePair<string, AssetBundle>>(func1))
		{
			if (!string.IsNullOrEmpty(search) && keyValuePair.Key.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) == -1)
			{
				continue;
			}
			strs.Add(keyValuePair.Key);
		}
		return strs.ToArray();
	}

	private void LoadBundle(string bundleName)
	{
		if (this.bundles.ContainsKey(bundleName))
		{
			return;
		}
		string str = string.Concat(this.assetPath, bundleName);
		if (!File.Exists(str))
		{
			return;
		}
		AssetBundle assetBundle = AssetBundle.LoadFromFile(str);
		if (assetBundle == null)
		{
			base.LoadError(string.Concat("Couldn't load AssetBundle - ", str));
			return;
		}
		this.bundles.Add(bundleName, assetBundle);
	}
}