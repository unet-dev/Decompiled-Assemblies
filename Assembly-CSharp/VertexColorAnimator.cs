using System;
using System.Collections.Generic;
using UnityEngine;

public class VertexColorAnimator : MonoBehaviour
{
	public List<MeshHolder> animationMeshes;

	public List<float> animationKeyframes;

	public float timeScale = 2f;

	public int mode;

	private float elapsedTime;

	public VertexColorAnimator()
	{
	}

	public void addMesh(Mesh mesh, float atPosition)
	{
		MeshHolder meshHolder = new MeshHolder();
		meshHolder.setAnimationData(mesh);
		this.animationMeshes.Add(meshHolder);
		this.animationKeyframes.Add(atPosition);
	}

	public void deleteKeyframe(int frameIndex)
	{
		this.animationMeshes.RemoveAt(frameIndex);
		this.animationKeyframes.RemoveAt(frameIndex);
	}

	public void initLists()
	{
		this.animationMeshes = new List<MeshHolder>();
		this.animationKeyframes = new List<float>();
	}

	public void replaceKeyframe(int frameIndex, Mesh mesh)
	{
		this.animationMeshes[frameIndex].setAnimationData(mesh);
	}

	public void scrobble(float scrobblePos)
	{
		if (this.animationMeshes.Count == 0)
		{
			return;
		}
		Color[] colorArray = new Color[(int)base.GetComponent<MeshFilter>().sharedMesh.colors.Length];
		int num = 0;
		for (int i = 0; i < this.animationKeyframes.Count; i++)
		{
			if (scrobblePos >= this.animationKeyframes[i])
			{
				num = i;
			}
		}
		if (num >= this.animationKeyframes.Count - 1)
		{
			base.GetComponent<VertexColorStream>().setColors(this.animationMeshes[num]._colors);
			return;
		}
		float item = this.animationKeyframes[num + 1] - this.animationKeyframes[num];
		float single = this.animationKeyframes[num];
		float single1 = (scrobblePos - single) / item;
		for (int j = 0; j < (int)colorArray.Length; j++)
		{
			colorArray[j] = Color.Lerp(this.animationMeshes[num]._colors[j], this.animationMeshes[num + 1]._colors[j], single1);
		}
		base.GetComponent<VertexColorStream>().setColors(colorArray);
	}

	private void Start()
	{
		this.elapsedTime = 0f;
	}

	private void Update()
	{
		if (this.mode == 0)
		{
			this.elapsedTime = this.elapsedTime + Time.fixedDeltaTime / this.timeScale;
		}
		else if (this.mode == 1)
		{
			this.elapsedTime = this.elapsedTime + Time.fixedDeltaTime / this.timeScale;
			if (this.elapsedTime > 1f)
			{
				this.elapsedTime = 0f;
			}
		}
		else if (this.mode == 2)
		{
			if (Mathf.FloorToInt(Time.fixedTime / this.timeScale) % 2 != 0)
			{
				this.elapsedTime = this.elapsedTime - Time.fixedDeltaTime / this.timeScale;
			}
			else
			{
				this.elapsedTime = this.elapsedTime + Time.fixedDeltaTime / this.timeScale;
			}
		}
		Color[] item = new Color[(int)base.GetComponent<MeshFilter>().sharedMesh.colors.Length];
		int num = 0;
		for (int i = 0; i < this.animationKeyframes.Count; i++)
		{
			if (this.elapsedTime >= this.animationKeyframes[i])
			{
				num = i;
			}
		}
		if (num >= this.animationKeyframes.Count - 1)
		{
			item = this.animationMeshes[num]._colors;
		}
		else
		{
			float single = this.animationKeyframes[num + 1] - this.animationKeyframes[num];
			float item1 = this.animationKeyframes[num];
			float single1 = (this.elapsedTime - item1) / single;
			for (int j = 0; j < (int)item.Length; j++)
			{
				item[j] = Color.Lerp(this.animationMeshes[num]._colors[j], this.animationMeshes[num + 1]._colors[j], single1);
			}
		}
		base.GetComponent<VertexColorStream>().setColors(item);
	}
}