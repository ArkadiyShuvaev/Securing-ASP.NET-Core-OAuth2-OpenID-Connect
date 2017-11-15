using System.Collections.Generic;
using Shuvaev.IDP.Entities;

namespace Shuvaev.IDP.Services
{
	public class UserRepository : IUserRepository
	{
		public User GetUserByUsername(string username)
		{
			throw new System.NotImplementedException();
		}

		public User GetUserBySubjectId(string subjectId)
		{
			throw new System.NotImplementedException();
		}

		public User GetUserByEmail(string email)
		{
			throw new System.NotImplementedException();
		}

		public User GetUserByProvider(string loginProvider, string providerKey)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<UserLogin> GetUserLoginsBySubjectId(string subjectId)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<UserClaim> GetUserClaimsBySubjectId(string subjectId)
		{
			throw new System.NotImplementedException();
		}

		public bool AreUserCredentialsValid(string username, string password)
		{
			throw new System.NotImplementedException();
		}

		public bool IsUserActive(string subjectId)
		{
			throw new System.NotImplementedException();
		}

		public void AddUser(User user)
		{
			throw new System.NotImplementedException();
		}

		public void AddUserLogin(string subjectId, string loginProvider, string providerKey)
		{
			throw new System.NotImplementedException();
		}

		public void AddUserClaim(string subjectId, string claimType, string claimValue)
		{
			throw new System.NotImplementedException();
		}
	}
}