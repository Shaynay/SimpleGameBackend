using System.Text;
using TestSHauss.Model;
using TestSHauss.Service;

namespace TestSHauss.Controller
{
	public static class HtmlStrings
	{
		public static string UserHtmlString(User user)
		{
			List<User> friends = Database.Users.LoadByIds(user.friendsIds);
			List<Item> ownedItems = Database.Items.LoadByIds(user.itemsWithQuantity.Keys.ToList());
			List<string> invites = Database.Invites.LoadByUserId(user.id);
			List<User> invitingUsers = Database.Users.LoadByIds(invites);
			string authToken = AuthenticationService.LoadToken(user);

			StringBuilder sb = new StringBuilder();
			sb.Append("<h1>Welcome ").Append(user.nickname).Append("</h1>").AppendLine();

			// Invientory
			sb.Append("<h2>Iventory:</h2>").AppendLine()
				.Append("<ul>").AppendLine()
					.Append("<li>items:")
					.Append("<ul>");
			foreach (Item item in ownedItems)
			{
				sb.Append("<li>").Append(item.name);
				if (item.canStack)
				{
					sb.Append(" (").Append(user.itemsWithQuantity[item.id]).Append(")");
				}
			}
			sb.Append("</ul>");
			sb.Append("<li>Money: ").Append(user.money).Append("</li>").AppendLine()
				.Append("</ul>").AppendLine();

			// Friends
			sb.Append("<h2>Friends</h2>").AppendLine()
				.Append("<ul>").AppendLine();
			foreach (User friend in friends)
			{
				sb.Append("<li>").Append(friend.nickname).Append("</li>").AppendLine();
			}
			sb.Append("</ul>");

			// Invites
			sb.Append("<h2>Invites</h2>").AppendLine()
				.Append("<ul>").AppendLine();
			foreach (User invitingUser in invitingUsers)
			{
				sb.Append("<li>")
					.Append(invitingUser.nickname)
					.Append("<form method=\"post\" action=\"accept-invitation\">")
					.Append("<input type=\"submit\" value=\"Accept\"/>")
					.Append("<input type=\"hidden\" name=\"userId\" value=\"").Append(invitingUser.id).Append("\"/>")
					.Append("<input type=\"hidden\" name=\"authToken\" value=\"").Append(authToken).Append("\"/>")
					.Append("</form>")
					.Append("<form method=\"post\" action=\"decline-invitation\">")
					.Append("<input type=\"submit\" value=\"Decline\"/>")
					.Append("<input type=\"hidden\" name=\"userId\" value=\"").Append(invitingUser.id).Append("\"/>")
					.Append("<input type=\"hidden\" name=\"authToken\" value=\"").Append(authToken).Append("\"/>")
					.Append("</form>")
					.Append("</li>");
			}
			sb.Append("</ul>");

			return sb.ToString();
		}

		public static string ShopHtmlString()
		{
			List<Item> items = Database.Items.LoadAll();
			StringBuilder sb = new StringBuilder();
			foreach (Item item in items)
			{
				sb.Append("<li>[ID:").Append(item.id).Append("] ").Append(item.name).Append(" - Price: ").Append(item.price).Append("</li>");
			}
			return sb.ToString();
		}

		public static string SearchResultHtmlString(List<User> users, User authentifiedUser)
		{
			string authToken = AuthenticationService.LoadToken(authentifiedUser);

			StringBuilder sb = new StringBuilder();
			sb.Append("<ul>");
			int foundUsers = 0;
			foreach (User user in users)
			{
				if (user.id == authentifiedUser.id || authentifiedUser.friendsIds.Contains(user.id))
				{
					continue;
				}
				foundUsers++;

				sb.Append("<li>")
					.Append(user.nickname)
					.Append("<form method=\"post\" action=\"invite\">")
					.Append("<input type=\"submit\" value=\"Add Friend\"/>")
					.Append("<input type=\"hidden\" name=\"userId\" value=\"").Append(user.id).Append("\"/>")
					.Append("<input type=\"hidden\" name=\"authToken\" value=\"").Append(authToken).Append("\"/>")
					.Append("</form>")
					.Append("</li>");
			}
			if (foundUsers == 0)
			{
				sb.Clear();
				return $"No user found with that nickname";
			}
			sb.Append("</ul>");
			return sb.ToString();
		}
	}
}
