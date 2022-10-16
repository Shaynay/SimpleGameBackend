namespace TestSHauss.Model
{
	public class User
	{
		public string nickname;
		public string id;
		public string login;
		public string password;
		public int money;
		public List<string> friendsIds = new List<string>();
		public Dictionary<int, int> itemsWithQuantity = new Dictionary<int, int>();

		public void AddItem(Item item)
		{
			if (itemsWithQuantity.TryGetValue(item.id, out int quantity))
			{
				itemsWithQuantity[item.id] = quantity + 1;
			}
			else
			{
				itemsWithQuantity.Add(item.id, 1);
			}
		}
	}
}
