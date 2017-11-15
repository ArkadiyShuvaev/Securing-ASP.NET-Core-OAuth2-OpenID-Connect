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
			return GetUserByUserNameImpl(username);
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

			var user = GetUserBySubjIdImpl(subjectId);

			return user.Claims;
		}

		public bool AreUserCredentialsValid(string username, string password)
		{
			if (username == null) throw new ArgumentNullException(nameof(username));
			if (password == null) throw new ArgumentNullException(nameof(password));

			var user = GetUserByUserNameImpl(username);

			return user.Password == password;
		}
		
		public bool IsUserActive(string subjectId)
		{
			if (subjectId == null) throw new ArgumentNullException(nameof(subjectId));

			var user = GetUserBySubjIdImpl(subjectId);

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

		private User GetUserBySubjIdImpl(string subjectId)
		{
			var user = _context.Users.FirstOrDefault(
				u => string.Compare(u.SubjectId.ToString(), subjectId, true) == 0);

			return user;
		}

		private User GetUserByUserNameImpl(string username)
		{
			return _context.Users.FirstOrDefault(u => string.Compare(username, u.Username, true) == 0);
		}
	}
}