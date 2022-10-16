using TestSHauss.Model;

namespace TestSHauss.Service
{
	public static class FriendService
	{
		public static string AcceptInvitation(User user, string invitingUserId)
		{
			List<string> invites = Database.Invites.LoadByUserId(user.id);
			if (!invites.Contains(invitingUserId))
			{
				return "Invalid invitation";
			}

			invites.Remove(invitingUserId);
			user.friendsIds.Add(invitingUserId);
			User otherUser = Database.Users.LoadById(invitingUserId);
			otherUser.friendsIds.Add(user.id);
			return "Friend added";
		}

		public static string DeclineInvitation(User user, string invitingUserId)
		{
			List<string> invites = Database.Invites.LoadByUserId(user.id);
			if (!invites.Contains(invitingUserId))
			{
				return "Invalid invitation";
			}

			invites.Remove(invitingUserId);
			return "Invitation declined"; 
		}

		public static string SendInvitation(User authentifiedUser, string otherUserId)
		{
			List<string> otherUserInvites = Database.Invites.LoadByUserId(otherUserId);
			if (otherUserInvites.Contains(authentifiedUser.id))
			{
				return "Invitation already sent";
			}

			Database.Invites.SendInvitation(authentifiedUser.id, otherUserId);
			return "Invitation sent";
		}
	}
}
