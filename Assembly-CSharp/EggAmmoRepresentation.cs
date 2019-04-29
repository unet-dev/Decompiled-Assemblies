using System;
using UnityEngine;

public class EggAmmoRepresentation : MonoBehaviour, IInventoryChanged, IClientComponent
{
	public GameObject[] eggAmmo;

	public EggAmmoRepresentation()
	{
	}
}