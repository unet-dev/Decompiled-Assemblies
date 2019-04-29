using Newtonsoft.Json;
using Oxide.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Oxide.Core.Configuration
{
	public class DynamicConfigFile : ConfigFile, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		private Dictionary<string, object> _keyvalues;

		private readonly JsonSerializerSettings _settings;

		private readonly string _chroot;

		public object this[string key]
		{
			get
			{
				object obj;
				if (!this._keyvalues.TryGetValue(key, out obj))
				{
					return null;
				}
				return obj;
			}
			set
			{
				this._keyvalues[key] = value;
			}
		}

		public object this[string keyLevel1, string keyLevel2]
		{
			get
			{
				return this.Get(new string[] { keyLevel1, keyLevel2 });
			}
			set
			{
				this.Set(new object[] { keyLevel1, keyLevel2, value });
			}
		}

		public object this[string keyLevel1, string keyLevel2, string keyLevel3]
		{
			get
			{
				return this.Get(new string[] { keyLevel1, keyLevel2, keyLevel3 });
			}
			set
			{
				this.Set(new object[] { keyLevel1, keyLevel2, keyLevel3, value });
			}
		}

		public JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings();

		public DynamicConfigFile(string filename) : base(filename)
		{
			this._keyvalues = new Dictionary<string, object>();
			this._settings = new JsonSerializerSettings();
			this._settings.Converters.Add(new KeyValuesConverter());
			this._chroot = Interface.Oxide.InstanceDirectory;
		}

		private string CheckPath(string filename)
		{
			filename = DynamicConfigFile.SanitizeName(filename);
			string fullPath = Path.GetFullPath(filename);
			if (!fullPath.StartsWith(this._chroot, StringComparison.Ordinal))
			{
				throw new Exception(string.Concat("Only access to oxide directory!\nPath: ", fullPath));
			}
			return fullPath;
		}

		public void Clear()
		{
			this._keyvalues.Clear();
		}

		public object ConvertValue(object value, Type destinationType)
		{
			if (!destinationType.IsGenericType)
			{
				return Convert.ChangeType(value, destinationType);
			}
			if (destinationType.GetGenericTypeDefinition() == typeof(List<>))
			{
				Type genericArguments = destinationType.GetGenericArguments()[0];
				IList lists = (IList)Activator.CreateInstance(destinationType);
				foreach (object obj in (IList)value)
				{
					lists.Add(Convert.ChangeType(obj, genericArguments));
				}
				return lists;
			}
			if (destinationType.GetGenericTypeDefinition() != typeof(Dictionary<,>))
			{
				throw new InvalidCastException("Generic types other than List<> and Dictionary<,> are not supported");
			}
			Type type = destinationType.GetGenericArguments()[0];
			Type genericArguments1 = destinationType.GetGenericArguments()[1];
			IDictionary dictionaries = (IDictionary)Activator.CreateInstance(destinationType);
			foreach (object key in ((IDictionary)value).Keys)
			{
				dictionaries.Add(Convert.ChangeType(key, type), Convert.ChangeType(((IDictionary)value)[key], genericArguments1));
			}
			return dictionaries;
		}

		public T ConvertValue<T>(object value)
		{
			return (T)this.ConvertValue(value, typeof(T));
		}

		public bool Exists(string filename = null)
		{
			filename = this.CheckPath(filename ?? base.Filename);
			string directoryName = Utility.GetDirectoryName(filename);
			if (directoryName != null && !Directory.Exists(directoryName))
			{
				return false;
			}
			return File.Exists(filename);
		}

		public object Get(params string[] path)
		{
			object obj;
			if ((int)path.Length < 1)
			{
				throw new ArgumentException("path must not be empty");
			}
			if (!this._keyvalues.TryGetValue(path[0], out obj))
			{
				return null;
			}
			for (int i = 1; i < (int)path.Length; i++)
			{
				Dictionary<string, object> strs = obj as Dictionary<string, object>;
				if (strs == null || !strs.TryGetValue(path[i], out obj))
				{
					return null;
				}
			}
			return obj;
		}

		public T Get<T>(params string[] path)
		{
			return this.ConvertValue<T>(this.Get(path));
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return this._keyvalues.GetEnumerator();
		}

		public override void Load(string filename = null)
		{
			filename = this.CheckPath(filename ?? base.Filename);
			string str = File.ReadAllText(filename);
			this._keyvalues = JsonConvert.DeserializeObject<Dictionary<string, object>>(str, this._settings);
		}

		public T ReadObject<T>(string filename = null)
		{
			T t;
			filename = this.CheckPath(filename ?? base.Filename);
			if (!this.Exists(filename))
			{
				t = Activator.CreateInstance<T>();
				this.WriteObject<T>(t, false, filename);
			}
			else
			{
				t = JsonConvert.DeserializeObject<T>(File.ReadAllText(filename), this.Settings);
			}
			return t;
		}

		public void Remove(string key)
		{
			this._keyvalues.Remove(key);
		}

		[Obsolete("SanitiseName is deprecated, use SanitizeName instead")]
		public static string SanitiseName(string name)
		{
			return DynamicConfigFile.SanitizeName(name);
		}

		public static string SanitizeName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return string.Empty;
			}
			name = name.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
			name = Regex.Replace(name, string.Concat("[", Regex.Escape(new string(Path.GetInvalidPathChars())), "]"), "_");
			name = Regex.Replace(name, "\\.+", ".");
			return name.TrimStart(new char[] { '.' });
		}

		public override void Save(string filename = null)
		{
			filename = this.CheckPath(filename ?? base.Filename);
			string directoryName = Utility.GetDirectoryName(filename);
			if (directoryName != null && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllText(filename, JsonConvert.SerializeObject(this._keyvalues, Formatting.Indented, this._settings));
		}

		public void Set(params object[] pathAndTrailingValue)
		{
			object obj;
			if ((int)pathAndTrailingValue.Length < 2)
			{
				throw new ArgumentException("path must not be empty");
			}
			string[] strArrays = new string[(int)pathAndTrailingValue.Length - 1];
			for (int i = 0; i < (int)pathAndTrailingValue.Length - 1; i++)
			{
				strArrays[i] = (string)pathAndTrailingValue[i];
			}
			object obj1 = pathAndTrailingValue[(int)pathAndTrailingValue.Length - 1];
			if ((int)strArrays.Length == 1)
			{
				this._keyvalues[strArrays[0]] = obj1;
				return;
			}
			if (!this._keyvalues.TryGetValue(strArrays[0], out obj))
			{
				Dictionary<string, object> strs = this._keyvalues;
				string str = strArrays[0];
				Dictionary<string, object> strs1 = new Dictionary<string, object>();
				obj = strs1;
				strs[str] = strs1;
			}
			for (int j = 1; j < (int)strArrays.Length - 1; j++)
			{
				if (!(obj is Dictionary<string, object>))
				{
					throw new ArgumentException("path is not a dictionary");
				}
				Dictionary<string, object> strs2 = (Dictionary<string, object>)obj;
				if (!strs2.TryGetValue(strArrays[j], out obj))
				{
					string str1 = strArrays[j];
					Dictionary<string, object> strs3 = new Dictionary<string, object>();
					obj = strs3;
					strs2[str1] = strs3;
				}
			}
			((Dictionary<string, object>)obj)[strArrays[(int)strArrays.Length - 1]] = obj1;
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._keyvalues.GetEnumerator();
		}

		public void WriteObject<T>(T config, bool sync = false, string filename = null)
		{
			filename = this.CheckPath(filename ?? base.Filename);
			string directoryName = Utility.GetDirectoryName(filename);
			if (directoryName != null && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			string str = JsonConvert.SerializeObject(config, Formatting.Indented, this.Settings);
			File.WriteAllText(filename, str);
			if (sync)
			{
				this._keyvalues = JsonConvert.DeserializeObject<Dictionary<string, object>>(str, this._settings);
			}
		}
	}
}