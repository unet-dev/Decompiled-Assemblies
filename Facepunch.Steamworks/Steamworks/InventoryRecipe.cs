using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Steamworks
{
	public struct InventoryRecipe : IEquatable<InventoryRecipe>
	{
		public InventoryDef Result;

		public InventoryRecipe.Ingredient[] Ingredients;

		public string Source;

		internal bool ContainsIngredient(InventoryDef inventoryDef)
		{
			bool flag = this.Ingredients.Any<InventoryRecipe.Ingredient>((InventoryRecipe.Ingredient x) => x.DefinitionId == inventoryDef.Id);
			return flag;
		}

		public override bool Equals(object p)
		{
			return this.Equals((InventoryRecipe)p);
		}

		public bool Equals(InventoryRecipe p)
		{
			return p.GetHashCode() == this.GetHashCode();
		}

		internal static InventoryRecipe FromString(string part, InventoryDef Result)
		{
			InventoryRecipe inventoryRecipe = new InventoryRecipe()
			{
				Result = Result,
				Source = part
			};
			InventoryRecipe array = inventoryRecipe;
			array.Ingredients = (
				from x in part.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				select InventoryRecipe.Ingredient.FromString(x) into x
				where x.DefinitionId != 0
				select x).ToArray<InventoryRecipe.Ingredient>();
			return array;
		}

		public override int GetHashCode()
		{
			return this.Source.GetHashCode();
		}

		public static bool operator ==(InventoryRecipe a, InventoryRecipe b)
		{
			return a.GetHashCode() == b.GetHashCode();
		}

		public static bool operator !=(InventoryRecipe a, InventoryRecipe b)
		{
			return a.GetHashCode() != b.GetHashCode();
		}

		public struct Ingredient
		{
			public int DefinitionId;

			public InventoryDef Definition;

			public int Count;

			internal static InventoryRecipe.Ingredient FromString(string part)
			{
				InventoryRecipe.Ingredient ingredient;
				InventoryRecipe.Ingredient ingredient1 = new InventoryRecipe.Ingredient()
				{
					Count = 1
				};
				try
				{
					if (part.Contains("x"))
					{
						int num = part.IndexOf('x');
						int num1 = 0;
						if (Int32.TryParse(part.Substring(num + 1), out num1))
						{
							ingredient1.Count = num1;
						}
						part = part.Substring(0, num);
					}
					ingredient1.DefinitionId = Int32.Parse(part);
					ingredient1.Definition = SteamInventory.FindDefinition(ingredient1.DefinitionId);
				}
				catch (Exception exception)
				{
					ingredient = ingredient1;
					return ingredient;
				}
				ingredient = ingredient1;
				return ingredient;
			}
		}
	}
}