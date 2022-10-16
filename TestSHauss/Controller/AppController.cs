using HttpListenerExample;
using TestSHauss.Model;
using TestSHauss.Service;

namespace TestSHauss.Controller
{
	public static class AppController
	{
		private const string SHUTDOWN_PATH = "/shutdown";
		private const string LOGIN_PATH = "/login-or-create";
		private const string LOGOUT = "/logout";
		private const string BUY = "/buy";
		private const string ACCEPT_INVITATION = "/accept-invitation";
		private const string DECLINE_INVITATION = "/decline-invitation";
		private const string SEARCH_USER = "/search-user";
		private const string INVITE = "/invite";

		public static User DispatchRequestAction(string path, Dictionary<string, string> formData, string authToken)
		{
			User authentifiedUser = null;
			if (!string.IsNullOrEmpty(authToken))
			{
				authentifiedUser = LoadAuthentifiedUser(authToken);
				if (authentifiedUser == null)
				{
					return null;
				}
			}

			switch (path)
			{
				case SHUTDOWN_PATH:
					Console.WriteLine("Shutdown requested");
					HttpServer.runServer = false;
					break;
				case LOGIN_PATH:
					string login = formData["login"];
					string password = formData["password"];
					HttpServer.feedbackString = LoginService.LoginOrCreate(login, password, out authentifiedUser);
					if (authentifiedUser != null)
					{
						AuthenticationService.GenerateAuthTokenForUser(authentifiedUser);
					}
					break;
				case BUY:
					string itemId = formData["itemId"];
					HttpServer.feedbackString = ShopService.Buy(authentifiedUser, int.Parse(itemId));
					break;
				case ACCEPT_INVITATION:
					string userId = formData["userId"];
					HttpServer.feedbackString = FriendService.AcceptInvitation(authentifiedUser, userId);
					break;
				case DECLINE_INVITATION:
					userId = formData["userId"];
					HttpServer.feedbackString = FriendService.DeclineInvitation(authentifiedUser, userId);
					break;
				case SEARCH_USER:
					string nickname = formData["nickname"];
					List<User> users = Database.Users.LoadByNickname(nickname);
					HttpServer.DisplayResult(users, authentifiedUser);
					break;
				case INVITE:
					userId = formData["userId"];
					HttpServer.feedbackString = FriendService.SendInvitation(authentifiedUser, userId);
					break;
				case LOGOUT:
					AuthenticationService.Logout(authToken);
					HttpServer.feedbackString = string.Empty;
					authentifiedUser = null;
					break;
				default:
					Console.WriteLine($"Unknown paht: {path}");
					break;
			}
			return authentifiedUser;
		}

		private static User LoadAuthentifiedUser(string authToken)
		{
			string userId = AuthenticationService.TokenToUserId(authToken);
			if (string.IsNullOrEmpty(userId))
			{
				HttpServer.feedbackString = "Not authentified!";
				return null;
			}
			return Database.Users.LoadById(userId);
		}
	}
}
