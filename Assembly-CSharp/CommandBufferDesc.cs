using System;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;

public class CommandBufferDesc
{
	public UnityEngine.Rendering.CameraEvent CameraEvent
	{
		get;
		private set;
	}

	public Action<CommandBuffer> FillDelegate
	{
		get;
		private set;
	}

	public int OrderId
	{
		get;
		private set;
	}

	public CommandBufferDesc(UnityEngine.Rendering.CameraEvent cameraEvent, int orderId, CommandBufferDesc.FillCommandBuffer fill)
	{
		this.CameraEvent = cameraEvent;
		this.OrderId = orderId;
		this.FillDelegate = new Action<CommandBuffer>(fill.Invoke);
	}

	public delegate void FillCommandBuffer(CommandBuffer cb);
}