using TestSHauss.Model;

namespace TestSHauss.Service
{
	public static class ShopService
	{
		public static string Buy(User user, int itemId)
		{
			Item item = Database.Items.Load(itemId);
			if (item == null)
			{
				return "Item not found!";
			}

			if (user.money < item.price)
			{
				return "Not enough money!";
			}

			if (user.itemsWithQuantity.TryGetValue(itemId, out int quantity) && !item.canStack)
			{
				if (quantity > 0)
				{
					return "Cannot have more than one!";
				}
			}
			user.money -= item.price;
			user.AddItem(item);
			return "Congrats!";
		}
	}
}
