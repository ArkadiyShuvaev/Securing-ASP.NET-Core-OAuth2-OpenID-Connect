using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shuvaev.IDP.Entities;
using Shuvaev.IDP.Services;

namespace Shuvaev.IDP.Controllers.UserRegistration
{
	public class UserRegistrationController : Controller
	{
		private readonly IUserRepository _marvinUserRepository;
		private readonly IIdentityServerInteractionService _interaction;

		public UserRegistrationController(IUserRepository marvinUserRepository,
			IIdentityServerInteractionService interaction)
		{
			_marvinUserRepository = marvinUserRepository;
			_interaction = interaction;
		}


		[HttpGet]
		public IActionResult RegisterUser(string returnUrl)
		{
			return View(new RegisterUserViewModel
			{
				ReturnUrl = returnUrl
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}


			var user = AutoMapper.Mapper.Map<User>(model);

			user.IsActive = true;
			user.Claims = new List<UserClaim>
			{
				new UserClaim {ClaimType = "address", ClaimValue = model.Address},
				new UserClaim {ClaimType = "country", ClaimValue = model.Country},
				new UserClaim {ClaimType = "family_name", ClaimValue = model.LastName},
				new UserClaim {ClaimType = "given_name", ClaimValue = model.FirstName},
				new UserClaim {ClaimType = "email", ClaimValue = model.Email},
				new UserClaim {ClaimType = "subscriptionlevel", ClaimValue = "FreeUser"}
			};

				

			if (!_marvinUserRepository.AddUser(user))
			{
				throw new Exception("Creating a user failed.");
			}

			await HttpContext.SignInAsync(user.SubjectId.ToString(), user.Username);

			// continue with the flow     
			if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
			{
				return Redirect(model.ReturnUrl);
			}

			return Redirect("~/");

			// ModelState invalid, return the view with the passed-in model
			// so changes can be made
		}

	}
}
