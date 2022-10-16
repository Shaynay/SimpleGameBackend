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
			User authentifiedUser;
			switch (path)
			{
				case SHUTDOWN_PATH:
					Console.WriteLine("Shutdown requested");
					HttpServer.runServer = false;
					return null;
				case LOGIN_PATH:
					string login = formData["login"];
					string password = formData["password"];
					HttpServer.feedbackString = LoginService.LoginOrCreate(login, password, out authentifiedUser);
					if (authentifiedUser != null)
					{
						if (AuthenticationService.IsAuthentified(authentifiedUser))
						{
							HttpServer.feedbackString = "User already logged in";
							return null;
						}
						AuthenticationService.GenerateAuthTokenForUser(authentifiedUser);
					}
					return authentifiedUser;
				case BUY:
					authentifiedUser = LoadAuthentifiedUser(authToken);
					if (authentifiedUser == null)
						return null;
					string itemId = formData["itemId"];
					HttpServer.feedbackString = ShopService.Buy(authentifiedUser, int.Parse(itemId));
					return authentifiedUser;
				case ACCEPT_INVITATION:
					authentifiedUser = LoadAuthentifiedUser(authToken);
					if (authToken == null)
						return null;
					string userId = formData["userId"];
					HttpServer.feedbackString = FriendService.AcceptInvitation(authentifiedUser, userId);
					return authentifiedUser;
				case DECLINE_INVITATION:
					authentifiedUser = LoadAuthentifiedUser(authToken);
					if (authToken == null)
						return null;
					userId = formData["userId"];
					HttpServer.feedbackString = FriendService.DeclineInvitation(authentifiedUser, userId);
					return authentifiedUser;
				case SEARCH_USER:
					authentifiedUser = LoadAuthentifiedUser(authToken);
					if (authToken == null)
						return null;
					string nickname = formData["nickname"];
					List<User> users = Database.Users.LoadByNickname(nickname);
					HttpServer.DisplayResult(users, authentifiedUser);
					return authentifiedUser;
				case INVITE:
					authentifiedUser = LoadAuthentifiedUser(authToken);
					if (authToken == null)
						return null;
					userId = formData["userId"];
					HttpServer.feedbackString = FriendService.SendInvitation(authentifiedUser, userId);
					return authentifiedUser;
				case LOGOUT:
					AuthenticationService.Logout(authToken);
					HttpServer.feedbackString = string.Empty;
					return null;
				default:
					Console.WriteLine($"Unknown paht: {path}");
					return null;
			}
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
