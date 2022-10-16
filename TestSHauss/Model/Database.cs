namespace TestSHauss.Model
{
	public static class Database
	{
		public static class Users
		{
			private const int BASE_MONEY = 1000;
			private static readonly List<string> DEFAULT_NICKNAMES = new List<string>()
			{
				"John",
				"Falcon",
				"Mimo",
				"Pacha",
				"Lisa",
				"Vera",
				"Shay"
			};

			private static Dictionary<string, User> allUsers = new Dictionary<string, User>()
			{
				// Add a default user for testing
				{ "default_user_id", new User() { id = "default_user_id", login = "tester", money = 1000, nickname = "Marcellus", password = "tester"} },
			};

			public static User LoadByLogin(string login)
			{
				return allUsers.Values.Where((user) => user.login == login).FirstOrDefault();
			}

			public static User LoadById(string id)
			{
				if (allUsers.TryGetValue(id, out User foundUser))
				{
					return foundUser;
				}
				return null;
			}

			public static User SaveNew(string login, string password)
			{
				User user = new User()
				{
					login = login,
					password = password,
					id = Guid.NewGuid().ToString(),
					money = BASE_MONEY,
					nickname = DEFAULT_NICKNAMES[new Random().Next(DEFAULT_NICKNAMES.Count)]
				};

				allUsers.Add(user.id, user);
				return user;
			}

			public static List<User> LoadByIds(List<string> usersIds)
			{
				List<User> loadedUsers = new List<User>();
				foreach(string userId in usersIds)
				{
					if (allUsers.TryGetValue(userId, out User foundUser))
					{
						loadedUsers.Add(foundUser);
					}
					else
					{
						Console.WriteLine($"User with id {userId} not found");
					}
				}
				return loadedUsers;
			}

			public static List<User> LoadByNickname(string nickname)
			{
				return allUsers.Values.Where(x => x.nickname.ToLower() == nickname.ToLower()).ToList();
			}
		}

		public static class Items
		{
			private static Dictionary<int, Item> allItems = new Dictionary<int, Item>()
			{
				{ 1, new Item() { id = 1, name = "Sword", price = 40 } },
				{ 2, new Item() { id = 2, name = "Shield", price = 60 } },
				{ 3, new Item() { id = 3, name = "Helm", price = 35 } },
				{ 4, new Item() { id = 4, name = "Armor", price = 140 } },
				{ 5, new Item() { id = 5, name = "Apple", price = 5, canStack = true } },
				{ 6, new Item() { id = 6, name = "Super Power", price = 500, canStack = true } },
				{ 7, new Item() { id = 7, name = "The Thing", price = 10000}},
			};

			public static Item Load(int itemId)
			{
				if (allItems.TryGetValue(itemId, out Item foundItem))
				{
					return foundItem;
				}
				return null;
			}

			public static List<Item> LoadAll()
			{
				return allItems.Values.ToList();
			}

			public static List<Item> LoadByIds(List<int> itemsIds)
			{
				List<Item> loadedItems = new List<Item>();
				foreach (int itemId in itemsIds)
				{
					if (allItems.TryGetValue(itemId, out Item foundItem))
					{
						loadedItems.Add(foundItem);
					}
					else
					{
						Console.WriteLine($"Item with id {itemId} not found");
					}
				}
				return loadedItems;
			}
		}

		public static class Invites
		{
			private static Dictionary<string, List<string>> allInvites = new Dictionary<string, List<string>>();

			public static void SendInvitation(string fromUserId, string toUserId)
			{
				if (!allInvites.TryGetValue(toUserId, out List<string> invitations))
				{
					invitations = new List<string>();
					allInvites.Add(toUserId, invitations);
				}
				invitations.Add(fromUserId);
			}

			public static List<string> LoadByUserId(string userId)
			{
				if (allInvites.TryGetValue(userId, out List<string> invitations))
				{
					return invitations;
				}
				return new List<string>();
			}
		}
	}
}
