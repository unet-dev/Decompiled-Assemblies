using Facepunch.Steamworks;
using Rust;
using Rust.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.Workshop.Game
{
	public class WorkshopCraftInfoModal : MonoBehaviour
	{
		public Text Name;

		public Text Requirements;

		public Text Label;

		public HttpImage IconImage;

		public GameObject ErrorPanel;

		public Text ErrorText;

		public GameObject CraftButton;

		public GameObject ProgressPanel;

		private Inventory.Recipe Recipe;

		public WorkshopCraftInfoModal()
		{
		}

		public static string BBCodeToUnity(string x)
		{
			x = x.Replace("[", "<");
			x = x.Replace("]", ">");
			return x;
		}

		public void Close()
		{
			base.gameObject.SetActive(false);
		}

		private IEnumerator CraftAnimation(Inventory.Result result)
		{
			WorkshopCraftInfoModal workshopCraftInfoModal = null;
			while (result.IsPending)
			{
				yield return null;
			}
			workshopCraftInfoModal.Close();
		}

		public void DoCraft()
		{
			List<Inventory.Item.Amount> amounts = new List<Inventory.Item.Amount>();
			Inventory.Recipe.Ingredient[] ingredients = this.Recipe.Ingredients;
			for (int i = 0; i < (int)ingredients.Length; i++)
			{
				Inventory.Recipe.Ingredient ingredient = ingredients[i];
				IEnumerable<Inventory.Item> items = (
					from x in Global.SteamClient.Inventory.Items
					where x.DefinitionId == ingredient.DefinitionId
					select x).Take<Inventory.Item>(ingredient.Count);
				if (items.Sum<Inventory.Item>((Inventory.Item x) => x.Quantity) < ingredient.Count)
				{
					UnityEngine.Debug.LogWarning(string.Concat("Not enough of ", ingredient.DefinitionId));
					this.Close();
					return;
				}
				int num = 0;
				foreach (Inventory.Item item in items)
				{
					int count = ingredient.Count - num;
					int quantity = item.Quantity;
					if (quantity > count)
					{
						quantity = count;
					}
					Inventory.Item.Amount amount = new Inventory.Item.Amount()
					{
						Item = item,
						Quantity = quantity
					};
					amounts.Add(amount);
					num += quantity;
					if (num < ingredient.Count)
					{
						continue;
					}
					goto Label0;
				}
			Label0:
			}
			this.ProgressPanel.SetActive(true);
			Inventory.Result result = Global.SteamClient.Inventory.CraftItem(amounts.ToArray(), this.Recipe.Result);
			base.StartCoroutine(this.CraftAnimation(result));
		}

		public void Open(int ItemDefinition)
		{
			if (Global.SteamClient.Inventory.Definitions == null)
			{
				UnityEngine.Debug.LogWarning("WorkshopCraftInfoModal failed: Definitions = null");
				return;
			}
			Inventory.Definition definition = Global.SteamClient.Inventory.FindDefinition(ItemDefinition);
			if (definition == null)
			{
				UnityEngine.Debug.LogWarning("WorkshopCraftInfoModal failed: def = null");
				return;
			}
			if (definition.Recipes == null)
			{
				UnityEngine.Debug.LogWarning("WorkshopCraftInfoModal failed: def.Recipes = null");
				return;
			}
			if (Global.SteamClient.Inventory.Items == null)
			{
				UnityEngine.Debug.LogWarning("WorkshopCraftInfoModal failed: Inventory.Items = null");
				return;
			}
			this.Recipe = definition.Recipes.FirstOrDefault<Inventory.Recipe>();
			base.gameObject.SetActive(true);
			this.ProgressPanel.SetActive(false);
			this.ErrorPanel.SetActive(false);
			this.CraftButton.SetActive(true);
			this.Name.text = definition.Name;
			this.Label.text = WorkshopCraftInfoModal.BBCodeToUnity(definition.Description);
			string[] array = (
				from x in (IEnumerable<Inventory.Recipe.Ingredient>)this.Recipe.Ingredients
				select string.Format("{0} x {1}", x.Count, x.Definition.Name)).ToArray<string>();
			this.Requirements.text = string.Join(", ", array);
			Inventory.Recipe.Ingredient[] ingredients = this.Recipe.Ingredients;
			for (int i = 0; i < (int)ingredients.Length; i++)
			{
				Inventory.Recipe.Ingredient ingredient = ingredients[i];
				if ((
					from x in Global.SteamClient.Inventory.Items
					where x.DefinitionId == ingredient.DefinitionId
					select x).Sum<Inventory.Item>((Inventory.Item x) => x.Quantity) < ingredient.Count)
				{
					this.CraftButton.SetActive(false);
					this.ErrorPanel.SetActive(true);
					this.ErrorText.text = string.Format("NEED {0} x {1}", ingredient.Count, ingredient.Definition.Name);
				}
			}
			this.IconImage.Load(definition.IconLargeUrl);
		}
	}
}