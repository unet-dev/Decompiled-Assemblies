using System;
using System.Collections.Generic;
using UnityEngine;

public interface ILerpTarget
{
	void DrawInterpolationState(TransformInterpolator.Segment segment, List<TransformInterpolator.Entry> entries);

	float GetExtrapolationTime();

	float GetInterpolationDelay();

	float GetInterpolationSmoothing();

	Vector3 GetNetworkPosition();

	Quaternion GetNetworkRotation();

	void SetNetworkPosition(Vector3 pos);

	void SetNetworkRotation(Quaternion rot);
}