using TestSHauss.Model;

namespace TestSHauss.Service
{
	public static class AuthenticationService
	{
		private static Dictionary<string, string> authTokenToUserId = new Dictionary<string, string>();
		private static Dictionary<string, string> userIdToToken = new Dictionary<string, string>();

		public static string GenerateAuthTokenForUser(User user)
		{
			string token = Guid.NewGuid().ToString();
			authTokenToUserId.Add(token, user.id);
			userIdToToken.Add(user.id, token);
			return token;
		}

		public static void Logout(string token)
		{
			string userId = authTokenToUserId[token];
			userIdToToken.Remove(userId);
			authTokenToUserId.Remove(token);
		}

		public static string TokenToUserId(string token)
		{
			return authTokenToUserId[token];
		}

		public static string LoadToken(User user)
		{
			if (user == null || userIdToToken.TryGetValue(user.id, out string token) == false)
			{
				return string.Empty;
			}
			return token;
		}

		public static bool IsAuthentified(User user)
		{
			return user != null && userIdToToken.ContainsKey(user.id);
		}
	}
}
