using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rust
{
	public class AsyncTextureLoad : CustomYieldInstruction
	{
		private IntPtr buffer = IntPtr.Zero;

		private int size;

		private int width;

		private int height;

		private int format;

		public string filename;

		public bool normal;

		public bool dither;

		public bool hqmode;

		public bool cache;

		private Action worker;

		public bool isDone
		{
			get
			{
				return this.worker == null;
			}
		}

		public bool isValid
		{
			get
			{
				if (this.buffer == IntPtr.Zero)
				{
					return false;
				}
				if (this.size == 0)
				{
					return false;
				}
				if (this.format == 0)
				{
					return false;
				}
				if (this.width < 32 || this.width > 8192 || !Mathf.IsPowerOfTwo(this.width))
				{
					return false;
				}
				if (this.height < 32 || this.height > 8192 || !Mathf.IsPowerOfTwo(this.height))
				{
					return false;
				}
				if (this.format != 12 && this.format != 10)
				{
					return false;
				}
				return true;
			}
		}

		public override bool keepWaiting
		{
			get
			{
				return this.worker != null;
			}
		}

		public Texture2D texture
		{
			get
			{
				if (!this.isValid)
				{
					return null;
				}
				TimeWarning.BeginSample("Texture2D.New");
				Texture2D texture2D = new Texture2D(this.width, this.height, (TextureFormat)this.format, true, this.normal);
				TimeWarning.EndSample();
				TimeWarning.BeginSample("Texture2D.LoadRawTextureData");
				texture2D.LoadRawTextureData(this.buffer, this.size);
				TimeWarning.EndSample();
				TimeWarning.BeginSample("Texture2D.Apply");
				texture2D.Apply(false);
				TimeWarning.EndSample();
				TimeWarning.BeginSample("Native.FreeTexture");
				AsyncTextureLoad.FreeTexture(ref this.buffer);
				TimeWarning.EndSample();
				return texture2D;
			}
		}

		public AsyncTextureLoad(string filename, bool normal, bool dither, bool hqmode, bool cache)
		{
			this.filename = filename;
			this.normal = normal;
			this.dither = dither;
			this.hqmode = hqmode;
			this.cache = cache;
			this.Invoke();
		}

		private void Callback(IAsyncResult result)
		{
			this.worker.EndInvoke(result);
			this.worker = null;
		}

		private void DoWork()
		{
			if (this.cache)
			{
				AsyncTextureLoad.LoadTextureFromCache(this.filename, ref this.buffer, ref this.size, ref this.width, ref this.height, ref this.format);
				return;
			}
			int num = 0;
			AsyncTextureLoad.LoadTextureFromFile(this.filename, ref this.buffer, ref this.size, ref this.width, ref this.height, ref num, this.normal, this.dither, this.hqmode);
			this.format = (num > 3 ? 12 : 10);
		}

		[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="free_texture", ExactSpelling=false)]
		private static extern void FreeTexture(ref IntPtr buffer);

		private void Invoke()
		{
			this.worker = new Action(this.DoWork);
			this.worker.BeginInvoke(new AsyncCallback(this.Callback), null);
		}

		public void LoadIntoTexture(Texture2D tex)
		{
			if (!this.isValid)
			{
				return;
			}
			if (tex.width != this.width || tex.height != this.height || (int)tex.format != this.format)
			{
				return;
			}
			TimeWarning.BeginSample("Texture2D.LoadRawTextureData");
			tex.LoadRawTextureData(this.buffer, this.size);
			TimeWarning.EndSample();
			TimeWarning.BeginSample("Texture2D.Apply");
			tex.Apply(false);
			TimeWarning.EndSample();
			TimeWarning.BeginSample("Native.FreeTexture");
			AsyncTextureLoad.FreeTexture(ref this.buffer);
			TimeWarning.EndSample();
		}

		[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="load_texture_from_cache", ExactSpelling=false)]
		private static extern void LoadTextureFromCache(string filename, ref IntPtr buffer, ref int size, ref int width, ref int height, ref int format);

		[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="load_texture_from_file", ExactSpelling=false)]
		private static extern void LoadTextureFromFile(string filename, ref IntPtr buffer, ref int size, ref int width, ref int height, ref int channels, bool normal, bool dither, bool hqmode);

		[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="save_texture_to_cache", ExactSpelling=false)]
		private static extern void SaveTextureToCache(string filename, IntPtr buffer, int size, int width, int height, int format);

		public void WriteToCache(string cachename)
		{
			AsyncTextureLoad.SaveTextureToCache(cachename, this.buffer, this.size, this.width, this.height, this.format);
		}
	}
}