using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseCommandBuffer : MonoBehaviour
{
	private Dictionary<Camera, Dictionary<int, CommandBuffer>> cameras = new Dictionary<Camera, Dictionary<int, CommandBuffer>>();

	public BaseCommandBuffer()
	{
	}

	protected void Cleanup()
	{
		foreach (KeyValuePair<Camera, Dictionary<int, CommandBuffer>> camera in this.cameras)
		{
			Camera key = camera.Key;
			Dictionary<int, CommandBuffer> value = camera.Value;
			if (!key)
			{
				continue;
			}
			foreach (KeyValuePair<int, CommandBuffer> keyValuePair in value)
			{
				key.RemoveCommandBuffer((CameraEvent)keyValuePair.Key, keyValuePair.Value);
			}
		}
	}

	protected void CleanupCamera(string name, Camera camera, CameraEvent cameraEvent)
	{
		CommandBuffer[] commandBuffers = camera.GetCommandBuffers(cameraEvent);
		for (int i = 0; i < (int)commandBuffers.Length; i++)
		{
			CommandBuffer commandBuffer = commandBuffers[i];
			if (commandBuffer.name == name)
			{
				camera.RemoveCommandBuffer(cameraEvent, commandBuffer);
			}
		}
	}

	protected void CleanupCommandBuffer(Camera camera, CameraEvent cameraEvent)
	{
		Dictionary<int, CommandBuffer> nums;
		CommandBuffer commandBuffer;
		if (!this.cameras.TryGetValue(camera, out nums))
		{
			return;
		}
		if (!nums.TryGetValue(cameraEvent, out commandBuffer))
		{
			return;
		}
		camera.RemoveCommandBuffer(cameraEvent, commandBuffer);
	}

	protected CommandBuffer GetCommandBuffer(string name, Camera camera, CameraEvent cameraEvent)
	{
		Dictionary<int, CommandBuffer> nums;
		CommandBuffer commandBuffer;
		if (!this.cameras.TryGetValue(camera, out nums))
		{
			nums = new Dictionary<int, CommandBuffer>();
			this.cameras.Add(camera, nums);
		}
		if (!nums.TryGetValue(cameraEvent, out commandBuffer))
		{
			commandBuffer = new CommandBuffer()
			{
				name = name
			};
			nums.Add(cameraEvent, commandBuffer);
			this.CleanupCamera(name, camera, cameraEvent);
			camera.AddCommandBuffer(cameraEvent, commandBuffer);
		}
		else
		{
			commandBuffer.Clear();
		}
		return commandBuffer;
	}
}