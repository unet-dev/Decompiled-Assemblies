using Newtonsoft.Json;
using Oxide.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Oxide.Core
{
	public class DataFileSystem
	{
		private readonly Dictionary<string, DynamicConfigFile> _datafiles;

		public string Directory
		{
			get;
			private set;
		}

		public DataFileSystem(string directory)
		{
			this.Directory = directory;
			this._datafiles = new Dictionary<string, DynamicConfigFile>();
			KeyValuesConverter keyValuesConverter = new KeyValuesConverter();
			(new JsonSerializerSettings()).Converters.Add(keyValuesConverter);
		}

		public bool ExistsDatafile(string name)
		{
			return this.GetFile(name).Exists(null);
		}

		public void ForEachObject<T>(string name, Action<T> callback)
		{
			string str = DynamicConfigFile.SanitizeName(name);
			foreach (DynamicConfigFile dynamicConfigFiles in 
				from  in this._datafiles
				where d.Key.StartsWith(str)
				select a.Value)
			{
				if (callback == null)
				{
					continue;
				}
				callback(dynamicConfigFiles.ReadObject<T>(null));
			}
		}

		public DynamicConfigFile GetDatafile(string name)
		{
			DynamicConfigFile file = this.GetFile(name);
			if (!file.Exists(null))
			{
				file.Save(null);
			}
			else
			{
				file.Load(null);
			}
			return file;
		}

		public DynamicConfigFile GetFile(string name)
		{
			DynamicConfigFile dynamicConfigFiles;
			name = DynamicConfigFile.SanitizeName(name);
			if (this._datafiles.TryGetValue(name, out dynamicConfigFiles))
			{
				return dynamicConfigFiles;
			}
			dynamicConfigFiles = new DynamicConfigFile(Path.Combine(this.Directory, string.Concat(name, ".json")));
			this._datafiles.Add(name, dynamicConfigFiles);
			return dynamicConfigFiles;
		}

		public string[] GetFiles(string path = "", string searchPattern = "*")
		{
			return System.IO.Directory.GetFiles(Path.Combine(this.Directory, path), searchPattern);
		}

		public T ReadObject<T>(string name)
		{
			if (this.ExistsDatafile(name))
			{
				return this.GetFile(name).ReadObject<T>(null);
			}
			T t = Activator.CreateInstance<T>();
			this.WriteObject<T>(name, t, false);
			return t;
		}

		public void SaveDatafile(string name)
		{
			this.GetFile(name).Save(null);
		}

		public void WriteObject<T>(string name, T Object, bool sync = false)
		{
			this.GetFile(name).WriteObject<T>(Object, sync, null);
		}
	}
}