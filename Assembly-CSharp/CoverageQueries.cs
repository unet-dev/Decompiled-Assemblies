using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class CoverageQueries : MonoBehaviour
{
	public float depthBias = -0.1f;

	public bool debug;

	public CoverageQueries()
	{
	}

	public class BufferSet
	{
		public int width;

		public int height;

		public Texture2D inputTexture;

		public RenderTexture resultTexture;

		public Color[] inputData;

		public Color32[] resultData;

		private Material coverageMat;

		private const int MaxAsyncGPUReadbackRequests = 10;

		private Queue<AsyncGPUReadbackRequest> asyncRequests;

		public BufferSet()
		{
		}

		public void Attach(Material coverageMat)
		{
			this.coverageMat = coverageMat;
		}

		public bool CheckResize(int count)
		{
			if (count <= (int)this.inputData.Length && (!(this.resultTexture != null) || this.resultTexture.IsCreated()))
			{
				return false;
			}
			this.Dispose(false);
			this.width = Mathf.CeilToInt(Mathf.Sqrt((float)count));
			this.height = Mathf.CeilToInt((float)count / (float)this.width);
			this.inputTexture = new Texture2D(this.width, this.height, TextureFormat.RGBAFloat, false, true)
			{
				name = "_Input",
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp
			};
			this.resultTexture = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
			{
				name = "_Result",
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp,
				useMipMap = false
			};
			this.resultTexture.Create();
			int length = (int)this.resultData.Length;
			int num = this.width * this.height;
			Array.Resize<Color>(ref this.inputData, num);
			Array.Resize<Color32>(ref this.resultData, num);
			Color32 color32 = new Color32(255, 0, 0, 0);
			for (int i = length; i < num; i++)
			{
				this.resultData[i] = color32;
			}
			return true;
		}

		public void Dispatch(int count)
		{
			if (this.inputData.Length != 0)
			{
				RenderBuffer renderBuffer = Graphics.activeColorBuffer;
				RenderBuffer renderBuffer1 = Graphics.activeDepthBuffer;
				this.coverageMat.SetTexture("_Input", this.inputTexture);
				Graphics.Blit(this.inputTexture, this.resultTexture, this.coverageMat, 0);
				Graphics.SetRenderTarget(renderBuffer, renderBuffer1);
			}
		}

		public void Dispose(bool data = true)
		{
			if (this.inputTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.inputTexture);
				this.inputTexture = null;
			}
			if (this.resultTexture != null)
			{
				RenderTexture.active = null;
				this.resultTexture.Release();
				UnityEngine.Object.DestroyImmediate(this.resultTexture);
				this.resultTexture = null;
			}
			if (data)
			{
				this.inputData = new Color[0];
				this.resultData = new Color32[0];
			}
		}

		public void GetResults()
		{
			if (this.resultData.Length != 0)
			{
				while (this.asyncRequests.Count > 0)
				{
					AsyncGPUReadbackRequest asyncGPUReadbackRequest = this.asyncRequests.Peek();
					if (!asyncGPUReadbackRequest.hasError)
					{
						if (!asyncGPUReadbackRequest.done)
						{
							break;
						}
						NativeArray<Color32> data = asyncGPUReadbackRequest.GetData<Color32>(0);
						for (int i = 0; i < data.Length; i++)
						{
							this.resultData[i] = data[i];
						}
						this.asyncRequests.Dequeue();
					}
					else
					{
						this.asyncRequests.Dequeue();
					}
				}
			}
		}

		public void IssueRead()
		{
			if (this.asyncRequests.Count < 10)
			{
				this.asyncRequests.Enqueue(AsyncGPUReadback.Request(this.resultTexture, 0, null));
			}
		}

		public void UploadData()
		{
			if (this.inputData.Length != 0)
			{
				this.inputTexture.SetPixels(this.inputData);
				this.inputTexture.Apply();
			}
		}
	}

	public class Query
	{
		public CoverageQueries.Query.Input input;

		public CoverageQueries.Query.Internal intern;

		public CoverageQueries.Query.Result result;

		public bool IsRegistered
		{
			get
			{
				return this.intern.id >= 0;
			}
		}

		public Query()
		{
		}

		public struct Input
		{
			public Vector3 position;

			public CoverageQueries.RadiusSpace radiusSpace;

			public float radius;

			public int sampleCount;

			public float smoothingSpeed;
		}

		public struct Internal
		{
			public int id;

			public void Reset()
			{
				this.id = -1;
			}
		}

		public struct Result
		{
			public int passed;

			public float coverage;

			public float smoothCoverage;

			public float weightedCoverage;

			public float weightedSmoothCoverage;

			public bool originOccluded;

			public int frame;

			public float originVisibility;

			public float originSmoothVisibility;

			public void Reset()
			{
				this.passed = 0;
				this.coverage = 0f;
				this.smoothCoverage = 0f;
				this.weightedCoverage = 0f;
				this.weightedSmoothCoverage = 0f;
				this.originOccluded = true;
				this.frame = -1;
				this.originVisibility = 0f;
				this.originSmoothVisibility = 0f;
			}
		}
	}

	public enum RadiusSpace
	{
		ScreenNormalized,
		World
	}
}