using TestSHauss.Model;

namespace TestSHauss.Service
{
	public static class LoginService
	{
		public static string LoginOrCreate(string login, string password, out User user)
		{
			user = Database.Users.LoadByLogin(login);
			if (user == null)
			{
				Console.WriteLine("login not found, creating new user");
				user = CreateUser(login, password);
				return "Account created!";
			}

			if (user.password != password)
			{
				user = null;
				return "Wrong Password!";
			}

			if (AuthenticationService.IsAuthentified(user))
			{
				user = null;
				return "User already logged in";
			}

			return "Logged in";
		}

		private static User CreateUser(string login, string password)
		{
			return Database.Users.SaveNew(login, password);
		}
	}
}
