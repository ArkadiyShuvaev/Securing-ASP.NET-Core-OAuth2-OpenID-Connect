using System;
using System.Collections.Generic;
using System.Linq;
using Shuvaev.IDP.Entities;

namespace Shuvaev.IDP.Services
{
	public class UserRepository : IUserRepository
	{
		private readonly UserContext _context;

		public UserRepository(UserContext context)
		{
			_context = context;
		}
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
			if (subjectId == null) throw new ArgumentNullException(nameof(subjectId));

			var user = GetUser(subjectId);

			return user.Claims;
		}

		private User GetUser(string subjectId)
		{
			var user = _context.Users.FirstOrDefault(
				u => string.Equals(u.SubjectId.ToString(), subjectId, StringComparison.OrdinalIgnoreCase));
			
			return user;
		}

		public bool AreUserCredentialsValid(string username, string password)
		{
			throw new System.NotImplementedException();
		}

		public bool IsUserActive(string subjectId)
		{
			if (subjectId == null) throw new ArgumentNullException(nameof(subjectId));

			var user = GetUser(subjectId);

			return user.IsActive;
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