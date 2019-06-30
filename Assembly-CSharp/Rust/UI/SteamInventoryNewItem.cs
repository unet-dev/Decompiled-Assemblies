using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Rust.UI
{
	public class SteamInventoryNewItem : MonoBehaviour
	{
		public SteamInventoryNewItem()
		{
		}

		public async Task Open(InventoryItem item)
		{
			base.gameObject.SetActive(true);
			base.GetComponentInChildren<SteamInventoryItem>().Setup(item);
			while (this && base.gameObject.activeSelf)
			{
				await Task.Delay(100);
			}
		}
	}
}