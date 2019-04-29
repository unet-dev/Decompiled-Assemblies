using System;
using UnityEngine;

public class Socket_Specific_Female : Socket_Base
{
	public int rotationDegrees;

	public int rotationOffset;

	public string[] allowedMaleSockets;

	public Socket_Specific_Female()
	{
	}

	public bool CanAccept(Socket_Specific socket)
	{
		string[] strArrays = this.allowedMaleSockets;
		for (int i = 0; i < (int)strArrays.Length; i++)
		{
			string str = strArrays[i];
			if (socket.targetSocketName == str)
			{
				return true;
			}
		}
		return false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}
}