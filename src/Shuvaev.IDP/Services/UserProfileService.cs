using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace Shuvaev.IDP.Services
{
    public class UserProfileService : IProfileService
	{
		private readonly IUserRepository _repository;

		public UserProfileService(IUserRepository repository)
		{
			_repository = repository;
		}
		public Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			var subjectId = context.Subject.Identity.GetSubjectId();
			var claims = _repository.GetUserClaimsBySubjectId(subjectId);

			var issuedClaims = claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
			context.IssuedClaims = issuedClaims;

			return Task.CompletedTask;
		}

		public Task IsActiveAsync(IsActiveContext context)
		{
			var subjectId = context.Subject.Identity.GetSubjectId();
			context.IsActive = _repository.IsUserActive(subjectId);

			return Task.FromResult(0);

		}
	}
}
