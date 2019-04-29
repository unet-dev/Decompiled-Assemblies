using System;
using UnityEngine;

public interface IRFObject
{
	int GetFrequency();

	float GetMaxRange();

	Vector3 GetPosition();

	void RFSignalUpdate(bool on);
}