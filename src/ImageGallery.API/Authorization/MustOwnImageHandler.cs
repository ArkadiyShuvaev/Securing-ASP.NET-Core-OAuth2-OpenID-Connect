using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ImageGallery.API.Authorization
{
	public class MustOwnImageHandler : AuthorizationHandler<MustOwnImageRequirement>
	{
		private readonly IGalleryRepository _repository;

		public MustOwnImageHandler(IGalleryRepository repository)
		{
			_repository = repository;
		}
		protected override Task HandleRequirementAsync(
			AuthorizationHandlerContext context, MustOwnImageRequirement requirement)
		{
			var filterContext = context.Resource as AuthorizationFilterContext;
			if (filterContext == null)
			{
				context.Fail();
				return Task.FromResult(0);
			}

			var imageId = filterContext.RouteData.Values["id"].ToString();
			Guid imageIdAsGuid;
			if (!Guid.TryParse(imageId, out imageIdAsGuid))
			{
				context.Fail();
				return Task.FromResult(0);
			}

			var ownerId = context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			if (!_repository.IsImageOwner(imageIdAsGuid, ownerId))
			{
				context.Fail();
				return Task.FromResult(0);
				;
			}

			context.Succeed(requirement);
			return Task.FromResult(0);
		}
	}
}
