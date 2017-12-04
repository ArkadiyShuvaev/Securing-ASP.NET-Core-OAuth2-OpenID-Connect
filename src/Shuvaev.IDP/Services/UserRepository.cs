using System;
using System.Collections.Generic;
using System.Linq;
using IdentityModel;
using Microsoft.EntityFrameworkCore;
using Shuvaev.IDP.Entities;
using Shuvaev.IDP.Contextes;

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
			return GetUserBySubjIdImpl(subjectId);
		}

		public User GetUserByEmail(string email)
		{
			if (email == null) throw new ArgumentNullException(nameof(email));

			return _context.Users.FirstOrDefault(
				u => u.Claims.Any(
					c => c.ClaimType == JwtClaimTypes.Email && string.Equals(c.ClaimValue, email, 
				                                                            StringComparison.OrdinalIgnoreCase)));
		}

		public User GetUserByProvider(string loginProvider, string providerKey)
		{
			return _context.Users.FirstOrDefault(
				u => u.Logins.Any(l => string.Equals(l.LoginProvider, loginProvider, StringComparison.OrdinalIgnoreCase) &&
				                       string.Equals(l.ProviderKey, providerKey, StringComparison.OrdinalIgnoreCase)));
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

		public bool AddUser(User user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));

			user.SubjectId = Guid.NewGuid();
			_context.Users.Add(user);

			return _context.SaveChanges() > 0;
		}

		public bool AddUserLogin(string subjectId, string loginProvider, string providerKey)
		{
			var user = GetUserBySubjIdImpl(subjectId);
			user.Logins.Add(new UserLogin
			{
				LoginProvider = loginProvider,
				ProviderKey = providerKey
			});

			return _context.SaveChanges() > 0;
		}

		public void AddUserClaim(string subjectId, string claimType, string claimValue)
		{
			throw new System.NotImplementedException();
		}

		private User GetUserBySubjIdImpl(string subjectId)
		{
			var user = _context.Users.Include("Claims").Include("Logins").FirstOrDefault(
				u => string.Compare(u.SubjectId.ToString(), subjectId, true) == 0);

			return user;
		}

		private User GetUserByUserNameImpl(string username)
		{
			return _context.Users.Include("Claims").Include("Logins").FirstOrDefault(u => string.Compare(username, u.Username, true) == 0);
		}
	}
}